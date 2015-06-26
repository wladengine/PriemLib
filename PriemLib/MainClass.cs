﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.DirectoryServices.AccountManagement;


using BaseFormsLib;
using EducServLib;

namespace PriemLib
{    
    public static partial class MainClass
    {  
        //test
        public static Form mainform;
        private static DBPriem _bdc = null;
        private static DBPriem _bdcOnlineReadWrite = null;

        public static bool IsTestDB { get; set; }

        public static PriemType dbType;
        public static string connString;
        public static string connStringOnline;        
        
        public static string directory;
        public static string dirTemplates;
        public static string saveTempFolder;
        public static string userName;
       
        public static List<int> lstStudyLevelGroupId = new List<int>();
        public static int foreignCountryRussiaId;
        public static int countryRussiaId;
        public static int educSchoolId;
        public static int pasptypeRFId;
        public static int olympSpbguId;

        //GLOBALS
        //-----------------------------------------------------
        public static string sPriemYear;
        public static int iPriemYear;

        public static bool b1kCheckProtocolsEnabled;
        public static bool bMagCheckProtocolsEnabled;

        public static bool bMagImportApplicationsEnabled;
        //-----------------------------------------------------

        public static QueryBuilder qBuilder;

        //пользовательские настройки
        public static ConfigFile _config;
        
        private static DataRefreshHandler _drHandler;
        private static ProtocolRefreshHandler _prHandler;
        
        public static DBPriem Bdc
        {
            get { return _bdc; }          
        }
        public static DBPriem BdcOnlineReadWrite
        {
            get { return _bdcOnlineReadWrite; }
        }

        /// <summary>
        /// opens DataBase
        /// </summary>
        /// <param name="connectionString"></param>
        public static bool Init(Form mf)
        {
            try
            {
                _bdc = new DBPriem();
                _bdc.OpenDatabase(connString);

                //открываем коннект
                try
                {
                    _bdcOnlineReadWrite = new DBPriem();
                    _bdcOnlineReadWrite.OpenDatabase(DBConstants.CS_PriemONLINE_ReadWrite);
                }
                //если не открывается, то и пёс с ним
                catch { }


                mainform = mf;            
                userName = System.Environment.UserName;
                
                // database constant id
                using (PriemEntities context = new PriemEntities())
                {
                    //постоянный id страны Россия
                    countryRussiaId = 1;
                    //постоянный id страны Россия (Foreign)
                    foreignCountryRussiaId = 193;
                    //постоянный id типа уч.заведения Школа
                    educSchoolId = 1;
                    //постоянный id типа паспорта Паспорт РФ
                    pasptypeRFId = 1;
                    //постоянный id олимпиады СПбГУ
                    olympSpbguId = 3;


                    Dictionary<string, string> dicSettings = context.C_AppSettings
                        .Select(x => new { x.ParamKey, x.ParamValue }).ToList().ToDictionary(x => x.ParamKey, y => y.ParamValue);

                    sPriemYear = dicSettings.ContainsKey("PriemYear") ? dicSettings["PriemYear"] : DateTime.Now.Year.ToString();
                    iPriemYear = int.Parse(sPriemYear);

                    string tmp = dicSettings.ContainsKey("b1kCheckProtocolsEnabled") ? dicSettings["b1kCheckProtocolsEnabled"] : "False";
                    b1kCheckProtocolsEnabled = bool.Parse(tmp);

                    tmp = dicSettings.ContainsKey("bMagCheckProtocolsEnabled") ? dicSettings["bMagCheckProtocolsEnabled"] : "False";
                    bMagCheckProtocolsEnabled = bool.Parse(tmp);

                    tmp = dicSettings.ContainsKey("bMagImportApplicationsEnabled") ? dicSettings["bMagImportApplicationsEnabled"] : "False";
                    bMagImportApplicationsEnabled = bool.Parse(tmp);
                }

                switch (MainClass.dbType)
                {
                    case PriemType.Priem: { lstStudyLevelGroupId.Add(1); break; }
                    case PriemType.PriemMag: { lstStudyLevelGroupId.Add(2); break; }
                    case PriemType.PriemSPO: { lstStudyLevelGroupId.Add(3); break; }
                    case PriemType.PriemAspirant: { lstStudyLevelGroupId.Add(4); lstStudyLevelGroupId.Add(5); break; }
                    case PriemType.PriemForeigners: 
                        { 
                            lstStudyLevelGroupId.Add(1);
                            lstStudyLevelGroupId.Add(2);
                            lstStudyLevelGroupId.Add(3);
                            lstStudyLevelGroupId.Add(4);
                            lstStudyLevelGroupId.Add(5);
                            break;
                        }
                    default: { lstStudyLevelGroupId.Add(1); break; }
                }
                
              /* 
               %APPDATA%/Priem
              */
                directory = string.Format(@"{0}\Priem", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));               
                saveTempFolder = string.Format(@"{0}\DocTempFiles\", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                try
                {
                    // уточнить у Дениса, кто создавал эту папку!!!!! может будет ошибка
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    // Determine whether the directory exists.
                    if (!Directory.Exists(saveTempFolder))
                        Directory.CreateDirectory(saveTempFolder);
                }
                catch (Exception e)
                {
                    WinFormsServ.Error("Ошибка при создании временного/служебного каталога", e);
                }
                
                //взяли конфиг
                _config = GetConfig();

                return true;
            }
            catch(Exception e)
            {
                WinFormsServ.Error("Ошибка в MainClass.Init()", e);
                return false;
            }
        }

        public static string GetUserName()
        {
            return GetADUserName(System.Environment.UserName);
        }
        public static string GetADUserName(string userName)
        {
            try
            {
                var ADPrincipal = new PrincipalContext(ContextType.Domain);
                UserPrincipal user = UserPrincipal.FindByIdentity(ADPrincipal, userName);

                if (user != null)
                    return user.DisplayName + " (" + userName + ")";
            }
            catch { }

            return userName;
        }

        //прочитали конфиг
        private static ConfigFile GetConfig()
        {
            string configFile = "config.xml";
            return new ConfigFile(directory, configFile);
        }

        public static void DeleteTempFiles()
        {
            try
            {
                foreach (string file in Directory.GetFiles(saveTempFolder))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                    }
                }

                foreach (var file in new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).GetFiles())
                {
                    if (file.Name.Contains("DocTempFiles"))
                        File.Delete(file.FullName);
                }
            }
            catch (Exception exc) 
            {
                WinFormsServ.Error("Ошибка в MainClass.DeleteTempFiles()", exc);
            }
        }

        public static void SaveParameters()
        {
            try
            {
                if (MainClass.IsOwner() || MainClass.IsPasha())
                {
                    using (PriemEntities context = new PriemEntities())
                    {
                        context.SetApplicationValue("bMagCheckProtocolsEnabled", bMagCheckProtocolsEnabled.ToString());
                        context.SetApplicationValue("b1kCheckProtocolsEnabled", b1kCheckProtocolsEnabled.ToString());
                        context.SetApplicationValue("PriemYear", sPriemYear);
                        context.SetApplicationValue("bMagImportApplicationsEnabled", bMagImportApplicationsEnabled.ToString());
                    }
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при сохранении параметров приложения в MainClass.SaveParameters()", exc);
            }
        }

        public static string GetAbitNum(string abNum, string perNum)
        {
            return perNum + @"\"+ abNum;           
        }       
        public static string GetStringAbitNumber(string abitView)
        {
            return string.Format(" substring('000' + Convert(nvarchar(2), {0}.FacultyId), len('000' + Convert(nvarchar(2), {0}.FacultyId))-1, 2) + substring('000000' + Convert(nvarchar(5), {0}.RegNum), len('000000' + Convert(nvarchar(5), {0}.RegNum))-4, 5)", abitView);
        }

        public static bool CheckPersonBarcode(int? barcode)
        {
            if (barcode == null)
                return true;

            using (PriemEntities context = new PriemEntities())
            {
                int cnt = (from pers in context.Person
                           where pers.Barcode == barcode
                           select pers).Count();

                if (cnt > 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Возвращает факт, наличия в рабочей базе заявлений с данным номером коммита
        /// </summary>
        /// <param name="commitNumber"></param>
        /// <returns></returns>
        public static bool CheckExistenseAbitCommitNumberInWorkBase(int? commitNumber)
        {
            if (commitNumber == null)
                return true;

            using (PriemEntities context = new PriemEntities())
            {
                int cnt = (from abit in context.Abiturient
                           where abit.CommitNumber == commitNumber
                           select abit).Count();

                if (cnt == 0)
                    return false;
                else
                    return true;
            }
        }

        public static IEnumerable<qEntry> GetEntry(PriemEntities context)
        {
            try
            {
                IEnumerable<qEntry> entry = context.qEntry;
                return entry.Where(c => lstStudyLevelGroupId.Contains(c.StudyLevelGroupId));
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка в MainClass.GetEntry() ", exc);
                return null;
            }      
        }

        public static string GetStLevelFilter(string tableName)
        {
            return string.Format(" AND {1}.StudyLevelGroupId IN ({0}) ", Util.BuildStringWithCollection(lstStudyLevelGroupId), tableName);          
        }
    }
}

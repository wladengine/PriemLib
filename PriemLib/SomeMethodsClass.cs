﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.Objects;
using System.Data;
using System.Transactions;

using BDClassLib;
using EducServLib;

namespace PriemLib
{
    public static class SomeMethodsClass
    {
        public static void FillOlymps()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "CSV|*.csv";
            ofd.Multiselect = true;

            if (!(ofd.ShowDialog() == DialogResult.OK))
                return;
            foreach (string fileName in ofd.FileNames)
            {
                using (StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding(1251)))
                {
                    List<string[]> list = new List<string[]>();

                    while (!sr.EndOfStream)
                    {
                        string str = sr.ReadLine();
                        char[] split = new char[] { ';' };
                        string[] values = str.Split(split, 4);

                        list.Add(values);
                    }

                    int i = 0;
                    foreach (string[] val in list)
                    {
                        i++;

                        if (val.Length < 4)
                        {
                            MessageBox.Show("Строка " + i.ToString() + ": некорректный формат строки!");
                            continue;
                        }

                        string num = val[0].Trim();
                        int j;
                        int? numInt;
                        if(int.TryParse(num, out j))
                            numInt = j;
                        else
                            numInt = null;
                       
                        string name = val[1].Trim();
                        string olName = num + " - " + name;                       

                        using (PriemEntities context = new PriemEntities())
                        {
                            int? olNameId;
                            var ol = from qq in context.OlympName
                                       where qq.Name == olName
                                       select qq;

                            if (ol != null && ol.Count() > 0)
                                olNameId = (ol.First()).Id;
                            else
                            {
                                ObjectParameter olEnt = new ObjectParameter("id", typeof(Int32));
                                context.OlympName_Insert(olName, numInt, olEnt);
                                olNameId = (int)olEnt.Value;
                            }

                            string subjName = val[2].Trim();
                            
                            int? subjId;
                            var subj = from qq in context.OlympSubject
                                       where qq.Name == subjName
                                       select qq;

                            if (subj != null && subj.Count() > 0)
                                subjId = (subj.First()).Id;
                            else
                            {
                                ObjectParameter sbEnt = new ObjectParameter("id", typeof(Int32));
                                context.OlympSubject_Insert(subjName, "", sbEnt);
                                subjId = (int)sbEnt.Value;
                            } 

                            int? levId;
                            int level;
                            if (!int.TryParse(val[3].Trim(), out level))
                                levId = null;
                            else
                                levId = level;

                            int cnt = (from ob in context.OlympBook
                                       where ob.OlympTypeId == 4 && ob.OlympNameId == olNameId && ob.OlympSubjectId == subjId && ob.OlympLevelId == levId
                                       select ob).Count();

                            if (cnt > 0)
                                continue;
                            
                            ObjectParameter EntId = new ObjectParameter("id", typeof(Int32));
                            context.OlympBook_Insert(4, olNameId, subjId, levId, EntId);                        
                        }
                    }
                }
            }

            MessageBox.Show("Выполнено!");
        }

        // проверка на уникальность абитуриента
        public static bool CheckThreeAbits(PriemEntities context, Guid? personId, int? LicenseProgramId)
        {
            // если прием - то проверяем на три заявления
            if (MainClass.dbType != PriemType.Priem)
                return true;  

            //просто сосчитаем количество созданных активных конкурсов на человека
            var concurses = (from allab in context.Abiturient
                             where allab.PersonId == personId
                                 && allab.Entry.LicenseProgramId != LicenseProgramId
                                 //&& allab.ObrazProgramId != ObrazProgramId
                                 //&& (ProfileId == null ? allab.ProfileId != null : allab.ProfileId != ProfileId)
                             && allab.Entry.StudyLevel.LevelGroupId == MainClass.studyLevelGroupId
                             && allab.BackDoc != true
                             && !allab.IsGosLine //считать гослинию нам не надо, только не-гослинию
                             select new { allab.Entry.LicenseProgramId }).Distinct();
            return (concurses.Count() < 3);
        }

        // проверка на уникальность абитуриента
        private static bool CheckIdent(extPerson person)
        {
            using (PriemEntities context = new PriemEntities())
            {
                ObjectParameter boolPar = new ObjectParameter("result", typeof(bool));

                context.CheckPersonIdent(person.Surname, person.Name, person.SecondName, person.BirthDate, person.PassportSeries, person.PassportNumber,
                    person.AttestatRegion, person.AttestatSeries, person.AttestatNum, boolPar);
               
                return Convert.ToBoolean(boolPar.Value);
            }
        }

//        public static void ImportMagAbits()
//        {
//            List<string> lstPersons = new List<string>();
//            List<string> lstAbits = new List<string>();

//            LoadFromInet loadClass = new LoadFromInet();
//            DBPriem _bdcInet = loadClass.BDCInet;

//            using (PriemEntities context = new PriemEntities())
//            {
//                string _sQuery = @"SELECT DISTINCT qAbiturient.Id, qAbiturient.PersonId, qAbiturient.Barcode AS AbitBarcode, extPerson.Barcode AS PersonBarcode
//                              FROM qAbiturient INNER JOIN extPerson ON qAbiturient.PersonId = extPerson.Id WHERE StudyLevelId = 17 AND Enabled = 1 AND IsApprovedByComission = 1 AND IsImported = 0";
//                DataSet ds = _bdcInet.GetDataSet(_sQuery);

//                Guid? personId;
//                foreach (DataRow dr in ds.Tables[0].Rows)
//                {
//                    int? abitBarcode = (int?)dr["AbitBarcode"];
//                    if (MainClass.CheckExistenseAbitCommitNumberInWorkBase(abitBarcode))
//                        continue;
                    
                    
//                    int? persBarcode = (int?)dr["PersonBarcode"]; 
//                    if (!MainClass.CheckPersonBarcode(persBarcode))
//                    {
//                        personId = (from pers in context.extPerson
//                                    where pers.Barcode == persBarcode
//                                    select pers.Id).FirstOrDefault();
//                    }
//                    else
//                    {
//                        extPerson person = loadClass.GetPersonByBarcode(persBarcode.Value);

//                        if (!CheckIdent(person))
//                        {
//                            lstPersons.Add(persBarcode.ToString());
//                            continue;
//                        }

//                        ObjectParameter entId = new ObjectParameter("id", typeof(Guid));
//                        context.Person_insert(person.Barcode, person.Name, person.SecondName, person.Surname, person.BirthDate, person.BirthPlace, person.PassportTypeId,
//                            person.PassportSeries, person.PassportNumber, person.PassportAuthor, person.PassportDate, person.Sex, person.CountryId, person.NationalityId, 
//                            person.RegionId, person.Phone, person.Mobiles, person.Email, person.Code, person.City, person.Street, person.House, person.Korpus, 
//                            person.Flat, person.CodeReal, person.CityReal, person.StreetReal, person.HouseReal, person.KorpusReal, person.FlatReal, person.KladrCode,
//                            person.HostelAbit, person.HostelEduc, false, null, false, null, person.IsExcellent, 
//                            person.LanguageId, person.SchoolCity, person.SchoolTypeId, person.SchoolName, person.SchoolNum, person.SchoolExitYear, person.SchoolAVG, 
//                            person.CountryEducId, person.RegionEducId, person.IsEqual, person.AttestatRegion, person.AttestatSeries, person.AttestatNum, person.DiplomSeries, person.DiplomNum, 
//                            person.HighEducation, person.HEProfession, person.HEQualification, person.HEEntryYear, person.HEExitYear, person.HEStudyFormId, 
//                            person.HEWork, person.Stag, person.WorkPlace, person.MSVuz, person.MSCourse, person.MSStudyFormId, person.Privileges, person.PassportCode,
//                            person.PersonalCode, person.PersonInfo, person.ExtraInfo, person.ScienceWork, person.StartEnglish, person.EnglishMark, false, person.SNILS, entId);

//                        //_bdcInet.ExecuteQuery("UPDATE Person SET IsImported = 1 WHERE Person.Barcode = " + persBarcode);     
                        
//                        personId = (Guid)entId.Value;
//                    }

//                    qAbiturient abit = loadClass.GetAbitByBarcode(abitBarcode.Value);

//                    int cnt = (from en in context.qEntry
//                               where en.Id == abit.EntryId && !en.IsClosed
//                               select en).Count();

//                    if(cnt == 0)
//                    {
//                        lstAbits.Add(abitBarcode.ToString());
//                        continue;
//                    }

//                    ObjectParameter abEntId = new ObjectParameter("id", typeof(Guid));

//                    int competitionId;
//                    if (abit.StudyBasisId == 1)
//                        competitionId = 4;
//                    else
//                        competitionId = 3;

//                    context.Abiturient_Insert(personId, abit.EntryId, competitionId, false, false, false, false, null, abit.DocDate, DateTime.Now,
//                    false, false, null, null, null, null, abit.LanguageId, false,
//                    abit.Priority, abit.Barcode, abit.CommitId, abit.CommitNumber, abit.IsGosLine, abEntId);

//                    // _bdcInet.ExecuteQuery("UPDATE Application SET IsImported = 1 WHERE Application.Barcode = " + abitBarcode);
//                }
//            }        
//        }

        public static void SetBackDocForBudgetInEntryView()
        {
            if (!MainClass.IsPasha())
                return;

            using (PriemEntities context = new PriemEntities())
            {
                if (MessageBox.Show("Проставить 'Забрал документы' для платных заявлений, поступивших на бесплатное?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {                    
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromHours(1)))
                    {
                        var abits = from ev in context.extEntryView
                                    join ab in context.extAbit
                                    on ev.AbiturientId equals ab.Id
                                    where ab.StudyLevelGroupId == 1 && ab.StudyBasisId == 1 && !ab.BackDoc && ab.HasOriginals
                                    select ab;

                        foreach (extAbit abit in abits)
                        {
                            var abBackDocks = from ab in context.extAbit
                                              where ab.StudyLevelGroupId == abit.StudyLevelGroupId
                                              && ab.IsReduced == abit.IsReduced && ab.IsParallel == abit.IsParallel && ab.IsSecond == abit.IsSecond
                                              && ab.FacultyId == abit.FacultyId && ab.LicenseProgramId == abit.LicenseProgramId
                                              && ab.ObrazProgramId == abit.ObrazProgramId
                                              && (abit.ProfileId == null ? ab.ProfileId == null : ab.ProfileId == abit.ProfileId)
                                              && ab.StudyFormId == abit.StudyFormId
                                              && ab.StudyBasisId == 2
                                              select ab;

                            if (abBackDocks.Count() > 0)
                            {
                                foreach(extAbit abBack in abBackDocks)
                                {
                                    context.Abiturient_UpdateBackDoc(true, DateTime.Now, abBack.Id);
                                }
                            }
                        }


                        transaction.Complete();
                    }
                }
            }
        }

        public static void DeleteDogFromFirstWave()
        {
            if (!MainClass.IsPasha())
                return;

            using (PriemEntities context = new PriemEntities())
            {
                if (MessageBox.Show("Удалить платников, забравших документы или зачисленных на 1 курс, из FirstWave?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {                    
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromHours(1)))
                    {
                        context.FirstWave_DELETE_DogEntryBack();

                        transaction.Complete();
                    }
                }
            }
        }

        public static void SplitEntryViews()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    var entryViewNums =
                        (from exEV in context.extEntryView
                         join Ab in context.Abiturient on exEV.AbiturientId equals Ab.Id
                         where Ab.CompetitionId == 1
                         select exEV.Number).Intersect
                        (from exEV in context.extEntryView
                         join Ab in context.Abiturient on exEV.AbiturientId equals Ab.Id
                         where Ab.CompetitionId == 2
                         select exEV.Number);

                    foreach (string EVNumber in entryViewNums)
                    {

                        var abitIds =
                            (from extEV in context.extEntryView
                             join Ab in context.Abiturient on extEV.AbiturientId equals Ab.Id
                             where Ab.CompetitionId == 2 && extEV.Number == EVNumber
                             select Ab.Id).ToList();

                        var protocolInfo = context.extEntryView.Where(x => x.Number == EVNumber)
                            .Select(x => new
                        {
                            x.StudyLevelGroupId,
                            x.FacultyId,
                            x.LicenseProgramId,
                            x.StudyFormId,
                            x.StudyBasisId,
                            x.Date,
                            x.Reason,
                            x.IsSecond,
                            x.IsReduced,
                            x.IsParallel,
                            x.IsListener,
                            x.EntryHeaderId,
                            x.Id
                        }).FirstOrDefault();

                        ObjectParameter idParam = new ObjectParameter("id", typeof(Guid));
                        context.Protocol_Insert(protocolInfo.StudyLevelGroupId, protocolInfo.FacultyId, protocolInfo.LicenseProgramId, protocolInfo.StudyFormId, protocolInfo.StudyBasisId, EVNumber + "\\1",
                            protocolInfo.Date, 4, protocolInfo.Reason, false, null, protocolInfo.IsSecond, protocolInfo.IsListener, idParam);

                        Guid protocolId = (Guid)idParam.Value;

                        foreach (var id in abitIds)
                        {
                            context.ProtocolHistory_Insert(id, protocolId, false, null, protocolInfo.EntryHeaderId, idParam);
                            context.ProtocolHistory_Delete(id, protocolInfo.Id);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error(exc.Message + (exc.InnerException == null ? "" : "\nINNER EXCEPTION: " + exc.InnerException.Message));
            }
        }

        public static void SetBackDockForCommit(int applicationCommitNumber, List<Guid> AppList)
        {
            using (PriemEntities context = new PriemEntities())
            {
                string message = "Проставить отказ от участия в конкурсе по следующим позициям:";
                int incrmntr = 1;
                var Orig_AppList = new Guid[AppList.Count];
                AppList.CopyTo(Orig_AppList);
                foreach (var App in Orig_AppList)
                {
                    var data = context.Abiturient.Where(x => x.Id == App && !x.BackDoc).Select(x => new
                    {
                        LP = x.Entry.SP_LicenseProgram.Code + " " + x.Entry.SP_LicenseProgram.Name,
                        OP = x.Entry.StudyLevel.Acronym + "." + x.Entry.SP_ObrazProgram.Number + "." + MainClass.sPriemYear + " " + x.Entry.SP_ObrazProgram.Name,
                        Prof = x.Entry.ProfileName,
                        StudyForm = x.Entry.StudyForm.Acronym,
                        StudyBasis = x.Entry.StudyBasis.Acronym
                    }).FirstOrDefault();

                    if (data == null)//бред, конечно, но кто знает
                        AppList.Remove(App);
                    else
                        message += "\n" + incrmntr++ + ") " + data.LP + "\n" + data.OP + "; Профиль:" + data.Prof + "; " + data.StudyForm + "; " + data.StudyBasis;
                }
                if (AppList.Count == 0)
                {
                    var abit = (from ab in context.Abiturient
                                where ab.CommitNumber == applicationCommitNumber
                                select ab).FirstOrDefault();
                    if (abit != null)
                    {
                        string persId = abit.PersonId.ToString();

                        var dialogres = MessageBox.Show("Открыть карточку абитуриента?", "Внимание", MessageBoxButtons.YesNo);
                        if (dialogres == System.Windows.Forms.DialogResult.Yes)
                            MainClass.OpenCardPerson(persId, null, null);
                    }
                }
                else
                {
                    var dr = MessageBox.Show(message, "Отзыв старого заявления", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        dr = MessageBox.Show("У меня есть на руках заявление об отказе от участия в конкурсе", "Внимание!", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            foreach (var App in AppList)
                            {
                                context.Abiturient_UpdateBackDoc(true, DateTime.Now, App);
                            }
                        }
                        else //тогда открываем карточку абитуриента
                        {
                            var abit = (from ab in context.Abiturient
                                        where ab.CommitNumber == applicationCommitNumber
                                        select ab).FirstOrDefault();
                            if (abit != null)
                            {
                                string persId = abit.PersonId.ToString();

                                dr = MessageBox.Show("Открыть карточку абитуриента?", "Внимание", MessageBoxButtons.YesNo);
                                if (dr == System.Windows.Forms.DialogResult.Yes)
                                    MainClass.OpenCardPerson(persId, null, null);
                            }
                        }
                    }
                    else //тогда открываем карточку абитуриента
                    {
                        extAbit abit = (from ab in context.extAbit
                                        where ab.CommitNumber == applicationCommitNumber
                                        select ab).FirstOrDefault();

                        if (abit != null)
                        {
                            string fio = abit.FIO;
                            string num = abit.PersonNum;
                            string persId = abit.PersonId.ToString();

                            dr = MessageBox.Show(string.Format("Абитуриент {0} с данным штрих-кодом уже импортирован в базу.\nОткрыть карточку абитуриента?", fio), "Внимание", MessageBoxButtons.YesNo);
                            if (dr == System.Windows.Forms.DialogResult.Yes)
                                MainClass.OpenCardPerson(persId, null, null);
                        }
                    }
                }
            }
        }
    }
}
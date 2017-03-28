using EducServLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PriemLib
{
    public class OlympFISImporterClass
    {
        class EntOlympBook
        {
            public int OlympYear { get; set; }
            public int OlympTypeID { get; set; }
            public string OlympName { get; set; }
            public int OlympLevelID { get; set; }
            public int OlympSubjectID { get; set; }
            public int OlympProfileID { get; set; }
            public string OlympicID { get; set; }
            public string OlympicNumber { get; set; }
        }
        public static void ImportDataFromXML(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            List<EntOlympBook> lstToImport = new List<EntOlympBook>();

            var lstnodes = doc.SelectNodes("/DictionaryData/DictionaryItems/DictionaryItem");
            foreach (XmlNode node in lstnodes)
            {
                string OlympicID = node.SelectSingleNode("OlympicID").FirstChild.Value;
                string OlympicName = node.SelectSingleNode("OlympicName").FirstChild.Value;
                string Year = node.SelectSingleNode("Year").FirstChild.Value;

                string OlympicNumber = null;
                var NumNode = node.SelectSingleNode("OlympicNumber");
                if (NumNode != null)
                    OlympicNumber = NumNode.FirstChild.Value;

                int iOlympType = 4;//прочие

                if (OlympicName.Equals("Олимпиада школьников Санкт-Петербургского государственного университета", StringComparison.OrdinalIgnoreCase))
                    iOlympType = 3;
                if (OlympicName.Equals("Всероссийская олимпиада школьников", StringComparison.OrdinalIgnoreCase))
                    iOlympType = 2;

                var lstProfiles = node.SelectNodes("Profiles/Profile");
                foreach (XmlNode profNode in lstProfiles)
                {
                    string ProfileID = profNode.SelectSingleNode("ProfileID").FirstChild.Value;
                    string LevelID = profNode.SelectSingleNode("LevelID").FirstChild.Value;
                    
                    
                    var lstSubjects = profNode.SelectNodes("Subjects/Subject");
                    if (lstSubjects.Count == 0)
                    {
                        EntOlympBook ol = new EntOlympBook();
                        ol.OlympYear = int.Parse(Year);
                        ol.OlympTypeID = iOlympType;
                        ol.OlympName = OlympicName;
                        ol.OlympLevelID = int.Parse(LevelID);
                        ol.OlympProfileID = int.Parse(ProfileID);
                        ol.OlympicNumber = OlympicNumber;

                        ol.OlympicID = OlympicID;
                        lstToImport.Add(ol);
                    }
                    else
                    {
                        foreach (XmlNode subjNode in lstSubjects)
                        {
                            EntOlympBook ol = new EntOlympBook();
                            ol.OlympYear = int.Parse(Year);
                            ol.OlympTypeID = iOlympType;
                            ol.OlympName = OlympicName;
                            ol.OlympLevelID = int.Parse(LevelID);
                            ol.OlympProfileID = int.Parse(ProfileID);
                            ol.OlympicID = OlympicID;
                            ol.OlympicNumber = OlympicNumber;

                            string SubjectID = subjNode.SelectSingleNode("SubjectID").FirstChild.Value;
                            ol.OlympSubjectID = int.Parse(SubjectID);

                            lstToImport.Add(ol);
                        }
                    }
                }
            }

            using (PriemEntities context = new PriemEntities())
            {
                foreach (var ol in lstToImport)
                {
                    string OlympSubjectName = "";

                    int iOlympNameId = context.OlympName.Where(x => x.Name == ol.OlympName).Select(x => x.Id).DefaultIfEmpty(0).First();
                    if (iOlympNameId == 0)
                    {
                        WinFormsServ.Error("Не найдено соответствие названию олимпиады OlympName=" + ol.OlympName);
                        break;
                    }

                    int iOlympLevelId = context.OlympLevel.Where(x => x.FISID == ol.OlympLevelID).Select(x => x.Id).DefaultIfEmpty(0).First();

                    int iOlympSubjectId = 0;
                    if (ol.OlympSubjectID == 0)
                    {
                        int? olSubj = context.OlympProfile.Where(x => x.Id == ol.OlympProfileID).Select(x => x.SubjectID_FIS).DefaultIfEmpty(0).First();
                        if (olSubj.HasValue && olSubj.Value != 0)
                            iOlympSubjectId = context.OlympSubject.Where(x => x.SubjectID_FIS == olSubj).Select(x => x.Id).DefaultIfEmpty(0).First();
                    }
                    else
                    {
                        OlympSubjectName = context.FISSubject.Where(x => x.Id == ol.OlympSubjectID).Select(x => x.Name)
                            .DefaultIfEmpty("название не определено, требуется дозагрузка справочника")
                            .First();
                        //ищем по ИД предмета OlympSubject.Id в базе
                        iOlympSubjectId = context.OlympSubject.Where(x => x.SubjectID_FIS == ol.OlympSubjectID).Select(x => x.Id).DefaultIfEmpty(0).First();
                        //если такого нет, смотрим по профилю
                        if (iOlympSubjectId == 0)
                        {
                            int? olSubj = context.OlympProfile.Where(x => x.Id == ol.OlympProfileID).Select(x => x.SubjectID_FIS).DefaultIfEmpty(0).First();
                            if (olSubj.HasValue && olSubj.Value != 0)
                                iOlympSubjectId = context.OlympSubject.Where(x => x.SubjectID_FIS == olSubj).Select(x => x.Id).DefaultIfEmpty(0).First();
                        }
                    }

                    if (iOlympSubjectId == 0)
                    {
                        WinFormsServ.Error("Не найдено соответствие предмету с SubjectID=" + ol.OlympSubjectID + "(" + OlympSubjectName + "); ProfileID=" + ol.OlympProfileID);
                        continue;
                    }

                    OlympBook OlB = context.OlympBook.Where(x => x.OlympYear == ol.OlympYear
                        && x.OlympTypeId == ol.OlympTypeID && x.OlympNameId == iOlympNameId && x.OlympLevelId == iOlympLevelId
                        && x.OlympProfileId == ol.OlympProfileID && x.OlympSubjectId == iOlympSubjectId).FirstOrDefault();

                    bool bInsert = false;
                    if (OlB == null && ol.OlympYear == MainClass.iPriemYear)
                    {
                        bInsert = true;
                        OlB = new OlympBook();
                        OlB.Id = Guid.NewGuid();
                        OlB.OlympYear = ol.OlympYear;
                        OlB.OlympTypeId = ol.OlympTypeID;
                        OlB.OlympNameId = iOlympNameId;
                        OlB.OlympLevelId = iOlympLevelId;
                        OlB.OlympProfileId = ol.OlympProfileID;
                        OlB.OlympSubjectId = iOlympSubjectId;
                    }

                    if (OlB != null)
                    {
                        OlB.OlympNumber = ol.OlympicNumber;
                        OlB.OlympicID = ol.OlympicID;
                    }

                    if (bInsert)
                        context.OlympBook.Add(OlB);

                    context.SaveChanges();
                }
            }
        }
    }

    
}

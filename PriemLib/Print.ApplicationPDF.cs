using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace PriemLib
{
    partial class Print
    {
        //1курс-магистратура ОСНОВНОЙ (AbitTypeId = 1)
        public static byte[] GetApplicationPDF(string dirPath, bool isMag, Guid PersonId)
        {
            if (MainClass.dbType == PriemType.PriemAspirant)
                return GetApplicationPDF_Aspirant(PersonId, dirPath);
            else if (MainClass.dbType == PriemType.PriemSPO)
                return GetApplicationPDF_SPO(PersonId, dirPath);
            else if (MainClass.dbType == PriemType.PriemAG)
                return GetApplicationPDF_AG(PersonId, dirPath);

            List<byte[]> lstFiles = new List<byte[]>();
            List<byte[]> lstAppendixes = new List<byte[]>();

            List<ApplicationData> abitList = ApplicationDataProvider.GetAppData(PersonId);
            extPerson person = PersonDataProvider.GetExtPerson(PersonId);
            extPerson_EducationInfo PersonEduc = PersonDataProvider.GetPersonCurrentInfo(PersonId);

            MemoryStream ms = new MemoryStream();
            string dotName;

            if (isMag)//mag
                dotName = "ApplicationMag_page3.pdf";
            else
                dotName = "Application_page3.pdf";

            PdfReader pdfRd = GetAcrobatFileFromTemplate(dirPath + "\\" + dotName);
            PdfStamper pdfStm = GetPdfStamper(ref pdfRd, ref ms, false);
            AcroFields acrFlds = pdfStm.AcroFields;

            FillInnerPriorData(abitList, dirPath, person, isMag, ref lstFiles, ref lstAppendixes);

            if (person.HostelEduc)
                acrFlds.SetField("HostelEducYes", "1");
            else
                acrFlds.SetField("HostelEducNo", "1");

            acrFlds.SetField("HostelAbitYes", person.HostelAbit ? "1" : "0");
            acrFlds.SetField("HostelAbitNo", person.HostelAbit ? "0" : "1");

            acrFlds.SetField("BirthDateYear", person.BirthDate.Year.ToString("D2"));
            acrFlds.SetField("BirthDateMonth", person.BirthDate.Month.ToString("D2"));
            acrFlds.SetField("BirthDateDay", person.BirthDate.Day.ToString());

            acrFlds.SetField("BirthPlace", person.BirthPlace);
            acrFlds.SetField("Male", person.Sex ? "1" : "0");
            acrFlds.SetField("Female", person.Sex ? "0" : "1");
            acrFlds.SetField("Nationality", person.NationalityName);
            acrFlds.SetField("PassportSeries", person.PassportSeries);
            acrFlds.SetField("PassportNumber", person.PassportNumber);

            //dd.MM.yyyy :12.05.2000
            string[] splitStr = GetSplittedStrings(person.PassportAuthor + " " + person.PassportDate.Value.ToString("dd.MM.yyyy"), 60, 70, 2);
            for (int i = 1; i <= 2; i++)
                acrFlds.SetField("PassportAuthor" + i, splitStr[i - 1]);

            if (person.HasRussianNationality || person.NationalityId == MainClass.countryRussiaId || person.ForeignNationalityId == MainClass.foreignCountryRussiaId)
                acrFlds.SetField("HasRussianNationalityYes", "1");
            else
                acrFlds.SetField("HasRussianNationalityNo", "1");

            string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.NationalityId == 1 ? (person.RegionName + ", ") ?? "" : person.CountryName + ", "), (person.City + ", ") ?? "") +
                string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                person.Flat == string.Empty ? "" : "кв. " + person.Flat);

            splitStr = GetSplittedStrings(Address, 50, 70, 3);
            for (int i = 1; i <= 3; i++)
                acrFlds.SetField("Address" + i, splitStr[i - 1]);

            acrFlds.SetField("EnglishMark", person.EnglishMark.ToString());
            if (person.StartEnglish)
                acrFlds.SetField("chbEnglishYes", "1");
            else
                acrFlds.SetField("chbEnglishNo", "1");

            acrFlds.SetField("Phone", person.Phone);
            acrFlds.SetField("Email", person.Email);
            acrFlds.SetField("Mobiles", person.Mobiles);

            acrFlds.SetField("ExitYear", PersonEduc.SchoolExitYear.ToString());
            splitStr = GetSplittedStrings(PersonEduc.SchoolName ?? "", 50, 70, 2);
            for (int i = 1; i <= 2; i++)
                acrFlds.SetField("School" + i, splitStr[i - 1]);

            //только у магистров
            acrFlds.SetField("HEProfession", PersonEduc.HEProfession ?? "");
            acrFlds.SetField("Qualification", PersonEduc.HEQualification ?? "");

            acrFlds.SetField("Original", "0");
            acrFlds.SetField("Copy", "0");
            acrFlds.SetField("CountryEduc", PersonEduc.CountryEducName ?? "");
            acrFlds.SetField("Language", person.LanguageName ?? "");

            string extraPerson = person.PersonInfo ?? "";
            splitStr = GetSplittedStrings(extraPerson, 70, 70, 3);
            for (int i = 1; i <= 3; i++)
            {
                acrFlds.SetField("Parents" + i.ToString(), splitStr[i - 1]);
                acrFlds.SetField("ExtraParents" + i.ToString(), splitStr[i - 1]);
            }

            string Attestat = PersonEduc.SchoolTypeId == 1 ? ("аттестат серия " + (PersonEduc.AttestatSeries ?? "") + " №" + (PersonEduc.AttestatNum ?? "")) :
                    ("диплом серия " + (PersonEduc.DiplomSeries ?? "") + " №" + (PersonEduc.DiplomNum ?? ""));
            acrFlds.SetField("Attestat", Attestat);
            acrFlds.SetField("Extra", person.ExtraInfo ?? "");

            if (PersonEduc.IsEqual && PersonEduc.CountryEducId != 193)
            {
                acrFlds.SetField("IsEqual", "1");
                acrFlds.SetField("EqualSertificateNumber", PersonEduc.EqualDocumentNumber);
            }
            else
                acrFlds.SetField("NoEqual", "1");

            if (person.Privileges > 0)
                acrFlds.SetField("HasPrivileges", "1");

            if ((PersonEduc.SchoolTypeId == 1) || (isMag && PersonEduc.SchoolTypeId == 4 && (PersonEduc.HEQualification).ToLower().IndexOf("магист") < 0))
                acrFlds.SetField("NoEduc", "1");
            else
            {
                acrFlds.SetField("HasEduc", "1");
                acrFlds.SetField("HighEducation", PersonEduc.SchoolName);
            }

            if (!isMag)
                AddEgeInfo(ref acrFlds, PersonId);

            if (!string.IsNullOrEmpty(PersonEduc.SchoolName))
                acrFlds.SetField("chbSchoolFinished", "1");

            if (!string.IsNullOrEmpty(person.Stag))
            {
                acrFlds.SetField("HasStag", "1");
                acrFlds.SetField("WorkPlace", person.WorkPlace);
                acrFlds.SetField("Stag", person.Stag);
            }
            else
                acrFlds.SetField("NoStag", "1");

            int comInd = 1;
            foreach (var comission in abitList.Select(x => x.ComissionAddress).Distinct().ToList())
                acrFlds.SetField("Comission" + comInd++, comission.ToString());

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            lstFiles.Add(ms.ToArray());

            return MergePdfFiles(lstFiles.Union(lstAppendixes).ToList());
        }
        public static void AddEgeInfo(ref AcroFields acrFlds, Guid PersonId)
        {
            using (PriemEntities context = new PriemEntities())
            {
                //EGE
                var exams =
                    context.extEgeMark.Where(x => x.PersonId == PersonId).Select(x => new
                    {
                        ExamName = x.EgeExamName,
                        MarkValue = x.Value,
                        x.Number
                    }).ToList();
                int egeCnt = 1;
                foreach (var ex in exams)
                {
                    acrFlds.SetField("TableName" + egeCnt, ex.ExamName);
                    acrFlds.SetField("TableValue" + egeCnt, ex.MarkValue.ToString());
                    acrFlds.SetField("TableNumber" + egeCnt, ex.Number);

                    if (egeCnt == 4)
                        break;
                    egeCnt++;
                }

                //VSEROS
                var OlympVseros = context.Olympiads.Where(x => x.Abiturient.PersonId == PersonId && x.OlympTypeId == 2)
                    .Select(x => new { x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).Distinct().ToList();
                egeCnt = 1;
                foreach (var ex in OlympVseros)
                {
                    acrFlds.SetField("OlympVserosName" + egeCnt, ex.Name);
                    acrFlds.SetField("OlympVserosYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                    acrFlds.SetField("OlympVserosDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                    if (egeCnt == 2)
                        break;
                    egeCnt++;
                }

                //OTHEROLYMPS
                var OlympNoVseros = context.Olympiads.Where(x => x.Abiturient.PersonId == PersonId && x.OlympTypeId != 2)
                    .Select(x => new { x.OlympName.Name, OlympSubject = x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                egeCnt = 1;
                foreach (var ex in OlympNoVseros)
                {
                    acrFlds.SetField("OlympName" + egeCnt, ex.Name + " (" + ex.OlympSubject + ")");
                    acrFlds.SetField("OlympYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                    acrFlds.SetField("OlympDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                    if (egeCnt == 2)
                        break;
                    egeCnt++;
                }


            }
        }
        private static void FillInnerPriorData(List<ApplicationData> abitList, string dirPath, extPerson person, bool isMag, ref List<byte[]> lstFiles, ref List<byte[]> lstAppendixes)
        {
            List<ShortAppcationDetails> abitProfileList = ApplicationDataProvider.GetAppProfileList(person.Id);
            List<ShortAppcation> lstApps =
                abitList.Select(x => new ShortAppcation()
                {
                    ApplicationId = x.ApplicationId,
                    LicenseProgramName = x.LicenseProgramCode + " " + x.LicenseProgramName,
                    ObrazProgramName = x.ObrazProgramCrypt + " " + x.ObrazProgramName,
                    ProfileName = x.ProfileName,
                    Priority = x.Priority ?? 1,
                    StudyBasisId = x.StudyBasisId,
                    StudyFormId = x.StudyFormId,
                    HasInnerPriorities = abitProfileList.Where(y => y.ApplicationId == x.ApplicationId).Count() > 0,
                    IsCrimea = x.IsCrimea,
                    IsForeign = x.IsForeign
                }).ToList();

            int incrmtr = 1;
            for (int i = 0; i < lstApps.Count; i++)
            {
                if (lstApps[i].HasInnerPriorities) //если есть профили
                {
                    lstApps[i].InnerPrioritiesNum = incrmtr; //то пишем об этом

                    //и сразу же создаём приложение с описанием - потом приложим
                    lstAppendixes.Add(GetApplicationPDF_Appendix_Mag(abitProfileList.Where(x => x.ApplicationId == lstApps[i].ApplicationId).ToList(), lstApps[i].LicenseProgramName, person.FIO, dirPath, incrmtr, isMag));
                    incrmtr++;
                }
            }

            //на 1 странице размещаем первые 3 конкурса
            List<ShortAppcation> lstAppsFirst = lstApps.Take(3).ToList();
            string code = (MainClass.iPriemYear % 100).ToString() + person.Num.ToString("D5");
            //добавляем первый файл
            lstFiles.Add(GetApplicationPDF_FirstPage(lstAppsFirst, lstApps, dirPath, isMag ? "ApplicationMag_page1.pdf" : "Application_page1.pdf", person.FIO, code, isMag));

            //остальные - по 4 на новую страницу
            int appcount = 3;
            while (appcount < lstApps.Count)
            {
                lstAppsFirst = new List<ShortAppcation>();
                for (int u = 0; u < 4; u++)
                {
                    if (lstApps.Count > appcount)
                        lstAppsFirst.Add(lstApps[appcount]);
                    else
                        break;
                    appcount++;
                }

                lstFiles.Add(GetApplicationPDF_NextPage(lstAppsFirst, lstApps, dirPath, "ApplicationMag_page2.pdf", person.FIO));
            }
        }

        public static byte[] GetApplicationPDF_Appendix_Mag(List<ShortAppcationDetails> lst, string LicenseProgramName, string FIO, string dirPath, int Num, bool bIsMag)
        {
            MemoryStream ms = new MemoryStream();
            string dotName = "PriorityProfiles_Mag2014.pdf";
            if (!bIsMag)
                dotName = "PriorityProfiles2015.pdf";

            PdfReader pdfRd = GetAcrobatFileFromTemplate(dirPath + "\\" + dotName);
            PdfStamper pdfStm = GetPdfStamper(ref pdfRd, ref ms, false);

            AcroFields acrFlds = pdfStm.AcroFields;
            acrFlds.SetField("Num", Num.ToString());
            acrFlds.SetField("FIO", FIO);

            acrFlds.SetField("ObrazProgramHead", lst.First().ObrazProgramName);
            acrFlds.SetField("LicenseProgram", LicenseProgramName);
            acrFlds.SetField("ObrazProgram", lst.First().ObrazProgramName);
            int rwind = 1;

            if (bIsMag)
            {
                foreach (var p in lst.Select(x => new { x.ProfileName, x.Priority }).Distinct().OrderBy(x => x.Priority))
                    acrFlds.SetField("Profile" + rwind++, p.ProfileName);
            }
            else
            {
                foreach (var Prof in lst.OrderBy(x => x.Priority))
                {
                    //если других программ нет, то нет смысла показывать её название.
                    bool bShowObrazProgram = lst.Where(x => x.ObrazProgramName != Prof.ObrazProgramName).Count() > 0;
                    acrFlds.SetField("Profile" + rwind++, (bShowObrazProgram ? Prof.ObrazProgramName + " /\n" : "") + Prof.ProfileName);
                }
            }

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }

        public static byte[] GetApplicationPDF_FirstPage(List<ShortAppcation> lst, List<ShortAppcation> lstFullSource, string dirPath, string dotName, string FIO, string regNum, bool isMag)
        {
            MemoryStream ms = new MemoryStream();
            PdfReader pdfRd = GetAcrobatFileFromTemplate(dirPath + "\\" + dotName);
            PdfStamper pdfStm = GetPdfStamper(ref pdfRd, ref ms, false);

            AcroFields acrFlds = pdfStm.AcroFields;
            acrFlds.SetField("FIO", FIO);
            acrFlds.SetField("RegNum", regNum);

            int rwind = 1;
            foreach (var p in lst.OrderBy(x => x.Priority))
            {
                acrFlds.SetField("Priority" + rwind, p.Priority.ToString());
                acrFlds.SetField("Profession" + rwind, p.LicenseProgramName);
                acrFlds.SetField("ObrazProgram" + rwind, p.ObrazProgramName);
                acrFlds.SetField("Specialization" + rwind, p.HasInnerPriorities ? "Приложение к заявлению № " + p.InnerPrioritiesNum : p.ProfileName);
                acrFlds.SetField("StudyForm" + p.StudyFormId.ToString() + rwind.ToString(), "1");
                acrFlds.SetField("StudyBasis" + p.StudyBasisId.ToString() + rwind.ToString(), "1");

                string sQuota = "";
                if (p.IsCrimea)
                    sQuota = "в рамках квоты мест для лиц, постоянно проживающих в Крыму";
                else if (p.IsForeign)
                    sQuota = "в рамках квоты мест для обучения иностранных граждан и лиц без гражданства";
                acrFlds.SetField("QuotaType" + rwind.ToString(), sQuota);

                rwind++;
            }

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }
        public static byte[] GetApplicationPDF_NextPage(List<ShortAppcation> lst, List<ShortAppcation> lstFullSource, string dirPath, string dotName, string FIO)
        {
            MemoryStream ms = new MemoryStream();
            PdfReader pdfRd = GetAcrobatFileFromTemplate(dirPath + "\\" + dotName);
            PdfStamper pdfStm = GetPdfStamper(ref pdfRd, ref ms, false);

            AcroFields acrFlds = pdfStm.AcroFields;
            int rwind = 1;
            foreach (var p in lst.OrderBy(x => x.Priority))
            {
                acrFlds.SetField("Priority" + rwind, p.Priority.ToString());
                acrFlds.SetField("Profession" + rwind, p.LicenseProgramName);
                acrFlds.SetField("ObrazProgram" + rwind, p.ObrazProgramName);
                acrFlds.SetField("Specialization" + rwind, p.HasInnerPriorities ? "Приложение к заявлению № " + p.InnerPrioritiesNum : p.ProfileName);
                acrFlds.SetField("StudyForm" + p.StudyFormId.ToString() + rwind.ToString(), "1");
                acrFlds.SetField("StudyBasis" + p.StudyBasisId.ToString() + rwind.ToString(), "1");

                if (lstFullSource.Where(x => x.LicenseProgramName == p.LicenseProgramName && x.ObrazProgramName == p.ObrazProgramName && x.ProfileName == p.ProfileName && x.StudyFormId == p.StudyFormId).Count() > 1)
                    acrFlds.SetField("IsPriority" + rwind, "1");

                rwind++;
            }

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }

        public static byte[] GetApplicationPDF_Agreement_Mag(string FIO, bool bSex)
        {
            MemoryStream ms = new MemoryStream();
            string dotName = string.Format("ApplicationAgreement_MagSex{0}.pdf", bSex ? "1" : "0");

            byte[] templateBytes;
            using (FileStream fs = new FileStream(MainClass.dirTemplates.TrimEnd('\\') + "\\" + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            AcroFields acrFlds = pdfStm.AcroFields;

            acrFlds.SetField("FIO", FIO);

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }
        public static byte[] GetApplicationPDF_Agreement_1k(string FIO, bool bSex)
        {
            MemoryStream ms = new MemoryStream();
            string dotName = string.Format("ApplicationAgreement_1kSex{0}.pdf", bSex ? "1" : "0");

            byte[] templateBytes;
            using (FileStream fs = new FileStream(MainClass.dirTemplates.TrimEnd('\\') + "\\" + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            AcroFields acrFlds = pdfStm.AcroFields;

            acrFlds.SetField("FIO", FIO);

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }

        //Аспирантура
        public static byte[] GetApplicationPDF_Aspirant(Guid PersonId, string dirPath)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var abitList = (from x in context.Abiturient
                                //join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.extEntry on x.EntryId equals Entry.Id
                                where x.PersonId == PersonId
                                select new
                                {
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,
                                    x.Priority,
                                    x.Entry.IsForeign,
                                    x.Entry.IsCrimea
                                }).ToList();

                
                var person = (from x in context.extPerson
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.HostelAbit,
                                  x.BirthDate,
                                  BirthPlace = x.BirthPlace ?? "",
                                  Sex = x.Sex,
                                  Nationality = x.NationalityName,
                                  Country = x.CountryName,
                                  PassportType = x.PassportTypeName,
                                  x.PassportSeries,
                                  x.PassportNumber,
                                  x.PassportAuthor,
                                  x.PassportDate,
                                  x.City,
                                  Region = x.RegionName,
                                  x.Code,
                                  x.Street,
                                  x.House,
                                  x.Korpus,
                                  x.Flat,
                                  x.Phone,
                                  x.Mobiles,
                                  AddInfo = x.ExtraInfo,
                                  Parents = x.PersonInfo,
                                  HasPrivileges = x.Privileges > 0,
                                  x.HostelEduc,
                                  IsRussia = x.NationalityId == 1,
                                  x.HasRussianNationality,
                                  StartEnglish = (bool?)x.StartEnglish,
                                  x.EnglishMark,
                                  Language = x.LanguageName,
                                  x.Email,
                                  x.Stag,
                                  x.WorkPlace,
                                  x.ScienceWork
                              }).FirstOrDefault();

                var personEducation = context.Person_EducationInfo.Where(x => x.PersonId == PersonId)
                    .Select(x => new
                    {
                        x.SchoolExitYear,
                        x.SchoolName,
                        x.IsEqual,
                        x.EqualDocumentNumber,
                        CountryEduc = x.Country.Name,
                        x.CountryEducId,
                        x.SchoolTypeId,
                        EducationDocumentSeries = x.SchoolTypeId == 1 ? x.AttestatSeries : x.DiplomSeries,
                        EducationDocumentNumber = x.SchoolTypeId == 1 ? x.AttestatNum : x.DiplomNum,
                    }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "\\ApplicationAsp_2015.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());

                if (person.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                acrFlds.SetField("HostelAbitYes", person.HostelAbit ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", person.HostelAbit ? "0" : "1");

                acrFlds.SetField("BirthDateYear", person.BirthDate.Year.ToString("D2"));
                acrFlds.SetField("BirthDateMonth", person.BirthDate.Month.ToString("D2"));
                acrFlds.SetField("BirthDateDay", person.BirthDate.Day.ToString());

                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");
                acrFlds.SetField("Nationality", person.Nationality);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);

                //dd.MM.yyyy :12.05.2000
                string[] splitStr = GetSplittedStrings(person.PassportAuthor + " " + person.PassportDate.Value.ToString("dd.MM.yyyy"), 60, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("PassportAuthor" + i, splitStr[i - 1]);

                string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.IsRussia ? (person.Region + ", ") ?? "" : person.Country + ", "), (person.City + ", ") ?? "") +
                    string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                    person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                    person.Flat == string.Empty ? "" : "кв. " + person.Flat);

                if (person.HasRussianNationality)
                    acrFlds.SetField("HasRussianNationalityYes", "1");
                else
                    acrFlds.SetField("HasRussianNationalityNo", "1");

                splitStr = GetSplittedStrings(Address, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Address" + i, splitStr[i - 1]);

                acrFlds.SetField("EnglishMark", person.EnglishMark.ToString());
                if (person.StartEnglish ?? false)
                    acrFlds.SetField("chbEnglishYes", "1");
                else
                    acrFlds.SetField("chbEnglishNo", "1");

                acrFlds.SetField("Phone", person.Phone);
                acrFlds.SetField("Email", person.Email);
                acrFlds.SetField("Mobiles", person.Mobiles);

                acrFlds.SetField("Language", person.Language);

                acrFlds.SetField("ExitYear", personEducation.SchoolExitYear.ToString());
                splitStr = GetSplittedStrings(personEducation.SchoolName ?? "", 50, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("School" + i, splitStr[i - 1]);

                //только у магистров
                var HEInfo = context.Person_EducationInfo
                    .Where(x => x.PersonId == PersonId)
                    .Select(x => new { x.HEProfession, x.HEQualification }).FirstOrDefault();
                acrFlds.SetField("HEProfession", HEInfo.HEProfession ?? "");
                acrFlds.SetField("Qualification", HEInfo.HEQualification ?? "");

                acrFlds.SetField("Original", "0");
                acrFlds.SetField("Copy", "0");
                acrFlds.SetField("CountryEduc", personEducation.CountryEduc ?? "");
                acrFlds.SetField("Language", person.Language ?? "");

                string extraPerson = person.Parents ?? "";
                splitStr = GetSplittedStrings(extraPerson, 70, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Parents" + i.ToString(), splitStr[i - 1]);

                string Attestat = personEducation.SchoolTypeId == 1 ? ("аттестат серия " + (personEducation.EducationDocumentSeries ?? "") + " №" + (personEducation.EducationDocumentNumber ?? "")) :
                        ("диплом серия " + (personEducation.EducationDocumentSeries ?? "") + " №" + (personEducation.EducationDocumentNumber ?? ""));
                acrFlds.SetField("Attestat", Attestat);
                acrFlds.SetField("Extra", person.AddInfo ?? "");

                if (personEducation.IsEqual && personEducation.CountryEducId != 193)
                {
                    acrFlds.SetField("IsEqual", "1");
                    acrFlds.SetField("EqualSertificateNumber", personEducation.EqualDocumentNumber);
                }
                else
                {
                    acrFlds.SetField("NoEqual", "1");
                }

                if (person.HasPrivileges)
                    acrFlds.SetField("HasPrivileges", "1");

                if ((personEducation.SchoolTypeId != 2) && (personEducation.SchoolTypeId != 5))//SSUZ & SPO
                    acrFlds.SetField("NoEduc", "1");
                else
                {
                    acrFlds.SetField("HasEduc", "1");
                    acrFlds.SetField("HighEducation", personEducation.SchoolName);
                }

                //VSEROS
                var OlympVseros = context.Olympiads.Where(x => x.Abiturient.PersonId == PersonId && x.OlympType.IsVseross)
                    .Select(x => new { x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                int egeCnt = 1;
                foreach (var ex in OlympVseros)
                {
                    acrFlds.SetField("OlympVserosName" + egeCnt, ex.Name);
                    acrFlds.SetField("OlympVserosYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                    acrFlds.SetField("OlympVserosDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                    if (egeCnt == 2)
                        break;
                    egeCnt++;
                }

                //OTHEROLYMPS
                var OlympNoVseros = context.Olympiads.Where(x => x.Abiturient.PersonId == PersonId && !x.OlympType.IsVseross)
                    .Select(x => new { x.OlympName.Name, OlympSubject = x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                egeCnt = 1;
                foreach (var ex in OlympNoVseros)
                {
                    acrFlds.SetField("OlympName" + egeCnt, ex.Name + " (" + ex.OlympSubject + ")");
                    acrFlds.SetField("OlympYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                    acrFlds.SetField("OlympDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                    if (egeCnt == 2)
                        break;
                    egeCnt++;
                }


                if (!string.IsNullOrEmpty(person.WorkPlace) && !string.IsNullOrEmpty(person.Stag))
                {
                    acrFlds.SetField("HasStag", "1");
                    acrFlds.SetField("WorkPlace", person.WorkPlace);
                    acrFlds.SetField("Stag", person.Stag);
                }
                else
                    acrFlds.SetField("NoStag", "1");

                int rwInd = 1;
                foreach (var abit in abitList.OrderBy(x => x.Priority))
                {
                    acrFlds.SetField("Profession" + rwInd, abit.ProfessionCode + " " + abit.Profession);
                    acrFlds.SetField("Specialization" + rwInd, abit.Specialization);
                    acrFlds.SetField("ObrazProgram" + rwInd, abit.ObrazProgram);
                    acrFlds.SetField("Priority" + rwInd, abit.Priority.ToString());

                    acrFlds.SetField("StudyForm" + abit.StudyFormId + rwInd, "1");
                    acrFlds.SetField("StudyBasis" + abit.StudyBasisId + rwInd, "1");

                    if (abitList.Where(x => x.Profession == abit.Profession && x.ObrazProgram == abit.ObrazProgram && x.Specialization == abit.Specialization && x.StudyFormId == abit.StudyFormId).Count() > 1)
                        acrFlds.SetField("IsPriority" + rwInd, "1");
                    rwInd++;
                }

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        //СПО
        public static byte[] GetApplicationPDF_SPO(Guid PersonId, string dirPath)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var abitList = (from x in context.extAbit
                                join Entry in context.extEntry on x.EntryId equals Entry.Id
                                where x.PersonId == PersonId
                                && MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId)
                                select new
                                {
                                    x.Id,
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,
                                    CommitIntNumber = x.CommitNumber,
                                    Priority = x.Priority ?? 0,
                                    IsGosLine = Entry.IsForeign && Entry.StudyBasisId == 1,
                                    //x.IsGosLine
                                }).ToList();

                var person = (from x in context.Person
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.Person_AdditionalInfo.HostelAbit,
                                  x.BirthDate,
                                  BirthPlace = x.BirthPlace ?? "",
                                  Sex = x.Sex,
                                  Nationality = x.Nationality.Name,
                                  Country = x.Person_Contacts.Country.Name,
                                  PassportType = x.PassportType.Name,
                                  x.PassportSeries,
                                  x.PassportNumber,
                                  x.PassportAuthor,
                                  x.PassportDate,
                                  x.Person_Contacts.City,
                                  Region = x.Person_Contacts.Region.Name,
                                  x.Person_Contacts.Code,
                                  x.Person_Contacts.Street,
                                  x.Person_Contacts.House,
                                  x.Person_Contacts.Korpus,
                                  x.Person_Contacts.Flat,
                                  x.Person_Contacts.Email,
                                  x.Person_Contacts.Phone,
                                  x.Person_Contacts.Mobiles,
                                  AddInfo = x.Person_AdditionalInfo.ExtraInfo,
                                  Parents = x.Person_AdditionalInfo.PersonInfo,
                                  HasPrivileges = x.Person_AdditionalInfo.Privileges > 0,
                                  SportQualificationName = x.PersonSportQualification.SportQualification.Name,
                                  x.PersonSportQualification.SportQualificationId,
                                  x.PersonSportQualification.SportQualificationLevel,
                                  x.PersonSportQualification.OtherSportQualification,
                                  x.Person_AdditionalInfo.HostelEduc,
                                  x.Person_Contacts.ForeignCountry.IsRussia,
                                  x.HasRussianNationality,
                                  x.Person_AdditionalInfo.StartEnglish,
                                  x.Person_AdditionalInfo.EnglishMark,
                                  Language = x.Person_AdditionalInfo.Language.Name,
                                  x.Person_AdditionalInfo.HasTRKI,
                                  x.Person_AdditionalInfo.TRKICertificateNumber,
                                  x.Person_AdditionalInfo.WorkPlace,
                                  x.Person_AdditionalInfo.Stag,
                                  x.Person_AdditionalInfo.ScienceWork
                              }).FirstOrDefault();

                var personEducation = context.Person_EducationInfo.Where(x => x.PersonId == PersonId)
                    .Select(x => new
                    {
                        x.SchoolExitYear,
                        x.SchoolName,
                        x.IsEqual,
                        x.EqualDocumentNumber,
                        CountryEduc = x.CountryEducId != null ? x.Country.Name : "",
                        x.CountryEducId,
                        x.SchoolTypeId,
                        EducationDocumentSeries = x.SchoolTypeId == 1 ? x.AttestatSeries : x.DiplomSeries,
                        EducationDocumentNumber = x.SchoolTypeId == 1 ? x.AttestatNum : x.DiplomNum,
                    }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "Application_page3.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + "\\" + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                //pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                List<byte[]> lstFiles = new List<byte[]>();

                if (person.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                if (abitList.Where(x => x.IsGosLine).Count() > 0)
                    acrFlds.SetField("IsGosLine", "1");

                acrFlds.SetField("HostelAbitYes", person.HostelAbit ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", person.HostelAbit ? "0" : "1");

                acrFlds.SetField("BirthDateYear", person.BirthDate.Year.ToString("D2"));
                acrFlds.SetField("BirthDateMonth", person.BirthDate.Month.ToString("D2"));
                acrFlds.SetField("BirthDateDay", person.BirthDate.Day.ToString());

                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");
                acrFlds.SetField("Nationality", person.Nationality);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);

                //dd.MM.yyyy :12.05.2000
                string[] splitStr = GetSplittedStrings(person.PassportAuthor + " " + person.PassportDate.Value.ToString("dd.MM.yyyy"), 60, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("PassportAuthor" + i, splitStr[i - 1]);

                string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.IsRussia ? (person.Region + ", ") ?? "" : person.Country + ", "), (person.City + ", ") ?? "") +
                    string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                    person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                    person.Flat == string.Empty ? "" : "кв. " + person.Flat);

                if (person.HasRussianNationality)
                    acrFlds.SetField("HasRussianNationalityYes", "1");
                else
                    acrFlds.SetField("HasRussianNationalityNo", "1");

                splitStr = GetSplittedStrings(Address, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Address" + i, splitStr[i - 1]);

                acrFlds.SetField("EnglishMark", person.EnglishMark.ToString());
                if (person.StartEnglish)
                    acrFlds.SetField("chbEnglishYes", "1");
                else
                    acrFlds.SetField("chbEnglishNo", "1");

                acrFlds.SetField("Phone", person.Phone);
                acrFlds.SetField("Email", person.Email);
                acrFlds.SetField("Mobiles", person.Mobiles);

                acrFlds.SetField("ExitYear", personEducation.SchoolExitYear.ToString());
                splitStr = GetSplittedStrings(personEducation.SchoolName ?? "", 50, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("School" + i, splitStr[i - 1]);

                //только у магистров
                var HEInfo = context.Person_EducationInfo
                    .Where(x => x.PersonId == PersonId)
                    .Select(x => new { ProgramName = x.HEProfession, Qualification = x.HEQualification }).FirstOrDefault();

                if (HEInfo != null)
                {
                    acrFlds.SetField("HEProfession", HEInfo.ProgramName ?? "");
                    acrFlds.SetField("Qualification", HEInfo.Qualification ?? "");

                    acrFlds.SetField("Original", "0");
                    acrFlds.SetField("Copy", "0");
                    acrFlds.SetField("CountryEduc", personEducation.CountryEduc ?? "");
                    acrFlds.SetField("Language", person.Language ?? "");
                }
                //SportQualification
                if (person.SportQualificationId == 0)
                    acrFlds.SetField("SportQualification", "нет");
                else if (person.SportQualificationId == 44)
                    acrFlds.SetField("SportQualification", person.OtherSportQualification);
                else
                    acrFlds.SetField("SportQualification", person.SportQualificationName + "; " + person.SportQualificationLevel);

                string extraPerson = person.Parents ?? "";
                splitStr = GetSplittedStrings(extraPerson, 70, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Parents" + i.ToString(), splitStr[i - 1]);

                string Attestat = personEducation.SchoolTypeId == 1 ? ("аттестат серия " + (personEducation.EducationDocumentSeries ?? "") + " №" + (personEducation.EducationDocumentNumber ?? "")) :
                        ("диплом серия " + (personEducation.EducationDocumentSeries ?? "") + " №" + (personEducation.EducationDocumentNumber ?? ""));
                acrFlds.SetField("Attestat", Attestat);
                acrFlds.SetField("Extra", person.AddInfo ?? "");

                if (personEducation.IsEqual && personEducation.CountryEducId != 193)
                {
                    acrFlds.SetField("IsEqual", "1");
                    acrFlds.SetField("EqualSertificateNumber", personEducation.EqualDocumentNumber);
                }
                else
                {
                    acrFlds.SetField("NoEqual", "1");
                }

                if (person.HasPrivileges)
                    acrFlds.SetField("HasPrivileges", "1");

                if ((personEducation.SchoolTypeId != 2) && (personEducation.SchoolTypeId != 5))//SSUZ & SPO
                    acrFlds.SetField("NoEduc", "1");
                else
                {
                    acrFlds.SetField("HasEduc", "1");
                    acrFlds.SetField("HighEducation", personEducation.SchoolName);
                }

                //VSEROS
                var OlympVseros = context.Olympiads.Where(x => x.Abiturient.PersonId == PersonId && x.OlympType.IsVseross)
                    .Select(x => new { x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                int egeCnt = 1;
                foreach (var ex in OlympVseros)
                {
                    acrFlds.SetField("OlympVserosName" + egeCnt, ex.Name);
                    acrFlds.SetField("OlympVserosYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                    acrFlds.SetField("OlympVserosDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                    if (egeCnt == 2)
                        break;
                    egeCnt++;
                }

                //OTHEROLYMPS
                var OlympNoVseros = context.Olympiads.Where(x => x.Abiturient.PersonId == PersonId && !x.OlympType.IsVseross)
                    .Select(x => new { x.OlympName.Name, OlympSubject = x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                egeCnt = 1;
                foreach (var ex in OlympNoVseros)
                {
                    acrFlds.SetField("OlympName" + egeCnt, ex.Name + " (" + ex.OlympSubject + ")");
                    acrFlds.SetField("OlympYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                    acrFlds.SetField("OlympDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                    if (egeCnt == 2)
                        break;
                    egeCnt++;
                }

                if (!string.IsNullOrEmpty(person.WorkPlace))
                {
                    acrFlds.SetField("HasStag", "1");
                    acrFlds.SetField("WorkPlace", person.WorkPlace);
                    acrFlds.SetField("Stag", person.Stag);
                }
                else
                    acrFlds.SetField("NoStag", "1");


                string FIO = ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim();

                {
                    int multiplyer = 3;
                    string code = ((multiplyer * 100000) + abitList.Select(x => x.CommitIntNumber ?? 0).DefaultIfEmpty(0).First()).ToString();

                    List<ShortAppcation> lstApps = abitList
                    .Select(x => new ShortAppcation()
                    {
                        ApplicationId = x.Id,
                        LicenseProgramName = x.ProfessionCode + " " + x.Profession,
                        ObrazProgramName = x.ObrazProgram,
                        ProfileName = x.Specialization,
                        Priority = x.Priority,
                        StudyBasisId = x.StudyBasisId,
                        StudyFormId = x.StudyFormId,
                        HasInnerPriorities = false,
                    }).ToList();

                    List<ShortAppcation> lstAppsFirst = new List<ShortAppcation>();
                    for (int u = 0; u < 3; u++)
                    {
                        if (lstApps.Count > u)
                            lstAppsFirst.Add(lstApps[u]);
                    }

                    //добавляем первый файл
                    lstFiles.Add(GetApplicationPDF_FirstPage(lstAppsFirst, lstApps, dirPath, "Application_page1.pdf", FIO, code, false));
                    //acrFlds.SetField("Version", sVersion);

                    //остальные - по 4 на новую страницу
                    int appcount = 3;
                    while (appcount < lstApps.Count)
                    {
                        lstAppsFirst = new List<ShortAppcation>();
                        for (int u = 0; u < 4; u++)
                        {
                            if (lstApps.Count > appcount)
                                lstAppsFirst.Add(lstApps[appcount]);
                            else
                                break;
                            appcount++;
                        }

                        lstFiles.Add(GetApplicationPDF_NextPage(lstAppsFirst, lstApps, dirPath, "Application_page2.pdf", FIO));
                    }
                }

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                lstFiles.Add(ms.ToArray());

                return MergePdfFiles(lstFiles);
            }
        }
        public static byte[] GetApplicationPDF_AG(Guid PersonId, string dirPath)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var abitList = (from x in context.extAbit
                                join Entry in context.extEntry on x.EntryId equals Entry.Id
                                join SL in context.StudyLevel on Entry.StudyLevelId equals SL.Id
                                where x.PersonId == PersonId
                                && MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId)
                                select new
                                {
                                    x.Id,
                                    x.PersonId,
                                    x.Barcode,
                                    Profession = Entry.LicenseProgramName,
                                    ObrazProgram = Entry.ObrazProgramName,
                                    Entry.StudyLevelId,
                                    CommitIntNumber = x.CommitNumber,
                                    Priority = x.Priority ?? 0,
                                    x.StudyLevelGroupId,
                                    x.EntryId,
                                    SL.ClassNum,
                                    SL.Duration
                                }).ToList();

                var person = (from x in context.Person
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.Person_AdditionalInfo.HostelEduc,
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "ApplicationAG_2015.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + "\\" + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                AcroFields acrFlds = pdfStm.AcroFields;

                int multiplyer = abitList.FirstOrDefault().StudyLevelGroupId;
                string code = ((multiplyer * 100000) + abitList.First().CommitIntNumber).ToString();
                Barcode128 barcode = new Barcode128();
                barcode.Code = code;
                PdfContentByte cb = pdfStm.GetOverContent(1);
                iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
                // img.SetAbsolutePosition(420, 720);
                img.SetAbsolutePosition(120, 775);
                cb.AddImage(img);

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                acrFlds.SetField("ObrazProgramYear", abitList.FirstOrDefault().Duration.ToString());
                acrFlds.SetField("EntryClass", abitList.FirstOrDefault().ClassNum);
                if (person.HostelEduc)
                    acrFlds.SetField("HostelAbitYes", "1");
                else
                    acrFlds.SetField("HostelAbitNo", "1");

                int inc = 0;
                bool hasSecondApp = abitList.Count() > 1;
                foreach (var abit in abitList)
                {
                    inc++;
                    string i = inc.ToString();
                    if (hasSecondApp)
                    {
                        acrFlds.SetField("chbIsPriority" + i, "1");
                        hasSecondApp = false;
                    }

                    if (abit.ClassNum.IndexOf("10") < 0)
                        acrFlds.SetField("chbSchoolLevel1" + i, "1");
                    else
                        acrFlds.SetField("chbSchoolLevel2" + i, "1");

                    acrFlds.SetField("RegNum" + i, (800000 + abit.Barcode).ToString());
                    acrFlds.SetField("Profession" + i, abit.Profession);
                    acrFlds.SetField("ObrazProgram" + i, abit.ObrazProgram);

                    var Exams = (from x in context.AbiturientSelectedExam
                                 join ex_un in context.ExamInEntryBlockUnit on x.ExamInEntryBlockUnitId equals ex_un.Id
                                 join ex_bl in context.ExamInEntryBlock on ex_un.ExamInEntryBlockId equals ex_bl.Id
                                 join ex in context.Exam on ex_un.ExamId equals ex.Id
                                 join ex_n in context.ExamName on ex.ExamNameId equals ex_n.Id
                                 where x.ApplicationId == abit.Id && ex_bl.EntryId == abit.EntryId
                                 select ex_n.Name ).ToList();
                    if (Exams.Count == 0)
                        acrFlds.SetField("ManualExam" + i, "нет");
                    else
                    {
                        string ExamNames = "";
                        foreach (var x in Exams)
                            ExamNames += x + ", ";
                        if (ExamNames.Length > 2)
                            ExamNames = ExamNames.Substring(0, ExamNames.Length - 2);
                        acrFlds.SetField("ManualExam" + i, ExamNames);
                    }
                }

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        public static PdfReader GetAcrobatFileFromTemplate(string templateFile)
        {
            byte[] templateBytes;
            using (FileStream fs = new FileStream(templateFile, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            return new PdfReader(templateBytes);
        }
        public static PdfStamper GetPdfStamper(ref PdfReader pdfRd, ref MemoryStream ms, bool setEncryption)
        {
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            if (setEncryption)
                pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);

            return pdfStm;
        }
    }
}

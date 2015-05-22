using EducServLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordOut;

namespace PriemLib
{
    public static partial class Print
    {
        public static void PrintOrderSPO(Guid protocolId, bool isRus, bool isCel)
        {
            try
            {
                WordDoc wd = new WordDoc(string.Format(@"{0}\EntryOrder.dot", MainClass.dirTemplates));
                TableDoc td = wd.Tables[0];

                string docNum;
                DateTime docDate;
                string formId;
                string facDat;

                string basisId;
                string basis = string.Empty;
                string basis2 = string.Empty;
                string form = string.Empty;
                string form2 = string.Empty;

                string LicenseProgramName;
                string LicenseProgramCode;
                int StudyLevelId;

                using (PriemEntities ctx = new PriemEntities())
                {

                    docNum = (from protocol in ctx.OrderNumbers
                              where protocol.ProtocolId == protocolId
                              select protocol.ComissionNumber).DefaultIfEmpty("НЕ УКАЗАН").FirstOrDefault();

                    docDate = (DateTime)(from protocol in ctx.OrderNumbers
                                         where protocol.ProtocolId == protocolId
                                         select protocol.ComissionDate).FirstOrDefault();

                    formId = (from protocol in ctx.Protocol
                              join studyForm in ctx.StudyForm on protocol.StudyFormId equals studyForm.Id
                              where protocol.Id == protocolId
                              select studyForm.Id).FirstOrDefault().ToString();

                    facDat = (from protocol in ctx.Protocol
                              join sP_Faculty in ctx.SP_Faculty on protocol.FacultyId equals sP_Faculty.Id
                              where protocol.Id == protocolId
                              select sP_Faculty.DatName).FirstOrDefault();

                    basisId = (from protocol in ctx.Protocol
                               join studyBasis in ctx.StudyBasis on protocol.StudyBasisId equals studyBasis.Id
                               where protocol.Id == protocolId
                               select studyBasis.Id).FirstOrDefault().ToString();

                    LicenseProgramName = (from entry in ctx.Entry
                                          join extentryView in ctx.extEntryView on entry.LicenseProgramId equals extentryView.LicenseProgramId
                                          where extentryView.Id == protocolId
                                          select entry.SP_LicenseProgram.Name).FirstOrDefault();

                    LicenseProgramCode = (from entry in ctx.Entry
                                          join extentryView in ctx.extEntryView on entry.LicenseProgramId equals extentryView.LicenseProgramId
                                          where extentryView.Id == protocolId
                                          select entry.SP_LicenseProgram.Code).FirstOrDefault();

                    StudyLevelId = (from entry in ctx.Entry
                                    join extentryView in ctx.extEntryView on entry.LicenseProgramId equals extentryView.LicenseProgramId
                                    where extentryView.Id == protocolId
                                    select entry.SP_LicenseProgram.StudyLevelId).FirstOrDefault();
                }

                switch (formId)
                {
                    case "1":
                        form = "очная форма обучения";
                        form2 = "очной";
                        break;
                    case "2":
                        form = "очно-заочная форма обучения";
                        form2 = "очно-заочной";
                        break;
                    case "3":
                        form = "заочная форма обучения";
                        form2 = "заочной";
                        break;
                }

                string naprspecRod = "", profspec = "", naprobProgRod = "", educDoc = "";

                naprobProgRod = "образовательной программе";
                naprspecRod = "специальности";
                profspec = "по профилю";

                string dogovorDoc = "";
                switch (basisId)
                {
                    case "1":
                        basis2 = " за счет бюджетных ассигнований федерального бюджета";
                        dogovorDoc = "";
                        educDoc = ", оригиналы документа установленного образца об образовании";
                        break;
                    case "2":
                        basis2 = " по договорам об образовании";
                        dogovorDoc = ", договоры об образовании";
                        educDoc = "";
                        break;
                }

                wd.SetFields("Граждан", isRus ? "граждан Российской Федерации" : "иностранных граждан");
                wd.SetFields("Граждан2", isRus ? "граждан Российской Федерации" : "");
                wd.SetFields("Стипендия", (basisId != "1" || formId != "1") ? "" : "и назначении стипендии");
                wd.SetFields("Форма2", form2);
                wd.SetFields("Основа2", basis2);

                wd.SetFields("ДатаПриказа", docDate.ToShortDateString());
                wd.SetFields("НомерПриказа", docNum);

                wd.SetFields("DogovorDoc", dogovorDoc);
                wd.SetFields("EducDoc", educDoc);

                int counter = 0;
                string curObProg = "-";
                string curCountry = "-";
                string curMotivation = "-";
                string Motivation = string.Empty;
                string AbitListString = string.Empty;
                string HeaderString = string.Empty;

                using (PriemEntities ctx = new PriemEntities())
                {
                    var lst = (from extabit in ctx.extAbit

                               join extentryView in ctx.extEntryView on extabit.Id equals extentryView.AbiturientId
                               join extperson in ctx.extPerson on extabit.PersonId equals extperson.Id
                               join country in ctx.Country on extperson.NationalityId equals country.Id
                               join competition in ctx.Competition on extabit.CompetitionId equals competition.Id
                               join extabitMarksSum in ctx.extAbitMarksSum on extabit.Id equals extabitMarksSum.Id into extabitMarksSum2
                               from extabitMarksSum in extabitMarksSum2.DefaultIfEmpty()
                               join entryHeader in ctx.EntryHeader on extentryView.EntryHeaderId equals entryHeader.Id into entryHeader2
                               from entryHeader in entryHeader2.DefaultIfEmpty()
                               join celCompetition in ctx.CelCompetition on extabit.CelCompetitionId equals celCompetition.Id into celCompetition2
                               from celCompetition in celCompetition2.DefaultIfEmpty()
                               where extentryView.Id == protocolId && (isRus ? extperson.NationalityId == 1 : extperson.NationalityId != 1)
                               orderby celCompetition.TvorName, extabit.ObrazProgramName, extabit.ProfileName, country.NameRod, entryHeader.SortNum, extabit.FIO
                               select new
                               {
                                   Id = extabit.Id,
                                   Рег_Номер = extabit.RegNum,
                                   Ид_номер = extabit.PersonNum,
                                   TotalSum = (extabit.CompetitionId == 8 || extabit.CompetitionId == 1) ? null : extabitMarksSum.TotalSum,
                                   ФИО = extabit.FIO,
                                   CelCompName = celCompetition.TvorName,
                                   LicenseProgramName = extabit.LicenseProgramName,
                                   LicenseProgramCode = extabit.LicenseProgramCode,
                                   ProfileName = extabit.ProfileName,
                                   ObrazProgram = extabit.ObrazProgramName,
                                   ObrazProgramId = extabit.ObrazProgramId,
                                   EntryHeaderId = entryHeader.Id,
                                   SortNum = entryHeader.SortNum,
                                   EntryHeaderName = entryHeader.Name,
                                   NameRod = country.NameRod,
                                   extabit.ObrazProgramCrypt
                               }).ToList().Distinct().Select(x =>
                                   new
                                   {
                                       Id = x.Id.ToString(),
                                       Рег_Номер = x.Рег_Номер,
                                       Ид_номер = x.Ид_номер,
                                       TotalSum = x.TotalSum.ToString(),
                                       ФИО = x.ФИО,
                                       CelCompName = x.CelCompName,
                                       LicenseProgramName = x.LicenseProgramName,
                                       LicenseProgramCode = x.LicenseProgramCode,
                                       ProfileName = x.ProfileName,
                                       ObrazProgram = x.ObrazProgram.Replace("(очно-заочная)", "").Replace(" ВВ", ""),
                                       x.ObrazProgramCrypt,
                                       EntryHeaderId = x.EntryHeaderId,
                                       SortNum = x.SortNum,
                                       EntryHeaderName = x.EntryHeaderName,
                                       NameRod = x.NameRod,
                                   }
                               ).OrderBy(x => x.CelCompName).ThenBy(x => x.ObrazProgram).ThenBy(x => x.ProfileName).ThenBy(x => x.NameRod).ThenBy(x => x.SortNum).ThenBy(x => x.ФИО).ToList();

                    string LP = lst.First().LicenseProgramName;
                    string LPCode = lst.First().LicenseProgramCode;
                    string header = lst.First().EntryHeaderName;
                    string obProg = lst.First().ObrazProgram;
                    string obProgCode = lst.First().ObrazProgramCrypt;
                    string profileName = lst.First().ProfileName;

                    HeaderString += string.Format("{3}\tпо {0} {1} \"{2}\"\n", naprspecRod, LPCode, LP, curObProg == "-" ? "" : "\r\n");
                    if (!string.IsNullOrEmpty(obProg))
                        HeaderString += string.Format("\tпо {0} {1} \"{2}\"\n", naprobProgRod, obProgCode, obProg);
                    if (!string.IsNullOrEmpty(profileName) && profileName != "нет")
                        HeaderString += string.Format("\t{0} \"{1}\"\n", profspec, profileName);
                    if (!isCel)
                        HeaderString += string.Format("\t{0}:\n", header + ", согласно Приложению");

                    wd.SetFields("EntryHeader", HeaderString);

                    foreach (var v in lst)
                    {
                        ++counter;

                        if (!isRus)
                        {
                            string country = v.NameRod;
                            if (country != curCountry)
                            {
                                AbitListString += string.Format("\r\n граждан {0}:\n", country);
                                curCountry = country;
                            }
                        }

                        string balls = v.TotalSum;
                        string ballToStr = " балл";

                        if (balls.Length == 0)
                            ballToStr = "";
                        else if (balls.EndsWith("1"))
                        {
                            if (balls.EndsWith("11"))
                                ballToStr += "ов";
                            else
                                ballToStr += "";
                        }
                        else if (balls.EndsWith("2") || balls.EndsWith("3") || balls.EndsWith("4"))
                        {
                            if ((balls.EndsWith("12") || balls.EndsWith("13") || balls.EndsWith("14")))
                                ballToStr += "ов";
                            else
                                ballToStr += "а";
                        }
                        else
                            ballToStr += "ов";

                        if (isCel && curMotivation == "-")
                            curMotivation = string.Format("ОСНОВАНИЕ: договор об организации целевого приема с {0} от … № …, Протокол заседания Приемной комиссии СПбГУ от 30.07.2013 № ..., личное заявление, оригинал документа государственного образца об образовании.", v.CelCompName);
                        string tmpMotiv = curMotivation;
                        Motivation = string.Format("ОСНОВАНИЕ: договор об организации целевого приема с {0} от … № …, Протокол заседания Приемной комиссии СПбГУ от 30.07.2013 № ..., личное заявление, оригинал документа государственного образца об образовании.", v.CelCompName);

                        if (isCel && curMotivation != Motivation)
                        {
                            string CelCompText = v.CelCompName;
                            Motivation = string.Format("ОСНОВАНИЕ: договор об организации целевого приема с {0} от … № …, Протокол заседания Приемной комиссии СПбГУ от 30.07.2013 № .., личное заявление, оригинал документа государственного образца об образовании.", CelCompText);
                            curMotivation = Motivation;
                        }
                        else
                            Motivation = string.Empty;

                        AbitListString += string.Format("\t\t1.{0}. {1} {2} {3}\n", counter, v.ФИО, (LP == "Физическая культура" ? balls + ballToStr : ""), string.IsNullOrEmpty(Motivation) ? "" : ("\n\n\t\t" + tmpMotiv + "\n"));
                    }
                }

                if (!string.IsNullOrEmpty(curMotivation) && isCel)
                    AbitListString += "\n\t\t" + curMotivation + "\n";

                wd.SetFields("AbitList", AbitListString);

                if (basisId != "2" && formId == "1")//платникам и всем не-очникам стипендия не платится
                {
                    wd.SetFields("Stip", "\r\n2.    Назначить лицам, указанным в Приложении к настоящему приказу, государственную академическую стипендию в размере 487 рублей ежемесячно с 01.09.2014 по 31.12.2014.");
                }
            }
            catch (WordException we)
            {
                WinFormsServ.Error(we);
            }
            catch (Exception exc)
            {
                WinFormsServ.Error(exc);
            }
        }

        public static void PrintOrderReviewSPO(Guid protocolId, bool isRus)
        {
            try
            {
                WordDoc wd = new WordDoc(string.Format(@"{0}\EntryOrderList.dot", MainClass.dirTemplates));

                string formId;
                string facDat;

                string basisId;
                string educDoc = "";
                string basis = string.Empty;
                string form2 = string.Empty;

                string profession;
                string professionCode;
                int StudyLevelId;

                string naprspecRod = "";
                using (PriemEntities ctx = new PriemEntities())
                {

                    formId = (from protocol in ctx.Protocol
                              join studyForm in ctx.StudyForm on protocol.StudyFormId equals studyForm.Id
                              where protocol.Id == protocolId
                              select studyForm.Id).FirstOrDefault().ToString();

                    facDat = (from protocol in ctx.Protocol
                              join sP_Faculty in ctx.SP_Faculty on protocol.FacultyId equals sP_Faculty.Id
                              where protocol.Id == protocolId
                              select sP_Faculty.DatName).FirstOrDefault();

                    basisId = (from protocol in ctx.Protocol
                               join studyBasis in ctx.StudyBasis on protocol.StudyBasisId equals studyBasis.Id
                               where protocol.Id == protocolId
                               select studyBasis.Id).FirstOrDefault().ToString();

                    profession = (from entry in ctx.Entry
                                  join extentryView in ctx.extEntryView on entry.LicenseProgramId equals extentryView.LicenseProgramId
                                  where extentryView.Id == protocolId
                                  select entry.SP_LicenseProgram.Name).FirstOrDefault();

                    professionCode = (from entry in ctx.Entry
                                      join extentryView in ctx.extEntryView on entry.LicenseProgramId equals extentryView.LicenseProgramId
                                      where extentryView.Id == protocolId
                                      select entry.SP_LicenseProgram.Code).FirstOrDefault();

                    StudyLevelId = (from entry in ctx.Entry
                                    join extentryView in ctx.extEntryView on entry.LicenseProgramId equals extentryView.LicenseProgramId
                                    where extentryView.Id == protocolId
                                    select entry.SP_LicenseProgram.StudyLevelId).FirstOrDefault();

                    switch (basisId)
                    {
                        case "1":
                            basis = "за счет бюджетных ассигнований федерального бюджета";
                            educDoc = ", оригиналы документа установленного образца об образовании";
                            break;
                        case "2":
                            basis = "по договорам об образовании";
                            educDoc = ", договоры об образовании";
                            break;
                    }

                    switch (formId)
                    {
                        case "1":
                            form2 = "очной форме";
                            break;
                        case "2":
                            form2 = "очно-заочной (вечерней) форме";
                            break;
                    }

                    string bakspec = "", profspec = "";
                    string naprobProgRod = "образовательной программе";

                    bakspec = "магистратуры";
                    profspec = "профилю";
                    naprspecRod = "направлению подготовки";

                    int curRow = 5, counter = 0;
                    TableDoc td = null;

                    DateTime? protocolDate;
                    protocolDate = (DateTime?)(from protocol in ctx.OrderNumbers
                                               where protocol.ProtocolId == protocolId
                                               select protocol.ComissionDate).FirstOrDefault();

                    string protocolNum;
                    protocolNum = (from protocol in ctx.OrderNumbers
                                   where protocol.ProtocolId == protocolId
                                   select protocol.ComissionNumber).DefaultIfEmpty("НЕ УКАЗАН").FirstOrDefault();

                    string docNum = "НОМЕР";
                    string docDate = "ДАТА";
                    DateTime tempDate;
                    if (isRus)
                    {
                        docNum = (from orderNumbers in ctx.OrderNumbers
                                  where orderNumbers.ProtocolId == protocolId
                                  select orderNumbers.OrderNum).FirstOrDefault();

                        tempDate = (DateTime)(from orderNumbers in ctx.OrderNumbers where orderNumbers.ProtocolId == protocolId select orderNumbers.OrderDate).FirstOrDefault();

                        docDate = tempDate.ToShortDateString();
                    }
                    else
                    {
                        docNum = (from orderNumbers in ctx.OrderNumbers
                                  where orderNumbers.ProtocolId == protocolId
                                  select orderNumbers.OrderNumFor).FirstOrDefault();

                        tempDate = (DateTime)(from orderNumbers in ctx.OrderNumbers
                                              where orderNumbers.ProtocolId == protocolId
                                              select orderNumbers.OrderDateFor).FirstOrDefault();

                        docDate = tempDate.ToShortDateString();
                    }

                    var lst = (from extabit in ctx.extAbit
                               join extentryView in ctx.extEntryView on extabit.Id equals extentryView.AbiturientId
                               join extperson in ctx.extPerson on extabit.PersonId equals extperson.Id
                               join country in ctx.Country on extperson.NationalityId equals country.Id
                               join competition in ctx.Competition on extabit.CompetitionId equals competition.Id
                               join entryHeader in ctx.EntryHeader on extentryView.EntryHeaderId equals entryHeader.Id into entryHeader2
                               from entryHeader in entryHeader2.DefaultIfEmpty()
                               join extabitMarksSum in ctx.extAbitMarksSum on extabit.Id equals extabitMarksSum.Id into extabitMarksSum2
                               from extabitMarksSum in extabitMarksSum2.DefaultIfEmpty()
                               where extentryView.Id == protocolId && (isRus ? extperson.NationalityId == 1 : extperson.NationalityId != 1)
                               orderby extabit.ObrazProgramName, extabit.ProfileName, country.NameRod, entryHeader.SortNum, extabit.FIO
                               select new
                               {
                                   Id = extabit.Id,
                                   Рег_Номер = extabit.RegNum,
                                   Ид_номер = extabit.PersonNum,
                                   TotalSum = extabitMarksSum.TotalSum,
                                   ФИО = extabit.FIO,
                                   LicenseProgramCodeAndName = extabit.LicenseProgramCode + " " + extabit.LicenseProgramName,
                                   ProfileName = extabit.InnerEntryInEntryProfileName ?? extabit.ProfileName,
                                   ObrazProgramAdd = extabit.InnerEntryInEntryObrazProgramName,
                                   ObrazProgram = extabit.ObrazProgramName,
                                   ObrazProgramCryptAdd = extabit.InnerEntryInEntryObrazProgramCrypt,
                                   ObrazProgramCrypt = extabit.ObrazProgramCrypt,
                                   ObrazProgramId = extabit.ObrazProgramId,
                                   EntryHeaderId = entryHeader.Id,
                                   EntryHeaderName = entryHeader.Name,
                                   NameRod = country.NameRod,
                                   extentryView.SignerName,
                                   extentryView.SignerPosition,
                                   extabit.CompetitionId
                               }).ToList().Distinct().Select(x =>
                                   new
                                   {
                                       Id = x.Id,
                                       Рег_Номер = x.Рег_Номер,
                                       Ид_номер = x.Ид_номер,
                                       TotalSum = x.TotalSum.ToString(),
                                       ФИО = x.ФИО,
                                       LicenseProgramCodeAndName = x.LicenseProgramCodeAndName,
                                       ProfileName = x.ProfileName,
                                       ObrazProgram = (x.ObrazProgramAdd ?? x.ObrazProgram).Replace("(очно-заочная)", "").Replace(" ВВ", ""),
                                       ObrazProgramCrypt = x.ObrazProgramCryptAdd ?? x.ObrazProgramCrypt,
                                       ObrazProgramId = x.ObrazProgramId,
                                       EntryHeaderId = x.EntryHeaderId,
                                       EntryHeaderName = x.EntryHeaderName,
                                       NameRod = x.NameRod,
                                       x.SignerName,
                                       x.SignerPosition,
                                       CompetitionId = x.CompetitionId
                                   }
                               );

                    foreach (var v in lst)
                    {
                        if (v.CompetitionId == 11 || v.CompetitionId == 12)
                            wd.InsertAutoTextInEnd("выпискаКРЫМ", true);
                        else
                            wd.InsertAutoTextInEnd("выписка", true);

                        wd.GetLastFields(14);
                        td = wd.Tables[counter];

                        wd.SetFields("Граждан", isRus ? "граждан РФ" : "иностранных граждан");
                        wd.SetFields("Граждан2", isRus ? "граждан Российской Федерации" : "");
                        wd.SetFields("Стипендия", (basisId == "2" || formId == "2") ? "" : "и назначении стипендии");
                        wd.SetFields("Форма2", form2);
                        wd.SetFields("Основа2", basis);
                        // wd.SetFields("НапрСпец", string.Format(" {0} {1} «{2}»", naprspecRod, professionCode, profession));
                        wd.SetFields("ПриказДата", docDate);
                        wd.SetFields("ПриказНомер", "№ " + docNum);
                        wd.SetFields("SignerName", v.SignerName);
                        wd.SetFields("SignerPosition", v.SignerPosition);
                        //SetFields("ДатаПечати", DateTime.Now.Date.ToShortDateString());


                        wd.SetFields("Основание", educDoc);
                        if (protocolDate.HasValue)
                            wd.SetFields("ДатаОснования", ((DateTime)protocolDate).ToShortDateString());
                        else
                            wd.SetFields("ДатаОснования", "ДАТА");
                        wd.SetFields("НомерОснования", protocolNum ?? "НОМЕР");


                        string curLPHeader = "-";
                        string curSpez = "-";
                        string curObProg = "-";
                        string curHeader = "-";
                        string curCountry = "-";

                        ++counter;

                        string LP = v.LicenseProgramCodeAndName;
                        if (curLPHeader != LP)
                        {
                            td.AddRow(1);
                            curRow++;
                            td[0, curRow] = string.Format("{2}\tпо {0} \"{1}\"", naprspecRod, LP, curObProg == "-" ? "" : "\r\n");
                            curLPHeader = LP;
                        }

                        string obProg = v.ObrazProgram;
                        string obProgCode = v.ObrazProgramCrypt;
                        if (obProg != curObProg)
                        {
                            if (!string.IsNullOrEmpty(obProg))
                            {
                                td.AddRow(1);
                                curRow++;
                                td[0, curRow] = string.Format("\tпо {0} {1} \"{2}\"", naprobProgRod, obProgCode, obProg);
                            }

                            string spez = v.ProfileName;

                            if (!string.IsNullOrEmpty(spez) && spez != "нет")
                            {
                                td.AddRow(1);
                                curRow++;
                                td[0, curRow] = string.Format("\t {0} \"{1}\"", profspec, spez);
                            }

                            curSpez = spez;

                            curObProg = obProg;
                        }
                        else
                        {
                            string spez = v.ProfileName;
                            if (spez != curSpez)
                            {
                                if (!string.IsNullOrEmpty(spez) && spez != "нет")
                                {
                                    td.AddRow(1);
                                    curRow++;
                                    td[0, curRow] = string.Format("\t {0} \"{1}\"", profspec, spez);
                                }

                                curSpez = spez;
                            }
                        }

                        if (!isRus)
                        {
                            string country = v.NameRod;
                            if (country != curCountry)
                            {
                                td.AddRow(1);
                                curRow++;
                                td[0, curRow] = string.Format("\r\n граждан {0}:", country);

                                curCountry = country;
                            }
                        }

                        string header = v.EntryHeaderName;
                        if (header != curHeader)
                        {
                            td.AddRow(1);
                            curRow++;
                            td[0, curRow] = string.Format("\t{0}:", header);

                            curHeader = header;
                        }

                        string balls = v.TotalSum;
                        string ballToStr = " балл";

                        if (balls.Length == 0)
                            ballToStr = "";
                        else if (balls.EndsWith("1"))
                        {
                            if (balls.EndsWith("11"))
                                ballToStr += "ов";
                            else
                                ballToStr += "";
                        }
                        else if (balls.EndsWith("2") || balls.EndsWith("3") || balls.EndsWith("4"))
                        {
                            if (balls.EndsWith("12") || balls.EndsWith("13") || balls.EndsWith("14"))
                                ballToStr += "ов";
                            else
                                ballToStr += "а";
                        }
                        else
                            ballToStr += "ов";

                        td.AddRow(1);
                        curRow++;
                        td[0, curRow] = string.Format("\t\t{0} {1}", v.ФИО, balls + ballToStr);

                        if (basisId == "1" && formId == "1")
                        {
                            td.AddRow(1);
                            curRow++;
                            td[0, curRow] = "\r\n2.      Назначить указанным лицам государственную академическую стипендию в размере 487 рубля ежемесячно до 31 декабря 2014 г.";
                        }
                    }
                }

            }
            catch (WordException we)
            {
                WinFormsServ.Error(we);
            }
            catch (Exception exc)
            {
                WinFormsServ.Error(exc);
            }
        }
    }
}

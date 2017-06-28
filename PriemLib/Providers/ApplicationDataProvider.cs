using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PriemLib
{
    public static class ApplicationDataProvider
    {
        public static void ChangeHasOriginalsDestination(Guid AbiturientId, Guid? TargetAbiturientId)
        {
            using (PriemEntities context = new PriemEntities())
            {
                Guid PersonId = context.Abiturient.Where(x => x.Id == AbiturientId).Select(x => x.PersonId)
                    .DefaultIfEmpty(Guid.Empty).First();

                if (PersonId == Guid.Empty)
                    throw new KeyNotFoundException("Не найдено записи исходного конкурса!");

                if (!TargetAbiturientId.HasValue)
                {
                    bool isForeigners = MainClass.dbType == PriemType.PriemForeigners;

                    TargetAbiturientId = context.Abiturient
                        .Where(x => x.PersonId == PersonId && !x.BackDoc && !x.NotEnabled && !x.HasOriginals && x.Id != AbiturientId
                            && x.Entry.IsForeign == isForeigners && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId))
                        .Select(x => new { x.Id, x.Priority })
                        .ToList()
                        .OrderBy(x => x.Priority)
                        .Select(x => x.Id)
                        .DefaultIfEmpty(Guid.Empty)
                        .First();
                }

                if (TargetAbiturientId == Guid.Empty)
                    throw new KeyNotFoundException("Не найдено доступного заявления для перемещения оригиналов!");

                if (context.Abiturient.Where(x => x.Id == TargetAbiturientId).Select(x => x.PersonId).DefaultIfEmpty(Guid.Empty).First() == PersonId)
                {
                    var AbSource = context.Abiturient.Where(x => x.Id == AbiturientId).FirstOrDefault();
                    var AbDest = context.Abiturient.Where(x => x.Id == TargetAbiturientId).FirstOrDefault();

                    if (!AbSource.HasOriginals)
                        throw new InvalidOperationException("Отстутствуют оригиналы на заявлении!");

                    AbSource.HasOriginals = false;
                    AbDest.HasOriginals = true;

                    context.SaveChanges();
                }
                else
                    throw new KeyNotFoundException("Не совпадают записи абитуриентов исходного и конечного заявления!");
            }
        }

        public static string CheckPersonPriveleges(Guid? _personId)
        {
            using (PriemEntities context = new PriemEntities())
            {
                int PersonPrivelege = (from x in context.extPerson
                                       where x.Id == _personId
                                       select x.Privileges).First();
                var Priveleges =
                    (from x in context.Privilege
                     select new { x.Number, x.Maska, x.Name }).ToList();

                var BenefitDocs
                    = (from x in context.BenefitDocumentType
                       select new { x.Id, x.Name }).ToList();

                List<string> PersonPriveleges = new List<string>();
                foreach (var x in Priveleges.OrderByDescending(x => x.Maska))
                {
                    if (PersonPrivelege >= x.Maska)
                    {
                        PersonPriveleges.Add(x.Name);
                        PersonPrivelege -= x.Maska;
                    }
                }

                List<List<int>> lstDocType = new List<List<int>>();

                string s = "";

                var PersonDocs = (from x in context.PersonBenefitDocument
                                  where x.PersonId == _personId
                                  select x.BenefitDocumentTypeId).ToList();

                if (PersonPriveleges.Count() == 0)
                {
                    return "Для предоставления конкурса 'в/к' отсутствуют льготы.";
                }
                else
                {
                    if (PersonPriveleges.Where(x => x.Contains("сирота")).Count() > 0)
                    {
                        lstDocType.Add(new List<int>() { 5 });
                    }
                    if (PersonPriveleges.Where(x => x.Contains("инвалид")).Count() > 0)
                    {
                        lstDocType.Add(new List<int>() { 1, 2 });
                        //lstDocType.Add(new List<int>() { 3 });
                    }
                    if (PersonPriveleges.Where(x => x.Contains("сирота")).Count() == 0 && PersonPriveleges.Where(x => x.Contains("инвалид")).Count() == 0)
                    {
                        lstDocType.Add(new List<int>() { 1, 2, 3, 5 });
                    }
                }

                foreach (var lst in lstDocType)
                {
                    if (PersonDocs.Where(x => lst.Contains(x)).Count() == 0)
                    {
                        List<string> benefitDocNames = new List<string>();
                        foreach (int docId in lst)
                        {
                            string docName = BenefitDocs.Where(x => x.Id == docId).Select(x => x.Name).First().ToString();
                            benefitDocNames.Add(docName);
                        }

                        s += benefitDocNames.Distinct().Aggregate((x, tail) => x + " или " + tail) + ";\n";
                    }
                }

                if (!String.IsNullOrEmpty(s))
                {
                    string sub = "";
                    sub = "Для предоставления льгот";
                    if (PersonPriveleges.Count() == 1)
                        sub += "ы";

                    s = sub + " необходимо ввести в персональной карточке абитуриента следующие документы:\n" + s;
                }

                return s;
            }
        }

        public static List<ApplicationData> GetAppData(Guid PersonId)
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<ApplicationData> abitList =
                    (from x in context.Abiturient
                     join Ent in context.Entry on x.EntryId equals Ent.Id
                     where MainClass.lstStudyLevelGroupId.Contains(Ent.StudyLevel.StudyLevelGroup.Id)
                     //&& x.Entry.IsForeign == false
                     && x.PersonId == PersonId
                     && x.BackDoc == false
                     select new ApplicationData
                     {
                         ApplicationId = x.Id,
                         PersonId = x.PersonId,
                         Barcode = x.Barcode,
                         FacultyName = Ent.SP_Faculty.Name,
                         LicenseProgramName = Ent.SP_LicenseProgram.Name,
                         LicenseProgramCode = Ent.SP_LicenseProgram.Code,
                         ObrazProgramCrypt = Ent.StudyLevel.Acronym + "." + Ent.SP_ObrazProgram.Number + "." + MainClass.sPriemYear,
                         ObrazProgramName = Ent.SP_ObrazProgram.Name,
                         ProfileName = Ent.SP_Profile.Name,
                         StudyFormId = Ent.StudyFormId,
                         StudyFormName = Ent.StudyForm.Name,
                         StudyBasisId = Ent.StudyBasisId,
                         StudyLevelId = Ent.StudyLevelId,
                         Priority = x.Priority,
                         IsForeign = x.Entry.IsForeign,
                         ComissionId = Ent.CommissionId,
                         ComissionAddress = Ent.CommissionId.HasValue ? Ent.Comission.Name : ""
                     }).OrderBy(x => x.Priority).ToList();

                return abitList;
            }
        }
        public static List<ShortAppcationDetails> GetAppProfileList(Guid PersonId)
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<ShortAppcationDetails> abitProfileList =
                    (from x in context.Abiturient
                     join Ad in context.extApplicationDetails on x.Id equals Ad.ApplicationId
                     join Ent in context.Entry on x.EntryId equals Ent.Id
                     where MainClass.lstStudyLevelGroupId.Contains(Ent.StudyLevel.StudyLevelGroup.Id)
                     && x.Entry.IsForeign == (MainClass.dbType == PriemType.PriemForeigners)
                     && x.PersonId == PersonId
                     && x.BackDoc == false
                     select new ShortAppcationDetails()
                     {
                         ApplicationId = x.Id,
                         Priority = Ad.InnerEntryInEntryPriority,
                         ObrazProgramName = Ad.ObrazProgramCrypt + " " + Ad.ObrazProgramName,
                         ProfileName = Ad.ProfileName,
                     }).Distinct().ToList();

                return abitProfileList;
            }
        }
    }

    public class ApplicationData
    {
        public Guid ApplicationId { get; set; }
        public Guid PersonId { get; set; }
        public Guid EntryId { get; set; }
        public int? Barcode { get; set; }
        public int? Priority { get; set; }

        public bool IsForeign { get; set; }

        public int FacultyId { get; set; }
        public string FacultyName { get; set; }

        public int StudyLevelId { get; set; }
        public string StudyLevelName { get; set; }

        public int LicenseProgramId { get; set; }
        public string LicenseProgramName { get; set; }
        public string LicenseProgramCode { get; set; }

        public int ObrazProgramId { get; set; }
        public string ObrazProgramName { get; set; }
        public string ObrazProgramCrypt { get; set; }

        public int ProfileId { get; set; }
        public string ProfileName { get; set; }

        public int StudyFormId { get; set; }
        public string StudyFormName { get; set; }

        public int StudyBasisId { get; set; }
        public string StudyBasisName { get; set; }

        public int? ComissionId { get; set; }
        public string ComissionAddress { get; set; }
    }
}

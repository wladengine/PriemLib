﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data;
using EducServLib;

namespace PriemLib
{
    public static class Exams
    {
        public static IEnumerable<extExamInEntry> GetExamsWithFilters(PriemEntities context, List<int> lstStudyLevelGroupId, int? facultyId, int? licenseProgramId, int? obrazProgramId, int? profileId, int? stFormId, int? stBasisId, bool? isSecond, bool? isReduced, bool? isParallel)
        {
            List<extExamInEntry> lst = new List<extExamInEntry>();
            foreach (int iStudyLevelGroupId in lstStudyLevelGroupId)
            {
                lst.AddRange(GetExamsWithFilters(context, iStudyLevelGroupId, facultyId, licenseProgramId, obrazProgramId, profileId, stFormId, stBasisId, isSecond, isReduced, isParallel));
            }

            return lst;
        }
        public static IEnumerable<extExamInEntry> GetExamsWithFiltersStudyLevelId(PriemEntities context, List<int> lstStudyLevelId, int? facultyId, int? licenseProgramId, int? obrazProgramId, int? profileId, int? stFormId, int? stBasisId, bool? isSecond, bool? isReduced, bool? isParallel)
        {
            List<extExamInEntry> lst = new List<extExamInEntry>();
            foreach (int ilstStudyLevelId in lstStudyLevelId)
            {
                lst.AddRange(GetExamsWithFilters(context, ilstStudyLevelId, facultyId, licenseProgramId, obrazProgramId, profileId, stFormId, stBasisId, isSecond, isReduced, isParallel));
            }
            return lst;
        }  
 
        public static IEnumerable<extExamInEntry> GetExamsWithFilters(PriemEntities context, int iStudyLevelGroupId, int? facultyId, int? licenseProgramId, int? obrazProgramId, int? profileId, int? stFormId, int? stBasisId, bool? isSecond, bool? isReduced, bool? isParallel, bool bMakeAllExams = false)
        {
            List<Guid> lst = 
                (from x in context.ExamInEntryBlock
                 where x.ParentExamInEntryBlockId != null
                 select x.ParentExamInEntryBlockId.Value).ToList();

            IEnumerable<extExamInEntry> exams =
                (from Unit in context.ExamInEntryBlockUnit
                join Ex in context.Exam on Unit.ExamId equals Ex.Id
                join ExName in context.ExamName on Ex.ExamNameId equals ExName.Id
                join Block in context.ExamInEntryBlock on Unit.ExamInEntryBlockId equals Block.Id
                join extEnt in context.extEntry on Block.EntryId equals extEnt.Id
                where extEnt.StudyLevelGroupId == iStudyLevelGroupId && (bMakeAllExams ? true : !lst.Contains(Block.Id))
                select new
                {
                    EgeMin = Unit.EgeMin,
                    EntryId  = Block.EntryId,
                    ExamDefName = ExName.NamePad,
                    ExamId = Unit.ExamId,
                    ExamName = ExName.Name,
                    ExamNameId= Ex.ExamNameId,
                    FacultyId = extEnt.FacultyId,
                    IsAdditional = Ex.IsAdditional,
                    IsCrimea = Block.IsCrimea,
                    IsForeign = extEnt.IsForeign,
                    IsGosLine = Block.IsGosLine,
                    IsParallel = extEnt.IsParallel,
                    IsProfil = Block.IsProfil,
                    IsReduced = extEnt.IsReduced,
                    IsSecond = extEnt.IsSecond,
                    LicenseProgramId = extEnt.LicenseProgramId,
                    ObrazProgramId = extEnt.ObrazProgramId,
                    OrderNumber = Block.OrderNumber,
                    ProfileId = extEnt.ProfileId,
                    StudyBasisId = extEnt.StudyBasisId,
                    StudyFormId = extEnt.StudyFormId,
                    StudyLevelGroupId = extEnt.StudyLevelGroupId,
                    StudyLevelId = extEnt.StudyLevelId
                }).ToList()
                .Select(x => new extExamInEntry()
                {
                    EgeMin = (int?)x.EgeMin,
                    EntryId  = x.EntryId,
                    ExamDefName = x.ExamDefName,
                    ExamId = x.ExamId,
                    ExamName = x.ExamName,
                    ExamNameId= x.ExamNameId,
                    FacultyId = x.FacultyId,
                    IsAdditional = x.IsAdditional,
                    IsCrimea = x.IsCrimea,
                    IsForeign = x.IsForeign,
                    IsGosLine = x.IsGosLine,
                    IsParallel = x.IsParallel,
                    IsProfil = x.IsProfil,
                    IsReduced = x.IsReduced,
                    IsSecond = x.IsSecond,
                    LicenseProgramId = x.LicenseProgramId,
                    ObrazProgramId = x.ObrazProgramId,
                    OrderNumber = x.OrderNumber,
                    ProfileId = x.ProfileId,
                    StudyBasisId = x.StudyBasisId,
                    StudyFormId = x.StudyFormId,
                    StudyLevelGroupId = x.StudyLevelGroupId,
                    StudyLevelId = x.StudyLevelId
                });

            if (facultyId != null)
                exams = exams.Where(c => c.FacultyId == facultyId);
            if (licenseProgramId != null)
                exams = exams.Where(c => c.LicenseProgramId == licenseProgramId);
            if (obrazProgramId != null)
                exams = exams.Where(c => c.ObrazProgramId == obrazProgramId);
            if (profileId != null)
                exams = exams.Where(c => c.ProfileId == profileId);
            if (stFormId != null)
                exams = exams.Where(c => c.StudyFormId == stFormId);
            if (stBasisId != null)
                exams = exams.Where(c => c.StudyBasisId == stBasisId);
            if (isSecond != null)
                exams = exams.Where(c => c.IsSecond == isSecond);
            if (isReduced != null)
                exams = exams.Where(c => c.IsReduced == isReduced);
            if (isParallel != null)
                exams = exams.Where(c => c.IsParallel == isParallel);

            return exams.OrderBy(c=>c.ExamName);           
        }
        public static IEnumerable<extExamInEntry> GetExamsWithFiltersStudyLevelId(PriemEntities context, int iStudyLevelId, int? facultyId, int? licenseProgramId, int? obrazProgramId, int? profileId, int? stFormId, int? stBasisId, bool? isSecond, bool? isReduced, bool? isParallel)
        {
            var lst = (from x in context.ExamInEntryBlock
                       select x.ParentExamInEntryBlockId).ToList();

            IEnumerable<extExamInEntry> exams =
                (from Unit in context.ExamInEntryBlockUnit
                 join Ex in context.Exam on Unit.ExamId equals Ex.Id
                 join ExName in context.ExamName on Ex.ExamNameId equals ExName.Id
                 join Block in context.ExamInEntryBlock on Unit.ExamInEntryBlockId equals Block.Id
                 join extEnt in context.extEntry on Block.EntryId equals extEnt.Id
                 where extEnt.StudyLevelId == iStudyLevelId && !lst.Contains(Block.Id)
                 select new
                 {
                     EgeMin = Unit.EgeMin,
                     EntryId = Block.EntryId,
                     ExamDefName = ExName.NamePad,
                     ExamId = Unit.ExamId,
                     ExamName = ExName.Name,
                     ExamNameId = Ex.ExamNameId,
                     FacultyId = extEnt.FacultyId,
                     IsAdditional = Ex.IsAdditional,
                     IsCrimea = Block.IsCrimea,
                     IsForeign = extEnt.IsForeign,
                     IsGosLine = Block.IsGosLine,
                     IsParallel = extEnt.IsParallel,
                     IsProfil = Block.IsProfil,
                     IsReduced = extEnt.IsReduced,
                     IsSecond = extEnt.IsSecond,
                     LicenseProgramId = extEnt.LicenseProgramId,
                     ObrazProgramId = extEnt.ObrazProgramId,
                     OrderNumber = Block.OrderNumber,
                     ProfileId = extEnt.ProfileId,
                     StudyBasisId = extEnt.StudyBasisId,
                     StudyFormId = extEnt.StudyFormId,
                     StudyLevelGroupId = extEnt.StudyLevelGroupId,
                     StudyLevelId = extEnt.StudyLevelId
                 }).ToList()
                .Select(x => new extExamInEntry()
                {
                    EgeMin = (int?)x.EgeMin,
                    EntryId = x.EntryId,
                    ExamDefName = x.ExamDefName,
                    ExamId = x.ExamId,
                    ExamName = x.ExamName,
                    ExamNameId = x.ExamNameId,
                    FacultyId = x.FacultyId,
                    IsAdditional = x.IsAdditional,
                    IsCrimea = x.IsCrimea,
                    IsForeign = x.IsForeign,
                    IsGosLine = x.IsGosLine,
                    IsParallel = x.IsParallel,
                    IsProfil = x.IsProfil,
                    IsReduced = x.IsReduced,
                    IsSecond = x.IsSecond,
                    LicenseProgramId = x.LicenseProgramId,
                    ObrazProgramId = x.ObrazProgramId,
                    OrderNumber = x.OrderNumber,
                    ProfileId = x.ProfileId,
                    StudyBasisId = x.StudyBasisId,
                    StudyFormId = x.StudyFormId,
                    StudyLevelGroupId = x.StudyLevelGroupId,
                    StudyLevelId = x.StudyLevelId
                });

            if (facultyId != null)
                exams = exams.Where(c => c.FacultyId == facultyId);
            if (licenseProgramId != null)
                exams = exams.Where(c => c.LicenseProgramId == licenseProgramId);
            if (obrazProgramId != null)
                exams = exams.Where(c => c.ObrazProgramId == obrazProgramId);
            if (profileId != null)
                exams = exams.Where(c => c.ProfileId == profileId);
            if (stFormId != null)
                exams = exams.Where(c => c.StudyFormId == stFormId);
            if (stBasisId != null)
                exams = exams.Where(c => c.StudyBasisId == stBasisId);
            if (isSecond != null)
                exams = exams.Where(c => c.IsSecond == isSecond);
            if (isReduced != null)
                exams = exams.Where(c => c.IsReduced == isReduced);
            if (isParallel != null)
                exams = exams.Where(c => c.IsParallel == isParallel);

            return exams.OrderBy(c => c.ExamName);
        }        

        public static string GetFilterForNotAddExam(int? examId, int? facultyId)
        {
            //всех, кроме 1 курса эти ограничения не касаются
            if (MainClass.dbType != PriemType.Priem)
                return "";

            string flt_privil = string.Empty;
            string flt_ssuz = string.Empty;
            string flt_underPrevYear = string.Empty;
            string flt_foreignEduc = string.Empty;
            string flt_second = string.Empty;
            string flt_hasEge = string.Empty;

            /* Ситуация на 2011 год:
             * Попадают в правый список все, у кого нет оценки по ЕГЭ в этом году
             * Если у чела вне конкурса, то его тоже надо выводить (зачем???)
             * Остальных не надо
             */

            //flt_privil = " (Person.Privileges & 512 > 0 OR Person.Privileges & 32 > 0 OR Person.CountryId > 1) ";
            //flt_ssuz = " OR Person.SchoolTypeId = 2 OR Person.SchoolTypeId = 5 ";
            //flt_underPrevYear = string.Format(" OR (Person.SchoolExitYear < {0} AND qAbiturient.StudyFormId IN (2,3)) ", (DateTime.Now.Year - 1).ToString());
            //flt_foreignEduc = " OR Person.CountryEducId > 1 ";
            //flt_second = " OR qAbiturient.ListenerTypeId > 0 ";
            //flt_hasEge = string.Format(" OR Person.Id NOT IN (SELECT PersonId FROM EgeCertificate LEFT JOIN EgeMark ON egeMArk.EgeCertificateId = EgeCertificate.Id LEFT JOIN EgeToExam ON EgeMArk.EgeExamNameId = EgeToExam.EgeExamNameId LEFT JOIN ExamInProgram ON EgeToExam.ExamNameId = ExamInProgram.ExamNameId WHERE ExamInProgram.Id IN (SELECT Id FROM extExamInProgram WHERE ExamNameId = {0} AND FacultyId = {1}))", examId, facultyId);

            //return " AND (" + flt_privil + flt_ssuz + flt_underPrevYear + flt_foreignEduc + flt_second + flt_hasEge + ")";

            //У которых нет сданного предмета ЕГЭ за последние 4 года
            flt_hasEge = string.Format(@" extPerson.Id NOT IN 
(
    SELECT PersonId 
    FROM ed.extEgeMark
    LEFT JOIN ed.EgeToExam ON extEgeMark.EgeExamNameId = EgeToExam.EgeExamNameId 
    LEFT JOIN ed.extExamInEntry ON EgeToExam.ExamId = extExamInEntry.ExamId 
    WHERE extExamInEntry.ExamId = {0} AND extExamInEntry.FacultyId = {1}
    AND extEgeMark.[Year] > {2}
)", examId, facultyId, MainClass.iPriemYear - 4);
            //flt_hasEge = " 1=1 ";
            //инвалиды, иностранцы, люди со средним проф образованием
            flt_privil = " OR extPerson.Privileges & 512 > 0 OR extPerson.Privileges & 32 > 0 OR extPerson.NationalityId <> 1 OR extPerson.EgeInSPbgu = 1";
            return " AND (" + flt_hasEge + flt_privil + " ) ";
        }       

        public static string GetExamInEntryId(string examId, string facultyId, string licenseProgramId, string obrazProgramId, string profileId, string stFormId, string stBasisId, bool? isSecond, bool? isParallel, bool? isReduced)
        {
            string fltStLevelGroup = " AND ed.qEntry.StudyLevelGroupId IN (" + Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId) + ")";
            string fltFaculty = facultyId == null || facultyId == string.Empty || facultyId == "0" ? "" : " AND ed.qEntry.FacultyId = " + facultyId;
            string fltProfession = licenseProgramId == null || licenseProgramId == string.Empty || licenseProgramId == "0" ? "" : " AND ed.qEntry.LicenseProgramId = " + licenseProgramId;
            string fltObrazProgram = obrazProgramId == null || obrazProgramId == string.Empty || obrazProgramId == "0" ? "" : " AND ed.qEntry.ObrazProgramId = " + obrazProgramId;
            string fltSpecialization = profileId == null || profileId == string.Empty || profileId == "0" ? " AND ed.qEntry.ProfileId IS NULL " : " AND ed.qEntry.ProfileId = '" + profileId + "'";
            string fltStudyForm = stFormId == null || stFormId == string.Empty || stFormId == "0" ? "" : " AND ed.qEntry.StudyFormId = " + stFormId;
            string fltStudyBasis = stBasisId == null || stBasisId == string.Empty || stBasisId == "0" ? "" : " AND ed.qEntry.StudyBasisId = " + stBasisId;

            string fltIsSecond = isSecond == null ? "" : " AND ed.qEntry.IsSecond = " + (isSecond.Value ? "1" : "0");
            string fltIsParallel = isParallel == null ? "" : " AND ed.qEntry.IsParallel = " + (isParallel.Value ? "1" : "0");
            string fltIsReduced = isReduced == null ? "" : " AND ed.qEntry.IsReduced = " + (isReduced.Value ? "1" : "0");


            string entryId = MainClass.Bdc.GetStringValue(string.Format("SELECT DISTINCT ed.qEntry.Id FROM ed.qEntry WHERE {9} {0} {1} {2} {3} {4} {5} {6} {7} {8}", fltFaculty, fltProfession, fltObrazProgram, fltSpecialization, fltStudyForm, fltStudyBasis, fltIsSecond, fltIsParallel, fltIsReduced, fltStLevelGroup));

            return GetExamInEntryId(examId, entryId);
        }

        public static string GetExamInEntryId(string examId, string entryId)
        {            
            if (string.IsNullOrEmpty(entryId))
                return null;

            string sExamInEntryId = MainClass.Bdc.GetStringValue(string.Format("SELECT DISTINCT extExamInEntry.Id FROM ed.extExamInEntry WHERE ExamId = {0} AND EntryId = '{1}' ", examId, entryId));
            if (string.IsNullOrEmpty(sExamInEntryId))
            {
                sExamInEntryId = MainClass.Bdc.GetStringValue(string.Format(@"SELECT extExamInEntry.Id 
	FROM ed.extExamInEntry 
	INNER JOIN ed.Exam ON Exam.Id = extExamInEntry.ExamId
	WHERE EntryId = '{1}' AND Exam.AlternateExamId = {0}", examId, entryId));
            }

            return sExamInEntryId;
        }

        public static List<string> GetExamIdsInEntry(string facultyId, string licenseProgramId, string obrazProgramId, string profileId, string stFormId, string stBasisId, bool isSecond, bool isParallel, bool isReduced)
        {
            string fltStLevelGroup = " AND ed.qEntry.StudyLevelGroupId IN (" + Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId) + ")";
            string fltSpecialization = (profileId == null || profileId == "") ? " AND ed.qEntry.ProfileId IS NULL" : " AND ed.qEntry.ProfileId = '" + profileId + "'";
            string entryId = MainClass.Bdc.GetStringValue(string.Format("SELECT DISTINCT ed.qEntry.Id FROM ed.qEntry WHERE {9} AND FacultyId = {0} AND LicenseProgramId = {1} AND ObrazProgramId = {2} {3} AND StudyFormId = {4} AND StudyBasisId = {5} AND IsSecond = {6} AND IsParallel = {7} AND IsReduced = {8}", facultyId, licenseProgramId, obrazProgramId, fltSpecialization, stFormId, stBasisId, (isSecond ? "1" : "0"), (isParallel ? "1" : "0"), (isReduced ? "1" : "0"), fltStLevelGroup));

            return GetExamIdsInEntry(entryId);
        }

        public static List<string> GetExamIdsInEntry(string entryId)
        {  
            List<string> lst = new List<string>();
            if (string.IsNullOrEmpty(entryId))
                return lst;

            DataSet ds = MainClass.Bdc.GetDataSet(string.Format(@"SELECT extExamInEntry.ExamId
FROM ed.extExamInEntry 
WHERE extExamInEntry.EntryId = '{0}'
UNION
SELECT Exam.AlternateExamId
FROM ed.extExamInEntry 
INNER JOIN ed.Exam ON Exam.Id = extExamInEntry.ExamId
WHERE extExamInEntry.EntryId = '{0}'
AND Exam.AlternateExamId IS NOT NULL", entryId));
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                lst.Add(dr["ExamId"].ToString());
            }
            return lst;
        }
    }
}

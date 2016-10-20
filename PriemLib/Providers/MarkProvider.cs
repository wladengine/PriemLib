using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace PriemLib
{
    public static class MarkProvider
    {
        public static void LoadExamsResultsToParentExam()
        {
            int insertCount = 0;
            int missCount = 0;
            
            using (PriemEntities context = new PriemEntities())
            {
                var MarksData =
                    (from Mrk in context.Mark
                     join abb in context.qAbitAll on Mrk.AbiturientId equals abb.Id
                     join EIE in context.extExamInEntry on Mrk.ExamInEntryBlockUnitId equals EIE.Id
                     join PAR in context.extExamInEntry on EIE.ParentExamInEntryBlockId equals PAR.ExamInEntryBlockId
                     where MainClass.lstStudyLevelGroupId.Contains(abb.StudyLevelGroupId)
                     && EIE.ParentExamInEntryBlockId != null
                     && EIE.FacultyId != 3
                     select new
                     {
                         Mrk.AbiturientId,
                         PAR.Id,
                         Mrk.Value,
                         EIE.ExamName
                     }).ToList();

                var MarksExistsVals =
                    (from Mrk in context.Mark
                     join abb in context.qAbitAll on Mrk.AbiturientId equals abb.Id
                     join EIE in context.extExamInEntry on Mrk.ExamInEntryBlockUnitId equals EIE.Id
                     where MainClass.lstStudyLevelGroupId.Contains(abb.StudyLevelGroupId)
                     && EIE.ParentExamInEntryBlockId == null
                     select new
                     {
                         Mrk.AbiturientId,
                         EIE.Id,
                     }).ToList();

                var Abits = MarksData.Select(x => new { x.AbiturientId, x.Id, x.ExamName }).Distinct().ToList();

                using (TransactionScope tran = new TransactionScope())
                {
                    foreach (var MrkEnt in Abits)
                    {
                        var MarksForAbit = MarksData.Where(x => x.AbiturientId == MrkEnt.AbiturientId && x.Id == MrkEnt.Id).ToList();
                        //decimal sum = (MarksData.Where(x => x.AbiturientId == MrkEnt.AbiturientId && x.Id == MrkEnt.Id && (x.ExamName.Contains('1') || x.ExamName.Contains('2'))).Select(x => x.Value).Sum() / 2) +
                        //    MarksData.Where(x => x.AbiturientId == MrkEnt.AbiturientId && x.Id == MrkEnt.Id && x.ExamName.Contains('3')).Select(x => x.Value).Sum();
                        decimal sum = MarksData.Where(x => x.AbiturientId == MrkEnt.AbiturientId && x.Id == MrkEnt.Id).Select(x => x.Value).Sum();

                        if (MarksForAbit.Where(x => x.ExamName.Contains('1')).Select(x => x.Value).Sum() == 0)
                            sum = 0;

                        if (sum > 100)
                            sum = 100;

                        int cnt = MarksExistsVals.Where(x => x.AbiturientId == MrkEnt.AbiturientId && x.Id == MrkEnt.Id).Count();
                        if (cnt == 0)
                        {
                            context.Mark_Insert(MrkEnt.AbiturientId, MrkEnt.Id, sum, DateTime.Now, false, false, false, null, null, null);
                            insertCount++;
                        }
                        else
                        {
                            context.Mark_updateByAbVedId(MrkEnt.AbiturientId, MrkEnt.Id, sum, DateTime.Now, null);
                        }
                            //missCount++;
                    }

                    tran.Complete();
                }
            }

            MessageBox.Show("Загружено: " + insertCount + "; Пропущено (уже есть оценка):" + missCount);
        }

        public static void UpdateFiveGradeMarks()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lstMarks =
                    (from Mrk in context.Mark
                     join Ex in context.extExamInEntry on Mrk.ExamInEntryBlockUnitId equals Ex.Id
                     join Bl in context.ExamInEntryBlock on Ex.ExamInEntryBlockId equals Bl.Id
                     where MainClass.lstStudyLevelGroupId.Contains(Ex.StudyLevelGroupId)
                     && Bl.Grade3MarkMax != null
                     && Bl.Grade3MarkMin != null
                     && Bl.Grade4MarkMax != null
                     && Bl.Grade4MarkMin != null
                     && Bl.Grade5MarkMax != null
                     && Bl.Grade5MarkMin != null
                     select new
                     {
                         Mrk.Id,
                         Mrk.Value,
                         Bl.Grade3MarkMax,
                         Bl.Grade3MarkMin,
                         Bl.Grade4MarkMax,
                         Bl.Grade4MarkMin,
                         Bl.Grade5MarkMax,
                         Bl.Grade5MarkMin,
                     }).ToList();

                using (TransactionScope tran = new TransactionScope())
                {
                    foreach (var Mark in lstMarks)
                    {
                        int iFiveGradeVal = 2;
                        if (Mark.Value >= Mark.Grade5MarkMin)
                            iFiveGradeVal = 5;
                        else if (Mark.Value >= Mark.Grade4MarkMin)
                            iFiveGradeVal = 4;
                        else if (Mark.Value >= Mark.Grade3MarkMin)
                            iFiveGradeVal = 3;

                        context.Mark_updateFiveGradeValue(Mark.Id, iFiveGradeVal);
                    }

                    tran.Complete();
                }
            }

            MessageBox.Show("OK");
        }

        public static void ReSetMarkFromBudget()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lstMarksB =
                    (from Mrk in context.Mark
                     join exInEnt in context.extExamInEntry on Mrk.ExamInEntryBlockUnitId equals exInEnt.Id
                     where exInEnt.StudyBasisId == 1
                     select new
                     {
                         Mrk.Abiturient.PersonId,
                         exInEnt.ExamId,
                         exInEnt.LicenseProgramId,
                         exInEnt.ObrazProgramId,
                         exInEnt.ProfileId,
                         exInEnt.StudyFormId,
                         exInEnt.StudyLevelGroupId,
                         Mrk.Value
                     }).ToList();

                var lstMarksP =
                    (from Mrk in context.Mark
                     join exInEnt in context.extExamInEntry on Mrk.ExamInEntryBlockUnitId equals exInEnt.Id
                     where exInEnt.StudyBasisId == 2
                     select new
                     {
                         Mrk.Abiturient.PersonId,
                         exInEnt.ExamId,
                         exInEnt.LicenseProgramId,
                         exInEnt.ObrazProgramId,
                         exInEnt.ProfileId,
                         exInEnt.StudyFormId,
                         exInEnt.StudyLevelGroupId,
                         Mrk.Value
                     }).ToList();

                var lstAbits = context.qAbitAll.Where(x => x.StudyBasisId == 2).Select(x => new
                {
                    x.Id,
                    x.PersonId,
                    x.LicenseProgramId,
                    x.ObrazProgramId,
                    x.ProfileId,
                    x.StudyFormId,
                    x.StudyLevelGroupId
                }).ToList().OrderBy(x => x.PersonId).ThenBy(x => x.LicenseProgramId).ThenBy(x => x.ObrazProgramId).ThenBy(x => x.ProfileId);

                var lstPersonWithDog = lstAbits.Select(x => x.PersonId).Distinct().ToList();

                var lstMarkToInsert = lstMarksB.Except(lstMarksP).ToList().Where(x => lstPersonWithDog.Contains(x.PersonId)).ToList();

                var examsP = context.extExamInEntry
                    .Where(x => x.StudyBasisId == 2)
                    .Select(exInEnt => new
                    {
                        exInEnt.Id,
                        exInEnt.ExamId,
                        exInEnt.LicenseProgramId,
                        exInEnt.ObrazProgramId,
                        exInEnt.ProfileId,
                        exInEnt.StudyFormId,
                        exInEnt.StudyLevelGroupId
                    }).ToList();

                int cntInsertAbits = 0;
                int cntMissAbits = 0;

                int cntInsertMarks = 0;
                int cntMissMarks = 0;

                //using (TransactionScope tran = new TransactionScope())
                //{
                    foreach (var M in lstMarkToInsert)
                    {
                        Guid ExamInEntryBlockId = examsP
                            .Where(x => x.ExamId == M.ExamId
                                && x.LicenseProgramId == M.LicenseProgramId
                                && x.ObrazProgramId == M.ObrazProgramId
                                && x.ProfileId == M.ProfileId
                                && x.StudyFormId == M.StudyFormId
                                && x.StudyLevelGroupId == M.StudyLevelGroupId
                             ).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).First();

                        Guid AbiturientId = lstAbits.Where(x => x.PersonId == M.PersonId
                            && x.LicenseProgramId == M.LicenseProgramId
                            && x.ObrazProgramId == M.ObrazProgramId
                            && x.ProfileId == M.ProfileId
                            && x.StudyFormId == M.StudyFormId
                            && x.StudyLevelGroupId == M.StudyLevelGroupId)
                            .Select(x => x.Id)
                            .DefaultIfEmpty(Guid.Empty)
                            .First();

                        if (AbiturientId != Guid.Empty)
                        {
                            if (ExamInEntryBlockId != Guid.Empty)
                            {
                                context.Mark_Insert(AbiturientId, ExamInEntryBlockId, M.Value, DateTime.Now, false, false, false, null, null, null);
                                cntInsertMarks++;
                                cntInsertAbits++;
                            }
                            else
                                cntMissMarks++;
                        }
                        else
                            cntMissAbits++;
                    }

                    //tran.Complete();

                    MessageBox.Show("Загружено оценок: " + cntInsertMarks + ", не найдено оценок: " + cntMissMarks + ", не найдено абитуриентов: " + cntMissAbits);
                //}
            }
        }
    }
}

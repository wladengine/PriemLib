using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PriemLib
{
    public static class ApplicationCommitSaveProvider
    {
        public static bool CheckAndUpdateNotUsedApplications(Guid personId, List<ShortCompetition> LstCompetitions)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var EntryIds = LstCompetitions.Select(x => x.EntryId).Distinct().ToList();

                var StudyLevelGroupId = context.extEntry.Where(z => EntryIds.Contains(z.Id))
                    .Select(x => x.StudyLevelGroupId)
                    .DefaultIfEmpty(MainClass.lstStudyLevelGroupId.FirstOrDefault())
                    .First();

                var notUsedApplications = context.Abiturient
                    .Where(x => x.PersonId == personId && !x.BackDoc && x.Entry.StudyLevel.LevelGroupId == StudyLevelGroupId)
                    .Select(x => x.EntryId).ToList()
                    .Except(LstCompetitions.Select(x => x.EntryId)).ToList();
                if (notUsedApplications.Count > 0)
                {
                    var dr = MessageBox.Show("У абитуриента в базе имеются " + notUsedApplications.Count +
                        " конкурсов, не перечисленных в заявлении. Вероятно, по ним был уже произведён отказ. Проставить по данным конкурсным позициям отказ от участия в конкурсе?",
                        "Внимание!", MessageBoxButtons.YesNo);
                    if (dr == System.Windows.Forms.DialogResult.Yes)
                    {
                        string str = "У меня есть на руках заявление об отказе в участии в следующих конкурсах:";
                        int incrmntr = 1;
                        foreach (var app_entry in notUsedApplications)
                        {
                            var entry = context.Entry.Where(x => x.Id == app_entry).FirstOrDefault();
                            str += "\n" + incrmntr++ + ")" + entry.SP_LicenseProgram.Code + " " + entry.SP_LicenseProgram.Name + "; "
                                + entry.StudyLevel.Acronym + "." + entry.SP_ObrazProgram.Number + " " + entry.SP_ObrazProgram.Name +
                                ";\nПрофиль:" + entry.SP_Profile.Name + ";" + entry.StudyForm.Acronym + ";" + entry.StudyBasis.Acronym;
                        }

                        dr = MessageBox.Show(str, "Внимание!", MessageBoxButtons.YesNo);
                        if (dr == System.Windows.Forms.DialogResult.Yes)
                        {
                            foreach (var app_entry in notUsedApplications)
                            {
                                var applst = context.Abiturient.Where(x => x.EntryId == app_entry && x.PersonId == personId && !x.BackDoc && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)).Select(x => x.Id).ToList();
                                foreach (var app in applst)
                                    context.Abiturient_UpdateBackDoc(true, DateTime.Now, app);
                            }
                        }
                        else
                        {
                            return false;
                        }

                        return true;
                    }
                    else
                    {
                        if (MainClass.dbType != PriemType.Priem)
                        {
                            var dr_ = MessageBox.Show("Оставить существующие конкурсные позиции без изменений?",
                        "Внимание!", MessageBoxButtons.YesNo);
                            if (dr_ == DialogResult.Yes)
                                return true;
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                }
                else
                    return true;
            }
        }
        public static void SaveApplicationCommitInWorkBase(Guid PersonId, List<ShortCompetition> LstCompetitions, int? LanguageId, int? _abitBarc)
        {
            using (PriemEntities context = new PriemEntities())
            {
                foreach (var Comp in LstCompetitions)
                {
                    var DocDate = Comp.DocDate;
                    var DocInsertDate = Comp.DocInsertDate == DateTime.MinValue ? DateTime.Now : Comp.DocInsertDate;

                    bool isViewed = Comp.HasCompetition;
                    Guid ApplicationId = Comp.Id;
                    bool hasLoaded = context.Abiturient.Where(x => x.PersonId == PersonId && x.EntryId == Comp.EntryId && !x.BackDoc).Count() == 0;
                    if (hasLoaded)
                    {
                        context.Abiturient_InsertDirectly(PersonId, Comp.EntryId, Comp.CompetitionId, Comp.IsListener,
                            false, false, false, null, DocDate, DocInsertDate,
                            false, false, null, Comp.OtherCompetitionId, Comp.CelCompetitionId, Comp.CelCompetitionText,
                            LanguageId, Comp.HasOriginals, Comp.Priority, Comp.Barcode, Comp.CommitId, _abitBarc, isViewed, ApplicationId);
                    }
                    else
                    {
                        ApplicationId = context.Abiturient.Where(x => x.PersonId == PersonId && x.EntryId == Comp.EntryId && !x.BackDoc).Select(x => x.Id).First();
                        context.Abiturient_UpdatePriority(Comp.Priority, ApplicationId);
                        context.Abiturient_UpdateBarcodeAndCommitId(Comp.Barcode, Comp.CommitId, ApplicationId);
                    }
                    if (Comp.lstInnerEntryInEntry.Count > 0)
                    {
                        //загружаем внутренние приоритеты по профилям
                        int currVersion = Comp.lstInnerEntryInEntry.Select(x => x.CurrVersion).FirstOrDefault();
                        DateTime currDate = Comp.lstInnerEntryInEntry.Select(x => x.CurrDate).FirstOrDefault();
                        Guid ApplicationVersionId = Guid.NewGuid();
                        context.ApplicationVersion.Add(new ApplicationVersion() { IntNumber = currVersion, Id = ApplicationVersionId, ApplicationId = ApplicationId, VersionDate = currDate });
                        foreach (var InnerEntryInEntry in Comp.lstInnerEntryInEntry)
                        {
                            context.Abiturient_UpdateInnerEntryInEntryPriority(InnerEntryInEntry.Id, InnerEntryInEntry.InnerEntryInEntryPriority, ApplicationId);
                        }

                        context.SaveChanges();
                    }

                    if (Comp.lstExamInEntryBlock.Count > 0)
                    {
                        var lstExams = context.AbiturientSelectedExam.Where(x => x.ApplicationId == ApplicationId);
                        context.AbiturientSelectedExam.RemoveRange(lstExams);

                        foreach (var Block in Comp.lstExamInEntryBlock)
                        {
                            if (Block.SelectedUnitId != Guid.Empty)
                                context.AbiturientSelectedExam.Add(new AbiturientSelectedExam()
                                {
                                    ApplicationId = ApplicationId,
                                    ExamInEntryBlockUnitId = Block.SelectedUnitId,
                                });
                        }

                        context.SaveChanges();
                    }
                }
            }
        }
    }
}

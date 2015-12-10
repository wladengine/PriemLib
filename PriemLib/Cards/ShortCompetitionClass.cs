﻿using EducServLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PriemLib
{
    public class ShortCompetition
    {
        public Guid Id { get; private set; }
        public Guid PersonId { get; private set; }
        public Guid EntryId { get; private set; }
        public Guid CommitId { get; private set; }

        public int? VersionNum { get; private set; }
        public DateTime? VersionDate { get; private set; }

        public int StudyLevelId { get; set; }
        public string StudyLevelName { get; set; }
        public int LicenseProgramId { get; set; }
        public string LicenseProgramName { get; set; }
        public int ObrazProgramId { get; set; }
        public string ObrazProgramName { get; set; }
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public int FacultyId { get; set; }
        public string FacultyName { get; set; }

        public int StudyFormId { get; set; }
        public string StudyFormName { get; set; }
        public int StudyBasisId { get; set; }
        public string StudyBasisName { get; set; }

        public bool HasCompetition { get; set; }
        public int CompetitionId { get; set; }
        public string CompetitionName { get; set; }

        public int Priority { get; set; }
        public int? OtherCompetitionId { get; set; }
        public int? CelCompetitionId { get; set; }
        public string CelCompetitionText { get; set; }

        public DateTime DocDate { get; set; }
        public DateTime DocInsertDate { get; set; }

        public bool HasOriginals { get; set; }

        public bool IsListener { get; set; }
        public bool IsReduced { get; set; }
        public bool IsSecond { get; set; }
        public bool IsForeign { get; set; }
        public bool IsCrimea { get; set; }

        public int Barcode { get; set; }

        public bool HasInnerPriorities { get; set; }
        public bool IsApprovedByComission { get; set; }
        public string ApproverName { get; set; }
        public List<ShortInnerEntryInEntry> lstInnerEntryInEntry { get; set; }

        public bool HasManualExams { get; set; }
        public List<ExamenBlock> lstExamInEntryBlock { get; set; }

        public ShortCompetition(Guid _Id, Guid _CommitId, Guid _EntryId, Guid _PersonId, int? _VersionNum, DateTime? _VersionDate)
        {
            Id = _Id;
            CommitId = _CommitId;
            EntryId = _EntryId;
            PersonId = _PersonId;
            VersionNum = _VersionNum;
            VersionDate = _VersionDate;
            lstExamInEntryBlock = new List<ExamenBlock>();
        }

        public void ChangeEntry()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var Entry = context.Entry
                    .Where(x => x.LicenseProgramId == LicenseProgramId
                        && x.ObrazProgramId == ObrazProgramId
                        && x.StudyLevelId == StudyLevelId
                        && x.StudyFormId == StudyFormId
                        && x.StudyBasisId == StudyBasisId
                        && x.ProfileId == ProfileId
                        && x.IsCrimea == IsCrimea
                        && x.IsForeign == IsForeign
                        );

                if (Entry.Count() == 1)
                    EntryId = Entry.First().Id;
                else if (Entry.Count() == 0)
                    WinFormsServ.Error("Не найден конкурс, отвечающий заданным параметрам!");
                else
                    WinFormsServ.Error("Найдено более 1 конкурса, отвечающих заданным параметрам!");
            }
        }
    }
    public class ShortInnerEntryInEntry
    {
        public Guid Id { get; private set; }
        public string ObrazProgramName { get; private set; }
        public string ProfileName { get; private set; }
        public int InnerEntryInEntryPriority { get; set; }
        public int CurrVersion { get; set; }
        public DateTime CurrDate { get; set; }

        public ShortInnerEntryInEntry(Guid _id, string _obrazProgramName, string _profileName)
        {
            Id = _id;
            ObrazProgramName = _obrazProgramName;
            ProfileName = _profileName;
        }
    }
    
}

//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PriemLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class extAbit
    {
        public System.Guid Id { get; set; }
        public System.Guid PersonId { get; set; }
        public string RegNum { get; set; }
        public System.Guid EntryId { get; set; }
        public int CompetitionId { get; set; }
        public bool IsListener { get; set; }
        public bool IsPaid { get; set; }
        public bool BackDoc { get; set; }
        public Nullable<System.DateTime> BackDocDate { get; set; }
        public System.DateTime DocDate { get; set; }
        public System.DateTime DocInsertDate { get; set; }
        public bool Checked { get; set; }
        public bool NotEnabled { get; set; }
        public Nullable<double> Coefficient { get; set; }
        public Nullable<decimal> Sum { get; set; }
        public Nullable<int> OtherCompetitionId { get; set; }
        public Nullable<int> CelCompetitionId { get; set; }
        public string CelCompetitionText { get; set; }
        public bool CompFromOlymp { get; set; }
        public int LanguageId { get; set; }
        public bool HasOriginals { get; set; }
        public string StudyNumber { get; set; }
        public Nullable<int> StudentStatus { get; set; }
        public Nullable<double> SessionAVG { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<int> Barcode { get; set; }
        public int FacultyId { get; set; }
        public string FacultyAcr { get; set; }
        public int LicenseProgramId { get; set; }
        public string LicenseProgramName { get; set; }
        public string FacultyName { get; set; }
        public string LicenseProgramCode { get; set; }
        public int ObrazProgramId { get; set; }
        public string ObrazProgramName { get; set; }
        public string ObrazProgramNumber { get; set; }
        public string ObrazProgramCrypt { get; set; }
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public int StudyBasisId { get; set; }
        public string StudyBasisName { get; set; }
        public int StudyFormId { get; set; }
        public string StudyFormName { get; set; }
        public int StudyLevelId { get; set; }
        public string StudyLevelName { get; set; }
        public Nullable<System.Guid> StudyPlanId { get; set; }
        public string StudyPlanNumber { get; set; }
        public string ProgramModeShortName { get; set; }
        public bool IsSecond { get; set; }
        public string PersonNum { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Surname { get; set; }
        public string StudyFormOldName { get; set; }
        public string LanguageName { get; set; }
        public int StudyLevelGroupId { get; set; }
        public string FIO { get; set; }
        public string PassportData { get; set; }
        public string ObrazProgramNameEx { get; set; }
        public bool WithHE { get; set; }
        public bool IsReduced { get; set; }
        public bool IsParallel { get; set; }
        public string StudyBasisFISName { get; set; }
        public string StudyFormFISName { get; set; }
        public Nullable<System.Guid> CommitId { get; set; }
        public Nullable<int> CommitNumber { get; set; }
        public Nullable<System.Guid> InnerEntryInEntryId { get; set; }
        public string InnerEntryInEntryProfileName { get; set; }
        public string InnerEntryInEntryObrazProgramCrypt { get; set; }
        public string InnerEntryInEntryObrazProgramName { get; set; }
        public Nullable<int> InnerEntryInEntryObrazProgramId { get; set; }
        public bool IsForeign { get; set; }
        public bool IsCrimea { get; set; }
        public bool BackDocByAdmissionHigh { get; set; }
        public Nullable<System.Guid> OlympiadId { get; set; }
        public bool HasEntryConfirm { get; set; }
        public Nullable<System.DateTime> DateEntryConfirm { get; set; }
        public bool HasDisabledEntryConfirm { get; set; }
        public Nullable<System.DateTime> DateDisableEntryConfirm { get; set; }
        public Nullable<int> StudyLevelGroupUpperCategoryId { get; set; }
    }
}

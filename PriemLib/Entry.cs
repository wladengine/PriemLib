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
    
    public partial class Entry
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Entry()
        {
            this.Abiturient = new HashSet<Abiturient>();
            this.EntryToCompetitiveGroup = new HashSet<EntryToCompetitiveGroup>();
            this.ExamInEntryBlock = new HashSet<ExamInEntryBlock>();
            this.InnerEntryInEntry = new HashSet<InnerEntryInEntry>();
            this.OlympResultToAdditionalMark = new HashSet<OlympResultToAdditionalMark>();
            this.OlympResultToCommonBenefit = new HashSet<OlympResultToCommonBenefit>();
            this.PayDataEntry = new HashSet<PayDataEntry>();
        }
    
        public System.Guid Id { get; set; }
        public int FacultyId { get; set; }
        public int LicenseProgramId { get; set; }
        public int ObrazProgramId { get; set; }
        public int ProfileId { get; set; }
        public int StudyBasisId { get; set; }
        public int StudyFormId { get; set; }
        public int StudyLevelId { get; set; }
        public Nullable<System.Guid> StudyPlanId { get; set; }
        public string StudyPlanNumber { get; set; }
        public string ProgramModeShortName { get; set; }
        public bool IsSecond { get; set; }
        public bool IsReduced { get; set; }
        public bool IsParallel { get; set; }
        public bool IsForeign { get; set; }
        public bool IsCrimea { get; set; }
        public Nullable<int> KCP { get; set; }
        public Nullable<int> KCPCel { get; set; }
        public bool IsClosed { get; set; }
        public string QualificationCode { get; set; }
        public Nullable<System.Guid> ThreeAbitsCompetitionId { get; set; }
        public string ObrazProgramPrintName { get; set; }
        public System.DateTime DateOfClose { get; set; }
        public System.DateTime DateOfStart { get; set; }
        public Nullable<int> CommissionId { get; set; }
        public Nullable<int> KCPQuota { get; set; }
        public Nullable<System.DateTime> DateFinishEduc { get; set; }
        public Nullable<System.DateTime> DateStartEduc { get; set; }
        public Nullable<System.Guid> ParentEntryId { get; set; }
        public string ExamName1 { get; set; }
        public string ExamName2 { get; set; }
        public string ExamName3 { get; set; }
        public string ExamName4 { get; set; }
        public string ExamName5 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Abiturient> Abiturient { get; set; }
        public virtual Comission Comission { get; set; }
        public virtual SP_Faculty SP_Faculty { get; set; }
        public virtual SP_LicenseProgram SP_LicenseProgram { get; set; }
        public virtual SP_Profile SP_Profile { get; set; }
        public virtual StudyBasis StudyBasis { get; set; }
        public virtual StudyForm StudyForm { get; set; }
        public virtual StudyLevel StudyLevel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EntryToCompetitiveGroup> EntryToCompetitiveGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamInEntryBlock> ExamInEntryBlock { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InnerEntryInEntry> InnerEntryInEntry { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OlympResultToAdditionalMark> OlympResultToAdditionalMark { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OlympResultToCommonBenefit> OlympResultToCommonBenefit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayDataEntry> PayDataEntry { get; set; }
    }
}

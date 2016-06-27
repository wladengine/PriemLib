//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PriemLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class Exam
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Exam()
        {
            this.EgeToExam = new HashSet<EgeToExam>();
            this.EgeToLanguage = new HashSet<EgeToLanguage>();
            this.OlympSubjectToExam = new HashSet<OlympSubjectToExam>();
            this.ExamInEntryBlockUnit = new HashSet<ExamInEntryBlockUnit>();
            this.OlympResultToAdditionalMark = new HashSet<OlympResultToAdditionalMark>();
            this.OlympResultToCommonBenefit = new HashSet<OlympResultToCommonBenefit>();
        }
    
        public int Id { get; set; }
        public int ExamNameId { get; set; }
        public bool IsAdditional { get; set; }
        public bool IsOpen { get; set; }
        public string Holder { get; set; }
        public bool IsPortfolioCommonPart { get; set; }
        public bool IsPortfolioAnonymPart { get; set; }
    
        public virtual ExamName ExamName { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EgeToExam> EgeToExam { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EgeToLanguage> EgeToLanguage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OlympSubjectToExam> OlympSubjectToExam { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamInEntryBlockUnit> ExamInEntryBlockUnit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OlympResultToAdditionalMark> OlympResultToAdditionalMark { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OlympResultToCommonBenefit> OlympResultToCommonBenefit { get; set; }
    }
}

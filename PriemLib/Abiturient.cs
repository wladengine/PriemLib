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
    
    public partial class Abiturient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Abiturient()
        {
            this.Olympiads = new HashSet<Olympiads>();
            this.PaidData = new HashSet<PaidData>();
            this.DocInventory = new HashSet<DocInventory>();
            this.Fixieren = new HashSet<Fixieren>();
            this.ApplicationDetails = new HashSet<ApplicationDetails>();
            this.ApplicationVersion = new HashSet<ApplicationVersion>();
            this.ProtocolHistory = new HashSet<ProtocolHistory>();
            this.AbiturientSelectedExam = new HashSet<AbiturientSelectedExam>();
            this.Mark = new HashSet<Mark>();
        }
    
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
        public Nullable<int> Sum { get; set; }
        public Nullable<double> Coefficient { get; set; }
        public Nullable<int> OtherCompetitionId { get; set; }
        public Nullable<int> CelCompetitionId { get; set; }
        public string CelCompetitionText { get; set; }
        public bool CompFromOlymp { get; set; }
        public int LanguageId { get; set; }
        public bool HasOriginals { get; set; }
        public string StudyNumber { get; set; }
        public Nullable<double> SessionAVG { get; set; }
        public Nullable<int> StudentStatus { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<int> Barcode { get; set; }
        public bool WithHE { get; set; }
        public bool IsOpen { get; set; }
        public string Holder { get; set; }
        public Nullable<bool> IsImportedToFIS { get; set; }
        public Nullable<bool> IsCommitedToFIS { get; set; }
        public Nullable<System.Guid> CommitId { get; set; }
        public Nullable<int> CommitNumber { get; set; }
        public bool IsViewed { get; set; }
        public Nullable<System.Guid> InnerEntryInEntryId { get; set; }
        public string Author { get; set; }
        public System.DateTime DateCreated { get; set; }
        public bool BackDocByAdmissionHigh { get; set; }
        public Nullable<System.Guid> OlympiadId { get; set; }
    
        public virtual CelCompetition CelCompetition { get; set; }
        public virtual Competition Competition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Olympiads> Olympiads { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaidData> PaidData { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocInventory> DocInventory { get; set; }
        public virtual Competition Competition1 { get; set; }
        public virtual Language Language { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fixieren> Fixieren { get; set; }
        public virtual Person Person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationDetails> ApplicationDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationVersion> ApplicationVersion { get; set; }
        public virtual Entry Entry { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProtocolHistory> ProtocolHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbiturientSelectedExam> AbiturientSelectedExam { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mark> Mark { get; set; }
    }
}
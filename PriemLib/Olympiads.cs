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
    
    public partial class Olympiads
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Olympiads()
        {
            this.Abiturient1 = new HashSet<Abiturient>();
            this.Mark = new HashSet<Mark>();
        }
    
        public System.Guid Id { get; set; }
        public int OlympTypeId { get; set; }
        public int OlympNameId { get; set; }
        public int OlympSubjectId { get; set; }
        public Nullable<int> OlympLevelId { get; set; }
        public int OlympValueId { get; set; }
        public Nullable<System.Guid> AbiturientId { get; set; }
        public bool IsOpen { get; set; }
        public string Holder { get; set; }
        public bool OriginDoc { get; set; }
        public string DocumentSeries { get; set; }
        public string DocumentNumber { get; set; }
        public Nullable<System.DateTime> DocumentDate { get; set; }
        public string Author { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int OlympYear { get; set; }
        public Nullable<int> OlympProfileId { get; set; }
        public System.Guid PersonId { get; set; }
    
        public virtual OlympLevel OlympLevel { get; set; }
        public virtual OlympValue OlympValue { get; set; }
        public virtual Abiturient Abiturient { get; set; }
        public virtual OlympiadCheckedByRectorat OlympiadCheckedByRectorat { get; set; }
        public virtual OlympName OlympName { get; set; }
        public virtual OlympSubject OlympSubject { get; set; }
        public virtual OlympType OlympType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Abiturient> Abiturient1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mark> Mark { get; set; }
    }
}

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
    
    public partial class ExamInEntryBlockUnit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExamInEntryBlockUnit()
        {
            this.Mark = new HashSet<Mark>();
        }
    
        public System.Guid Id { get; set; }
        public System.Guid ExamInEntryBlockId { get; set; }
        public int ExamId { get; set; }
        public Nullable<decimal> EgeMin { get; set; }
    
        public virtual ExamInEntryBlock ExamInEntryBlock { get; set; }
        public virtual Exam Exam { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mark> Mark { get; set; }
    }
}

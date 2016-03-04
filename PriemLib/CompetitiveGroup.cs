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
    
    public partial class CompetitiveGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CompetitiveGroup()
        {
            this.EntryToCompetitiveGroup = new HashSet<EntryToCompetitiveGroup>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> KCP { get; set; }
        public bool IsOpen { get; set; }
        public string Holder { get; set; }
        public int LicenseProgramId { get; set; }
        public int ObrazProgramId { get; set; }
        public int ProfileId { get; set; }
        public bool IsReduced { get; set; }
        public bool IsSecond { get; set; }
        public bool IsParallel { get; set; }
        public bool IsForeign { get; set; }
        public bool IsCrimea { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EntryToCompetitiveGroup> EntryToCompetitiveGroup { get; set; }
    }
}
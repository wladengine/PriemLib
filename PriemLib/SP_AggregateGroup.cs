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
    
    public partial class SP_AggregateGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SP_AggregateGroup()
        {
            this.SP_LicenseProgram = new HashSet<SP_LicenseProgram>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }
        public string Acronym { get; set; }
        public string AcronymEng { get; set; }
        public Nullable<int> ProfessionListId { get; set; }
        public bool IsOpen { get; set; }
        public string Holder { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SP_LicenseProgram> SP_LicenseProgram { get; set; }
    }
}

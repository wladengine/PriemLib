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
    
    public partial class Prorektor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prorektor()
        {
            this.PaidData = new HashSet<PaidData>();
            this.PayDataEntry = new HashSet<PayDataEntry>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string DateDov { get; set; }
        public string NumberDov { get; set; }
        public string NameFull { get; set; }
        public string PassportData { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaidData> PaidData { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayDataEntry> PayDataEntry { get; set; }
    }
}

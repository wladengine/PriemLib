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
    
    public partial class ApplicationVersion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ApplicationVersion()
        {
            this.ApplicationVersionDetails = new HashSet<ApplicationVersionDetails>();
        }
    
        public System.Guid Id { get; set; }
        public int IntNumber { get; set; }
        public System.Guid ApplicationId { get; set; }
        public System.DateTime VersionDate { get; set; }
    
        public virtual Abiturient Abiturient { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationVersionDetails> ApplicationVersionDetails { get; set; }
    }
}
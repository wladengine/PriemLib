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
    
    public partial class BenefitDocument
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BenefitDocument()
        {
            this.PersonBenefitDocument = new HashSet<PersonBenefitDocument>();
        }
    
        public int Id { get; set; }
        public int BenefitDocumentTypeId { get; set; }
        public string Name { get; set; }
        public Nullable<int> FISID { get; set; }
    
        public virtual BenefitDocumentType BenefitDocumentType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonBenefitDocument> PersonBenefitDocument { get; set; }
    }
}

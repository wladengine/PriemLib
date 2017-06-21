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
    
    public partial class OlympValue
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OlympValue()
        {
            this.Olympiads = new HashSet<Olympiads>();
            this.OlympResultToAdditionalMark = new HashSet<OlympResultToAdditionalMark>();
            this.OlympResultToCommonBenefit = new HashSet<OlympResultToCommonBenefit>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string Acronym { get; set; }
        public bool IsOpen { get; set; }
        public string Holder { get; set; }
        public string PrintNameShort { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Olympiads> Olympiads { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OlympResultToAdditionalMark> OlympResultToAdditionalMark { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OlympResultToCommonBenefit> OlympResultToCommonBenefit { get; set; }
    }
}

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
    
    public partial class PersonScienceWork
    {
        public System.Guid Id { get; set; }
        public System.Guid PersonId { get; set; }
        public Nullable<int> WorkTypeId { get; set; }
        public string WorkInfo { get; set; }
        public string WorkYear { get; set; }
    
        public virtual ScienceWorkType ScienceWorkType { get; set; }
    }
}

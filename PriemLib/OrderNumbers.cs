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
    
    public partial class OrderNumbers
    {
        public System.Guid ProtocolId { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string OrderNum { get; set; }
        public string OrderNumFor { get; set; }
        public Nullable<System.DateTime> OrderDateFor { get; set; }
        public int SignerId { get; set; }
        public System.DateTime ComissionDate { get; set; }
        public string ComissionNumber { get; set; }
    
        public virtual Signer Signer { get; set; }
    }
}

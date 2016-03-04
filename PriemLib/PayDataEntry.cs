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
    
    public partial class PayDataEntry
    {
        public System.Guid EntryId { get; set; }
        public string UniverName { get; set; }
        public string UniverAddress { get; set; }
        public string UniverINN { get; set; }
        public string UniverRS { get; set; }
        public string UniverDop { get; set; }
        public int ProrektorId { get; set; }
        public string Qualification { get; set; }
        public string EducPeriod { get; set; }
        public System.DateTime DateStart { get; set; }
        public System.DateTime DateFinish { get; set; }
        public string Props { get; set; }
        public System.Guid Id { get; set; }
        public bool IsOpen { get; set; }
        public string Holder { get; set; }
    
        public virtual Prorektor Prorektor { get; set; }
        public virtual Entry Entry { get; set; }
    }
}
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
    
    public partial class EgeMark
    {
        public System.Guid Id { get; set; }
        public int Value { get; set; }
        public int EgeExamNameId { get; set; }
        public System.Guid EgeCertificateId { get; set; }
        public bool IsAppeal { get; set; }
        public bool IsCurrent { get; set; }
    
        public virtual EgeCertificate EgeCertificate { get; set; }
        public virtual EgeExamName EgeExamName { get; set; }
    }
}
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
    
    public partial class AttMarks
    {
        public System.Guid Id { get; set; }
        public System.Guid PersonId { get; set; }
        public int AttSubjectId { get; set; }
        public int Value { get; set; }
        public string Author { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual AttSubject AttSubject { get; set; }
        public virtual Person Person { get; set; }
    }
}

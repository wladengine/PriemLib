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
    
    public partial class PersonOtherPassport
    {
        public int Id { get; set; }
        public System.Guid PersonId { get; set; }
        public int PassportTypeId { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public bool IsOpen { get; set; }
        public string Holder { get; set; }
        public Nullable<System.DateTime> PassportDate { get; set; }
    
        public virtual PassportType PassportType { get; set; }
        public virtual Person Person { get; set; }
    }
}
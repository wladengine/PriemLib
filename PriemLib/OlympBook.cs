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
    
    public partial class OlympBook
    {
        public System.Guid Id { get; set; }
        public Nullable<int> OlympTypeId { get; set; }
        public Nullable<int> OlympNameId { get; set; }
        public Nullable<int> OlympSubjectId { get; set; }
        public Nullable<int> OlympLevelId { get; set; }
        public bool IsOpen { get; set; }
        public string Holder { get; set; }
        public int OlympYear { get; set; }
    
        public virtual OlympLevel OlympLevel { get; set; }
        public virtual OlympSubject OlympSubject { get; set; }
        public virtual OlympType OlympType { get; set; }
        public virtual OlympName OlympName { get; set; }
    }
}

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
    
    public partial class extEnableProtocol
    {
        public System.Guid Id { get; set; }
        public System.Guid AbiturientId { get; set; }
        public bool Excluded { get; set; }
        public Nullable<System.Guid> ExcludeProtocolId { get; set; }
        public Nullable<int> EntryHeaderId { get; set; }
        public int StudyLevelGroupId { get; set; }
        public Nullable<int> LicenseProgramId { get; set; }
        public Nullable<int> FacultyId { get; set; }
        public Nullable<int> StudyFormId { get; set; }
        public Nullable<int> StudyBasisId { get; set; }
        public string Number { get; set; }
        public System.DateTime Date { get; set; }
        public int ProtocolTypeId { get; set; }
        public string Reason { get; set; }
        public bool IsOld { get; set; }
        public Nullable<System.Guid> ParentProtocolId { get; set; }
        public bool IsSecond { get; set; }
        public bool IsListener { get; set; }
        public string ProtocolTypeName { get; set; }
    }
}

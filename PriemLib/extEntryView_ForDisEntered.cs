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
    
    public partial class extEntryView_ForDisEntered
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
        public bool IsParallel { get; set; }
        public bool IsReduced { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string OrderNum { get; set; }
        public string OrderNumFor { get; set; }
        public Nullable<System.DateTime> OrderDateFor { get; set; }
        public string SignerName { get; set; }
        public string SignerPosition { get; set; }
    }
}

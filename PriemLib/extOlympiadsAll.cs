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
    
    public partial class extOlympiadsAll
    {
        public System.Guid Id { get; set; }
        public int OlympTypeId { get; set; }
        public int OlympNameId { get; set; }
        public int OlympSubjectId { get; set; }
        public Nullable<int> OlympLevelId { get; set; }
        public int OlympValueId { get; set; }
        public Nullable<System.Guid> AbiturientId { get; set; }
        public string OlympLevelName { get; set; }
        public string OlympValueName { get; set; }
        public string OlympName { get; set; }
        public string OlympSubjectName { get; set; }
        public string OlympTypeName { get; set; }
        public System.Guid PersonId { get; set; }
        public bool OriginDoc { get; set; }
        public Nullable<int> Number { get; set; }
        public string DocumentSeries { get; set; }
        public string DocumentNumber { get; set; }
        public Nullable<System.DateTime> DocumentDate { get; set; }
        public Nullable<int> OlympValueSortOrder { get; set; }
        public short OlympYear { get; set; }
        public Nullable<int> OlympProfileId { get; set; }
        public string OlympProfileName { get; set; }
        public string OlympicID { get; set; }
        public string FIS_OlympicID { get; set; }
        public Nullable<int> OlympLevel_FISID { get; set; }
    }
}

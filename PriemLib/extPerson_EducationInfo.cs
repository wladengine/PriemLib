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
    
    public partial class extPerson_EducationInfo
    {
        public int Id { get; set; }
        public System.Guid PersonId { get; set; }
        public int SchoolTypeId { get; set; }
        public string SchoolTypeName { get; set; }
        public string SchoolTypeFullName { get; set; }
        public int CountryEducId { get; set; }
        public string CountryEducName { get; set; }
        public string CountryEducOSKMCode { get; set; }
        public int RegionEducId { get; set; }
        public string RegionEducName { get; set; }
        public string RegionEducNumber { get; set; }
        public int ForeignCountryEducId { get; set; }
        public string ForeignCountryEducName { get; set; }
        public string ForeignCountryEducOSKMCode { get; set; }
        public Nullable<int> ForeignCountryEducPriemDictionaryId { get; set; }
        public string SchoolCity { get; set; }
        public string SchoolName { get; set; }
        public string SchoolNum { get; set; }
        public int SchoolExitYear { get; set; }
        public string AttestatSeries { get; set; }
        public string AttestatNum { get; set; }
        public string DiplomSeries { get; set; }
        public string DiplomNum { get; set; }
        public Nullable<double> SchoolAVG { get; set; }
        public bool IsExcellent { get; set; }
        public bool IsEqual { get; set; }
        public string EqualDocumentNumber { get; set; }
        public string HighEducation { get; set; }
        public string HEProfession { get; set; }
        public string HEQualification { get; set; }
        public Nullable<int> HEStudyFormId { get; set; }
        public string HEStudyFormName { get; set; }
        public string HEStudyFormFISName { get; set; }
        public Nullable<int> HEEntryYear { get; set; }
        public Nullable<int> HEExitYear { get; set; }
        public string HEWork { get; set; }
    }
}

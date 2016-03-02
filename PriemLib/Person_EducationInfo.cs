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
    
    public partial class Person_EducationInfo
    {
        public System.Guid PersonId { get; set; }
        public bool IsExcellent { get; set; }
        public string SchoolCity { get; set; }
        public int SchoolTypeId { get; set; }
        public string SchoolName { get; set; }
        public string SchoolNum { get; set; }
        public int SchoolExitYear { get; set; }
        public int CountryEducId { get; set; }
        public string AttestatSeries { get; set; }
        public string AttestatNum { get; set; }
        public string DiplomSeries { get; set; }
        public string DiplomNum { get; set; }
        public Nullable<double> SchoolAVG { get; set; }
        public string HighEducation { get; set; }
        public string HEProfession { get; set; }
        public string HEQualification { get; set; }
        public Nullable<int> HEStudyFormId { get; set; }
        public Nullable<int> HEEntryYear { get; set; }
        public Nullable<int> HEExitYear { get; set; }
        public string HEWork { get; set; }
        public int ForeignCountryEducId { get; set; }
        public bool IsEqual { get; set; }
        public string EqualDocumentNumber { get; set; }
        public int RegionEducId { get; set; }
        public int Id { get; set; }
        public string Author { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<int> SchoolExitClassId { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual Person Person { get; set; }
        public virtual SchoolType SchoolType { get; set; }
        public virtual StudyForm StudyForm { get; set; }
        public virtual ForeignCountry ForeignCountry { get; set; }
    }
}

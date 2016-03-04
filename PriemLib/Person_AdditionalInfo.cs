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
    
    public partial class Person_AdditionalInfo
    {
        public System.Guid PersonId { get; set; }
        public bool HostelAbit { get; set; }
        public bool HasAssignToHostel { get; set; }
        public Nullable<int> HostelFacultyId { get; set; }
        public bool HasExamPass { get; set; }
        public Nullable<int> ExamPassFacultyId { get; set; }
        public int Privileges { get; set; }
        public string PersonInfo { get; set; }
        public string ExtraInfo { get; set; }
        public string ScienceWork { get; set; }
        public Nullable<int> MSStudyFormId { get; set; }
        public string MSVuz { get; set; }
        public string MSCourse { get; set; }
        public string Stag { get; set; }
        public string WorkPlace { get; set; }
        public bool HostelEduc { get; set; }
        public bool HasTRKI { get; set; }
        public string TRKICertificateNumber { get; set; }
        public bool StartEnglish { get; set; }
        public Nullable<double> EnglishMark { get; set; }
        public int LanguageId { get; set; }
        public bool EgeInSPbgu { get; set; }
        public string Author { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual Person Person { get; set; }
        public virtual SP_Faculty ExamPassFacultyFaculty { get; set; }
        public virtual SP_Faculty HostelFaculty { get; set; }
        public virtual Language Language { get; set; }
    }
}
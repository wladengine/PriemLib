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
    
    public partial class EgeToExam
    {
        public int Id { get; set; }
        public int EgeExamNameId { get; set; }
        public Nullable<int> ExamId { get; set; }
    
        public virtual EgeExamName EgeExamName { get; set; }
        public virtual Exam Exam { get; set; }
    }
}

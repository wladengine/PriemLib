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
    
    public partial class Mark
    {
        public System.Guid Id { get; set; }
        public System.Guid AbiturientId { get; set; }
        public System.Guid ExamInEntryBlockUnitId { get; set; }
        public decimal Value { get; set; }
        public Nullable<System.DateTime> PassDate { get; set; }
        public bool IsFromEge { get; set; }
        public bool IsFromOlymp { get; set; }
        public bool IsManual { get; set; }
        public Nullable<System.Guid> ExamVedId { get; set; }
        public Nullable<System.Guid> OlympiadId { get; set; }
        public Nullable<System.Guid> EgeCertificateId { get; set; }
        public Nullable<byte> FiveGradeValue { get; set; }
    
        public virtual Abiturient Abiturient { get; set; }
        public virtual EgeCertificate EgeCertificate { get; set; }
        public virtual ExamInEntryBlockUnit ExamInEntryBlockUnit { get; set; }
        public virtual Olympiads Olympiads { get; set; }
    }
}

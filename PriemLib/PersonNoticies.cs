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
    
    public partial class PersonNoticies
    {
        public int Id { get; set; }
        public System.Guid PersonId { get; set; }
        public string NoticeText { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string CreateAuthor { get; set; }
    
        public virtual Person Person { get; set; }
    }
}

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
    
    public partial class PersonAchievement
    {
        public System.Guid Id { get; set; }
        public System.Guid PersonId { get; set; }
        public int AchievementTypeId { get; set; }
        public bool IsOpen { get; set; }
        public string Holder { get; set; }
        public string Author { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual AchievementType AchievementType { get; set; }
        public virtual Person Person { get; set; }
    }
}

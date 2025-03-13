using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    
    
    
    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SystemName { get; set; }
        
        
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
} 

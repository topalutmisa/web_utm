﻿using System;

namespace VinlandSaga.Domain.Models
{
    
    
    
    public class RolePermission
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        
        
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
} 

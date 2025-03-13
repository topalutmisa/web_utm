﻿using System;

namespace VinlandSaga.Domain.Models
{
    
    
    
    public class UserRole
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime AssignedDate { get; set; }
        
        
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
} 

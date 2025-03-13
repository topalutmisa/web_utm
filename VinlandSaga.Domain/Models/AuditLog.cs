using System;

namespace VinlandSaga.Domain.Models
{
    
    
    
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public Guid? EntityId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }
        public bool IsSuccess { get; set; }
        
        
        public virtual User User { get; set; }
    }
} 

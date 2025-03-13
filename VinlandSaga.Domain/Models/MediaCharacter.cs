using System;

namespace VinlandSaga.Domain.Models
{
    
    
    
    public class MediaCharacter
    {
        public Guid Id { get; set; }
        public Guid MediaId { get; set; }
        public Guid CharacterId { get; set; }
        public string Role { get; set; } 
        
        
        public virtual Media Media { get; set; }
        public virtual Character Character { get; set; }
    }
} 

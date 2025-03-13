using System;

namespace VinlandSaga.Domain.Models
{
    
    
    
    public class CharacterRelationship
    {
        public Guid Id { get; set; }
        public Guid SourceCharacterId { get; set; }
        public Guid TargetCharacterId { get; set; }
        public string RelationType { get; set; }
        public string Description { get; set; }
        
        
        public virtual Character SourceCharacter { get; set; }
        public virtual Character TargetCharacter { get; set; }
    }
} 

using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    public class Character
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string JapaneseName { get; set; }
        public string Description { get; set; }
        public string Biography { get; set; }
        public string ImageUrl { get; set; }
        public string Alias { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Occupation { get; set; }
        public string Affiliation { get; set; }
        public string Clan { get; set; }
        public string FirstAppearance { get; set; }
        public string Status { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public int PopularityRank { get; set; }
        public bool IsFeatured { get; set; }
        
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<CharacterRelationship> SourceRelationships { get; set; }
        public virtual ICollection<CharacterRelationship> TargetRelationships { get; set; }
    }
} 
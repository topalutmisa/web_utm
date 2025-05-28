using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.DTOs
{
    public class CharacterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Clan { get; set; }
        public string Status { get; set; } // "Alive", "Dead", "Unknown"
        public string Occupation { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public List<string> Relationships { get; set; }
        public List<string> Appearances { get; set; }
        
        public CharacterDto()
        {
            Relationships = new List<string>();
            Appearances = new List<string>();
        }
    }
} 
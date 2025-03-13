using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    
    
    
    public class Media
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } 
        public string Season { get; set; }
        public string Episodes { get; set; }
        public string Chapters { get; set; }
        public string Status { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Studio { get; set; }
        public string Director { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }
        public string ExternalLink { get; set; }
        
        
        public virtual ICollection<MediaCharacter> Characters { get; set; }
    }
} 

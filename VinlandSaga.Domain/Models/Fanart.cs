using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    
    
    
    public class Fanart
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime UploadDate { get; set; }
        public int LikesCount { get; set; }
        public bool IsApproved { get; set; }
        public bool IsFeatured { get; set; }
        
        
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        
        
        public virtual User User { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
} 

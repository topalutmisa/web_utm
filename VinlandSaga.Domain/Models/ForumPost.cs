using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    
    
    
    public class ForumPost
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int LikesCount { get; set; }
        
        
        public Guid UserId { get; set; }
        public Guid ForumTopicId { get; set; }
        
        
        public virtual User User { get; set; }
        public virtual ForumTopic ForumTopic { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
} 

using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    
    
    
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int LikesCount { get; set; }
        
        
        public Guid UserId { get; set; }
        public Guid? ParentCommentId { get; set; }
        
        
        public Guid? FanartId { get; set; }
        public Guid? ForumPostId { get; set; }
        public Guid? CharacterId { get; set; }
        public Guid? NewsId { get; set; }
        
        
        public virtual User User { get; set; }
        public virtual Comment ParentComment { get; set; }
        public virtual ICollection<Comment> ChildComments { get; set; }
        public virtual Fanart Fanart { get; set; }
        public virtual ForumPost ForumPost { get; set; }
        public virtual Character Character { get; set; }
        public virtual News News { get; set; }
    }
} 

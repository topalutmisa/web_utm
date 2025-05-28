using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    public class ForumTopic
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastPostDate { get; set; }
        public int PostsCount { get; set; }
        public int ViewsCount { get; set; }
        public bool IsPinned { get; set; }
        public bool IsLocked { get; set; }
        public bool IsSticky { get; set; }

        public Guid UserId { get; set; }
        public Guid AuthorId { get; set; }
        public Guid CategoryId { get; set; }

        public virtual User User { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<ForumPost> Posts { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
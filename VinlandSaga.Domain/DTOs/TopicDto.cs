using System;

namespace VinlandSaga.Domain.DTOs
{
    public class TopicDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastPostDate { get; set; }
        public string AuthorName { get; set; }
        public Guid AuthorId { get; set; }
        public string CategoryName { get; set; }
        public Guid CategoryId { get; set; }
        public int PostsCount { get; set; }
        public int ViewsCount { get; set; }
        public bool IsSticky { get; set; }
        public bool IsLocked { get; set; }
        public bool IsFeatured { get; set; }
        public string LastPostAuthor { get; set; }
    }
} 
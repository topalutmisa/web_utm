using System;

namespace VinlandSaga.Domain.DTOs
{
    public class NewsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishDate { get; set; }
        public string AuthorName { get; set; }
        public Guid AuthorId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsFeatured { get; set; }
        public int ViewsCount { get; set; }
        public string[] Tags { get; set; }
    }
} 
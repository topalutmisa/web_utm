using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    public class News
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime PublishDate { get; set; }
        public Guid AuthorId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsFeatured { get; set; }
        public int ViewCount { get; set; }
        public int ViewsCount { get; set; }
        
        // Навигационные свойства
        public virtual User Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
} 
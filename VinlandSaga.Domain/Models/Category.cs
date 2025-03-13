using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Slug { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        
        public Guid? ParentCategoryId { get; set; }
        
        
        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> ChildCategories { get; set; }
        public virtual ICollection<Fanart> Fanarts { get; set; }
        public virtual ICollection<ForumTopic> ForumTopics { get; set; }
    }
} 

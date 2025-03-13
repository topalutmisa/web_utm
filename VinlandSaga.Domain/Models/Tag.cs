using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int UsageCount { get; set; }
        
        public virtual ICollection<Fanart> Fanarts { get; set; }
        public virtual ICollection<ForumTopic> ForumTopics { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
    }
} 
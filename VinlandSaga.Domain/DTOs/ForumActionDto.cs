using System;

namespace VinlandSaga.Domain.DTOs
{
    public class ForumActionDto
    {
        public string Action { get; set; } // "CreateTopic", "CreatePost", "LikeTopic", "SubscribeToTopic"
        public Guid? TopicId { get; set; }
        public Guid? PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public Guid? CategoryId { get; set; }
    }
} 
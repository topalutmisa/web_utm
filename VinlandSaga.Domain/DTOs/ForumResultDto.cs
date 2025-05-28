using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.DTOs
{
    public class ForumResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public Guid? CreatedTopicId { get; set; }
        public Guid? CreatedPostId { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; }
        
        public ForumResultDto()
        {
            AdditionalData = new Dictionary<string, object>();
        }
    }
} 
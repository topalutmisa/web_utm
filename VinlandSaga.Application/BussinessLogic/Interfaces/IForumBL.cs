using System;
using System.Collections.Generic;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Domain.Models;

namespace VinlandSaga.Application.BussinessLogic.Interfaces
{
    public interface IForumBL
    {
        // Темы форума
        ForumResultDto CreateTopic(ForumActionDto actionDto);
        TopicDto GetTopic(Guid topicId);
        List<TopicDto> GetTopicsByCategory(Guid categoryId, int page = 1, int pageSize = 20);
        List<TopicDto> GetRecentTopics(int count = 10);
        List<TopicDto> GetFeaturedTopics(int count = 5);
        bool UpdateTopic(TopicDto topicDto);
        bool DeleteTopic(Guid topicId);
        
        // Посты
        ForumResultDto CreatePost(ForumActionDto actionDto);
        List<ForumPost> GetPostsByTopic(Guid topicId, int page = 1, int pageSize = 20);
        bool UpdatePost(Guid postId, string content);
        bool DeletePost(Guid postId);
        
        // Категории
        List<Category> GetAllCategories();
        Category GetCategory(Guid categoryId);
        bool CreateCategory(string name, string description);
        
        // Статистика
        int GetTopicsCount();
        int GetPostsCount();
        TopicDto GetLastTopic();
        
        // Поиск
        List<TopicDto> SearchTopics(string searchTerm, int page = 1, int pageSize = 20);
    }
} 
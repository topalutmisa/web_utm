using System;
using System.Collections.Generic;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Domain.Models;

namespace VinlandSaga.Application.BussinessLogic.Interfaces
{
    public interface INewsBL
    {
        // CRUD операции
        NewsDto GetNews(Guid newsId);
        List<NewsDto> GetAllNews(int page = 1, int pageSize = 20);
        bool CreateNews(NewsDto newsDto);
        bool UpdateNews(NewsDto newsDto);
        bool DeleteNews(Guid newsId);
        
        // Публикация
        bool PublishNews(Guid newsId);
        bool UnpublishNews(Guid newsId);
        List<NewsDto> GetPublishedNews(int page = 1, int pageSize = 20);
        List<NewsDto> GetDraftNews(int page = 1, int pageSize = 20);
        
        // Рекомендуемые новости
        List<NewsDto> GetFeaturedNews(int count = 5);
        bool SetNewsFeatured(Guid newsId, bool isFeatured);
        
        // Статистика
        int GetNewsCount();
        int GetPublishedNewsCount();
        NewsDto GetLatestNews();
        
        // Поиск
        List<NewsDto> SearchNews(string searchTerm, int page = 1, int pageSize = 20);
        List<NewsDto> GetNewsByAuthor(Guid authorId, int page = 1, int pageSize = 20);
        
        // Просмотры
        bool IncrementViewCount(Guid newsId);
        List<NewsDto> GetMostViewedNews(int count = 10);
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using VinlandSaga.Application.BussinessLogic.Core;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Domain.Models;

namespace VinlandSaga.Application.BussinessLogic.BLogic
{
    public class NewsBL : BaseApi, INewsBL
    {
        public NewsDto GetNews(Guid newsId)
        {
            var news = GetById<News>(newsId);
            return news != null ? MapToDto(news) : null;
        }

        public List<NewsDto> GetAllNews(int page = 1, int pageSize = 20)
        {
            var allNews = GetPaged<News>(page, pageSize);
            return allNews.OrderByDescending(n => n.PublishDate)
                         .Select(n => MapToDto(n))
                         .Where(dto => dto != null)
                         .ToList();
        }

        public bool CreateNews(NewsDto newsDto)
        {
            var news = new News
            {
                Id = Guid.NewGuid(),
                Title = newsDto.Title,
                Content = newsDto.Content,
                Summary = newsDto.Summary,
                AuthorId = newsDto.AuthorId,
                PublishedDate = DateTime.Now,
                PublishDate = DateTime.Now,
                IsPublished = false,
                IsFeatured = false,
                ViewCount = 0,
                ViewsCount = 0
            };

            return Create(news);
        }

        public bool UpdateNews(NewsDto newsDto)
        {
            var news = GetById<News>(newsDto.Id);
            if (news == null) return false;

            news.Title = newsDto.Title;
            news.Content = newsDto.Content;
            news.Summary = newsDto.Summary;
            news.IsFeatured = newsDto.IsFeatured;

            return Update(news);
        }

        public bool DeleteNews(Guid newsId)
        {
            return Delete<News>(newsId);
        }

        public bool PublishNews(Guid newsId)
        {
            var news = GetById<News>(newsId);
            if (news == null) return false;

            news.IsPublished = true;
            news.PublishedDate = DateTime.Now;
            news.PublishDate = DateTime.Now;
            
            return Update(news);
        }

        public bool UnpublishNews(Guid newsId)
        {
            var news = GetById<News>(newsId);
            if (news == null) return false;

            news.IsPublished = false;
            return Update(news);
        }

        public List<NewsDto> GetPublishedNews(int page = 1, int pageSize = 20)
        {
            try
            {
                var allNews = GetAll<News>();
                var skip = (page - 1) * pageSize;
                return allNews
                    .Where(n => n.IsPublished)
                    .OrderByDescending(n => n.PublishDate)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(n => MapToDto(n))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<NewsDto>();
            }
        }

        public List<NewsDto> GetDraftNews(int page = 1, int pageSize = 20)
        {
            try
            {
                var allNews = GetAll<News>();
                var skip = (page - 1) * pageSize;
                return allNews
                    .Where(n => !n.IsPublished)
                    .OrderByDescending(n => n.PublishDate)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(n => MapToDto(n))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<NewsDto>();
            }
        }

        public List<NewsDto> GetFeaturedNews(int count = 5)
        {
            try
            {
                var allNews = GetAll<News>();
                return allNews
                    .Where(n => n.IsPublished && n.IsFeatured)
                    .OrderByDescending(n => n.PublishDate)
                    .Take(count)
                    .Select(n => MapToDto(n))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<NewsDto>();
            }
        }

        public bool SetNewsFeatured(Guid newsId, bool isFeatured)
        {
            var news = GetById<News>(newsId);
            if (news == null) return false;

            news.IsFeatured = isFeatured;
            return Update(news);
        }

        public int GetNewsCount()
        {
            return Count<News>();
        }

        public int GetPublishedNewsCount()
        {
            try
            {
                var allNews = GetAll<News>();
                return allNews.Count(n => n.IsPublished);
            }
            catch
            {
                return 0;
            }
        }

        public NewsDto GetLatestNews()
        {
            try
            {
                var allNews = GetAll<News>();
                var news = allNews
                    .Where(n => n.IsPublished)
                    .OrderByDescending(n => n.PublishDate)
                    .FirstOrDefault();

                return news != null ? MapToDto(news) : null;
            }
            catch
            {
                return null;
            }
        }

        public List<NewsDto> GetLatestNews(int count)
        {
            try
            {
                var allNews = GetAll<News>();
                return allNews
                    .Where(n => n.IsPublished)
                    .OrderByDescending(n => n.PublishDate)
                    .Take(count)
                    .Select(n => MapToDto(n))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<NewsDto>();
            }
        }

        public List<NewsDto> SearchNews(string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                var allNews = GetAll<News>();
                var skip = (page - 1) * pageSize;
                return allNews
                    .Where(n => n.IsPublished && 
                               (n.Title.Contains(searchTerm) || 
                                n.Content.Contains(searchTerm) ||
                                n.Summary.Contains(searchTerm)))
                    .OrderByDescending(n => n.PublishDate)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(n => MapToDto(n))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<NewsDto>();
            }
        }

        public List<NewsDto> GetNewsByAuthor(Guid authorId, int page = 1, int pageSize = 20)
        {
            try
            {
                var allNews = GetAll<News>();
                var skip = (page - 1) * pageSize;
                return allNews
                    .Where(n => n.AuthorId == authorId)
                    .OrderByDescending(n => n.PublishDate)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(n => MapToDto(n))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<NewsDto>();
            }
        }

        public bool IncrementViewCount(Guid newsId)
        {
            var news = GetById<News>(newsId);
            if (news == null) return false;

            news.ViewsCount++;
            news.ViewCount++;
            return Update(news);
        }

        public List<NewsDto> GetMostViewedNews(int count = 10)
        {
            try
            {
                var allNews = GetAll<News>();
                return allNews
                    .Where(n => n.IsPublished)
                    .OrderByDescending(n => n.ViewsCount)
                    .Take(count)
                    .Select(n => MapToDto(n))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<NewsDto>();
            }
        }

        private NewsDto MapToDto(News news)
        {
            if (news == null) return null;

            try
            {
                var allUsers = GetAll<User>();
                var author = allUsers.FirstOrDefault(u => u.Id == news.AuthorId);

                return new NewsDto
                {
                    Id = news.Id,
                    Title = news.Title,
                    Content = news.Content,
                    Summary = news.Summary,
                    AuthorId = news.AuthorId,
                    AuthorName = author?.DisplayName ?? author?.Username ?? "Неизвестный автор",
                    PublishedDate = news.PublishedDate,
                    PublishDate = news.PublishDate,
                    IsPublished = news.IsPublished,
                    IsFeatured = news.IsFeatured,
                    ViewCount = news.ViewCount,
                    ViewsCount = news.ViewsCount
                };
            }
            catch
            {
                return null;
            }
        }
    }
} 
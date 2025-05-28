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
            try
            {
                var news = _context.News.FirstOrDefault(n => n.Id == newsId);
                if (news == null) return null;

                return MapToDto(news);
            }
            catch
            {
                return null;
            }
        }

        public List<NewsDto> GetAllNews(int page = 1, int pageSize = 20)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                return _context.News
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

        public bool CreateNews(NewsDto newsDto)
        {
            try
            {
                var news = new News
                {
                    Id = Guid.NewGuid(),
                    Title = newsDto.Title,
                    Content = newsDto.Content,
                    Summary = newsDto.Summary,
                    ImageUrl = newsDto.ImageUrl,
                    AuthorId = newsDto.AuthorId,
                    PublishedDate = DateTime.Now,
                    PublishDate = DateTime.Now,
                    IsPublished = newsDto.IsPublished,
                    IsFeatured = false,
                    ViewCount = 0,
                    ViewsCount = 0
                };

                _context.News.Add(news);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateNews(NewsDto newsDto)
        {
            try
            {
                var news = _context.News.FirstOrDefault(n => n.Id == newsDto.Id);
                if (news == null) return false;

                news.Title = newsDto.Title;
                news.Content = newsDto.Content;
                news.Summary = newsDto.Summary;
                news.ImageUrl = newsDto.ImageUrl;
                news.IsFeatured = newsDto.IsFeatured;

                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteNews(Guid newsId)
        {
            try
            {
                var news = _context.News.FirstOrDefault(n => n.Id == newsId);
                if (news == null) return false;

                _context.News.Remove(news);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool PublishNews(Guid newsId)
        {
            try
            {
                var news = _context.News.FirstOrDefault(n => n.Id == newsId);
                if (news == null) return false;

                news.IsPublished = true;
                news.PublishedDate = DateTime.Now;
                news.PublishDate = DateTime.Now;
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UnpublishNews(Guid newsId)
        {
            try
            {
                var news = _context.News.FirstOrDefault(n => n.Id == newsId);
                if (news == null) return false;

                news.IsPublished = false;
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<NewsDto> GetPublishedNews(int page = 1, int pageSize = 20)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                return _context.News
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
                var skip = (page - 1) * pageSize;
                return _context.News
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
                return _context.News
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
            try
            {
                var news = _context.News.FirstOrDefault(n => n.Id == newsId);
                if (news == null) return false;

                news.IsFeatured = isFeatured;
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetNewsCount()
        {
            try
            {
                return _context.News.Count();
            }
            catch
            {
                return 0;
            }
        }

        public int GetPublishedNewsCount()
        {
            try
            {
                return _context.News.Count(n => n.IsPublished);
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
                var news = _context.News
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

        public List<NewsDto> SearchNews(string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                return _context.News
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
                var skip = (page - 1) * pageSize;
                return _context.News
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
            try
            {
                var news = _context.News.FirstOrDefault(n => n.Id == newsId);
                if (news == null) return false;

                news.ViewsCount++;
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<NewsDto> GetMostViewedNews(int count = 10)
        {
            try
            {
                return _context.News
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
                var author = _context.Users.FirstOrDefault(u => u.Id == news.AuthorId);

                return new NewsDto
                {
                    Id = news.Id,
                    Title = news.Title,
                    Content = news.Content,
                    Summary = news.Summary,
                    ImageUrl = news.ImageUrl,
                    PublishDate = news.PublishDate,
                    AuthorName = author?.Username ?? "Неизвестно",
                    AuthorId = news.AuthorId,
                    IsPublished = news.IsPublished,
                    IsFeatured = news.IsFeatured,
                    ViewsCount = news.ViewsCount,
                    Tags = new string[0] // Можно расширить позже
                };
            }
            catch
            {
                return null;
            }
        }
    }
} 
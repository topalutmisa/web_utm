using System;
using System.Collections.Generic;
using System.Linq;
using VinlandSaga.Application.BussinessLogic.Core;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Domain.Models;

namespace VinlandSaga.Application.BussinessLogic.BLogic
{
    public class ForumBL : BaseApi, IForumBL
    {
        public ForumResultDto CreateTopic(ForumActionDto actionDto)
        {
            try
            {
                var topic = new ForumTopic
                {
                    Id = Guid.NewGuid(),
                    Title = actionDto.Title,
                    Description = actionDto.Content,
                    Content = actionDto.Content,
                    UserId = actionDto.UserId,
                    AuthorId = actionDto.UserId,
                    CategoryId = actionDto.CategoryId ?? Guid.Empty,
                    CreatedDate = DateTime.Now,
                    LastPostDate = DateTime.Now,
                    ViewsCount = 0,
                    PostsCount = 0,
                    IsPinned = false,
                    IsSticky = false,
                    IsLocked = false
                };

                _context.ForumTopics.Add(topic);
                SaveChanges();

                return new ForumResultDto
                {
                    IsSuccess = true,
                    Message = "Тема успешно создана",
                    CreatedTopicId = topic.Id
                };
            }
            catch (Exception ex)
            {
                return new ForumResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = $"Ошибка создания темы: {ex.Message}"
                };
            }
        }

        public TopicDto GetTopic(Guid topicId)
        {
            var topic = GetById<ForumTopic>(topicId);
            return topic != null ? MapTopicToDto(topic) : null;
        }

        public List<TopicDto> GetTopicsByCategory(Guid categoryId, int page = 1, int pageSize = 20)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                return _context.ForumTopics
                    .Where(t => t.CategoryId == categoryId)
                    .OrderByDescending(t => t.IsSticky)
                    .ThenByDescending(t => t.LastPostDate)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(t => MapTopicToDto(t))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<TopicDto>();
            }
        }

        public List<TopicDto> GetRecentTopics(int count = 10)
        {
            try
            {
                return _context.ForumTopics
                    .OrderByDescending(t => t.CreatedDate)
                    .Take(count)
                    .Select(t => MapTopicToDto(t))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<TopicDto>();
            }
        }

        public List<TopicDto> GetFeaturedTopics(int count = 5)
        {
            try
            {
                return _context.ForumTopics
                    .Where(t => t.IsSticky || t.ViewsCount > 100) // Критерии для рекомендуемых
                    .OrderByDescending(t => t.ViewsCount)
                    .Take(count)
                    .Select(t => MapTopicToDto(t))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<TopicDto>();
            }
        }

        public bool UpdateTopic(TopicDto topicDto)
        {
            var topic = GetById<ForumTopic>(topicDto.Id);
            if (topic == null) return false;

            try
            {
                topic.Title = topicDto.Title;
                topic.Content = topicDto.Content;
                topic.IsSticky = topicDto.IsSticky;
                topic.IsLocked = topicDto.IsLocked;

                return Update(topic);
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTopic(Guid topicId)
        {
            try
            {
                // Удаляем все посты в теме
                var posts = _context.ForumPosts.Where(p => p.TopicId == topicId).ToList();
                _context.ForumPosts.RemoveRange(posts);

                return Delete<ForumTopic>(topicId);
            }
            catch
            {
                return false;
            }
        }

        public ForumResultDto CreatePost(ForumActionDto actionDto)
        {
            try
            {
                var post = new ForumPost
                {
                    Id = Guid.NewGuid(),
                    Content = actionDto.Content,
                    UserId = actionDto.UserId,
                    AuthorId = actionDto.UserId,
                    TopicId = actionDto.TopicId ?? Guid.Empty,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,
                    LikesCount = 0
                };

                var success = Create(post);
                if (!success) 
                {
                    return new ForumResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Ошибка создания поста"
                    };
                }

                // Обновляем дату последнего поста в теме и счетчик постов
                var topic = GetById<ForumTopic>(actionDto.TopicId ?? Guid.Empty);
                if (topic != null)
                {
                    topic.LastPostDate = DateTime.Now;
                    topic.PostsCount = _context.ForumPosts.Count(p => p.TopicId == topic.Id);
                    Update(topic);
                }

                return new ForumResultDto
                {
                    IsSuccess = true,
                    Message = "Пост успешно создан",
                    CreatedPostId = post.Id
                };
            }
            catch (Exception ex)
            {
                return new ForumResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = $"Ошибка создания поста: {ex.Message}"
                };
            }
        }

        public List<ForumPost> GetPostsByTopic(Guid topicId, int page = 1, int pageSize = 20)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                return _context.ForumPosts
                    .Where(p => p.TopicId == topicId)
                    .OrderBy(p => p.CreatedDate)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();
            }
            catch
            {
                return new List<ForumPost>();
            }
        }

        public bool UpdatePost(Guid postId, string content)
        {
            var post = GetById<ForumPost>(postId);
            if (post == null) return false;

            try
            {
                post.Content = content;
                return Update(post);
            }
            catch
            {
                return false;
            }
        }

        public bool DeletePost(Guid postId)
        {
            return Delete<ForumPost>(postId);
        }

        public List<Category> GetAllCategories()
        {
            return GetAll<Category>().OrderBy(c => c.Name).ToList();
        }

        public Category GetCategory(Guid categoryId)
        {
            return GetById<Category>(categoryId);
        }

        public bool CreateCategory(string name, string description)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description
            };

            return Create(category);
        }

        public int GetTopicsCount()
        {
            return Count<ForumTopic>();
        }

        public int GetPostsCount()
        {
            return Count<ForumPost>();
        }

        public TopicDto GetLastTopic()
        {
            try
            {
                var topic = _context.ForumTopics
                    .OrderByDescending(t => t.CreatedDate)
                    .FirstOrDefault();

                return topic != null ? MapTopicToDto(topic) : null;
            }
            catch
            {
                return null;
            }
        }

        public List<TopicDto> SearchTopics(string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                return _context.ForumTopics
                    .Where(t => t.Title.Contains(searchTerm) || t.Content.Contains(searchTerm))
                    .OrderByDescending(t => t.LastPostDate)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(t => MapTopicToDto(t))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<TopicDto>();
            }
        }

        private TopicDto MapTopicToDto(ForumTopic topic)
        {
            if (topic == null) return null;

            try
            {
                var author = _context.Users.FirstOrDefault(u => u.Id == topic.AuthorId);
                var category = _context.Categories.FirstOrDefault(c => c.Id == topic.CategoryId);
                var postsCount = _context.ForumPosts.Count(p => p.TopicId == topic.Id);

                var lastPost = _context.ForumPosts
                    .Where(p => p.TopicId == topic.Id)
                    .OrderByDescending(p => p.CreatedDate)
                    .FirstOrDefault();

                var lastPostAuthor = lastPost != null ? 
                    _context.Users.FirstOrDefault(u => u.Id == lastPost.AuthorId)?.Username : 
                    author?.Username;

                return new TopicDto
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    Content = topic.Content,
                    CreatedDate = topic.CreatedDate,
                    LastPostDate = topic.LastPostDate ?? topic.CreatedDate,
                    AuthorName = author?.Username ?? "Неизвестно",
                    AuthorId = topic.AuthorId,
                    CategoryName = category?.Name ?? "Без категории",
                    CategoryId = topic.CategoryId,
                    PostsCount = postsCount,
                    ViewsCount = topic.ViewsCount,
                    IsSticky = topic.IsSticky,
                    IsLocked = topic.IsLocked,
                    IsFeatured = topic.ViewsCount > 100,
                    LastPostAuthor = lastPostAuthor ?? "Неизвестно"
                };
            }
            catch
            {
                return null;
            }
        }
    }
} 
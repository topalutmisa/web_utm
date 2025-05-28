using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VinlandSaga.Web.Models
{
    public class ForumIndexViewModel
    {
        public List<ForumTopicViewModel> Topics { get; set; } = new List<ForumTopicViewModel>();
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalTopics { get; set; }
        public Guid? SelectedCategoryId { get; set; }
    }

    public class ForumTopicViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastPostDate { get; set; }
        public int PostsCount { get; set; }
        public int ViewsCount { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPinned { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string AuthorName { get; set; }
        public string CategoryName { get; set; }
    }

    public class ForumTopicDetailsViewModel
    {
        public ForumTopicViewModel Topic { get; set; }
        public List<ForumPostViewModel> Posts { get; set; } = new List<ForumPostViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalPosts { get; set; }
        public bool CanReply { get; set; }
    }

    public class ForumPostViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid UserId { get; set; }
        public Guid TopicId { get; set; }
        public string AuthorName { get; set; }
        public bool CanEdit { get; set; }
    }

    public class CategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TopicsCount { get; set; }
    }

    public class CreateForumTopicViewModel
    {
        [Required(ErrorMessage = "Заголовок темы обязателен")]
        [Display(Name = "Заголовок темы")]
        [StringLength(200, ErrorMessage = "Заголовок не может быть длиннее 200 символов")]
        public string Title { get; set; }

        [Display(Name = "Описание темы")]
        [StringLength(500, ErrorMessage = "Описание не может быть длиннее 500 символов")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Категория")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Содержание первого сообщения обязательно")]
        [Display(Name = "Содержание сообщения")]
        public string FirstPostContent { get; set; }

        // Дублирующее свойство для совместимости с контроллером
        public string Content => FirstPostContent;
    }

    public class EditForumPostViewModel
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }

        [Required(ErrorMessage = "Содержание сообщения обязательно")]
        [Display(Name = "Содержание сообщения")]
        public string Content { get; set; }
    }
} 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VinlandSaga.Web.Models
{
    public class NewsViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishedDate { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public bool IsPublished { get; set; }
        public int ViewCount { get; set; }
    }

    public class NewsListViewModel
    {
        public List<NewsViewModel> News { get; set; } = new List<NewsViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalNews { get; set; }
    }

    public class NewsDetailsViewModel : NewsViewModel
    {
        // Дополнительные поля для детального просмотра могут быть добавлены здесь
    }

    public class CreateNewsViewModel
    {
        [Required(ErrorMessage = "Заголовок новости обязателен")]
        [Display(Name = "Заголовок")]
        [StringLength(200, ErrorMessage = "Заголовок не может быть длиннее 200 символов")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Содержание новости обязательно")]
        [Display(Name = "Содержание")]
        public string Content { get; set; }

        [Display(Name = "Краткое описание")]
        [StringLength(500, ErrorMessage = "Краткое описание не может быть длиннее 500 символов")]
        public string Summary { get; set; }

        [Display(Name = "URL изображения")]
        [StringLength(500, ErrorMessage = "URL изображения не может быть длиннее 500 символов")]
        public string ImageUrl { get; set; }

        [Display(Name = "Опубликовать")]
        public bool IsPublished { get; set; } = true;
    }

    public class EditNewsViewModel : CreateNewsViewModel
    {
        public Guid Id { get; set; }
    }
} 
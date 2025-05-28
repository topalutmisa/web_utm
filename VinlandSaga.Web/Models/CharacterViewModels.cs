using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VinlandSaga.Web.Models
{
    public class CharacterViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Age { get; set; }
        public string Occupation { get; set; }
        public string Status { get; set; }
        public string Clan { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
    }

    public class CharactersListViewModel
    {
        public List<CharacterViewModel> Characters { get; set; } = new List<CharacterViewModel>();
        public string SearchTerm { get; set; }
    }

    public class CreateCharacterViewModel
    {
        [Required(ErrorMessage = "Имя персонажа обязательно")]
        [Display(Name = "Имя персонажа")]
        [StringLength(100, ErrorMessage = "Имя не может быть длиннее 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Описание персонажа обязательно")]
        [Display(Name = "Описание")]
        [StringLength(2000, ErrorMessage = "Описание не может быть длиннее 2000 символов")]
        public string Description { get; set; }

        [Display(Name = "URL изображения")]
        [StringLength(500, ErrorMessage = "URL изображения не может быть длиннее 500 символов")]
        public string ImageUrl { get; set; }

        [Display(Name = "Возраст")]
        [StringLength(50, ErrorMessage = "Возраст не может быть длиннее 50 символов")]
        public string Age { get; set; }

        [Display(Name = "Род занятий")]
        [StringLength(200, ErrorMessage = "Род занятий не может быть длиннее 200 символов")]
        public string Occupation { get; set; }

        [Display(Name = "Статус")]
        [StringLength(100, ErrorMessage = "Статус не может быть длиннее 100 символов")]
        public string Status { get; set; }

        [Display(Name = "Клан")]
        [StringLength(200, ErrorMessage = "Клан не может быть длиннее 200 символов")]
        public string Clan { get; set; }

        [Display(Name = "Дата рождения")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Дата смерти")]
        public DateTime? DeathDate { get; set; }
    }

    public class EditCharacterViewModel : CreateCharacterViewModel
    {
        public Guid Id { get; set; }
    }
} 
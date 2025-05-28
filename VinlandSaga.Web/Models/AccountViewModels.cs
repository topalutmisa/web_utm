using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VinlandSaga.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Имя пользователя")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

    public class ProfileViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string About { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }

    public class EditProfileViewModel
    {
        [Display(Name = "Отображаемое имя")]
        [StringLength(100, ErrorMessage = "Отображаемое имя не может быть длиннее 100 символов")]
        public string DisplayName { get; set; }

        [Display(Name = "О себе")]
        [StringLength(500, ErrorMessage = "Описание не может быть длиннее 500 символов")]
        public string About { get; set; }

        [Display(Name = "URL аватара")]
        [StringLength(200, ErrorMessage = "URL аватара не может быть длиннее 200 символов")]
        public string AvatarUrl { get; set; }
    }

    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }

    public class UsersListViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public int TotalUsers { get; set; }
    }

    public class UserDetailsViewModel : UserViewModel
    {
        public string About { get; set; }
        public string AvatarUrl { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
} 
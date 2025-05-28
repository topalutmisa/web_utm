using System;

namespace VinlandSaga.Domain.DTOs
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
        public string About { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string[] Roles { get; set; }
        public int PostsCount { get; set; }
        public int TopicsCount { get; set; }
    }
} 
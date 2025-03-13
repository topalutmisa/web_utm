using System;
using System.Collections.Generic;

namespace VinlandSaga.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string AvatarUrl { get; set; }
        public string DisplayName { get; set; }
        public string About { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailConfirmed { get; set; }
        
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Fanart> Fanarts { get; set; }
        public virtual ICollection<ForumPost> ForumPosts { get; set; }
    }
} 
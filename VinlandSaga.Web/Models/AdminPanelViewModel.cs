using System;
using System.Collections.Generic;

namespace VinlandSaga.Web.Models
{
    public class AdminPanelViewModel
    {
        public int UsersCount { get; set; }
        public int RolesCount { get; set; }
        public int CategoriesCount { get; set; }
        public int TopicsCount { get; set; }
        public int PostsCount { get; set; }
        public int NewsCount { get; set; }
        public int CharactersCount { get; set; }
        
        public List<RecentUserViewModel> RecentUsers { get; set; } = new List<RecentUserViewModel>();
        public List<RecentTopicViewModel> RecentTopics { get; set; } = new List<RecentTopicViewModel>();
        
        public string Error { get; set; }
        public string UserError { get; set; }
        public string TopicError { get; set; }
    }
    
    public class RecentUserViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
    
    public class RecentTopicViewModel
    {
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ViewsCount { get; set; }
        public int PostsCount { get; set; }
    }
} 
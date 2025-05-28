using System;
using System.Collections.Generic;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Domain.Models;

namespace VinlandSaga.Application.BussinessLogic.Interfaces
{
    public interface IUserBL
    {
        // Аутентификация
        AuthResultDto Login(string email, string password);
        AuthResultDto Register(string username, string email, string password, string displayName);
        SignOutResultDto SignOut();
        
        // Управление пользователями
        UserProfileDto GetUserProfile(Guid userId);
        UserProfileDto GetUserProfile(string username);
        bool UpdateUserProfile(UserProfileDto userDto);
        bool DeleteUser(Guid userId);
        
        // Роли и права
        string[] GetUserRoles(Guid userId);
        bool IsUserInRole(Guid userId, string roleName);
        bool AddUserToRole(Guid userId, string roleName);
        bool RemoveUserFromRole(Guid userId, string roleName);
        
        // Статистика
        List<UserProfileDto> GetRecentUsers(int count = 10);
        List<UserProfileDto> GetActiveUsers(int count = 10);
        int GetTotalUsersCount();
        
        // Поиск
        List<UserProfileDto> SearchUsers(string searchTerm, int page = 1, int pageSize = 20);
    }
} 
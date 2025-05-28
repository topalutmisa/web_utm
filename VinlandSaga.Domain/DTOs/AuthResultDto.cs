using System;
using System.Web;

namespace VinlandSaga.Domain.DTOs
{
    public class AuthResultDto
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public Guid? UserId { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public HttpCookie AuthCookie { get; set; }
    }
} 
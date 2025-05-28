using System.Web;

namespace VinlandSaga.Domain.DTOs
{
    public class SignOutResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public HttpCookie ExpiredCookie { get; set; }
        public string RedirectUrl { get; set; }
    }
} 
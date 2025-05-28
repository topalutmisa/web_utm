using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace VinlandSaga.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Включаем поддержку legacy timestamp для PostgreSQL
            System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.User.Identity is FormsIdentity formsIdentity)
                {
                    // Получаем билет аутентификации из куки
                    FormsAuthenticationTicket ticket = formsIdentity.Ticket;
                    
                    // Разбиваем данные пользователя: userId|email|roles
                    string[] userData = ticket.UserData.Split('|');
                    if (userData.Length >= 3)
                    {
                        // Получаем строку ролей и разделяем их по запятой
                        string rolesString = userData[2];
                        string[] roles = rolesString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        
                        // Убираем лишние пробелы из ролей
                        for (int i = 0; i < roles.Length; i++)
                        {
                            roles[i] = roles[i].Trim();
                        }
                        
                        // Устанавливаем принципал с ролями
                        HttpContext.Current.User = new GenericPrincipal(formsIdentity, roles);
                    }
                }
            }
        }
    }
}
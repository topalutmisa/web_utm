using System;
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
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            // Настройка принципала для обработки IsInRole
        }
        
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.User.Identity is FormsIdentity formsIdentity)
                {
                    FormsAuthenticationTicket ticket = formsIdentity.Ticket;
                    string userData = ticket.UserData;
                    string[] userDataParts = userData.Split('|');
                    
                    if (userDataParts.Length >= 3)
                    {
                        string[] roles = { userDataParts[2] };
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(formsIdentity, roles);
                    }
                }
            }
        }
    }
}
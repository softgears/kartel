using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Kartel.Domain.Infrastructure.Routing;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;

namespace Kartel.Trade.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            RoutesManager.RegisterActionRoutes();

            // Регистрация роутов статических страниц
            using (var httpScope = Locator.BeginNestedHttpRequestScope())
            {
                var pagesRep = Locator.GetService<IStaticPagesRepository>();
                foreach (var staticPage in pagesRep.FindAll())
                {
                    RoutesManager.RegisterRoute("Static-page-" + staticPage.Id, staticPage.Route, new { controller = "Main", Action = "StaticPage", id = staticPage.Id });
                }
            }

            routes.Add("DomainRoute", new Kartel.Domain.Infrastructure.Routing.DomainRoute("www.{subdomain}.example.com", "{action}/{id}" , new { controller = "UserSite", action = "Subdomain", subdomain = "" }));

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Main", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
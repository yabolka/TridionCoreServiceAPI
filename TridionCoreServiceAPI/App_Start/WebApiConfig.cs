using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TridionCoreServiceAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { controller = "Values", action = "Get", id = RouteParameter.Optional }
            );

            config.EnableCors();
        }
    }
}

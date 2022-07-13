using System.Web.Routing;

namespace MvcForum.Web
{
    using MvcForum.Core.Ioc;
    using System.Configuration;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Unity;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));



            // Web API routes
            config.MapHttpAttributeRoutes();

            if (ConfigurationManager.AppSettings["AzurePlatform:ApplicationGateway:FQDN"] == "http://localhost:8888/")
            {
                var cors = new EnableCorsAttribute("localhost:5000", "*", "*");
                config.EnableCors(cors);
            }

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );

            config.Routes.MapHttpRoute(
                name: "Api_Get",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );

            config.Routes.MapHttpRoute(
                name: "Api_Post",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, action = "Post" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            // Configures Swagger.
            SwaggerUIConfig.Register(config);
        }
    }
}
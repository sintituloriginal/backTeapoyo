using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace proyecto
{
    public static class WebApiConfig
    {
        
        public static void Register(HttpConfiguration config)
        {
            //var cors = new EnableCorsAttribute("*", "Origin, X-Requested-With, Content-Type, Accept, Authorization, X-File-Name,cache-control", "GET, POST, PUT, DELETE, OPTIONS , PATCH");
            ////[EnableCors(origins: "http://localhost:8083/, http://localhost:8080/,http://localhost:8088/,http://192.168.1.9:8083/", headers: "*", methods: "GET, POST, PUT, DELETE, OPTIONS , PATCH")]
            //// Configuración y servicios de Web API
            //config.EnableCors(cors);
            // Configure Web API para usar solo la autenticación de token de portador.
           config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Rutas de Web API
            config.MapHttpAttributeRoutes();
           
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

    }
}

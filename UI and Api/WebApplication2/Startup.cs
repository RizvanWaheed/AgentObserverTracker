using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using AgentObserverWeb.Models;

[assembly: OwinStartup(typeof(AgentObserverWeb.Startup))]

namespace AgentObserverWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //var config = new HttpConfiguration();
            //app.Use<GlobalExceptionMiddleware>();
            //app.UseWebApi.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());
            //var config = new HttpConfiguration();
            //config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());
        }
    }
}

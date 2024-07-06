using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using AgentObserver.Authorization;
using AgentObserver.Authorization.Roles;
using AgentObserver.Authorization.Users;
using AgentObserver.Observer;
using AgentObserver.Users.Dto;
using AgentObserver.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AgentObserver.Web;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Abp.Dependency;

namespace AgentObserver.UserActivities
{
    public class AppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        public IConfigurationRoot Configuration { get; }

        public AppConfigurationAccessor(IWebHostEnvironment env)
        {
            //Configuration = env.GetAppConfiguration();
        }
    }
}


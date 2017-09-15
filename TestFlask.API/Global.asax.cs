using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Routing;
using TestFlask.Data.Repos;

namespace TestFlask.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // New code
            

            GlobalConfiguration.Configure((config) =>
            {
                WebApiConfig.Register(config);
                InitCounters(config);

                var cors = new EnableCorsAttribute("*", "*", "*");
                config.EnableCors(cors);
                config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            AreaRegistration.RegisterAllAreas();
        }

        //I did not like to initialize counters here :(
        private void InitCounters(HttpConfiguration config)
        {
            var counterRepo = config.DependencyResolver.GetService(typeof(ICounterRepo)) as ICounterRepo;

            counterRepo.InitCounters(new string[] { "project", "scenario", "step" });
        }
    }
}

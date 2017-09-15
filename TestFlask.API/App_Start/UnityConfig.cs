using Microsoft.Practices.Unity;
using MongoDB.Driver;
using System.Configuration;
using System.Web.Http;
using TestFlask.API.Controllers;
using TestFlask.API.Unity;
using TestFlask.Data.Repos;

namespace TestFlask.API
{
    public static class UnityConfig
    {
        public static void Register(HttpConfiguration config)
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            RegisterMongo(container);

            RegisterRepos(container);

            config.DependencyResolver = new UnityDependencyResolver(container);
        }

        private static void RegisterMongo(IUnityContainer container)
        {
            string connStr = ConfigurationManager.AppSettings["testFlaskMongoDbServer"];
            string dbName = ConfigurationManager.AppSettings["testFlaskMongoDbName"];

            container.RegisterType<IMongoClient, MongoClient>(new ContainerControlledLifetimeManager(), new InjectionConstructor(connStr));
            container.RegisterType<IMongoDatabase>(new ContainerControlledLifetimeManager(), new InjectionFactory(con => con.Resolve<IMongoClient>().GetDatabase(dbName)));
        }

        private static void RegisterRepos(IUnityContainer container)
        {
            container.RegisterType<IProjectRepo, ProjectRepo>();
            container.RegisterType<ICounterRepo, CounterRepo>();
            container.RegisterType<IScenarioRepo, ScenarioRepo>();
            container.RegisterType<IAssertionRepo, AssertionRepo>();
        }

    }
}
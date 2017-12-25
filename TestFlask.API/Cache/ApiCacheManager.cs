using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;

namespace TestFlask.API.Cache
{
    public interface IApiCacheManager
    {
        Scenario GetCachedScenario(long scenarioNo);
        Project GetCachedProject(string projectKey);
    }

    public class ApiCacheManager : IApiCacheManager
    {
        private readonly IScenarioRepo scenarioRepo;
        private readonly IProjectRepo projectRepo;

        public ApiCacheManager(IScenarioRepo pScenarioRepo, IProjectRepo pProjectRepo)
        {
            scenarioRepo = pScenarioRepo;
            projectRepo = pProjectRepo;
        }

        public Scenario GetCachedScenario(long scenarioNo)
        {
            var scenario = ApiCache.GetScenario(scenarioNo);

            if (scenario == null)
            {
                scenario = scenarioRepo.GetScenarioFlat(scenarioNo);

                if (scenario != null)
                {
                    ApiCache.AddScenario(scenario);
                }
            }

            return scenario;
        }

        public Project GetCachedProject(string projectKey)
        {
            var project = ApiCache.GetProject(projectKey);

            if (project == null)
            {
                project = projectRepo.Get(projectKey);

                if (project != null)
                {
                    ApiCache.AddProject(project);
                }
            }

            return project;
        }
    }
}
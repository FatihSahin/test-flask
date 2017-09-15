using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;

namespace TestFlask.API.Controllers
{
    /// <summary>
    /// Will provide project management interface
    /// </summary>
    public class ProjectController : ApiController
    {
        private readonly IProjectRepo projectRepo;
        private readonly IScenarioRepo scenarioRepo;

        public ProjectController(IProjectRepo pProjectRepo, IScenarioRepo pScenarioRepo)
        {
            projectRepo = pProjectRepo;
            scenarioRepo = pScenarioRepo;
        }

        [Route("api/project")]
        public IEnumerable<Project> Get()
        {
            return projectRepo.GetAll();
        }

        [Route("api/project/scenarios/{projectKey}")]
        
        public IEnumerable<Scenario> GetScenarios(string projectKey)
        {
            return scenarioRepo.GetScenariosFlatByProject(projectKey);
        }

        [Route("api/project")]
        public Project Post(Project project)
        {
            projectRepo.Insert(project);

            return project;
        }
    }
}
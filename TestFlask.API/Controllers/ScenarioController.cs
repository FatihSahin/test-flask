using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestFlask.API.Cache;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;

namespace TestFlask.API.Controllers
{
    /// <summary>
    /// Will provide scenario management interface
    /// </summary>
    public class ScenarioController : ApiController
    {
        private readonly IScenarioRepo scenarioRepo;

        public ScenarioController(IScenarioRepo pScenarioRepo)
        {
            scenarioRepo = pScenarioRepo;
        }

        [Route("api/scenario/{scenarioNo}")]
        public Scenario Get(long scenarioNo)
        {
            return scenarioRepo.GetScenarioFlat(scenarioNo);
        }

        [Route("api/scenario/deep/{scenarioNo}")]
        public Scenario GetDeep(long scenarioNo)
        {
            return scenarioRepo.GetScenario(scenarioNo);
        }

        [Route("api/scenario")]
        public Scenario Post(Scenario scenario)
        {
            var result = scenarioRepo.Insert(scenario);
            return result;
        }

        [Route("api/scenario")]
        public Scenario Put(Scenario scenario)
        {
            var result = scenarioRepo.Update(scenario);

            ApiCache.DeleteScenario(result.ScenarioNo);

            return result;
        }
    }
}
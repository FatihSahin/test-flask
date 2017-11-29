using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TestFlask.API.Cache;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;

namespace TestFlask.API.Controllers
{


    /// <summary>
    // Will provide variable management interface
    /// </summary>
    public class VariableController : ApiController
    {
        private readonly IVariableRepo variableRepo;

        public VariableController(IVariableRepo pVariableRepo)
        {
            variableRepo = pVariableRepo;
        }

        [Route("api/step/Variable/{stepNo}")]
        public IEnumerable<Variable> GetByStep(long stepNo)
        {
            return variableRepo.GetByStep(stepNo);
        }

        [Route("api/Variable")]
        public Variable Put(Variable variable)
        {
            return variableRepo.InsertOrUpdate(variable);
        }

        [Route("api/scenario/Variable/{scenarioNo}")]
        public IEnumerable<Variable> GetByScenario(long scenarioNo)
        {
            return variableRepo.GetByScenario(scenarioNo);
        }

        [Route("api/project/Variable/{projectKey}")]
        public IEnumerable<Variable> GetByProject(string projectKey)
        {
            return variableRepo.GetByProject(projectKey);
        }

    }
}
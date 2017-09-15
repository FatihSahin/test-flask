using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;

namespace TestFlask.API.Controllers
{
    public class AssertionController : ApiController
    {
        private readonly IAssertionRepo assertionRepo;

        public AssertionController(IAssertionRepo pAssertionRepo)
        {
            assertionRepo = pAssertionRepo;
        }

        [Route("api/step/assertion/{stepNo}")]
        public Assertion GetByStep(long stepNo)
        {
            return assertionRepo.GetByStep(stepNo);
        }

        [Route("api/step/assertion/{stepNo}")]
        public Assertion Put(Assertion assertion)
        {
            return assertionRepo.InsertOrUpdate(assertion);
        }

        [Route("api/scenario/assertion/{scenarioNo}")]
        public IEnumerable<Assertion> GetByScenario(long scenarioNo)
        {
            return assertionRepo.GetByScenario(scenarioNo);
        }

        [Route("api/project/assertion/{projectKey}")]
        public IEnumerable<Assertion> GetByScenario(string projectKey)
        {
            return assertionRepo.GetByProject(projectKey);
        }
    }
}
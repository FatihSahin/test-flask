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
    /// Will provide step management interface
    /// </summary>
    public class StepController : ApiController
    {
        private readonly IScenarioRepo scenarioRepo;

        public StepController(IScenarioRepo pScenarioRepo)
        {
            scenarioRepo = pScenarioRepo;
        }

        [Route("api/step/{stepNo}")]
        public Step Get(long stepNo)
        {
            return scenarioRepo.GetStep(stepNo);
        }

        [Route("api/step/invocations")]
        public void PutInvocations(Step step)
        {
            scenarioRepo.InsertInvocationsForStep(step);
        }

        [Route("api/step/")]
        public Step Post(Step step)
        {
            return scenarioRepo.InsertStep(step);
        }

        [Route("api/step/")]
        public Step Put(Step step)
        {
            scenarioRepo.UpdateStep(step);
            return step;
        }

        [Route("api/step/invocation")]
        public void PutInvocation(Invocation invocation)
        {
            scenarioRepo.UpdateInvocation(invocation);
        }

        [Route("api/step/invocation/{instanceHashCode}")]
        public Invocation GetInvocation(string instanceHashCode)
        {
            return scenarioRepo.GetInvocation(instanceHashCode);
        }
    }
}
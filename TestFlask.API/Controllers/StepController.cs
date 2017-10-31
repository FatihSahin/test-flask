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

        [Route("api/step/invocations/complete")]
        public void PutCompletedInvocations(Step step)
        {            
            var dbStep = scenarioRepo.GetStep(step.StepNo);

            dbStep.Invocations.AddRange(step.Invocations);

            scenarioRepo.InsertInvocationsForStep(dbStep);
        }

        [Route("api/step/invocations/append")]
        public void PutAppendedInvocations(Step step)
        {
            scenarioRepo.AppendInvocationsForStep(step);
        }

        [Route("api/step/invocations/{scenarioNo}/{stepNo}")]
        public void DeleteInvocations(long scenarioNo, long stepNo)
        {
            scenarioRepo.DeleteInvocationsForStep(scenarioNo, stepNo);
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

        [Route("api/step/shallow")]
        public Step PutShallow(Step step)
        {
            return scenarioRepo.UpdateStepShallow(step);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestFlask.API.Cache;
using TestFlask.API.InvocationVariable;
using TestFlask.API.Loader;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.API.Controllers
{
    /// <summary>
    /// Will provide step management interface
    /// </summary>
    public class StepController : ApiController
    {
        private readonly IScenarioRepo scenarioRepo;
        private readonly IStepLoader stepLoader;

        public StepController(IScenarioRepo pScenarioRepo, IStepLoader pStepLoader)
        {
            scenarioRepo = pScenarioRepo;
            stepLoader = pStepLoader;
        }

        [Route("api/step/{stepNo}")]
        public Step Get(long stepNo)
        {
            return scenarioRepo.GetStep(stepNo);
        }

        /// <summary>
        /// Loads a step and deternines matching strategy to properly load matching invocation on the player
        /// It also caches scenario to optimize assertion performance
        /// </summary>
        [Route("api/step/load/{stepNo}")]
        public Step GetLoad(long stepNo)
        {
            return stepLoader.Load(stepNo);     
        }

        [Route("api/step/invocations/complete")]
        public void PutCompletedInvocations(Step step)
        {
            var dbStep = scenarioRepo.GetStep(step.StepNo);
 
            dbStep.Invocations.AddRange(step.Invocations.OrderBy(i => i.Depth).ThenBy(i => i.InvocationIndex).ThenBy(i => i.RecordedOn));

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

        /// <summary>
        /// Matches an invocation by supplied invocation strategy
        /// </summary>
        [Route("api/step/invocation/{instanceHashCode}")]
        public Invocation GetInvocation(string instanceHashCode)
        {
            return scenarioRepo.GetInvocation(instanceHashCode);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestFlask.API.Cache;
using TestFlask.API.InvocationMatcher;
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

        public StepController(IScenarioRepo pScenarioRepo)
        {
            scenarioRepo = pScenarioRepo;
        }

        [Route("api/step/{stepNo}")]
        public Step Get(long stepNo)
        {
            return scenarioRepo.GetStep(stepNo);
        }

        /// <summary>
        /// Loads a step and mainpulates invocation responses using a matching strategy
        /// It also caches scenario to optimize assertion performance
        /// </summary>
        [Route("api/step/load/{stepNo}")]
        public Step GetLoad(long stepNo)
        {
            var step = scenarioRepo.GetStep(stepNo);

            //get cached scenario instance, if not cached fetch and cache
            var scenario = ApiCache.GetScenario(step.ScenarioNo);

            if (scenario == null)
            {
                scenario = scenarioRepo.GetScenarioFlat(step.ScenarioNo);

                if (scenario != null)
                {
                    ApiCache.AddScenario(scenario);
                }
            }

            MatcherStrategy matcher = new MatcherStrategyFactory(scenario, step).ProvideStrategy();
            matcher.Match();

            return step;
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
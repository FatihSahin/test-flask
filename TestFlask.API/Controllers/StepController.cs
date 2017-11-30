using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestFlask.API.Cache;
using TestFlask.API.InvocationMatcher;
using TestFlask.API.InvocationVariable;
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
        private readonly IProjectRepo projectRepo;
        private readonly IInvocationVariableProcessor variableProcessor;

        public StepController(IScenarioRepo pScenarioRepo, IProjectRepo pProjectRepo, IInvocationVariableProcessor pVariableProcessor)
        {
            scenarioRepo = pScenarioRepo;
            projectRepo = pProjectRepo;
            variableProcessor = pVariableProcessor;
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

            //get cached project instance, if not cached fetch and cache
            Project project = GetCachedProject(step);
            //get cached scenario instance, if not cached fetch and cache
            Scenario scenario = GetCachedScenario(step);

            Matcher matcher = new MatcherProvider(project, scenario, step).Provide();
            matcher.Match();

            variableProcessor.ResolveVariables(step);

            return step;
        }

        private Scenario GetCachedScenario(Step step)
        {
            var scenario = ApiCache.GetScenario(step.ScenarioNo);

            if (scenario == null)
            {
                scenario = scenarioRepo.GetScenarioFlat(step.ScenarioNo);

                if (scenario != null)
                {
                    ApiCache.AddScenario(scenario);
                }
            }

            return scenario;
        }

        private Project GetCachedProject(Step step)
        {
            var project = ApiCache.GetProject(step.ProjectKey);

            if (project == null)
            {
                project = projectRepo.Get(step.ProjectKey);

                if (project != null)
                {
                    ApiCache.AddProject(project);
                }
            }

            return project;
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
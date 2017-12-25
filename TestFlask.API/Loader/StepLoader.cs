using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.API.Cache;
using TestFlask.API.InvocationVariable;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.API.Loader
{
    public interface IStepLoader
    {
        Step Load(long stepNo);
    }

    public class StepLoader : IStepLoader
    {
        private readonly IScenarioRepo scenarioRepo;
        private readonly IApiCacheManager cacheManager;
        private readonly IInvocationVariableProcessor variableProcessor;

        public StepLoader(IScenarioRepo pScenarioRepo, IApiCacheManager pCacheManager, IInvocationVariableProcessor pVariableProcessor)
        {
            scenarioRepo = pScenarioRepo;
            cacheManager = pCacheManager;
            variableProcessor = pVariableProcessor;
        }

        public Step Load(long stepNo)
        {
            var step = scenarioRepo.GetStep(stepNo);

            //get cached project instance, if not cached fetch and cache
            Project project = cacheManager.GetCachedProject(step.ProjectKey);
            //get cached scenario instance, if not cached fetch and cache
            Scenario scenario = cacheManager.GetCachedScenario(step.ScenarioNo);

            InvocationMatch stepMatchStrategy = DetermineMatchStrategy(project, scenario, step);
            step.LoadedMatchStrategy = stepMatchStrategy;

            variableProcessor.ResolveVariables(step);

            return step;
        }

        private InvocationMatch DetermineMatchStrategy(Project project, Scenario scenario, Step step)
        {
            InvocationMatch scenarioMatchStrategy = scenario.InvocationMatchStrategy != InvocationMatch.Inherit
                ? scenario.InvocationMatchStrategy
                : project.InvocationMatchStrategy;

            InvocationMatch stepMatchStrategy = step.InvocationMatchStrategy != InvocationMatch.Inherit
                ? step.InvocationMatchStrategy
                : scenarioMatchStrategy;

            if (stepMatchStrategy == InvocationMatch.Inherit)
            {
                stepMatchStrategy = InvocationMatch.Exact; //Exact is the default strategy if not set in any level
            }

            return stepMatchStrategy;
        }
    }
}
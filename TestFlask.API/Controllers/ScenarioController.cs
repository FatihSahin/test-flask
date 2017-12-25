using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestFlask.API.Cache;
using TestFlask.API.Loader;
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
        private readonly ICounterRepo counterRepo;
        private readonly IStepLoader stepLoader;

        public ScenarioController(IScenarioRepo pScenarioRepo, ICounterRepo pCounterRepo, IStepLoader pStepLoader)
        {
            scenarioRepo = pScenarioRepo;
            counterRepo = pCounterRepo;
            stepLoader = pStepLoader;
        }

        [Route("api/scenario/{scenarioNo}")]
        public Scenario Get(long scenarioNo)
        {
            return scenarioRepo.GetScenarioFlat(scenarioNo);
        }

        [Route("api/scenario/load/{scenarioNo}")]
        public Scenario GetLoadScenario(long scenarioNo)
        {
            var scenario = scenarioRepo.GetScenarioFlat(scenarioNo);

            List<Step> loadedSteps = new List<Step>();

            foreach (var rawStep in scenario.Steps)
            {
                loadedSteps.Add(stepLoader.Load(rawStep.StepNo));
            }

            //override steps with loaded ones
            scenario.Steps = loadedSteps;

            return scenario;
        }

        [Route("api/scenario/clone/{scenarioNo}")]
        public Scenario PostCloneScenario(long scenarioNo)
        {
            Scenario scenario = scenarioRepo.GetScenarioDeep(scenarioNo);

            long oldScenarioNo = scenario.ScenarioNo;

            scenario.Id = null;
            scenario.ScenarioNo = counterRepo.GetNextCounter("scenario").CounterValue;
            scenario.ScenarioName = !string.IsNullOrWhiteSpace(scenario.ScenarioName) ? $"{scenario.ScenarioName.Trim()}_CloneOf{oldScenarioNo}" : $"CloneOf{oldScenarioNo}";

            foreach (Step step in scenario.Steps)
            {
                long oldStepNo = step.StepNo;
                step.ScenarioNo = scenario.ScenarioNo;
                step.CreatedOn = DateTime.UtcNow;

                step.StepNo = counterRepo.GetNextCounter("step").CounterValue;
                foreach (Invocation invocation in step.Invocations.OrderBy(i => i.Depth))
                {
                    invocation.StepNo = step.StepNo;
                    invocation.ScenarioNo = step.ScenarioNo;
                    // Recalculate hash codes
                    invocation.SignatureHashCode = invocation.GetSignatureHashCode();
                    invocation.RequestHashCode = invocation.GetRequestHashCode();
                    invocation.DeepHashCode = invocation.GetDeepHashCode();
                    invocation.LeafHashCode = invocation.GetLeafHashCode();

                    string oldInstanceHashCode = invocation.InstanceHashCode;
                    invocation.InstanceHashCode = invocation.GetInvocationInstanceHashCode();
                    //update parent instance hash code to preserve tree strucuture for new instance hash codes
                    foreach(var child in step.Invocations.Where(s => s.ParentInstanceHashCode == oldInstanceHashCode))
                    {
                        child.ParentInstanceHashCode = invocation.InstanceHashCode;
                    };
                }
            }

            var inserted = scenarioRepo.Insert(scenario, autoGenerateNos: false);

            return inserted;
        }

        [Route("api/scenario")]
        public Scenario Post(Scenario scenario)
        {
            var result = scenarioRepo.Insert(scenario, autoGenerateNos: true);
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
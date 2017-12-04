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
        private readonly ICounterRepo counterRepo;

        public ScenarioController(IScenarioRepo pScenarioRepo, ICounterRepo pCounterRepo)
        {
            scenarioRepo = pScenarioRepo;
            counterRepo = pCounterRepo;
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

        [Route("api/scenario/clone/{scenarioNo}")]
        public Scenario PostCloneScenario(long scenarioNo)
        {
            Scenario scenario = scenarioRepo.GetScenario(scenarioNo);

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestFlask.Models.Entity;

namespace TestFlask.Data.Repos
{
    public interface IScenarioRepo : IMongoRepo<Scenario>
    {
        Scenario Insert(Scenario scenario);
        Step GetStep(long stepNo);
        void InsertInvocationsForStep(Step step);
        IEnumerable<Scenario> GetScenariosFlatByProject(string projectKey);
        Scenario GetScenarioFlat(long scenarioNo);
        Step InsertStep(Step step);
        Step UpdateStep(Step step);
        Step UpdateStepShallow(Step step);
        Scenario Update(Scenario scenario);
        void UpdateInvocation(Invocation invocation);
        Invocation GetInvocation(string instanceHashCode);
        void AppendInvocationsForStep(Step step);
        void DeleteInvocationsForStep(long scenarioNo, long stepNo);
        Scenario GetScenario(long scenarioNo);
    }

    public class ScenarioRepo : MongoRepo<Scenario>, IScenarioRepo
    {
        private readonly ICounterRepo counterRepo;

        public ScenarioRepo(IMongoDatabase mongoDb, ICounterRepo pCounterRepo) : base(mongoDb, "scenarios")
        {
            counterRepo = pCounterRepo;
        }

        public Step GetStep(long stepNo)
        {
            var step = Collection.AsQueryable()
                .SelectMany(sc => sc.Steps)
                .SingleOrDefault(st => st.StepNo == stepNo);

            return step;
        }

        public Step InsertStep(Step step)
        {
            step.StepNo = counterRepo.GetNextCounter("step").CounterValue;
            step.CreatedOn = DateTime.UtcNow;

            if (step.Invocations == null)
            {
                step.Invocations = new List<Invocation>();
            }

            var scenarioFilter = Builders<Scenario>.Filter.Eq(sc => sc.ScenarioNo, step.ScenarioNo);
            var addStep = Builders<Scenario>.Update.Push("Steps", step);

            Collection.FindOneAndUpdate(scenarioFilter, addStep);

            return step;
        }

        public Step UpdateStep(Step step)
        {
            var scenarioStepFilter = Builders<Scenario>.Filter.And(
                Builders<Scenario>.Filter.Eq(sc => sc.ScenarioNo, step.ScenarioNo),
                Builders<Scenario>.Filter.Eq("Steps.StepNo", step.StepNo));

            var updateStep = Builders<Scenario>.Update.Set("Steps.$", step);

            Collection.FindOneAndUpdate(scenarioStepFilter, updateStep);

            return step;
        }

        public Step UpdateStepShallow(Step step)
        {
            Collection.FindOneAndUpdate(
               Builders<Scenario>.Filter.And(
                    Builders<Scenario>.Filter.Eq(sc => sc.ScenarioNo, step.ScenarioNo),
                    Builders<Scenario>.Filter.Eq("Steps.StepNo", step.StepNo)
               ),
               Builders<Scenario>.Update.Combine(
                   Builders<Scenario>.Update.Set("Steps.$.StepName", step.StepName),
                   Builders<Scenario>.Update.Set("Steps.$.RootInvocationReflectedType", step.RootInvocationReflectedType)
               )
            );

            return step;
        }

        public Scenario Insert(Scenario scenario)
        {
            scenario.ScenarioNo = counterRepo.GetNextCounter("scenario").CounterValue;
            scenario.CreatedOn = DateTime.UtcNow;

            if (scenario.Steps != null)
            {
                foreach (var step in scenario.Steps)
                {
                    step.StepNo = counterRepo.GetNextCounter("step").CounterValue;
                    step.ScenarioNo = scenario.ScenarioNo;

                    if (step.Invocations == null)
                    {
                        step.Invocations = new List<Invocation>();
                    }
                }
            }
            else
            {
                scenario.Steps = new List<Step>();
            }

            Collection.InsertOne(scenario);

            return scenario;
        }

        public Scenario Update(Scenario scenario)
        {
            Collection.FindOneAndUpdate(
                Builders<Scenario>.Filter.Eq(s => s.Id, scenario.Id),
                Builders<Scenario>.Update.Combine(
                    Builders<Scenario>.Update.Set(s => s.ScenarioName, scenario.ScenarioName),
                    Builders<Scenario>.Update.Set(s => s.ScenarioDescription, scenario.ScenarioDescription),
                    Builders<Scenario>.Update.Set(s => s.InvocationMatchStrategy, scenario.InvocationMatchStrategy)
                )
            );

            return scenario;
        }

        public void InsertInvocationsForStep(Step step)
        {
            //set rootinvocation container type info in step level
            var root = step.Invocations.SingleOrDefault(i => i.Depth == 1);
            if (root != null)
            {
                step.RootInvocationReflectedType = root.ReflectedType;
            }

            var scenarioFilter = Builders<Scenario>.Filter.Eq(sc => sc.ScenarioNo, step.ScenarioNo);
            var stepFilter = Builders<Scenario>.Filter.Eq("Steps.StepNo", step.StepNo);

            var compositeFilter = Builders<Scenario>.Filter.And(scenarioFilter, stepFilter);

            var deleteInvocations = Builders<Scenario>.Update.PullFilter("Steps.$.Invocations", Builders<Invocation>.Filter.Empty);

            Collection.FindOneAndUpdate(compositeFilter, deleteInvocations);

            var addInvocations = Builders<Scenario>.Update.PushEach("Steps.$.Invocations", step.Invocations);

            Collection.FindOneAndUpdate(compositeFilter, addInvocations);

            UpdateStepShallow(step);
        }

        public void DeleteInvocationsForStep(long scenarioNo, long stepNo)
        {
            var scenarioFilter = Builders<Scenario>.Filter.Eq(sc => sc.ScenarioNo, scenarioNo);
            var stepFilter = Builders<Scenario>.Filter.Eq("Steps.StepNo", stepNo);

            var compositeFilter = Builders<Scenario>.Filter.And(scenarioFilter, stepFilter);

            var deleteInvocations = Builders<Scenario>.Update.PullFilter("Steps.$.Invocations", Builders<Invocation>.Filter.Empty);

            Collection.FindOneAndUpdate(compositeFilter, deleteInvocations);
        }

        public void AppendInvocationsForStep(Step step)
        {
            var scenarioFilter = Builders<Scenario>.Filter.Eq(sc => sc.ScenarioNo, step.ScenarioNo);
            var stepFilter = Builders<Scenario>.Filter.Eq("Steps.StepNo", step.StepNo);

            var compositeFilter = Builders<Scenario>.Filter.And(scenarioFilter, stepFilter);

            var addInvocations = Builders<Scenario>.Update.PushEach("Steps.$.Invocations", step.Invocations.OrderBy(i => i.Depth).ThenBy(i => i.InvocationIndex).ThenBy(i => i.RecordedOn));

            Collection.FindOneAndUpdate(compositeFilter, addInvocations);
        }

        public IEnumerable<Scenario> GetScenariosFlatByProject(string projectKey)
        {
            return Collection
                .Find(Builders<Scenario>.Filter.Eq(sc => sc.ProjectKey, projectKey))
                .Project<Scenario>(Builders<Scenario>.Projection.Exclude(sc => sc.Steps))
                .ToEnumerable();
        }

        public Scenario GetScenarioFlat(long scenarioNo)
        {
            return Collection
               .Find(Builders<Scenario>.Filter.Eq(sc => sc.ScenarioNo, scenarioNo))
               .Project<Scenario>(Builders<Scenario>.Projection.Exclude("Steps.Invocations"))
               .SingleOrDefault();
        }

        public Scenario GetScenario(long scenarioNo)
        {
            return Collection
               .Find(Builders<Scenario>.Filter.Eq(sc => sc.ScenarioNo, scenarioNo))
               .SingleOrDefault();
        }

        /// <summary>
        /// This method updates assertion result of any given invocation
        /// </summary>
        public void UpdateInvocation(Invocation invocation)
        {
            //could not find any way to properly update 2 level nested invocation object by itself
            //So instead i updated step as a whole with modified invocation object.
            var step = GetStep(invocation.StepNo);

            var dbInvocation = step.Invocations.Single(i => i.InstanceHashCode == invocation.InstanceHashCode);
            dbInvocation.AssertionResult = invocation.AssertionResult;

            UpdateStep(step);
        }

        public Invocation GetInvocation(string instanceHashCode)
        {
            var scenarioFilter = Builders<Scenario>.Filter.Eq("Steps.Invocations.InstanceHashCode", instanceHashCode);
            var scenario = Collection.Find(scenarioFilter).SingleOrDefault();
            return scenario?.Steps.SelectMany(st => st.Invocations).FirstOrDefault(i => i.InstanceHashCode == instanceHashCode);
        }
    }
}

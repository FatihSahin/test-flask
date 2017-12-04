using System;
using MongoDB.Driver;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TestFlask.API.Tests.IntegrationTests.Data
{
    [TestFixture, Category("Integration")]
    public class ScenarioRepoTests : BaseRepoTests
    {
        [Test]
        public void Test_Int_GetStep_Success()
        {
            CounterRepo counterRepo = new CounterRepo(db);
            ScenarioRepo scenarioRepo = new ScenarioRepo(db, counterRepo);

            Scenario scenario = new Scenario
            {
                ProjectKey = "p1",
                ScenarioName = "sc name",
                Steps = new List<Step>()
            };

            scenario.Steps.Add(new Step
            {
                ProjectKey = "p1",
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        ProjectKey = "p1",
                        InstanceHashCode = "someInvocationCode"
                    }
                }
            });

            scenarioRepo.Insert(scenario, autoGenerateNos: true);

            long stepNo = scenario.Steps[0].StepNo;

            var step = scenarioRepo.GetStep(stepNo);

            Assert.IsNotNull(step);
            Assert.AreEqual("p1", step.ProjectKey);
        }


        [Test]
        public void Test_Int_InsertInvocationsForStep_Success()
        {
            CounterRepo counterRepo = new CounterRepo(db);
            ScenarioRepo scenarioRepo = new ScenarioRepo(db, counterRepo);

            var scenario = scenarioRepo.Insert(new Scenario
            {
                ScenarioName = "aScenarioName",
                Steps = new List<Step>
                {
                    new Step
                    {
                        StepName = "aStepName"
                    },
                    new Step
                    {
                        StepName = "bStepName"
                    }
                }
            }, autoGenerateNos: true);

            var aStep = scenario.Steps.First();

            Step dummyStep = new Step
            {
                ScenarioNo = scenario.ScenarioNo,
                StepNo = aStep.StepNo,
                Invocations = new List<Invocation>
                {
                    new Invocation { ScenarioNo = scenario.ScenarioNo, StepNo = aStep.StepNo, InstanceHashCode = "i1" },
                    new Invocation { ScenarioNo = scenario.ScenarioNo, StepNo = aStep.StepNo, InstanceHashCode = "i2" }
                }
            };

            scenarioRepo.InsertInvocationsForStep(dummyStep);
        }

        [Test]
        public void Test_Int_InsertStep_Success()
        {
            CounterRepo counterRepo = new CounterRepo(db);
            ScenarioRepo scenarioRepo = new ScenarioRepo(db, counterRepo);

            var scenario = scenarioRepo.Insert(new Scenario
            {
                ScenarioName = "stepfulScenarioName",
            }, autoGenerateNos: true);

            Step dummyStep = new Step
            {
                ScenarioNo = scenario.ScenarioNo,
                StepName = "someEmptyStepName"
            };

            scenarioRepo.InsertStep(dummyStep);
        }

        [Test]
        public void Test_Int_UpdateStep_Success()
        {
            CounterRepo counterRepo = new CounterRepo(db);
            ScenarioRepo scenarioRepo = new ScenarioRepo(db, counterRepo);

            var scenario = scenarioRepo.Insert(new Scenario
            {
                ScenarioName = "stepfulScenarioName",
            }, autoGenerateNos: true);

            Step dummyStep = new Step
            {
                ScenarioNo = scenario.ScenarioNo,
                StepName = "someEmptyStepName", 
            };

            var dbStep = scenarioRepo.InsertStep(dummyStep);

            Step dummyStep2 = new Step
            {
                ScenarioNo = scenario.ScenarioNo,
                StepName = "someEmptyStepName2",
                Invocations = new List<Invocation> {
                    new Invocation {
                         ScenarioNo = scenario.ScenarioNo,
                         InvocationSignature = "signature1"
                    }
                }
            };

            var dbStep2 = scenarioRepo.InsertStep(dummyStep2);

            dbStep2.StepName = "updatedStepName2";
            dbStep2.Invocations.First().InvocationSignature = "updatedSignature1";

            scenarioRepo.UpdateStep(dbStep2);
        }

        [Test]
        public void Test_Int_GetInvocation_Success()
        {
            CounterRepo counterRepo = new CounterRepo(db);
            ScenarioRepo scenarioRepo = new ScenarioRepo(db, counterRepo);

            var invocation = scenarioRepo.GetInvocation("1001-1001-2136472689--1922254082-2-0");

            Assert.IsNotNull(invocation);
        }

        [Test]
        public void Test_Int_UpdateInvocation_Success()
        {
            CounterRepo counterRepo = new CounterRepo(db);
            ScenarioRepo scenarioRepo = new ScenarioRepo(db, counterRepo);

            string now = DateTime.Now.ToString();

            var invocation = scenarioRepo.GetInvocation("1001-31-1540425389--1922254082-1-0");
            invocation.AssertionResult = now; //meaningless update for test's sake

            scenarioRepo.UpdateInvocation(invocation);

            var updatedInvocation = scenarioRepo.GetInvocation("1001-31-1540425389--1922254082-1-0");
            Assert.AreEqual(now, updatedInvocation.AssertionResult);
        }
    }
}

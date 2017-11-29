using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFlask.Models.Entity;

namespace TestFlask.API.Tests.UnitTests
{
    [TestClass]
    public class VariableTest
    {
        [TestMethod]
        public void Level_should_be_step_when_step_no_bigger_than_zero()
        {
            var variable = new Variable
            {
                ProjectKey = "",
                ScenarioNo = 1,
                StepNo = 1
            };

            Assert.AreEqual(variable.GetLevel(), Models.Enums.VariableLevel.Step);
        }

        [TestMethod]
        public void Level_should_be_scenario_when_step_no_is_zero()
        {
            var variable = new Variable
            {
                ProjectKey = "",
                ScenarioNo = 1,
                StepNo = 0
            };

            Assert.AreEqual(variable.GetLevel(), Models.Enums.VariableLevel.Scenario);
        }

        public void Level_should_be_scenario_when_step_no_and_scenario_no_are_zero()
        {
            var variable = new Variable
            {
                ProjectKey = "",
                ScenarioNo = 0,
                StepNo = 0
            };

            Assert.AreEqual(variable.GetLevel(), Models.Enums.VariableLevel.Scenario);
        }
    }
}

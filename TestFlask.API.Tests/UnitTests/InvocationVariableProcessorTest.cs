using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestFlask.API.InvocationVariable;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;

namespace TestFlask.API.Tests.UnitTests
{
    [TestClass]
    public class InvocationVariableProcessorTest
    {
        [TestMethod]
        public void TestProcess1()
        {
            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="mailAddress",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=1,
                Value="sahinnunlu@gmail.com",
                InvocationVariableRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            } };

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            processor.ValueToVariable("1", new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "Merhaba mail adresim sahinnunlu@gmail.com sahin@yahoo.com",
                        Depth = 1
                    }
                }
            });
        }

        [TestMethod]
        public void TestProcess2()
        {
            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="mailAddress",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=1,
                Value="sahinnunlu@gmail.com",
                InvocationVariableRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            } };

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            processor.VariableToValue("1", new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "Merhaba mail adresim {{mailAddress}}",
                        Depth = 1
                    }
                }
            });
        }

        [TestMethod]
        public void TestProcess3()
        {
            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="mailAddress",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=1,
                Value="sahinnunlu@gmail.com",
                InvocationVariableRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            },
            new Variable {
                Id = "var2",
                IsEnabled=true,
                Name="mailAddress1",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=0,
                Value="sahinnunlu1@gmail.com",
                InvocationVariableRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"
            } };

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = processor.VariableToValue("1", new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "Merhaba mail adresim {{mailAddress}} {{mailAddress1}}",
                        Depth = 1
                    }
                }
            });

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("Merhaba mail adresim sahinnunlu@gmail.com sahinnunlu1@gmail.com", actual);
        }
    }
}

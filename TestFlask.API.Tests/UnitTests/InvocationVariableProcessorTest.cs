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
        public void Should_replace_mail_address_with_variable()
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
                Value="xxxyyy@gmail.com",
                InvocationVariableRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            }};

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = processor.ValueToVariable("1", new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "Mails xxxyyy@gmail.com",
                        Depth = 1
                    }
                }
            });

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("Mails {{mailAddress}}", actual);
        }

        [TestMethod]
        public void Should_replace_variable_with_mail_address()
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
                Value="xxxyyy@gmail.com",
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
                        RequestRaw = "mails {{mailAddress}}",
                        Depth = 1
                    }
                }
            });

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("mails xxxyyy@gmail.com", actual);
        }

        [TestMethod]
        public void Should_replace_url_with_variable()
        {
            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="url",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=1,
                Value="https://www.google.com",
                InvocationVariableRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"

            }};

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = processor.ValueToVariable("1", new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "urls https://www.google.com",
                        Depth = 1
                    }
                }
            });

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("urls {{url}}", actual);
        }

        [TestMethod]
        public void Should_replace_variable_with_url()
        {
            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="url",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=1,
                Value="https://www.google.com",
                InvocationVariableRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"
            } };

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = processor.VariableToValue("1", new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "url {{url}}",
                        Depth = 1
                    }
                }
            });

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("url https://www.google.com", actual);
        }

        [TestMethod]
        public void Should_replace_variables_with_url_and_mail()
        {
            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="url",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=1,
                Value="https://www.google.com",
                InvocationVariableRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"
            },new Variable
            {
                Id = "var2",
                IsEnabled=true,
                Name="mailAddress",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=1,
                Value="xxxyyy@gmail.com",
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
                        RequestRaw = "url {{url}} {{mailAddress}}",
                        Depth = 1
                    }
                }
            });

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("url https://www.google.com xxxyyy@gmail.com", actual);
        }

        [TestMethod]
        public void Should_replace_mail_address_and_url_with_variables()
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
                Value="xxxyyy@gmail.com",
                InvocationVariableRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            },new Variable
            {
                Id = "var2",
                IsEnabled=true,
                Name="url",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=1,
                Value="https://www.google.com",
                InvocationVariableRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"

            }};

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = processor.ValueToVariable("1", new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "Mails xxxyyy@gmail.com https://www.google.com",
                        Depth = 1
                    }
                }
            });

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("Mails {{mailAddress}} {{url}}", actual);
        }

        [TestMethod]
        public void Should_apply_step_variable()
        {
            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "stepVar",
                IsEnabled=true,
                Name="mailAddress",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=1,
                Value="xxxyyy@gmail.com",
                InvocationVariableRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            },new Variable
            {
                Id = "scenVar",
                IsEnabled=true,
                Name="mailAddress1",
                ProjectKey="1",
                ScenarioNo=1,
                StepNo=0,
                Value="xxxyyy@gmail.com",
                InvocationVariableRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            }};

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = processor.ValueToVariable("1", new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "Mails xxxyyy@gmail.com",
                        Depth = 1
                    }
                }
            });


            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("Mails {{mailAddress}}", actual);
        }
    }
}

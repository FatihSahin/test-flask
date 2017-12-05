using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TestFlask.API.Cache;
using TestFlask.API.InvocationVariable;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;

namespace TestFlask.API.Tests.UnitTests
{
    [TestFixture]
    public class InvocationVariableProcessorTest
    {
        string ProjectKey = "testProject";

        [Test]
        public void VariableProcessor_should_replace_mail_address_with_mail_address_variable()
        {
            ApiCache.DeleteVariableByProject(ProjectKey);

            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="mailAddress",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=1,
                Value="xxxyyy@gmail.com",
                GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            },
            new Variable
            {
                Id = "var2",
                IsEnabled=true,
                Name="mailAddress2",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=0,
                Value="xxxyyyzz@gmail.com",
                GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"
            },new Variable
            {
                Id = "var3",
                IsEnabled=true,
                Name="mailAddress3",
                ProjectKey=ProjectKey,
                ScenarioNo=0,
                StepNo=0,
                Value="xxxyyyzz@gmail.com",
                GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"
            }};

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "Mails xxxyyy@gmail.com",
                        Depth = 1
                    }
                },

                ProjectKey = ProjectKey,
                ScenarioNo = 1,
                StepNo = 1

            };

            processor.GenerateVariables(step);

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("Mails {{mailAddress}}", actual);
        }

        [Test]
        public void VariableProcessor_should_replace_mail_address_variable_with_mail_address()
        {
            ApiCache.DeleteVariableByProject(ProjectKey);

            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="mailAddress",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=1,
                Value="xxxyyy@gmail.com",
                GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            } };

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "mails {{mailAddress}}",
                        Depth = 1
                    }
                },


                ProjectKey = ProjectKey,
                ScenarioNo = 1,
                StepNo = 1
            };

            processor.ResolveVariables(step);

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("mails xxxyyy@gmail.com", actual);
        }

        [Test]
        public void VariableProcessor_should_replace_url_with_url_variable()
        {
            ApiCache.DeleteVariableByProject(ProjectKey);

            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="url",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=1,
                Value="https://www.google.com",
                GeneratorRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"

            }};

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "urls https://www.google.com",
                        Depth = 1
                    }
                },


                ProjectKey = ProjectKey,
                ScenarioNo = 1,
                StepNo = 1
            };

            processor.GenerateVariables(step);

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("urls {{url}}", actual);
        }

        [Test]
        public void VariableProcessor_should_replace_variable_with_url()
        {
            ApiCache.DeleteVariableByProject(ProjectKey);

            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="url",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=1,
                Value="https://www.google.com",
                GeneratorRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"
            } };

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "url {{url}}",
                        Depth = 1
                    }
                },

                ProjectKey = ProjectKey,
                ScenarioNo = 1,
                StepNo = 1
            };

            processor.ResolveVariables(step);

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("url https://www.google.com", actual);
        }

        [Test]
        public void VariableProcessor_should_replace_url_and_mail_variables_with_url_and_mail()
        {
            ApiCache.DeleteVariableByProject(ProjectKey);

            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="url",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=1,
                Value="https://www.google.com",
                GeneratorRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"
            },new Variable
            {
                Id = "var2",
                IsEnabled=true,
                Name="mailAddress",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=1,
                Value="xxxyyy@gmail.com",
                GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            } };

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "url {{url}} {{mailAddress}}",
                        Depth = 1
                    }
                },
                ProjectKey = ProjectKey,
                ScenarioNo = 1,
                StepNo = 1
            };

            processor.ResolveVariables(step);

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("url https://www.google.com xxxyyy@gmail.com", actual);
        }

        [Test]
        public void VariableProcessor_should_replace_mail_address_and_url_with_url_and_mail_variables()
        {
            ApiCache.DeleteVariableByProject(ProjectKey);

            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "var1",
                IsEnabled=true,
                Name="mailAddress",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=1,
                Value="xxxyyy@gmail.com",
                GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            },new Variable
            {
                Id = "var2",
                IsEnabled=true,
                Name="url",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=1,
                Value="https://www.google.com",
                GeneratorRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"

            }};

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "Mails xxxyyy@gmail.com https://www.google.com",
                        Depth = 1
                    }
                },

                ProjectKey = ProjectKey,
                ScenarioNo = 1,
                StepNo = 1
            };

            processor.GenerateVariables(step);

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("Mails {{mailAddress}} {{url}}", actual);
        }

        [Test]
        public void VariableProcessor_should_apply_step_variable()
        {
            ApiCache.DeleteVariableByProject(ProjectKey);

            Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
            var vars = new List<Variable>{new Variable
            {
                Id = "stepVar",
                IsEnabled=true,
                Name="mailAddress",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=1,
                Value="xxxyyy@gmail.com",
                GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            },new Variable
            {
                Id = "scenVar",
                IsEnabled=true,
                Name="mailAddress1",
                ProjectKey=ProjectKey,
                ScenarioNo=1,
                StepNo=0,
                Value="xxxyyy@gmail.com",
                GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

            }};

            repo.Setup<IEnumerable<Variable>>(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
            InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

            var step = new Step
            {
                Invocations = new List<Invocation>
                {
                    new Invocation
                    {
                        RequestRaw = "Mails xxxyyy@gmail.com",
                        Depth = 1
                    }
                },

                ProjectKey = ProjectKey,
                ScenarioNo = 1,
                StepNo = 1
            };

            processor.GenerateVariables(step);

            var actual = step.Invocations[0].RequestRaw;
            Assert.AreEqual("Mails {{mailAddress}}", actual);
        }
    }
}

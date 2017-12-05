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
	public class InvocationVariableProcessorTests
	{
		string projectKey = "testProject";

		[Test]
		public void VariableProcessor_GeneratesMailAddressVariable()
		{
			ApiCache.DeleteVariableByProject(projectKey);

			Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
			var vars = new List<Variable> { new Variable
			{
				Id = "var1",
				IsEnabled=true,
				Name="mailAddress",
				ProjectKey=projectKey,
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
				ProjectKey=projectKey,
				ScenarioNo=1,
				StepNo=0,
				Value="xxxyyyzz@gmail.com",
				GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"
			},
			new Variable
			{
				Id = "var3",
				IsEnabled=true,
				Name="mailAddress3",
				ProjectKey=projectKey,
				ScenarioNo=0,
				StepNo=0,
				Value="xxxyyyzz@gmail.com",
				GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"
			}};

			repo.Setup(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
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

				ProjectKey = projectKey,
				ScenarioNo = 1,
				StepNo = 1

			};

			processor.GenerateVariables(step);

			var actual = step.Invocations[0].RequestRaw;
			Assert.AreEqual("Mails {{mailAddress}}", actual);
		}

		[Test]
		public void VariableProcessor_ResolvesMailAddress()
		{
			ApiCache.DeleteVariableByProject(projectKey);

			Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
			var vars = new List<Variable>{new Variable
			{
				Id = "var1",
				IsEnabled=true,
				Name="mailAddress",
				ProjectKey=projectKey,
				ScenarioNo=1,
				StepNo=1,
				Value="xxxyyy@gmail.com",
				GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

			} };

			repo.Setup(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
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


				ProjectKey = projectKey,
				ScenarioNo = 1,
				StepNo = 1
			};

			processor.ResolveVariables(step);

			var actual = step.Invocations[0].RequestRaw;
			Assert.AreEqual("mails xxxyyy@gmail.com", actual);
		}

		[Test]
		public void VariableProcessor_GeneratesUrlVariable()
		{
			ApiCache.DeleteVariableByProject(projectKey);

			Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
			var vars = new List<Variable>{new Variable
			{
				Id = "var1",
				IsEnabled=true,
				Name="url",
				ProjectKey=projectKey,
				ScenarioNo=1,
				StepNo=1,
				Value="https://www.google.com",
				GeneratorRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"

			}};

			repo.Setup(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
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


				ProjectKey = projectKey,
				ScenarioNo = 1,
				StepNo = 1
			};

			processor.GenerateVariables(step);

			var actual = step.Invocations[0].RequestRaw;
			Assert.AreEqual("urls {{url}}", actual);
		}

		[Test]
		public void VariableProcessor_ResolvesUrl()
		{
			ApiCache.DeleteVariableByProject(projectKey);

			Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
			var vars = new List<Variable>{new Variable
			{
				Id = "var1",
				IsEnabled=true,
				Name="url",
				ProjectKey=projectKey,
				ScenarioNo=1,
				StepNo=1,
				Value="https://www.google.com",
				GeneratorRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"
			} };

			repo.Setup(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
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

				ProjectKey = projectKey,
				ScenarioNo = 1,
				StepNo = 1
			};

			processor.ResolveVariables(step);

			var actual = step.Invocations[0].RequestRaw;
			Assert.AreEqual("url https://www.google.com", actual);
		}

		[Test]
		public void VariableProcessor_GeneratesBothUrlAndMailAddressVariables()
		{
			ApiCache.DeleteVariableByProject(projectKey);

			Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
			var vars = new List<Variable>{new Variable
			{
				Id = "var1",
				IsEnabled=true,
				Name="url",
				ProjectKey=projectKey,
				ScenarioNo=1,
				StepNo=1,
				Value="https://www.google.com",
				GeneratorRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"
			},new Variable
			{
				Id = "var2",
				IsEnabled=true,
				Name="mailAddress",
				ProjectKey=projectKey,
				ScenarioNo=1,
				StepNo=1,
				Value="xxxyyy@gmail.com",
				GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

			} };

			repo.Setup(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
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
				ProjectKey = projectKey,
				ScenarioNo = 1,
				StepNo = 1
			};

			processor.ResolveVariables(step);

			var actual = step.Invocations[0].RequestRaw;
			Assert.AreEqual("url https://www.google.com xxxyyy@gmail.com", actual);
		}

		[Test]
		public void VariableProcessor_ResolvesBothMailAddressAndUrl()
		{
			ApiCache.DeleteVariableByProject(projectKey);

			Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
			var vars = new List<Variable>{new Variable
			{
				Id = "var1",
				IsEnabled=true,
				Name="mailAddress",
				ProjectKey=projectKey,
				ScenarioNo=1,
				StepNo=1,
				Value="xxxyyy@gmail.com",
				GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

			},new Variable
			{
				Id = "var2",
				IsEnabled=true,
				Name="url",
				ProjectKey=projectKey,
				ScenarioNo=1,
				StepNo=1,
				Value="https://www.google.com",
				GeneratorRegex=@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)"

			}};

			repo.Setup(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
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

				ProjectKey = projectKey,
				ScenarioNo = 1,
				StepNo = 1
			};

			processor.GenerateVariables(step);

			var actual = step.Invocations[0].RequestRaw;
			Assert.AreEqual("Mails {{mailAddress}} {{url}}", actual);
		}

		[Test]
		public void VariableProcessor_AppliesStepLevelVariable()
		{
			ApiCache.DeleteVariableByProject(projectKey);

			Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
			var vars = new List<Variable>{new Variable
			{
				Id = "stepVar",
				IsEnabled=true,
				Name="mailAddress",
				ProjectKey=projectKey,
				ScenarioNo=1,
				StepNo=1,
				Value="xxxyyy@gmail.com",
				GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

			},new Variable
			{
				Id = "scenVar",
				IsEnabled=true,
				Name="mailAddress1",
				ProjectKey=projectKey,
				ScenarioNo=1,
				StepNo=0,
				Value="xxxyyy@gmail.com",
				GeneratorRegex=@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"

			}};

			repo.Setup(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
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

				ProjectKey = projectKey,
				ScenarioNo = 1,
				StepNo = 1
			};

			processor.GenerateVariables(step);

			var actual = step.Invocations[0].RequestRaw;
			Assert.AreEqual("Mails {{mailAddress}}", actual);
		}

		[Test]
		public void VariableProcessor_GeneratesVariableInsideWrapperGroup()
		{
			ApiCache.DeleteVariableByProject(projectKey);

			Mock<IVariableRepo> repo = new Mock<IVariableRepo>();
			var vars = new List<Variable>{
				new Variable
				{
					Id = "someId",
					IsEnabled=true,
					Name="securityToken",
					ProjectKey=projectKey,
					ScenarioNo=1,
					StepNo=1,
					Value= string.Empty,
					GeneratorRegex=@"(?<wrapStart><\w+:BinarySecurityToken.+>).+(?<wrapEnd></\w+:BinarySecurityToken>)"
				}
			};

			repo.Setup(p => p.GetByProject(It.IsAny<string>())).Returns(vars);
			InvocationVariableProcessor processor = new InvocationVariableProcessor(repo.Object);

			var step = new Step
			{
				Invocations = new List<Invocation>
				{
					new Invocation
					{
						RequestRaw =
						@"
<?xml version=""1.0"" encoding=""UTF-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
	<s:Header>
		<Security s:mustUnderstand=""1"" xmlns=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
			<wsse:BinarySecurityToken wsu:Id=""SecurityToken-995dc72e-c32b-49d3-9a02-926fbcf3cc40"">cQ4Kife4onsDnWQ+QaARZYYWTxeo+xnB/jUy7XugPpOtUDOnufUF/pq9DItLpoi2Rq4WA9b0mK9w1JtPLcJ07T9tRt/GIViXoytnp/oTy08hh1YofA4/jnxmV5b92owljZi92L6Pin1taDJtl96UqoD8WJ/hSiL6W+9F65csXp8L/th4Z34GMNCS92kBQXiGqPZMaB56aniFLj9TjFcU/gi5g3Y992J613GO569u1fHWj4qqcKdT+Hs489cNO0COTP6UcR+LeS5etVsyu9SvqijOSmMT0odElA0JNQ1mXP+scyUfoLtYbLiVnXtUWoRqjo4EhjPXYNCHUn1jPAJ2ioEfbTUHoIHDpr+3X9QE8Zo=</wsse:BinarySecurityToken>
		</Security>
	</s:Header>
	<s:Body xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
		<generateToken xmlns=""http://tempuri.org/Technical/EA/Utils/TokenGenerator/V1"">
			<dummyRequest xsi:nil=""true"" xmlns=""""/>
		</generateToken>
	</s:Body>
</s:Envelope>
",
						Depth = 1
					}
				},
				ProjectKey = projectKey,
				ScenarioNo = 1,
				StepNo = 1
			};

			processor.GenerateVariables(step);

			var actual = step.Invocations[0].RequestRaw;
			Assert.AreEqual(@"
<?xml version=""1.0"" encoding=""UTF-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
	<s:Header>
		<Security s:mustUnderstand=""1"" xmlns=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
			<wsse:BinarySecurityToken wsu:Id=""SecurityToken-995dc72e-c32b-49d3-9a02-926fbcf3cc40"">{{securityToken}}</wsse:BinarySecurityToken>
		</Security>
	</s:Header>
	<s:Body xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
		<generateToken xmlns=""http://tempuri.org/Technical/EA/Utils/TokenGenerator/V1"">
			<dummyRequest xsi:nil=""true"" xmlns=""""/>
		</generateToken>
	</s:Body>
</s:Envelope>
"
			, actual);

		}
	}
}

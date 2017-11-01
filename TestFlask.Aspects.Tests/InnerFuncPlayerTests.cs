using AssemblyToProcess.Samples;
using Mono.Cecil;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TestFlask.Aspects.ApiClient;
using TestFlask.Aspects.Context;
using TestFlask.Aspects.Enums;
using TestFlask.Aspects.Identifiers;
using TestFlask.Aspects.Player;
using TestFlask.Models.Context;

namespace TestFlask.Aspects.Tests
{
    [TestFixture]
    public class InnerFuncPlayerTests
    {
        private Assembly assembly;
        private string newAssemblyPath;
        private string assemblyPath;

        private Mock<HttpContextBase> mockHttpContext;
        private Mock<ITestFlaskApi> mockTestFlaskApi;
        private Mock<HttpRequestBase> mockHttpRequest;

        [OneTimeSetUp]
        public void Setup()
        {
            var projectPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\AssemblyToProcess\AssemblyToProcess.csproj"));
            assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\AssemblyToProcess.dll");

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\"));

#if (!DEBUG)
        assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

            newAssemblyPath = assemblyPath.Replace(".dll", ".AspectsTests.dll");
            File.Copy(assemblyPath, newAssemblyPath, true);

            using (var moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters { AssemblyResolver = resolver }))
            {
                var weavingTask = new ModuleWeaver
                {
                    ModuleDefinition = moduleDefinition
                };

                weavingTask.Execute();
                moduleDefinition.Write(newAssemblyPath);
            }

            assembly = Assembly.LoadFile(newAssemblyPath);
        }

        [SetUp]
        public void Init()
        {
            mockHttpRequest = new Mock<HttpRequestBase>();

            NameValueCollection requestHeaders = new NameValueCollection();
            requestHeaders.Add(ContextKeys.ProjectKey, "UnitTest");
            requestHeaders.Add(ContextKeys.ScenarioNo, "999");
            requestHeaders.Add(ContextKeys.TestMode, TestModes.NoMock.ToString());

            mockHttpRequest.Setup(r => r.Headers).Returns(requestHeaders);

            mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(c => c.Items).Returns(new Dictionary<string, object>());
            mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);

            mockTestFlaskApi = new Mock<ITestFlaskApi>();

            HttpContextFactory.Current = mockHttpContext.Object;
            TestFlaskApiFactory.TestFlaskApi = mockTestFlaskApi.Object;
        }

        [Test]
        public void Test_OK()
        {
            FuncPlayer<int, Customer> funcPlayer = new FuncPlayer<int, Customer>
                ("AssemblyToProcess.Samples.Customer AssemblyToProcess.Samples.CustomerBiz::GetCustomer(System.Int32)",
                new CustomerIdIdentifier(), new CustomerResponseIdentifier());

            funcPlayer.StartInvocation(1);


        }
    }
}

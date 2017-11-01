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
using TestFlask.Models.Entity;

namespace TestFlask.Aspects.Tests.PlayerTests
{
    [TestFixture]
    public class FuncPlayerTests
    {
        private Mock<HttpContextBase> mockHttpContext;
        private Mock<ITestFlaskApi> mockTestFlaskApi;
        private Mock<HttpRequestBase> mockHttpRequest;

        private CustomerIdIdentifier customerIdIdentifier;
        private CustomerResponseIdentifier customerResponseIdentifier;
        private FuncPlayer<int, Customer> funcPlayer;

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

            customerIdIdentifier = new CustomerIdIdentifier();
            customerResponseIdentifier = new CustomerResponseIdentifier();

            funcPlayer = new FuncPlayer<int, Customer>
                ("AssemblyToProcess.Samples.Customer AssemblyToProcess.Samples.CustomerBiz::GetCustomer(System.Int32)",
                customerIdIdentifier, customerResponseIdentifier);
        }

        [Test]
        public void FuncPlayer_StartInvocation_SetsRequestFields()
        {
            funcPlayer.StartInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual("[1]", invocation.Request);
            Assert.AreEqual(customerIdIdentifier.ResolveIdentifierKey(1) , invocation.RequestIdentifierKey);
            Assert.AreEqual(customerIdIdentifier.ResolveDisplayInfo(1), invocation.RequestDisplayInfo);

        }

        [Test]
        public void FuncPlayer_StartInvocation_IncrementsCurrentDepth()
        {
            TestFlaskContext.CurrentDepth = 5;
            funcPlayer.StartInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(6, TestFlaskContext.CurrentDepth);
            Assert.AreEqual(6, invocation.Depth);
        }

        [Test]
        public void FuncPlayer_StartInvocation_SetsInvocationToLatestParentForCurrentDepth()
        {
            TestFlaskContext.CurrentDepth = 5;
            funcPlayer.StartInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(invocation.InstanceHashCode, TestFlaskContext.InvocationParentTable[6]);
        }

        [Test]
        public void FuncPlayer_StartInvocation_SetsParent()
        {
            string parentInstanceHashCode = "someParentInstanceHashCode";

            TestFlaskContext.InvocationParentTable[3] = parentInstanceHashCode;
            TestFlaskContext.CurrentDepth = 3;
            funcPlayer.StartInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(parentInstanceHashCode, invocation.ParentInstanceHashCode);
        }


    }
}

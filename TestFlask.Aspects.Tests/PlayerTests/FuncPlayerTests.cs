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

        private Dictionary<string, object> httpItems;
        private NameValueCollection requestHeaders;

        [SetUp]
        public void Init()
        {
            mockHttpRequest = new Mock<HttpRequestBase>();
            mockHttpContext = new Mock<HttpContextBase>();

            httpItems = new Dictionary<string, object>();
            mockHttpContext.Setup(c => c.Items).Returns(httpItems);

            requestHeaders = new NameValueCollection();
            requestHeaders.Add(ContextKeys.ProjectKey, "UnitTest");
            requestHeaders.Add(ContextKeys.ScenarioNo, "999");
            requestHeaders.Add(ContextKeys.TestMode, TestModes.NoMock.ToString());

            mockHttpRequest.Setup(r => r.Headers).Returns(requestHeaders);
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
        public void FuncPlayer_BeginInvocation_SetsRequestFields()
        {
            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual("[1]", invocation.Request);
            Assert.AreEqual(customerIdIdentifier.ResolveIdentifierKey(1) , invocation.RequestIdentifierKey);
            Assert.AreEqual(customerIdIdentifier.ResolveDisplayInfo(1), invocation.RequestDisplayInfo);

        }

        [Test]
        public void FuncPlayer_BeginInvocation_IncrementsCurrentDepth()
        {
            TestFlaskContext.CurrentDepth = 5;
            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(6, TestFlaskContext.CurrentDepth);
            Assert.AreEqual(6, invocation.Depth);
        }

        [Test]
        public void FuncPlayer_BeginInvocation_SetsInvocationToLatestParentForCurrentDepth()
        {
            TestFlaskContext.CurrentDepth = 5;
            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(invocation.InstanceHashCode, TestFlaskContext.InvocationParentTable[6]);
        }

        [Test]
        public void FuncPlayer_BeginInvocation_SetsParent()
        {
            string parentInstanceHashCode = "someParentInstanceHashCode";

            TestFlaskContext.InvocationParentTable[3] = parentInstanceHashCode;
            TestFlaskContext.CurrentDepth = 3;
            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(parentInstanceHashCode, invocation.ParentInstanceHashCode);
        }

        [Test]
        public void FuncPlayer_BeginInvocation_BuildsStepNoFromHttpItemsFirst()
        {
            httpItems.Add(ContextKeys.StepNo, 55L);
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(55, invocation.StepNo);
        }

        [Test]
        public void FuncPlayer_BeginInvocation_BuildsStepNoFromRequestHeadersFirst()
        {
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(66L, invocation.StepNo);
        }

        [Test]
        public void FuncPlayer_BeginInvocation_SetsOverwriteStepNo_WhenItBuildsStepFromRequestHeaders()
        {
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.IsTrue(TestFlaskContext.IsOverwriteStep);
        }

        [Test]
        public void FuncPlayer_BeginInvocation_SetsNewContextId_WhenItIsRootInvocation()
        {
            TestFlaskContext.CurrentDepth = 0;

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.IsNotNull(TestFlaskContext.ContextId);
        }

        [Test]
        public void FuncPlayer_BeginInvocation_DoesNotAlterContextId_WhenItIsNotRootInvocation()
        {
            string contextId = Guid.NewGuid().ToString();
            TestFlaskContext.ContextId = contextId;
            TestFlaskContext.CurrentDepth = 3;

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(contextId, TestFlaskContext.ContextId);
        }

        [Test]
        public void FuncPlayer_BeginInvocation_IncrementsInvocationIndex_ForSameLeafCodeSiblingCalls()
        {
            string parentInstanceHashCode = "someParentInstanceHashCode";

            TestFlaskContext.InvocationParentTable[3] = parentInstanceHashCode;
            TestFlaskContext.CurrentDepth = 3;

            funcPlayer.BeginInvocation(1);
            Invocation invocation1 = funcPlayer.innerPlayer.requestedInvocation;
            TestFlaskContext.CurrentDepth--;

            funcPlayer.BeginInvocation(1);
            Invocation invocation2 = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(invocation2.InvocationIndex, invocation1.InvocationIndex + 1);

        }

        [Test]
        public void FuncPlayer_BeginInvocation_DoesNotIncrementInvocationIndex_ForDiffLeafCodeSiblingCalls()
        {
            string parentInstanceHashCode = "someParentInstanceHashCode";

            TestFlaskContext.InvocationParentTable[3] = parentInstanceHashCode;
            TestFlaskContext.CurrentDepth = 3;

            funcPlayer.BeginInvocation(1);
            Invocation invocation1 = funcPlayer.innerPlayer.requestedInvocation;
            TestFlaskContext.CurrentDepth--;

            funcPlayer.BeginInvocation(2); //different request causes different leaf because this player has requestIdentifier
            Invocation invocation2 = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(invocation2.InvocationIndex, invocation1.InvocationIndex);

        }

        [Test]
        public void FuncPlayer_BeginInvocation_IdenticalInvocations_HaveSameLeafCodes_ButDiffInstanceCodes()
        {
            string parentInstanceHashCode = "someParentInstanceHashCode";

            TestFlaskContext.InvocationParentTable[3] = parentInstanceHashCode;
            TestFlaskContext.CurrentDepth = 3;

            funcPlayer.BeginInvocation(1);
            Invocation invocation1 = funcPlayer.innerPlayer.requestedInvocation;
            TestFlaskContext.CurrentDepth--;

            funcPlayer.BeginInvocation(1); //same request ensures same leaf code
            Invocation invocation2 = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(invocation1.LeafHashCode, invocation1.LeafHashCode, "Leaf hash codes must be same");
            //But they always have different instance code (because of incrementing invocation indexes)
            Assert.AreNotEqual(invocation1.InstanceHashCode, invocation2.InstanceHashCode, "Instance hash codes must differ");
        }

        [Test]
        public void FuncPlayer_BeginInvocation_StartsDepthFromCallerDepth_WhenGivenInRequestHeaders()
        {
            requestHeaders.Add(ContextKeys.CallerDepth, "5");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(6, invocation.Depth);
            Assert.AreEqual(6, TestFlaskContext.CurrentDepth);
        }

        [Test]
        public void FuncPlayer_DetermineTestMode_SuppliesRequestedMode_NoMock()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.NoMock.ToString();

            funcPlayer.BeginInvocation(1);
            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.NoMock, testMode);
        }

        [Test]
        public void FuncPlayer_DetermineTestMode_SuppliesRequestedMode_Record()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Record.ToString();

            funcPlayer.BeginInvocation(1);
            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.Record, testMode);
        }

        [Test]
        public void FuncPlayer_DetermineTestMode_ReturnsPlayMode_WhenPlayRequested_And_InvocationIsReplayable()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.IsReplayable = true;

            Step dummyLoadedStep = new Step { Invocations = new List<Invocation> { invocation } };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.Play, testMode);
        }

        [Test]
        public void FuncPlayer_DetermineTestMode_ReturnsNoMockMode_WhenPlayRequested_But_InvocationIsNotReplayable()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.IsReplayable = false;

            Step dummyLoadedStep = new Step { Invocations = new List<Invocation> { invocation } };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.NoMock, testMode);
        }

        [Test]
        public void FuncPlayer_DetermineTestMode_ReturnsPlayMode_WhenAssertRequested_And_InvocationIsReplayable()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Assert.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.IsReplayable = true;

            Step dummyLoadedStep = new Step { Invocations = new List<Invocation> { invocation } };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.Play, testMode);
        }

        [Test]
        public void FuncPlayer_DetermineTestMode_ReturnsNoMockMode_WhenPlayRequested_But_MatchingInvocationIsNotFound()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.IsReplayable = true;

            Step dummyLoadedStep = new Step { Invocations = new List<Invocation> { } };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.NoMock, testMode);
        }

        [Test]
        public void FuncPlayer_CallsApiDeleteInvocation_WhenOverwritingStepOnRecordMode_And_InRootDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Record.ToString();
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.DeleteStepInvocations(TestFlaskContext.RequestedStep), Times.Once);
        }

        [Test]
        public void FuncPlayer_DoesNotCallApiDeleteInvocation_WhenOverwritingStepOnRecordMode_And_InInitialDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Record.ToString();
            requestHeaders.Add(ContextKeys.CallerDepth, "5");
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.DeleteStepInvocations(TestFlaskContext.RequestedStep), Times.Never);
        }

        [Test]
        public void FuncPlayer_CallsApiGetStep_WhenOnPlayMode_And_InRootDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();
            requestHeaders.Add(ContextKeys.StepNo, "44");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Step dummyLoadedStep = new Step { Invocations = new List<Invocation> { invocation } };
            mockTestFlaskApi.Setup(api => api.GetStep(44L)).Returns(dummyLoadedStep);

            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.GetStep(44L), Times.Once);
        }

        [Test]
        public void FuncPlayer_CallsApiGetStep_WhenOnPlayMode_And_InInitialDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();
            requestHeaders.Add(ContextKeys.CallerDepth, "5");
            requestHeaders.Add(ContextKeys.StepNo, "44");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Step dummyLoadedStep = new Step { Invocations = new List<Invocation> { invocation } };
            mockTestFlaskApi.Setup(api => api.GetStep(44L)).Returns(dummyLoadedStep);

            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.GetStep(44L), Times.Once);
        }

        [Test]
        public void FuncPlayer_DoesNotCallApiGetStep_WhenOnPlayMode_And_NotInRootDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();
            requestHeaders.Add(ContextKeys.StepNo, "44");
            TestFlaskContext.CurrentDepth = 2;

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Step dummyLoadedStep = new Step { Invocations = new List<Invocation> { invocation } };
            TestFlaskContext.LoadedStep = dummyLoadedStep; ;

            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.GetStep(44L), Times.Never);
        }

        [Test]
        public void FuncPlayer_DoesNotCallsApiGetStep_WhenOnPlayMode_And_NotInInitialDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();
            requestHeaders.Add(ContextKeys.CallerDepth, "5");
            requestHeaders.Add(ContextKeys.StepNo, "44");
            TestFlaskContext.CurrentDepth = 7;

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Step dummyLoadedStep = new Step { Invocations = new List<Invocation> { invocation } };
            TestFlaskContext.LoadedStep = dummyLoadedStep; ;

            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.GetStep(44L), Times.Never);
        }
    }
}

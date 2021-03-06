﻿using Mono.Cecil;
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
using TestFlask.Models.Enums;

namespace TestFlask.Aspects.Tests.PlayerTests
{
    [TestFixture]
    public class InnerPlayerTests : PlayerTestsBase
    {
        private FooIdIdentifier fooIdIdentifier;
        private FooResponseIdentifier fooResponseIdentifier;
        private FuncPlayer<int, Foo> funcPlayer;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            fooIdIdentifier = new FooIdIdentifier();
            fooResponseIdentifier = new FooResponseIdentifier();

            funcPlayer = new FuncPlayer<int, Foo>
                ("SomeAssembly.Foo SomeAssembly.FooBiz::GetFoo(System.Int32)",
                fooIdIdentifier, fooResponseIdentifier);
        }

        [Test]
        public void InnerPlayer_BeginInvocation_SetsRequestFields()
        {
            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual("{\"$type\":\"System.Object[], mscorlib\",\"$values\":[1]}", invocation.Request);
            Assert.AreEqual(fooIdIdentifier.ResolveIdentifierKey(1), invocation.RequestIdentifierKey);
            Assert.AreEqual(fooIdIdentifier.ResolveDisplayInfo(1), invocation.RequestDisplayInfo);

        }

        [Test]
        public void InnerPlayer_BeginInvocation_IncrementsCurrentDepth()
        {
            TestFlaskContext.CurrentDepth = 5;
            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(6, TestFlaskContext.CurrentDepth);
            Assert.AreEqual(6, invocation.Depth);
        }

        [Test]
        public void InnerPlayer_BeginInvocation_SetsInvocationToLatestParentForCurrentDepth()
        {
            TestFlaskContext.CurrentDepth = 5;
            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(invocation.InstanceHashCode, TestFlaskContext.InvocationParentTable[6]);
        }

        [Test]
        public void InnerPlayer_BeginInvocation_SetsParent()
        {
            string parentInstanceHashCode = "someParentInstanceHashCode";

            TestFlaskContext.InvocationParentTable[3] = parentInstanceHashCode;
            TestFlaskContext.CurrentDepth = 3;
            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(parentInstanceHashCode, invocation.ParentInstanceHashCode);
        }

        [Test]
        public void InnerPlayer_BeginInvocation_BuildsStepNoFromHttpItemsFirst()
        {
            httpItems.Add(ContextKeys.StepNo, 55L);
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(55, invocation.StepNo);
        }

        [Test]
        public void InnerPlayer_BeginInvocation_BuildsStepNoFromRequestHeadersFirst()
        {
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(66L, invocation.StepNo);
        }

        [Test]
        public void InnerPlayer_BeginInvocation_SetsOverwriteStepNo_WhenItBuildsStepFromRequestHeaders()
        {
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.IsTrue(TestFlaskContext.IsOverwriteStep);
        }

        [Test]
        public void InnerPlayer_BeginInvocation_SetsNewContextId_WhenItIsRootInvocation()
        {
            TestFlaskContext.CurrentDepth = 0;

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.IsNotNull(TestFlaskContext.ContextId);
        }

        [Test]
        public void InnerPlayer_BeginInvocation_DoesNotAlterContextId_WhenItIsNotRootInvocation()
        {
            string contextId = Guid.NewGuid().ToString();
            TestFlaskContext.ContextId = contextId;
            TestFlaskContext.CurrentDepth = 3;

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(contextId, TestFlaskContext.ContextId);
        }

        [Test]
        public void InnerPlayer_BeginInvocation_IncrementsInvocationIndex_ForSameLeafCodeSiblingCalls()
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
        public void InnerPlayer_BeginInvocation_DoesNotIncrementInvocationIndex_ForDiffLeafCodeSiblingCalls()
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
        public void InnerPlayer_BeginInvocation_IdenticalInvocations_HaveSameLeafCodes_ButDiffInstanceCodes()
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
        public void InnerPlayer_BeginInvocation_StartsDepthFromCallerDepth_WhenGivenInRequestHeaders()
        {
            requestHeaders.Add(ContextKeys.CallerDepth, "5");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Assert.AreEqual(6, invocation.Depth);
            Assert.AreEqual(6, TestFlaskContext.CurrentDepth);
        }

        [Test]
        public void InnerPlayer_DetermineTestMode_SuppliesRequestedMode_NoMock()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.NoMock.ToString();

            funcPlayer.BeginInvocation(1);
            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.NoMock, testMode);
        }

        [Test]
        public void InnerPlayer_DetermineTestMode_SuppliesRequestedMode_Record()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Record.ToString();

            funcPlayer.BeginInvocation(1);
            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.Record, testMode);
        }

        [Test]
        public void InnerPlayer_DetermineTestMode_ReturnsPlayMode_WhenPlayRequested_And_InvocationIsReplayable()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.IsReplayable = true;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { invocation }
            };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.Play, testMode);
        }

        [Test]
        public void InnerPlayer_DetermineTestMode_ReturnsNoMockMode_WhenPlayRequested_But_InvocationIsNotReplayable()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.IsReplayable = false;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { invocation }
            };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.NoMock, testMode);
        }

        [Test]
        public void InnerPlayer_DetermineTestMode_ReturnsPlayMode_WhenAssertRequested_And_InvocationIsReplayable()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Assert.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.IsReplayable = true;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { invocation }
            };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.Play, testMode);
        }

        [Test]
        public void InnerPlayer_DetermineTestMode_ReturnsNoMockMode_WhenPlayRequested_But_MatchingInvocationIsNotFound()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.IsReplayable = true;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { }
            };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.NoMock, testMode);
        }

        [Test]
        public void InnerPlayer_DetermineTestMode_ReturnsRecordMode_WhenIntelliRecordRequested_But_MatchingInvocationIsNotFound()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.IntelliRecord.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { }
            };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.Record, testMode);
        }

        [Test]
        public void InnerPlayer_DetermineTestMode_ReturnsPlayMode_WhenIntelliRecordRequested_And_MatchingReplayableInvocationIsFound()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.IntelliRecord.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.IsReplayable = true;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { invocation }
            };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.Play, testMode);
        }

        [Test]
        public void InnerPlayer_DetermineTestMode_ReturnsRecordMode_WhenIntelliRecordRequested_And_MatchingNotReplayableInvocationIsFound()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.IntelliRecord.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.IsReplayable = false;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { invocation }
            };

            TestFlaskContext.LoadedStep = dummyLoadedStep;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual(TestModes.Record, testMode);
        }

        [Test]
        public void InnerPlayer_CallsApiDeleteInvocation_WhenOverwritingStepOnRecordMode_And_InRootDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Record.ToString();
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.DeleteStepInvocations(TestFlaskContext.RequestedStep), Times.Once);
        }

        [Test]
        public void InnerPlayer_CallsApiDeleteInvocation_WhenOverwritingStepOnIntelliRecordMode_And_InRootDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.IntelliRecord.ToString();
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { invocation }
            };
            mockTestFlaskApi.Setup(api => api.LoadStep(66L)).Returns(dummyLoadedStep);
            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.DeleteStepInvocations(TestFlaskContext.RequestedStep), Times.Once);
        }

        [Test]
        public void InnerPlayer_DoesNotCallApiDeleteInvocation_WhenOverwritingStepOnRecordMode_And_InInitialDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Record.ToString();
            requestHeaders.Add(ContextKeys.CallerDepth, "5");
            requestHeaders.Add(ContextKeys.StepNo, "66");

            funcPlayer.BeginInvocation(1);
            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.DeleteStepInvocations(TestFlaskContext.RequestedStep), Times.Never);
        }

        [Test]
        public void InnerPlayer_CallsApiGetStep_WhenOnPlayMode_And_InRootDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();
            requestHeaders.Add(ContextKeys.StepNo, "44");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { invocation }
            };
            mockTestFlaskApi.Setup(api => api.LoadStep(44L)).Returns(dummyLoadedStep);

            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.LoadStep(44L), Times.Once);
        }

        [Test]
        public void InnerPlayer_CallsApiGetStep_WhenOnPlayMode_And_InInitialDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();
            requestHeaders.Add(ContextKeys.CallerDepth, "5");
            requestHeaders.Add(ContextKeys.StepNo, "44");

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { invocation }
            };
            mockTestFlaskApi.Setup(api => api.LoadStep(44L)).Returns(dummyLoadedStep);

            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.LoadStep(44L), Times.Once);
        }

        [Test]
        public void InnerPlayer_DoesNotCallApiGetStep_WhenOnPlayMode_And_NotInRootDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();
            requestHeaders.Add(ContextKeys.StepNo, "44");
            TestFlaskContext.CurrentDepth = 2;

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { invocation }
            };
            TestFlaskContext.LoadedStep = dummyLoadedStep; ;

            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.LoadStep(44L), Times.Never);
        }

        [Test]
        public void InnerPlayer_DoesNotCallsApiGetStep_WhenOnPlayMode_And_NotInInitialDepth()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();
            requestHeaders.Add(ContextKeys.CallerDepth, "5");
            requestHeaders.Add(ContextKeys.StepNo, "44");
            TestFlaskContext.CurrentDepth = 7;

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;

            Step dummyLoadedStep = new Step
            {
                LoadedMatchStrategy = InvocationMatch.Exact,
                Invocations = new List<Invocation> { invocation }
            };
            TestFlaskContext.LoadedStep = dummyLoadedStep; ;

            var testMode = funcPlayer.DetermineTestMode(1);

            mockTestFlaskApi.Verify(api => api.LoadStep(44L), Times.Never);
        }
    }
}

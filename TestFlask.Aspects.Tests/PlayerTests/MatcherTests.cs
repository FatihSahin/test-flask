using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Aspects.Context;
using TestFlask.Aspects.Enums;
using TestFlask.Aspects.Player;
using TestFlask.Models.Context;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.Aspects.Tests.PlayerTests
{
    [TestFixture]
    public class MatcherTests : PlayerTestsBase
    {
        private Step step;
        private Invocation i1_d1;
        private Invocation i2_d2_0;
        private Invocation i3_d2_1;
        private Invocation i4_dx2_0;
        private Invocation i5_d3_0;
        private Invocation i6_d3_1;
        private Invocation i7_dx3_1;
        private Invocation i8_d3_0;
        private FuncPlayer<int, string> funcPlayer;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            string respType = "System.String, mscorlib";

            step = new Step
            {
                ProjectKey = "testMatcher",
                ScenarioNo = 1,
                StepNo = 1,
                StepName = "step1"
            };

            var now = DateTime.UtcNow;

            i1_d1 = new Invocation
            {
                ProjectKey = "testMatcher",
                StepNo = 1,
                ScenarioNo = 1,
                SignatureHashCode = "s1",
                RequestHashCode = "r1",
                Depth = 1,
                InvocationIndex = 0,
                DeepHashCode = "d1",
                LeafHashCode = "l1",
                InstanceHashCode = "i1",
                Response = "'response_i1'",
                ResponseType = respType,
                RecordedOn = now,
                IsReplayable = false
            };

            i2_d2_0 = new Invocation
            {
                ProjectKey = "testMatcher",
                StepNo = 1,
                ScenarioNo = 1,
                SignatureHashCode = "s2",
                RequestHashCode = "r2",
                Depth = 2,
                InvocationIndex = 0,
                DeepHashCode = "d2",
                LeafHashCode = "l2",
                InstanceHashCode = "i2",
                Response = "'response_i2'",
                ResponseType = respType,
                RecordedOn = now.AddSeconds(1),
                IsReplayable = true
            };

            i3_d2_1 = new Invocation
            {
                ProjectKey = "testMatcher",
                StepNo = 1,
                ScenarioNo = 1,
                SignatureHashCode = "s2",
                RequestHashCode = "r2",
                Depth = 2,
                InvocationIndex = 1,
                DeepHashCode = "d2",
                LeafHashCode = "l2",
                InstanceHashCode = "i3",
                Response = "'response_i3'",
                ResponseType = respType,
                RecordedOn = now.AddSeconds(2),
                IsReplayable = true
            };

            i4_dx2_0 = new Invocation
            {
                ProjectKey = "testMatcher",
                StepNo = 1,
                ScenarioNo = 1,
                SignatureHashCode = "sx2",
                RequestHashCode = "rx2",
                Depth = 2,
                InvocationIndex = 0,
                DeepHashCode = "dx2",
                LeafHashCode = "lx2",
                InstanceHashCode = "i4",
                Response = "'response_i4'",
                ResponseType = respType,
                RecordedOn = now.AddSeconds(3),
                IsReplayable = true
            };

            i5_d3_0 = new Invocation
            {
                ProjectKey = "testMatcher",
                StepNo = 1,
                ScenarioNo = 1,
                SignatureHashCode = "s3",
                RequestHashCode = "r3",
                Depth = 3,
                InvocationIndex = 0,
                DeepHashCode = "d3",
                LeafHashCode = "l3",
                InstanceHashCode = "i5",
                Response = "'response_i5'",
                ResponseType = respType,
                RecordedOn = now.AddSeconds(4),
                IsReplayable = true
            };

            i6_d3_1 = new Invocation
            {
                ProjectKey = "testMatcher",
                StepNo = 1,
                ScenarioNo = 1,
                SignatureHashCode = "s3",
                RequestHashCode = "r3",
                Depth = 3,
                InvocationIndex = 1,
                DeepHashCode = "d3",
                LeafHashCode = "l3",
                InstanceHashCode = "i6",
                Response = "'response_i6'",
                ResponseType = respType,
                RecordedOn = now.AddSeconds(5),
                IsReplayable = true
            };

            i7_dx3_1 = new Invocation
            {
                ProjectKey = "testMatcher",
                StepNo = 1,
                ScenarioNo = 1,
                SignatureHashCode = "s3",
                RequestHashCode = "rx3",
                Depth = 3,
                InvocationIndex = 0,
                DeepHashCode = "dx3",
                LeafHashCode = "lx3",
                InstanceHashCode = "i7",
                Response = "'response_i7'",
                ResponseType = respType,
                RecordedOn = now.AddSeconds(6),
                IsReplayable = true
            };

            i8_d3_0 = new Invocation
            {
                ProjectKey = "testMatcher",
                StepNo = 1,
                ScenarioNo = 1,
                SignatureHashCode = "s3",
                RequestHashCode = "r3",
                Depth = 3,
                InvocationIndex = 0,
                DeepHashCode = "d3",
                LeafHashCode = "lxx3",
                InstanceHashCode = "i8",
                Response = "'response_i8'",
                ResponseType = respType,
                RecordedOn = now.AddSeconds(7),
                IsReplayable = true
            };

            step.Invocations = new List<Invocation>
            {
                i1_d1,
                i2_d2_0,
                i3_d2_1,
                i4_dx2_0,
                i5_d3_0,
                i6_d3_1,
                i7_dx3_1,
                i8_d3_0
            };

            recordedSteps.Add(step.StepNo, step);


            funcPlayer = new FuncPlayer<int, string>("System.String SomeAssembly.FooBiz::GetFooStr(System.Int32)");
        }

        [Test]
        public void Matcher_LoadsExactResponse_WhenSetToExactStrategy()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.InstanceHashCode = "i3";
            invocation.IsReplayable = true;

            step.LoadedMatchStrategy = InvocationMatch.Exact;
            TestFlaskContext.LoadedStep = step;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual("response_i3", funcPlayer.Play(1));
        }

        [Test]
        public void Matcher_LoadsExactResponse_WhenSetToRequestStrategy()
        {
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();

            funcPlayer.BeginInvocation(1);
            Invocation invocation = funcPlayer.innerPlayer.requestedInvocation;
            invocation.RequestHashCode = "r3";
            invocation.InstanceHashCode = "irrelevant";
            invocation.IsReplayable = true;

            step.LoadedMatchStrategy = InvocationMatch.Request;
            TestFlaskContext.LoadedStep = step;

            var testMode = funcPlayer.DetermineTestMode(1);

            Assert.AreEqual("response_i5", funcPlayer.Play(1));
        }
    }
}

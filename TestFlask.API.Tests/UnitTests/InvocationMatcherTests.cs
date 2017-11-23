using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.API.InvocationMatcher;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.API.Tests.UnitTests
{
    [TestFixture]
    public class InvocationMatcherTests
    {
        private Project project;
        private Scenario scenario;
        private Step step;
        private Invocation i1_d1;
        private Invocation i2_d2_0;
        private Invocation i3_d2_1;
        private Invocation i4_dx2_0;
        private Invocation i5_d3_0;
        private Invocation i6_d3_1;
        private Invocation i7_dx3_1;
        private Invocation i8_d3_0;

        [SetUp]
        public void SetUp()
        {
            project = new Project
            {
                ProjectKey = "testMatcher"
            };

            step = new Step
            {
                ProjectKey = "testMatcher",
                ScenarioNo = 1,
                StepNo = 1,
                StepName = "step1"
            };

            scenario = new Scenario
            {
                ProjectKey = "testMatcher",
                ScenarioNo = 1,
                ScenarioName = "scenario1",
                Steps = new List<Step> { step }
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
                Response = "response_i1",
                RecordedOn = now,
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
                Response = "response_i2",
                RecordedOn = now.AddSeconds(1)
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
                Response = "response_i3",
                RecordedOn = now.AddSeconds(2)
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
                Response = "response_i4",
                RecordedOn = now.AddSeconds(3)
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
                Response = "response_i5",
                RecordedOn = now.AddSeconds(4)
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
                Response = "response_i6",
                RecordedOn = now.AddSeconds(5)
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
                Response = "response_i7",
                RecordedOn = now.AddSeconds(6)
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
                Response = "response_i8",
                RecordedOn = now.AddSeconds(7)
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
        }

        [Test]
        public void Matcher_ReturnsExactMatches_ByDefault()
        {
            Matcher matcher = new MatcherProvider(project, scenario, step).Provide();

            matcher.Match();

            Assert.AreEqual("response_i1", i1_d1.Response);
            Assert.AreEqual("response_i2", i2_d2_0.Response);
            Assert.AreEqual("response_i3", i3_d2_1.Response);
            Assert.AreEqual("response_i4", i4_dx2_0.Response);
            Assert.AreEqual("response_i5", i5_d3_0.Response);
            Assert.AreEqual("response_i6", i6_d3_1.Response);
            Assert.AreEqual("response_i7", i7_dx3_1.Response);
            Assert.AreEqual("response_i8", i8_d3_0.Response);
        }

        [Test]
        public void Matcher_ReturnsSignatureMatches_WhenSetOnProjectLevel()
        {
            project.InvocationMatchStrategy = InvocationMatch.Signature;

            Matcher matcher = new MatcherProvider(project, scenario, step).Provide();

            matcher.Match();

            Assert.IsTrue(matcher is Matcher);
            Assert.AreEqual("response_i1", i1_d1.Response);
            Assert.AreEqual("response_i2", i2_d2_0.Response);
            Assert.AreEqual("response_i2", i3_d2_1.Response);
            Assert.AreEqual("response_i4", i4_dx2_0.Response);
            Assert.AreEqual("response_i5", i5_d3_0.Response);
            Assert.AreEqual("response_i5", i6_d3_1.Response);
            Assert.AreEqual("response_i5", i7_dx3_1.Response);
            Assert.AreEqual("response_i5", i8_d3_0.Response);
        }

        [Test]
        public void Matcher_ReturnsSignatureMatches_WhenSetOnScenarioLevel()
        {
            project.InvocationMatchStrategy = InvocationMatch.Exact;
            scenario.InvocationMatchStrategy = InvocationMatch.Signature;

            Matcher matcher = new MatcherProvider(project, scenario, step).Provide();

            matcher.Match();

            Assert.IsTrue(matcher is SignatureMatcher);
        }

        [Test]
        public void Matcher_ReturnsSignatureMatches_WhenSetOnStepLevel()
        {
            project.InvocationMatchStrategy = InvocationMatch.Exact;
            scenario.InvocationMatchStrategy = InvocationMatch.Request;
            step.InvocationMatchStrategy = InvocationMatch.Signature;

            Matcher matcher = new MatcherProvider(project, scenario, step).Provide();

            matcher.Match();

            Assert.IsTrue(matcher is SignatureMatcher);
        }

        [Test]
        public void Matcher_ReturnsRequestMatches_WhenSetExplicit()
        {
            project.InvocationMatchStrategy = InvocationMatch.Request;

            Matcher matcher = new MatcherProvider(project, scenario, step).Provide();

            matcher.Match();

            Assert.IsTrue(matcher is RequestMatcher);
            Assert.AreEqual("response_i1", i1_d1.Response);
            Assert.AreEqual("response_i2", i2_d2_0.Response);
            Assert.AreEqual("response_i2", i3_d2_1.Response);
            Assert.AreEqual("response_i4", i4_dx2_0.Response);
            Assert.AreEqual("response_i5", i5_d3_0.Response);
            Assert.AreEqual("response_i5", i6_d3_1.Response);
            Assert.AreEqual("response_i7", i7_dx3_1.Response);
            Assert.AreEqual("response_i5", i8_d3_0.Response);
        }


        [Test]
        public void Matcher_ReturnsDepthMatches_WhenSetExplicit()
        {
            project.InvocationMatchStrategy = InvocationMatch.Depth;

            Matcher matcher = new MatcherProvider(project, scenario, step).Provide();

            matcher.Match();

            Assert.IsTrue(matcher is DepthMatcher);
            Assert.AreEqual("response_i1", i1_d1.Response);
            Assert.AreEqual("response_i2", i2_d2_0.Response);
            Assert.AreEqual("response_i2", i3_d2_1.Response);
            Assert.AreEqual("response_i4", i4_dx2_0.Response);
            Assert.AreEqual("response_i5", i5_d3_0.Response);
            Assert.AreEqual("response_i5", i6_d3_1.Response);
            Assert.AreEqual("response_i7", i7_dx3_1.Response);
            Assert.AreEqual("response_i5", i8_d3_0.Response);
        }

        [Test]
        public void Matcher_ReturnsSiblingMatches_WhenSetExplicit()
        {
            project.InvocationMatchStrategy = InvocationMatch.Sibling;

            Matcher matcher = new MatcherProvider(project, scenario, step).Provide();

            matcher.Match();

            Assert.IsTrue(matcher is SiblingMatcher);
            Assert.AreEqual("response_i1", i1_d1.Response);
            Assert.AreEqual("response_i2", i2_d2_0.Response);
            Assert.AreEqual("response_i2", i3_d2_1.Response);
            Assert.AreEqual("response_i4", i4_dx2_0.Response);
            Assert.AreEqual("response_i5", i5_d3_0.Response);
            Assert.AreEqual("response_i5", i6_d3_1.Response);
            Assert.AreEqual("response_i7", i7_dx3_1.Response);
            Assert.AreEqual("response_i8", i8_d3_0.Response);
        }
    }
}

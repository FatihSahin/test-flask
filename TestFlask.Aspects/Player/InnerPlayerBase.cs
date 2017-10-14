using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TestFlask.Aspects.ApiClient;
using TestFlask.Aspects.Context;
using TestFlask.Aspects.Enums;
using TestFlask.Aspects.Identifiers;
using TestFlask.Models.Entity;

namespace TestFlask.Aspects.Player
{
    public class InnerPlayerBase
    {
        protected readonly string methodSignature;
        protected readonly string requestDisplayInfo;
        protected readonly string requestIdentifierKey;
        protected Invocation requestedInvocation;
        protected TestFlaskApi api;
        private bool mustPersistAssertionResult;

        public InnerPlayerBase(string pMethodSignature, string pRequestIdentifierKey, string pRequestDisplayInfo)
        {
            methodSignature = pMethodSignature;
            requestIdentifierKey = pRequestIdentifierKey;
            requestDisplayInfo = pRequestDisplayInfo;
            api = new TestFlaskApi();
        }

        public void StartInvocation(params object[] requestArgs)
        {
            int currentDepth = TestFlaskContext.CurrentDepth;

            string parentInstanceHashCode = TestFlaskContext.InvocationParentTable.ContainsKey(currentDepth) ? TestFlaskContext.InvocationParentTable[currentDepth] : null;

            TestFlaskContext.CurrentDepth++;

            var step = TestFlaskContext.RequestedStep;

            requestedInvocation = new Invocation();

            requestedInvocation.Depth = TestFlaskContext.CurrentDepth;

            requestedInvocation.ScenarioNo = step.ScenarioNo;
            requestedInvocation.ProjectKey = step.ProjectKey;
            requestedInvocation.StepNo = step.StepNo;

            requestedInvocation.IsReplayable = requestedInvocation.Depth > 1; //root invocation not playable by default
            requestedInvocation.InvocationSignature = methodSignature;
            requestedInvocation.RequestDisplayInfo = requestDisplayInfo;
            requestedInvocation.RequestIdentifierKey = requestIdentifierKey;
            requestedInvocation.Request = JsonConvert.SerializeObject(requestArgs);

            //set hash codes
            requestedInvocation.HashCode = requestedInvocation.GetInvocationHashCode();
            requestedInvocation.DeepHashCode = requestedInvocation.GetInvocationDeepHashCode();

            //need to find an invocation index on that depth level using something like testFlaskContext.InvocationDepthTable (deep hash code)
            var invocationDepthTable = TestFlaskContext.InvocationDepthTable;
            if (!invocationDepthTable.ContainsKey(requestedInvocation.DeepHashCode))
            {
                invocationDepthTable[requestedInvocation.DeepHashCode] = 0;
                requestedInvocation.InvocationIndex = 0;
            }
            else
            {
                invocationDepthTable[requestedInvocation.DeepHashCode] = invocationDepthTable[requestedInvocation.DeepHashCode] + 1;
                requestedInvocation.InvocationIndex = invocationDepthTable[requestedInvocation.DeepHashCode];
            }

            requestedInvocation.InstanceHashCode = requestedInvocation.GetInvocationInstanceHashCode();
            requestedInvocation.ParentInstanceHashCode = parentInstanceHashCode;

            //make this invocation latest parent for the current depth
            TestFlaskContext.InvocationParentTable[TestFlaskContext.CurrentDepth] = requestedInvocation.InstanceHashCode;
        }

        protected void EndInvocation(object result = null)
        {
            TestFlaskContext.CurrentDepth--;

            if (mustPersistAssertionResult && result != null)
            {
                Invocation rootInvocation = TestFlaskContext.GetRootInvocation();
                rootInvocation.AssertionResult = JsonConvert.SerializeObject(result);
                api.PutInvocation(rootInvocation); //persist assertion result
            }
        }

        public TestModes DetermineTestMode(params object[] requestArgs)
        {
            var requestedMode = TestFlaskContext.RequestedMode;

            if (requestedMode == TestModes.Record || requestedMode == TestModes.NoMock)
            {
                return requestedMode;
            }
            else
            {
                if (TestFlaskContext.LoadedStep == null && TestFlaskContext.CurrentDepth == 1)
                {
                    TestFlaskContext.LoadedStep = api.GetStep(requestedInvocation.StepNo);
                }

                Invocation existingInvocation = TestFlaskContext.GetInvocation(requestedInvocation.InstanceHashCode);

                if (existingInvocation != null)
                {
                    if (requestedMode == TestModes.Assert)
                    {
                        mustPersistAssertionResult = (TestFlaskContext.CurrentDepth == 1);
                    }
                    //lookup existing invocation and determine test mode
                    return existingInvocation.IsReplayable ? TestModes.Play : TestModes.NoMock;
                }
                else
                {
                    //cannot find same invocation
                    return TestModes.NoMock;
                }
            }
        }

        protected void TryPersistStepInvocations()
        {
            var step = TestFlaskContext.RequestedStep;

            if (step.Invocations == null)
            {
                step.Invocations = new List<Invocation>();
            }

            step.Invocations.Add(requestedInvocation);

            if (TestFlaskContext.CurrentDepth == 1)
            {
                api.PutStepInvocations(step);
            }
        }

        protected void RecordException(Exception ex, long duration, params object[] requestArgs)
        {
            requestedInvocation.IsFaulted = true;
            requestedInvocation.ExceptionType = ex.GetType().ToString();
            requestedInvocation.Exception = JsonConvert.SerializeObject(ex);

            requestedInvocation.Duration = duration;

            if (requestedInvocation.Depth == 1)    //root invocation
            {
                requestedInvocation.RequestRaw = TestFlaskContext.RawRequest;
            }

            TryPersistStepInvocations();

            EndInvocation(ex);
        }
    }
}
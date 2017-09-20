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

namespace TestFlask.Aspects
{
    public class InnerPlayer<TRes>
    {
        private readonly string methodSignature;
        private readonly string requestDisplayInfo;
        private readonly string requestIdentifierKey;
        private readonly IResponseIdentifier<TRes> responseIdentifier;
        private Invocation requestedInvocation;
        private TestFlaskApiClient api;
        private bool mustPersistAssertionResult;

        public InnerPlayer(string pMethodSignature, string pRequestIdentifierKey, string pRequestDisplayInfo, IResponseIdentifier<TRes> pResponseIdentifier)
        {
            methodSignature = pMethodSignature;
            requestIdentifierKey = pRequestIdentifierKey;
            requestDisplayInfo = pRequestDisplayInfo;
            responseIdentifier = pResponseIdentifier;
            api = new TestFlaskApiClient();
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

        private void EndInvocation(object result)
        {
            TestFlaskContext.CurrentDepth--;

            if (mustPersistAssertionResult)
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

        public TRes CallOriginal(object target, MethodInfo originalMethodInfo, params object[] requestArgs)
        {
            try
            {
                TRes response = (TRes)(originalMethodInfo.Invoke(target, requestArgs));
                EndInvocation(response);
                return response;
            }
            catch (Exception ex)
            {
                EndInvocation(ex.InnerException); //outer exception is TargetInvocationException (System.Reflection)
                throw ex.InnerException;
            }
        }

        //not thread safe 
        public TRes Record(object target, MethodInfo originalMethodInfo, params object[] requestArgs)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                TRes response = (TRes)(originalMethodInfo.Invoke(target, requestArgs));
                requestedInvocation.Duration = sw.ElapsedMilliseconds;

                requestedInvocation.ResponseDisplayInfo = responseIdentifier?.ResolveDisplayInfo(response);
                requestedInvocation.Response = JsonConvert.SerializeObject(response);
                var responseType = response.GetType();

                var regex = new Regex(@"(, PublicKeyToken=(null|\w{16}))|(, Version=[^,]+)|(, Culture=[^,]+)");

                requestedInvocation.ResponseType = regex.Replace(responseType.AssemblyQualifiedName, string.Empty); //save without version, public key token, culture
                
                if (requestedInvocation.Depth == 1)    //root invocation
                {
                    requestedInvocation.RequestRaw = TestFlaskContext.RawRequest;
                }

                TryPersistStepInvocations();

                EndInvocation(response);

                return response;
            }
            catch (Exception ex)
            {
                //outer exception is TargetInvocationException (System.Reflection)
                RecordException(ex.InnerException, sw.ElapsedMilliseconds, requestArgs);
                throw ex.InnerException;
            }
            finally
            {
                sw.Stop();
            }
        }

        private void TryPersistStepInvocations()
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

        private void RecordException(Exception ex, long duration, params object[] requestArgs)
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

        public TRes Play(params object[] requestArgs)
        {
            var loadedInvocation = TestFlaskContext.GetInvocation(requestedInvocation.InstanceHashCode);

            if (!loadedInvocation.IsFaulted)
            {
                var response = (TRes)JsonConvert.DeserializeObject(loadedInvocation.Response, Type.GetType(loadedInvocation.ResponseType));
                EndInvocation(response);
                return response;
            }
            else
            {
                var exception = (Exception)JsonConvert.DeserializeObject(loadedInvocation.Exception, Type.GetType(loadedInvocation.ExceptionType));
                EndInvocation(exception);
                throw exception;
            }
        }
    }
}
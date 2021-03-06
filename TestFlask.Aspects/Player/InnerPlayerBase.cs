﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
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
using TestFlask.Models.Enums;

namespace TestFlask.Aspects.Player
{
    public class InnerPlayerBase
    {
        protected readonly string methodSignature;
        protected readonly string requestDisplayInfo;
        protected readonly string requestIdentifierKey;
        protected internal Invocation requestedInvocation;
        protected readonly ITestFlaskApi api;

        private bool mustPersistAssertionResult;
        protected Regex typeNameSimplifierRegex = new Regex(@"(, PublicKeyToken=(null|\w{16}))|(, Version=[^,]+)|(, Culture=[^,]+)");

        public InnerPlayerBase(string pMethodSignature, string pRequestIdentifierKey, string pRequestDisplayInfo)
        {
            methodSignature = pMethodSignature;
            requestIdentifierKey = pRequestIdentifierKey;
            requestDisplayInfo = pRequestDisplayInfo;
            api = TestFlaskApiFactory.TestFlaskApi;
        }

        public void BeginInvocation(params object[] requestArgs)
        {
            InitContext();

            int currentDepth = ResolveDepth();

            string parentInstanceHashCode = TestFlaskContext.InvocationParentTable.ContainsKey(currentDepth) ? TestFlaskContext.InvocationParentTable[currentDepth] : null;

            TestFlaskContext.CurrentDepth = currentDepth + 1;

            //if it is root
            if (TestFlaskContext.IsRootDepth)
            {
                TestFlaskContext.ContextId = Guid.NewGuid().ToString();
            }

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
            requestedInvocation.Request = JsonConvert.SerializeObject(requestArgs, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
            });

            //set hash codes
            requestedInvocation.ParentInstanceHashCode = parentInstanceHashCode;
            requestedInvocation.SignatureHashCode = requestedInvocation.GetSignatureHashCode();
            requestedInvocation.RequestHashCode = requestedInvocation.GetRequestHashCode();
            requestedInvocation.DeepHashCode = requestedInvocation.GetDeepHashCode();
            requestedInvocation.LeafHashCode = requestedInvocation.GetLeafHashCode();
            //set an invocation index and set instance hashcode for the leaf
            SetInstanceHashCode();

            requestedInvocation.RecordedOn = DateTime.UtcNow;

            //make this invocation latest parent for the current depth
            TestFlaskContext.InvocationParentTable[TestFlaskContext.CurrentDepth] = requestedInvocation.InstanceHashCode;

            //add this invocation to InvocationStack
            //TODO: make this optional to reduce memory peformance issues
            TestFlaskContext.InvocationStack.Push(requestedInvocation);
        }

        private void SetInstanceHashCode()
        {
            var invocationLeafTable = TestFlaskContext.InvocationLeafTable;
            if (!invocationLeafTable.ContainsKey(requestedInvocation.LeafHashCode))
            {
                invocationLeafTable[requestedInvocation.LeafHashCode] = 0;
                requestedInvocation.InvocationIndex = 0;
            }
            else
            {
                invocationLeafTable[requestedInvocation.LeafHashCode] = invocationLeafTable[requestedInvocation.LeafHashCode] + 1;
                requestedInvocation.InvocationIndex = invocationLeafTable[requestedInvocation.LeafHashCode];
            }

            requestedInvocation.InstanceHashCode = requestedInvocation.GetInvocationInstanceHashCode();
        }

        private void InitContext()
        {
            //if it is initial for a cross request
            if (TestFlaskContext.CurrentDepth == 0 && TestFlaskContext.CallerDepth > 0)
            {
                TestFlaskContext.InvocationParentTable[TestFlaskContext.CallerDepth] = TestFlaskContext.InitialParentInvocationInstance;

                if (TestFlaskContext.RequestedMode != TestModes.NoMock)
                {
                    TestFlaskContext.InvocationLeafTable = api.GetLeafTable(TestFlaskContext.ContextId);
                }
            }
        }

        private int ResolveDepth()
        {
            return Math.Max(TestFlaskContext.CurrentDepth, TestFlaskContext.CallerDepth);
        }

        protected void EndInvocation(object result = null)
        {
            TryPersistStepInvocations();

            TestFlaskContext.CurrentDepth--;

            //if it is back to initial for a cross request
            if (TestFlaskContext.CallerDepth > 0 && TestFlaskContext.CallerDepth == TestFlaskContext.CurrentDepth)
            {
                api.PostLeafTable(TestFlaskContext.ContextId, TestFlaskContext.InvocationLeafTable);
            }

            if (mustPersistAssertionResult && result != null)
            {
                Invocation rootInvocation = TestFlaskContext.GetRootInvocation();
                rootInvocation.AssertionResult = JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                });
                api.PutInvocation(rootInvocation); //persist assertion result
            }
        }

        public TestModes DetermineTestMode(params object[] requestArgs)
        {
            var requestedMode = TestFlaskContext.RequestedMode;

            if (requestedMode == TestModes.NoMock)
            {
                return requestedMode;
            }
            else if (requestedMode == TestModes.Record)
            {
                TryCleanStepInvocations();
                return requestedMode;
            }
            else
            {
                TryLoadStep();

                if (requestedMode == TestModes.IntelliRecord)
                {
                    TryCleanStepInvocations();
                }

                Invocation matchedInvocation = TestFlaskContext.GetMatchedInvocation(requestedInvocation);

                if (matchedInvocation != null)
                {
                    if (requestedMode == TestModes.Assert)
                    {
                        mustPersistAssertionResult = TestFlaskContext.IsRootDepth;
                    }

                    if (requestedMode == TestModes.IntelliRecord)
                    {
                        //lookup matched invocation and determine test mode
                        return matchedInvocation.IsReplayable ? TestModes.Play : TestModes.Record;
                    }
                    else
                    {
                        //lookup matched invocation and determine test mode
                        return matchedInvocation.IsReplayable ? TestModes.Play : TestModes.NoMock;
                    }
                }
                else
                {
                    //cannot find matching invocation
                    if (requestedMode == TestModes.IntelliRecord)
                    {
                        return TestModes.Record;
                    }
                    else
                    {
                        return TestModes.NoMock;
                    }
                }
            }
        }

        private void TryLoadStep()
        {
            if (TestFlaskContext.LoadedStep == null && (TestFlaskContext.IsRootDepth || TestFlaskContext.IsInitialDepth))
            {
                TestFlaskContext.LoadedStep = api.LoadStep(requestedInvocation.StepNo);
            }
        }

        protected void TryCleanStepInvocations()
        {
            var step = TestFlaskContext.RequestedStep;

            if (TestFlaskContext.IsRootDepth && TestFlaskContext.IsOverwriteStep)
            {
                api.DeleteStepInvocations(step);
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

            var requestedMode = TestFlaskContext.RequestedMode;

            if (requestedMode == TestModes.Record || requestedMode == TestModes.IntelliRecord)
            {
                if (TestFlaskContext.IsInitialDepth)
                {
                    api.AppendStepInvocations(step);
                }

                if (TestFlaskContext.IsRootDepth)
                {
                    api.CompleteStepInvocations(step);
                }
            }
        }

        protected void RecordException(Exception ex, long duration, params object[] requestArgs)
        {
            SetException(InvocationMode.Call, duration, ex);
            EndInvocation(ex);
        }

        protected void SetException(InvocationMode invocationMode, long duration, Exception exception)
        {
            requestedInvocation.IsFaulted = true;
            requestedInvocation.ExceptionType = typeNameSimplifierRegex.Replace(exception.GetType().AssemblyQualifiedName, string.Empty);
            requestedInvocation.Exception = JsonConvert.SerializeObject(exception, Type.GetType(requestedInvocation.ExceptionType), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
            });

            requestedInvocation.Duration = duration;
            requestedInvocation.InvocationMode = invocationMode;

            if (requestedInvocation.Depth == 1)    //root invocation
            {
                requestedInvocation.RequestRaw = TestFlaskContext.RawRequest;
            }
        }

        /// <summary>
        /// This method obtains the interface that declared the weaved method, and if there is no interface it returns class that the method is declared
        /// Reflected type is persisted in invocation entity to increase the flexibilty of the tool. 
        /// It is actually used in unit test generation to declare SUT type.
        /// </summary>
        /// <param name="originalMethodInfo"></param>
        protected void ResolveReflectedInterfaceType(MethodInfo originalMethodInfo)
        {
            MethodInfo methodInterface = GetInterfaceMethod(originalMethodInfo);
            requestedInvocation.ReflectedType = typeNameSimplifierRegex.Replace(methodInterface.ReflectedType.AssemblyQualifiedName, string.Empty);
        }

        protected static MethodInfo GetInterfaceMethod(MethodInfo concreteMethod)
        {
            //find weaved method using declared type as they are definitely in the same type
            MethodInfo weavedMethod = concreteMethod.DeclaringType.GetMethod(concreteMethod.Name.Replace("__Original", string.Empty),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, //these flags are enough? TestFlask does not support property weaving yet
                null,
                concreteMethod.GetParameters().Select(pi => pi.ParameterType).ToArray(),
                null);

            // get the type that the method is defined in (use reflected type to overcome polymorphism)
            Type classType = weavedMethod.ReflectedType;

            // get the interfaces that the type implements
            Type[] interfaces = classType.GetInterfaces();

            // iterate through each interface creating mappings,
            // looking for a match to our MethodInfo
            foreach (Type interfaceType in interfaces)
            {
                // get the mapping for the interface.
                InterfaceMapping map = classType.GetInterfaceMap(interfaceType);

                // iterate through the MethodInfos
                for (int i = 0; i < map.TargetMethods.Length; i++)
                {
                    // look for a match to the passed in MethodInfo
                    if (map.TargetMethods[i] == weavedMethod)
                    {
                        // return the corresponding interface method.
                        return map.InterfaceMethods[i];
                    }
                }
            }

            // return concrete method info if no interfaces found
            return weavedMethod;
        }
    }
}
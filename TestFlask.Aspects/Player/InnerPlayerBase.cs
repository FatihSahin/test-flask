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
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
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
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
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
                if (TestFlaskContext.LoadedStep == null && (TestFlaskContext.IsRootDepth || TestFlaskContext.IsInitialDepth))
                {
                    TestFlaskContext.LoadedStep = api.LoadStep(requestedInvocation.StepNo);
                }

                Invocation existingInvocation = TestFlaskContext.GetLoadedInvocation(requestedInvocation.InstanceHashCode);

                if (existingInvocation != null)
                {
                    if (requestedMode == TestModes.Assert)
                    {
                        mustPersistAssertionResult = TestFlaskContext.IsRootDepth;
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

        protected void TryCleanStepInvocations()
        {
            var step = TestFlaskContext.RequestedStep;

            if (TestFlaskContext.IsRootDepth && TestFlaskContext.IsOverwriteStep)
            {
                api.DeleteStepInvocations(step);
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

            if (TestFlaskContext.IsInitialDepth)
            {
                api.AppendStepInvocations(step);
            }

            if (TestFlaskContext.IsRootDepth)
            {
                api.CompleteStepInvocations(step);
            }
        }

        protected void RecordException(Exception ex, long duration, params object[] requestArgs)
        {
            requestedInvocation.IsFaulted = true;
            requestedInvocation.ExceptionType = ex.GetType().ToString();
            requestedInvocation.Exception = JsonConvert.SerializeObject(ex, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            });

            requestedInvocation.Duration = duration;

            if (requestedInvocation.Depth == 1)    //root invocation
            {
                requestedInvocation.RequestRaw = TestFlaskContext.RawRequest;
            }

            TryPersistStepInvocations();

            EndInvocation(ex);
        }

        protected void ResolveReflectedInterfaceType(MethodInfo originalMethodInfo)
        {
            MethodInfo methodInterface = GetInterfaceMethod(originalMethodInfo);
            requestedInvocation.ReflectedType = typeNameSimplifierRegex.Replace(methodInterface.ReflectedType.AssemblyQualifiedName, string.Empty);
        }

        protected static MethodInfo GetInterfaceMethod(MethodInfo concreteMethod)
        {
            // get the type that the method is defined in
            MethodInfo weavedMethod = concreteMethod.DeclaringType.GetMethod(concreteMethod.Name.Replace("__Original", string.Empty),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                null,
                concreteMethod.GetParameters().Select(pi => pi.ParameterType).ToArray(),
                null);

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
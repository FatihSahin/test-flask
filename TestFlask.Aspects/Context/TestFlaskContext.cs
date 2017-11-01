using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TestFlask.Aspects.Config;
using TestFlask.Aspects.Enums;
using TestFlask.Models.Context;
using TestFlask.Models.Entity;

namespace TestFlask.Aspects.Context
{
    public static class TestFlaskContext
    {
        public static string ContextId
        {
            get
            {
                return GetContextId();
            }
            set
            {
                SetContextId(value);
            }
        }

        public static Dictionary<string, int> InvocationLeafTable
        {
            get
            {
                return GetInvocationLeafTable();
            }
            set
            {
                SetInvocationLeafTable(value);
            }
        }

        public static Dictionary<int, string> InvocationParentTable => GetInvocationParentTable();

        public static Step RequestedStep => GetRequestedStep();

        public static string InitialParentInvocationInstance => GetInitialParentInvocationInstance();

        public static Step LoadedStep
        {
            get
            {
                return GetLoadedStep();
            }
            set
            {
                SetLoadedStep(value);
            }
        }

        public static int CurrentDepth
        {
            get
            {
                return GetDepth();
            }
            set
            {
                SetDepth(value);
            }
        }

        public static TestModes RequestedMode => GetRequestedMode();

        public static string RawRequest
        {
            get
            {
                return GetRawRequest();
            }
            set
            {
                SetRawRequest(value);
            }
        }

        public static int InitialDepth => GetInitialDepth();

        public static bool IsRootDepth => CurrentDepth == 1;

        public static bool IsInitialDepth => InitialDepth > 0 && CurrentDepth == (InitialDepth + 1);

        public static bool IsOverwriteStep => bool.Parse(HttpContextFactory.Current.Items["TestFlask_OverwriteStep"].ToString() ?? "false");

        #region NotPublic

        private static string GetContextId()
        {
            if (HttpContextFactory.Current != null)
            {
                if (HttpContextFactory.Current.Items.Contains(ContextKeys.ContextId))
                {
                    return HttpContextFactory.Current.Items[ContextKeys.ContextId].ToString();
                }

                string ctxId = HttpContextFactory.Current.Request.Headers[ContextKeys.ContextId];

                if (ctxId != null)
                {
                    HttpContextFactory.Current.Items[ContextKeys.ContextId] = ctxId;
                }

                return ctxId;
            }

            return null;
        }

        private static void SetContextId(string id)
        {
            if (HttpContextFactory.Current != null)
            {
                HttpContextFactory.Current.Items[ContextKeys.ContextId] = id;
            }
        }

        private static TestModes GetRequestedMode()
        {
            if (HttpContextFactory.Current != null)
            {
                var testMode = HttpContextFactory.Current.Request.Headers[ContextKeys.TestMode];

                if (testMode != null)
                {
                    return (TestModes)Enum.Parse(typeof(TestModes), testMode);
                }
            }

            return TestModes.NoMock; //if no header provided it is a probable real call
        }
        private static Dictionary<string, int> GetInvocationLeafTable()
        {
            if (HttpContextFactory.Current != null)
            {
                var invocationLeafTable = HttpContextFactory.Current.Items["TestFlask_InvocationLeafTable"] as Dictionary<string, int>;

                if (invocationLeafTable == null)
                {
                    invocationLeafTable = new Dictionary<string, int>();
                    HttpContextFactory.Current.Items["TestFlask_InvocationLeafTable"] = invocationLeafTable;
                }

                return invocationLeafTable;
            }

            return null;
        }

        private static void SetInvocationLeafTable(Dictionary<string, int> leafTable)
        {
            if (HttpContextFactory.Current != null)
            {
                HttpContextFactory.Current.Items["TestFlask_InvocationLeafTable"] = leafTable;
            }
        }

        private static Dictionary<int, string> GetInvocationParentTable()
        {
            if (HttpContextFactory.Current != null)
            {
                var invocationParentTable = HttpContextFactory.Current.Items["TestFlask_InvocationParentTable"] as Dictionary<int, string>;

                if (invocationParentTable == null)
                {
                    invocationParentTable = new Dictionary<int, string>();
                    HttpContextFactory.Current.Items["TestFlask_InvocationParentTable"] = invocationParentTable;
                }

                return invocationParentTable;
            }

            return null;
        }

        private static Step GetRequestedStep()
        {
            if (HttpContextFactory.Current != null)
            {
                var step = HttpContextFactory.Current.Items["TestFlask_RequestedStep"] as Step;
                if (step == null)
                {
                    step = BuildRequestedStep();
                    HttpContextFactory.Current.Items["TestFlask_RequestedStep"] = step;
                }

                return step;
            }

            return null;
        }

        internal static Invocation GetRootInvocation()
        {
            return LoadedStep.Invocations.SingleOrDefault(inv => inv.Depth == 1);
        }

        private static Step GetLoadedStep()
        {
            if (HttpContextFactory.Current != null)
            {
                var step = HttpContextFactory.Current.Items["TestFlask_LoadedStep"] as Step;
                return step;
            }

            return null;
        }

        private static void SetLoadedStep(Step step)
        {
            if (HttpContextFactory.Current != null)
            {
                HttpContextFactory.Current.Items["TestFlask_LoadedStep"] = step;
            }
        }

        private static int GetDepth()
        {
            if (HttpContextFactory.Current != null)
            {
                object depthObj = HttpContextFactory.Current.Items["TestFlask_CurrentDepth"];
                if (depthObj != null)
                {
                    return (int)depthObj;
                }
                else
                {
                    HttpContextFactory.Current.Items["TestFlask_CurrentDepth"] = 0;
                    return 0;
                }
            }

            return 0;
        }

        private static void SetDepth(int value)
        {
            if (HttpContextFactory.Current != null)
            {
                HttpContextFactory.Current.Items["TestFlask_CurrentDepth"] = value;
            }
        }

        private static string GetRawRequest()
        {
            if (HttpContextFactory.Current != null)
            {
                object rawRequest = HttpContextFactory.Current.Items["TestFlask_RawRequest"];
                if (rawRequest != null)
                {
                    return (string)rawRequest;
                }
            }

            return null;
        }

        private static void SetRawRequest(string rawRequest)
        {
            if (HttpContextFactory.Current != null)
            {
                HttpContextFactory.Current.Items["TestFlask_RawRequest"] = rawRequest;
            }
        }

        private static string GetInitialParentInvocationInstance()
        {
            if (HttpContextFactory.Current != null)
            {
                return HttpContextFactory.Current.Request.Headers[ContextKeys.ParentInvocationInstance];
            }

            return null;
        }

        private static Step BuildRequestedStep()
        {
            if (HttpContextFactory.Current != null)
            {
                return BuildStepFromHttpContext();
            }

            throw new NotSupportedException("HttpContext must be provided");
        }

        private static Step BuildStepFromHttpContext()
        {
            var request = HttpContextFactory.Current.Request;

            var step = new Step();

            var scNo = request.Headers[ContextKeys.ScenarioNo];
            if (scNo != null)
            {
                step.ScenarioNo = long.Parse(scNo); //clients should always provide scenario no in the header (while beginning a testing sessions ans selecting specific scenario)
            }

            step.StepNo = BuildStepNo();
            step.ProjectKey = TestFlaskConfig.Instance.Project.Key;

            return step;
        }

        private static long BuildStepNo()
        {
            HttpContextFactory.Current.Items.Add("TestFlask_OverwriteStep", false);

            //normally we expect the backend service to intercept incoming call and create a step beforehand on the fly (via httpModule or sth.) and set generated step no on http context items
            if (HttpContextFactory.Current.Items.Contains(ContextKeys.StepNo))
            {
                return (long)HttpContextFactory.Current.Items[ContextKeys.StepNo];
            }
            else if (HttpContextFactory.Current.Request.Headers[ContextKeys.StepNo] != null)
            {
                HttpContextFactory.Current.Items["TestFlask_OverwriteStep"] = true;
                //but if it does not we expect step no (which created elsewhere before that service invocation) on the header
                return long.Parse(HttpContextFactory.Current.Request.Headers[ContextKeys.StepNo]);
            }
            else
            {
                return default(long); //no step no provided, (possibly a nomock mode call)
            }
        }

        public static Invocation GetInvocation(string instanceHashCode)
        {
            return LoadedStep.Invocations.SingleOrDefault(inv => inv.InstanceHashCode == instanceHashCode);
        }

        private static int GetInitialDepth()
        {
            if (HttpContextFactory.Current != null)
            {
                var initialDepth = HttpContextFactory.Current.Request.Headers[ContextKeys.InitialDepth];

                if (initialDepth != null)
                {
                    return int.Parse(initialDepth);
                }
            }

            return 0;
        }

        #endregion
    }
}

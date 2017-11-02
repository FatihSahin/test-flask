using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Aspects.Context;
using TestFlask.Assistant.Core.Models;

namespace TestFlask.Assistant.Core.Outgoing
{
    public static class OutgoingHeadersHelper
    {
        public static string ResolveStepNo()
        {
            return TestFlaskContext.RequestedStep?.StepNo.ToString() ?? AssistantIncomingContext.StepNo;
        }

        public static string ResolveCallerDepth()
        {
            int depth = TestFlaskContext.CurrentDepth;

            if (depth == 0)
            {
                depth = int.Parse(AssistantIncomingContext.CallerDepth ?? "0");
            }

            return depth.ToString();
        }

        public static string ResolveParentInvocationInstanceHashCode()
        {
            return TestFlaskContext.InvocationParentTable.ContainsKey(TestFlaskContext.CurrentDepth) 
                ? TestFlaskContext.InvocationParentTable[TestFlaskContext.CurrentDepth]
                : AssistantIncomingContext.ParentInvocationInstance;
        }

        public static string ResolveContextId()
        {
            return TestFlaskContext.ContextId ?? AssistantIncomingContext.ContextId;
        }
    }
}

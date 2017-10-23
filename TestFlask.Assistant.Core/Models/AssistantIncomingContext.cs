using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Assistant.Core.Config;
using TestFlask.Models.Context;

namespace TestFlask.Assistant.Core.Models
{
    //Just a simple wrapper around TestFlask Http Request Headers for easy access
    public static class AssistantIncomingContext
    {
        public static string ProjectKey
        {
            get
            {
                return HttpContext.Current.Request.Headers[ContextKeys.ProjectKey];
            }
        }

        public static string ScenarioNo
        {
            get
            {
                return HttpContext.Current.Request.Headers[ContextKeys.ScenarioNo];
            }
        }

        public static string StepNo
        {
            get
            {
                return HttpContext.Current.Request.Headers[ContextKeys.StepNo];
            }
        }

        public static string TestMode
        {
            get
            {
                return HttpContext.Current.Request.Headers[ContextKeys.TestMode];
            }
        }

        public static bool HasTestFlaskHeaders
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ProjectKey) 
                    && !string.IsNullOrWhiteSpace(ScenarioNo);
            }
        }
    }
}
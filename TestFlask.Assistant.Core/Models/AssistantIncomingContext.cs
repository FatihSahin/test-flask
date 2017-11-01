using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Aspects.Context;
using TestFlask.Aspects.Enums;
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
                return HttpContextFactory.Current.Request.Headers[ContextKeys.ProjectKey];
            }
        }

        public static string ScenarioNo
        {
            get
            {
                return HttpContextFactory.Current.Request.Headers[ContextKeys.ScenarioNo];
            }
        }

        public static string StepNo
        {
            get
            {
                return HttpContextFactory.Current.Request.Headers[ContextKeys.StepNo];
            }
        }

        public static string TestMode
        {
            get
            {
                return HttpContextFactory.Current.Request.Headers[ContextKeys.TestMode] ?? TestModes.NoMock.ToString();
            }
        }

        public static string ParentInvocationInstance
        {
            get
            {
                return HttpContextFactory.Current.Request.Headers[ContextKeys.ParentInvocationInstance];
            }
        }

        public static string InitialDepth
        {
            get
            {
                return HttpContextFactory.Current.Request.Headers[ContextKeys.InitialDepth];
            }
        }

        public static string ContextId
        {
            get
            {
                return HttpContextFactory.Current.Request.Headers[ContextKeys.ContextId];
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
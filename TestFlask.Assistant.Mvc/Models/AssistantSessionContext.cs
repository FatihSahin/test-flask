using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Assistant.Core.Config;

namespace TestFlask.Assistant.Mvc.Models
{
    public class AssistantSessionContext
    {
        private const string SessionKey = "TestFlask-AssistantContext";
        private TestFlaskAssistantConfig config;

        public AssistantSessionContext()
        {
            config = TestFlaskAssistantConfig.Instance;
        }
        
        public static AssistantSessionContext Current
        {
            get
            {
                if (HttpContext.Current.Session[SessionKey] == null)
                {
                    HttpContext.Current.Session[SessionKey] = new AssistantSessionContext();                    
                }

                return HttpContext.Current.Session[SessionKey] as AssistantSessionContext;
            }
        }

        public string ProjectKey => config.Project.Key;

        public string ManagerUrl => config.Manager.Url;

        public bool IsViewExpanded { get; set; }

        public int CurrentScenarioNo { get; set; }

        public int OverwriteStepNo { get; set; }

        public bool IsInRecordMode { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Assistant.Config;

namespace TestFlask.Assistant.Core.Models
{
    public class TestFlaskAssistantContext
    {
        private const string SessionKey = "TestFlask-AssistantContext";
        private TestFlaskAssistantConfig config;

        public TestFlaskAssistantContext()
        {
            config = TestFlaskAssistantConfig.Instance;
        }
        
        public static TestFlaskAssistantContext Current
        {
            get
            {
                if (HttpContext.Current.Session[SessionKey] == null)
                {
                    HttpContext.Current.Session[SessionKey] = new TestFlaskAssistantContext();                    
                }

                return HttpContext.Current.Session[SessionKey] as TestFlaskAssistantContext;
            }
        }

        public string ProjectKey => config.Project.Key;

        public string ManagerUrl => config.Manager.Url;

        public bool IsViewExpanded { get; set; }

        public int CurrentScenarioNo { get; set; }

        public int OverwriteStepNo { get; set; }

        public bool RecordMode { get; set; }
    }
}
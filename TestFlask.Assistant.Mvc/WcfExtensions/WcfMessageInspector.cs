using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;
using TestFlask.Assistant.Core.Config;
using TestFlask.Assistant.Mvc.Models;
using TestFlask.Models.Context;

namespace TestFlask.Assistant.Mvc.WcfExtensions
{
    public class WcfMessageInspector : IClientMessageInspector
    {
        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a WCF service.
        /// </summary>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var config = TestFlaskAssistantConfig.Instance;
            var sessionContext = AssistantSessionContext.Current;

            if (config.Enabled && sessionContext != null && sessionContext.IsInRecordMode) 
            {
                HttpRequestMessageProperty property = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
               
                property.Headers[ContextKeys.ProjectKey] = config.Project.Key;
                property.Headers[ContextKeys.ScenarioNo] = sessionContext.CurrentScenarioNo.ToString();

                property.Headers[ContextKeys.TestMode] = "Record";

                if (sessionContext.OverwriteStepNo > 0)
                {
                    property.Headers[ContextKeys.StepNo] = sessionContext.OverwriteStepNo.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            // Nothing special here
        }
    }
}
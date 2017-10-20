using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;
using TestFlask.Assistant.Config;
using TestFlask.Assistant.Models;
using TestFlask.Models.Context;

namespace TestFlask.Assistant.WcfExtensions 
{
    public class WcfMessageInspector : IClientMessageInspector
    {
        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var config = TestFlaskAssistantConfig.Instance;
            var context = TestFlaskAssistantContext.Current;

            if (config.Enabled && context != null && context.RecordMode) 
            {
                HttpRequestMessageProperty property = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
               
                property.Headers[ContextKeys.ProjectKey] = config.Project.Key;
                property.Headers[ContextKeys.ScenarioNo] = context.CurrentScenarioNo.ToString();

                property.Headers[ContextKeys.TestMode] = "Record";

                if (context.OverwriteStepNo > 0)
                {
                    property.Headers[ContextKeys.StepNo] = context.OverwriteStepNo.ToString();
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
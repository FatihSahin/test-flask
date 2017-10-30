using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;
using TestFlask.Assistant.Core.Config;
using TestFlask.Assistant.Core.Models;
using TestFlask.Assistant.Core.Outgoing;
using TestFlask.Models.Context;

namespace TestFlask.Assistant.Core.WcfExtensions
{
    public class WcfMessageInspector : IClientMessageInspector
    {
        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var config = TestFlaskAssistantConfig.Instance;

            if (config.Enabled && AssistantIncomingContext.HasTestFlaskHeaders)
            {
                string projectKey = AssistantIncomingContext.ProjectKey;
                string scenarioNo = AssistantIncomingContext.ScenarioNo;
                string stepNo = OutgoingHeadersHelper.ResolveStepNo();
                string testMode = AssistantIncomingContext.TestMode;
                string initialDepth = OutgoingHeadersHelper.ResolveInitialDepth();
                string parentInvocationInstance = OutgoingHeadersHelper.ResolveParentInvocationInstanceHashCode();

                HttpRequestMessageProperty property = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;

                property.Headers[ContextKeys.ProjectKey] = projectKey;
                property.Headers[ContextKeys.ScenarioNo] = scenarioNo;

                if (!string.IsNullOrWhiteSpace(stepNo))
                {
                    property.Headers[ContextKeys.StepNo] = stepNo;
                }

                if (!string.IsNullOrWhiteSpace(testMode))
                {
                    property.Headers[ContextKeys.TestMode] = testMode;
                }

                if (!string.IsNullOrEmpty(initialDepth))
                {
                    property.Headers[ContextKeys.InitialDepth] = initialDepth;
                }

                if (!string.IsNullOrEmpty(parentInvocationInstance))
                {
                    property.Headers[ContextKeys.ParentInvocationInstance] = parentInvocationInstance;
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
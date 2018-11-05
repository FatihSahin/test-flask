using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;
using TestFlask.Aspects.ApiClient;
using TestFlask.Aspects.Context;
using TestFlask.Aspects.Enums;
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
                string callerDepth = OutgoingHeadersHelper.ResolveCallerDepth();
                string parentInvocationInstance = OutgoingHeadersHelper.ResolveParentInvocationInstanceHashCode();
                string contextId = OutgoingHeadersHelper.ResolveContextId();

                HttpRequestMessageProperty property = null;
                if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    property = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                }
                else
                {
                    property = new HttpRequestMessageProperty();
                    request.Properties.Add(HttpRequestMessageProperty.Name, property);
                }

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

                if (!string.IsNullOrEmpty(callerDepth))
                {
                    property.Headers[ContextKeys.CallerDepth] = callerDepth;
                }

                if (!string.IsNullOrEmpty(parentInvocationInstance))
                {
                    property.Headers[ContextKeys.ParentInvocationInstance] = parentInvocationInstance;
                }

                if (!string.IsNullOrEmpty(contextId))
                {
                    property.Headers[ContextKeys.ContextId] = contextId;
                }

                var mode = (TestModes)Enum.Parse(typeof(TestModes), testMode);

                if (mode != TestModes.NoMock && TestFlaskContext.InvocationLeafTable?.Count > 0)
                {
                    TestFlaskApi api = new TestFlaskApi();
                    api.PostLeafTable(contextId, TestFlaskContext.InvocationLeafTable);
                }
            }

            return null;
        }

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            string testMode = AssistantIncomingContext.TestMode;
            var mode = (TestModes)Enum.Parse(typeof(TestModes), testMode);
            string contextId = OutgoingHeadersHelper.ResolveContextId();

            if (mode != TestModes.NoMock && TestFlaskContext.InvocationLeafTable?.Count > 0)
            {
                TestFlaskApi api = new TestFlaskApi();
                var leafTable = api.GetLeafTable(contextId);

                if (leafTable?.Count > 0)
                {
                    TestFlaskContext.InvocationLeafTable = leafTable;
                }
            }
        }
    }
}
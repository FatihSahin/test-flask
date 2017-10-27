using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestFlask.Aspects.Context;
using TestFlask.Assistant.Core.Config;
using TestFlask.Assistant.Core.Models;
using TestFlask.Assistant.Core.Outgoing;
using TestFlask.Models.Context;

namespace TestFlask.Assistant.Core.WebApi
{
    /// <summary>
    /// This class is used for ASP.NET Web API clients. 
    /// HttpClient that is plugged in with this handler is created like this => HttpClient client = HttpClientFactory.Create(new TestFlaskMessageHandler());
    /// </summary>
    public class TestFlaskMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
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

                request.Headers.Add(ContextKeys.ProjectKey, projectKey);
                request.Headers.Add(ContextKeys.ScenarioNo, scenarioNo);

                if (!string.IsNullOrWhiteSpace(stepNo))
                {
                    request.Headers.Add(ContextKeys.StepNo, stepNo);
                }

                if (!string.IsNullOrWhiteSpace(testMode))
                {
                    request.Headers.Add(ContextKeys.TestMode, testMode);
                }

                if (!string.IsNullOrEmpty(initialDepth))
                {
                    request.Headers.Add(ContextKeys.InitialDepth, initialDepth);
                }
                
                if (!string.IsNullOrEmpty(parentInvocationInstance))
                {
                    request.Headers.Add(ContextKeys.ParentInvocationInstance, parentInvocationInstance);
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}

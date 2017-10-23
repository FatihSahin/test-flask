using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestFlask.Assistant.Core.Config;
using TestFlask.Assistant.Mvc.Models;
using TestFlask.Models.Context;

namespace TestFlask.Assistant.Mvc.WebApi
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
            var context = AssistantSessionContext.Current;

            if (config.Enabled && context != null && context.IsInRecordMode)
            {
                request.Headers.Add(ContextKeys.ProjectKey, config.Project.Key);
                request.Headers.Add(ContextKeys.ScenarioNo, context.CurrentScenarioNo.ToString());
                request.Headers.Add(ContextKeys.TestMode, "Record");

                if (context.OverwriteStepNo > 0)
                {
                    request.Headers.Add(ContextKeys.StepNo, context.OverwriteStepNo.ToString());
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestFlask.Aspects.Enums;
using TestFlask.Models.Context;

namespace TestFlask.Aspects.Context
{
    /// <summary>
    /// This class is Web API equivalent of <see cref="TestFlaskHttpModule"/>. 
    /// It actually captures raw request and sets it to TestFlaskContext.
    /// Also it may create an auto step document on record mode if no existing step no is provided 
    /// It must be registered in WebApiConfig class via config.MessageHandlers.Add(new TestFlaskMessageHandler());
    /// </summary>
    public class TestFlaskMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Contains(ContextKeys.TestMode) 
                && request.Headers.Contains(ContextKeys.ProjectKey)
                && request.Headers.Contains(ContextKeys.ScenarioNo))
            {
                //if backend is called with record mode
                if (request.Headers.GetValues(ContextKeys.TestMode).First() == TestModes.Record.ToString())
                {
                    CaptureRawRequest(request);

                    if (!request.Headers.Contains(ContextKeys.StepNo))
                    {
                        CreateAutoStep(request);
                    }
                }
            }

            return base.SendAsync(request, cancellationToken);
        }

        private void CaptureRawRequest(HttpRequestMessage request)
        {
            StringBuilder requestBuilder = new StringBuilder();

            //First Line, I could not find a way to get server protocol, instead, it is hardcoded as HTTP/1.1
            requestBuilder.Append($"{request.Method} {request.RequestUri.AbsoluteUri} HTTP/1.1").Append("\n");

            //Headers
            foreach (var header in request.Headers)
            {
                var multiVal = String.Join(";", header.Value);
                requestBuilder.Append($"{header.Key}: {multiVal}").Append("\n");
            }

            //request body
            requestBuilder.Append("\n");
            requestBuilder.Append(request.Content.ReadAsStringAsync().Result);

            TestFlaskContext.RawRequest = requestBuilder.ToString();
        }

        private void CreateAutoStep(HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }


    }
}

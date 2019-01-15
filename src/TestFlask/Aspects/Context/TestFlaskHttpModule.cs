using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TestFlask.Aspects.ApiClient;
using TestFlask.Aspects.Enums;
using TestFlask.Models.Context;
using TestFlask.Models.Entity;

namespace TestFlask.Aspects.Context
{
    /// <summary>
    /// This class works for WCF and ASP.NET Web API servers. It actually captures raw request and sets it to TestFlaskContext.
    /// Also may create an auto step document on record mode if no existing step no is provided 
    /// It must be registered in system.webServer > modules section inside service web.config.
    /// </summary>
    public class TestFlaskHttpModule : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            HttpRequest request = context.Request;

            string testMode = request.Headers[ContextKeys.TestMode];

            //if in no mock mode dont bother to do anything
            if (testMode == TestModes.NoMock.ToString())
            {
                return;
            }

            CaptureRawRequest(request);

            //if backend is called with record mode (do not create an auto step for intellirecord, intellirecord is meant to be called upon an early recorded step that is already provided)
            if (testMode == TestModes.Record.ToString())
            {
                if (request.Headers[ContextKeys.StepNo] == null)
                {
                    CreateAutoStep(request);
                }
            }
        }

        private static void CaptureRawRequest(HttpRequest request)
        {
            StringBuilder requestBuilder = new StringBuilder();

            //First Line
            requestBuilder.Append($"{request.HttpMethod} {request.Url.AbsoluteUri} {request.ServerVariables["SERVER_PROTOCOL"]}").Append("\n");

            //Headers
            foreach (string headerKey in request.Headers)
            {
                string headerValue = request.Headers[headerKey];
                requestBuilder.Append($"{headerKey}: {headerValue}").Append("\n");
            }

            //request body
            var bytes = new byte[request.InputStream.Length];
            request.InputStream.Read(bytes, 0, bytes.Length);
            request.InputStream.Position = 0;
            string body = Encoding.UTF8.GetString(bytes);

            requestBuilder.Append("\n");
            requestBuilder.Append(body);

            TestFlaskContext.RawRequest = requestBuilder.ToString();
        }

        private void CreateAutoStep(HttpRequest request)
        {
            Step autoStep = new Step();

            autoStep.StepName = $"Auto Step {DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            autoStep.StepDescription = "Auto Step Description";
            autoStep.ProjectKey = request.Headers[ContextKeys.ProjectKey];
            autoStep.ScenarioNo = long.Parse(request.Headers[ContextKeys.ScenarioNo]);
            
            var api = new TestFlaskApi();
            var dbStep = api.InsertStep(autoStep);

            HttpContextFactory.Current.Items.Add(ContextKeys.StepNo, dbStep.StepNo);
        }
    }
}


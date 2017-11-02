using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TestFlask.Aspects.ApiClient;
using TestFlask.Aspects.Context;
using TestFlask.Aspects.Enums;
using TestFlask.Models.Context;
using TestFlask.Models.Entity;

namespace TestFlask.Aspects.Tests
{
    public abstract class PlayerTestsBase
    {
        protected Mock<HttpContextBase> mockHttpContext;
        protected Mock<ITestFlaskApi> mockTestFlaskApi;
        protected Mock<HttpRequestBase> mockHttpRequest;

        protected Dictionary<string, object> httpItems;
        protected NameValueCollection requestHeaders;

        protected Dictionary<long, Step> recordedSteps;

        protected virtual void SetUp()
        {
            mockHttpRequest = new Mock<HttpRequestBase>();
            mockHttpContext = new Mock<HttpContextBase>();

            httpItems = new Dictionary<string, object>();
            mockHttpContext.Setup(c => c.Items).Returns(httpItems);

            requestHeaders = new NameValueCollection();
            requestHeaders.Add(ContextKeys.ProjectKey, "UnitTest");
            requestHeaders.Add(ContextKeys.ScenarioNo, "999");
            requestHeaders.Add(ContextKeys.TestMode, TestModes.NoMock.ToString());

            mockHttpRequest.Setup(r => r.Headers).Returns(requestHeaders);
            mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);

            recordedSteps = new Dictionary<long, Step>();
            mockTestFlaskApi = new Mock<ITestFlaskApi>();

            mockTestFlaskApi.Setup(api => api.CompleteStepInvocations(It.IsAny<Step>())).Callback<Step>(step => {
                recordedSteps.Add(step.StepNo, step);
            });

            mockTestFlaskApi.Setup(api => api.GetStep(It.IsAny<long>())).Returns<long>(stepNo => recordedSteps[stepNo]);

            HttpContextFactory.Current = mockHttpContext.Object;
            TestFlaskApiFactory.TestFlaskApi = mockTestFlaskApi.Object;
        }

        [TearDown]
        protected virtual void TearDown()
        {
            requestHeaders.Clear();
            httpItems.Clear();
            recordedSteps.Clear();
        }
    }
}

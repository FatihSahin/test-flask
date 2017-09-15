using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Aspects.Config;
using TestFlask.Aspects.Context;
using TestFlask.Models.Context;
using TestFlask.Models.Entity;

namespace TestFlask.Aspects.ApiClient
{
    //It is called from weaver (do not use DI container here)
    public class TestFlaskApiClient
    {
        private readonly TestFlaskConfig config;

        public TestFlaskApiClient()
        {
            config = TestFlaskConfig.Instance;
        }

        private HttpClient PrepareClient()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(config.Api.Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add(ContextKeys.ProjectKey, config.Project.Key);

            return client;
        }

        public void PutStepInvocations(Step step)
        {
            var httpClient = PrepareClient();
            var response = httpClient.PutAsJsonAsync("api/step/invocations", step).Result;
        }

        public Step InsertStep(Step step)
        {
            var httpClient = PrepareClient();

            var response = httpClient.PostAsJsonAsync($"api/step/", step).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Step>().Result;
            }

            return null;
        }

        public Step GetStep(long stepNo)
        {
            var httpClient = PrepareClient();

            var response = httpClient.GetAsync($"api/step/{stepNo}").Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Step>().Result;
            }

            return null;
        }

        internal void PutInvocation(Invocation invocation)
        {
            var httpClient = PrepareClient();
            var response = httpClient.PutAsJsonAsync($"api/step/invocation", invocation).Result; 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using TestFlask.Assistant.Core.Config;
using TestFlask.Models.Context;
using TestFlask.Models.Entity;

namespace TestFlask.Assistant.Mvc.ApiClient
{
    public class TestFlaskApi
    {
        private readonly TestFlaskAssistantConfig config;

        public TestFlaskApi()
        {
            config = TestFlaskAssistantConfig.Instance;
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

        public IEnumerable<Scenario> GetScenarios()
        {
            var httpClient = PrepareClient();
            var response = httpClient.GetAsync($"api/project/scenarios/{config.Project.Key}").Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Scenario[]>().Result;
            }

            return null;
        }

        public IEnumerable<Step> GetSteps(int scenarioNo)
        {
            var httpClient = PrepareClient();
            var response = httpClient.GetAsync($"api/scenario/{scenarioNo}").Result;

            if (response.IsSuccessStatusCode)
            {
                var scenario = response.Content.ReadAsAsync<Scenario>().Result;
                return scenario.Steps;
            }

            return null;
        }

        public Scenario CreateScenario(Scenario scenario)
        {
            var httpClient = PrepareClient();
            var response = httpClient.PostAsJsonAsync("api/scenario", scenario).Result;

            if (response.IsSuccessStatusCode)
            {
                var apiScenario = response.Content.ReadAsAsync<Scenario>().Result;
                return apiScenario;
            }

            return null;
        }

        public Step UpdateStepShallow(Step step)
        {
            var httpClient = PrepareClient();
            var response = httpClient.PutAsJsonAsync("api/step/shallow", step).Result;

            if (response.IsSuccessStatusCode)
            {
                var apiStep = response.Content.ReadAsAsync<Step>().Result;
                return apiStep;
            }

            return null;
        }
    }
}
﻿using System;
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
    public class TestFlaskApi : ITestFlaskApi
    {
        private readonly TestFlaskConfig config;

        public TestFlaskApi()
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

        public void CompleteStepInvocations(Step step)
        {
            var httpClient = PrepareClient();
            var response = httpClient.PutAsJsonAsync("api/step/invocations/complete", step).Result;
        }

        public void AppendStepInvocations(Step step)
        {
            var httpClient = PrepareClient();
            var response = httpClient.PutAsJsonAsync("api/step/invocations/append", step).Result;
        }

        public void DeleteStepInvocations(Step step)
        {
            var httpClient = PrepareClient();
            var response = httpClient.DeleteAsync($"api/step/invocations/{step.ScenarioNo}/{step.StepNo}").Result;
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

        public Step LoadStep(long stepNo)
        {
            var httpClient = PrepareClient();

            var response = httpClient.GetAsync($"api/step/load/{stepNo}").Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Step>().Result;
            }

            return null;
        }

        public void PutInvocation(Invocation invocation)
        {
            var httpClient = PrepareClient();
            var response = httpClient.PutAsJsonAsync($"api/step/invocation", invocation).Result; 
        }

        public Dictionary<string, int> GetLeafTable(string contextId)
        {
            var httpClient = PrepareClient();
            var response = httpClient.GetAsync($"api/context/leafTable/{contextId}").Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Dictionary<string, int>>().Result;
            }

            return null;
        }

        public void PostLeafTable(string contextId, Dictionary<string, int> leafTable)
        {
            var httpClient = PrepareClient();
            var response = httpClient.PostAsJsonAsync($"api/context/leafTable/{contextId}", leafTable).Result;
        }

        public Assertion GetAssertion(long stepNo)
        {
            var httpClient = PrepareClient();

            var response = httpClient.GetAsync($"api/step/assertion/{stepNo}").Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Assertion>().Result;
            }

            return null;
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

        public IEnumerable<Scenario> SearchScenarios(Scenario searchObj)
        {
            var httpClient = PrepareClient();

            string searchQuery = PrepareSearchQueryForScenario(searchObj);

            var response = httpClient.GetAsync($"api/project/scenarios/search?{searchQuery}").Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Scenario[]>().Result;
            }

            return null;
        }

        private string PrepareSearchQueryForScenario(Scenario searchObj)
        {
            StringBuilder sb = new StringBuilder($"ProjectKey={searchObj.ProjectKey}");

            if (searchObj.Labels != null && searchObj.Labels.Any())
            {
                foreach (var label in searchObj.Labels) {
                    sb.Append($"&Labels={label}");
                }
            }

            return sb.ToString();
        }

        public Scenario LoadScenario(long scenarioNo)
        {
            var httpClient = PrepareClient();
            var response = httpClient.GetAsync($"api/scenario/load/{scenarioNo}").Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Scenario>().Result;
            }

            return null;
        }
    }
}

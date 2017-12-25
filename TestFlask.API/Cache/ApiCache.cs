using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using TestFlask.Models.Entity;

namespace TestFlask.API.Cache
{
    public static class ApiCache
    {
        private static MemoryCache memoryCache = MemoryCache.Default;

        private const string scenarioPrefix = "Scenario";
        private const string projectPrefix = "Project";
        private const string variablePrefix = "Variable";

        public static T Get<T>(string key)
        {
            return (T)memoryCache.Get(key);
        }

        public static bool Add(string key, object value)
        {
            return Add(key, value, DateTimeOffset.UtcNow.AddMinutes(30));
        }

        public static bool Add(string key, object value, DateTimeOffset absExpiration)
        {
            return memoryCache.Add(key, value, absExpiration);
        }

        public static void Delete(string key)
        {
            if (memoryCache.Contains(key))
            {
                memoryCache.Remove(key);
            }
        }

        public static bool AddProject(Project project)
        {
            string key = GetProjectKey(project.ProjectKey);
            return Add(key, project);
        }

        public static bool AddScenario(Scenario scenario)
        {
            string key = GetScenarioKey(scenario.ScenarioNo);
            return Add(key, scenario);
        }

        private static string GetProjectKey(string projectKey)
        {
            return $"{projectPrefix}-{projectKey}";
        }

        private static string GetScenarioKey(long scenarioNo)
        {
            return $"{scenarioPrefix}-{scenarioNo}";
        }

        internal static Project GetProject(string projectKey)
        {
            string key = GetProjectKey(projectKey);
            return Get<Project>(key);
        }

        public static Scenario GetScenario(long scenarioNo)
        {
            string key = GetScenarioKey(scenarioNo);
            return Get<Scenario>(key);
        }

        public static void DeleteProject(string projectKey)
        {
            string key = GetProjectKey(projectKey);
            Delete(key);
        }

        public static void DeleteScenario(long scenarioNo)
        {
            string key = GetScenarioKey(scenarioNo);
            Delete(key);
        }

        public static IEnumerable<Variable> GetVariablesByProject(string projectKey)
        {
            return Get<IEnumerable<Variable>>($"{variablePrefix}-{projectKey}");
        }


        public static void AddVariableByProject(string projectKey, IEnumerable<Variable> variables)
        {
            Add($"{variablePrefix}-{projectKey}", variables.ToList());
        }

        public static void DeleteVariableByProject(string projectKey)
        {
            Delete($"{variablePrefix}-{projectKey}");
        }

        private static string GetVariableKey(string projectKey, long scenarioNo, long stepNo, string name)
        {
            return $"{variablePrefix}-{projectKey}-{scenarioNo}-{name}";
        }

        private static string GetVariableKey(Variable variable)
        {
            return $"{variablePrefix}-{variable.ProjectKey}-{variable.ScenarioNo}-{variable.Name}";
        }

        public static void AddVariable(Variable variable)
        {
            Add(GetVariableKey(variable), variable);
        }

        public static void DeleteVariable(Variable variable)
        {
            Delete(GetVariableKey(variable));
        }

        public static Variable GetVariable(string projectKey, long scenarioNo, long stepNo, string name)
        {
            return Get<Variable>(GetVariableKey(projectKey, scenarioNo, stepNo, name));
        }
    }
}
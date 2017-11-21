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

        public static bool AddScenario(Scenario scenario)
        {
            string key = GetScenarioKey(scenario.ScenarioNo);
            return Add(key, scenario);
        }

        private static string GetScenarioKey(long scenarioNo)
        {
            return $"{scenarioPrefix}-{scenarioNo}";
        }

        public static Scenario GetScenario(long scenarioNo)
        {
            string key = GetScenarioKey(scenarioNo);
            return Get<Scenario>(key);
        }

        public static void DeleteScenario(long scenarioNo)
        {
            string key = GetScenarioKey(scenarioNo);
            Delete(key);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TestFlask.API.Cache;

namespace TestFlask.API.Controllers
{
    public class ContextController : ApiController
    {
        [Route("api/context/leafTable/{contextId}")]
        public Dictionary<string, int> GetLeafTable(string contextId)
        {
            var leafTable = ApiCache.Get<Dictionary<string, int>>(GetLeafTableKey(contextId));
            return leafTable; 
        }

        [Route("api/context/leafTable/{contextId}")]
        public void PostLeafTable([FromUri] string contextId, [FromBody] Dictionary<string, int> leafTable)
        {
            string key = GetLeafTableKey(contextId);

            ApiCache.Delete(key);
            ApiCache.Add(key, leafTable);
        }

        private static string GetLeafTableKey(string contextId)
        {
            return $"LeafTable-{contextId}";
        }
    }
}
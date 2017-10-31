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
        private static MemoryCacher cacher = new MemoryCacher();

        [Route("api/context/leafTable/{contextId}")]
        public Dictionary<string, int> GetLeafTable(string contextId)
        {
            var leafTable = cacher.GetValue(GetLeafTableKey(contextId)) as Dictionary<string, int>;
            return leafTable; 
        }

        [Route("api/context/leafTable/{contextId}")]
        public void PostLeafTable([FromUri] string contextId, [FromBody] Dictionary<string, int> leafTable)
        {
            string key = GetLeafTableKey(contextId);

            cacher.Delete(key);
            cacher.Add(key, leafTable, DateTimeOffset.UtcNow.AddMinutes(15)); //fifteen minutes for caching
        }

        private static string GetLeafTableKey(string contextId)
        {
            return $"LeafTable-{contextId}";
        }
    }
}
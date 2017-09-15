using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;

namespace TestFlask.Data.Repos
{
    public interface ICounterRepo : IMongoRepo<Counter>
    {
        Counter GetNextCounter(string counterKey);

        IList<Counter> InitCounters(string[] keys);
    }

    public class CounterRepo : MongoRepo<Counter>, ICounterRepo
    {
        public CounterRepo(IMongoDatabase mongoDb) : base(mongoDb, "counters")
        {
        }

        public IList<Counter> InitCounters(string[] keys)
        {
            List<Counter> counterList = new List<Counter>();

            foreach (var key in keys)
            {
                var counterQuery = Builders<Counter>.Filter.Eq(c => c.CounterKey, key);

                var counter = Collection.Find(counterQuery).SingleOrDefault();

                if (counter == null)
                {
                    counter = new Counter { CounterKey = key, CounterValue = 1 };
                    Collection.InsertOne(counter);
                }

                counterList.Add(counter);
            }

            return counterList;
        }

        public Counter GetNextCounter(string counterKey)
        {
            var counterQuery = Builders<Counter>.Filter.Eq(c => c.CounterKey, counterKey);

            var counter = Collection.FindOneAndUpdate(counterQuery,
                Builders<Counter>.Update.Inc(c => c.CounterValue, 1));

            return counter;
        }
    }
}

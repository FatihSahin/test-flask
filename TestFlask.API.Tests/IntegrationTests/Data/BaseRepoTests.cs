using System;
using MongoDB.Driver;
using TestFlask.Data.Repos;
using NUnit.Framework;

namespace TestFlask.API.Tests.IntegrationTests.Data
{
    public class BaseRepoTests
    {
        protected MongoClient client;
        protected IMongoDatabase db;

        [SetUp]
        public void Initialize()
        {
            client = new MongoClient("mongodb://localhost");
            db = client.GetDatabase("test");
        }

        private void InitCounters()
        {
            CounterRepo counterRepo = new CounterRepo(db);
            counterRepo.InitCounters(new string[] { "project", "scenario" });
        }
    }
}

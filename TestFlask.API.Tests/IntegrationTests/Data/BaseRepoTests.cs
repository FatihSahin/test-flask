using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using TestFlask.Data.Repos;

namespace TestFlask.API.Tests.IntegrationTests.Data
{
    public class BaseRepoTests
    {
        protected MongoClient client;
        protected IMongoDatabase db;

        [TestInitialize]
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

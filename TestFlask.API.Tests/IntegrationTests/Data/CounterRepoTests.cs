using System;
using MongoDB.Driver;
using TestFlask.Data.Repos;
using System.Linq;
using NUnit.Framework;

namespace TestFlask.API.Tests.IntegrationTests.Data
{
    [TestFixture]
    public class CounterRepoTests : BaseRepoTests
    {
        [Test]
        public void Test_Int_InitCounters_Success()
        {
            CounterRepo counterRepo = new CounterRepo(db);

            var keys = new string[] { "project", "scenario" };
            var counterList = counterRepo.InitCounters(keys);

            Assert.AreEqual(keys.Length, counterList.Count);
            Assert.IsTrue(counterList.All(c => c.CounterValue == 1));
        }


        [Test]
        public void Test_Int_GetLastCounter_Success()
        {
            CounterRepo counterRepo = new CounterRepo(db);

            string key = "scenario";

            var counter = counterRepo.GetNextCounter(key);
            long exValue = counter.CounterValue;
            counter = counterRepo.GetNextCounter(key);

            Assert.AreEqual(exValue + 1, counter.CounterValue);
        }
    }
}

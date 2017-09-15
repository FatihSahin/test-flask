using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using TestFlask.Data.Repos;
using System.Linq;

namespace TestFlask.API.Tests.IntegrationTests.Data
{
    [TestClass]
    public class CounterRepoTests : BaseRepoTests
    {
        [TestMethod]
        public void Test_Int_InitCounters_Success()
        {
            CounterRepo counterRepo = new CounterRepo(db);

            var keys = new string[] { "project", "scenario" };
            var counterList = counterRepo.InitCounters(keys);

            Assert.AreEqual(keys.Length, counterList.Count);
            Assert.IsTrue(counterList.All(c => c.CounterValue == 1));
        }


        [TestMethod]
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

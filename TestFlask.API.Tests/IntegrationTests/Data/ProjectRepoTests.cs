using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using TestFlask.Data.Repos;

namespace TestFlask.API.Tests.IntegrationTests.Data
{
    [TestClass]
    public class ProjectRepoTests : BaseRepoTests
    {
        [TestMethod]
        public void Test_Int_InsertProject_Success()
        {
            CounterRepo counterRepo = new CounterRepo(db);
            ProjectRepo projectRepo = new ProjectRepo(db, counterRepo);

            var project = projectRepo.Insert(new Models.Entity.Project
            {
                ProjectDescription = "unitTestProject description",
                ProjectKey = "unitTestProject"
            });

            Assert.IsNotNull(project);
            Assert.IsNotNull(project.Id);
            Assert.IsTrue(project.ProjectNo > 0);
        }
    }
}

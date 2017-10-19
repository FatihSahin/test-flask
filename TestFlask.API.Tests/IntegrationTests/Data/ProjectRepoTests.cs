using System;
using MongoDB.Driver;
using TestFlask.Data.Repos;
using NUnit.Framework;

namespace TestFlask.API.Tests.IntegrationTests.Data
{
    [TestFixture, Category("Integration")]
    public class ProjectRepoTests : BaseRepoTests
    {
        [Test]
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

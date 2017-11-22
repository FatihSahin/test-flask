using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestFlask.Models.Entity;

namespace TestFlask.Data.Repos
{
    public interface IProjectRepo : IMongoRepo<Project>
    {
        Project Insert(Project project);
        IEnumerable<Project> GetAll();
        Project Get(string projectKey);
        Project Update(Project project);
    }

    public class ProjectRepo : MongoRepo<Project>, IProjectRepo
    {
        private readonly ICounterRepo counterRepo;

        public ProjectRepo(IMongoDatabase mongoDb, ICounterRepo pCounterRepo) : base(mongoDb, "projects")
        {
            counterRepo = pCounterRepo;
        }

        public Project Get(string projectKey)
        {
            return Collection
                .Find(Builders<Project>.Filter.Eq(p => p.ProjectKey, projectKey)).SingleOrDefault();
        }

        public IEnumerable<Project> GetAll()
        {
            return Collection
                .Find(Builders<Project>.Filter.Empty).ToList();
        }

        public Project Insert(Project project)
        {
            project.ProjectNo = counterRepo.GetNextCounter("project").CounterValue;
            project.CreatedOn = DateTime.Now;
            Collection.InsertOne(project);

            return project;
        }

        public Project Update(Project project)
        {
            Collection.FindOneAndUpdate(
                Builders<Project>.Filter.Eq(p => p.ProjectKey, project.ProjectKey),
                Builders<Project>.Update.Combine(
                    Builders<Project>.Update.Set(p => p.ProjectName, project.ProjectName),
                    Builders<Project>.Update.Set(p => p.ProjectDescription, project.ProjectDescription),
                    Builders<Project>.Update.Set(p => p.InvocationMatchStrategy, project.InvocationMatchStrategy)
                )
            );

            return project;
        }
    }
}

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Models.Entity;

namespace TestFlask.Data.Repos
{
    public interface IAssertionRepo : IMongoRepo<Assertion>
    {
        Assertion Insert(Assertion project);
        IEnumerable<Assertion> GetByProject(string projectKey);
        IEnumerable<Assertion> GetByScenario(long scenarioNo);
        Assertion GetByStep(long stepNo);
        Assertion InsertOrUpdate(Assertion assertion);
    }

    public class AssertionRepo : MongoRepo<Assertion>, IAssertionRepo
    {

        public AssertionRepo(IMongoDatabase mongoDb) : base(mongoDb, "assertions")
        {
        }

        public IEnumerable<Assertion> GetByProject(string projectKey)
        {
            return Collection.Find(Builders<Assertion>.Filter.Eq(a => a.ProjectKey, projectKey)).ToList();
        }

        public IEnumerable<Assertion> GetByScenario(long scenarioNo)
        {
            return Collection.Find(Builders<Assertion>.Filter.Eq(a => a.ScenarioNo, scenarioNo)).ToList();
        }

        public Assertion GetByStep(long stepNo)
        {
            return Collection.Find(Builders<Assertion>.Filter.Eq(a => a.StepNo, stepNo)).SingleOrDefault();
        }

        public Assertion Insert(Assertion assertion)
        {
            Collection.InsertOne(assertion);

            return assertion;
        }

        public Assertion InsertOrUpdate(Assertion assertion)
        {
            var existing = GetByStep(assertion.StepNo);

            if (existing != null)
            {
                assertion.Id = existing.Id;
                return Collection.FindOneAndReplace(Builders<Assertion>.Filter.Eq(a => a.StepNo, assertion.StepNo), assertion);
            }
            else
            {
                return Insert(assertion);
            }
        }
    }
}

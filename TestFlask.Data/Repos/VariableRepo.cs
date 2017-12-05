using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestFlask.Models.Entity;

namespace TestFlask.Data.Repos
{
    public interface IVariableRepo : IMongoRepo<Variable>
    {
        Variable Insert(Variable project);
        IEnumerable<Variable> GetByProject(string projectKey);
        IEnumerable<Variable> GetByScenario(long scenarioNo);
        IEnumerable<Variable> GetByStep(long stepNo);
        Variable InsertOrUpdate(Variable assertion);
        Variable GetByKey(string projectKey, long scenario, long stepNo, string name);
    }

    public class VariableRepo : MongoRepo<Variable>, IVariableRepo
    {

        public VariableRepo(IMongoDatabase mongoDb) : base(mongoDb, "variables")
        {
        }

        public IEnumerable<Variable> GetByProject(string projectKey)
        {
            return Collection.Find(Builders<Variable>.Filter.Eq(a => a.ProjectKey, projectKey)).ToList();
        }

        public IEnumerable<Variable> GetByScenario(long scenarioNo)
        {
            return Collection.Find(Builders<Variable>.Filter.Eq(a => a.ScenarioNo, scenarioNo)).ToList();
        }

        public IEnumerable<Variable> GetByStep(long stepNo)
        {
            return Collection.Find(Builders<Variable>.Filter.Eq(a => a.StepNo, stepNo)).ToList();
        }


        public Variable Insert(Variable variable)
        {
            Collection.InsertOne(variable);

            return variable;
        }

        public Variable InsertOrUpdate(Variable variable)
        {
            var existing = GetByKey(variable.ProjectKey, variable.ScenarioNo, variable.StepNo, variable.Name);

            if (existing != null)
            {
                variable.Id = existing.Id;
                return Collection.FindOneAndReplace(Builders<Variable>.Filter.Eq(a => a.Id, variable.Id), variable);
            }
            else
            {
                return Insert(variable);
            }
        }

        public Variable GetByKey(string projectKey, long scenario, long stepNo, string name)
        {
            return Collection.Find(Builders<Variable>.Filter.Where(a => a.ProjectKey == projectKey && a.ScenarioNo == scenario && a.StepNo == stepNo && a.Name == name)).SingleOrDefault();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace TestFlask.Models.Entity
{
    public class Scenario : MongoEntity
    {
        //must be auto incremented somehow (atomical findAndModify)
        public long ScenarioNo { get; set; }

        public string ScenarioName { get; set; }

        public string ScenarioDescription { get; set; }

        public string ProjectKey { get; set; }

        public List<Step> Steps { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

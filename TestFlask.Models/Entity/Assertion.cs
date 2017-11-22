using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using TestFlask.Models.Enums;

namespace TestFlask.Models.Entity
{    
    public class Assertion : MongoEntity
    {
        public string ProjectKey { get; set; }
        public long ScenarioNo { get; set; }
        public long StepNo { get; set; }
        public string Expected { get; set; }
        public AssertionStatus Status { get; set; }
        public DateTime LastAssertedOn { get; set; }
        public long Duration { get; set; }
        public string Title { get; set; }
        public string LastAssertionResult { get; set; }
    }
}

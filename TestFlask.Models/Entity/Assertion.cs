using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TestFlask.Models.Entity
{    
    public enum AssertionStatus
    {
        NotAsserted = 0,
        Success = 1,
        Fail = 2
    }

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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using TestFlask.Models.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace TestFlask.Models.Entity
{
    public class Step
    {
        //auto increment (unique)
        public long StepNo { get; set; }

        public long ScenarioNo { get; set; }
        //immutable
        public string ProjectKey { get; set; }

        public string StepName { get; set; }

        public string StepDescription { get; set; }

        //Root invocations probably will not be replayable by default
        public List<Invocation> Invocations { get; set; }

        public DateTime CreatedOn { get; set; }

        public InvocationMatch InvocationMatchStrategy { get; set; }

        public string RootInvocationReflectedType { get; set; }

        [BsonIgnore]
        public InvocationMatch LoadedMatchStrategy { get; set; }

        public Invocation GetRootInvocation()
        {
            return Invocations.FirstOrDefault(i => i.Depth == 1);
        }
    }
}

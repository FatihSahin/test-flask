using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Models.Enums;

namespace TestFlask.Models.Entity
{
    public class Project : MongoEntity
    {
        //must be auto incremented somehow (atomical findAndModify)
        public long ProjectNo { get; set; }
        //it is used as a FK, cannot be modified
        public string ProjectKey { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime CreatedOn { get; set; }
        public InvocationMatch InvocationMatchStrategy { get; set; }
    }
}

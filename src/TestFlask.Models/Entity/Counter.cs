using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Models.Entity
{
    public class Counter : MongoEntity
    {
        public string CounterKey { get; set; }

        public long CounterValue { get; set; }
    }
}

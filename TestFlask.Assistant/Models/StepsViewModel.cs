using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Models.Entity;

namespace TestFlask.Assistant.Models
{
    public class StepsViewModel
    {
        public TestFlaskAssistantContext Context { get; set; }
        public IEnumerable<Step> Steps { get; set; }

        public StepsViewModel(TestFlaskAssistantContext context, IEnumerable<Step> steps)
        {
            Context = context;
            Steps = steps;
        }

    }
}
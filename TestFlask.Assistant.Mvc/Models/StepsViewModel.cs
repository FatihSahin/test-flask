using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Models.Entity;

namespace TestFlask.Assistant.Mvc.Models
{
    public class StepsViewModel
    {
        public AssistantSessionContext Context { get; set; }
        public IEnumerable<Step> Steps { get; set; }

        public StepsViewModel(AssistantSessionContext context, IEnumerable<Step> steps)
        {
            Context = context;
            Steps = steps;
        }
    }
}
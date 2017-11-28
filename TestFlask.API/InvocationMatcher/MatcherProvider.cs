using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.API.InvocationMatcher
{
    public class MatcherProvider
    {
        private readonly Project project;
        private readonly Scenario scenario;
        private readonly Step step;

        public MatcherProvider(Project project, Scenario scenario, Step step)
        {
            this.project = project;
            this.scenario = scenario;
            this.step = step;
        }

        public Matcher Provide()
        {
            InvocationMatch scenarioMatchStrategy = scenario.InvocationMatchStrategy != InvocationMatch.Inherit 
                ? scenario.InvocationMatchStrategy 
                : project.InvocationMatchStrategy;

            InvocationMatch stepMatchStrategy = step.InvocationMatchStrategy != InvocationMatch.Inherit 
                ? step.InvocationMatchStrategy 
                : scenarioMatchStrategy;

            if (stepMatchStrategy == InvocationMatch.Inherit)
            {
                stepMatchStrategy = InvocationMatch.Exact; //Exact is the default strategy if not set in any level
            }

            switch (stepMatchStrategy)  
            {
                case InvocationMatch.Signature:
                    return new SignatureMatcher(step);
                case InvocationMatch.Request:
                    return new RequestMatcher(step);
                case InvocationMatch.Depth:
                    return new DepthMatcher(step);
                case InvocationMatch.Sibling:
                    return new SiblingMatcher(step);
                case InvocationMatch.Inherit:
                case InvocationMatch.Exact:
                default:
                    return new Matcher(step);
            }
        }
    }
}
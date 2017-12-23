using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.Aspects.InvocationMatcher
{
    public static class MatcherFactory
    {
        public static Matcher CreateMatcher(Step step)
        {
            switch (step.LoadedMatchStrategy)  
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
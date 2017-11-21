using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.API.InvocationMatcher
{
    public class MatcherStrategyFactory
    {
        private readonly Scenario scenario;
        private readonly Step step;

        public MatcherStrategyFactory(Scenario scenario, Step step)
        {
            this.scenario = scenario;
            this.step = step;
        }
        public MatcherStrategy ProvideStrategy()
        {
            var stepMatchStrategy = step.InvocationMatchStrategy != Models.Enums.InvocationMatch.Inherit ? step.InvocationMatchStrategy : scenario.InvocationMatchStrategy;

            if (stepMatchStrategy == InvocationMatch.Inherit)
            {
                stepMatchStrategy = InvocationMatch.Exact; //Exact is the default strategy if not set in any level
            }

            switch (stepMatchStrategy)
            {
                case InvocationMatch.Signature:
                    return new SignatureMatcherStrategy(step);
                case InvocationMatch.Request:
                    return new RequestMatcherStrategy(step);
                case InvocationMatch.Depth:
                    return new DepthMatcherStrategy(step);
                case InvocationMatch.Sibling:
                    return new SiblingMatcherStrategy(step);
                case InvocationMatch.Inherit:
                case InvocationMatch.Exact:
                default:
                    return new MatcherStrategy(step);
            }
        }
    }

    public class MatcherStrategy
    {
        private readonly Step step;

        protected Dictionary<string, Invocation> matches;

        public MatcherStrategy(Step step)
        {
            this.step = step;
        }

        public void Match()
        {
            if (KeyFunc != null)
            {
                var ordered = step.Invocations.Where(i => i.Depth > 1).OrderBy(i => i.Depth).ThenBy(i => i.InvocationIndex);

                foreach (var invocation in ordered)
                {
                    string key = KeyFunc(invocation);

                    if (!matches.ContainsKey(key))
                    {
                        matches.Add(key, invocation);
                    }
                    else
                    {
                        var matched = matches[key];
                        invocation.Response = matched.Response;
                    }
                }
            }
        }

        protected virtual Func<Invocation, string> KeyFunc => null;
    }

    public class SignatureMatcherStrategy : MatcherStrategy
    {
        public SignatureMatcherStrategy(Step step) : base(step) { }

        protected override Func<Invocation, string> KeyFunc => (inv) => inv.SignatureHashCode;

    }

    public class RequestMatcherStrategy : MatcherStrategy
    {
        public RequestMatcherStrategy(Step step) : base(step) { }

        protected override Func<Invocation, string> KeyFunc => (inv) => inv.RequestHashCode;
    }

    public class DepthMatcherStrategy : MatcherStrategy
    {
        public DepthMatcherStrategy(Step step) : base(step) { }

        protected override Func<Invocation, string> KeyFunc => (inv) => inv.DeepHashCode;
    }

    public class SiblingMatcherStrategy : MatcherStrategy
    {
        public SiblingMatcherStrategy(Step step) : base(step) { }


        protected override Func<Invocation, string> KeyFunc => (inv) => inv.LeafHashCode;
    }
}
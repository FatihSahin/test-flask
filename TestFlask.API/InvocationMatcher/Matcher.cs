using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.API.InvocationMatcher
{
    public class Matcher
    {
        private readonly Step step;

        protected Dictionary<string, Invocation> matches;

        public Matcher(Step step)
        {
            this.step = step;
            matches = new Dictionary<string, Invocation>();
        }

        public void Match()
        {
            if (KeyFunc != null)
            {
                var ordered = step.Invocations.Where(i => i.Depth > 1).OrderBy(i => i.Depth).ThenBy(i => i.InvocationIndex).ThenBy(i => i.RecordedOn);

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
}
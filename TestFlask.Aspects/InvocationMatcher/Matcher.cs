using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.Aspects.InvocationMatcher
{
    public class Matcher
    {
        protected Dictionary<string, string> matches;

        public Matcher(Step step)
        {
            GenerateMatches(step);
        }

        private void GenerateMatches(Step step)
        {
            matches = new Dictionary<string, string>();
            
            foreach (var invocation in step.Invocations)
            {
                string key = KeyFunc(invocation);

                if (!matches.ContainsKey(key)) 
                {
                    matches.Add(key, invocation.InstanceHashCode);
                }
            }
        }

        public string Match(Invocation requestedInvocation)
        {
            string key = KeyFunc(requestedInvocation);

            if (matches.ContainsKey(key))
            {
                return matches[key];
            }

            return null;
        }

        protected virtual Func<Invocation, string> KeyFunc => null;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Models.Entity;

namespace TestFlask.API.InvocationMatcher
{
    public class SiblingMatcher : Matcher
    {
        public SiblingMatcher(Step step) : base(step) { }

        protected override Func<Invocation, string> KeyFunc => (inv) => inv.LeafHashCode;
    }
}
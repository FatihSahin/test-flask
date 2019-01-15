using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestFlask.Models.Entity;

namespace TestFlask.Aspects.InvocationMatcher
{
    public class SignatureMatcher : Matcher
    {
        public SignatureMatcher(Step step) : base(step) { }

        protected override Func<Invocation, string> KeyFunc => (inv) => inv.SignatureHashCode;

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Models.Entity;

namespace TestFlask.Aspects.Context
{
    public class InvocationStack : Stack<Invocation>
    {
        public InvocationStack()
        {
        }

        public InvocationStack(IEnumerable<Invocation> collection) : base(collection)
        {
        }

        public InvocationStack(int capacity) : base(capacity)
        {
        }

        public IEnumerable<Invocation> ExceptionStack
        {
            get
            {
                return this.Where(i => i.IsFaulted);
            }
        }
    }
}

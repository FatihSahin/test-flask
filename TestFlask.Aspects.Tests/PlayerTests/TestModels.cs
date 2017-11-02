using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Aspects.Identifiers;

namespace TestFlask.Aspects.Tests.PlayerTests
{
    public class Foo
    {
        public int FooId { get; set; }
        public string Name { get; set; }
    }

    public class FooIdIdentifier : IRequestIdentifier<int>
    {
        public string ResolveDisplayInfo(int req)
        {
            return $"FooID: {req}";
        }

        public string ResolveIdentifierKey(int req)
        {
            return req.ToString();
        }
    }

    public class FooResponseIdentifier : IResponseIdentifier<Foo>
    {
        public string ResolveDisplayInfo(Foo res)
        {
            return $"Foo: {res.Name}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Models.Enums
{
    public enum InvocationMatch
    {
        Inherit, //inherits strategy from parent object, this is default
        Signature, //matches invocation by method signature
        Request, //matches invocation by method signature + request identifier key
        Depth, //matches invocation using deep hash code
        Sibling, //matches invocation with same parent using leaf hash code
        Exact, //matches exact invocation by using invocation instance hash code
    }
}

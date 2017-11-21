using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Models.Enums
{
    public enum InvocationMatch
    {
        Inherit = 0, //inherits strategy from parent object, this is default
        Signature = 10, //matches invocation by method signature
        Request = 20, //matches invocation by method signature + request identifier key
        Depth = 30, //matches invocation using deep hash code
        Sibling = 40, //matches invocation with same parent using leaf hash code
        Exact = 50, //matches exact invocation by using invocation instance hash code
    }
}

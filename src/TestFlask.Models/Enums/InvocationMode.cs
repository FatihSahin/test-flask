using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Models.Enums
{
    /// <summary>
    /// Last invocation mode for persisted invocation
    /// </summary>
    public enum InvocationMode
    {
        Call, //It means that invocation is generated for recording or without mocking
        Replay //It means that invocation is generated for replay (play, assert or intellirecord)
    }
}

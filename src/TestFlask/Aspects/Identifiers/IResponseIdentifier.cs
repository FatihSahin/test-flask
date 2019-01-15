using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFlask.Aspects.Identifiers
{
    /// <summary>
    /// The clasees that implement this interface will supply idenitification display data to recorder aspect.
    /// </summary>
    /// <typeparam name="TRes"></typeparam>
    public interface IResponseIdentifier<TRes>
    {
        /// <summary>
        /// Supplies display details for current method invocation response to feed proper UI info.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        string ResolveDisplayInfo(TRes res);
    }
}

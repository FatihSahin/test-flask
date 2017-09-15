using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFlask.Aspects.Identifiers
{
    //Marker interface
    public interface IRequestIdentifier { }

    /// <summary>
    /// The clasees that implement this interface will supply idenitification data to recorder aspect in order to override or create a method recording.
    /// </summary>
    /// <typeparam name="TReq"></typeparam>
    public interface IRequestIdentifier<TReq> : IRequestIdentifier
    {
        /// <summary>
        /// Supplies a unique indentifier for the request object. Can also be thought as an idempotency key.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        string ResolveIdentifierKey(TReq req);

        /// <summary>
        /// Supplies display details for current method invocation request to feed proper UI info.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        string ResolveDisplayInfo(TReq req);
    }

    public interface IRequestIdentifier<TArg0, TArg1> : IRequestIdentifier
    {
        string ResolveIdentifierKey(TArg0 arg0, TArg1 arg1);
        string ResolveDisplayInfo(TArg0 arg0, TArg1 arg1);
    }

    public interface IRequestIdentifier<TArg0, TArg1, TArg2> : IRequestIdentifier
    {
        string ResolveIdentifierKey(TArg0 arg0, TArg1 arg1, TArg2 arg2);
        string ResolveDisplayInfo(TArg0 arg0, TArg1 arg1, TArg2 arg2);
    }

    public interface IRequestIdentifier<TArg0, TArg1, TArg2, TArg3> : IRequestIdentifier
    {
        string ResolveIdentifierKey(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3);
        string ResolveDisplayInfo(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public interface IRequestIdentifier<TArg0, TArg1, TArg2, TArg3, TArg4> : IRequestIdentifier
    {
        string ResolveIdentifierKey(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
        string ResolveDisplayInfo(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
    }
}

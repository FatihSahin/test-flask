using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Aspects.Enums;
using TestFlask.Aspects.Identifiers;

namespace TestFlask.Aspects.Player
{
    public class FuncPlayerBase<TRes>
    {
        protected InnerFuncPlayer<TRes> innerPlayer;
    }

    //A player for a method with no args, returns response
    public class FuncPlayer<TRes> : FuncPlayerBase<TRes>
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier requestIdentifier;
        private readonly IResponseIdentifier<TRes> responseIdentifer;

        public FuncPlayer(string pMethodSignature, IRequestIdentifier reqIdentifer = null, IResponseIdentifier<TRes> resIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
            responseIdentifer = resIdentifer;
        }

        public void StartInvocation()
        {
            innerPlayer = new InnerFuncPlayer<TRes>(methodSignature,
                null,
                null,
                responseIdentifer);

            innerPlayer.StartInvocation();
        }

        public TestModes DetermineTestMode()
        {
            return innerPlayer.DetermineTestMode();
        }

        public TRes Play()
        {
            return innerPlayer.Play();
        }

        public TRes Record(Func<TRes> originalMethod)
        {
            return innerPlayer.Record(originalMethod.Target, originalMethod.Method);
        }

        public TRes CallOriginal(Func<TRes> originalMethod)
        {
            return innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method);
        }
    }

    //TODO: A more elegant (params object[]) solution seems possible
    public class FuncPlayer<TReq, TRes> : FuncPlayerBase<TRes>
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier<TReq> requestIdentifier;
        private readonly IResponseIdentifier<TRes> responseIdentifer;

        public FuncPlayer(string pMethodSignature, IRequestIdentifier<TReq> reqIdentifer = null, IResponseIdentifier<TRes> resIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
            responseIdentifer = resIdentifer;
        }

        public void StartInvocation(TReq request)
        {
            innerPlayer = new InnerFuncPlayer<TRes>(methodSignature,
                requestIdentifier?.ResolveIdentifierKey(request),
                requestIdentifier?.ResolveDisplayInfo(request),
                responseIdentifer);

            innerPlayer.StartInvocation(request);
        }

        public TestModes DetermineTestMode(TReq request)
        {
            return innerPlayer.DetermineTestMode(request);
        }

        public TRes Play(TReq request)
        {
            return innerPlayer.Play(request);
        }

        public TRes Record(TReq req, Func<TReq, TRes> originalMethod)
        {
            return innerPlayer.Record(originalMethod.Target, originalMethod.Method, req);
        }

        public TRes CallOriginal(TReq req, Func<TReq, TRes> originalMethod)
        {
            return innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method, req);
        }
    }

    public class FuncPlayer<TArg0, TArg1, TRes> : FuncPlayerBase<TRes>
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier<TArg0, TArg1> requestIdentifier;
        private readonly IResponseIdentifier<TRes> responseIdentifer;

        public FuncPlayer(string pMethodSignature, IRequestIdentifier<TArg0, TArg1> reqIdentifer = null, IResponseIdentifier<TRes> resIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
            responseIdentifer = resIdentifer;
        }

        public void StartInvocation(TArg0 reqArg0, TArg1 reqArg1)
        {
            innerPlayer = new InnerFuncPlayer<TRes>(methodSignature,
                requestIdentifier?.ResolveIdentifierKey(reqArg0, reqArg1),
                requestIdentifier?.ResolveDisplayInfo(reqArg0, reqArg1),
                responseIdentifer);

            innerPlayer.StartInvocation(reqArg0, reqArg1);
        }

        public TestModes DetermineTestMode(TArg0 reqArg0, TArg1 reqArg1)
        {
            return innerPlayer.DetermineTestMode(reqArg0, reqArg1);
        }

        public TRes Play(TArg0 reqArg0, TArg1 reqArg1)
        {
            return innerPlayer.Play(reqArg0, reqArg1);
        }

        public TRes Record(TArg0 reqArg0, TArg1 reqArg1, Func<TArg0, TArg1, TRes> originalMethod)
        {
            return innerPlayer.Record(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1); 
        }

        public TRes CallOriginal(TArg0 reqArg0, TArg1 reqArg1, Func<TArg0, TArg1, TRes> originalMethod)
        {
            return innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1);
        }
    }

    public class FuncPlayer<TArg0, TArg1, TArg2, TRes> : FuncPlayerBase<TRes>
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier<TArg0, TArg1, TArg2> requestIdentifier;
        private readonly IResponseIdentifier<TRes> responseIdentifer;

        public FuncPlayer(string pMethodSignature, IRequestIdentifier<TArg0, TArg1, TArg2> reqIdentifer = null, IResponseIdentifier<TRes> resIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
            responseIdentifer = resIdentifer;
        }

        public void StartInvocation(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2)
        {
            innerPlayer = new InnerFuncPlayer<TRes>(methodSignature,
                requestIdentifier?.ResolveIdentifierKey(reqArg0, reqArg1, reqArg2),
                requestIdentifier?.ResolveDisplayInfo(reqArg0, reqArg1, reqArg2),
                responseIdentifer);

            innerPlayer.StartInvocation(reqArg0, reqArg1, reqArg2);
        }

        public TestModes DetermineTestMode(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2)
        {
            return innerPlayer.DetermineTestMode(reqArg0, reqArg1, reqArg2);
        }

        public TRes Play(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2)
        {
            return innerPlayer.Play(reqArg0, reqArg1, reqArg2);
        }

        public TRes Record(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, Func<TArg0, TArg1, TArg2, TRes> originalMethod)
        {
            return innerPlayer.Record(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2);
        }

        public TRes CallOriginal(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, Func<TArg0, TArg1, TArg2, TRes> originalMethod)
        {
            return innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2);
        }
    }

    public class FuncPlayer<TArg0, TArg1, TArg2, TArg3, TRes> : FuncPlayerBase<TRes>
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier<TArg0, TArg1, TArg2, TArg3> requestIdentifier;
        private readonly IResponseIdentifier<TRes> responseIdentifer;

        public FuncPlayer(string pMethodSignature, IRequestIdentifier<TArg0, TArg1, TArg2, TArg3> reqIdentifer = null, IResponseIdentifier<TRes> resIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
            responseIdentifer = resIdentifer;
        }

        public void StartInvocation(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3)
        {
            innerPlayer = new InnerFuncPlayer<TRes>(methodSignature,
               requestIdentifier?.ResolveIdentifierKey(reqArg0, reqArg1, reqArg2, reqArg3),
               requestIdentifier?.ResolveDisplayInfo(reqArg0, reqArg1, reqArg2, reqArg3),
               responseIdentifer);

            innerPlayer.StartInvocation(reqArg0, reqArg1, reqArg2, reqArg3);
        }

        public TestModes DetermineTestMode(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3)
        {
            return innerPlayer.DetermineTestMode(reqArg0, reqArg1, reqArg2, reqArg3);
        }

        public TRes Play(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3)
        {
            return innerPlayer.Play(reqArg0, reqArg1, reqArg2, reqArg3);
        }

        public TRes Record(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, Func<TArg0, TArg1, TArg2, TArg3, TRes> originalMethod)
        {
            return innerPlayer.Record(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2, reqArg3);
        }

        public TRes CallOriginal(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, Func<TArg0, TArg1, TArg2, TArg3, TRes> originalMethod)
        {
            return innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2, reqArg3);
        }
    }

    public class FuncPlayer<TArg0, TArg1, TArg2, TArg3, TArg4, TRes> : FuncPlayerBase<TRes>
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier<TArg0, TArg1, TArg2, TArg3, TArg4> requestIdentifier;
        private readonly IResponseIdentifier<TRes> responseIdentifer;

        public FuncPlayer(string pMethodSignature, IRequestIdentifier<TArg0, TArg1, TArg2, TArg3, TArg4> reqIdentifer = null, IResponseIdentifier<TRes> resIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
            responseIdentifer = resIdentifer;
        }

        public void StartInvocation(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, TArg4 reqArg4)
        {
            innerPlayer = new InnerFuncPlayer<TRes>(methodSignature,
               requestIdentifier?.ResolveIdentifierKey(reqArg0, reqArg1, reqArg2, reqArg3, reqArg4),
               requestIdentifier?.ResolveDisplayInfo(reqArg0, reqArg1, reqArg2, reqArg3, reqArg4),
               responseIdentifer);

            innerPlayer.StartInvocation(reqArg0, reqArg1, reqArg2, reqArg3, reqArg4);
        }

        public TestModes DetermineTestMode(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, TArg4 reqArg4)
        {
            return innerPlayer.DetermineTestMode(reqArg0, reqArg1, reqArg2, reqArg3, reqArg4);
        }

        public TRes Play(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, TArg4 reqArg4)
        {
            return innerPlayer.Play(reqArg0, reqArg1, reqArg2, reqArg3, reqArg4);
        }

        public TRes Record(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, TArg4 reqArg4, Func<TArg0, TArg1, TArg2, TArg3, TArg4, TRes> originalMethod)
        {
            return innerPlayer.Record(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2, reqArg3, reqArg4);
        }

        public TRes CallOriginal(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, TArg4 reqArg4, Func<TArg0, TArg1, TArg2, TArg3, TArg4, TRes> originalMethod)
        {
            return innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2, reqArg3, reqArg4);
        }
    }
}

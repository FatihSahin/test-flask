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
    public class ActionPlayerBase
    {
        protected InnerActionPlayer innerPlayer;
    }

    //A player for a method with no args, no response
    public class ActionPlayer : ActionPlayerBase
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier requestIdentifier;

        public ActionPlayer(string pMethodSignature, IRequestIdentifier reqIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
        }

        public void StartInvocation()
        {
            innerPlayer = new InnerActionPlayer(methodSignature,
                null,
                null);

            innerPlayer.StartInvocation();
        }

        public TestModes DetermineTestMode()
        {
            return innerPlayer.DetermineTestMode();
        }

        public void Play()
        {
            innerPlayer.Play();
        }

        public void Record(Action originalMethod)
        {
            innerPlayer.Record(originalMethod.Target, originalMethod.Method);
        }

        public void CallOriginal(Action originalMethod)
        {
            innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method);
        }
    }

    //TODO: A more elegant (params object[]) solution seems possible
    public class ActionPlayer<TReq> : ActionPlayerBase
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier<TReq> requestIdentifier;

        public ActionPlayer(string pMethodSignature, IRequestIdentifier<TReq> reqIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
        }

        public void StartInvocation(TReq request)
        {
            innerPlayer = new InnerActionPlayer(methodSignature,
                requestIdentifier?.ResolveIdentifierKey(request),
                requestIdentifier?.ResolveDisplayInfo(request));

            innerPlayer.StartInvocation(request);
        }

        public TestModes DetermineTestMode(TReq request)
        {
            return innerPlayer.DetermineTestMode(request);
        }

        public void Play(TReq request)
        {
            innerPlayer.Play(request);
        }

        public void Record(TReq req, Action<TReq> originalMethod)
        {
            innerPlayer.Record(originalMethod.Target, originalMethod.Method, req);
        }

        public void CallOriginal(TReq req, Action<TReq> originalMethod)
        {
            innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method, req);
        }
    }

    public class ActionPlayer<TArg0, TArg1> : ActionPlayerBase
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier<TArg0, TArg1> requestIdentifier;

        public ActionPlayer(string pMethodSignature, IRequestIdentifier<TArg0, TArg1> reqIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
        }

        public void StartInvocation(TArg0 reqArg0, TArg1 reqArg1)
        {
            innerPlayer = new InnerActionPlayer(methodSignature,
                requestIdentifier?.ResolveIdentifierKey(reqArg0, reqArg1),
                requestIdentifier?.ResolveDisplayInfo(reqArg0, reqArg1));

            innerPlayer.StartInvocation(reqArg0, reqArg1);
        }

        public TestModes DetermineTestMode(TArg0 reqArg0, TArg1 reqArg1)
        {
            return innerPlayer.DetermineTestMode(reqArg0, reqArg1);
        }

        public void Play(TArg0 reqArg0, TArg1 reqArg1)
        {
            innerPlayer.Play(reqArg0, reqArg1);
        }

        public void Record(TArg0 reqArg0, TArg1 reqArg1, Action<TArg0, TArg1> originalMethod)
        {
            innerPlayer.Record(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1);
        }

        public void CallOriginal(TArg0 reqArg0, TArg1 reqArg1, Action<TArg0, TArg1> originalMethod)
        {
            innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1);
        }
    }

    public class ActionPlayer<TArg0, TArg1, TArg2> : ActionPlayerBase
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier<TArg0, TArg1, TArg2> requestIdentifier;

        public ActionPlayer(string pMethodSignature, IRequestIdentifier<TArg0, TArg1, TArg2> reqIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
        }

        public void StartInvocation(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2)
        {
            innerPlayer = new InnerActionPlayer(methodSignature,
                requestIdentifier?.ResolveIdentifierKey(reqArg0, reqArg1, reqArg2),
                requestIdentifier?.ResolveDisplayInfo(reqArg0, reqArg1, reqArg2));

            innerPlayer.StartInvocation(reqArg0, reqArg1, reqArg2);
        }

        public TestModes DetermineTestMode(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2)
        {
            return innerPlayer.DetermineTestMode(reqArg0, reqArg1, reqArg2);
        }

        public void Play(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2)
        {
            innerPlayer.Play(reqArg0, reqArg1, reqArg2);
        }

        public void Record(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, Action<TArg0, TArg1, TArg2> originalMethod)
        {
            innerPlayer.Record(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2);
        }

        public void CallOriginal(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, Action<TArg0, TArg1, TArg2> originalMethod)
        {
            innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2);
        }
    }

    public class ActionPlayer<TArg0, TArg1, TArg2, TArg3> : ActionPlayerBase
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier<TArg0, TArg1, TArg2, TArg3> requestIdentifier;

        public ActionPlayer(string pMethodSignature, IRequestIdentifier<TArg0, TArg1, TArg2, TArg3> reqIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
        }

        public void StartInvocation(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3)
        {
            innerPlayer = new InnerActionPlayer(methodSignature,
                requestIdentifier?.ResolveIdentifierKey(reqArg0, reqArg1, reqArg2, reqArg3),
                requestIdentifier?.ResolveDisplayInfo(reqArg0, reqArg1, reqArg2, reqArg3));

            innerPlayer.StartInvocation(reqArg0, reqArg1, reqArg2, reqArg3);
        }

        public TestModes DetermineTestMode(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3)
        {
            return innerPlayer.DetermineTestMode(reqArg0, reqArg1, reqArg2, reqArg3);
        }

        public void Play(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3)
        {
            innerPlayer.Play(reqArg0, reqArg1, reqArg2, reqArg3);
        }

        public void Record(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, Action<TArg0, TArg1, TArg2, TArg3> originalMethod)
        {
            innerPlayer.Record(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2, reqArg3);
        }

        public void CallOriginal(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, Action<TArg0, TArg1, TArg2, TArg3> originalMethod)
        {
            innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2, reqArg3);
        }
    }

    public class ActionPlayer<TArg0, TArg1, TArg2, TArg3, TArg4> : ActionPlayerBase
    {
        private readonly string methodSignature;
        private readonly IRequestIdentifier<TArg0, TArg1, TArg2, TArg3, TArg4> requestIdentifier;

        public ActionPlayer(string pMethodSignature, IRequestIdentifier<TArg0, TArg1, TArg2, TArg3, TArg4> reqIdentifer = null)
        {
            methodSignature = pMethodSignature;
            requestIdentifier = reqIdentifer;
        }

        public void StartInvocation(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, TArg4 reqArg4)
        {
            innerPlayer = new InnerActionPlayer(methodSignature,
                requestIdentifier?.ResolveIdentifierKey(reqArg0, reqArg1, reqArg2, reqArg3, reqArg4),
                requestIdentifier?.ResolveDisplayInfo(reqArg0, reqArg1, reqArg2, reqArg3, reqArg4));

            innerPlayer.StartInvocation(reqArg0, reqArg1, reqArg2, reqArg3, reqArg4);
        }

        public TestModes DetermineTestMode(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, TArg4 reqArg4)
        {
            return innerPlayer.DetermineTestMode(reqArg0, reqArg1, reqArg2, reqArg3, reqArg4);
        }

        public void Play(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, TArg4 reqArg4)
        {
            innerPlayer.Play(reqArg0, reqArg1, reqArg2, reqArg3, reqArg4);
        }

        public void Record(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, TArg4 reqArg4, Action<TArg0, TArg1, TArg2, TArg3, TArg4> originalMethod)
        {
            innerPlayer.Record(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2, reqArg3, reqArg4);
        }

        public void CallOriginal(TArg0 reqArg0, TArg1 reqArg1, TArg2 reqArg2, TArg3 reqArg3, TArg4 reqArg4, Action<TArg0, TArg1, TArg2, TArg3, TArg4> originalMethod)
        {
            innerPlayer.CallOriginal(originalMethod.Target, originalMethod.Method, reqArg0, reqArg1, reqArg2, reqArg3, reqArg4);
        }
    }
}

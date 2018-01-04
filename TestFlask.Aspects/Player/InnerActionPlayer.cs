using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TestFlask.Aspects.ApiClient;
using TestFlask.Aspects.Context;
using TestFlask.Aspects.Enums;
using TestFlask.Aspects.Identifiers;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.Aspects.Player
{
    public class InnerActionPlayer : InnerPlayerBase
    {
        public InnerActionPlayer(string pMethodSignature, string pRequestIdentifierKey, string pRequestDisplayInfo) : base(pMethodSignature, pRequestIdentifierKey, pRequestDisplayInfo)
        {
        }

        public void CallOriginal(object target, MethodInfo originalMethodInfo, params object[] requestArgs)
        {
            Record(target, originalMethodInfo, requestArgs); 
        }

        public void Record(object target, MethodInfo originalMethodInfo, params object[] requestArgs)
        {
            ResolveReflectedInterfaceType(originalMethodInfo);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                originalMethodInfo.Invoke(target, requestArgs);
                SetResponse(InvocationMode.Call, sw.ElapsedMilliseconds);

                EndInvocation();
            }
            catch (Exception ex)
            {
                //outer exception is TargetInvocationException (System.Reflection)
                RecordException(ex.InnerException, sw.ElapsedMilliseconds, requestArgs);
                throw ex.InnerException;
            }
            finally
            {
                sw.Stop();
            }
        }

        private void SetResponse(InvocationMode invocationMode, long callDuration)
        {
            requestedInvocation.Duration = callDuration;
            requestedInvocation.InvocationMode = invocationMode;

            requestedInvocation.ResponseType = "System.Void";

            if (requestedInvocation.Depth == 1)    //root invocation
            {
                requestedInvocation.RequestRaw = TestFlaskContext.RawRequest;
            }
        }

        public void Play(params object[] requestArgs)
        {
            Invocation matchedInvocation = TestFlaskContext.GetMatchedInvocation(requestedInvocation);

            if (!matchedInvocation.IsFaulted)
            {
                SetResponse(InvocationMode.Replay, -1);
                EndInvocation();
            }
            else
            {
                var exception = (Exception)JsonConvert.DeserializeObject(matchedInvocation.Exception, Type.GetType(matchedInvocation.ExceptionType), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                });

                SetException(InvocationMode.Replay, -1L, exception);
                EndInvocation(exception);

                throw exception;
            }
        }
    }
}
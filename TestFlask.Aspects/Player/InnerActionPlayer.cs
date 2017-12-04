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

namespace TestFlask.Aspects.Player
{
    public class InnerActionPlayer : InnerPlayerBase
    {
        public InnerActionPlayer(string pMethodSignature, string pRequestIdentifierKey, string pRequestDisplayInfo) : base(pMethodSignature, pRequestIdentifierKey, pRequestDisplayInfo)
        {
        }

        public void CallOriginal(object target, MethodInfo originalMethodInfo, params object[] requestArgs)
        {
            try
            {
                originalMethodInfo.Invoke(target, requestArgs);
                EndInvocation();
            }
            catch (Exception ex)
            {
                EndInvocation(ex.InnerException); //outer exception is TargetInvocationException (System.Reflection)
                throw ex.InnerException;
            }
        }

        public void Record(object target, MethodInfo originalMethodInfo, params object[] requestArgs)
        {
            ResolveReflectedInterfaceType(originalMethodInfo);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                originalMethodInfo.Invoke(target, requestArgs);
                requestedInvocation.Duration = sw.ElapsedMilliseconds;

                requestedInvocation.ResponseType = "System.Void";

                ResolveReflectedInterfaceType(originalMethodInfo);

                if (requestedInvocation.Depth == 1)    //root invocation
                {
                    requestedInvocation.RequestRaw = TestFlaskContext.RawRequest;
                }

                TryPersistStepInvocations();

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

        public void Play(params object[] requestArgs)
        {
            var loadedInvocation = TestFlaskContext.GetLoadedInvocation(requestedInvocation.InstanceHashCode);

            if (!loadedInvocation.IsFaulted)
            {
                EndInvocation();
            }
            else
            {
                var exception = (Exception)JsonConvert.DeserializeObject(loadedInvocation.Exception, Type.GetType(loadedInvocation.ExceptionType), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                });
                EndInvocation(exception);
                throw exception;
            }
        }
    }
}
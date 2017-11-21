using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public class InnerFuncPlayer<TRes> : InnerPlayerBase
    {
        private readonly IResponseIdentifier<TRes> responseIdentifier;

        public InnerFuncPlayer(string pMethodSignature, string pRequestIdentifierKey, string pRequestDisplayInfo, IResponseIdentifier<TRes> pResponseIdentifier) 
            : base(pMethodSignature, pRequestIdentifierKey, pRequestDisplayInfo)
        {
            responseIdentifier = pResponseIdentifier;
        }

        public TRes CallOriginal(object target, MethodInfo originalMethodInfo, params object[] requestArgs)
        {
            try
            {
                TRes response = (TRes)(originalMethodInfo.Invoke(target, requestArgs));
                EndInvocation(response);
                return response;
            }
            catch (Exception ex)
            {
                EndInvocation(ex.InnerException); //outer exception is TargetInvocationException (System.Reflection)
                throw ex.InnerException;
            }
        }

        public TRes Record(object target, MethodInfo originalMethodInfo, params object[] requestArgs)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                TRes response = (TRes)(originalMethodInfo.Invoke(target, requestArgs));
                requestedInvocation.Duration = sw.ElapsedMilliseconds;

                requestedInvocation.ResponseDisplayInfo = responseIdentifier?.ResolveDisplayInfo(response);
                requestedInvocation.Response = JsonConvert.SerializeObject(response);

                //if response is not null, use its type (as it may me a derived type, if null we have no choice to use declared generic type
                //Does not support proxified entities
                var responseType = response != null ? response.GetType() : typeof(TRes); 

                var regex = new Regex(@"(, PublicKeyToken=(null|\w{16}))|(, Version=[^,]+)|(, Culture=[^,]+)");

                requestedInvocation.ResponseType = regex.Replace(responseType.AssemblyQualifiedName, string.Empty); //save without version, public key token, culture
                
                if (requestedInvocation.Depth == 1)    //root invocation
                {
                    requestedInvocation.RequestRaw = TestFlaskContext.RawRequest;
                }

                TryPersistStepInvocations();

                EndInvocation(response);

                return response;
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

        public TRes Play(params object[] requestArgs)
        {
            var loadedInvocation = TestFlaskContext.GetLoadedInvocation(requestedInvocation.InstanceHashCode);

            if (!loadedInvocation.IsFaulted)
            {
                var response = (TRes)JsonConvert.DeserializeObject(loadedInvocation.Response, Type.GetType(loadedInvocation.ResponseType));
                EndInvocation(response);
                return response;
            }
            else
            {
                var exception = (Exception)JsonConvert.DeserializeObject(loadedInvocation.Exception, Type.GetType(loadedInvocation.ExceptionType));
                EndInvocation(exception);
                throw exception;
            }
        }
    }
}
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
            ResolveReflectedInterfaceType(originalMethodInfo);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                TRes response = (TRes)(originalMethodInfo.Invoke(target, requestArgs));
                requestedInvocation.Duration = sw.ElapsedMilliseconds;

                //if response is not null, use its type (as it may me a derived type, if null we have no choice to use declared generic type
                //Does not support proxified entities
                var responseType = response != null ? response.GetType() : typeof(TRes);

                requestedInvocation.ResponseDisplayInfo = responseIdentifier?.ResolveDisplayInfo(response);
                requestedInvocation.Response = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                });
                
                requestedInvocation.ResponseType = typeNameSimplifierRegex.Replace(responseType.AssemblyQualifiedName, string.Empty); //save without version, public key token, culture

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
            Invocation matchedInvocation = TestFlaskContext.GetMatchedInvocation(requestedInvocation);

            if (!matchedInvocation.IsFaulted)
            {
                var response = (TRes)JsonConvert.DeserializeObject(matchedInvocation.Response, Type.GetType(matchedInvocation.ResponseType), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                });
                EndInvocation(response);
                return response;
            }
            else
            {
                var exception = (Exception)JsonConvert.DeserializeObject(matchedInvocation.Exception, Type.GetType(matchedInvocation.ExceptionType), new JsonSerializerSettings
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
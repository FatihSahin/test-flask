using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFlask.Models.Entity
{
    public class Invocation 
    {

        //immutable
        public string ProjectKey { get; set; }

        //immutable
        public long ScenarioNo { get; set; }

        //Used as a FK, immutable
        public long StepNo { get; set; }

        public int Depth { get; set; }

        public long Duration { get; set; }

        //an array of request invocation argument objects (sequentially synced with invocation signature)
        public string Request { get; set; }

        //raw network request (for root invocation, otherwise empty)
        public string RequestRaw { get; set; }

        public string Response { get; set; }

        public string AssertionResult { get; set; }

        public string ResponseType { get; set; }

        public bool IsFaulted { get; set; }

        public string ExceptionType { get; set; }

        public string Exception { get; set; }

        public string InvocationSignature { get; set; }

        public string RequestIdentifierKey { get; set; }

        public string RequestDisplayInfo { get; set; }

        public string ResponseDisplayInfo { get; set; }

        public bool IsReplayable { get; set; }

        public int InvocationIndex { get; set; }

        public string HashCode { get; set; }

        public string DeepHashCode { get; set; }

        public string InstanceHashCode { get; set; }

        public string ParentInstanceHashCode { get; set; }

        public string GetInvocationHashCode()
        {
            string signatureHash = InvocationSignature.GetHashCode().ToString();
            string requestIdentifierHash = !string.IsNullOrWhiteSpace(RequestIdentifierKey) ? RequestIdentifierKey.GetHashCode().ToString() : string.Empty;

            StringBuilder strBuilder = new StringBuilder(signatureHash);

            if (!string.IsNullOrWhiteSpace(requestIdentifierHash))
            {
                strBuilder.Append($"-{requestIdentifierHash}");
            }

            return strBuilder.ToString();
        }

        public string GetInvocationDeepHashCode()
        {
            string invocationHashCode = GetInvocationHashCode();
            return $"{StepNo}-{invocationHashCode}-{Depth}";
        }

        public string GetInvocationInstanceHashCode()
        {
            string invocationDeepHashCode = GetInvocationDeepHashCode();
            return $"{ScenarioNo}-{invocationDeepHashCode}-{InvocationIndex}";
        }
    }
}

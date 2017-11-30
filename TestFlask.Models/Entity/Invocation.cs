using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlask.Models.Enums;
using TestFlask.Models.Utils;

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

        public string ReflectedType { get; set; }

        public bool IsFaulted { get; set; }

        public string ExceptionType { get; set; }

        public string Exception { get; set; }

        public string InvocationSignature { get; set; }

        public string RequestIdentifierKey { get; set; }

        public string RequestDisplayInfo { get; set; }

        public string ResponseDisplayInfo { get; set; }

        public bool IsReplayable { get; set; }

        public int InvocationIndex { get; set; }

        public string SignatureHashCode { get; set; }

        public string RequestHashCode { get; set; }

        public string DeepHashCode { get; set; }

        public string LeafHashCode { get; set; }

        public string InstanceHashCode { get; set; }

        public string ParentInstanceHashCode { get; set; }

        public DateTime RecordedOn { get; set; }

        public string GetSignatureHashCode()
        {
            return HashUtil.Crc32Hash(InvocationSignature);
        }

        public string GetRequestHashCode()
        {
            string signatureHash = GetSignatureHashCode(); 
            string requestIdentifierHash = !string.IsNullOrWhiteSpace(RequestIdentifierKey) ? HashUtil.Crc32Hash(RequestIdentifierKey) : "0";

            StringBuilder strBuilder = new StringBuilder(signatureHash);

            strBuilder.Append($"_{requestIdentifierHash}");

            return strBuilder.ToString();
        }

        public string GetDeepHashCode()
        {
            string requestHashCode = GetRequestHashCode();
            return $"{StepNo}_{requestHashCode}_{Depth}";
        }

        public string GetLeafHashCode()
        {
            string deepHashCode = GetDeepHashCode();
            string parentInstanceHashCodeHashCode = HashUtil.Crc32Hash(ParentInstanceHashCode ?? "null");
            return $"{deepHashCode}_{parentInstanceHashCodeHashCode}";
        }

        public string GetInvocationInstanceHashCode()
        {
            string leafHashCode = GetLeafHashCode();
            return $"{ScenarioNo}_{leafHashCode}_{InvocationIndex}";
        }

        /// <summary>
        /// Parses and extracts a dummy invocation instance with proper hash code parsed from instanceHashCode
        /// </summary>
        public static Invocation Parse(string instanceHashCode)
        {
            if (!string.IsNullOrWhiteSpace(instanceHashCode))
            {
                //scenarioNo_stepNo_signaturehash_reqKeyHash_depth_parentInstanceHashHash_invIndex
                string[] hashParts = instanceHashCode.Split('_');

                if (hashParts.Length == 7)
                {
                    Invocation parsed = new Invocation();

                    parsed.ScenarioNo = long.Parse(hashParts[0]);
                    parsed.StepNo = long.Parse(hashParts[1]);
                    parsed.SignatureHashCode = hashParts[2];
                    parsed.RequestHashCode = $"{parsed.SignatureHashCode}_{hashParts[3]}";
                    parsed.Depth = int.Parse(hashParts[4]);
                    parsed.DeepHashCode = $"{parsed.StepNo}_{parsed.RequestHashCode}_{parsed.Depth}";
                    parsed.LeafHashCode = $"{parsed.DeepHashCode}_{hashParts[5]}";
                    parsed.InvocationIndex = int.Parse(hashParts[6]);
                    parsed.InstanceHashCode = instanceHashCode;

                    return parsed;
                }
            }

            return null;
        }
    }
}

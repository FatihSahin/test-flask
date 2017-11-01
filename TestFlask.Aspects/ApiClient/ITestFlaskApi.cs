using System.Collections.Generic;
using TestFlask.Models.Entity;

namespace TestFlask.Aspects.ApiClient
{
    public interface ITestFlaskApi
    {
        void CompleteStepInvocations(Step step);
        void AppendStepInvocations(Step step);
        void DeleteStepInvocations(Step step);
        Dictionary<string, int> GetLeafTable(string contextId);
        Step GetStep(long stepNo);
        void PutInvocation(Invocation invocation);
        Step InsertStep(Step step);
        void PostLeafTable(string contextId, Dictionary<string, int> leafTable);
    }
}
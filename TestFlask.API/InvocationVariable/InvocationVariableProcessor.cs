using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TestFlask.API.Cache;
using TestFlask.Data.Repos;
using TestFlask.Models.Entity;
using TestFlask.Models.Enums;

namespace TestFlask.API.InvocationVariable
{
    public class InvocationVariableProcessor : IInvocationVariableProcessor
    {
        private const string variableRegexPattern = @"{{\s*[\w\.]+\s*}}";

        private const string variableOpeningTag = "{{";

        private const string variableClosingTag = "}}";

        private readonly IVariableRepo variableRepo;

        public InvocationVariableProcessor(IVariableRepo pVariableRepo)
        {
            variableRepo = pVariableRepo;
        }

        public Step VariableToValue(string projectKey, Step step)
        {
            foreach (var invocation in step.Invocations.Where(p => p.Depth == 1))
            {
                var inputVariables = Find(invocation.RequestRaw, variableRegexPattern);

                if (inputVariables != null)
                {
                    foreach (var inputVariable in inputVariables)
                    {
                        string value = GetVariable(projectKey, inputVariable)?.Value;

                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }

                        invocation.RequestRaw = invocation.RequestRaw.Replace(inputVariable, GetVariable(projectKey, inputVariable).Value);
                    }
                }
            }

            return step;
        }

        public Step ValueToVariable(string projectKey, Step step)
        {
            var variables = GetProjectVariables(projectKey).Where(p => !string.IsNullOrEmpty(p.InvocationVariableRegex)).OrderBy(p => p.GetLevel());

            if (variables != null)
            {
                foreach (var variable in variables)
                {
                    foreach (var invocation in step.Invocations.Where(p => p.Depth == 1))
                    {
                        invocation.RequestRaw = Regex.Replace(invocation.RequestRaw, variable.InvocationVariableRegex, $"{variableOpeningTag}{variable.Name}{variableClosingTag}");
                    }
                }
            }
            return step;
        }

        private Variable GetVariable(string projectKey, string name)
        {
            var variables = GetProjectVariables(projectKey);
            var variableName = name.Replace(variableOpeningTag, "").Replace(variableClosingTag, "");
            if (variables != null)
            {
                return variables.Where(p => p.Name == variableName).OrderBy(p => p.GetLevel()).FirstOrDefault();
            }

            return null;
        }

        private List<Variable> GetProjectVariables(string projectKey)
        {
            IList<Variable> variables = ApiCache.GetVariableByProject(projectKey);
            if (variables == null)
            {
                variables = variableRepo.GetByProject(projectKey).Where(p => p.IsEnabled == true).ToList();

                ApiCache.AddVariableByProject(projectKey, variables.ToList());
            }

            return variables.ToList();
        }

        private IList<string> Find(string input, string regexPattern)
        {
            var variables = new List<string>();
            var matches = Regex.Matches(input, regexPattern);

            if (matches == null || matches.Count == 0)
            {
                return null;
            }

            foreach (Match m in matches)
            {
                variables.Add(m.Value);
            }

            return variables;
        }
    }

    public interface IInvocationVariableProcessor
    {
        Step VariableToValue(string projectKey, Step step);
        Step ValueToVariable(string projectKey, Step step);
    }
}
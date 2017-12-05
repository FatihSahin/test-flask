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

        public void ResolveVariables(Step step)
        {
            var invocation = step.Invocations.SingleOrDefault(p => p.Depth == 1);
            var inputVariables = FindVariables(invocation.RequestRaw, variableRegexPattern);

            if (inputVariables != null)
            {
                foreach (var inputVariable in inputVariables)
                {
                    string value = GetVariable(step, inputVariable)?.Value;

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    invocation.RequestRaw = invocation.RequestRaw.Replace(inputVariable, value);
                }
            }
        }

        public void GenerateVariables(Step step)
        {
            var variables = GetStepVariables(step).Where(p => !string.IsNullOrEmpty(p.GeneratorRegex));

            if (variables != null)
            {
                foreach (var variable in variables)
                {
                    var invocation = step.Invocations.SingleOrDefault(p => p.Depth == 1);
                    invocation.RequestRaw = Regex.Replace(invocation.RequestRaw, variable.GeneratorRegex, $"{variableOpeningTag}{variable.Name}{variableClosingTag}");
                }
            }
        }

        private Variable GetVariable(Step step, string name)
        {
            var variables = GetVariableByProject(step.ProjectKey);

            if (variables == null)
            {
                return null;
            }

            name = name.Replace(variableOpeningTag, "").Replace(variableClosingTag, "");
            string variableKey = Variable.CreateKey(step.ProjectKey, step.ScenarioNo, step.StepNo, name);

            var variable = variables.SingleOrDefault(p => p.GetKey() == variableKey);
            if (variable == null)
            {
                variableKey = Variable.CreateKey(step.ProjectKey, step.ScenarioNo, 0, name);
                variable = variables.SingleOrDefault(p => p.GetKey() == variableKey);
                if (variable == null)
                {
                    variableKey = Variable.CreateKey(step.ProjectKey, 0, 0, name);
                    variable = variables.SingleOrDefault(p => p.GetKey() == variableKey);
                }
            }

            return variable;
        }

        private IList<Variable> GetStepVariables(Step step)
        {
            var variables = GetVariableByProject(step.ProjectKey);

            if (variables == null)
            {
                return null;
            }

            var result = new List<Variable>();

            string variableKey = Variable.CreateKey(step.ProjectKey, step.ScenarioNo, step.StepNo, "");
            result.AddRange(variables.Where(p => p.GetStepKey() == variableKey));

            variableKey = Variable.CreateKey(step.ProjectKey, step.ScenarioNo, 0, "");
            result.AddRange(variables.Where(p => p.GetStepKey() == variableKey));

            variableKey = Variable.CreateKey(step.ProjectKey, 0, 0, "");
            result.AddRange(variables.Where(p => p.GetStepKey() == variableKey));

            return result;
        }

        private IList<Variable> GetVariableByProject(string projectKey)
        {
            var variables = ApiCache.GetVariableByProject(projectKey);
            if (variables == null)
            {
                variables = variableRepo.GetByProject(projectKey).Where(p => p.IsEnabled == true).ToList();

                ApiCache.AddVariableByProject(projectKey, variables.ToList());
            }

            return variables;
        }


        private IList<string> FindVariables(string input, string regexPattern)
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
        void ResolveVariables(Step step);
        void GenerateVariables(Step step);
    }
}
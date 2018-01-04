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
    public interface IInvocationVariableProcessor
    {
        void ResolveVariables(Step step);
        void GenerateVariables(Step step);
    }

    public class InvocationVariableProcessor : IInvocationVariableProcessor
    {
        private const string variableOpeningTag = "{{";
        private const string variableClosingTag = "}}";
        private string variableRegexPattern = $@"{variableOpeningTag}\s*[\w\.]+\s*{variableClosingTag}";

        private readonly IVariableRepo variableRepo;

        public InvocationVariableProcessor(IVariableRepo pVariableRepo)
        {
            variableRepo = pVariableRepo;
        }

        public void ResolveVariables(Step step)
        {
            var invocation = step.GetRootInvocation();

            //can be null on intellirecord mode on cross domain requests.
            //A more complete solution is required to be able to process inner cross requests' variables too.
            if (invocation != null) 
            {
                var inputVariables = FindVariables(invocation.RequestRaw, variableRegexPattern);

                foreach (var inputVariable in inputVariables)
                {
                    string value = GetVariable(step, inputVariable)?.Value;

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        invocation.RequestRaw = invocation.RequestRaw.Replace(inputVariable, value);
                    }
                }
            }
        }

        public void GenerateVariables(Step step)
        {
            var variables = GetStepVariables(step).Where(p => !string.IsNullOrEmpty(p.GeneratorRegex));

            foreach (var variable in variables)
            {
                var rootInvocation = step.GetRootInvocation();
                Regex genRegex = new Regex(variable.GeneratorRegex, RegexOptions.Singleline);

                string replacement = $"{variableOpeningTag}{variable.Name}{variableClosingTag}";

                var groupNames = genRegex.GetGroupNames();
                bool hasContentGroup = groupNames.Contains("wrapStart") && groupNames.Contains("wrapEnd");

                if (hasContentGroup)
                {
                    replacement = $"${{wrapStart}}{replacement}${{wrapEnd}}";
                }

                rootInvocation.RequestRaw = Regex.Replace(rootInvocation.RequestRaw, variable.GeneratorRegex, replacement);
            }
        }

        private Variable GetVariable(Step step, string name)
        {
            var variables = GetVariablesByProject(step.ProjectKey);
            Variable variable = null;

            if (variables.Any())
            {
                name = name.Replace(variableOpeningTag, string.Empty).Replace(variableClosingTag, string.Empty);
                string variableKey = Variable.CreateKey(step.ProjectKey, step.ScenarioNo, step.StepNo, name);

                variable = variables.SingleOrDefault(p => p.GetKey() == variableKey);
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
            }

            return variable;
        }

        private IEnumerable<Variable> GetStepVariables(Step step)
        {
            var variables = GetVariablesByProject(step.ProjectKey);

            var result = new List<Variable>();

            if (variables.Any())
            {
                string variableKey = Variable.CreateKey(step.ProjectKey, step.ScenarioNo, step.StepNo, string.Empty);
                result.AddRange(variables.Where(p => p.GetStepKey() == variableKey));

                variableKey = Variable.CreateKey(step.ProjectKey, step.ScenarioNo, 0, string.Empty);
                result.AddRange(variables.Where(p => p.GetStepKey() == variableKey));

                variableKey = Variable.CreateKey(step.ProjectKey, 0, 0, string.Empty);
                result.AddRange(variables.Where(p => p.GetStepKey() == variableKey));
            }

            return result;
        }

        private IEnumerable<Variable> GetVariablesByProject(string projectKey)
        {
            var variables = ApiCache.GetVariablesByProject(projectKey);
            if (variables == null)
            {
                variables = variableRepo.GetByProject(projectKey).Where(p => p.IsEnabled);

                if (variables.Any())
                {
                    ApiCache.AddVariableByProject(projectKey, variables);
                }
            }

            return variables;
        }


        private IEnumerable<string> FindVariables(string input, string regexPattern)
        {
            var variables = new HashSet<string>();

            var matches = Regex.Matches(input, regexPattern);

            foreach (Match m in matches)
            {
                if (!variables.Contains(m.Value))
                {
                    variables.Add(m.Value);
                }
            }

            return variables;
        }
    }
}
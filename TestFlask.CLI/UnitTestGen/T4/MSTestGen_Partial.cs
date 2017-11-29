using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestFlask.Aspects.ApiClient;
using TestFlask.Aspects.Config;
using TestFlask.CLI.Options;
using TestFlask.Models.Entity;

namespace TestFlask.CLI.UnitTestGen.T4
{
    public partial class MSTestGen
    {
        private readonly UnitTestGenOptions options;
        public TestFlaskApi Api { get; set; }

        public Dictionary<string, string> Subjects { get; set; }

        private static Regex signatureRegex = new Regex(@"(?<responseType>.+)\s.+::(?<methodName>\w+)\((?<requestType>.+)\)", RegexOptions.Compiled);
        private Match signatureMatch;

        public MSTestGen(UnitTestGenOptions pOptions)
        {
            options = pOptions;
            Subjects = new Dictionary<string, string>();

            TestFlaskConfig.Instance = new TestFlaskConfig
            {
                Api = new ApiConfig
                {
                    Url = pOptions.ApiUrl
                },
                Project = new ProjectConfig
                {
                    Key = pOptions.ProjectKey
                }
            };

            Api = new TestFlaskApi();
        }

        private IEnumerable<Scenario> scenarios;
        public IEnumerable<Scenario> Scenarios
        {
            get
            {
                if (scenarios == null)
                {
                    scenarios = GetScenarios();
                }

                return scenarios;
            }
        }

        private IEnumerable<Scenario> GetScenarios()
        {
            return Api.GetScenarios();
        }

        public string GetTypeName(string simpleTypeName)
        {
            return simpleTypeName.Split(',')[0].Trim();
        }

        public Scenario GetScenarioDeep(long scenarioNo)
        {
            Scenario scenario = Api.GetScenarioDeep(scenarioNo); 

            foreach (var step in scenario.Steps)
            {
                var subjectType = GetTypeName(step.RootInvocationReflectedType);
                if (!Subjects.ContainsKey(subjectType))
                {
                    Subjects.Add(subjectType, subjectType.Contains('.') ? subjectType.Split('.').Last() : subjectType);
                }
            }

            return scenario;
        }

        public string GetScenarioTestMethodName(Scenario scenario)
        {
            return $"Scenario{scenario.ScenarioNo}_{scenario.ScenarioName.Replace(" ", string.Empty)}";
        }

        public string GetStepAssertMethodName(Scenario scenario, Step step)
        {
            return $"Scenario{scenario.ScenarioNo}_{scenario.ScenarioName.Replace(" ", string.Empty)}_Step{step.StepNo}_{step.StepName.Replace(" ", string.Empty)}";
        }

        /// <summary>
        /// Gets root invocation method args type names
        /// MSTestGen only supports single arg root invocations in unit test generator for now. Need to enhance this.
        /// However, this method returns all args type names for future development.
        /// </summary>
        public string GetRequestTypeName()
        {
            //return JToken.Parse(rootInvocation.Request).SelectTokens("$.$values[*].$type").Select(t => GetTypeName(t.ToString()));
            return signatureMatch.Groups["requestType"].Value;
        }

        public string GetResponseTypeName()
        {
            return signatureMatch.Groups["responseType"].Value;
        }

        public string GetSubjectValue(Step step)
        {
            return Subjects[GetTypeName(step.RootInvocationReflectedType)];
        }

        public string GetRootMethodName()
        { 
            return signatureMatch.Groups["methodName"].Value;
        }
    }
}

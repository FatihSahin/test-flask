using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Scenario GetScenarioDeep(long scenarioNo)
        {
            Scenario scenario = Api.GetScenarioDeep(scenarioNo);

            foreach (var step in scenario.Steps)
            {
                var subjectType = step.RootInvocationReflectedType.Split(',')[0].Trim();
                if (!Subjects.ContainsKey(subjectType))
                {
                    Subjects.Add(subjectType, subjectType.Contains('.') ? subjectType.Split('.').Last() : subjectType);
                }
            }

            return scenario;
        }
    }
}

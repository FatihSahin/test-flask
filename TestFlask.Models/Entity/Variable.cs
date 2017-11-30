using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Models.Enums;

namespace TestFlask.Models.Entity
{
    public class Variable : MongoEntity
    {
        public string ProjectKey { get; set; }
        public long ScenarioNo { get; set; }
        public long StepNo { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsEnabled { get; set; }
        public string GeneratorRegex { get; set; }

        public string GetKey()
        {
            return CreateKey(ProjectKey, ScenarioNo, StepNo, Name);
        }
        public string GetStepKey()
        {
            return CreateKey(ProjectKey, ScenarioNo, StepNo, "");
        }

        public static string CreateKey(string projectKey, long ScenarioNo, long StepNo, string Name)
        {
            return $"{projectKey}-{ScenarioNo}-{StepNo}-{Name}";
        }
    }
}

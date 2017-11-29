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
        public string InvocationVariableRegex { get; set; }

        public VariableLevel GetLevel()
        {
            VariableLevel level = VariableLevel.Project;

            if (ScenarioNo != 0)
            {
                level = VariableLevel.Scenario;
            }

            if (StepNo != 0)
            {
                level = VariableLevel.Step;
            }

            return level;
        }

        public string GetKey()
        {
            return $"{ProjectKey}-{ScenarioNo}-{StepNo}-{Name}";
        }

    }
}

using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.CLI.Options
{
    [Verb("unit", HelpText = "Generate unit tests")]
    public class UnitTestGenOptions
    {
        [Option('t', "testfw", Default = "mstest", HelpText = "Unit test framework [mstest|nunit]")]
        public string UnitTestFramework { get; set; }

        [Option('a', "api", Required = true, HelpText = "TestFlask api host url that holds scenarios for which unit test will be generated.")]
        public string ApiUrl { get; set; }

        [Option('p', "project", Required = true, HelpText = "TestFlask project key that owns the scenarios")]
        public string ProjectKey { get; set; }

        [Option('f', "filename", Default = "TestFlaskTests_Auto.cs", HelpText= "Output file name")]
        public string FileName { get; set; }
    }
}

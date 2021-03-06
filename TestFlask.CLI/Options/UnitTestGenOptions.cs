﻿using CommandLine;
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
        public string TestFramework { get; set; }

        [Option('a', "api", Required = true, HelpText = "TestFlask api host url that holds scenarios for which unit test will be generated")]
        public string ApiUrl { get; set; }

        [Option('p', "project", Required = true, HelpText = "TestFlask project key that owns the scenarios")]
        public string ProjectKey { get; set; }

        [Option('c', "classname", Default = "TestFlaskTests_Auto.cs", HelpText= "Output class name. Output file name will be [classname]_Auto.cs")]
        public string ClassName { get; set; }

        [Option('n', "namespace", Default = "TestFlaskAutoTests", HelpText="Namespace for your generated test class")]
        public string Namespace { get; set; }

        [Option('m', "mode", Default = "aot", HelpText = "Test generation mode. Use aot to embed test data into test files, and use jit to fetch test data on execution time")]
        public string TestGenMode { get; set; }

        [Option('l', "labels", HelpText = "Comma seperated list of labels of scenarios for which to generate unit tests")]
        public string Labels { get; set; }
    }
}

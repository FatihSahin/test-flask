using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.CLI.Options;
using TestFlask.CLI.UnitTestGen.T4;

namespace TestFlask.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<UnitTestGenOptions>(args)
                .MapResult(
                    (UnitTestGenOptions options) => RunGenerateUnitTests(options),
                     errs => 1);
        }

        private static int RunGenerateUnitTests(UnitTestGenOptions options)
        {
            MSTestGen testGenerator = new MSTestGen(options);

            string contents = testGenerator.TransformText();

            File.WriteAllText($"{options.ClassName}_Auto.cs", contents);

            Console.WriteLine("Tests are generated successfully! Press any key to exit...");
            Console.ReadLine();

            return 0;
        }
    }
}

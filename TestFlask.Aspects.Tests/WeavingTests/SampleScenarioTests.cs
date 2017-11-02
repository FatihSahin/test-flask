using Mono.Cecil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Aspects.Tests.WeaveTests
{
    [TestFixture]
    public class SampleScenarioTests
    {
        private Assembly assembly;
        private string newAssemblyPath;
        private string assemblyPath;

        [OneTimeSetUp]
        public void Setup()
        {
            var projectPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\AssemblyToProcess\AssemblyToProcess.csproj"));
            assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\AssemblyToProcess.dll");

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\"));

#if (!DEBUG)
            assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

            newAssemblyPath = assemblyPath.Replace(".dll", ".AspectsTests.dll");
            File.Copy(assemblyPath, newAssemblyPath, true);

            using (var moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters { AssemblyResolver = resolver }))
            {
                var weavingTask = new ModuleWeaver
                {
                    ModuleDefinition = moduleDefinition
                };

                weavingTask.Execute();
                moduleDefinition.Write(newAssemblyPath);
            }

            assembly = Assembly.LoadFile(newAssemblyPath);
        }
    }
}

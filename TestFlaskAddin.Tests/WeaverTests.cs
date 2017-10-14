using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;
using AssemblyToProcess;

[TestFixture]
public class WeaverTests
{
    Assembly assembly;
    string newAssemblyPath;
    string assemblyPath;

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

        newAssemblyPath = assemblyPath.Replace(".dll", ".TestFlask.dll");
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

    [Test]
    public void InvokeGetSomeOriginal()
    {
        var request = assembly.CreateInstance("AssemblyToProcess.SomeRequest");
        request.GetType().GetProperty("SomeReqProperty").SetValue(request, 111, null);

        var response = assembly.GetType("AssemblyToProcess.SomeClient")
            .GetMethod("GetSome__Original")
            .Invoke(assembly.CreateInstance("AssemblyToProcess.SomeClient"), new object[] { request });

        Assert.AreEqual(111, response.GetType().GetProperty("SomeProperty").GetValue(response, null));
    }


    [Test]
    public void WeaveDoSome()
    {
       
    }


#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(assemblyPath, newAssemblyPath);
    }
#endif
}
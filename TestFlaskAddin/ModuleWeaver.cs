using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;
using System.IO;
using Newtonsoft.Json;
using TestFlask.Aspects;
using TestFlask.Aspects.Enums;
using System.Collections.Generic;
using TestFlaskAddin.Fody;

public class ModuleWeaver
{
    // Will log an informational message to MSBuild
    public Action<string> LogInfo { get; set; }

    // An instance of Mono.Cecil.ModuleDefinition for processing
    public ModuleDefinition ModuleDefinition { get; set; }

    // Will contain the full directory path of the current weaver.
    public string AddinDirectoryPath { get; set; }

    // An instance of Mono.Cecil.IAssemblyResolver for resolving assembly references. OPTIONAL
    public IAssemblyResolver AssemblyResolver { get; set; }

    // Will contain the full directory path of the target project. 
    // A copy of $(ProjectDir). OPTIONAL
    public string ProjectDirectoryPath { get; set; }

    private ModuleDefinition TestFlaskAspectsModule { get; set; }

    private List<ModuleDefinition> ReferencedModules { get; set; }

    TypeSystem typeSystem;

    // Init logging delegates to make testing easier
    public ModuleWeaver()
    {
        LogInfo = m => { };
    }

    public void Execute()
    {
        if (!string.IsNullOrEmpty(AddinDirectoryPath)) //it is not empty when triggered from MS Build (Fody)
        {
            var defAssemblyResolver = new DefaultAssemblyResolver();
            defAssemblyResolver.AddSearchDirectory(ProjectDirectoryPath + "bin");
            defAssemblyResolver.AddSearchDirectory(ProjectDirectoryPath + @"bin\Debug");
            defAssemblyResolver.AddSearchDirectory(ProjectDirectoryPath + @"bin\Release");
            AssemblyResolver = defAssemblyResolver;
        }
        else //for testing
        { 
            AssemblyResolver = ModuleDefinition.AssemblyResolver;
        }

        ReferencedModules = new List<ModuleDefinition>();
        foreach(var x in ModuleDefinition.AssemblyReferences)
        {
            ReferencedModules.Add(AssemblyResolver.Resolve(x).MainModule);
        }

        TestFlaskAspectsModule = ResolveTestFlaskAspectsModuleDefinition();

        typeSystem = ModuleDefinition.TypeSystem;

        var playableMethods = ModuleDefinition.GetAllTypes().SelectMany(
            t => t.Methods.Where(
                md => md.CustomAttributes.Any(ca => ca.AttributeType.Name.Equals("PlaybackAttribute")))
        ).ToList();

        foreach (var playableMethod in playableMethods)
        {
            var playbackAttr = playableMethod.CustomAttributes.Single(ca => ca.AttributeType.Name.Equals("PlaybackAttribute"));

            TypeDefinition requestIdentifierTypeDef = playbackAttr.ConstructorArguments[0].Value as TypeDefinition;
            TypeDefinition responseIdentifierTypeDef = playbackAttr.ConstructorArguments[1].Value as TypeDefinition;

            var clonePlayableMethod = CloneOriginalMethod(playableMethod);

            DecoratePlaybackAndRecord(playableMethod, clonePlayableMethod, requestIdentifierTypeDef, responseIdentifierTypeDef);

        }
    }

    private ModuleDefinition GetModuleForReferencedType(TypeReference typeRef)
    {
        if (typeRef.Scope is AssemblyNameReference)
        {
            ReferencedModules.First(rm => rm.Assembly.FullName == (typeRef.Scope as AssemblyNameReference).FullName);
        }

        return ModuleDefinition;
    }

    private ModuleDefinition ResolveTestFlaskAspectsModuleDefinition()
    {
        var testFlaskAspectsAssemblyReference = new AssemblyNameReference("TestFlask.Aspects", new Version("1.0.0.0"));

        AssemblyDefinition definition;

        if (!string.IsNullOrEmpty(AddinDirectoryPath))
        {
            //todo use an integration test to test this!
            var assemblyResolver = new DefaultAssemblyResolver();

            assemblyResolver.AddSearchDirectory(AddinDirectoryPath);

            definition = assemblyResolver.Resolve(testFlaskAspectsAssemblyReference);

            if (definition == null)
            {
                throw new Exception("Can't find TestFlask.Aspects assembly. Make sure you've downloaded and installed the nuget package!");
            }
        }
        else
        {
            definition = ModuleDefinition.AssemblyResolver.Resolve(testFlaskAspectsAssemblyReference);
        }

        return definition.MainModule;
    }

    void DecoratePlaybackAndRecord(MethodDefinition playableMethod, MethodDefinition originalMethod, TypeDefinition requestIdentifier, TypeDefinition responseIdentifier)
    {
        var reqResTypes = new List<TypeReference>();

        foreach(var pType in playableMethod.Parameters)
        {
            var module = GetModuleForReferencedType(pType.ParameterType);
            var def = module.ImportReference(pType.ParameterType);
            reqResTypes.Add(ModuleDefinition.ImportReference(def.Resolve()));
        }

        var resModule = GetModuleForReferencedType(playableMethod.ReturnType);

        var responseType = ModuleDefinition.ImportReference(resModule.ImportReference(playableMethod.ReturnType).Resolve());
        reqResTypes.Add(responseType);

        var reqResArray = reqResTypes.ToArray();

        MethodReference requestIdentifierCtorRef = null;
        if (requestIdentifier != null)
        {
            requestIdentifierCtorRef = requestIdentifier != null ? ModuleDefinition.ImportReference(requestIdentifier.GetConstructors().First()) : null;
        }

        MethodReference responseIdentifierCtorRef = null;
        if (responseIdentifier != null)
        {
            responseIdentifierCtorRef = responseIdentifier != null ? ModuleDefinition.ImportReference(responseIdentifier.GetConstructors().First()) : null;
        }

        var dynPlayerTypeDef = TestFlaskAspectsModule.GetType(GetPlayerType(playableMethod.Parameters.Count).FullName);
        var dynPlayerTypeRef = ModuleDefinition.ImportReference(dynPlayerTypeDef);
        GenericInstanceType playerType = dynPlayerTypeRef.MakeGenericInstanceType(reqResArray);
        var genericPlayerTypeDef = playerType.Resolve();

        var dynTestModesDef = TestFlaskAspectsModule.GetType(typeof(TestModes).FullName);
        var dynTestModesRef = ModuleDefinition.ImportReference(dynTestModesDef);
        var testModesDef = dynTestModesRef.Resolve();

        MethodReference playerCtorRef = ModuleDefinition.ImportReference(genericPlayerTypeDef.GetConstructors().First().MakeHostInstanceGeneric(reqResArray));
        MethodReference startInvocationMethodRef = ModuleDefinition.ImportReference(genericPlayerTypeDef.Methods.First(m => m.Name == "StartInvocation").MakeHostInstanceGeneric(reqResArray));
        MethodReference determineTestModeMethodRef = ModuleDefinition.ImportReference(genericPlayerTypeDef.Methods.First(m => m.Name == "DetermineTestMode").MakeHostInstanceGeneric(reqResArray));
        MethodReference playMethodRef = ModuleDefinition.ImportReference(genericPlayerTypeDef.Methods.First(m => m.Name == "Play").MakeHostInstanceGeneric(reqResArray));
        MethodReference recordMethodRef = ModuleDefinition.ImportReference(genericPlayerTypeDef.Methods.First(m => m.Name == "Record").MakeHostInstanceGeneric(reqResArray));
        MethodReference callOriginalMethodRef = ModuleDefinition.ImportReference(genericPlayerTypeDef.Methods.First(m => m.Name == "CallOriginal").MakeHostInstanceGeneric(reqResArray));

        var orgFuncRef = ModuleDefinition.ImportReference(GetOrgFuncType(playableMethod.Parameters.Count));
        GenericInstanceType orgFuncType = orgFuncRef.MakeGenericInstanceType(reqResArray);
        var genericOrgFuncTypeDef = orgFuncType.Resolve();

        MethodReference orgFuncCtorRef = ModuleDefinition.ImportReference(genericOrgFuncTypeDef.GetConstructors()
            .Where(c => c.Parameters.Count == 2) //(object, IntPtr)
            .First().MakeHostInstanceGeneric(reqResArray));

        //Start re-writing body
        MethodBody body = playableMethod.Body;

        body.Variables.Clear();
        body.Variables.Add(new VariableDefinition(playerType));
        body.Variables.Add(new VariableDefinition(ModuleDefinition.ImportReference(testModesDef)));
        var returnResponse = new VariableDefinition(responseType);
        body.Variables.Add(returnResponse);

        var il = body.GetILProcessor();

        //these are jump markers
        var playClause = il.Create(OpCodes.Ldloc_0);
        var recordClause = il.Create(OpCodes.Ldloc_0);
        var noMockClause = il.Create(OpCodes.Ldloc_0);
        var defaultClause = il.Create(OpCodes.Ldnull);
        var endOfMethod = il.Create(OpCodes.Ldloc_2);

        body.Instructions.Clear();
        body.Instructions.Add(il.Create(OpCodes.Nop));

        //create player
        body.Instructions.Add(il.Create(OpCodes.Ldstr, playableMethod.FullName));

        if (requestIdentifierCtorRef != null)
        {
            body.Instructions.Add(il.Create(OpCodes.Newobj, requestIdentifierCtorRef));
        }
        else
        {
            body.Instructions.Add(il.Create(OpCodes.Ldnull));
        }

        if (responseIdentifier != null)
        {
            body.Instructions.Add(il.Create(OpCodes.Newobj, responseIdentifierCtorRef));
        }
        else
        {
            body.Instructions.Add(il.Create(OpCodes.Ldnull));
        }

        body.Instructions.Add(il.Create(OpCodes.Newobj, playerCtorRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_0)); //player

        //start invocation
        body.Instructions.Add(il.Create(OpCodes.Ldloc_0)); //player
        LoadAllArgs(playableMethod, body, il);
        body.Instructions.Add(il.Create(OpCodes.Callvirt, startInvocationMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Nop));

        //determine mode
        body.Instructions.Add(il.Create(OpCodes.Ldloc_0)); //player
        LoadAllArgs(playableMethod, body, il);
        body.Instructions.Add(il.Create(OpCodes.Callvirt, determineTestModeMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_1)); //test mode

        //beginning switch
        body.Instructions.Add(il.Create(OpCodes.Ldloc_1)); //test mode
        body.Instructions.Add(il.Create(OpCodes.Switch, new Instruction[] { noMockClause, recordClause, playClause }));
        body.Instructions.Add(il.Create(OpCodes.Br_S, defaultClause));

        //noMockClause
        body.Instructions.Add(noMockClause);
        LoadAllArgs(playableMethod, body, il);
        body.Instructions.Add(il.Create(OpCodes.Ldarg_0)); //this
        body.Instructions.Add(il.Create(OpCodes.Ldftn, originalMethod));
        body.Instructions.Add(il.Create(OpCodes.Newobj, orgFuncCtorRef));
        body.Instructions.Add(il.Create(OpCodes.Callvirt, callOriginalMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_2));
        body.Instructions.Add(il.Create(OpCodes.Br_S, endOfMethod));

        //recordClause
        body.Instructions.Add(recordClause);
        LoadAllArgs(playableMethod, body, il);
        body.Instructions.Add(il.Create(OpCodes.Ldarg_0)); //this
        body.Instructions.Add(il.Create(OpCodes.Ldftn, originalMethod));
        body.Instructions.Add(il.Create(OpCodes.Newobj, orgFuncCtorRef));
        body.Instructions.Add(il.Create(OpCodes.Callvirt, recordMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_2));
        body.Instructions.Add(il.Create(OpCodes.Br_S, endOfMethod));

        //playClause
        body.Instructions.Add(playClause);
        LoadAllArgs(playableMethod, body, il);
        body.Instructions.Add(il.Create(OpCodes.Callvirt, playMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_2));
        body.Instructions.Add(il.Create(OpCodes.Br_S, endOfMethod));

        //defaultClause
        body.Instructions.Add(defaultClause);
        body.Instructions.Add(il.Create(OpCodes.Stloc_2));
        body.Instructions.Add(il.Create(OpCodes.Br_S, endOfMethod));

        //end context
        body.Instructions.Add(endOfMethod);
        body.Instructions.Add(il.Create(OpCodes.Ret));
    }

    private Type GetOrgFuncType(int reqArgCount)
    {
        switch (reqArgCount)
        {
            case 0:
                return typeof(Func<>);
            case 1:
                return typeof(Func<,>);
            case 2:
                return typeof(Func<,,>);
            case 3:
                return typeof(Func<,,,>);
            case 4:
                return typeof(Func<,,,,>);
            case 5:
                return typeof(Func<,,,,,>);
            default:
                throw new ArgumentException("Up to 5 request args are supported");
        }
    }

    private static void LoadAllArgs(MethodDefinition playableMethod, MethodBody body, ILProcessor il)
    {
        for (int i = 0; i < playableMethod.Parameters.Count; i++)
        {
            body.Instructions.Add(il.Create(OpCodes.Ldarg, i + 1));
        }
    }

    private Type GetPlayerType(int reqArgCount)
    {
        switch (reqArgCount)
        {
            case 0:
                return typeof(Player<>);
            case 1:
                return typeof(Player<,>);
            case 2:
                return typeof(Player<,,>);
            case 3:
                return typeof(Player<,,,>);
            case 4:
                return typeof(Player<,,,,>);
            case 5:
                return typeof(Player<,,,,,>);
            default:
                throw new ArgumentException("Up to 5 request args are supported");
        }
    }

    MethodDefinition CloneOriginalMethod(MethodDefinition playableMethod)
    {
        MethodDefinition clonePlayable = new MethodDefinition($"{playableMethod.Name}__Original", playableMethod.Attributes, playableMethod.ReturnType);
        clonePlayable.DeclaringType = playableMethod.DeclaringType;
        clonePlayable.Body.InitLocals = true;

        foreach (var parameterDefinition in playableMethod.Parameters)
        {
            clonePlayable.Parameters.Add(parameterDefinition);
        }

        foreach (var variable in playableMethod.Body.Variables)
        {
            clonePlayable.Body.Variables.Add(variable);
        }

        foreach (var instr in playableMethod.Body.Instructions)
        {
            clonePlayable.Body.Instructions.Add(instr);
        }

        playableMethod.DeclaringType.Methods.Add(clonePlayable);

        return clonePlayable;
    }
}
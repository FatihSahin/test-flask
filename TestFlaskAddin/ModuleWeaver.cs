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
using System.Xml.Linq;
using System.Threading.Tasks;
using TestFlask.Aspects.Player;

public class ModuleWeaver
{
    // Will contain the full element XML from FodyWeavers.xml. OPTIONAL
    public XElement Config { get; set; }

    // Will log an MessageImportance.Normal message to MSBuild. OPTIONAL
    public Action<string> LogDebug { get; set; }

    // Will log an MessageImportance.High message to MSBuild. OPTIONAL
    public Action<string> LogInfo { get; set; }

    // Will log an warning message to MSBuild. OPTIONAL
    public Action<string> LogWarning { get; set; }

    // Will log an warning message to MSBuild at a specific point in the code. OPTIONAL
    public Action<string, SequencePoint> LogWarningPoint { get; set; }

    // Will log an error message to MSBuild. OPTIONAL
    public Action<string> LogError { get; set; }

    // Will log an error message to MSBuild at a specific point in the code. OPTIONAL
    public Action<string, SequencePoint> LogErrorPoint { get; set; }

    // An instance of Mono.Cecil.IAssemblyResolver for resolving assembly references. OPTIONAL
    public IAssemblyResolver AssemblyResolver { get; set; }

    // An instance of Mono.Cecil.ModuleDefinition for processing. REQUIRED
    public ModuleDefinition ModuleDefinition { get; set; }

    // Will contain the full path of the target assembly. OPTIONAL
    public string AssemblyFilePath { get; set; }

    // Will contain the full directory path of the target project. 
    // A copy of $(ProjectDir). OPTIONAL
    public string ProjectDirectoryPath { get; set; }

    // Will contain the full directory path of the current weaver. OPTIONAL
    public string AddinDirectoryPath { get; set; }

    // Will contain the full directory path of the current solution.
    // A copy of `$(SolutionDir)` or, if it does not exist, a copy of `$(MSBuildProjectDirectory)..\..\..\`. OPTIONAL
    public string SolutionDirectoryPath { get; set; }

    // Will contain a semicomma delimetered string that contains 
    // all the references for the target project. 
    // A copy of the contents of the @(ReferencePath). OPTIONAL
    public string References { get; set; }

    // Will a list of all the references marked as copy-local. 
    // A copy of the contents of the @(ReferenceCopyLocalPaths). OPTIONAL
    public List<string> ReferenceCopyLocalPaths { get; set; }

    // Will a list of all the msbuild constants. 
    // A copy of the contents of the $(DefineConstants). OPTIONAL
    public List<string> DefineConstants { get; set; }

    private ModuleDefinition TestFlaskAspectsModule { get; set; }

    private List<ModuleDefinition> ReferencedModules { get; set; }

    // Init logging delegates to make testing easier
    public ModuleWeaver()
    {
        LogInfo = m => { };
    }

    public void Execute()
    {
        if (!string.IsNullOrEmpty(AddinDirectoryPath)) //it is not empty when triggered from MS Build (Fody)
        {
            List<string> localRefDirs = new List<string>();
            foreach (var refCopyLocalPath in ReferenceCopyLocalPaths)
            {
                localRefDirs.Add(Directory.GetParent(refCopyLocalPath).FullName);
            }

            var defAssemblyResolver = new DefaultAssemblyResolver();

            defAssemblyResolver.AddSearchDirectory(AddinDirectoryPath);

            localRefDirs.ForEach(refDir => defAssemblyResolver.AddSearchDirectory(refDir));

            AssemblyResolver = defAssemblyResolver;
        }
        else //for testing
        {
            AssemblyResolver = ModuleDefinition.AssemblyResolver;
        }

        ReferencedModules = new List<ModuleDefinition>();
        foreach (var assemblyRef in ModuleDefinition.AssemblyReferences)
        {
            var refModuleDefinition = AssemblyResolver.Resolve(assemblyRef).MainModule;
            ReferencedModules.Add(refModuleDefinition);
        }

        TestFlaskAspectsModule = ResolveTestFlaskAspectsModuleDefinition();

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

            DecorateMethod(playableMethod, clonePlayableMethod, requestIdentifierTypeDef, responseIdentifierTypeDef);
        }

        AssemblyResolver?.Dispose();
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

        AssemblyDefinition definition = ModuleDefinition.AssemblyResolver.Resolve(testFlaskAspectsAssemblyReference);

        return definition.MainModule;
    }

    void DecorateMethod(MethodDefinition playableMethod, MethodDefinition originalMethod, TypeDefinition requestIdentifier, TypeDefinition responseIdentifier)
    {        
        var originalMethodRef = CreateOrgMethodInstanceRef(originalMethod);

        var reqResTypes = new List<TypeReference>();

        foreach (var pType in playableMethod.Parameters)
        {
            var module = GetModuleForReferencedType(pType.ParameterType);
            var def = module.ImportReference(pType.ParameterType);
            reqResTypes.Add(ModuleDefinition.ImportReference(def));
        }

        var resModule = GetModuleForReferencedType(playableMethod.ReturnType);

        var responseType = ModuleDefinition.ImportReference(resModule.ImportReference(playableMethod.ReturnType));

        bool isFunc = responseType.FullName != "System.Void"; //this is to determine if original method is a function or an action

        if (isFunc)
        {
            reqResTypes.Add(responseType);
        }

        var reqResArray = reqResTypes.ToArray();

        var dynPlayerTypeDef = TestFlaskAspectsModule.GetType(GetPlayerType(playableMethod.Parameters.Count, isFunc).FullName);
        var dynPlayerTypeRef = ModuleDefinition.ImportReference(dynPlayerTypeDef);

        TypeDefinition playerTypeDef;
        TypeReference playerTypeRef;

        bool isPlayerGeneric = isFunc || playableMethod.Parameters.Count > 0; //if the player is instance is generic somehow (usually it is)

        if (isPlayerGeneric)
        {
            playerTypeRef = dynPlayerTypeRef.MakeGenericInstanceType(reqResArray);
            playerTypeDef = playerTypeRef.Resolve();
        }
        else //this is method with no args and no response (it is not generic at all)
        {
            playerTypeRef = dynPlayerTypeRef;
            playerTypeDef = dynPlayerTypeDef;
        }

        var dynTestModesDef = TestFlaskAspectsModule.GetType(typeof(TestModes).FullName);
        var dynTestModesRef = ModuleDefinition.ImportReference(dynTestModesDef);
        var testModesDef = dynTestModesRef.Resolve();

        MethodReference playerCtorRef;
        MethodReference startInvocationMethodRef;
        MethodReference determineTestModeMethodRef;
        MethodReference playMethodRef;
        MethodReference recordMethodRef;
        MethodReference callOriginalMethodRef;

        if (isPlayerGeneric)
        {
            playerCtorRef = ModuleDefinition.ImportReference(playerTypeDef.GetConstructors().First().MakeHostInstanceGeneric(reqResArray), playableMethod);
            startInvocationMethodRef = ModuleDefinition.ImportReference(playerTypeDef.Methods.First(m => m.Name == "BeginInvocation").MakeHostInstanceGeneric(reqResArray), playableMethod);
            determineTestModeMethodRef = ModuleDefinition.ImportReference(playerTypeDef.Methods.First(m => m.Name == "DetermineTestMode").MakeHostInstanceGeneric(reqResArray), playableMethod);
            playMethodRef = ModuleDefinition.ImportReference(playerTypeDef.Methods.First(m => m.Name == "Play").MakeHostInstanceGeneric(reqResArray), playableMethod);
            recordMethodRef = ModuleDefinition.ImportReference(playerTypeDef.Methods.First(m => m.Name == "Record").MakeHostInstanceGeneric(reqResArray), playableMethod);
            callOriginalMethodRef = ModuleDefinition.ImportReference(playerTypeDef.Methods.First(m => m.Name == "CallOriginal").MakeHostInstanceGeneric(reqResArray), playableMethod);
        }
        else //this is method with no args and no response (it is not generic at all)
        {
            playerCtorRef = ModuleDefinition.ImportReference(playerTypeDef.GetConstructors().First());
            startInvocationMethodRef = ModuleDefinition.ImportReference(playerTypeDef.Methods.First(m => m.Name == "BeginInvocation"));
            determineTestModeMethodRef = ModuleDefinition.ImportReference(playerTypeDef.Methods.First(m => m.Name == "DetermineTestMode"));
            playMethodRef = ModuleDefinition.ImportReference(playerTypeDef.Methods.First(m => m.Name == "Play"));
            recordMethodRef = ModuleDefinition.ImportReference(playerTypeDef.Methods.First(m => m.Name == "Record"));
            callOriginalMethodRef = ModuleDefinition.ImportReference(playerTypeDef.Methods.First(m => m.Name == "CallOriginal"));
        }

        var orgMethodRef = ModuleDefinition.ImportReference(GetOrgMethodType(playableMethod.Parameters.Count, isFunc));

        MethodReference orgMethodCtorRef;

        if (isPlayerGeneric)
        {
            GenericInstanceType orgMethodType = orgMethodRef.MakeGenericInstanceType(reqResArray);
            var genericOrgMethodTypeDef = orgMethodType.Resolve();

            orgMethodCtorRef = ModuleDefinition.ImportReference(genericOrgMethodTypeDef.GetConstructors()
                .Where(c => c.Parameters.Count == 2) //(object, IntPtr)
                .First().MakeHostInstanceGeneric(reqResArray), playableMethod);
        }
        else
        {
            orgMethodCtorRef = ModuleDefinition.ImportReference(orgMethodRef.Resolve().GetConstructors()
                .Where(c => c.Parameters.Count == 2) //(object, IntPtr)
                .First());
        }

        MethodReference requestIdentifierCtorRef = null;
        if (requestIdentifier != null)
        {
            requestIdentifierCtorRef = requestIdentifier != null ? ModuleDefinition.ImportReference(requestIdentifier.GetConstructors().First(), playableMethod) : null;
        }

        MethodReference responseIdentifierCtorRef = null;
        if (responseIdentifier != null)
        {
            responseIdentifierCtorRef = responseIdentifier != null ? ModuleDefinition.ImportReference(responseIdentifier.GetConstructors().First(), playableMethod) : null;
        }

        bool isStatic = playableMethod.IsStatic;

        if (isFunc)
        {
            DecorateFunc(playableMethod, originalMethodRef, playerTypeRef,
                testModesDef, responseType, requestIdentifierCtorRef, responseIdentifierCtorRef, playerCtorRef, startInvocationMethodRef,
                determineTestModeMethodRef, orgMethodCtorRef, callOriginalMethodRef, recordMethodRef, playMethodRef, isStatic);
        }
        else
        {
            DecorateAction(playableMethod, originalMethodRef, playerTypeRef,
                testModesDef, responseType, requestIdentifierCtorRef, responseIdentifierCtorRef, playerCtorRef, startInvocationMethodRef,
                determineTestModeMethodRef, orgMethodCtorRef, callOriginalMethodRef, recordMethodRef, playMethodRef, isStatic);
        }
    }

    private MethodReference CreateOrgMethodInstanceRef(MethodDefinition originalMethod)
    {
        MethodReference originalMethodRef = originalMethod as MethodReference;

        if (originalMethod.CallingConvention == MethodCallingConvention.Generic)
        {
            var genericInstanceMethod = new GenericInstanceMethod(originalMethod);
            foreach (var genericParam in originalMethod.GenericParameters)
            { 
                genericInstanceMethod.GenericArguments.Add(genericParam);
            }

            originalMethodRef = genericInstanceMethod;
        }

        return originalMethodRef;
    }

    void DecorateFunc(MethodDefinition playableMethod, MethodReference originalMethodRef,
        TypeReference playerTypeRef, TypeDefinition testModesDef, TypeReference responseType,
        MethodReference requestIdentifierCtorRef, MethodReference responseIdentifierCtorRef,
        MethodReference playerCtorRef, MethodReference startInvocationMethodRef, MethodReference determineTestModeMethodRef,
        MethodReference orgMethodCtorRef, MethodReference callOriginalMethodRef, MethodReference recordMethodRef, MethodReference playMethodRef, bool isStatic)
    {
        //Start re-writing body
        MethodBody body = new MethodBody(playableMethod.Body.Method);

        body.SimplifyMacros();

        body.Instructions.Clear();
        body.ExceptionHandlers.Clear();
        body.Variables.Clear();

        body.Variables.Add(new VariableDefinition(playerTypeRef));
        body.Variables.Add(new VariableDefinition(ModuleDefinition.ImportReference(testModesDef)));
        var returnResponse = new VariableDefinition(responseType);
        body.Variables.Add(returnResponse);

        body.InitLocals = true;

        var il = body.GetILProcessor();

        //these are jump markers
        var playClause = il.Create(OpCodes.Ldloc_0);
        var recordClause = il.Create(OpCodes.Ldloc_0);
        var noMockClause = il.Create(OpCodes.Ldloc_0);
        var defaultClause = il.Create(OpCodes.Ldstr, "Invalid TestFlask test mode detected!");

        var endOfMethod = il.Create(OpCodes.Ldloc_2);

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

        if (responseIdentifierCtorRef != null)
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
        LoadAllArgs(playableMethod, body, il, isStatic);
        body.Instructions.Add(il.Create(OpCodes.Callvirt, startInvocationMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Nop));

        //determine mode
        body.Instructions.Add(il.Create(OpCodes.Ldloc_0)); //player
        LoadAllArgs(playableMethod, body, il, isStatic);
        body.Instructions.Add(il.Create(OpCodes.Callvirt, determineTestModeMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_1)); //test mode

        //beginning switch
        body.Instructions.Add(il.Create(OpCodes.Ldloc_1)); //test mode
        body.Instructions.Add(il.Create(OpCodes.Switch, new Instruction[] { noMockClause, recordClause, playClause }));
        body.Instructions.Add(il.Create(OpCodes.Br_S, defaultClause));

        //noMockClause
        body.Instructions.Add(noMockClause);
        LoadAllArgs(playableMethod, body, il, isStatic);
        LoadThis(body, il, isStatic);
        body.Instructions.Add(il.Create(OpCodes.Ldftn, originalMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Newobj, orgMethodCtorRef));
        body.Instructions.Add(il.Create(OpCodes.Callvirt, callOriginalMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_2));
        body.Instructions.Add(il.Create(OpCodes.Br_S, endOfMethod));

        //recordClause
        body.Instructions.Add(recordClause);
        LoadAllArgs(playableMethod, body, il, isStatic);
        LoadThis(body, il, isStatic);
        body.Instructions.Add(il.Create(OpCodes.Ldftn, originalMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Newobj, orgMethodCtorRef));
        body.Instructions.Add(il.Create(OpCodes.Callvirt, recordMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_2));
        body.Instructions.Add(il.Create(OpCodes.Br_S, endOfMethod));

        //playClause
        body.Instructions.Add(playClause);
        LoadAllArgs(playableMethod, body, il, isStatic);
        body.Instructions.Add(il.Create(OpCodes.Callvirt, playMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_2));
        body.Instructions.Add(il.Create(OpCodes.Br_S, endOfMethod));

        //defaultClause (with =>  throw new Exception("Invalid TestFlask test mode detected!"))
        body.Instructions.Add(defaultClause);
        TypeReference exceptionTypeRef = ModuleDefinition.ImportReference(typeof(Exception));        //get ctor reference for ctor Exception(string message)
        MethodReference exceptionCtorRef = ModuleDefinition.ImportReference(exceptionTypeRef.Resolve().GetConstructors().Where(c => c.Parameters.Count == 1).First());
        body.Instructions.Add(il.Create(OpCodes.Newobj, exceptionCtorRef));
        body.Instructions.Add(il.Create(OpCodes.Throw));

        //end context
        body.Instructions.Add(endOfMethod);
        body.Instructions.Add(il.Create(OpCodes.Ret));

        body.OptimizeMacros();

        playableMethod.Body = body;
    }

    void DecorateAction(MethodDefinition playableMethod, MethodReference originalMethodRef,
        TypeReference playerTypeRef, TypeDefinition testModesDef, TypeReference responseType,
        MethodReference requestIdentifierCtorRef, MethodReference responseIdentifierCtorRef,
        MethodReference playerCtorRef, MethodReference startInvocationMethodRef, MethodReference determineTestModeMethodRef,
        MethodReference orgMethodCtorRef, MethodReference callOriginalMethodRef, MethodReference recordMethodRef, MethodReference playMethodRef, bool isStatic)
    {
        //Start re-writing body
        MethodBody body = new MethodBody(playableMethod.Body.Method);

        body.SimplifyMacros();

        body.Instructions.Clear();
        body.ExceptionHandlers.Clear();
        body.Variables.Clear();

        body.Variables.Add(new VariableDefinition(playerTypeRef));
        body.Variables.Add(new VariableDefinition(ModuleDefinition.ImportReference(testModesDef)));

        body.InitLocals = true;

        var il = body.GetILProcessor();

        //these are jump markers
        var returnMethod = il.Create(OpCodes.Ret);

        var playClause = il.Create(OpCodes.Ldloc_0);
        var recordClause = il.Create(OpCodes.Ldloc_0);
        var noMockClause = il.Create(OpCodes.Ldloc_0);

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

        body.Instructions.Add(il.Create(OpCodes.Newobj, playerCtorRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_0)); //player

        //start invocation
        body.Instructions.Add(il.Create(OpCodes.Ldloc_0)); //player
        LoadAllArgs(playableMethod, body, il, isStatic);
        body.Instructions.Add(il.Create(OpCodes.Callvirt, startInvocationMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Nop));

        //determine mode
        body.Instructions.Add(il.Create(OpCodes.Ldloc_0)); //player
        LoadAllArgs(playableMethod, body, il, isStatic);
        body.Instructions.Add(il.Create(OpCodes.Callvirt, determineTestModeMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Stloc_1)); //test mode

        //beginning switch
        body.Instructions.Add(il.Create(OpCodes.Ldloc_1)); //test mode
        body.Instructions.Add(il.Create(OpCodes.Switch, new Instruction[] { noMockClause, recordClause, playClause }));
        body.Instructions.Add(il.Create(OpCodes.Br_S, returnMethod));

        //noMockClause
        body.Instructions.Add(noMockClause);
        LoadAllArgs(playableMethod, body, il, isStatic);
        LoadThis(body, il, isStatic);
        body.Instructions.Add(il.Create(OpCodes.Ldftn, originalMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Newobj, orgMethodCtorRef));
        body.Instructions.Add(il.Create(OpCodes.Callvirt, callOriginalMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Nop));
        body.Instructions.Add(il.Create(OpCodes.Br_S, returnMethod));

        //recordClause
        body.Instructions.Add(recordClause);
        LoadAllArgs(playableMethod, body, il, isStatic);
        LoadThis(body, il, isStatic);
        body.Instructions.Add(il.Create(OpCodes.Ldftn, originalMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Newobj, orgMethodCtorRef));
        body.Instructions.Add(il.Create(OpCodes.Callvirt, recordMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Nop));
        body.Instructions.Add(il.Create(OpCodes.Br_S, returnMethod));

        //playClause
        body.Instructions.Add(playClause);
        LoadAllArgs(playableMethod, body, il, isStatic);
        body.Instructions.Add(il.Create(OpCodes.Callvirt, playMethodRef));
        body.Instructions.Add(il.Create(OpCodes.Nop));
        body.Instructions.Add(il.Create(OpCodes.Br_S, returnMethod));

        //defaultClause
        body.Instructions.Add(il.Create(OpCodes.Br_S, returnMethod));

        //return
        body.Instructions.Add(returnMethod);

        body.OptimizeMacros();

        playableMethod.Body = body;
    }

    private static void LoadThis(MethodBody body, ILProcessor il, bool isStatic)
    {
        if (!isStatic) //static methods do not have 'this' as first arg
        {
            body.Instructions.Add(il.Create(OpCodes.Ldarg_0)); //this
        }
        else
        {
            body.Instructions.Add(il.Create(OpCodes.Ldnull));
        }
    }

    private static void LoadAllArgs(MethodDefinition playableMethod, MethodBody body, ILProcessor il, bool isStatic)
    {
        for (int i = 0; i < playableMethod.Parameters.Count; i++)
        {
            body.Instructions.Add(il.Create(OpCodes.Ldarg, i + (isStatic ? 0 : 1)));
        }
    }

    MethodDefinition CloneOriginalMethod(MethodDefinition playableMethod)
    {
        MethodDefinition clonePlayable = new MethodDefinition($"{playableMethod.Name}__Original", playableMethod.Attributes, playableMethod.ReturnType);
        clonePlayable.DeclaringType = playableMethod.DeclaringType;
        clonePlayable.CallingConvention = playableMethod.CallingConvention;
        clonePlayable.HasThis = playableMethod.HasThis;
        clonePlayable.ExplicitThis = playableMethod.ExplicitThis;
        clonePlayable.Body.InitLocals = true;

        foreach (var parameterDefinition in playableMethod.Parameters)
        {
            clonePlayable.Parameters.Add(parameterDefinition);
        }

        foreach (var genericParameterDef in playableMethod.GenericParameters)
        {
            clonePlayable.GenericParameters.Add(new GenericParameter(genericParameterDef.Name, playableMethod));
        }

        foreach (var variable in playableMethod.Body.Variables)
        {
            clonePlayable.Body.Variables.Add(variable);
        }

        foreach (var instr in playableMethod.Body.Instructions)
        {
            clonePlayable.Body.Instructions.Add(instr);
        }

        foreach (var exhHandlers in playableMethod.Body.ExceptionHandlers)
        {
            clonePlayable.Body.ExceptionHandlers.Add(exhHandlers);
        }

        playableMethod.DeclaringType.Methods.Add(clonePlayable);

        return clonePlayable;
    }

    private Type GetOrgMethodType(int reqArgCount, bool isFunc)
    {
        switch (reqArgCount)
        {
            case 0:
                return isFunc ? typeof(Func<>) : typeof(Action);
            case 1:
                return isFunc ? typeof(Func<,>) : typeof(Action<>);
            case 2:
                return isFunc ? typeof(Func<,,>) : typeof(Action<,>);
            case 3:
                return isFunc ? typeof(Func<,,,>) : typeof(Action<,,>);
            case 4:
                return isFunc ? typeof(Func<,,,,>) : typeof(Action<,,,>);
            case 5:
                return isFunc ? typeof(Func<,,,,,>) : typeof(Action<,,,,>);
            default:
                throw new ArgumentException("Up to 5 request args are supported");
        }
    }

    private Type GetPlayerType(int reqArgCount, bool isFunc)
    {
        switch (reqArgCount)
        {
            case 0:
                return isFunc ? typeof(FuncPlayer<>) : typeof(ActionPlayer);
            case 1:
                return isFunc ? typeof(FuncPlayer<,>) : typeof(ActionPlayer<>);
            case 2:
                return isFunc ? typeof(FuncPlayer<,,>) : typeof(ActionPlayer<,>);
            case 3:
                return isFunc ? typeof(FuncPlayer<,,,>) : typeof(ActionPlayer<,,>);
            case 4:
                return isFunc ? typeof(FuncPlayer<,,,,>) : typeof(ActionPlayer<,,,>);
            case 5:
                return isFunc ? typeof(FuncPlayer<,,,,,>) : typeof(ActionPlayer<,,,,>);
            default:
                throw new ArgumentException("Up to 5 request args are supported");
        }
    }
}
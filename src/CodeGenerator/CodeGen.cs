using CppSharp;
using CppSharp.Generators;
using CppSharp.Generators.Cpp;
using CppSharp.Passes;
using ASTContext = CppSharp.AST.ASTContext;

namespace CodeGenerator;

public class CodeGen : ILibrary
{
    private const string LIBRARY_NAME = "lua54";
    
    public void Setup(Driver driver)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "vendor", "lua", "include");

        var options = driver.Options;
        options.GeneratorKind = GeneratorKind.CSharp;
        options.GenerateFinalizers = true;
        options.Verbose = true;
        options.GenerateInternalImports = true;
        options.GenerateDefaultValuesForArguments = true;
        

        var module = options.AddModule(LIBRARY_NAME);
        module.OutputNamespace = "PlutoNET";
        module.Defines.Add("LUA_BUILD_AS_DLL");
        module.Defines.Add("PLUTO_C_LINKAGE=true");
        
        module.Headers.Add(Path.Combine(path, "lualib.h"));
    }

    public void SetupPasses(Driver driver)
    {
        // TODO Add More passes to improve generated code even more
        driver.Context.TranslationUnitPasses.RenameDeclsUpperCase(RenameTargets.Property);
        driver.Context.TranslationUnitPasses.RenameDeclsUpperCase(RenameTargets.Class);
        driver.Context.TranslationUnitPasses.AddPass(new CheckDuplicatedNamesPass());
        driver.Context.TranslationUnitPasses.AddPass(new CheckFlagEnumsPass());
        driver.Context.TranslationUnitPasses.AddPass(new FixDefaultParamValuesOfOverridesPass());
        driver.Context.TranslationUnitPasses.AddPass(new HandleDefaultParamValuesPass());
        driver.Context.TranslationUnitPasses.AddPass(new MakeProtectedNestedTypesPublicPass());
        driver.Context.TranslationUnitPasses.AddPass(new ConstructorToConversionOperatorPass());
        driver.Context.TranslationUnitPasses.AddPass(new MoveFunctionToClassPass());
        driver.Context.TranslationUnitPasses.AddPass(new CheckMacroPass());
        driver.Context.TranslationUnitPasses.AddPass(new FixupPureMethodsPass());
        driver.Context.TranslationUnitPasses.AddPass(new DelegatesPass());
        driver.Context.TranslationUnitPasses.AddPass(new GenerateSymbolsPass());
        driver.Context.TranslationUnitPasses.AddPass(new FunctionToStaticMethodPass());
    }
    
    public void Preprocess(Driver driver, ASTContext ctx)
    {
        // TODO
    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
        // TODO
    }
}
using CppSharp.AST;
using CppSharp.Passes;

namespace CodeGenerator.TranslitionUnitPasses;

public class MakeInternalAndExternPass : TranslationUnitPass
{
    public override bool VisitClassDecl(Class @class)
    {
        if (!base.VisitClassDecl(@class))
            return false;

        if (@class.IsStatic)
            @class.Access = AccessSpecifier.Internal;

        return true;
    }

    public override bool VisitFunctionDecl(Function function)
    {
        if (!base.VisitFunctionDecl(function))
            return false;

        function.CallingConvention = CallingConvention.C;
        return true;
    }
}
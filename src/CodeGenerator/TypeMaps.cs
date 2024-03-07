using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Types;

namespace CodeGenerator;

public class TypeMaps
{
    [TypeMap("lua_State", GeneratorKind = GeneratorKind.CSharp)]
    public class LuaStateTypeMap : TypeMap
    {
        public override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
        {
            return new CustomType("IntPtr");
        }
    }
}
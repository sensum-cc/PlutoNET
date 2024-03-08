using CppSharp;
using CppSharp.Generators;
using CppSharp.Passes;

namespace CodeGenerator.GeneratorOutputPasses;

public class RenameOutputClasses : GeneratorOutputPass
{
    public override void VisitGeneratorOutput(GeneratorOutput output)
    {
        var unkowns = output.Outputs.SelectMany(i => i.FindBlocks(BlockKind.Unknown));

        foreach (var unkown in unkowns)
        {
            unkown.Text.StringBuilder.Replace("public", "internal");
            unkown.Text.StringBuilder.Replace("public", "internal");
        }
    }
}
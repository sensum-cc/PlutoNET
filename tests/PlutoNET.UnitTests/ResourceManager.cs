using System.Reflection;

namespace PlutoNET.UnitTests;

// This is ideal way to make your C# program single file (view .csproj to see how to embed resources to your assembly)
internal static class ResourceManager
{
    private static void WriteResourceToFile(string resourceName, string fileName)
    {
        using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (resource == null) return;

        using var file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan);
        resource.CopyTo(file);
        file.Close();
    }
    
    internal static void ExtractEmbedded()
    {
        foreach (var line in Assembly.GetExecutingAssembly().GetManifestResourceNames())
        {
            var name = line.Replace("PlutoNET.UnitTests.Libs.", "");

            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name)))
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name));

            WriteResourceToFile(line, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name));
        }
    }
}
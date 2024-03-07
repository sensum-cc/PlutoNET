using CodeGenerator;
using CppSharp;

ConsoleDriver.Run(new CodeGen());
var path = Path.Combine(Directory.GetCurrentDirectory(), "lua54.cs");
var fileText = File.ReadAllText(path);
//Remove line what causes error
fileText = fileText.Replace("public static global::PlutoNET.Pluto.PreloadedLibrary[] AllPreloaded { get; } = new global::PlutoNET.Pluto.PreloadedLibrary[2] { (global::PlutoNET.Pluto.PreloadedLibrary)preloaded_assert, (global::PlutoNET.Pluto.PreloadedLibrary) preloaded_vector3 };", "");
File.WriteAllText(path, fileText);
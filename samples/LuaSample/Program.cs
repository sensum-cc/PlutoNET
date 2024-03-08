using LuaSample;
using PlutoNET;

ResourceManager.ExtractEmbedded(); // Extract embedded libraries

var lua = new Lua();
lua.OpenLibs(); // Opens all lua std libs
lua.DoString("print('Hello World')"); // prints Hello World
/*lua.DoString("print('Hello World')"); // prints Hello World

// To learn more of the custom lua api features read from https://pluto-lang.org/docs/Introduction
lua.DoString("function printTest(message) \n print(message ?? 'Message was nil') \n end \n printTest(nil)");
lua.DoString("local x: int = 10 \n print($'variable x is {x}')");
lua.DoString("local x: string = 10 \n function test(a: string): number \n return tonumber(a) \n end \n test(x)"); // this will throw warning because of type mismatch*/
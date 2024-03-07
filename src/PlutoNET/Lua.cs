using static PlutoNET.lauxlib;

namespace PlutoNET;

public class Lua : IDisposable
{
    /* Native Lua State */
    public IntPtr L { get; set; }


    public Lua(bool openLibs = true)
    {
        L = LuaL_newstate();
    }

    public void OpenLibs()
    {

    }
    
    
    public void Dispose()
    {
        // TODO: Implement Dispose
    }

    ~Lua() => Dispose();
}
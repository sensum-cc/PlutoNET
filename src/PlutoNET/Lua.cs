using System.Runtime.CompilerServices;
using PlutoNET.Enums;
using static PlutoNET.lauxlib;
using static PlutoNET.lua;
using static PlutoNET.lualib;
using LuaStatus = PlutoNET.Enums.LuaStatus;
using LuaType = PlutoNET.Enums.LuaType;

namespace PlutoNET;

public class Lua : IDisposable
{
    /* Native Lua State */
    public IntPtr State { get; set; }
    

    /// <summary>
    /// Creates a new Lua state
    /// </summary>
    public Lua()
    {
        State = LuaL_newstate();
    }

    /// <summary>
    /// Creates new Lua instance from existing state
    /// </summary>
    /// <param name="state">lua state</param>
    public Lua(IntPtr state) => State = state;

    /// <summary>
    /// Opens all standard Lua libraries into the given state.
    /// </summary>
    public void OpenLibs() => LuaL_openlibs(State);

    public LuaStatus LoadString(string str) => (LuaStatus)LuaL_loadstring(State, str);

    public bool DoString(string str) => LoadString(str) != LuaStatus.Ok || PCall(0, -1, 0) != LuaStatus.Ok;

    public LuaStatus PCall(int args, int results, int errorFunctionIndex)  => (LuaStatus)LuaPcallk(State, args, results, 0, errorFunctionIndex, null);
    
    public void Call(int args, int results) => LuaCallk(State, args, results, 0, null);
    public void CallK(int args, int results, long ctx, LuaKFunction k) => LuaCallk(State, args, results, ctx, k);
    
    public LuaType GetGlobal(string name) => (LuaType)LuaGetglobal(State, name);
    public LuaType GetInteger(int index, long i) => (LuaType)LuaGeti(State, index, i);
    public LuaType Type(int index) => (LuaType)LuaType(State, index);
    public string TypeName(LuaType type) => LuaTypename(State, (int)type);
    

    /// <summary>
    /// Preforms lua error with message/value from top of the stack as error output
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Error()
    { 
        return LuaError(State);
    }
    
    /* Is<type> Methods (These methods are used to check if the value in given index is the <type> checked for) */
    public bool IsNone(int index) => Type(index) == LuaType.None;
    public bool IsNil(int index) => Type(index) == LuaType.Nil;
    public bool IsBoolean(int index) => Type(index) == LuaType.Boolean;
    public bool IsLightUserData(int index) => Type(index) == LuaType.LightUserData;
    public bool IsNumber(int index) => Type(index) == LuaType.Number;
    public bool IsString(int index) => Type(index) == LuaType.String;
    public bool IsTable(int index) => Type(index) == LuaType.Table;
    public bool IsFunction(int index) => Type(index) == LuaType.Function;
    public bool IsUserData(int index) => Type(index) == LuaType.UserData;
    public bool IsThread(int index) => Type(index) == LuaType.Thread;
    
    
    
    public void PushNil() => LuaPushnil(State);
    public void PushBoolean(bool value) => LuaPushboolean(State, value ? 1 : 0);
    public void PushLightUserData(IntPtr value) => LuaPushlightuserdata(State, value);
    public void PushNumber(double value) => LuaPushnumber(State, value);
    public void PushString(string value) => LuaPushstring(State, value);
    public void PushInteger(int value) => LuaPushinteger(State, value);
    public void PushThread() => LuaPushthread(State);
    
    public void CreateTable(int arr = 0, int rec = 0) => LuaCreatetable(State, arr, rec);
    
    
    /// <summary>
    /// Closes the Lua state
    /// </summary>
    public void Close()
    {
        LuaClose(State);
    }
    
    /// <summary>
    /// Free up resources
    /// </summary>
    public void Dispose()
    {
        if (State == 0) return;
        Close();
        State = 0;
        GC.SuppressFinalize(this);
    }

    ~Lua() => Dispose();
}
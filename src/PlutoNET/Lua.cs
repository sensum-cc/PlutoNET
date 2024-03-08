using static PlutoNET.lauxlib;
using static PlutoNET.lua;
using static PlutoNET.lualib;
using LuaStatus = PlutoNET.Enums.LuaStatus;
using LuaType = PlutoNET.Enums.LuaType;

namespace PlutoNET;

public class Lua(IntPtr l) : IDisposable
{
    public IntPtr L { get; set; } = l;

    public Lua() : this(LuaL_newstate()) {}
    
    public void OpenLibs() => LuaL_openlibs(L);
    
    /* ---------------------------------------- */
    
    public LuaStatus LoadBuffer(string buffer, string name, string? mode) => (LuaStatus)LuaL_loadbufferx(L, buffer, (ulong)buffer.Length, name, mode);
    public LuaStatus LoadBuffer(string buffer, string name) => LoadBuffer(buffer, name, null);
    public LuaStatus LoadString(string str) => (LuaStatus)LuaL_loadstring(L, str);
    public LuaStatus LoadFile(string path, string? mode) => (LuaStatus)LuaL_loadfilex(L, path, mode);
    public LuaStatus LoadFile(string path) => LoadFile(path, null);
    
    /* ---------------------------------------- */
    
    public void CallK(int args, int results, long ctx, LuaKFunction k) => LuaCallk(L, args, results, ctx, k);
    public LuaStatus PCallK(int args, int results, int errFunc, long ctx, LuaKFunction? k) => (LuaStatus)LuaPcallk(L, args, results, errFunc, ctx, k);
    public LuaStatus PCall(int args, int results, int errFunc) => PCallK(args, results, errFunc, 0, null);
    
    /* ---------------------------------------- */
    
    public bool DoString(string str) => LoadString(str) != LuaStatus.Ok || PCall(0, -1, 0) != LuaStatus.Ok;
    public bool DoFile(string path) => LoadFile(path) != LuaStatus.Ok || PCall(0, -1, 0) != LuaStatus.Ok;
    
    /* ---------------------------------------- */

    public void CreateTable(int arr, int rec) => LuaCreatetable(L, arr, rec);
    public void NewTable() => CreateTable(0, 0);
    
    /* ---------------------------------------- */
    
    public void SetGlobal(string name) => LuaSetglobal(L, name);
    public void SetTable(int index) => LuaSettable(L, index);
    
    
    /* ---------------------------------------- */

    public LuaType Type(int index) => (LuaType)LuaType(L, index);
    
    public void Register(string name, LuaCFunction function)
    {
        PushCFunction(function);
        SetGlobal(name);
    }
    
    /* ---------------------------------------- */

    public bool IsFunction(int index) => Type(index) == LuaType.Function;
    public bool IsTable(int index) => Type(index) == LuaType.Table;
    public bool IsLightUserData(int index) => Type(index) == LuaType.LightUserData;
    public bool IsNil(int index) => Type(index) == LuaType.Nil;
    public bool IsBoolean(int index) => Type(index) == LuaType.Boolean;
    public bool IsThread(int index) => Type(index) == LuaType.Thread;
    public bool IsNone(int index) => Type(index) == LuaType.None;
    public bool IsNoneOrNil(int index) => Type(index) <= LuaType.Nil;
    
    /* ---------------------------------------- */
    
    public void PushNil() => LuaPushnil(L);
    public void PushNumber(double value) => LuaPushnumber(L, value);
    public void PushInteger(int value) => LuaPushinteger(L, value);
    public void PushLString(string s, ulong len) => LuaPushlstring(L, s, len);
    public void PushString(string value) => LuaPushstring(L, value);
    public void PlutoPushString(string value) => PlutoPushstring(L, value);
    public void PushFormatString(string fmt) => LuaPushfstring(L, fmt);
    public void PushCClosure(LuaCFunction function, int n) => LuaPushcclosure(L, function, n);
    public void PushBoolean(bool value) => LuaPushboolean(L, value ? 1 : 0);
    public void PushLightUserData(IntPtr value) => LuaPushlightuserdata(L, value);
    public void PushThread() => LuaPushthread(L);
    public void PushCFunction(LuaCFunction function) => PushCClosure(function, 0);
    
    /* ---------------------------------------- */

    public long CheckInteger(int index) => LuaL_checkinteger(L, index);
    public double CheckNumber(int index) => LuaL_checknumber(L, index);
    public string CheckString(int index)
    {
        ulong length = 0;
        var buff = LuaL_checklstring(L, index, ref length);
        return buff ?? "";
    }
    
    
    /* ---------------------------------------- */
    
    public void Close() => LuaClose(L);

    public void Dispose()
    {
        if (L == 0) Close();
        GC.SuppressFinalize(this);
    }

    ~Lua() => Dispose();
}
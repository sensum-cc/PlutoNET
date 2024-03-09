using System.Runtime.InteropServices;
using static PlutoNET.lauxlib;
using static PlutoNET.lua;
using static PlutoNET.lualib;
using LuaStatus = PlutoNET.Enums.LuaStatus;
using LuaType = PlutoNET.Enums.LuaType;

namespace PlutoNET;

public class Lua(IntPtr l) : IDisposable
{
    private readonly List<IntPtr> _handles = [];
    public IntPtr L { get; set; } = l;

    public Lua() : this(LuaL_newstate()) {}

    /// <summary>
    /// Opens all standard Lua libraries into the Lua state.
    /// </summary>
    public void OpenLibs() => LuaL_openlibs(L);

    /* ---------------------------------------- */

    /// <summary>
    /// Loads a Lua chunk from a string buffer into the Lua state.
    /// </summary>
    /// <param name="buffer">The string buffer containing the Lua chunk.</param>
    /// <param name="name">The name of the chunk, for debug information.</param>
    /// <param name="mode">The mode in which to load the chunk. If null, defaults to "bt".</param>
    /// <returns>The status of the load operation.</returns>
    public LuaStatus LoadBuffer(string buffer, string name, string? mode) => (LuaStatus)LuaL_loadbufferx(L, buffer, (ulong)buffer.Length, name, mode);

    /// <summary>
    /// Loads a Lua chunk from a string buffer into the Lua state with default mode ("bt").
    /// </summary>
    /// <param name="buffer">The string buffer containing the Lua chunk.</param>
    /// <param name="name">The name of the chunk, for debug information.</param>
    /// <returns>The status of the load operation.</returns>
    public LuaStatus LoadBuffer(string buffer, string name) => LoadBuffer(buffer, name, null);

    /// <summary>
    /// Loads a Lua chunk from a string into the Lua state.
    /// </summary>
    /// <param name="str">The string containing the Lua chunk.</param>
    /// <returns>The status of the load operation.</returns>
    public LuaStatus LoadString(string str) => (LuaStatus)LuaL_loadstring(L, str);

    /// <summary>
    /// Loads a Lua chunk from a file into the Lua state.
    /// </summary>
    /// <param name="path">The path to the file containing the Lua chunk.</param>
    /// <param name="mode">The mode in which to load the chunk. If null, defaults to "bt".</param>
    /// <returns>The status of the load operation.</returns>
    public LuaStatus LoadFile(string path, string? mode) => (LuaStatus)LuaL_loadfilex(L, path, mode);

    /// <summary>
    /// Loads a Lua chunk from a file into the Lua state with default mode ("bt").
    /// </summary>
    /// <param name="path">The path to the file containing the Lua chunk.</param>
    /// <returns>The status of the load operation.</returns>
    public LuaStatus LoadFile(string path) => LoadFile(path, null);

    /* ---------------------------------------- */

    /// <summary>
    /// Calls a Lua function in protected mode with continuation support.
    /// </summary>
    /// <param name="args">The number of arguments that you pushed onto the stack.</param>
    /// <param name="results">The number of results that the function will push onto the stack.</param>
    /// <param name="ctx">The context value to be passed to the continuation function.</param>
    /// <param name="k">The continuation function to call when the thread yields.</param>
    public void CallK(int args, int results, long ctx, LuaKFunction k) => LuaCallk(L, args, results, ctx, k);

    /// <summary>
    /// Calls a Lua function in protected mode with continuation support and error handling.
    /// </summary>
    /// <param name="args">The number of arguments that you pushed onto the stack.</param>
    /// <param name="results">The number of results that the function will push onto the stack.</param>
    /// <param name="errFunc">The stack index of an error handler function.</param>
    /// <param name="ctx">The context value to be passed to the continuation function.</param>
    /// <param name="k">The continuation function to call when the thread yields.</param>
    /// <returns>The status of the call.</returns>
    public LuaStatus PCallK(int args, int results, int errFunc, long ctx, LuaKFunction? k) => (LuaStatus)LuaPcallk(L, args, results, errFunc, ctx, k);

    /// <summary>
    /// Calls a Lua function in protected mode with error handling.
    /// </summary>
    /// <param name="args">The number of arguments that you pushed onto the stack.</param>
    /// <param name="results">The number of results that the function will push onto the stack.</param>
    /// <param name="errFunc">The stack index of an error handler function.</param>
    /// <returns>The status of the call.</returns>
    public LuaStatus PCall(int args, int results, int errFunc) => PCallK(args, results, errFunc, 0, null);

    /* ---------------------------------------- */

    /// <summary>
    /// Executes a Lua script from a string.
    /// </summary>
    /// <param name="str">The Lua script to execute.</param>
    /// <returns>True if the script execution failed, false otherwise.</returns>
    public bool DoString(string str) => LoadString(str) != LuaStatus.Ok || PCall(0, -1, 0) != LuaStatus.Ok;

    /// <summary>
    /// Executes a Lua script from a file.
    /// </summary>
    /// <param name="path">The path to the Lua script file to execute.</param>
    /// <returns>True if the script execution failed, false otherwise.</returns>
    public bool DoFile(string path) => LoadFile(path) != LuaStatus.Ok || PCall(0, -1, 0) != LuaStatus.Ok;

    /* ---------------------------------------- */

    /// <summary>
    /// Creates a new table in the Lua state with the given array and record sizes.
    /// </summary>
    /// <param name="arr">The size of the array part of the table.</param>
    /// <param name="rec">The size of the record part of the table.</param>
    public void CreateTable(int arr, int rec) => LuaCreatetable(L, arr, rec);

    /// <summary>
    /// Creates a new table in the Lua state with default array and record sizes (0).
    /// </summary>
    public void NewTable() => CreateTable(0, 0);
    /* ---------------------------------------- */

    /// <summary>
    /// Returns the value of a table field from the Lua stack at the given index.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>The value of the table field at the given index.</returns>
    public int GetTable(int index) => LuaGettable(L, index);

    /// <summary>
    /// Returns the metatable of the Lua value at the given index.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>The metatable of the Lua value at the given index.</returns>
    public int GetMetaTable(int index) => LuaGetmetatable(L, index);

    /// <summary>
    /// Returns the value of a field in a table at the given index, using the provided key.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <param name="key">The key of the field to get.</param>
    /// <returns>The value of the field at the given index.</returns>
    public int GetField(int index, string key) => LuaGetfield(L, index, key);

    /// <summary>
    /// Returns the index of the top element in the Lua stack. This is the number of elements in the stack.
    /// </summary>
    /// <returns>The index of the top element in the Lua stack.</returns>
    public int GetTop() => LuaGettop(L);


    /* ---------------------------------------- */

    /// <summary>
    /// Converts the Lua value at the given acceptable index to a boolean.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the Lua value is true, false otherwise.</returns>
    public bool ToBoolean(int index) => LuaToboolean(L, index) == 1;

    /// <summary>
    /// Returns the user data associated with the Lua value at the given acceptable index.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>The user data as an IntPtr.</returns>
    public IntPtr ToUserData(int index) => LuaTouserdata(L, index);

    /// <summary>
    /// Returns the user data associated with the Lua value at the given acceptable index, cast to the specified type.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <param name="free">If true, the associated GCHandle is freed after the user data is retrieved.</param>
    /// <returns>The user data cast to the specified type, or null if the Lua value is nil or not a user data.</returns>
    public T? ToUserData<T>(int index, bool free = false)
    {
        if (IsNil(index) || !IsUserData(index)) return default;

        var ptr = ToUserData(index);
        if (ptr == 0) return default;

        var handle = GCHandle.FromIntPtr(ptr);
        if (handle.IsAllocated == false) return default;

        var obj = (T?)handle.Target;

        if (free)
            handle.Free();

        return obj;
    }

    /* ---------------------------------------- */

    /// <summary>
    /// Sets a global variable in the Lua state with the given name.
    /// </summary>
    /// <param name="name">The name of the global variable.</param>
    public void SetGlobal(string name) => LuaSetglobal(L, name);

    /// <summary>
    /// Sets a table in the Lua state at the given index.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    public void SetTable(int index) => LuaSettable(L, index);

    /// <summary>
    /// Sets a metatable in the Lua state at the given index.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    public void SetMetaTable(int index) => LuaSetmetatable(L, index);

    /// <summary>
    /// Sets a field in the Lua state at the given index with the given key.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <param name="key">The key of the field to set.</param>
    public void SetField(int index, string key) => LuaSetfield(L, index, key);


    /* ---------------------------------------- */

    /// <summary>
    /// Returns the type of the value at the given index in the Lua stack.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>The type of the value at the given index.</returns>
    public LuaType Type(int index) => (LuaType)LuaType(L, index);

    /// <summary>
    /// Registers a C function in Lua with the given name.
    /// </summary>
    /// <param name="name">The name to register the function under.</param>
    /// <param name="function">The C function to register.</param>
    public void Register(string name, LuaCFunction function)
    {
        PushCFunction(function);
        SetGlobal(name);
    }

    /* ---------------------------------------- */

    /// <summary>
    /// Checks if the value at the given index is a function.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is a function, false otherwise.</returns>
    public bool IsFunction(int index) => Type(index) == LuaType.Function;

    /// <summary>
    /// Checks if the value at the given index is a table.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is a table, false otherwise.</returns>
    public bool IsTable(int index) => Type(index) == LuaType.Table;

    /// <summary>
    /// Checks if the value at the given index is a light user data.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is a light user data, false otherwise.</returns>
    public bool IsLightUserData(int index) => Type(index) == LuaType.LightUserData;

    /// <summary>
    /// Checks if the value at the given index is nil.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is nil, false otherwise.</returns>
    public bool IsNil(int index) => Type(index) == LuaType.Nil;

    /// <summary>
    /// Checks if the value at the given index is a boolean.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is a boolean, false otherwise.</returns>
    public bool IsBoolean(int index) => Type(index) == LuaType.Boolean;

    /// <summary>
    /// Checks if the value at the given index is a thread.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is a thread, false otherwise.</returns>
    public bool IsThread(int index) => Type(index) == LuaType.Thread;

    /// <summary>
    /// Checks if the value at the given index is none.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is none, false otherwise.</returns>
    public bool IsNone(int index) => Type(index) == LuaType.None;

    /// <summary>
    /// Checks if the value at the given index is a number.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is a number, false otherwise.</returns>
    public bool IsNumber(int index) => Type(index) == LuaType.Number;

    /// <summary>
    /// Checks if the value at the given index is a string.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is a string, false otherwise.</returns>
    public bool IsString(int index) => Type(index) == LuaType.String;

    /// <summary>
    /// Checks if the value at the given index is a user data.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is a user data, false otherwise.</returns>
    public bool IsUserData(int index) => LuaIsuserdata(L, index) == 1;

    /// <summary>
    /// Checks if the value at the given index is none or nil.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is none or nil, false otherwise.</returns>
    public bool IsNoneOrNil(int index) => Type(index) <= LuaType.Nil;

    /// <summary>
    /// Checks if the value at the given index is an integer.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>True if the value is an integer, false otherwise.</returns>
    public bool IsInteger(int index) => LuaIsinteger(L, index) == 1;

    /* ---------------------------------------- */

    /// <summary>
    /// Pushes a nil value onto the Lua stack.
    /// </summary>
    public void PushNil() => LuaPushnil(L);

    /// <summary>
    /// Pushes a number onto the Lua stack.
    /// </summary>
    /// <param name="value">The number to push.</param>
    public void PushNumber(double value) => LuaPushnumber(L, value);

    /// <summary>
    /// Pushes an integer onto the Lua stack.
    /// </summary>
    /// <param name="value">The integer to push.</param>
    public void PushInteger(long value) => LuaPushinteger(L, value);

    /// <summary>
    /// Pushes a string onto the Lua stack, specifying its length.
    /// </summary>
    /// <param name="s">The string to push.</param>
    /// <param name="len">The length of the string.</param>
    public void PushLString(string s, ulong len) => LuaPushlstring(L, s, len);

    /// <summary>
    /// Pushes a string onto the Lua stack.
    /// </summary>
    /// <param name="value">The string to push.</param>
    public void PushString(string value) => LuaPushstring(L, value);

    /// <summary>
    /// Pushes a string onto the Lua stack using the Pluto string function.
    /// </summary>
    /// <param name="value">The string to push.</param>
    public void PlutoPushString(string value) => PlutoPushstring(L, value);

    /// <summary>
    /// Pushes a formatted string onto the Lua stack.
    /// </summary>
    /// <param name="fmt">The format of the string.</param>
    public void PushFormatString(string fmt) => LuaPushfstring(L, fmt);

    /// <summary>
    /// Pushes a C closure onto the Lua stack.
    /// </summary>
    /// <param name="function">The function to push.</param>
    /// <param name="n">The number of upvalues to associate with the function.</param>
    public void PushCClosure(LuaCFunction function, int n) => LuaPushcclosure(L, function, n);

    /// <summary>
    /// Pushes a boolean value onto the Lua stack.
    /// </summary>
    /// <param name="value">The boolean value to push.</param>
    public void PushBoolean(bool value) => LuaPushboolean(L, value ? 1 : 0);

    /// <summary>
    /// Pushes a light user data onto the Lua stack.
    /// </summary>
    /// <param name="value">The user data to push.</param>
    public void PushLightUserData(IntPtr value) => LuaPushlightuserdata(L, value);

    /// <summary>
    /// Pushes the current thread onto the Lua stack.
    /// </summary>
    public void PushThread() => LuaPushthread(L);

    /// <summary>
    /// Pushes a copy of the element at the given valid index onto the Lua stack.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    public void PushValue(int index) => LuaPushvalue(L, index);

    /// <summary>
    /// Pushes a C function onto the Lua stack. This is done by pushing a C closure onto the stack.
    /// </summary>
    /// <param name="function">The function to push.</param>
    public void PushCFunction(LuaCFunction function) => PushCClosure(function, 0);

    /// <summary>
    /// Pushes a light user data onto the Lua stack. The user data is a managed object.
    /// </summary>
    /// <param name="obj">The managed object to push.</param>
    public void PushLightUserData<T>(T obj)
    {
        var handle = GCHandle.Alloc(obj);
        var ptr = GCHandle.ToIntPtr(handle);
        _handles.Add(ptr);
        PushLightUserData(ptr);
    }

    /* ---------------------------------------- */

    /// <summary>
    /// Checks if the value at the given acceptable index is a number (integer), and returns it.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>The integer at the given index.</returns>
    public long CheckInteger(int index) => LuaL_checkinteger(L, index);

    /// <summary>
    /// Checks whether the function argument at the given acceptable index is a number (float), and returns it.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>The number at the given index.</returns>
    public double CheckNumber(int index) => LuaL_checknumber(L, index);

    /// <summary>
    /// Checks whether the function argument at the given acceptable index is a string and returns this string.
    /// </summary>
    /// <param name="index">The index in the Lua stack.</param>
    /// <returns>The string at the given index. If the value is not a string, it returns an empty string.</returns>
    public string CheckString(int index)
    {
        ulong length = 0;
        var buff = LuaL_checklstring(L, index, ref length);
        return buff ?? "";
    }

    /* ---------------------------------------- */

    /// <summary>
    /// Closes the Lua state and frees any allocated handles.
    /// </summary>
    public void Close()
    {
        LuaClose(L);
        foreach (var handlePtr in _handles)
        {
            if (handlePtr == IntPtr.Zero) continue;
            var handle = GCHandle.FromIntPtr(handlePtr);
            if (handle.IsAllocated) handle.Free();
        }

        _handles.Clear();
    }

    public void Dispose()
    {
        if (L == 0) Close();
        GC.SuppressFinalize(this);
    }

    ~Lua() => Dispose();
}
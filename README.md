# PlutoNET [WIP]
PlutoNET is C# Wrapper for our modified version of PlutoLang's Pluto (Lua) Interpreter
<br>
This could still be used with official PlutoLang's Pluto native builds

# Building Native Libs
Building Native Libs is not required as we have pre-built shared libraries for both windows and linux they're located in libs folder
<br>
These are the instructions for both windows and linux
### pre-requirements
- CMake
- C++ Compiler with c++17 support
- Visual Studio 2019 or CLion (Windows) | GCC (Linux)
- Git

### Building
1. Clone the repository
2. Run `cd PlutoNET` to navigate to the project directory
3. Run `git submodule update --init --recursive` to clone the submodules
4. If you're using CLion you can open the project in CLion file and build the project and ignore rest build guides
4. Run `cd vendor/lua` to navigate to the lua directory
4. Run `cmake -S . -B ./build` to generate the build files

<br>
Whn you're done building the shared library you can move it to libs folder
# NeoUnityian-Physics
This is a Neohookean physics plugin for Unity.  Note that it's not currently working correctly.

### External Dependencies
The following repositories are required for this plugin, and are included as submodules:
- GAUSS (https://github.com/dilevin/GAUSS)
- Geometry3sharp (https://github.com/gradientspace/geometry3Sharp)

The workflow also requires Tetgen (http://wias-berlin.de/software/index.jsp?id=TetGen&lang=1#Download), which you must manually download and compile manually.

You may also need the appropriate version of QT in order to compile GAUSS (QT 5.9 on Windows, >=5.8 otherwise)

### Installation
(Note that this plugin is currently only compatible with Windows. It has only been tested on a Windows 10 64bit machine as of right now, using the Visual Studio 15 compiler)

Make sure you add the flag "--recursive" when cloning this repository, so that the submodules will be propertly included!

Firstly, you must compile the C++ plugin.  Copy the "Plugin" folder into GAUSS/src. Then, add the following line to the end of GAUSS/CMakeLists.txt: `add_subdirectory(${PROJECT_SOURCE_DIR}/src/Plugin)`

You may also want to change line 11 to `option(GAUSS_USE_UI     "Use Gauss' UI Library" OFF)` in order to prevent all of the QT-based examples and UI libraries from being compiled, since we're using Unity for that.

To build, you generally do the following in the GAUSS repository:
`mkdir build
cd build
cmake -DCMAKE_BUILD_TYPE=Release -C ../config.cmake ../
make`

In order to have it work correctly on my end, I used the Developer Command Prompt for Visual Studio, and I had to change the third command to: `cmake -DCMAKE_BUILD_TYPE=Release -C ../config.cmake ../ -G "Visual Studio 15 Win64"`, as it otherwise defaults to Win32 and changing it manually is a pain.  

After compiling, you should have the file GAUSS/build/bin/Release/NeoUnityian.dll. Copy this to your Assets/Plugins folder in your Unity project.  You should also copy the Scripts folder into your Assets.

The last step is to compile Tetgen (using the VS command prompt, `cl tetgen.c`). Create a folder called Utilities in your Assets folder, and drop Tetgen.exe there.  You should then be good to go.

### Unity Project Setup

1. Create an empty GameObject, and name it WorldManager.  Add the script of the same name to it.

2. Put the "TetMesh" script component on any other GameObject you wish to test this on.  Said GameObject is required to have both a MeshFilter and a MeshRenderer.  In the inspector, you can then add a mesh to the Tetmesh component.

3. Add your GameObject to WorldManager's `objects` array.  

When you start the game, the plugin will detect that the input mesh was changed.  It will use Tetgen to generate a new volumetric mesh (as opposed to the surface meshes Unity generally uses).  TetMesh will then send the new mesh's vertex and face data to the MeshFilter, and WorldManager will initalize a GAUSS world using the vertex and tetrahedron data of every object it is given. On every frame, WorldManager will fetch the vertex displacements from GAUSS and update its objects with them.

### Known Issues

- This plugin was working correctly at one point, but the DLL is no longer generating new displacements after the third step or so. It was broken in the process of adding support for multiple objects.
- At the moment, you have to manually change NeoUnityian.cpp in order to change what physics systems will be used for the simulation.  Will have to add some way to change the systems used from within Unity instead.
- Unity keeps flipping the normals the wrong way

### Future Steps

- Add some way to link Unity objects together within the GAUSS world
- Will eventually replace GAUSS (or heavily modify it) with a faster Quasi-Newtonian solver.

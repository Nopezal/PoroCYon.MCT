Mod Creation Tools
==================

Mod Creaiton Tools for the Terraria Application (Mod) Programming Interface

http://www.terrariaonline.com/threads/132361/

Initialize MCT
--------------

    // in ModBase.OnLoad:
    Mct.Init();
    
Enable Edit & Continue
----------------------

Use these command-line arguments when launching tAPI:
    
    tAPI.exe -debug <internal name> <path to assembly>

(either the prefix -, --, / or no prefix can be used for the 'debug' argument)

Edit & Continue does not always work in constructors of 'global types' (ModBase, ModWorld, ModInterface, ...) and the ModBase.OnLoad method.

Invoke MCT Tools.exe
--------------------

The MCT Tools is a command-line program that has a custom built compiler and a mod decompiler.
It can be invoked through CMD using the 'mct' command.
You can also use the TAPIBINDIR, TAPIMODDIR, TAPIMODSRCDIR, TAPIMODOUTDIR and MCTDIR environment variables.

use the /? or /help argument to display help info.

Examples:

    mct compile "Ingame Cheat Menu"

(that's the command I use to compile the ICM)
The path points either to a path relative to where you invoked the MCT,
or a folder in the %tapimodsrcdir% folder. You can also use an absolute path.

    mct decompile "Unsorted\\MyMod.tapi"

    
(either the prefix -, --, / or no prefix can be used for the arguments (excepth paths/names))

Automate building & debugging in Visual Studio
----------------------------------------------

Make sure the Target framework is 4.0(.30319) (NOT the client profile),
and the Platform target is x86 (for all build configurations).

Building:
    
    Put this code in the Post-build script:

        mct compile "$(ProjectDir)"

Debugging:

    Set the startup object to your project,
    and set the 'Start Action' under the 'Debug' tab in the project properties to 'Start external program', and browse to tAPI.exe.

    Set the Command-line arguments (under 'Start options') to

        -debug <internal name> <path to assembly>

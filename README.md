Mod Creation Tools
==================

Mod Creaiton Tools for the Terraria Application (Mod) Programming Interface

http://www.terrariaonline.com/threads/132361/

Initialize MCT
--------------

    // in ModBase.OnLoad:
    Mct.Init();
    
Invoke MCT Tools.exe
--------------------

The MCT Tools is a command-line program that has a custom built compiler and a mod decompiler.
It can be invoked through CMD using the 'mct' command. You can also use the TAPIBINDIR, TAPIMODDIR, TAPIMODSRCDIR, TAPIMODOUTDIR and MCTDIR environment variables.

use the /? argument to display help info.

Examples:

    mct compile "Ingame Cheat Menu"
(that's the command I use to compile the ICM)

    mct decompile "Unsorted\\MyMod.tapi"

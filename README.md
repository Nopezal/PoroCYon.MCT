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

The MCT Tools is a command-line program
that can build a mod from a managed .dll,
decompile a .tapi or .tapimod file,
or pack a mod written in JScript.NET or Visual Basic.

use the /? argument to display help info.

Examples:

    -build "C:\SomeDll.dll"
    -decompile "Unsorted\MyMod.tapi"
    -pack "MyJSMod"

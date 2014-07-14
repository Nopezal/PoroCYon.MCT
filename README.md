# Mod Creation Tools

Mod Creaiton Tools for the Terraria Application (Mod) Programming Interface

http://www.terrariaonline.com/threads/132361/

---

## Table of Contents
1. [Initialize MCT](#init)
1. [Enable Edit & Continue in Visual Studio](#enc)
1. [Invoke MCT Tools.exe](#mcttools)
1. [Automate building & debugging in Visual Studio](#vsautomation)

## Initialize MCT <a id="init"></a>

```csharp
// in ModBase.OnLoad:
Mct.Init();
```
    
This is needed to enable most features of the MCT.

Use IntelliSense/Look around in the repo to discover other features.

## Enable Edit & Continue in Visual Studio <a id="enc"></a>

Use these command-line arguments when launching tAPI:
    
`tAPI.exe -debug <internal name> <path to assembly>`

(either the prefix -, --, / or no prefix can be used for the 'debug' argument)

Edit &amp; Continue does not always work in constructors of 'global types' (ModBase, ModWorld, ModInterface, ...) and the ModBase.OnLoad method.

## Invoke MCT Tools.exe <a id="mcttools"></a>

The MCT Tools is a command-line program that has a custom built compiler and a mod decompiler.
It can be invoked through CMD using the 'mct' command.
You can also use the TAPIBINDIR, TAPIMODDIR, TAPIMODSRCDIR, TAPIMODOUTDIR and MCTDIR environment variables.

use the `/?` or `/help` argument to display help info.

Examples:

`mct compile "Ingame Cheat Menu"`
(that's the command I use to compile the ICM)

The path points either to a path relative to where you invoked the MCT,
or a folder in the %tapimodsrcdir% folder. You can also use an absolute path.

`mct decompile "Unsorted\\MyMod.tapi"`

(either the prefix -, --, / or no prefix can be used for the arguments (excepth paths/names))

## Automate building & debugging in Visual Studio <a id="vsautomation"></a>

Make sure the Target framework is 4.0(.30319) (NOT the client profile), and the Platform target is x86 (for all build configurations).

### Building

Put this code in the Post-build script:
`mct compile "$(ProjectDir)"`

### Debugging

1. Set the startup object to your project, and set the 'Start Action' under the 'Debug' tab in the project properties to 'Start external program', and browse to tAPI.exe.
2. Set the Command-line arguments (under 'Start options') to `-debug <internal name> <path to assembly>`

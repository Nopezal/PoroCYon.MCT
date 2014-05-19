using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;
using PoroCYon.MCT.Internal;

namespace PoroCYon.MCT.Tools
{
    /// <summary>
    /// An object containing the name (display and internal), author, version and info of a mod.
    /// </summary>
    class ModInformation
    {
        /// <summary>
        /// The display name of the mod
        /// </summary>
        internal string DisplayName
        {
            get;
            private set;
        }
        /// <summary>
        /// The internal name of the mod
        /// </summary>
        internal string InternalName
        {
            get;
            private set;
        }
        /// <summary>
        /// The author name of the mod
        /// </summary>
        internal string Author
        {
            get;
            private set;
        }
        /// <summary>
        /// The version name of the mod
        /// </summary>
        internal Version Version
        {
            get;
            private set;
        }
        /// <summary>
        /// The description name of the mod
        /// </summary>
        internal string Description
        {
            get;
            private set;
        }

        internal ModInformation(JsonData modInfo)
        {
            if (!modInfo.Has("internalName"))
                throw new JsonException("The ModInfo.json file doesn't have an internalName field.");
            if (!modInfo.Has("displayName"))
                throw new JsonException("The ModInfo.json file doesn't have a displayName field.");

            DisplayName = (string)modInfo["displayName"];
            InternalName = (string)modInfo["internalName"];

            Author = !modInfo.Has("author") ? "<unknown>" : (string)modInfo["author"];
            Version = new Version(!modInfo.Has("version") ? "0.0.0.0" : (string)modInfo["version"]);
            Description = !modInfo.Has("info")
                ? DisplayName + " v" + Version + " by " + Author
                : (string)modInfo["info"];
        }
    }

    /// <summary>
    /// Options for mod compilation
    /// </summary>
    class CompilerOptions
    {
        /// <summary>
        /// Wether the compiler should use MSBuild or CodeDom to compile the mod.
        /// </summary>
        internal bool UseMSBuild = false;
        /// <summary>
        /// The MSBuild file used to compile the mod. (Only used when UseMSBuild is set to true)
        /// </summary>
        internal string MSBuildFile = null;
        /// <summary>
        /// Wether the mod should include debug symbols (PDB) or not.
        /// </summary>
        internal bool IncludeDebugSymbols = false;
        /// <summary>
        /// Wether the compiler should zip the output file (.tapi) or not (.tapimod).
        /// </summary>
        internal bool ZipOutputFile = true;
        /// <summary>
        /// The output file of the mod, not including the extension.
        /// </summary>
        internal string OutputPath = null;
        /// <summary>
        /// The mod information of the mod (ModInfo.json) (other data than the members of this class)
        /// </summary>
        internal ModInformation ModInfo = null;
        /// <summary>
        /// Wether to check the JSON files for syntax errors and missing fields.
        /// </summary>
        internal bool Validate = true;
        /// <summary>
        /// Wether to check the built assembly for missing classes, etc.
        /// </summary>
        internal bool Check = true;

        internal JsonData RawModInfo;

        /// <summary>
        /// Creates a new instance of the CompilerOptions class
        /// </summary>
        /// <param name="modName">The directory name of the mod, relative to the Mods\Sources folder.</param>
        /// <param name="zipFile">Wether the output file should be compressed (.tapi) or not (.tapimod).</param>
        internal CompilerOptions(string modName)
        {
            try
            {
                RawModInfo = JsonMapper.ToObject(File.ReadAllText(CommonToolUtilities.modsSrcDir + "\\" + modName + "\\ModInfo.json"));

                ModInfo = new ModInformation(RawModInfo);

                UseMSBuild = RawModInfo.Has("MSBuild") ? (bool)RawModInfo["MSBuild"] : false;
                if (UseMSBuild)
                    MSBuildFile = RawModInfo.Has("msBuildFile") ? (string)RawModInfo["msBuildFile"] : modName + ".csproj";

                IncludeDebugSymbols = RawModInfo.Has("includePDB") ? (bool)RawModInfo["includePDB"] : false;
                ZipOutputFile = RawModInfo.Has("zipOutputFile") ? (bool)RawModInfo["zipOutputFile"] : true;
                Validate = RawModInfo.Has("validate") ? (bool)RawModInfo["validate"] : true;
                Check = RawModInfo.Has("check") ? (bool)RawModInfo["check"] : true;

                OutputPath = CommonToolUtilities.modsBinDir + "\\modName";
            }
            catch (Exception e)
            {
                throw new CompilerException("Invalid ModInfo.json file in mod '" + modName + "'.", e);
            }
        }
    }
}

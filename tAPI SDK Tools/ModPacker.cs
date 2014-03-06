using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.CSharp;
using Microsoft.JScript;
using Microsoft.VisualBasic;
using Ionic.Zip;
using TAPI.SDK.Internal;

namespace TAPI.SDK.Tools.Packer
{
    // not the ones in Microsoft.VisualBasic (or was it JScript?)
    using CompilerError = System.CodeDom.Compiler.CompilerError;
    using CompilerParameters = System.CodeDom.Compiler.CompilerParameters;
    // nested classes <_>
    using CompileException = TAPI.ModsCompile.CompileException;
    using ValidateJson = TAPI.ModsCompile.ValidateJson;

    /// <summary>
    /// The tAPI SDK Extended mod packed
    /// </summary>
    public static class ModPacker
    {
        /// <summary>
        /// Packs a mod
        /// </summary>
        /// <param name="modDirectory">The directory of the mod to pack</param>
        /// <param name="outputDirectory">The output directory</param>
        /// <returns>A CompilerException if there are compiler errors, null if none.</returns>
        public static void Pack(string modDirectory, string outputDirectory)
        {
            CompileException cex = new CompileException(modDirectory + "\\");

            string modName = Path.GetDirectoryName(modDirectory);

            #region validating ModInfo.json
            string jsonFile = modDirectory + "\\ModInfo.json";
            JsonData json = null;
            Dictionary<string, string> dictNames = null;
            if (!File.Exists(jsonFile))
            {
                File.WriteAllText(jsonFile, CommonToolUtilities.CreateDefaultModInfo(modName));
                Console.WriteLine("Warning: You do not have a ModInfo.json file.\n\tUsing the default ModInfo...");
            }
            try
            {
                json = JsonMapper.ToObject(File.ReadAllText(jsonFile));
                if (!json.Has("displayName"))
                    cex.AddProblem(jsonFile, "Missing ModInfo field 'displayName'");
                if (!json.Has("author"))
                    cex.AddProblem(jsonFile, "Missing ModInfo field 'author'");
                if (!json.Has("internalName"))
                    cex.AddProblem(jsonFile, "Missing ModInfo field 'internalName'");
                if (json.Has("modReferences"))
                    dictNames = Mods.GetInternalNameToPathDictionary();
                ValidateJson.ModInfo(jsonFile, json, cex, dictNames);
            }
            catch (Exception e)
            {
                cex.AddProblem(jsonFile, "Invalid JSON file.\n" + e.Message);
            }
            #endregion

            // .pdb stuff is done in BuildSource and WriteData

            BuildSource(modDirectory, outputDirectory, json, cex, dictNames);

            List<Tuple<string, byte[]>> files = new List<Tuple<string, byte[]>>();
            List<string> allowExt = new List<string> { ".png", ".json", ".fx", ".dll", ".wav", ".xnb", ".xml", ".xaml", ".html" };

            foreach (string fileName in Directory.EnumerateFiles(modDirectory, "*.*", SearchOption.AllDirectories))
            {
                if (fileName.EndsWith(".cs") || fileName.EndsWith(".vb") || fileName.EndsWith(".js"))
                    continue;

                foreach (string ext in allowExt)
                    if (fileName.EndsWith(ext))
                    {
                        string fname = fileName.Substring(modDirectory.Length + 1).Replace('\\', '/');

                        if (fname == "ModInfo.json")
                            continue;

                        files.Add(new Tuple<string, byte[]>(fname, File.ReadAllBytes(fileName)));
                        break;
                    }
            }

            ModsCompile.WriteData(modDirectory, outputDirectory, files, json);
        }

        static void BuildSource(string sourcePath, string outputPath, JsonData modInfo, CompileException cex, Dictionary<string, string> dictNames)
        {

            string[] modPathSplit = sourcePath.Split('\\', '/');
            string modName = modPathSplit[modPathSplit.Length - 1].Split('.')[0];

            if (modInfo.Has("MSBuild"))
                if ((bool)modInfo["MSBuild"])
                {
                    // done by msbuild anyway
                    ModsCompile.BuildSource(sourcePath, outputPath, modInfo, cex, dictNames);
                    return;
                }

            bool generatePDB = false;
            if (modInfo.Has("includePDB"))
                generatePDB = (bool)modInfo["includePDB"];

            // but this has to change - other CodeDomProviders (default stays C#)
            CodeDomProvider cdp = new CSharpCodeProvider();

            foreach (string fileName in Directory.EnumerateFiles(sourcePath, "*.json", SearchOption.AllDirectories))
            {
                string fname = fileName.Substring(sourcePath.Length + 1).Replace('\\', '/');
                if (fname == "ModInfo.json")
                    continue;
                try
                {
                    JsonData json2 = JsonMapper.ToObject(File.ReadAllText(fileName));
                    if (fname.ToLower().StartsWith("item/"))
                        ValidateJson.Item(modName, fileName, json2, cex);
                    if (fname.ToLower().StartsWith("npc/"))
                        ValidateJson.NPC(modName, fileName, json2, cex);
                    if (fname.ToLower().StartsWith("projectile/"))
                        ValidateJson.Projectile(modName, fileName, json2, cex);
                    //TODO: check all the JSON files other than ModInfo.json for required fields
                }
                catch (Exception e)
                {
                    cex.AddProblem(fileName, "Invalid JSON file.\n" + e.Message);
                }
            }

            CompilerParameters cp = new CompilerParameters();

            cp.GenerateExecutable = false;

            cp.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            cp.IncludeDebugInformation = generatePDB;

            cp.ReferencedAssemblies.Add("mscorlib.dll");
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add("System.Numerics.dll");
            cp.ReferencedAssemblies.Add("System.Xml.dll");

            cp.ReferencedAssemblies.Add("System.Drawing.dll");
            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            cp.ReferencedAssemblies.Add("Microsoft.Xna.Framework.dll");
            cp.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Xact.dll");
            cp.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Game.dll");
            cp.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Graphics.dll");

            if (modInfo != null)
            {
                if (modInfo.Has("language"))
                    switch (((string)modInfo["language"]).ToLowerInvariant())
                    {
                        case "js":
                        case "js.net":
                        case "jscript":
                        case "jscript.net":
                        case "javascript":
                        case "javascript.net":
                            cdp = new JScriptCodeProvider();
                            break;
                        case "vb":
                        case "vb.net":
                        case "visualbasic":
                        case "visualbasic.net":
                        case "visual basic":
                        case "visual basic.net":
                            cdp = new VBCodeProvider();
                            break;
                    }

                if (modInfo.Has("modReferences"))
                {
                    if (Directory.Exists(Mods.pathDirMods + "/.Temp"))
                        Directory.Delete(Mods.pathDirMods + "/.Temp", true);
                    Directory.CreateDirectory(Mods.pathDirMods + "/.Temp");
                    JsonData jRefs = (JsonData)modInfo["modReferences"];
                    for (int i = 0; i < jRefs.Count; i++)
                    {
                        string jRef = (string)jRefs[i];
                        if (!dictNames.ContainsKey(jRef))
                            continue;
                        string modfile = dictNames[jRef];
                        cp.ReferencedAssemblies.Add(Mods.pathDirMods + "/.Temp/" + jRef + ".dll");

                        string[] split = jRef.Split('\\');
                        if (modfile.EndsWith(".tapimod"))
                        {
                            using (FileStream fileStream = new FileStream(modfile, FileMode.Open))
                            {
                                BinBuffer bb2 = new BinBuffer(new BinBufferStream(fileStream));
                                bb2.ReadInt();
                                bb2.ReadString();
                                int count = bb2.ReadInt();
                                int skip = 0;
                                while (count-- > 0)
                                {
                                    bb2.ReadString();
                                    skip += bb2.ReadInt();
                                }
                                while (skip-- > 0)
                                    bb2.ReadByte();
                                File.WriteAllBytes(Mods.pathDirMods + "/.Temp/" + jRef + ".dll", bb2.ReadBytes(bb2.BytesLeft()));
                            }
                        }
                        else if (modfile.EndsWith(".tapi"))
                        {
                            using (ZipFile zip = ZipFile.Read(modfile))
                            {
                                if (zip.ContainsEntry("Mod.tapimod"))
                                {
                                    ZipEntry ze = zip["Mod.tapimod"];
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        ze.Extract(ms);
                                        ms.Position = 0;
                                        BinBuffer bb2 = new BinBuffer(new BinBufferStream(ms));
                                        bb2.ReadInt();
                                        bb2.ReadString();
                                        int count = bb2.ReadInt();
                                        int skip = 0;
                                        while (count-- > 0)
                                        {
                                            bb2.ReadString();
                                            skip += bb2.ReadInt();
                                        }
                                        while (skip-- > 0)
                                            bb2.ReadByte();
                                        File.WriteAllBytes(Mods.pathDirMods + "/.Temp/" + jRef + ".dll", bb2.ReadBytes(bb2.BytesLeft()));
                                    }
                                }
                            }
                        }
                    }
                }
                if (modInfo.Has("dllReferences"))
                {
                    JsonData jRefs = (JsonData)modInfo["dllReferences"];
                    for (int i = 0; i < jRefs.Count; i++)
                    {
                        string jRef = (string)jRefs[i];
                        if (File.Exists(sourcePath + "/" + jRef)) // remove .dll -> can also reference .exes
                            cp.ReferencedAssemblies.Add(sourcePath + "/" + jRef);
                        else
                            cp.ReferencedAssemblies.Add(jRef); // somewhere else, like the GAC
                    }
                }
            }

            cp.OutputAssembly = outputPath + (cdp is VBCodeProvider ? "" : ".dll"); // VBCodeProvider automatically adds '.dll'

            List<string> toCompile = new List<string>();
            foreach (string fileName in Directory.EnumerateFiles(sourcePath, cdp.FileExtension, SearchOption.AllDirectories))
                toCompile.Add(fileName);

            CompilerResults cr = cdp.CompileAssemblyFromFile(cp, toCompile.ToArray());

            if (Directory.Exists(Mods.pathDirMods + "/.Temp"))
                Directory.Delete(Mods.pathDirMods + "/.Temp", true);

            if (cr.Errors.HasErrors)
            {
                foreach (CompilerError ce in cr.Errors)
                {
                    StringBuilder sb = new StringBuilder();
                    if (ce.FileName != "")
                    {
                        sb.Append("(" + ce.Column + "," + ce.Line + "): " + ce.ErrorText);
                        sb.Append("\n" + File.ReadLines(ce.FileName).Skip(ce.Line - 1).Take(1).First().Replace("\t", " "));
                        sb.Append('\n');
                        for (int i = 0; i < ce.Column - 1; i++)
                            sb.Append(' ');
                        sb.Append('^');
                        cex.AddProblem(ce.FileName, sb.ToString());
                    }
                    else // general error (without file) - .dll not found, etc
                        cex.AddProblem(outputPath, (ce.IsWarning ? "warning" : "error") + " " + ce.ErrorNumber + ": " + ce.ErrorText);
                }
            }

            if (cex.problems.Count != 0)
            {
                if (File.Exists(outputPath + ".dll"))
                    File.Delete(outputPath + ".dll");

                throw cex;
            }
        }
    }
}

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using Microsoft.JScript;
using Microsoft.VisualBasic;
using TAPI.SDK.Internal;

namespace TAPI.Packer.Extended
{
    // not the ones in Microsoft.VisualBasic (or was it JScript?)
    using CompilerError = System.CodeDom.Compiler.CompilerError;
    using CompilerParameters = System.CodeDom.Compiler.CompilerParameters;

    // easier to use than a return value
    [Serializable]
    public class CompilerException : Exception
    {
        const string DEFAULT_MESSAGE = "Failed to compile a mod";

        public CompilerException(string error)
            : base(error, null)
        {

        }
        public CompilerException(Exception inner)
            : base(DEFAULT_MESSAGE, inner)
        {

        }
        public CompilerException(string error, Exception inner)
            : base(error, inner)
        {

        }

        public static CompilerException CreateException(IEnumerable errors)
        {
            string error = "";

            foreach (CompilerError ce in errors)
            {
                string[] codeLines = Regex.Split(File.ReadAllText(ce.FileName), @"\r?\n|\r"); // \n, \r or \r\n

                error += " " + ce.ErrorText + ":" + Environment.NewLine;
                if (ce.Line - 1 >= 0)
                    error += "   " + codeLines[ce.Line - 1] + Environment.NewLine;
                error += "   " + "^".PadLeft(ce.Column) + Environment.NewLine;
            }

            return new CompilerException(error);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

    public static class ModCompiler
    {
        public static CompilerException Compile(string modDirectory, string outputDirectory)
        {
            CommonToolUtilities.RefreshHashes();

            string modName = Path.GetDirectoryName(modDirectory);

            string jsonFile, modInfo;

            CodeDomProvider cdcp = new CSharpCodeProvider();
            string ext = "*.cs";

            #region validating ModInfo.json
            jsonFile = modDirectory + "\\ModInfo.json";

            if (!File.Exists(jsonFile))
            {
                File.WriteAllText(jsonFile, "{\"name\":\"" + modName + "\",\"author\":\"<unknown>\"}");
                Console.WriteLine("Warning: You do not have a ModInfo.json file.\n\tUsing the default ModInfo...");
            }

            try
            {
                JsonData json = JsonMapper.ToObject(File.ReadAllText(jsonFile));

                if (!json.Has("name"))
                    throw new CompilerException("Missing ModInfo filed 'name'");
                else
                    modName = (string)json["name"];

                if (!json.Has("author"))
                    throw new CompilerException("Missing ModInfo filed 'author'");

                if (json.Has("code"))
                {
                    JsonData jCode = json["code"];
                    if (jCode.Has("codeType"))
                        switch (jCode["codeType"].ToString().ToLower().Trim())
                        {
                            case "vb":
                            case "vb.net":
                            case "basic":
                            case "basic.net":
                            case "vbasic":
                            case "vbasic.net":
                            case "visualbasic":
                            case "visualbasic.net":
                            case "visual basic":
                            case "visual basic.net":
                                cdcp = new VBCodeProvider();
                                ext = "*.vb";
                                Console.WriteLine("Using Visual Basic.NET (VBCodeDomProvider)...");
                                break;
                            case "js":
                            case "js.net":
                            case "jscript":
                            case "jscript.net":
                            case "javascript":
                            case "javascript.net":
                                cdcp = new JScriptCodeProvider();
                                ext = "*.js";
                                Console.WriteLine("Using JScript.NET (JScriptCodeProvider)...");
                                break;
                            case "cs":
                            case "c#":
                            case "csharp":
                            case "visual cs":
                            case "visual c#":
                            case "visual csharp":
                                // inited as C#
                                Console.WriteLine("Using C# (CSharpCodeProvider)...");
                                break;
                            default:
                                Console.WriteLine("Language not explicitely defined, using C# (CSharpCodeProvider)...");
                                break;
                        }
                }

                //if (!json.Has("version"))
                //    throw new CompileException("Missing ModInfo field 'version'");
                //if (!json.Has("info"))
                //    throw new CompileException("Missing ModInfo field 'info'");

                modInfo = (string)json;
            }
            catch (Exception e)
            {
                throw new CompilerException("Invalid file: ModInfo.json", e);
            }
            #endregion

            #region compile the code
            List<string> toCompile = new List<string>();
            foreach (string file in Directory.EnumerateFiles(modDirectory, ext, SearchOption.AllDirectories))
                toCompile.Add(file);

            CompilerParameters cp = new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = false,

                OutputAssembly = outputDirectory + "\\" + modName + ".tapimod"
            };

            string
                xna = Environment.GetEnvironmentVariable("XNAGSv4") + "\\References\\Windows\\x86\\",
                wpf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                    + "\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.0\\Profile\\Client\\",
                here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            cp.ReferencedAssemblies.Add("tAPI.exe");
            cp.ReferencedAssemblies.Add("Microsoft.JScript.dll");
            cp.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
            cp.ReferencedAssemblies.Add("Microsoft.CSharp.dll");

            cp.ReferencedAssemblies.Add("Accessibility.dll");
            cp.ReferencedAssemblies.Add("mscorlib.dll");
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add("System.Drawing.dll");
            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            cp.ReferencedAssemblies.Add("System.Numerics.dll");
            cp.ReferencedAssemblies.Add("System.Xml.dll");

            cp.ReferencedAssemblies.Add(wpf + "PresentationCore.dll");
            cp.ReferencedAssemblies.Add(wpf + "PresentationFramework.dll");
            cp.ReferencedAssemblies.Add(wpf + "WindowsBase.dll");

            cp.ReferencedAssemblies.Add(xna + "Microsoft.Xna.Framework.dll");
            cp.ReferencedAssemblies.Add(xna + "Microsoft.Xna.Framework.Xact.dll");
            cp.ReferencedAssemblies.Add(xna + "Microsoft.Xna.Framework.Game.dll");
            cp.ReferencedAssemblies.Add(xna + "Microsoft.Xna.Framework.Graphics.dll");

            CompilerResults cr = cdcp.CompileAssemblyFromFile(cp, toCompile.ToArray());

            if (cr.Errors.HasErrors)
                return CompilerException.CreateException(cr.Errors);
            #endregion

            #region save to .tapimod file
            /*
             * How a .tapimod file looks like:
             * 
             *   - version (uint)
             * 
             *   - modinfo (string)
             * 
             *   - file amount (int)
             * 
             *    files: 
             *     - file name (string)
             *     - file data length (int)
             *   
             *    files:
             *     - file data (byte[])
             *   
             *   - assembly data
             */

            // VBCodeProvider automatically adds '.dll'
            string mod = outputDirectory + (cdcp is VBCodeProvider ? ".tapimod.dll" : ".tapimod");

            List<Tuple<string, byte[]>> files = new List<Tuple<string, byte[]>>();
            foreach (string fileName in Directory.EnumerateFiles(modDirectory, "*.*", SearchOption.AllDirectories))
                if (!Path.GetExtension(fileName).EndsWith(ext.Substring(2)))
                    files.Add(new Tuple<string, byte[]>(fileName.Substring(modDirectory.Length + 1).Replace('\\', '/'), File.ReadAllBytes(fileName)));

            BinBuffer bb = new BinBuffer();

            bb.Write(Constants.versionAssembly);

            bb.Write(modInfo);

            bb.Write(files.Count);
            foreach (Tuple<string, byte[]> pfile in files)
            {
                bb.Write(pfile.Item1);
                bb.Write(pfile.Item2.Length);
            }
            foreach (Tuple<string, byte[]> pfile in files)
                bb.Write(pfile.Item2);

            bb.Pos = 0;
            File.WriteAllBytes(cdcp is VBCodeProvider ? Path.ChangeExtension(mod, null) : mod, bb.ReadBytes(bb.GetSize()));

            // generate false hashes
            CommonToolUtilities.AddHashes(modName, modDirectory);
            #endregion

            return null;
        }
    }
}

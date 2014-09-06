using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using LitJson;
using Ionic.Zip;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Compiler;

namespace PoroCYon.MCT.Tools.Internal.Compiler
{
    class Writer(ModCompiler mc) : CompilerPhase(mc)
    {
        internal IEnumerable<CompilerError> Write()
        {
            List<CompilerError> errors = new List<CompilerError>();

            BinBuffer bb = new BinBuffer();

            Compiler.Log("Writing header.", MessageImportance.Low);
            bb.Write(API.versionAssembly);
            bb.Write(Building.JSONs[0].Json.ToJson());
            bb.Write(Building.Files.Count + (Building.Info.includePDB ? 1 : 0));

            byte[] pdb = null;

            if (Building.Info.includePDB)
                try
                {
                    pdb = File.ReadAllBytes(Path.ChangeExtension(Building.Assembly.Location, ".pdb"));
                }
                catch { }

            if (pdb != null)
            {
                Compiler.Log("Got PDB file.", MessageImportance.Low);
                bb.Write("DebugInformation.pdb");
                bb.Write(pdb.Length);
            }
            //Uri baseUri = new Uri(File.Exists(mod.OriginPath) ? Path.GetDirectoryName(mod.OriginPath) : mod.OriginPath);

            //List<Tuple<string, byte[]>> jsons = new List<Tuple<string, byte[]>>();
            //for (int i = 1; i < mod.JSONs.Count; i++)
            //{
            //    if (mod.JSONs[i] == null)
            //        continue;

            //    Uri relative =  baseUri.MakeRelativeUri(new Uri(mod.JSONs[i].Path));
            //    jsons.Add
            //    (
            //        var t = new Tuple<string, byte[]>
            //        (
            //            relative.OriginalString,
            //            Encoding.UTF8.GetBytes(JsonMapper.ToJson(mod.JSONs[i].Json))
            //        )
            //    );

            //    bb.Write(jsons[jsons.Count - 1].Item1       );
            //    bb.Write(jsons[jsons.Count - 1].Item2.Length);
            //}
            Compiler.Log("Writing file headers.", MessageImportance.Low);
            foreach (KeyValuePair<string, byte[]> current in Building.Files)
            {
                bb.Write(current.Key);
                bb.Write(current.Value.Length);
            }

            if (pdb != null)
                bb.Write(pdb);
            Compiler.Log("Writing files.", MessageImportance.Low);
            //for (int i = 0; i < jsons.Count; i++)
            //    bb.Write(jsons[i].Item2);
            foreach (KeyValuePair<string, byte[]> current in Building.Files)
                bb.Write(current.Value);

            Compiler.Log("Writing assembly.", MessageImportance.Low);
            bb.Write(File.ReadAllBytes(Building.Assembly.Location));
            bb.Pos = 0;

            string outputFile = CommonToolUtilities.modsBinDir + "\\" + Building.Info.outputName + (Building.Info.compress ? ".tapi" : ".tapimod");

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            if (Building.Info.compress)
                using (ZipFile zf = new ZipFile())
                {
                    Compiler.Log("Compressing output.", MessageImportance.Low);
                    if (!Directory.Exists(CommonToolUtilities.modsBinDir))
                        Directory.CreateDirectory(CommonToolUtilities.modsBinDir);

                    zf.AddEntry("Mod.tapimod", bb.ReadBytes(bb.GetSize()));
                    zf.AddEntry("ModInfo.json", Encoding.UTF8.GetBytes(Building.JSONs[0].Json.ToJson()));
                    if (pdb != null)
                        zf.AddEntry("DebugInformation.pdb", pdb);

                    foreach (KeyValuePair<string, byte[]> current in Building.Files)
                        zf.AddEntry(current.Key, current.Value);

                    Compiler.Log("Writing compressed output to disk.", MessageImportance.Low);
                    zf.Save(outputFile);
                }
            else
            {
                Compiler.Log("Writing uncompressed output to disk.", MessageImportance.Low);
                File.WriteAllBytes(outputFile, bb.ReadBytes(bb.GetSize()));
            }

            if (Building.Info.extractDLL)
            {
                string dll = Path.ChangeExtension(outputFile, ".dll");

                if (File.Exists(dll))
                    File.Delete(dll);

                File.Copy(Building.Assembly.Location, dll);

                File.WriteAllBytes(Path.ChangeExtension(outputFile, ".pdb"), pdb);
            }

            return errors;
        }
    }
}

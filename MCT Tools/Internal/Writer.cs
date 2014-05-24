using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using TAPI;
using PoroCYon.MCT.Internal;

namespace PoroCYon.MCT.Tools.Internal
{
    static class Writer
    {
        internal static IEnumerable<CompilerError> Write(ModData mod)
        {
            List<CompilerError> errors = new List<CompilerError>();

            BinBuffer bb = new BinBuffer();

            bb.Write(API.versionAssembly);
            bb.Write(mod.JSONs[0].Json.ToJson());
            bb.Write(mod.Files.Count + (mod.Info.includePDB ? 1 : 0));

            byte[] pdb = null;

            if (mod.Info.includePDB)
                try
                {
                    pdb = File.ReadAllBytes(Path.ChangeExtension(mod.Assembly.Location, ".pdb"));
                }
                catch { }

            if (pdb != null)
            {
                bb.Write("DebugInformation.pdb");
                bb.Write(pdb.Length);
            }
            foreach (KeyValuePair<string, byte[]> current in mod.Files)
            {
                bb.Write(current.Key);
                bb.Write(current.Value.Length);
            }

            if (pdb != null)
                bb.Write(pdb);
            foreach (KeyValuePair<string, byte[]> current in mod.Files)
                bb.Write(current.Value);

            bb.Write(File.ReadAllBytes(mod.Assembly.Location));
            bb.Pos = 0;

            string outputFile = CommonToolUtilities.modsBinDir + "\\" + mod.Info.internalName + (mod.Info.compress ? ".tapi" : ".tapimod");

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            if (mod.Info.compress)
                using (ZipFile zf = new ZipFile())
                {
                    if (!Directory.Exists(CommonToolUtilities.modsBinDir))
                        Directory.CreateDirectory(CommonToolUtilities.modsBinDir);

                    zf.AddEntry("Mod.tapimod", bb.ReadBytes(bb.GetSize()));
                    zf.AddEntry("ModInfo.json", Encoding.UTF8.GetBytes(mod.JSONs[0].Json.ToJson()));
                    if (pdb != null)
                        zf.AddEntry("DebugInformation.pdb", pdb);

                    foreach (KeyValuePair<string, byte[]> current in mod.Files)
                        zf.AddEntry(current.Key, current.Value);

                    zf.Save(outputFile);
                }
            else
                File.WriteAllBytes(outputFile, bb.ReadBytes(bb.GetSize()));

            if (mod.Info.extractDLL)
            {
                string dll = Path.ChangeExtension(outputFile, ".dll");

                if (File.Exists(dll))
                    File.Delete(dll);

                File.Copy(mod.Assembly.Location, dll);

                File.WriteAllBytes(Path.ChangeExtension(outputFile, ".pdb"), pdb);
            }

            return errors;
        }
    }
}

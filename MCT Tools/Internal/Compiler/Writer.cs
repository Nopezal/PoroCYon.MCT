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
using PoroCYon.Extensions.Collections;

namespace PoroCYon.MCT.Tools.Internal.Compiler
{
	class Writer : CompilerPhase
    {
        readonly static string PNG = ".png";

		static void WritePrefixedArray(BinBuffer bb, byte[] toWrite)
		{
			bb.Write(toWrite.Length);
			bb.Write(toWrite);
		}

        public Writer(ModCompiler mc)
            : base(mc)
        {

        }

		/*
		 * .tapimod file format:
		 *
		 * - mod version (int)
		 * - modinfo (string)
		 * - icon
		 *   ?
		 *     - icon size (in bytes) (int)
		 *     - icon data (byte[])
		 *   :
		 *     - 0 (int)
		 *
		 * - assembly size (in bytes) (int)
		 * - assembly data (byte[])
		 *
		 * - image count (ushort)
		 * - [for each image]
		 *     - path (string)
		 *     - image size (bytes) (int)
		 *     - image content (byte[])

		 * - file count (ushort)
		 *   [for each file]
		 *     - path (string)
		 *     - file size (bytes) (int)
		 *     - file content (byte[])
		 *
		 **/

		string[] FindImages()
		{
			// screw tAPI code, I'm doing it the functional way.
			return (from kvp in Building.Files where Path.GetExtension(kvp.Key).ToLowerInvariant() == PNG select kvp.Key).ToArray();
		}
		string[] FindFiles ()
		{
			return (from kvp in Building.Files where Path.GetExtension(kvp.Key).ToLowerInvariant() != PNG select kvp.Key).ToArray();
		}

		internal IEnumerable<CompilerError> Write()
        {
            List<CompilerError> errors = new List<CompilerError>();

            BinBuffer bb = new BinBuffer(131072);  // no idea why it's 131072, but w/e

			Compiler.Log("Writing header.", MessageImportance.Low);
            bb.Write((ushort)ModCompile.MOD_VERSION);
            bb.Write(Building.JSONs[0].Json.ToJson());

			if (Building.Info.icon == null)
				bb.Write(0);
			else
			{
				Compiler.Log("Writing icon file.", MessageImportance.Low);

				WritePrefixedArray(bb, Building.Files[Building.Info.icon + ".png"]);
			}

			Compiler.Log("Writing assembly.", MessageImportance.Low);
			WritePrefixedArray(bb, File.ReadAllBytes(Building.Assembly.Location));

			Compiler.Log("Writing images.", MessageImportance.Low);
			string[] arr = FindImages();
			bb.Write((ushort)arr.Length); // so long ago that I typed Length, I typed 'Count' first...
			for (int i = 0; i < arr.Length; i++)
			{
				bb.Write(Path.ChangeExtension(arr[i], null));
				WritePrefixedArray(bb, Building.Files[arr[i]]);
			}

            Compiler.Log("Writing files.", MessageImportance.Low);
			arr = FindFiles();
			bb.Write((ushort)(arr.Length + (Building.Info.includePDB ? 1 : 0)));
			for (int i = 0; i < arr.Length; i++)
			{
				bb.Write(arr[i]);
				WritePrefixedArray(bb, Building.Files[arr[i]]);
			}

			byte[] pdb = null;

			if (Building.Info.includePDB)
				try
				{
					pdb = File.ReadAllBytes(Path.ChangeExtension(Building.Assembly.Location, ".pdb"));
				}
				catch { }

            // write the PDB output

            if (!Directory.Exists(Mods.pathCompiled))
                Directory.CreateDirectory(Mods.pathCompiled);
            if (!Directory.Exists(Mods.pathPDB     ))
                Directory.CreateDirectory(Mods.pathPDB     );
            File.WriteAllBytes(Mods.pathCompiled + "\\" + Building.Info.outputName + ".pdb", pdb);
            File.WriteAllBytes(Mods.pathPDB      + "\\" + Building.Info.outputName + ".pdb", pdb);

            if (pdb != null)
			{
				Compiler.Log("Writing PDB file.", MessageImportance.Low);

				bb.Write("DebugInformation.pdb");
				WritePrefixedArray(bb, pdb);
			}

			bb.Pos = 0;
			string outputFile = Mods.pathCompiled + "\\" + Building.Info.outputName + (Building.Info.compress ? ".tapi" : ".tapimod");

			if (File.Exists(outputFile))
				File.Delete(outputFile);

			if (Building.Info.compress)
				using (ZipFile zf = new ZipFile())
				{
					Compiler.Log("Compressing output.", MessageImportance.Low);
					if (!Directory.Exists(Mods.pathCompiled))
						Directory.CreateDirectory(Mods.pathCompiled);

					zf.AddEntry("!Mod.tapimod", bb.ReadBytes(bb.Size));
					zf.AddEntry("ModInfo.json", Encoding.UTF8.GetBytes(Building.JSONs[0].Json.ToJson()));
					if (pdb != null)
						zf.AddEntry("!DebugInformation.pdb", pdb);

                    if (Building.Info.includeFiles)
					    foreach (KeyValuePair<string, byte[]> current in Building.Files)
						    zf.AddEntry(current.Key, current.Value);

					Compiler.Log("Writing compressed output to disk.", MessageImportance.Low);
					zf.Save(outputFile);
				}
			else
			{
				Compiler.Log("Writing uncompressed output to disk.", MessageImportance.Low);
				File.WriteAllBytes(outputFile, bb.ReadBytes());
			}

			if (Building.Info.extractDLL)
			{
				string dll = Path.ChangeExtension(outputFile, ".dll");

				if (File.Exists(dll))
					File.Delete(dll);

				File.Copy(Building.Assembly.Location, dll);

				File.WriteAllBytes(Path.ChangeExtension(dll, ".pdb"), pdb);
			}

            return errors;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TAPI.SDK.Internal;

namespace TAPI.SDK.ModBuilder
{
    public static class ModCompiler
    {
        public static void Compile(string dllFile)
        {
            CommonToolUtilities.RefreshHashes();

            #region setup
            string
                modName = Path.GetFileNameWithoutExtension(dllFile),
                modPath = CommonToolUtilities.modsSrcDir + "\\" + modName,
                modFile = CommonToolUtilities.modsBinDir + "\\" + modName + ".tapimod";

            if (File.Exists(modFile))
                File.Delete(modFile);

            Assembly asm = Assembly.LoadFrom(dllFile);

            List<Tuple<string, byte[]>> files = new List<Tuple<string, byte[]>>();
            #endregion

            string modInfo = null;

            #region load all resources into the list
            foreach (string res in asm.GetManifestResourceNames())
                using (Stream stream = asm.GetManifestResourceStream(res))
                {
                    MemoryStream ms = new MemoryStream();
                    stream.CopyTo(ms);

                    bool foundExt = false;
                    int index = 0;

                    for (int i = res.Length - 1; i >= 0; i--)
                    {
                        // find file name
                        if (res[i] == '.')
                            if (foundExt)
                            {
                                index = i + 1;
                                break;
                            }
                            else
                                foundExt = true;
                    }

                    // not putting ModInfo.json in it
                    if (res.Substring(index) != "ModInfo.json")
                        files.Add(new Tuple<string, byte[]>(res.Substring(index), ms.ToArray()));
                    else
                    {
                        StreamReader r = new StreamReader(new MemoryStream(ms.ToArray()));
                        modInfo = r.ReadToEnd();

                        if (modInfo == null)
                            modInfo = "{\n\t\"displayName\": \"" + modName + "\",\n\t\"author\": \"<unknown>\"\n\t\"info\": \"\"\n}";
                        else
                            try
                            {
                                JsonData j = JsonMapper.ToObject(modInfo);

                                if (!j.Has("displayName") || !j.Has("author"))
                                    modInfo = "{\n\t\"displayName\": \"" + modName + "\",\n\t\"author\": \"<unknown>\"\n\t\"info\": \"\"\n}";
                            }
                            catch (JsonException) // invalid JSON thrown in JsonMapper.ToObject
                            {
                                modInfo = "{\n\t\"displayName\": \"" + modName + "\",\n\t\"author\": \"<unknown>\"\n\t\"info\": \"\"\n}";
                            }

                        r.Close();
                    }
                    ms.Dispose();
                }
            #endregion

            File.Copy(dllFile, modFile);

            #region write data
            BinBuffer bb = new BinBuffer();

            // write version
            bb.Write(Constants.versionAssembly);

            // write modinfo
            bb.Write(modInfo);

            // write file name + length
            bb.Write(files.Count);
            foreach (Tuple<string, byte[]> pfile in files)
            {
                bb.Write(pfile.Item1);
                bb.Write(pfile.Item2.Length);
            }

            // write file data
            foreach (Tuple<string, byte[]> pfile in files)
                    bb.Write(pfile.Item2);

            // write assembly
            bb.Write(File.ReadAllBytes(dllFile));

            // reset
            bb.Pos = 0;

            // write it all to the .tapimod file
            File.WriteAllBytes(modFile, bb.ReadBytes(bb.GetSize()));
            #endregion

            #region generate false folders & files to foul the hash checker, and generate hashes
            // not checked anymore in r4

            /*if (!Directory.Exists(modPath))
                Directory.CreateDirectory(modPath);

            // get info to write to modinfo.cs
            JsonData jModInfo = JsonMapper.ToObject(modInfo);

            string actualModName = modName;
            string codeModBaseName = "TAPI." + actualModName + ".ModBase";

            if (jModInfo.Has("name"))
                codeModBaseName = "TAPI." + (actualModName = (string)jModInfo["name"]) + ".ModBase";

            if (jModInfo != null && jModInfo.Has("code") && jModInfo["code"].Has("modBaseName"))
                codeModBaseName = (string)jModInfo["code"]["modBaseName"];

            // get namespace (and modbase class name)
            string[] nsSplit = codeModBaseName.Split('.');

            string @namespace = nsSplit[0];
            for (int i = 1; i < nsSplit.Length - 1; i++)
                @namespace += "." + nsSplit[i];

            // kinda crucial
            using (FileStream fs = new FileStream(modPath + "\\" + nsSplit[nsSplit.Length - 1] + ".cs", FileMode.Create))
            {
                using (StreamWriter bw = new StreamWriter(fs))
                {
                    bw.Write(
                            "using System;\n" +
                            "using TAPI;\n" +
                            "\n" +
                            "namespace " + @namespace + "\n" +
                            "{\n" +
                            "    public class " + nsSplit[nsSplit.Length - 1] + " : TAPI.ModBase\n" +
                            "    {\n" +
                            "        public " + nsSplit[nsSplit.Length - 1] + "() : base() { }\n" +
                            "    }\n" +
                            "}\n"
                        );
                }
            }

            // write modinfo
            File.WriteAllText(modPath + "\\ModInfo.json", modInfo);

            // write .dll file, you'll never know if...
            File.Copy(dllFile, modPath + "\\" + Path.GetFileName(dllFile), true);

            CommonToolUtilities.AddHashes(modName, modPath);*/
            #endregion
        }
    }
}

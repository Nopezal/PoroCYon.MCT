using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ionic.Zip;
using LitJson;
using TAPI;
using Terraria;

namespace PoroCYon.MCT.Internal
{
    static class MctDebugger
    {
        internal static List<APIModBase> tempBases = new List<APIModBase>();

        static string TrimArg(string arg)
        {
            return arg.TrimStart('-', '/');
        }

        static string GetInternalModName(BinBuffer bb)
        {
            uint vNum = bb.ReadUInt();
            return (string)JsonMapper.ToObject(bb.ReadString())["internalName"];
        }
        static BinBuffer GetBinModData(string fileName)
        {
            if (fileName.ToLower().EndsWith(".tapi") && File.Exists(Mods.pathDirModsUnsorted + "/" + fileName))
                using (ZipFile zf = ZipFile.Read(Mods.pathDirModsUnsorted + "/" + fileName))
                {
                    ZipEntry ze = zf["Mod.tapimod"];
                    if (ze != null)
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ze.Extract(ms);
                            return new BinBuffer(new BinBufferByte(ms.ToArray()));
                        }
                }

            if (fileName.ToLower().EndsWith(".tapimod") && File.Exists(Mods.pathDirModsUnsorted + "/" + fileName))
                return new BinBuffer(new BinBufferByte(File.ReadAllBytes(Mods.pathDirModsUnsorted + "/" + fileName)));

            return null;
        }
        static BinBuffer FindMod(string internalName, out string fileName)
        {
            var list = Mods.LoadModState(Mods.pathDirModsUnsorted);
            fileName = null;

            for (int i = 0; i < list.Count; i++)
            {
                string[] split = list[i].Split('\\', '/');
                fileName = split[split.Length - 1];

                try
                {
                    BinBuffer bb = GetBinModData(fileName);

                    if (bb != null && GetInternalModName(bb) == internalName)
                    {
                        bb.Pos = 0;
                        return bb;
                    }
                }
                catch { } // welp
            }

            return null;
        }

        static void LoadDebugMod(APIModBase amb, string pathToAsm)
        {
            string fileName;
            BinBuffer bb = FindMod(amb.modName, out fileName);

            if (bb == null)
                throw new FileNotFoundException("The mod with the given internal name is not found.");

            tempBases.Add(amb); // remove temporary modbase instance after all mods have loaded

            // ---

            uint version = bb.ReadUInt();

            string _mi = bb.ReadString();
            JsonData modInfo = JsonMapper.ToObject(_mi);

            string internalName = (string)modInfo["internalName"];

            //if (!modInfo.Has("includePDB") || !(bool)modInfo["includePDB"])
            //    throw new InvalidOperationException("The .pdb file must be included in order to debug the mod!");

            List<Tuple<string, byte[]>> fileData = new List<Tuple<string, byte[]>>();
            List<Tuple<string, int>> fileHeader = new List<Tuple<string, int>>();
            int fileCount = bb.ReadInt();

            while (fileCount-- > 0)
                fileHeader.Add(new Tuple<string, int>(bb.ReadString(), bb.ReadInt()));

            foreach (Tuple<string, int> t in fileHeader)
                fileData.Add(new Tuple<string, byte[]>(t.Item1, bb.ReadBytes(t.Item2)));

            Assembly asm = Assembly.LoadFile(pathToAsm);

            Type[] bases = ReflectionHelper.FindSubclasses(asm, typeof(ModBase));
            if (bases.Length > 1)
                throw new Mods.LoadException("Multiple ModBases found");

            ModBase @base = (bases.Length == 1) ? ((ModBase)ReflectionHelper.Instantiate(bases[0], null)) : new ModBase();

            if (@base != null)
            {
                @base.code = asm;
                @base.fileName = fileName;
                @base.modName = (string)modInfo["internalName"];
                @base.modInfo = new ModInfo(modInfo);

                foreach (Tuple<string, byte[]> f in fileData)
                    @base.files.Add(f.Item1, f.Item2);

                if (modInfo.Has("dllReferences"))
                {
                    JsonData jsonData2 = modInfo["dllReferences"];
                    for (int i = 0; i < jsonData2.Count; i++)
                    {
                        string @ref = (string)jsonData2[i];

                        if (@base.files.ContainsKey(@ref))
                        {
                            Assembly item = Assembly.Load(@base.files[@ref]);
                            Mods.dlls.Add(item);
                            @base.dlls.Add(item);
                        }
                        else
                            try
                            {
                                Assembly.Load(AssemblyName.GetAssemblyName(@ref));
                            }
                            catch
                            {
                                Mods.dlls.Add(Assembly.LoadFrom(@ref));
                            }
                    }
                }

                @base.modIndex = Mods.loadOrder.Count;

                if (!Mods.loadOrder.Contains(fileName))
                    Mods.loadOrder.Add(fileName);

                for (int i = 0; i < Mods.modBases.Count; i++)
                {
                    if (Mods.modBases[i].modName == @base.modName)
                        Mods.modBases[i] = @base;
                    else if (i == Mods.modBases.Count - 1)
                        Mods.modBases.Add(@base);
                }

                Type[] players = ReflectionHelper.FindSubclasses(asm, typeof(ModPlayer));
                for (int i = 0; i < players.Length; i++)
                    @base.modPlayers.Add((ModPlayer)ReflectionHelper.Instantiate(players[i], new [] { typeof(ModBase), typeof(Player) }, new [] { @base, null }));

                Type[] worlds = ReflectionHelper.FindSubclasses(asm, typeof(ModWorld));
                for (int i = 0; i < worlds.Length; i++)
                    @base.modWorlds.Add((ModWorld)ReflectionHelper.Instantiate(worlds[i], new [] { typeof(ModBase) }, new [] { @base }));

                Type[] uis = ReflectionHelper.FindSubclasses(asm, typeof(ModInterface));
                for (int i = 0; i < uis.Length; i++)
                    @base.modInterfaces.Add((ModInterface)ReflectionHelper.Instantiate(uis[i], new [] { typeof(ModBase) }, new [] { @base }));

                Type[] pfixes = ReflectionHelper.FindSubclasses(asm, typeof(ModPrefix));
                for (int i = 0; i < pfixes.Length; i++)
                    @base.modPrefixes.Add((ModPrefix)ReflectionHelper.Instantiate(pfixes[i], new [] { typeof(ModBase) }, new [] { @base }));

                Type[] items = ReflectionHelper.FindSubclasses(asm, typeof(ModItem));
                for (int i = 0; i < items.Length; i++)
                    if (items[i].IsDefined(typeof(GlobalModAttribute), true))
                        @base.modItems.Add((ModItem)ReflectionHelper.Instantiate(items[i], new [] { typeof(ModBase), typeof(Item) }, new [] { @base, null }));

                Type[] npcs = ReflectionHelper.FindSubclasses(asm, typeof(ModNPC));
                for (int i = 0; i < npcs.Length; i++)
                    if (npcs[i].IsDefined(typeof(GlobalModAttribute), true))
                        @base.modNPCs.Add((ModNPC)ReflectionHelper.Instantiate(npcs[i], new [] { typeof(ModBase), typeof(NPC) }, new object[] { @base, null }));

                Type[] projs = ReflectionHelper.FindSubclasses(asm, typeof(ModProjectile));
                for (int i = 0; i < projs.Length; i++)
                    if (projs[i].IsDefined(typeof(GlobalModAttribute), true))
                        @base.modProjectiles.Add((ModProjectile)ReflectionHelper.Instantiate(projs[i], new [] { typeof(ModBase), typeof(Projectile) }, new [] { @base, null }));

                Type[] tiles = ReflectionHelper.FindSubclasses(asm, typeof(ModTileType));
                for (int i = 0; i < tiles.Length; i++)
                    if (tiles[i].IsDefined(typeof(GlobalModAttribute), true) && !tiles[i].IsSubclassOf(typeof(ModTile)))
                        @base.modTileTypes.Add((ModTileType)ReflectionHelper.Instantiate(tiles[i], new [] { typeof(ModBase) }, new [] { @base }));

                ModsLoadContent.Load(asm, @base);
                @base.OnLoad();

                // increase timesLoade
                Type t = typeof(ModBase);
                FieldInfo fi = t.GetField("timesLoaded",
                    BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

                // >:D
                fi.SetValue(@base, (int)fi.GetValue(@base) + 1);
            }
            else
                throw new Mods.LoadException("Failed loading ModBase");
        }

        internal static void LoadDebugMods()
        {
            // Hacky stuff #ilostthecount
            // remove a mod (.tapi(mod)) from the mod list, and insert the mod from the .dll

            string[] args = Environment.GetCommandLineArgs();

            for (int i = 1; i < args.Length; i++)
                switch (TrimArg(args[i]).ToUpperInvariant())
                {
                    case "DEBUG":

                        if (i >= args.Length - 2)
                            throw new FormatException("Incorrect DEBUG arguments." + Environment.NewLine +
                                                      "The correct syntax is: -DEBUG <internal mod name> <path to assembly>." + Environment.NewLine +
                                                      "Eg. -DEBUG \"Internal mod name\" \"C:\\Path\\To\\Assembly.dll\".");

                        if (!File.Exists(args[i + 2]))
                            throw new FileNotFoundException("The assembly to debug (" + args[i + 2] + ") was not found.");

                        // tAPI dev team bad idea #1 (making it public)
                        APIModBase amb = new APIModBase()
                        {
                            modName = args[i + 1] // internal name of the mod to debug
                        };

                        // set timesLoaded (internal field)
                        Type t = typeof(ModBase);
                        FieldInfo fi = t.GetField("timesLoaded",
                            BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

                        // >:D
                        fi.SetValue(amb, 1);

                        LoadDebugMod(amb, args[i + 2]);

                        break;
                }
        }
    }
}

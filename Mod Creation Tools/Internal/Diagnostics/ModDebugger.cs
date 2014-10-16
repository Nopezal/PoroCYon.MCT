using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ionic.Zip;
using LitJson;
using Terraria;
using TAPI;
using PoroCYon.MCT.ModControlling;

namespace PoroCYon.MCT.Internal.Diagnostics
{
	// Hacky stuff #ilostthecount
	// remove a mod (.tapi(mod)) from the mod list, and insert the mod from the .dll, so it can be debugged using VS.

	static class ModDebugger
	{
		const string DEBUG_SWITCH = "DEBUG";

        static bool? cachedShouldDebug = null;

		readonly static string
            DEBUG_SWITCH_INST   = DEBUG_SWITCH,
			DEBUG_HELP_TEXT     = "-DEBUG <internal mod name> <path to assembly>.",
            INVALID_ARGS_SYNTAX = "Incorrect DEBUG arguments." + Environment.NewLine +
								   "The correct syntax is: " + DEBUG_HELP_TEXT + Environment.NewLine +
								   "Eg. -DEBUG \"Me.MyMod\" \"C:\\Path\\To\\Assembly.dll\".";

        internal static bool ShouldDebug
        {
            get
            {
                return cachedShouldDebug ?? (cachedShouldDebug = Environment.CommandLine.Split(' ').Any(s => s.TrimStart('-', '/').ToUpperInvariant() == DEBUG_SWITCH_INST)).Value;
            }
        }

		static string TrimArg(string arg)
		{
			return arg.TrimStart('-', '/');
		}

		// modified versions of ModController.LoadMod/LoadModInternal
		static void LoadModInternal(Mod      mod, ModBase  @base)
		{
			ModController.CheckModBaseAndInfo(mod, @base);

			Mods.mods.Add(mod);

			ModController.SetupNoContent(mod);

			@base.OnLoad();

			@base.SetTimesLoaded(@base.GetTimesLoaded() + 1);
		}
		static void LoadMod        (Mod      mod, ModBase  @base)
		{
			mod.enabled = true;

			mod.modBase = @base;
			@base.mod = mod;

			LoadModInternal(mod, @base);
		}
		static void LoadMod        (Assembly asm, JsonData info , Texture2D icon)
		{
			Mod m = new Mod(asm.Location);

			ModBase @base = ModController.InstantiateAndReturnTypes<ModBase>(asm).FirstOrDefault() ?? new ModBase();

			m.SetModInfo(info);
			m.SetIcon(icon);

			ModController.LoadClasses(m);

			LoadMod(m, @base);
		}

		internal static IEnumerable<Tuple<string, string>> GetModsToDebug()
		{
			List<Tuple<string, string>> ret = new List<Tuple<string, string>>();

			if (!Debugger.IsAttached || !ShouldDebug)
				return ret;

			string[] args = Environment.GetCommandLineArgs();

			for (int i = 0; i < args.Length; i++)
				switch (TrimArg(args[i].ToUpperInvariant()))
				{
					case DEBUG_SWITCH:
						if (i == args.Length - 1)
							throw new FormatException(INVALID_ARGS_SYNTAX);
						if (i == args.Length - 2)
						{
							if ((string arg = TrimArg(args[i + 1].ToUpperInvariant())) == "HELP" || arg == "?")
							{
								Console.WriteLine(DEBUG_HELP_TEXT);
								Debug  .WriteLine(DEBUG_HELP_TEXT);
							}
							else
								throw new FormatException(INVALID_ARGS_SYNTAX);
						}
						else
							ret.Add(new Tuple<string, string>(args[++i], args[++i])); // built-in tuples would be nice...
						break;
				}

		return ret;
	}

		internal static void DebugMod (Tuple<string, string> modIdent)
		{
			if (!Debugger.IsAttached || !ShouldDebug)
				return;

			string
				internalName = modIdent.Item1,
				asmPath      = modIdent.Item2;

			Mod old;
			if ((old = Mods.GetMod(internalName)) == null)
				throw new InvalidOperationException("Mod " + internalName + " is nowhere to be found.");
			if (!File.Exists(asmPath))
				throw new FileNotFoundException("Assembly " + asmPath + " not found.");

			Assembly asm = Assembly.LoadFrom(asmPath);

			JsonData  info = old.ModInfo;
			Texture2D icon = old.Icon   ;

			old.Unload(); // don't need to unload content objects, they're kept alive. code is late-bound, don't have to change that either.
			
			LoadMod(asm, info, icon);
		}
		internal static void DebugMods()
		{
            if (!ShouldDebug)
                return;

			if (!Debugger.IsAttached)
			{
				TConsole.Track("NO DEBUGGER ATTACHED", Color.Red, "MCT Debugger", 900);

				return;
			}

			foreach (var t in GetModsToDebug())
				DebugMod(t);
		}
	}

    /*static class _ModDebugger
    {
        internal static List<APIModBase           > tempBases = new List<APIModBase           >();
        internal static List<Tuple<string, string>> toDebug   = new List<Tuple<string, string>>();

        static string TrimArg(string arg)
        {
            return arg.TrimStart('-', '/');
        }

        static string GetInternalModName(BinBuffer bb, out uint vNum)
        {
            vNum = bb.ReadUShort();
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

                    if (bb != null && GetInternalModName(bb, out uint vNum) == internalName && vNum == API.versionRelease)
                    {
                        bb.Pos = 0;
                        return bb;
                    }
                }
                catch { } // welp
            }

            return null;
        }

        static ModBase LoadMod(BinBuffer bb, string pathToAsm, string fileName)
        {
            uint version = bb.ReadUInt();

            string _mi = bb.ReadString();
            JsonData modInfo = JsonMapper.ToObject(_mi);

            string internalName = (string)modInfo["internalName"];

            if (!modInfo.Has("includePDB") || !(bool)modInfo["includePDB"])
                throw new Mods.LoadException("The .pdb file must be included in order to debug the mod!");

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
                throw new Mods.LoadException("Multiple ModBases found.");

            try
            {
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

                    #region load mod components
                    Type[] players = ReflectionHelper.FindSubclasses(asm, typeof(ModPlayer));
                    for (int i = 0; i < players.Length; i++)
                        @base.modPlayers.Add((ModPlayer)ReflectionHelper.Instantiate(players[i], new[] { typeof(ModBase), typeof(Player) }, new[] { @base, null }));

                    Type[] worlds = ReflectionHelper.FindSubclasses(asm, typeof(ModWorld));
                    for (int i = 0; i < worlds.Length; i++)
                        @base.modWorlds.Add((ModWorld)ReflectionHelper.Instantiate(worlds[i], new[] { typeof(ModBase) }, new[] { @base }));

                    Type[] uis = ReflectionHelper.FindSubclasses(asm, typeof(ModInterface));
                    for (int i = 0; i < uis.Length; i++)
                        @base.modInterfaces.Add((ModInterface)ReflectionHelper.Instantiate(uis[i], new[] { typeof(ModBase) }, new[] { @base }));

                    Type[] pfixes = ReflectionHelper.FindSubclasses(asm, typeof(ModPrefix));
                    for (int i = 0; i < pfixes.Length; i++)
                        @base.modPrefixes.Add((ModPrefix)ReflectionHelper.Instantiate(pfixes[i], new[] { typeof(ModBase) }, new[] { @base }));

                    Type[] items = ReflectionHelper.FindSubclasses(asm, typeof(ModItem));
                    for (int i = 0; i < items.Length; i++)
                        if (items[i].IsDefined(typeof(GlobalModAttribute), true))
                            @base.modItems.Add((ModItem)ReflectionHelper.Instantiate(items[i], new[] { typeof(ModBase), typeof(Item) }, new[] { @base, null }));

                    Type[] npcs = ReflectionHelper.FindSubclasses(asm, typeof(ModNPC));
                    for (int i = 0; i < npcs.Length; i++)
                        if (npcs[i].IsDefined(typeof(GlobalModAttribute), true))
                            @base.modNPCs.Add((ModNPC)ReflectionHelper.Instantiate(npcs[i], new[] { typeof(ModBase), typeof(NPC) }, new object[] { @base, null }));

                    Type[] projs = ReflectionHelper.FindSubclasses(asm, typeof(ModProjectile));
                    for (int i = 0; i < projs.Length; i++)
                        if (projs[i].IsDefined(typeof(GlobalModAttribute), true))
                            @base.modProjectiles.Add((ModProjectile)ReflectionHelper.Instantiate(projs[i], new[] { typeof(ModBase), typeof(Projectile) }, new[] { @base, null }));

                    Type[] tiles = ReflectionHelper.FindSubclasses(asm, typeof(ModTileType));
                    for (int i = 0; i < tiles.Length; i++)
                        if (tiles[i].IsDefined(typeof(GlobalModAttribute), true) && !tiles[i].IsSubclassOf(typeof(ModTile)))
                            @base.modTileTypes.Add((ModTileType)ReflectionHelper.Instantiate(tiles[i], new[] { typeof(ModBase) }, new[] { @base }));
                    #endregion

                    ModsLoadContent.Load(asm, @base);
                    @base.OnLoad();

                    // increase timesLoaded:

                    // get the field
                    FieldInfo fi = typeof(ModBase).GetField("timesLoaded",
						BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

                    // >:D
                    fi.SetValue(@base, (int)fi.GetValue(@base) + 1);

                    return @base;
                }
                else
                    throw new Mods.LoadException("Failed loading ModBase");
            }
            catch (ReflectionTypeLoadException rtle)
            {
                string msg = "A type could not be loaded." + Environment.NewLine;

                if (rtle.LoaderExceptions.Length > 0) // should always be true, but you never know.....
                {
                    msg += rtle.LoaderExceptions[0];

                    for (int i = 1; i < rtle.LoaderExceptions.Length; i++)
                        msg += "\t" + Environment.NewLine + rtle.LoaderExceptions[i];
                }
                else
                    msg += "\tNo additional information was given.";

                throw new FileLoadException(msg, rtle);
            }
        }
        static void LoadDebugMod(APIModBase tempBase, string pathToAsm)
        {
            string fileName;
            BinBuffer bb = FindMod(tempBase.modName, out fileName);

            if (bb == null)
                throw new FileNotFoundException("The mod with the given internal name is not found.");

            tempBases.Add(tempBase); // remove temporary modbase instance after all mods have loaded

            LoadMod(bb, pathToAsm, fileName);
        }

        static void DebugMod(string internalName, string pathToAsm)
        {
            // tAPI dev team bad idea #1 (making it public)
            APIModBase amb = new APIModBase()
            {
                modName = internalName
            };

            // set timesLoaded (internal field)
            FieldInfo fi = typeof(ModBase).GetField("timesLoaded",
                BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

            // >:D
            fi.SetValue(amb, 1);

            LoadDebugMod(amb, pathToAsm);
        }

        internal static void GetModsToDebug()
        {
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

                        toDebug.Add(new Tuple<string, string>(args[i + 1], args[i + 2]));
                        break;
                }
        }
        internal static void TryDebugMod(Assembly asm)
        {
            ModBase @base = Mods.modBases.FirstOrDefault(mb => mb.GetType().Assembly == asm);

            if (@base == null)
                throw new InvalidOperationException("This mod isn't loaded. Mct.Init() must be called from the mod itself.");

            if (!toDebug.Any(t => t.Item1 == @base.modName))
                return; // don't debug this one.

            Trace.WriteLine("Debugging " + @base, "MCT Debugger");
        }
    }*/
}

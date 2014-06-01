using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using Microsoft.Xna.Framework;
using Terraria;
using TAPI;
using PoroCYon.MCT.Internal;

namespace PoroCYon.MCT.Tools.Internal
{
    using Item = Validation.Entities.Item;
    using NPC = Validation.Entities.NPC;
    using Recipe = Validation.Entities.Recipe;

    static class Checker
    {
        //static Game main;

        static void CreateMainAndLoadContent()
        {
            //main = new Main();

            //Main.dedServ = true;
            //Main.showSplash = false;

            //string dir = Environment.CurrentDirectory;
            //Environment.CurrentDirectory = Environment.GetEnvironmentVariable("TAPIBINDIR", EnvironmentVariableTarget.Machine);
            //main.Content.RootDirectory = Environment.CurrentDirectory + "\\Content";
            //typeof(Game).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase).Invoke(main, null);
            //Environment.CurrentDirectory = dir;

            //Lang.setLang(false);

            /*// probably also hacky stuff

            main = new Main();
            Type mType = typeof(Game);

            // from Game.RunGame(bool)

            // graphicsDeviceManager = Services.GetService(typeof(IGraphicsDeviceManager)) as IGraphicsDeviceManager;
            // if (graphicsDeviceManager != null)
            //     graphicsDeviceManager.CreateDevice();
            FieldInfo gdm = mType.GetField("graphicsDeviceManager",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.SetField);

            gdm.SetValue(main, main.Services.GetService(typeof(IGraphicsDeviceManager)) as IGraphicsDeviceManager);
            if (gdm.GetValue(main) != null)
            {
                MethodInfo CreateDevice = typeof(IGraphicsDeviceManager).GetMethod("CreateDevice");

                CreateDevice.Invoke(gdm.GetValue(main), null);
            }

            string dir = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Environment.GetEnvironmentVariable("TAPIBINDIR", EnvironmentVariableTarget.Machine);
            main.Content.RootDirectory = Environment.CurrentDirectory + "\\Content";
            mType.GetMethod("LoadContent", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase).Invoke(main, null);
            Environment.CurrentDirectory = dir;*/
        }
        static void LoadDefs()
        {
            CreateMainAndLoadContent();

            Defs.FillVanillaBuffNames();
            //Defs.FillVanillaBuffs();
            Defs.FillVanillaGores();
            Defs.FillVanillaItems();
            Defs.FillVanillaNPCs();
            Defs.FillVanillaPrefixes();
            Defs.FillVanillaProjectiles();
            Defs.FillVanillaAudio();
            Defs.FillVanillaSounds();
            Defs.FillVanillaNPCShops();
            Defs.FillVanillaCraftingGroups();
        }

        [TargetedPatchingOptOut(Consts.TPOOReason)]
        static void AddIfNotNull(CompilerError err, List<CompilerError> list)
        {
            if (err != null)
                list.Add(err);
        }
        [TargetedPatchingOptOut(Consts.TPOOReason)]
        static void AddIfNotNull(IEnumerable<CompilerError> coll, List<CompilerError> list)
        {
            if (coll != null && coll.Count() != 0)
                foreach (CompilerError e in coll)
                    AddIfNotNull(e, list);
        }

        static CompilerError CheckBuffExists(int buff, string file)
        {
            if (buff < 0 || buff >= Main.maxBuffs)
                return new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = file,
                    IsWarning = true,
                    Message = "Buff ID " + buff + " is not a valid vanilla buff ID. This will cause unwanted behaviour (wrong buff or an exception)."
                };

            return null;
        }
        static CompilerError CheckBuffExists(string buff, string file)
        {
            if (buff.StartsWith("Vanilla:"))
            {
                if (!Defs.buffType.ContainsKey(buff))
                    return new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = file,
                        IsWarning = true,
                        Message = "Buff '" + buff + "' not found."
                    };
            }
            else if (buff.Contains(':'))
            {
                string modName = buff.Substring(buff.IndexOf(':'));

                if (!ModCompiler.current.Info.modReferences.Contains(modName))
                    return new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = file,
                        IsWarning = true,
                        Message = "Buff '" + buff + "' not found because mod '" + modName + "' isn't referenced."
                    };
            }
            else
                return new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = file,
                    IsWarning = true,
                    Message = "Buff '" + buff + "' has an invalid name. Use '<modInternalName>:<buffName>'."
                };

            return null;
        }
        static CompilerError CheckItemExists(int item, string file)
        {
            if (item < 0 || item >= Main.maxItemTypes)
                return new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = file,
                    IsWarning = true,
                    Message = "Item ID " + item + " is not a valid vanilla item ID. This will cause unwanted behaviour (wrong item or an exception)."
                };

            return null;
        }
        static CompilerError CheckItemExists(string item, string file)
        {
            if (item.StartsWith("Vanilla:"))
            {
                if (!Defs.items.ContainsKey(item))
                    return new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = file,
                        IsWarning = true,
                        Message = "Item '" + item + "' not found."
                    };
            }
            else if (item.Contains(':'))
            {
                string modName = item.Substring(item.IndexOf(':'));

                if (!ModCompiler.current.Info.modReferences.Contains(modName))
                    return new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = file,
                        IsWarning = true,
                        Message = "Item '" + item + "' not found because mod '" + modName + "' isn't referenced."
                    };
            }
            else
                return new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = file,
                    IsWarning = true,
                    Message = "Item '" + item + "' has an invalid name. Use '<modInternalName>:<itemName>'."
                };

            return null;
        }
        static CompilerError CheckProjExists(int proj, string file)
        {
            if (proj < 0 || proj >= Main.maxProjectileTypes)
                return new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = file,
                    IsWarning = true,
                    Message = "Projectile ID " + proj + " is not a valid vanilla projectile ID. This will cause unwanted behaviour (wrong projectile or an exception)."
                };

            return null;
        }
        static CompilerError CheckProjExists(string proj, string file)
        {
            if (proj.StartsWith("Vanilla:"))
            {
                if (!Defs.projectiles.ContainsKey(proj))
                    return new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = file,
                        IsWarning = true,
                        Message = "Projectile '" + proj + "' not found."
                    };
            }
            else if (proj.Contains(':'))
            {
                string modName = proj.Substring(proj.IndexOf(':'));

                if (!ModCompiler.current.Info.modReferences.Contains(modName))
                    return new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = file,
                        IsWarning = true,
                        Message = "Projectile '" + proj + "' not found because mod '" + modName + "' isn't referenced."
                    };
            }
            else
                return new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = file,
                    IsWarning = true,
                    Message = "Projectile '" + proj + "' has an invalid name. Use '<modInternalName>:<projName>'."
                };

            return null;
        }
        static CompilerError CheckTileExists(int tile, string file)
        {
            if (tile < 0 || tile >= Main.maxTileSets)
                return new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = file,
                    IsWarning = true,
                    Message = "Tile ID " + tile + " is not a valid vanilla tile ID. This will cause unwanted behaviour (wrong tile or an exception)."
                };

            return null;
        }
        static CompilerError CheckTileExists(string tile, string file)
        {
            if (tile.StartsWith("Vanilla:"))
            {
                if (!Defs.createTile.ContainsKey(tile))
                    return new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = file,
                        IsWarning = true,
                        Message = "Tile '" + tile + "' not found."
                    };
            }
            else if (tile.Contains(':'))
            {
                string modName = tile.Substring(tile.IndexOf(':'));

                if (!ModCompiler.current.Info.modReferences.Contains(modName))
                    return new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = file,
                        IsWarning = true,
                        Message = "Tile '" + tile + "' not found because mod '" + modName + "' isn't referenced."
                    };
            }
            else
                return new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = file,
                    IsWarning = true,
                    Message = "Tile '" + tile + "' has an invalid name. Use '<modInternalName>:<tileName>'."
                };

            return null;
        }
        static CompilerError CheckWallExists(int wall, string file)
        {
            if (wall < 0 || wall >= Main.maxWallTypes)
                return new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = file,
                    IsWarning = true,
                    Message = "Wall ID " + wall + " is not a valid vanilla wall ID. This will cause unwanted behaviour (wrong wall or an exception)."
                };

            return null;
        }
        static CompilerError CheckWallExists(string wall, string file)
        {
            if (wall.StartsWith("Vanilla:"))
            {
                if (!Defs.projectiles.ContainsKey(wall))
                    return new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = file,
                        IsWarning = true,
                        Message = "Wall '" + wall + "' not found."
                    };
            }
            else if (wall.Contains(':'))
            {
                string modName = wall.Substring(wall.IndexOf(':'));

                if (!ModCompiler.current.Info.modReferences.Contains(modName))
                    return new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = file,
                        IsWarning = true,
                        Message = "Wall '" + wall + "' not found because mod '" + modName + "' isn't referenced."
                    };
            }
            else
                return new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = file,
                    IsWarning = true,
                    Message = "Wall '" + wall + "' has an invalid name. Use '<modInternalName>:<wallName>'."
                };

            return null;
        }

        static CompilerError CheckForModBase(Assembly asm)
        {
            bool foundModBase = false;

            foreach (Type t in asm.GetTypes())
                if (t.IsSubclassOf(typeof(ModBase)))
                    foundModBase = true;

            if (!foundModBase)
                return new CompilerError()
                {
                    Cause = new TypeLoadException(),
                    FilePath = asm.Location,
                    IsWarning = false,
                    Message = "No ModBase class found."
                };

            return null;
        }

        static List<CompilerError> CheckBuffsExist()
        {
            List<CompilerError> errors = new List<CompilerError>();

            // potion (item), buff immunities (npc)

            for (int i = 0; i < ModCompiler.current.items.Count; i++)
            {
                Item it = ModCompiler.current.items[i];

                if (it.buff is int)
                    AddIfNotNull(CheckItemExists((int)it.buff, it.code), errors);
                if (it.buff is string)
                    AddIfNotNull(CheckItemExists(it.buff.ToString(), it.code), errors);
            }

            for (int i = 0; i < ModCompiler.current.npcs.Count; i++)
            {
                NPC n = ModCompiler.current.npcs[i];

                for (int j = 0; j < n.buffImmune.Length; j++)
                {
                    if (n.buffImmune[j] is int)
                        AddIfNotNull(CheckItemExists((int)n.buffImmune[j], n.code + " (" + n.displayName + ")"), errors);
                    if (n.buffImmune[j] is string)
                        AddIfNotNull(CheckItemExists(n.buffImmune[j].ToString(), n.code + " (" + n.displayName + ")"), errors);
                }
            }

            return errors;
        }
        static List<CompilerError> CheckItemsExist()
        {
            List<CompilerError> errors = new List<CompilerError>();

            // recipes (item), drops (npc)

            for (int i = 0; i < ModCompiler.current.items.Count; i++)
                for (int j = 0; j < ModCompiler.current.items[i].recipes.Count; j++)
                    foreach (string key in ModCompiler.current.items[i].recipes[j].items.Keys)
                        AddIfNotNull(CheckItemExists(key, ModCompiler.current.items[i].code), errors);

            for (int i = 0; i < ModCompiler.current.npcs.Count; i++)
                for (int j = 0; j < ModCompiler.current.npcs[i].drops.Count; j++)
                    AddIfNotNull(CheckItemExists(ModCompiler.current.npcs[i].drops[j].item,
                        ModCompiler.current.npcs[i].code + " (" + ModCompiler.current.npcs[i].displayName + ")"), errors);

            return errors;
        }
        static List<CompilerError> CheckNPCsExist()
        {
            List<CompilerError> errors = new List<CompilerError>();

            // hmmm...

            return errors;
        }
        static List<CompilerError> CheckProjsExist()
        {
            List<CompilerError> errors = new List<CompilerError>();

            // ammo, useAmmo (both item)

            for (int i = 0; i < ModCompiler.current.items.Count; i++)
            {
                Item it = ModCompiler.current.items[i];

                if (it.ammo is int)
                    AddIfNotNull(CheckProjExists((int)it.ammo, it.code), errors);
                if (it.ammo is string)
                    AddIfNotNull(CheckProjExists(it.ammo.ToString(), it.code), errors);

                if (it.useAmmo is int)
                    AddIfNotNull(CheckProjExists((int)it.useAmmo, it.code), errors);
                if (it.useAmmo is string)
                    AddIfNotNull(CheckProjExists(it.useAmmo.ToString(), it.code), errors);
            }

            return errors;
        }
        static List<CompilerError> CheckPrefixesExist()
        {
            List<CompilerError> errors = new List<CompilerError>();
            
            // hmmm...

            return errors;
        }
        static List<CompilerError> CheckTilesExist()
        {
            List<CompilerError> errors = new List<CompilerError>();

            // createTile, tileWand (both item)

            for (int i = 0; i < ModCompiler.current.items.Count; i++)
            {
                Item it = ModCompiler.current.items[i];

                if (it.createTile is int)
                    AddIfNotNull(CheckTileExists((int)it.createTile, it.code), errors);
                if (it.createTile is string)
                    AddIfNotNull(CheckTileExists(it.createTile.ToString(), it.code), errors);

                if (it.tileWand is int)
                    AddIfNotNull(CheckTileExists((int)it.tileWand, it.code), errors);
                if (it.tileWand is string)
                    AddIfNotNull(CheckTileExists(it.tileWand.ToString(), it.code), errors);
            }

            return errors;
        }
        static List<CompilerError> CheckWallsExist()
        {
            List<CompilerError> errors = new List<CompilerError>();

            // createWall (item)

            for (int i = 0; i < ModCompiler.current.items.Count; i++)
            {
                Item it = ModCompiler.current.items[i];

                if (it.createWall is int)
                    AddIfNotNull(CheckWallExists((int)it.createWall, it.code), errors);
                if (it.createWall is string)
                    AddIfNotNull(CheckWallExists(it.createWall.ToString(), it.code), errors);
            }

            return errors;
        }

        // only returns warnings, except for modbase not found.
        internal static IEnumerable<CompilerError> Check(Assembly asm)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(CheckForModBase(asm), errors);

            //LoadDefs();

            //AddIfNotNull(CheckItemsExist(),    errors);
            //AddIfNotNull(CheckNPCsExist(),     errors);
            //AddIfNotNull(CheckProjsExist(),    errors);
            //AddIfNotNull(CheckPrefixesExist(), errors);
            //AddIfNotNull(CheckTilesExist(),    errors);
            //AddIfNotNull(CheckWallsExist(),    errors);

            //main.Dispose();

            return errors;
        }
    }
}

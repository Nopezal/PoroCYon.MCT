using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using Microsoft.Xna.Framework;
using PoroCYon.Extensions;
using Terraria;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Compiler;
using PoroCYon.MCT.Tools.ModCompiler;

namespace PoroCYon.MCT.Tools.Internal.Compiler
{
    using Item = Tools.Compiler.Validation.Entities.Item;
    using NPC = Tools.Compiler.Validation.Entities.NPC;
    using Recipe = Tools.Compiler.Validation.Entities.Recipe;

    static class Checker
    {
        // static Game main;

        static bool loaded = false;

        [Obsolete]
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

        internal static void LoadDefs()
        {
            if (loaded)
                return;

            Main.dedServ = true;
            Main.rand = new Random();
            Main.player[Main.myPlayer] = new Player();
            Defs.itemNextType = Main.maxItemTypes;

            PropertyInfo pi = typeof(API).GetProperty("SoundAvailable", BindingFlags.Static | BindingFlags.Public);
            pi.SetValue(null, false, BindingFlags.NonPublic | BindingFlags.Static, null, null, null);

            // CreateMainAndLoadContent();

            Defs.FillVanillaBuffNames();
            Defs.FillVanillaBuffs();
            Defs.FillVanillaItems();
            Defs.FillVanillaNPCs();
            Defs.FillVanillaPrefixes();
            Defs.FillVanillaProjectiles();
            Defs.FillVanillaSounds();
            Defs.FillVanillaCraftingGroups();

            loaded = true;
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
            if (coll != null)
                foreach (CompilerError e in coll)
                    AddIfNotNull(e, list);
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

        static CompilerError CheckBuffExists(Union<string, int> id, string source, string file)
        {
            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i <= 0 || i >= Main.maxBuffs)
                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(i.ToString()),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Buff " + i + " in " + source + "."
                    };
            }
            else
            {
                string name = (string)id;

                if (String.IsNullOrEmpty(name)) // not defined
                    return null;

                if (!name.Contains(':'))
                    name = "Vanilla:" + name;

                if (!Defs.buffNames.Any(kvp => kvp.Value == name) && !current.buffs.Any(b => b.internalName == name))
                {
                    string internalName = name.Split(':')[0];

                    if (internalName != "Vanilla" && internalName != current.Info.internalName)
                        return new CompilerError()
                        {
                            Cause = new ObjectNotFoundException(name),
                            FilePath = file,
                            IsWarning = false,
                            Message = "Could not find Buff " + name + " in " + source + ": mod '" + internalName + "' not found."
                        };

                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(name),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Buff " + name + " in " + source + "."
                    };
                }
            }

            return null;
        }
        static CompilerError CheckItemExists(Union<string, int> id, string source, string file)
        {
            // this method also checks for craft groups

            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i == 0 || i < -48 || i >= Main.maxItemTypes)
                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(i.ToString()),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Item " + i + " in " + source + "."
                    };
            }
            else
            {
                string name = (string)id;

                if (String.IsNullOrEmpty(name)) // not defined
                    return null;

                if (!name.Contains(':'))
                    name = "Vanilla:" + name;
                else if (name.StartsWith("g:") && !Defs.itemGroups.Any(kvp => kvp.Key == name)
                        && !current.CraftGroups.itemGroups.Any(icg => "g:" + icg.name == name))
                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(name),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find craft group " + name + " in " + source + "."
                    };

                if (!Defs.items.ContainsKey(name) && !current.items.Any(i => i.internalName == name))
                {

                    string internalName = name.Split(':')[0];

                    if (internalName != "Vanilla" && internalName != current.Info.internalName)
                        return new CompilerError()
                        {
                            Cause = new ObjectNotFoundException(name),
                            FilePath = file,
                            IsWarning = false,
                            Message = "Could not find Item " + name + " in " + source + ": mod '" + internalName + "' not found."
                        };

                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(name),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Item " + name + " in " + source + "."
                    };
                }
            }

            return null;
        }
        static CompilerError CheckNPCExists (Union<string, int> id, string source, string file)
        {
            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i == 0 || i < -65 || i >= Main.maxNPCTypes)
                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(i.ToString()),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find NPC " + i + " in " + source + "."
                    };
            }
            else
            {
                string name = (string)id;

                if (String.IsNullOrEmpty(name)) // not defined
                    return null;

                if (!name.Contains(':'))
                    name = "Vanilla:" + name;

                if (!Defs.npcs.ContainsKey(name) && !current.npcs.Any(n => n.internalName == name))
                {
                    string internalName = name.Split(':')[0];

                    if (internalName != "Vanilla" && internalName != current.Info.internalName)
                        return new CompilerError()
                        {
                            Cause = new ObjectNotFoundException(name),
                            FilePath = file,
                            IsWarning = false,
                            Message = "Could not find NPC " + name + " in " + source + ": mod '" + internalName + "' not found."
                        };

                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(name),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find NPC " + name + " in " + source + "."
                    };
                }
            }

            return null;
        }
        static CompilerError CheckProjExists(Union<string, int> id, string source, string file)
        {
            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i <= 0 || i >= Main.maxProjectileTypes)
                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(i.ToString()),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Projectile " + i + " in " + source + "."
                    };
            }
            else
            {
                string name = (string)id;

                if (String.IsNullOrEmpty(name)) // not defined
                    return null;

                if (!name.Contains(':'))
                    name = "Vanilla:" + name;

                if (!Defs.projectiles.ContainsKey(name) && !current.projs.Any(p => p.internalName == name))
                {
                    string internalName = name.Split(':')[0];

                    if (internalName != "Vanilla" && internalName != current.Info.internalName)
                        return new CompilerError()
                        {
                            Cause = new ObjectNotFoundException(name),
                            FilePath = file,
                            IsWarning = false,
                            Message = "Could not find Projectile " + name + " in " + source + ": mod '" + internalName + "' not found."
                        };

                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(name),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Projectile " + name + " in " + source + "."
                    };
                }
            }

            return null;
        }
        static CompilerError CheckPfixExists(      string       id, string source, string file)
        {
            if (String.IsNullOrEmpty(id)) // not defined
                return null;

            if (!Defs.prefixes.ContainsKey(id) && !current.pfixes.Any(p => p.internalName == id))
            {
                string internalName = id.Split(':')[0];

                if (internalName != "Vanilla" && internalName != current.Info.internalName)
                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(id),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Prefix " + id + " in " + source + ": mod '" + internalName + "' not found."
                    };

                return new CompilerError()
                {
                    Cause = new ObjectNotFoundException(id),
                    FilePath = file,
                    IsWarning = false,
                    Message = "Could not find Prefix " + id + " in " + source + "."
                };
            }

            return null;
        }
        static CompilerError CheckTileExists(Union<string, int> id, string source, string file)
        {
            // tile craft groups aren't implemented yet

            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i <= 0 || i >= Main.maxTileSets)
                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(i.ToString()),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Tile " + i + " in " + source + "."
                    };
            }
            else
            {
                string name = (string)id;

                if (String.IsNullOrEmpty(name)) // not defined
                    return null;

                if (name == "Iron Anvil" || name == "Lead Anvil" || name == "Iron or Lead Anvil")
                    name = "Anvil";
                if (name == "Mythril Anvil" || name == "Orichalcum Anvil")
                    name = "Mythril or Orichalcum Anvil";
                if (name == "Adamantite Forge" || name == "Titanium Forge")
                    name = "Adamantite or Titanium Forge";

                if (!TileDef.type.Any(kvp => kvp.Key == name) && !current.tiles.Any(t => t.internalName == name))
                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(name),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Tile " + name + " in " + source + "."
                    };
            }

            return null;
        }
        static CompilerError CheckWallExists(Union<string, int> id, string source, string file)
        {
            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i <= 0 || i >= Main.maxWallTypes)
                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(i.ToString()),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Wall " + i + " in " + source + "."
                    };
            }
            else
            {
                string name = (string)id;

                if (String.IsNullOrEmpty(name)) // not defined
                    return null;

                if (!TileDef.wall.Any(kvp => kvp.Key == name) && !current.walls.Any(w => w.internalName == name))
                    return new CompilerError()
                    {
                        Cause = new ObjectNotFoundException(name),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Wall " + name + " in " + source + "."
                    };
            }

            return null;
        }

        static List<CompilerError> CheckBuffs ()
        {
            // buff references:
            // * item.buff
            // * npc.buffImmunity

            List<CompilerError> errors = new List<CompilerError>();

            // item.buff
            for (int i = 0; i < current.items.Count; i++)
                AddIfNotNull(CheckBuffExists(current.items[i].buff, "Key 'buff' in " + current.items[i].internalName, current.items[i].internalName), errors);

            // npc.buffImmunity
            for (int i = 0; i < current.npcs.Count; i++)
                for (int j = 0; j < current.npcs[i].buffImmune.Count; j++)
                    AddIfNotNull(CheckBuffExists(current.npcs[i].buffImmune[j], "Buff " + (j + 1) + " in " + current.npcs[i].internalName, current.npcs[i].internalName), errors);

            return errors;
        }
        static List<CompilerError> CheckItems ()
        {
            // item references:
            // * item.recipes.items
            // * npc.drops.item
            // * tile.drop
            // * wall.drop

            List<CompilerError> errors = new List<CompilerError>();

            // item.recipes.items
            for (int i = 0; i < current.items.Count; i++)
                for (int j = 0; j < current.items[i].recipes.Count; j++)
                    foreach (var kvp in current.items[i].recipes[j].items)
                        AddIfNotNull(CheckItemExists(kvp.Key, "Recipe " + (j + 1) + " of Item " + current.items[i].internalName, current.items[i].internalName), errors);

            // npc.drops.item
            for (int i = 0; i < current.npcs.Count; i++)
                for (int j = 0; j < current.npcs[i].drops.Count; j++)
                    AddIfNotNull(CheckItemExists(current.npcs[i].drops[j].item, "Drop " + (j + 1) + " of NPC " + current.npcs[i].internalName, current.npcs[i].internalName), errors);

            // tile.drop
            for (int i = 0; i < current.tiles.Count; i++)
                AddIfNotNull(CheckItemExists(current.tiles[i].drop, "Key 'drop' of Tile " + current.tiles[i].internalName, current.tiles[i].internalName), errors);

            // wall.drop
            for (int i = 0; i < current.walls.Count; i++)
                AddIfNotNull(CheckItemExists(current.walls[i].drop, "Key 'drop' of Wall " + current.walls[i].internalName, current.walls[i].internalName), errors);

            return errors;
        }
        static List<CompilerError> CheckNPCs  ()
        {
            // npc references:
            // * ...

            return null;

            //List<CompilerError> errors = new List<CompilerError>();



            //return errors;
        }
        static List<CompilerError> CheckProjs ()
        {
            // proj references:
            // * item.shoot

            List<CompilerError> errors = new List<CompilerError>();

            // item.shoot
            for (int i = 0; i < current.items.Count; i++)
                AddIfNotNull(CheckProjExists(current.items[i].shoot, "Key 'shoot' in Item " + current.items[i].internalName, current.items[i].internalName), errors);

            return errors;
        }
        static List<CompilerError> CheckPfixes()
        {
            // prefix references:
            // * ...

            return null;

            //List<CompilerError> errors = new List<CompilerError>();



            //return errors;
        }
        static List<CompilerError> CheckTiles ()
        {
            // tile references:
            // * item.createTile
            // * item.tileWand

            List<CompilerError> errors = new List<CompilerError>();

            for (int i = 0; i < current.items.Count; i++)
            {
                AddIfNotNull(CheckTileExists(current.items[i].createTile, "Key 'createTile' in Item " + current.items[i].internalName, current.items[i].internalName), errors);

                AddIfNotNull(CheckTileExists(current.items[i].tileWand  , "Key 'tileWand' in Item "   + current.items[i].internalName, current.items[i].internalName), errors);
            }

            return errors;
        }
        static List<CompilerError> CheckWalls ()
        {
            // wall references:
            // * item.createWall

            List<CompilerError> errors = new List<CompilerError>();

            for (int i = 0; i < current.items.Count; i++)
                AddIfNotNull(CheckTileExists(current.items[i].createWall, "Key 'createWall' in Item " + current.items[i].internalName, current.items[i].internalName), errors);

            return errors;
        }

        internal static IEnumerable<CompilerError> Check()
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(CheckForModBase(current.Assembly), errors);

            LoadDefs();

            AddIfNotNull(CheckItems (), errors);
            AddIfNotNull(CheckBuffs (), errors);
            AddIfNotNull(CheckNPCs  (), errors);
            AddIfNotNull(CheckProjs (), errors);
            AddIfNotNull(CheckPfixes(), errors);
            AddIfNotNull(CheckTiles (), errors);
            AddIfNotNull(CheckWalls (), errors);

            // main.Dispose();

            return errors;
        }
    }
}

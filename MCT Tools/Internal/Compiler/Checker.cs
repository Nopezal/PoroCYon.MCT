using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using Microsoft.Build.Framework;
using PoroCYon.Extensions;
using PoroCYon.Extensions.Collections;
using Terraria;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Compiler;
using PoroCYon.MCT.Tools.Compiler.Validation.Entities;

namespace PoroCYon.MCT.Tools.Internal.Compiler
{
	class Checker : CompilerPhase
    {
        static bool loaded = false;

        public Checker(ModCompiler mc)
            : base(mc)
        {

        }

        internal static void LoadDefs()
        {
            if (loaded)
                return;

            Main.dedServ = true;
            Main.rand = new Random();
			ItemDef.nextType = Main.maxItemTypes;

            PropertyInfo pi = typeof(API).GetProperty("SoundAvailable", BindingFlags.Static | BindingFlags.Public);
            pi.SetValue(null, false, BindingFlags.NonPublic | BindingFlags.Static, null, null, null);

			// CreateMainAndLoadContent();

			BuffDef.FillBuffNames();
			BuffDef.FillVanilla();
			ItemDef.FillVanilla();
			 NPCDef.FillVanilla();
            Defs.FillVanillaPrefixes();
			ProjDef.FillVanilla();
			SoundDef.FillVanillaSounds();
			ItemDef.FillVanillaCraftingGroups();

			Biome.InitBiomes();

			Main.player[Main.myPlayer] = new Player();

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

        CompilerError CheckForModBase()
        {
			List<Type> bases = new List<Type>();

			foreach (Type t in Building.Assembly.GetTypes())
				if (t.IsSubclassOf(typeof(ModBase)))
				{
					bases.Add(t);

					if (bases.Count >= 2)
						break;
				}

            if (bases.Count <= 0)
                return new CompilerError(Building)
                {
                    Cause = new TypeLoadException(),
                    FilePath = Building.Assembly.Location,
                    IsWarning = true,
                    Message = "No ModBase class found, using default"
                };
			if (bases.Count > 1)
				return new CompilerError(Building)
				{
					Cause = new TypeLoadException(),
					FilePath = Building.Assembly.Location,
					IsWarning = false,
					Message = "Multiple ModBase classes found: " + bases.CastAll(t => t.FullName).Join(CommonJoinValues.Comma) + "."
				};

			bases.Clear();

			return null;
        }

        CompilerError CheckBuffExists(Union<string, int> id, string source, string file)
        {
            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i <= 0 || i >= Main.maxBuffTypes)
                    return new CompilerError(Building)
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

                if (!BuffDef.byName.Any(kvp => kvp.Key == name) && !Building.buffs.Any(b => b.internalName == name))
                {
                    string internalName = name.Split(':')[0];

                    if (internalName != "Vanilla" && internalName != Building.Info.internalName)
                        return new CompilerError(Building)
                        {
                            Cause = new ObjectNotFoundException(name),
                            FilePath = file,
                            IsWarning = false,
                            Message = "Could not find Buff " + name + " in " + source + ": mod '" + internalName + "' not found."
                        };

                    return new CompilerError(Building)
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
        CompilerError CheckItemExists(Union<string, int> id, string source, string file)
        {
            // this method also checks for craft groups

            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i == 0 || i < -48 || i >= Main.maxItemTypes)
                    return new CompilerError(Building)
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
                else if (name.StartsWith("g:") && !ItemDef.itemGroups.Any(kvp => kvp.Key == name)
                        && !Building.CraftGroups.itemGroups.Any(icg => "g:" + icg.name == name))
                    return new CompilerError(Building)
                    {
                        Cause = new ObjectNotFoundException(name),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find craft group " + name + " in " + source + "."
                    };

                if (!ItemDef.byName.ContainsKey(name) && !Building.items.Any(i => i.internalName == name))
                {

                    string internalName = name.Split(':')[0];

                    if (internalName != "Vanilla" && internalName != Building.Info.internalName)
                        return new CompilerError(Building)
                        {
                            Cause = new ObjectNotFoundException(name),
                            FilePath = file,
                            IsWarning = false,
                            Message = "Could not find Item " + name + " in " + source + ": mod '" + internalName + "' not found."
                        };

                    return new CompilerError(Building)
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
        CompilerError CheckNPCExists (Union<string, int> id, string source, string file)
        {
            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i == 0 || i < -65 || i >= Main.maxNPCTypes)
                    return new CompilerError(Building)
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

                if (!NPCDef.byName.ContainsKey(name) && !Building.npcs.Any(n => n.internalName == name))
                {
                    string internalName = name.Split(':')[0];

                    if (internalName != "Vanilla" && internalName != Building.Info.internalName)
                        return new CompilerError(Building)
                        {
                            Cause = new ObjectNotFoundException(name),
                            FilePath = file,
                            IsWarning = false,
                            Message = "Could not find NPC " + name + " in " + source + ": mod '" + internalName + "' not found."
                        };

                    return new CompilerError(Building)
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
        CompilerError CheckProjExists(Union<string, int> id, string source, string file)
        {
            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i <= 0 || i >= Main.maxProjectileTypes)
                    return new CompilerError(Building)
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

                if (!ProjDef.byName.ContainsKey(name) && !Building.projs.Any(p => p.internalName == name))
                {
                    string internalName = name.Split(':')[0];

                    if (internalName != "Vanilla" && internalName != Building.Info.internalName)
                        return new CompilerError(Building)
                        {
                            Cause = new ObjectNotFoundException(name),
                            FilePath = file,
                            IsWarning = false,
                            Message = "Could not find Projectile " + name + " in " + source + ": mod '" + internalName + "' not found."
                        };

                    return new CompilerError(Building)
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
        CompilerError CheckPfixExists(      string       id, string source, string file)
        {
            if (String.IsNullOrEmpty(id)) // not defined
                return null;

            if (!Defs.prefixes.ContainsKey(id) && !Building.pfixes.Any(p => p.internalName == id))
            {
                string internalName = id.Split(':')[0];

                if (internalName != "Vanilla" && internalName != Building.Info.internalName)
                    return new CompilerError(Building)
                    {
                        Cause = new ObjectNotFoundException(id),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Prefix " + id + " in " + source + ": mod '" + internalName + "' not found."
                    };

                return new CompilerError(Building)
                {
                    Cause = new ObjectNotFoundException(id),
                    FilePath = file,
                    IsWarning = false,
                    Message = "Could not find Prefix " + id + " in " + source + "."
                };
            }

            return null;
        }
        CompilerError CheckTileExists(Union<string, int> id, string source, string file)
        {
            // tile craft groups aren't implemented yet

            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i <= 0 || i >= Main.maxTileSets)
                    return new CompilerError(Building)
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

                if (!TileDef.byName.Any(kvp => kvp.Key == name) && !Building.tiles.Any(t => t.internalName == name))
                    return new CompilerError(Building)
                    {
                        Cause = new ObjectNotFoundException(name),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Tile " + name + " in " + source + "."
                    };
            }

            return null;
        }
        CompilerError CheckWallExists(Union<string, int> id, string source, string file)
        {
            if (id.UsedObjectNum == 1)
            {
                int i = (int)id;

                if (i == 0) // not defined
                    return null;

                if (i <= 0 || i >= Main.maxWallTypes)
                    return new CompilerError(Building)
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

                if (!TileDef.wallByName.Any(kvp => kvp.Key == name) && !Building.walls.Any(w => w.internalName == name))
                    return new CompilerError(Building)
                    {
                        Cause = new ObjectNotFoundException(name),
                        FilePath = file,
                        IsWarning = false,
                        Message = "Could not find Wall " + name + " in " + source + "."
                    };
            }

            return null;
        }

        CompilerError CheckTypeExists(Buff buff)
        {
            if (!buff.hasCode || buff.code == null) // code not specified in JSON
                return null;

            if (Building.Assembly.GetType(buff.code, false, false) == null)
                return new CompilerError(Building)
                {
                    Cause = new TypeLoadException("Type " + buff.code + " not found."),
                    FilePath = buff.internalName,
                    IsWarning = false,
                    Message = "Could not load type " + buff.code + "."
                };

            return null;
        }
        CompilerError CheckTypeExists(EntityValidator entity)
        {
            if (entity.codeDefaultValue)
                return null;

            if (Building.Assembly.GetType(entity.code, false, false) == null)
                return new CompilerError(Building)
                {
                    Cause = new TypeLoadException("Type " + entity.code + " not found."),
                    FilePath = entity.internalName,
                    IsWarning = false,
                    Message = "Could not load type " + entity.code + "."
                };

            return null;
        }
        CompilerError CheckBiomeExists(Tools.Compiler.Validation.Entities.Item item)
        {
            for (int i = 0; i < item.fish.biomes.Length; i++)
                if (!Biome.Biomes.ContainsKey(item.fish.biomes[i]))
                    return new CompilerError(Building)
                    {
                        Cause = new ObjectNotFoundException(item.fish.biomes[i]),
                        FilePath = item.internalName,
                        IsWarning = item.fish.biomes[i].Split(':')[0] == Building.Info.internalName,
                        Message = "Biome " + item.fish.biomes[i] + " not found."
                    };

            return null;
        }

        List<CompilerError> CheckBuffs ()
        {
            // buff references:
            // * item.buff
            // * npc.buffImmunity

            List<CompilerError> errors = new List<CompilerError>();

            // item.buff
            for (int i = 0; i < Building.items.Count; i++)
                AddIfNotNull(CheckBuffExists(Building.items[i].buff, "Key 'buff' in " + Building.items[i].internalName, Building.items[i].internalName), errors);

            // npc.buffImmunity
            for (int i = 0; i < Building.npcs.Count; i++)
                for (int j = 0; j < Building.npcs[i].buffImmune.Count; j++)
                    AddIfNotNull(CheckBuffExists(Building.npcs[i].buffImmune[j], "Buff " + (j + 1) + " in " + Building.npcs[i].internalName, Building.npcs[i].internalName), errors);

            return errors;
        }
        List<CompilerError> CheckItems ()
		{
			// item references:
			// * item.recipes.items
            // * item.tileWand
			// * npc.drops.item
			// * tile.drop
			// * wall.drop
			// * npc.catchItem

			List<CompilerError> errors = new List<CompilerError>();

            // item.tileWand, item.recipes.items
            for (int i = 0; i < Building.items.Count; i++)
            {
                AddIfNotNull(CheckItemExists(Building.items[i].tileWand, "field tileWand of Item " + Building.items[i].internalName, Building.items[i].internalName), errors);

                for (int j = 0; j < Building.items[i].recipes.Count; j++)
                    foreach (var kvp in Building.items[i].recipes[j].items)
                        AddIfNotNull(CheckItemExists(kvp.Key, "Recipe " + (j + 1) + " of Item " + Building.items[i].internalName, Building.items[i].internalName), errors);
            }

            // npc.drops.item
            for (int i = 0; i < Building.npcs.Count; i++)
                for (int j = 0; j < Building.npcs[i].drops.Count; j++)
                    if (Building.npcs[i].drops[j].usesChoose)
                        for (int k = 0; k < Building.npcs[i].drops[j].choose.Length; k++)
                            AddIfNotNull(CheckItemExists(Building.npcs[i].drops[j].choose[k].item, "Choice " + (k + 1) + " in Drop " + (j + 1) + " of NPC " + Building.npcs[i].internalName, Building.npcs[i].internalName), errors);
                    else
                        AddIfNotNull(CheckItemExists(Building.npcs[i].drops[j].item, "Drop " + (j + 1) + " of NPC " + Building.npcs[i].internalName, Building.npcs[i].internalName), errors);

            // tile.drop
            for (int i = 0; i < Building.tiles.Count; i++)
                AddIfNotNull(CheckItemExists(Building.tiles[i].drop, "Key 'drop' of Tile " + Building.tiles[i].internalName, Building.tiles[i].internalName), errors);

            // wall.drop
            for (int i = 0; i < Building.walls.Count; i++)
                AddIfNotNull(CheckItemExists(Building.walls[i].drop, "Key 'drop' of Wall " + Building.walls[i].internalName, Building.walls[i].internalName), errors);

			// npc.catchItem
			for (int i = 0; i < Building.npcs.Count; i++)
				AddIfNotNull(CheckItemExists(Building.npcs[i].catchItem, "Catch item of NPC " + Building.npcs[i].internalName, Building.npcs[i].internalName), errors);

			return errors;
        }
        List<CompilerError> CheckNPCs  ()
		{
			// npc references:
			// * ...

			return null;

            //List<CompilerError> errors = new List<CompilerError>();



            //return errors;
        }
        List<CompilerError> CheckProjs ()
        {
            // proj references:
            // * item.shoot

            List<CompilerError> errors = new List<CompilerError>();

            // item.shoot
            for (int i = 0; i < Building.items.Count; i++)
                AddIfNotNull(CheckProjExists(Building.items[i].shoot, "Key 'shoot' in Item " + Building.items[i].internalName, Building.items[i].internalName), errors);

            return errors;
        }
        List<CompilerError> CheckPfixes()
        {
            // prefix references:
            // * ...

            return null;

            //List<CompilerError> errors = new List<CompilerError>();



            //return errors;
        }
        List<CompilerError> CheckTiles ()
        {
            // tile references:
            // * item.createTile

            List<CompilerError> errors = new List<CompilerError>();

            for (int i = 0; i < Building.items.Count; i++)
                AddIfNotNull(CheckTileExists(Building.items[i].createTile, "Key 'createTile' in Item " + Building.items[i].internalName, Building.items[i].internalName), errors);

            return errors;
        }
        List<CompilerError> CheckWalls ()
        {
            // wall references:
            // * item.createWall

            List<CompilerError> errors = new List<CompilerError>();

            for (int i = 0; i < Building.items.Count; i++)
                AddIfNotNull(CheckWallExists(Building.items[i].createWall, "Key 'createWall' in Item " + Building.items[i].internalName, Building.items[i].internalName), errors);

            return errors;
        }

        List<CompilerError> CheckTypeNames()
        {
            List<CompilerError> errors = new List<CompilerError>();

            // type references:
            // * buff (modbuff)
            // * item (moditem)
            // * npc  (modnpc)
            // * proj (modprojectile)
            // * tile (modtile, modtiletype)

            // buffs
            for (int i = 0; i < Building.buffs.Count; i++)
                errors.Add(CheckTypeExists(Building.buffs[i]));

            // items
            for (int i = 0; i < Building.items.Count; i++)
                errors.Add(CheckTypeExists(Building.items[i]));

            // npcs
            for (int i = 0; i < Building.npcs.Count; i++)
                errors.Add(CheckTypeExists(Building.npcs[i]));

            // projs
            for (int i = 0; i < Building.projs.Count; i++)
                errors.Add(CheckTypeExists(Building.projs[i]));

            // tiles
            for (int i = 0; i < Building.tiles.Count; i++)
                errors.Add(CheckTypeExists(Building.tiles[i]));

            return errors;
        }
        List<CompilerError> CheckBiomes   ()
        {
            List<CompilerError> errors = new List<CompilerError>();

            // biome references:
            // * item.fish.biomes

            for (int i = 0; i < Building.items.Count; i++)
                AddIfNotNull(CheckBiomeExists(Building.items[i]), errors);

            return errors;
        }

        internal IEnumerable<CompilerError> Check()
        {
            List<CompilerError> errors = new List<CompilerError>();

            Compiler.Log("Checking for ModBase...", MessageImportance.Low);
            AddIfNotNull(CheckForModBase(), errors);

            Compiler.Log("Checking entity types...", MessageImportance.Low);
            AddIfNotNull(CheckTypeNames(), errors);

            LoadDefs();

            Compiler.Log("Checking for items...", MessageImportance.Low);
            AddIfNotNull(CheckItems (), errors);

            Compiler.Log("Checking for buffs...", MessageImportance.Low);
            AddIfNotNull(CheckBuffs (), errors);

            Compiler.Log("Checking for NPCs...", MessageImportance.Low);
            AddIfNotNull(CheckNPCs  (), errors);

            Compiler.Log("Checking for projectiles...", MessageImportance.Low);
            AddIfNotNull(CheckProjs (), errors);

            Compiler.Log("Checking for prefixes...", MessageImportance.Low);
            AddIfNotNull(CheckPfixes(), errors);

            Compiler.Log("Checking for tiles...", MessageImportance.Low);
            AddIfNotNull(CheckTiles (), errors);

            Compiler.Log("Checking for walls...", MessageImportance.Low);
            AddIfNotNull(CheckWalls (), errors);

            // main.Dispose();

            return errors;
        }
    }
}

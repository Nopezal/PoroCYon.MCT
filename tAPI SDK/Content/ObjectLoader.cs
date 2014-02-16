using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAPI.SDK.Content
{
    /// <summary>
    /// Provides helper functions to add content by code
    /// </summary>
    public static class ObjectLoader
    {
        /// <summary>
        /// Adds an Item to the default game data
        /// </summary>
        /// <param name="i">The Item to add</param>
        /// <param name="texture">The texture of the Item to add</param>
        /// <param name="param">Common object loading parameters</param>
        /// <param name="Aparam">Parameters for armor items</param>
        /// <returns>The IDs of the armor parts as an ArmorReturnValues</returns>
        public static ArmorReturnValues AddToGame(Item i, Texture2D texture, LoadParameters param, ArmorParameters Aparam = default(ArmorParameters))
        {
            ArmorReturnValues ret = new ArmorReturnValues(-1, -1, -1);

            i.netID = i.type = Defs.itemNextType++;
            i.stack = 1;
            i.name = param.ModBase.modName + ":" + param.Name;
            i.displayName = param.Name;

            if (Aparam.HeadTexture != null)
            {
                i.headSlot = Defs.headSlotNextType++;

                if (Item.headType.ContainsKey(i.headSlot))
                    Item.headType[i.headSlot] = i.type;
                else
                    Item.headType.Add(i.headSlot, i.type);

                if (!Main.dedServ)
                    Main.armorHeadTexture.Add(i.headSlot, Aparam.HeadTexture);

                ret.HeadID = i.headSlot;
            }

            if (Aparam.BodyTexture != null)
            {
                i.bodySlot = Defs.bodySlotNextType++;

                if (Item.bodyType.ContainsKey(i.bodySlot))
                    Item.bodyType[i.bodySlot] = i.type;
                else
                    Item.bodyType.Add(i.bodySlot, i.type);

                if (!Main.dedServ)
                {
                    Main.armorHeadTexture.Add(i.bodySlot, Aparam.BodyTexture);

                    if (Aparam.FemaleBodyTexture != null)
                        Main.femaleBodyTexture.Add(i.bodySlot, Aparam.FemaleBodyTexture);
                }

                ret.BodyID = i.bodySlot;
            }

            if (Aparam.LegsTexture != null)
            {
                i.legSlot = Defs.legSlotNextType++;

                if (Item.legType.ContainsKey(i.legSlot))
                    Item.legType[i.legSlot] = i.type;
                else
                    Item.legType.Add(i.legSlot, i.type);

                if (!Main.dedServ)
                    Main.armorLegTexture.Add(i.legSlot, Aparam.LegsTexture);

                ret.LegsID = i.legSlot;
            }

            if (param.SubClassTypeName != null)
            {
                i.subClass = (ModItem)param.Assembly.CreateInstance(param.SubClassTypeName, false, BindingFlags.Public | BindingFlags.Instance, null,
                    new object[] { param.ModBase, i }, CultureInfo.CurrentCulture, new object[] { });

                if (i.subClass != null)
                    Defs.FillCallPriorities(i.subClass.GetType());
            }

            if (!Main.dedServ)
                Main.itemTexture.Add(i.type, texture);
            Defs.items.Add(i.name, i);
            Defs.itemNames.Add(i.type, i.name);

            return ret;
        }

        /// <summary>
        /// Adds a Recipe to the game.
        /// </summary>
        /// <param name="r">The Recipe to add</param>
        public static void AddToGame(Recipe r)
        {
            Recipe.newRecipe = r;
            Recipe.AddRecipe();
        }

        /// <summary>
        /// Adds an NPC to the game.
        /// </summary>
        /// <param name="n">The NPC to add</param>
        /// <param name="texture">The texture of the NPC to add</param>
        /// <param name="param">Common object loading parameters</param>
        public static void AddToGame(NPC n, Texture2D texture, LoadParameters param)
        {
            n.netID = n.type = Defs.npcNextType++;
            n.name = param.ModBase.modName + ":" + param.Name;
            n.displayName = param.Name;

            if (!String.IsNullOrEmpty(param.SubClassTypeName))
            {
                n.subClass = (ModNPC)param.Assembly.CreateInstance(param.SubClassTypeName, false, BindingFlags.Public | BindingFlags.Instance, null,
                    new object[] { param.ModBase, n }, CultureInfo.CurrentCulture, new object[] { });

                if (n.subClass != null)
                    Defs.FillCallPriorities(n.subClass.GetType());
            }

            if (!Main.dedServ)
                Main.npcTexture.Add(n.type, texture);
            Defs.npcs.Add(n.name, n);
            Defs.npcNames.Add(n.type, n.name);
        }

        /// <summary>
        /// Adds a Projectile to the game
        /// </summary>
        /// <param name="p">The Projectile to add</param>
        /// <param name="texture">The texture of the projectile to add</param>
        /// <param name="param">Common object loading parameters</param>
        /// <param name="pet">Wether the Projectile is a pet or not</param>
        public static void AddToGame(Projectile p, Texture2D texture, LoadParameters param, bool pet = false)
        {
            p.type = Defs.projectileNextType++;
            p.name = param.ModBase.modName + ":" + param.Name;

            if (!String.IsNullOrEmpty(param.SubClassTypeName))
            {
                p.subClass = (ModProjectile)param.Assembly.CreateInstance(param.SubClassTypeName, false, BindingFlags.Public | BindingFlags.Instance, null,
                    new object[] { param.ModBase, p }, CultureInfo.CurrentCulture, new object[] { });

                if (p.subClass != null)
                    Defs.FillCallPriorities(p.subClass.GetType());
            }

            if (pet)
            {
                Array.Resize(ref Main.projPet, Main.projPet.Length + 1);
                Main.projPet[p.type] = pet;
            }

            if (!Main.dedServ)
                Main.projectileTexture.Add(p.type, texture);
            Defs.projectiles.Add(p.name, p);
            Defs.projectileNames.Add(p.type, p.name);
        }

        /// <summary>
        /// Adds a Tile to the game
        /// </summary>
        /// <param name="Tparam">The Tile data to add to the game</param>
        /// <param name="param">Common object loading parameters</param>
        public static void AddToGame(TileParameters Tparam, LoadParameters param)
        {
            int type = Defs.tileNextType++;
            TileDef.ResizeTiles(Defs.tileNextType);

            if (!Main.dedServ)
                Main.tileTexture[type] = Tparam.Texture;

            if (!String.IsNullOrEmpty(param.SubClassTypeName))
            {
                TileDef.codeClass[type] = (ModTile)param.Assembly.CreateInstance(param.SubClassTypeName, false, BindingFlags.Public | BindingFlags.Instance, null,
                    new object[] { param.ModBase }, CultureInfo.CurrentCulture, new object[] { });

                if (TileDef.codeClass[type] != null)
                    Defs.FillCallPriorities(TileDef.codeClass[type].GetType());
            }

            TileDef.name[type] = param.ModBase.modName + ":" + param.Name;
            TileDef.displayName[type] = param.Name;

            TileDef.width[type] = Tparam.Width;
            TileDef.height[type] = Tparam.Height;
            TileDef.frameWidth[type] = Tparam.FrameWidth;
            TileDef.frameHeight[type] = Tparam.FrameHeight;
            TileDef.sheetColumns[type] = Tparam.SheetColumns;
            TileDef.sheetLines[type] = Tparam.SheetLines;

            TileDef.solid[type] = Tparam.Solid;
            TileDef.solidTop[type] = Tparam.SolidTop;
            TileDef.frameImportant[type] = Tparam.FrameImportant;

            TileDef.breaksFast[type] = Tparam.BreaksFast;
            TileDef.breaksByPick[type] = Tparam.BreaksByPic;
            TileDef.breaksByAxe[type] = Tparam.BreaksByAxe;
            TileDef.breaksByHammer[type] = Tparam.BreaksByHammer;
            TileDef.breaksByCut[type] = Tparam.BreaksByCut;
            TileDef.breaksByWater[type] = Tparam.BreaksByWater;
            TileDef.breaksByLava[type] = Tparam.BreaksByLava;

            TileDef.table[type] = Tparam.Table;
            TileDef.rope[type] = Tparam.Rope;
            TileDef.noAttach[type] = Tparam.NoAttach;
            TileDef.tileDungeon[type] = Tparam.Dungeon;

            TileDef.blocksLight[type] = Tparam.BlocksAnyLight;
            TileDef.blocksSun[type] = Tparam.BlocksSunlight;
            TileDef.glows[type] = Tparam.Glows;
            TileDef.shines[type] = Tparam.Shines;
            TileDef.shineChance[type] = Tparam.ShineChance;
            TileDef.frame[type] = Tparam.Frame;
            TileDef.frameCounter[type] = Tparam.FrameCounter;

            TileDef.brick[type] = Tparam.Brick;
            TileDef.moss[type] = Tparam.Moss;
            TileDef.stone[type] = Tparam.Stone;
            TileDef.mergeDirt[type] = Tparam.Dirt;

            TileDef.tileSand[type] = Tparam.Sand;
            TileDef.tileFlame[type] = Tparam.Flame;
            TileDef.alchemyflower[type] = Tparam.AlchemyFlower;
        }

        /// <summary>
        /// Adds a Wall to the game
        /// </summary>
        /// <param name="texture">The texture of the Wall to add</param>
        /// <param name="param">Common object loading parameters</param>
        public static void AddWallToGame(Texture2D texture, LoadParameters param)
        {
            int type = Defs.wallNextType++;
            TileDef.ResizeWalls(Defs.wallNextType);

            if (!Main.dedServ)
                Main.wallTexture[type] = texture;

            TileDef.wall[param.ModBase.modName + ":" + param.Name] = (ushort)type;
        }

        /// <summary>
        /// Adds a Prefix to the game
        /// </summary>
        /// <param name="pfix">The Prefix to add</param>
        /// <param name="param">Common object loading parameters</param>
        public static void AddToGame(Prefix pfix, LoadParameters param)
        {
            pfix.name = param.ModBase.modName + ":" + param.Name;

            pfix.GetType().GetField("id", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(pfix, Defs.prefixNextType++);
            Defs.prefixes.Add(pfix.name, pfix);
        }

        /// <summary>
        /// Adds a Gore to the game
        /// </summary>
        /// <param name="texture">The texture of the Gore to add</param>
        /// <param name="param">Common object loading parameters</param>
        public static void AddGoreToGame(Texture2D texture, LoadParameters param)
        {
            int type = Defs.goreNextType++;

            if (!Main.dedServ)
                Main.goreTexture[type] = texture;

            Defs.gores[param.ModBase.modName + ":" + param.Name] = type;
        }

        /// <summary>
        /// Adds a Buff to the game
        /// </summary>
        /// <param name="Bparam">The Buff data to add to the game</param>
        /// <param name="param">Common object loading parameters</param>
        public static void AddToGame(BuffParameters Bparam, LoadParameters param)
        {
            int type = Defs.buffNextType++;

            if (!Main.dedServ)
                Main.buffTexture.Add(type, Bparam.Texture);

            Defs.buffNames[type] = param.ModBase.modName + ":" + param.Name;
            Defs.buffType[param.ModBase.modName + ":" + param.Name] = type;

            if (!String.IsNullOrEmpty(param.SubClassTypeName))
            {
                Defs.buffs.Add(type, (ModBuff)param.Assembly.CreateInstance(param.SubClassTypeName, false, BindingFlags.Public | BindingFlags.Instance,
                    null, new object[] { param.ModBase }, System.Globalization.CultureInfo.CurrentCulture, new object[] { }));

                if (Defs.buffs[type] != null)
                    Defs.FillCallPriorities(Defs.buffs[type].GetType());
            }

            Array.Resize(ref Main.buffName, Main.buffName.Length + 1);
            Main.buffName[type] = param.Name;
            Array.Resize(ref Main.buffTip, Main.buffTip.Length + 1);
            Main.buffTip[type] = Bparam.Tip;
            Array.Resize(ref Main.debuff, Main.debuff.Length + 1);
            Main.debuff[type] = (Bparam.Type & BuffType.Debuff) == BuffType.Debuff;
            Array.Resize(ref Main.vanityPet, Main.vanityPet.Length + 1);
            Main.vanityPet[type] = Bparam.VanityPet;
            Array.Resize(ref Main.lightPet, Main.lightPet.Length + 1);
            Main.vanityPet[type] = Bparam.LightPet;
            Array.Resize(ref Main.meleeBuff, Main.meleeBuff.Length + 1);
            Main.meleeBuff[type] = (Bparam.Type & BuffType.WeaponBuff) == BuffType.WeaponBuff;
        }

        /// <summary>
        /// Adds Wings to the game
        /// </summary>
        /// <param name="texture">The texture of the Wings to add</param>
        /// <param name="param">Common object loading parameters</param>
        /// <returns>The ID of the newly added wings</returns>
        public static int AddWingsToGame(Texture2D texture, LoadParameters param)
        {
            int id = Main.wingsTexture.Count;

            Main.wingsTexture.Add(id, texture);
            Array.Resize(ref Main.wingsLoaded, Main.wingsLoaded.Length + 1);
            Main.wingsLoaded[id] = true;

            return id;
        }
    }
}

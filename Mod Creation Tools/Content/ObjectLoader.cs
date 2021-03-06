﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.Content
{
    /// <summary>
    /// Provides helper functions to add content by code
    /// </summary>
    public static class ObjectLoader
    {
        static int invasionNextType = 0;

        /// <summary>
        /// Constants.mainInstance.Content
        /// </summary>
        public static ContentManager Content
        {
            get
            {
                return API.main.Content;
            }
        }

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

            i.netID = i.type = ItemDef.nextType++;
            i.stack = 1;
            i.name = param.ModBase.mod.InternalName + ":" + param.Name;
            //i.displayName = param.Name;

            #region Aparam
            if (Aparam.HeadTexture != null)
            {
                i.headSlot = ItemDef.headSlotNextType++;

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
                i.bodySlot = ItemDef.bodySlotNextType++;

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
                i.legSlot = ItemDef.legSlotNextType++;

                if (Item.legType.ContainsKey(i.legSlot))
                    Item.legType[i.legSlot] = i.type;
                else
                    Item.legType.Add(i.legSlot, i.type);

                if (!Main.dedServ)
                    Main.armorLegTexture.Add(i.legSlot, Aparam.LegsTexture);

                ret.LegsID = i.legSlot;
            }
            #endregion

            if (!Main.dedServ)
                Main.itemTexture.Add(i.type, texture);

            ItemDef.byName.Add(i.name, i);
            ItemDef.byType.Add(i.type, i);

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
            n.netID = n.type = NPCDef.nextType++;
            n.name = param.ModBase.mod.InternalName + ":" + param.Name;
            n.displayName = param.Name;

            if (!Main.dedServ)
                Main.npcTexture.Add(n.type, texture);

			NPCDef.byName.Add(n.name, n);
            NPCDef.byType.Add(n.type, n);
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
            p.type = ProjDef.nextType++;
            p.name = param.ModBase.mod.InternalName + ":" + param.Name;

            if (pet)
            {
                Array.Resize(ref Main.projPet, Main.projPet.Length + 1);
                Main.projPet[p.type] = pet;
            }

            if (!Main.dedServ)
                Main.projectileTexture.Add(p.type, texture);

			p.def = new ProjDef(param.ModBase);

            ProjDef.byName.Add(p.name, p);
            ProjDef.byType.Add(p.type, p);
        }

        /// <summary>
        /// Adds a Tile to the game
        /// </summary>
        /// <param name="Tparam">The Tile data to add to the game</param>
        /// <param name="param">Common object loading parameters</param>
        public static void AddToGame(TileParameters Tparam, LoadParameters param)
        {
            int type = TileDef.tileNextType++;
            TileDef.ResizeTiles(TileDef.tileNextType);

            if (!Main.dedServ)
                Main.tileTexture[type] = Tparam.Texture;

            if (!String.IsNullOrEmpty(param.SubClassTypeName))
            {
                TileDef.codeClass[type] = (ModTile)param.Assembly.CreateInstance(param.SubClassTypeName, false, BindingFlags.Public | BindingFlags.Instance, null,
                    new object[] { param.ModBase }, CultureInfo.CurrentCulture, new object[] { });
            }

            // helk
            TileDef.byType[type] = param.ModBase.mod.InternalName + ":" + param.Name;
            TileDef.byName[param.ModBase.mod.InternalName + ":" + param.Name] = (ushort)type;
            TileDef.displayName[type] = param.Name;

            TileDef.width[type] = Tparam.Width;
            TileDef.height[type] = Tparam.Height;
            TileDef.frameWidth[type] = Tparam.FrameWidth;
            TileDef.frameHeight[type] = Tparam.FrameHeight;
            //TileDef.sheetColumns[type] = Tparam.SheetColumns;
            //TileDef.sheetRows[type] = Tparam.SheetRows;

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
            TileDef.alchemyFlower[type] = Tparam.AlchemyFlower;
        }

        /// <summary>
        /// Adds a Wall to the game
        /// </summary>
        /// <param name="texture">The texture of the Wall to add</param>
        /// <param name="param">Common object loading parameters</param>
        public static void AddWallToGame(Texture2D texture, LoadParameters param)
        {
            int type = TileDef.wallNextType++;
            TileDef.ResizeWalls(TileDef.wallNextType);

            if (!Main.dedServ)
                Main.wallTexture[type] = texture;

            TileDef.wallByName[param.ModBase.mod.InternalName + ":" + param.Name] = (ushort)type;
            TileDef.wallByType[(ushort)type] = param.ModBase.mod.InternalName + ":" + param.Name;
        }

        /// <summary>
        /// Adds a Prefix to the game
        /// </summary>
        /// <param name="pfix">The Prefix to add</param>
        /// <param name="param">Common object loading parameters</param>
        public static void AddToGame(Prefix pfix, LoadParameters param)
        {
            pfix.name = param.ModBase.mod.InternalName + ":" + param.Name;

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
			int type = GoreDef.goreNextType++;

			if (!Main.dedServ)
				Main.goreTexture[type] = texture;

			GoreDef.gores[param.ModBase.mod.InternalName + ":" + param.Name] = type;
        }

        /// <summary>
        /// Adds a Buff to the game
        /// </summary>
        /// <param name="Bparam">The Buff data to add to the game</param>
        /// <param name="param">Common object loading parameters</param>
        public static void AddToGame(BuffParameters Bparam, LoadParameters param)
        {
            int type = BuffDef.nextType++;

            if (!Main.dedServ)
                Main.buffTexture.Add(type, Bparam.Texture);

			BuffDef def = new BuffDef(param.ModBase);

			BuffDef.byType[type] = param.ModBase.mod.InternalName + ":" + param.Name;
            BuffDef.byName[param.ModBase.mod.InternalName + ":" + param.Name] = type;

			if (!Main.dedServ)
				Main.buffTexture[type] = Bparam.Texture;

			if (!String.IsNullOrEmpty(param.SubClassTypeName))
			{
				try
				{
					def.modBuffType = param.Assembly.GetType(param.SubClassTypeName);
				}
				catch (Exception e)
				{
					throw new ArgumentException("Invalid subclass name: " + param.SubClassTypeName, "param", e);
				}
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

            if (Main.wingsTexture.ContainsValue(texture))
            {
                foreach (KeyValuePair<int, Texture2D> kvp in Main.wingsTexture)
                    if (kvp.Value == texture)
                    {
						if (id < 0 || id >= Main.wingsLoaded.Length)
							Array.Resize(ref Main.wingsLoaded, Math.Max(Main.wingsLoaded.Length, id) + 1);

						Main.wingsLoaded[id] = true;

                        //if (Main.wingsLoaded.ContainsKey(id))
                        //    Main.wingsLoaded[id] = true;
                        //else
                        //    Main.wingsLoaded.Add(id, true);

                        id = kvp.Key;
                    }
            }
            else
                Main.wingsTexture.Add(id, texture);

			if (id < 0 || id >= Main.wingsLoaded.Length)
				Array.Resize(ref Main.wingsLoaded, Math.Max(Main.wingsLoaded.Length, id) + 1);

			Main.wingsLoaded[id] = true;

			//if (Main.wingsLoaded.ContainsKey(id))
			//    Main.wingsLoaded[id] = true;
			//else
			//    Main.wingsLoaded.Add(id, true);

			return id;
        }

        /// <summary>
        /// Loads a Texture2D from a byte array as a .png
        /// </summary>
        /// <param name="data">The .png file to load as a Texture2D</param>
        /// <returns>The .png data as a Texture2D</returns>
        public static Texture2D LoadTexture(byte[] data)
        {
            return LoadTexture(new MemoryStream(data));
        }
        /// <summary>
        /// Loads a SoundEffect from a byte array as a .wav
        /// </summary>
        /// <param name="data">The .wav file to load as a SoundEffect</param>
        /// <returns>The .wav data as a SoundEffect</returns>
        public static SoundEffect LoadSound(byte[] data)
        {
            return LoadSound(new MemoryStream(data));
        }
        /// <summary>
        /// Loads a Texture2D from a stream as a .png
        /// </summary>
        /// <param name="stream">The .png file to load as a Texture2D</param>
        /// <returns>The .png data as a Texture2D</returns>
        public static Texture2D LoadTexture(Stream stream)
        {
            return Texture2D.FromStream(API.main.GraphicsDevice, stream);
        }
        /// <summary>
        /// Loads a SoundEffect from a stream as a .wav
        /// </summary>
        /// <param name="stream">The .wav file to load as a SoundEffect</param>
        /// <returns>The .wav data as a SoundEffect</returns>
        public static SoundEffect LoadSound(Stream stream)
        {
            return SoundEffect.FromStream(stream);
        }

        /// <summary>
        /// Loads a Texture2D from a byte array through the Content Manager (.xnb) 
        /// </summary>
        /// <param name="data">The .xnb file to load as a Texture2D</param>
        /// <returns>The .xnb data as a Texture2D</returns>
        public static Texture2D LoadImage(byte[] data)
        {
            return LoadContentObject<Texture2D>(data);
        }
        /// <summary>
        /// Loads a SoundEffect from a byte array through the Content Manager (.xnb) 
        /// </summary>
        /// <param name="data">The .xnb file to load as a SoundEffect</param>
        /// <returns>The .xnb data as a SoundEffect</returns>
        public static SoundEffect LoadSoundEffect(byte[] data)
        {
            return LoadContentObject<SoundEffect>(data);
        }
        /// <summary>
        /// Loads an Effect from a byte array through the Content Manager (.xnb) 
        /// </summary>
        /// <param name="data">The .xnb file to load as an Effect</param>
        /// <returns>The .xnb data as an Effect</returns>
        public static Effect LoadEffect(byte[] data)
        {
            return LoadContentObject<Effect>(data);
        }
        /// <summary>
        /// Loads a SpriteFont from a byte array through the Content Manager (.xnb) 
        /// </summary>
        /// <param name="data">The .xnb file to load as a SpriteFont</param>
        /// <returns>The .xnb data as a SpriteFont</returns>
        public static SpriteFont LoadFont(byte[] data)
        {
            return LoadContentObject<SpriteFont>(data);
        }

        /// <summary>
        /// Loads an object through the Content Manager (.xnb)
        /// </summary>
        /// <typeparam name="T">The type of the object to load</typeparam>
        /// <param name="data">The data of the object to load</param>
        /// <returns>The object loaded by the Content Manager</returns>
        public static T LoadContentObject<T>(byte[] data)
        {
            if (!Directory.Exists("Content\\MCT\\Temp"))
                Directory.CreateDirectory("Content\\MCT\\Temp");

            string tempP = Path.GetTempFileName(), temp = Path.GetFileName(tempP);
            File.WriteAllBytes(temp + ".xnb", data);

            T ret = Content.Load<T>("MCT\\Temp\\" + temp);

            File.Delete(temp + ".xnb");
            File.Delete(tempP);

            return ret;
        }

        /// <summary>
        /// Adds an IGameComponent to the Game Components
        /// </summary>
        /// <param name="component">The IGameComponent to add</param>
        public static void AddComponent(IGameComponent component)
        {
            API.main.Components.Add(component);
        }
        /// <summary>
        /// Adds a service provider to the Game Services
        /// </summary>
        /// <param name="serviceProvider">The service provider to add</param>
        public static void AddService(object serviceProvider)
        {
            API.main.Services.AddService(serviceProvider.GetType(), serviceProvider);
        }

        /// <summary>
        /// Adds an Invasion to the Game.
        /// </summary>
        /// <param name="base">The ModBase of the Invasion to add.</param>
        /// <param name="name">The (internal) name of the Invasion to add.</param>
        /// <param name="invasion">The Invasion to add.</param>
        /// <returns>The ID of the added Invasion.</returns>
        public static int AddInvasion(ModBase @base, string name, Invasion invasion)
        {
            invasion.ID = ++invasionNextType;

            Invasion.invasions.Add(@base.mod.InternalName + ":" + name, invasion);
            Invasion.invasionTypes.Add(invasion.ID, @base.mod.InternalName + ":" + name);

            return invasion.ID;
        }
    }
}

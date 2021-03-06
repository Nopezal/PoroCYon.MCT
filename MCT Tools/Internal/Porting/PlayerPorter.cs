﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Xna.Framework;
using Ionic.Zip;
using LitJson;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.Tools.Internal.Porting
{
    static class PlayerPorter
    {
        // not a very good key...
        readonly static byte[] unicodeKey = new byte[]
        {
            104, 0, 51, 0, 121, 0, 95, 0, 103, 0, 85, 0, 121, 0, 90, 0
        };

        static JsonData AddColour(Color c)
        {
            return AddColour(JsonMapper.ToObject("[]"), c);
        }
        static JsonData AddColour(JsonData data, Color c)
        {
            data.Add((int)c.R);
            data.Add((int)c.G);
            data.Add((int)c.B);

            return data;
        }

        internal static void WriteItem(BinBuffer bb, Item toWrite)
        {
            if (toWrite.stack < 0 || toWrite.netID > Main.maxItemTypes || toWrite.netID < -48)
                toWrite.stack = 0;

            bb.Write(toWrite.stack);

            if (toWrite.stack <= 0)
                return;

            bb.Write((short)toWrite.netID);

            bb.Write((int)toWrite.prefix);

            bb.Write((byte)0); // mod data (none ofc)
        }

        internal static byte[] Read7BitContinuous(BinBuffer bb)
        {
            BigInteger ret = 0;

            // temporary store data
            byte data = 0;
            do
            {
                // read 1 byte from the stream
                int r = bb.ReadByte();
                if (r == -1)
                    break;
                data = (byte)r;

                ret <<= 7; // push data 7 bits to the left
                ret |= (uint)(data & 0x7f); // set 7 rightmost bits (01111111, 0x7f, or 127)
            }
            while ((data & 0x80) == 0x80); // while the 8th bit is 1 (10000000, or 0x80)

            // obvious enough
            return ret.ToByteArray();
        }
        internal unsafe static string ReadString(BinBuffer bb)
        {
            byte[] data = Read7BitContinuous(bb);

            int length;

            fixed (byte* ptr = &data[0])
            {
                length = *(int*)ptr;
            }

            return Encoding.UTF8.GetString(bb.ReadBytes(length));
        }

        internal static PlayerFile ReadPlayer(string path)
        {
            // warning: here be dragons (or worse)!

            BinBuffer bb;

            using (CryptoStream cs = new CryptoStream(new MemoryStream(File.ReadAllBytes(path)),
                new RijndaelManaged().CreateDecryptor(unicodeKey, unicodeKey), CryptoStreamMode.Read))
            {
                // copy the Stream to a BinBufferByte so it can be closed immediately.

                MemoryStream ms = new MemoryStream();
                cs.CopyTo(ms);

                bb = new BinBuffer(new BinBufferByte(ms.ToArray()));

                ms.Close();
                cs.Close();
            }

            PlayerFile ret = new PlayerFile();

            int ver = ret.version = bb.ReadInt();


            ret.name = ReadString(bb);

            if (ver >= 10)
            {
                if (ver >= 17)
                    ret.difficulty = (Difficulty)bb.ReadByte();
                else if (bb.ReadBool())
                    ret.difficulty = Difficulty.Hardcore;
            }

            ret.hair = bb.ReadInt();

            if (ver >= 82)
                ret.hairDye = bb.ReadByte();
            if (ver >= 83)
                ret.hideVisual = bb.ReadByte();

            ret.gender = ver <= 17
                ? (ret.hair == 5 || ret.hair == 6 || ret.hair == 9 || ret.hair == 11
                    ? Gender.Female
                    : Gender.Male)
                : (Gender)bb.ReadByte();

            ret.life = bb.ReadInt();
            ret.lifeMax = Math.Min(bb.ReadInt(), 500);
            ret.life = Math.Min(ret.lifeMax, ret.lifeMax);

            ret.mana = bb.ReadInt();
            ret.manaMax = Math.Min(bb.ReadInt(), 400);
            ret.mana = Math.Min(ret.mana, ret.manaMax);

            ret.hairColour = bb.ReadColorRGB();
            ret.skinColour = bb.ReadColorRGB();
            ret.eyeColour = bb.ReadColorRGB();
            ret.shirtColour = bb.ReadColorRGB();
            ret.undershirtColour = bb.ReadColorRGB();
            ret.pantsColour = bb.ReadColorRGB();
            ret.shoeColour = bb.ReadColorRGB();

            #region inventory
            if (ver < 38)
            {
                // screw this
                throw new FormatException("Player format too old.");
            }

            for (int i = 0; i < (ver >= 81 ? 16 : 11); i++)
                ret.armour[i] = new Item()
                {
                    netID = bb.ReadInt(),
                    stack = 1,
                    prefix = bb.ReadByte()
                };

            if (ver >= 47)
                for (int i = 0; i < (ver >= 81 ? 8 : 3); i++)
                    if (i < 3) // 1.2.2!
                        ret.dye[i] = new Item()
                        {
                            netID = bb.ReadInt(),
                            stack = 1,
                            prefix = bb.ReadByte()
                        };

            for (int i = 0; i < (ver >= 58 ? 58 : 48); i++)
                ret.inventory[i] = new Item()
                {
                    netID = bb.ReadInt(),
                    stack = bb.ReadInt(),
                    prefix = bb.ReadByte()
                };

            ret.inventory[58] = new Item(); // mouse item in tapi

            for (int i = 0; i < (ver >= 58 ? 40 : 20); i++)
                ret.piggyBank[i] = new Item()
                {
                    netID = bb.ReadInt(),
                    stack = bb.ReadInt(),
                    prefix = bb.ReadByte()
                };

            for (int i = 0; i < (ver >= 58 ? 40 : 20); i++)
                ret.safe[i] = new Item()
                {
                    netID = bb.ReadInt(),
                    stack = bb.ReadInt(),
                    prefix = bb.ReadByte()
                };
            #endregion

            // buffs
            if (ver >= 11)
                for (int i = 0; i < (ver >= 74 ? 22 : 10); i++)
                {
                    ret.buffType[i] = bb.ReadInt();
                    ret.buffTime[i] = bb.ReadInt();
                }

            // spawn
            for (int i = 0; i < 200; i++)
            {
                int spX = bb.ReadInt();

                if (spX <= -1)
                    break;

                ret.spX[i] = spX;
                ret.spY[i] = bb.ReadInt();
                ret.spI[i] = bb.ReadInt();
                ret.spN[i] = ReadString(bb);
            }

            if (ver >= 16)
                ret.hbLocked = bb.ReadBool();
            if (ver >= 98)
                ret.anglerQuestsFinished = bb.ReadInt();

            return ret;
        }
        internal static void WritePlayer(PlayerFile player)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                + "\\My Games\\Terraria\\tAPI\\Players\\" + player.name + ".plr";

            if (File.Exists(path))
                File.Delete(path);

            using (ZipFile zf = new ZipFile(path))
            {
                #region info.json
                JsonData info = JsonMapper.ToObject("{}");

                JsonData format = JsonMapper.ToObject("{}");

                format["release"] = API.versionRelease;
                format["subrelease"] = API.versionSubrelease;

                info["format"] = format;

                info["name"] = player.name;
                info["difficulty"] = (byte)player.difficulty;
                info["hotbarLocked"] = player.hbLocked;

                JsonData life = JsonMapper.ToObject("[]");

                life.Add(player.life);
                life.Add(player.lifeMax);

                info["life"] = life;

                JsonData mana = JsonMapper.ToObject("[]");

                mana.Add(player.mana);
                mana.Add(player.manaMax);

                info["mana"] = mana;

                JsonData appearance = JsonMapper.ToObject("{}");

                appearance["male"] = player.gender == Gender.Male;
                appearance["hair"] = player.hair;

                JsonData colours = JsonMapper.ToObject("{}");

                colours["hair"] = AddColour(player.hairColour);
                colours["skin"] = AddColour(player.skinColour);
                colours["eye"] = AddColour(player.eyeColour);
                colours["shirt"] = AddColour(player.shirtColour);
                colours["undershirt"] = AddColour(player.undershirtColour);
                colours["pants"] = AddColour(player.pantsColour);
                colours["shoes"] = AddColour(player.shoeColour);

                appearance["colors"] = colours;

                info["appearance"] = appearance;

                string json = JsonMapper.ToJson(info);
                zf.AddEntry("Info.json", json);
                #endregion

                BinBuffer bb = new BinBuffer();

                #region data.dat
                for (int i = 0; i < player.armour.Length; i++)
                    WriteItem(bb, player.armour[i]);
                for (int i = 0; i < player.dye.Length; i++)
                    WriteItem(bb, player.dye[i]);
                for (int i = 0; i < player.inventory.Length; i++)
                    WriteItem(bb, player.inventory[i]);
                for (int i = 0; i < player.piggyBank.Length; i++)
                    WriteItem(bb, player.piggyBank[i]);
                for (int i = 0; i < player.safe.Length; i++)
                    WriteItem(bb, player.safe[i]);

                bb.Write((byte)player.buffType.Length);

                for (int i = 0; i < player.buffType.Length; i++)
                {
                    bb.Write((ushort)(player.buffType[i] >= Main.maxBuffTypes ? 0 : player.buffType[i]));

                    if (player.buffType[i] != 0 && player.buffType[i] < Main.maxBuffTypes)
                    {
                        bb.Write(player.buffTime[i]);

                        if (player.buffTime[i] != 0)
                            bb.Write((ushort)0); // mod data (none ofc)
                    }
                }

                for (int i = 0; i < 200; i++)
                {
                    if (player.spN[i] == null)
                    {
                        bb.Write(-1);

                        break;
                    }

                    bb.WriteX(player.spX[i], player.spY[i], player.spI[i]);
                    bb.Write(player.spN[i]);
                }
                #endregion

                bb.Pos = 0;

                zf.AddEntry("Data.dat", bb.ReadBytes());

                zf.Save();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Ionic.Zip;
using LitJson;
using TAPI;
using PoroCYon.MCT.Tools.Internal.Porting;
using Microsoft.Xna.Framework;

namespace PoroCYon.MCT.Tools
{
    /// <summary>
    /// Ports files in the Vanilla format to the tAPI format.
    /// </summary>
    public unsafe static class FilePorter
    {
        // warning: there be dragons (or worse) in the Read_ methods!

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
            data.Add(c.R);
            data.Add(c.G);
            data.Add(c.B);

            return data;
        }

        static void WriteItem(BinBuffer bb, Item toWrite)
        {
            if (toWrite.stack < 0)
                toWrite.stack = 0;

            bb.Write(toWrite.stack);

            if (toWrite.stack <= 0)
                return;

            bb.Write(toWrite.netID);

            bb.Write((int)toWrite.prefix);

            bb.Write((byte)0); // mod data (none ofc)
        }

        static PlayerFile ReadPlayer(string path)
        {
            // ._.
            BinBuffer bb = new BinBuffer(
                new BinBufferStream(
                    new CryptoStream(
                        new MemoryStream(File.ReadAllBytes(path)),
                        new RijndaelManaged().CreateDecryptor(unicodeKey, unicodeKey),
                        CryptoStreamMode.Read
                    )
                )
            );

            PlayerFile ret = new PlayerFile();

            int version = ret.version = bb.ReadInt();

            ret.name = bb.ReadString();

            if (version >= 10)
            {
                if (version >= 17)
                    ret.difficulty = (Difficulty)bb.ReadByte();
                else if (bb.ReadBool())
                    ret.difficulty = Difficulty.Hardcore;
            }

            ret.hair = bb.ReadInt();

            if (version >= 82)
                ret.hairDye = bb.ReadByte();
            if (version >= 83)
                ret.hideVisual = bb.ReadByte();

            ret.gender = version <= 17
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
            if (version < 38)
            {
                // screw this
                throw new FormatException("Player format too old.");
            }

            for (int i = 0; i < (version >= 81 ? 16 : 11); i++)
                ret.armour[i] = new Item()
                {
                    netID = bb.ReadInt(),
                    stack = 1,
                    prefix = bb.ReadByte()
                };

            if (version >= 47)
                for (int i = 0; i < (version >= 81 ? 8 : 3); i++)
                    if (i < 3) // 1.2.2!
                        ret.dye[i] = new Item()
                        {
                            netID = bb.ReadInt(),
                            stack = 1,
                            prefix = bb.ReadByte()
                        };

            for (int i = 0; i < (version >= 58 ? 58 : 48); i++)
                ret.inventory[i] = new Item()
                {
                    netID = bb.ReadInt(),
                    stack = bb.ReadInt(),
                    prefix = bb.ReadByte()
                };

            for (int i = 0; i < (version >= 58 ? 40 : 20); i++)
                ret.piggyBank[i] = new Item()
                {
                    netID = bb.ReadInt(),
                    stack = bb.ReadInt(),
                    prefix = bb.ReadByte()
                };

            for (int i = 0; i < (version >= 58 ? 40 : 20); i++)
                ret.safe[i] = new Item()
                {
                    netID = bb.ReadInt(),
                    stack = bb.ReadInt(),
                    prefix = bb.ReadByte()
                };
            #endregion

            // buffs
            if (version >= 11)
                for (int i = 0; i < (version < 74 ? 22 : 10); i++)
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
                ret.spN[i] = bb.ReadString();
            }

            if (version >= 16)
                ret.hbLocked = bb.ReadBool();
            if (version >= 98)
                ret.anglerQuestsFinished = bb.ReadInt();

            return ret;
        }
        static void WritePlayer(PlayerFile player)
        {
            using (ZipFile zf = new ZipFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                + "\\My Games\\Terraria\\tAPI\\Players\\" + player.name + ".plr"))
            {
                #region info.json
                JsonData info = JsonMapper.ToObject("{}");

                JsonData format = JsonMapper.ToObject("{}");

                format["release"   ] = API.versionRelease   ;
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

                JsonData appearance = JsonMapper.ToObject("{}");

                appearance["male"] = player.gender == Gender.Male;
                appearance["hair"] = player.hair;

                JsonData colours = JsonMapper.ToObject("{}");

                colours["hair" ] = AddColour(player.hairColour );
                colours["skin" ] = AddColour(player.skinColour );
                colours["eye"  ] = AddColour(player.eyeColour  );
                colours["shirt"] = AddColour(player.shirtColour);
                colours["undershirt"] = AddColour(player.undershirtColour);
                colours["pants"] = AddColour(player.pantsColour);
                colours["shoes"] = AddColour(player.shoeColour );

                appearance["colors"] = colours;

                zf.AddEntry("Info.json", JsonMapper.ToJson(info));
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
                    bb.Write((ushort)player.buffType[i]);

                    if (player.buffType[i] != 0)
                    {
                        bb.Write(player.buffTime[i]);

                        bb.Write(0); // mod data (none ofc)
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

        static WorldFile ReadWorld(string path)
        {
            WorldFile ret = new WorldFile();

            return ret;
        }
        static void WriteWorld(WorldFile world)
        {

        }

        /// <summary>
        /// Ports a Player file.
        /// </summary>
        /// <param name="path">The path to the Player file to port.</param>
        public static void PortPlayer(string path)
        {
            WritePlayer(ReadPlayer(path));
        }

        /// <summary>
        /// Ports a world file.
        /// </summary>
        /// <param name="path">The path to the world file to port.</param>
        public static void PortWorld(string path)
        {
            WriteWorld(ReadWorld(path));
        }
    }
}

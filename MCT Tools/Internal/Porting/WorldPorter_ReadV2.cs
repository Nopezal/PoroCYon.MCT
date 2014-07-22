using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.Tools.Internal.Porting
{
    static partial class WorldPorter
    {
        delegate LoadError ReadV2Del(ref WorldFile ret, BinBuffer bb);

        readonly static ReadV2Del[] read = { ReadFormat, ReadHeader, ReadTiles, ReadChests, ReadSigns, ReadNPCs, ReadFooter };

        static bool[] importance;
        static int[] positions;
        static int ver;

        static LoadError ReadFormat(ref WorldFile ret, BinBuffer bb)
        {
            positions = new int[bb.ReadShort()];
            for (int i = 0; i < positions.Length; i++)
                positions[i] = bb.ReadInt();

            importance = new bool[bb.ReadShort()];

            byte b = 0, b2 = 128;

            for (int i = 0; i < importance.Length; i++)
            {
                if (b2 == 128)
                {
                    b = bb.ReadByte();

                    b2 = 1;
                }
                else
                    b2 <<= 1;

                if ((b & b2) == b2)
                    importance[i] = true;
            }

            return LoadError.Success;
        }
        static LoadError ReadHeader(ref WorldFile ret, BinBuffer bb)
        {
            ret.name = bb.ReadString();
            ret.ID = bb.ReadInt();
            ret.bounds = new Vector4(bb.ReadInt(), bb.ReadInt(), bb.ReadInt(), bb.ReadInt());
            ret.size = new Point(bb.ReadInt(), bb.ReadInt());
            ret.moonType = bb.ReadInt();

            for (int i = 0; i < ret.treeX.Length; i++)
                ret.treeX[i] = bb.ReadInt();
            for (int i = 0; i < ret.treeStyle.Length; i++)
                ret.treeStyle[i] = bb.ReadInt();
            for (int i = 0; i < ret.caveBackX.Length; i++)
                ret.caveBackX[i] = bb.ReadInt();
            for (int i = 0; i < ret.caveBackStyle.Length; i++)
                ret.caveBackStyle[i] = bb.ReadInt();

            ret.iceBackStyle = bb.ReadInt();
            ret.jungleBackStyle = bb.ReadInt();
            ret.hellBackStyle = bb.ReadInt();

            ret.spawn = new Point(bb.ReadInt(), bb.ReadInt());

            ret.surface = bb.ReadDouble();
            ret.rockLayer = bb.ReadDouble();
            ret.time = bb.ReadDouble();
            ret.day = bb.ReadBool();
            ret.moonPhase = bb.ReadInt();
            ret.bloodMoon = bb.ReadBool();
            ret.eclipse = bb.ReadBool();
            ret.dungeon = new Point(bb.ReadInt(), bb.ReadInt());
            ret.crimson = bb.ReadBool();
            ret.DefeatedEoC = bb.ReadBool();
            ret.DefeatedEoW = bb.ReadBool();
            ret.DefeatedSkeletron = bb.ReadBool();
            ret.DefeatedQueenBee = bb.ReadBool();
            ret.DefeatedDestroyer = bb.ReadBool();
            ret.DefeatedTwins = bb.ReadBool();
            ret.DefeatedSkeletronPrime = bb.ReadBool();
            bb.ReadBool(); // downedMechBossAny. Pretty redundant.
            ret.SavedGoblin = bb.ReadBool();
            ret.SavedWizard = bb.ReadBool();
            ret.SavedMechanic = bb.ReadBool();
            ret.DefeatedGoblins = bb.ReadBool();
            ret.DefeatedFrostLegion = bb.ReadBool();
            ret.DefeatedPirates = bb.ReadBool();
            ret.SmashedShadowOrb = bb.ReadBool();
            ret.MeteorHasLanded = bb.ReadBool();
            ret.shadowOrbsSmashed = bb.ReadByte();
            ret.altarsSmashed = bb.ReadInt();
            ret.Hardmode = bb.ReadBool();
            ret.invasionDelay = bb.ReadInt();
            ret.invasionSize = bb.ReadInt();
            ret.invasionType = bb.ReadInt();
            ret.invasionX = bb.ReadDouble();
            ret.Raining = bb.ReadBool();
            ret.rainTime = bb.ReadInt();
            ret.maxRain = bb.ReadFloat();
            ret.ore1Type = bb.ReadInt();
            ret.ore2Type = bb.ReadInt();
            ret.ore3Type = bb.ReadInt();

            for (int i = 0; i < ret.bgStyles.Length; i++)
                WorldGen.setBG(i, ret.bgStyles[i]);

            ret.cloudBgActive = bb.ReadInt();
            ret.cloudAmt = bb.ReadShort();
            ret.windSpeed = bb.ReadFloat();
            
            if (ver >= 95)
            {
                int amt = bb.ReadInt();
                for (int i = 0; i < amt; i++)
                    ret.finishedAnglerToday.Add(bb.ReadString());

                if (ver >= 99)
                {
                    ret.SavedAngler = bb.ReadBool();

                    if (ver >= 101)
                        ret.anglerQuest = bb.ReadInt();
                }
            }

            return LoadError.Success;
        }
        static LoadError ReadTiles(ref WorldFile ret, BinBuffer bb)
        {
            for (int x = 0; x < ret.size.X; x++)
                for (int y = 0; y < ret.size.Y; y++)
                {
                    int type = -1;

                    // what on earth is this?
                    byte b = 0, b2 = 0, b3 = bb.ReadByte();

                    if ((b3 & 1) != 0)
                    {
                        b2 = bb.ReadByte();
                        if ((b2 & 1) != 0)
                        {
                            b = bb.ReadByte();
                        }
                    }

                    if ((b3 & 2) != 0)
                    {
                        ret.tiles[x, y].Active = true;

                        if ((b3 & 32) != 0)
                            type = bb.ReadByte() | bb.ReadByte() << 8;
                        else
                            type = bb.ReadByte();

                        ret.tiles[x, y].type = (ushort)type;

                        if (importance[type])
                        {
                            ret.tiles[x, y].frameX = bb.ReadShort();
                            ret.tiles[x, y].frameY = bb.ReadShort();

                            if (type == 144)
                                ret.tiles[x, y].frameY = 0;
                        }
                        else
                            ret.tiles[x, y].frameX = ret.tiles[x, y].frameY = -1;

                        if ((b & 8) != 0)
                            ret.tiles[x, y].Colour = bb.ReadByte();
                    }
                    if ((b3 & 4) != 0)
                    {
                        ret.tiles[x, y].wall = bb.ReadByte();

                        if ((b & 16) != 0)
                            ret.tiles[x, y].WallColour = bb.ReadByte();
                    }

                    byte b4 = (byte)((b3 & 24) >> 3);
                    if (b4 != 0)
                    {
                        ret.tiles[x, y].liquid = bb.ReadByte();

                        if (b4 > 1)
                        {
                            if (b4 == 2)
                                ret.tiles[x, y].Lava = true;
                            else
                                ret.tiles[x, y].Honey = true;
                        }
                    }
                    if (b2 > 1)
                    {
                        ret.tiles[x, y].Wire  = (b2 & 2) != 0;
                        ret.tiles[x, y].Wire2 = (b2 & 4) != 0;
                        ret.tiles[x, y].Wire3 = (b2 & 8) != 0;

                        b4 = (byte)((b2 & 112) >> 4);
                        if (b4 != 0)
                        {
                            if (b4 == 1)
                                ret.tiles[x, y].HalfBrick = true;
                            else
                                ret.tiles[x, y].Slope = (byte)(b4 - 1);
                        }
                    }
                    if (b > 0)
                    {
                        ret.tiles[x, y].Actuator = (b & 2) != 0;
                        ret.tiles[x, y].Inactive = (b & 4) != 0;
                    }

                    b4 = (byte)((b3 & 192) >> 6);
                    if (b4 == 1)
                        bb.ReadByte();
                    else if (b4 != 0)
                        bb.ReadShort();
                }

            return LoadError.Success;
        }
        static LoadError ReadChests(ref WorldFile ret, BinBuffer bb)
        {
            int amt = bb.ReadShort();

            int rItemAmt = bb.ReadShort(), itemAmt, garbageDataAmt;

            if (rItemAmt < Terraria.Chest.maxItems)
            {
                itemAmt = rItemAmt;
                garbageDataAmt = 0;
            }
            else
                garbageDataAmt = rItemAmt - (itemAmt = Terraria.Chest.maxItems);

            for (int i = 0; i < ret.chests.Length; i++)
            {
                if (i >= amt)
                {
                    ret.chests[i] = null;

                    continue;
                }

                ret.chests[i] = new Chest()
                {
                    position = new Point(bb.ReadInt(), bb.ReadInt()),
                    name = bb.ReadString()
                };

                for (int j = 0; j < itemAmt; j++)
                {
                    Item it = new Item();

                    short stack = Math.Abs(bb.ReadShort());

                    if (stack <= 0)
                        continue;

                    it.netID = bb.ReadInt();
                    it.prefix = bb.ReadByte();
                    it.stack = stack;

                    ret.chests[i].items[j] = it;
                }
                for (int j = 0; i < garbageDataAmt; j++)
                    if (bb.ReadShort() > 0)
                    {
                        bb.ReadInt();
                        bb.ReadByte();
                    }
            }

            return LoadError.Success;
        }
        static LoadError ReadSigns(ref WorldFile ret, BinBuffer bb)
        {


            return LoadError.Success;
        }
        static LoadError ReadNPCs(ref WorldFile ret, BinBuffer bb)
        {


            return LoadError.Success;
        }
        static LoadError ReadFooter(ref WorldFile ret, BinBuffer bb)
        {


            return LoadError.Success;
        }

        static LoadError ReadV2(ref WorldFile ret, BinBuffer bb)
        {
            ver = ret.version;

            for (int i = 0; i < read.Length; i++)
            {
                var le = read[i](ref ret, bb);

                if (le != LoadError.Success)
                    return le;
                if (bb.Pos != positions[i])
                    return LoadError.InvalidBufferLength;
            }

            return LoadError.Success;
        }
    }
}

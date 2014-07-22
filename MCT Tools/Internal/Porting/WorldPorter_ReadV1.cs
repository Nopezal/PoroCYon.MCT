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
        static void InitFrameImportant()
        {
            TileDef.frameImportant[36] = true;
            TileDef.frameImportant[171] = true;
            TileDef.frameImportant[247] = true;
            TileDef.frameImportant[245] = true;
            TileDef.frameImportant[246] = true;
            TileDef.frameImportant[239] = true;
            TileDef.frameImportant[240] = true;
            TileDef.frameImportant[241] = true;
            TileDef.frameImportant[242] = true;
            TileDef.frameImportant[243] = true;
            TileDef.frameImportant[244] = true;
            TileDef.frameImportant[254] = true;
            TileDef.frameImportant[237] = true;
            TileDef.frameImportant[238] = true;
            TileDef.frameImportant[235] = true;
            TileDef.frameImportant[236] = true;
            TileDef.frameImportant[233] = true;
            TileDef.frameImportant[227] = true;
            TileDef.frameImportant[228] = true;
            TileDef.frameImportant[231] = true;
            TileDef.frameImportant[216] = true;
            TileDef.frameImportant[217] = true;
            TileDef.frameImportant[218] = true;
            TileDef.frameImportant[219] = true;
            TileDef.frameImportant[220] = true;
            TileDef.frameImportant[165] = true;
            TileDef.frameImportant[209] = true;
            TileDef.frameImportant[215] = true;
            TileDef.frameImportant[210] = true;
            TileDef.frameImportant[212] = true;
            TileDef.frameImportant[207] = true;
            TileDef.frameImportant[178] = true;
            TileDef.frameImportant[184] = true;
            TileDef.frameImportant[185] = true;
            TileDef.frameImportant[186] = true;
            TileDef.frameImportant[187] = true;
            TileDef.frameImportant[173] = true;
            TileDef.frameImportant[174] = true;
            TileDef.frameImportant[139] = true;
            TileDef.frameImportant[149] = true;
            TileDef.frameImportant[142] = true;
            TileDef.frameImportant[143] = true;
            TileDef.frameImportant[144] = true;
            TileDef.frameImportant[136] = true;
            TileDef.frameImportant[137] = true;
            TileDef.frameImportant[138] = true;
            TileDef.frameImportant[201] = true;
            TileDef.frameImportant[3] = true;
            TileDef.frameImportant[4] = true;
            TileDef.frameImportant[5] = true;
            TileDef.frameImportant[10] = true;
            TileDef.frameImportant[11] = true;
            TileDef.frameImportant[12] = true;
            TileDef.frameImportant[13] = true;
            TileDef.frameImportant[14] = true;
            TileDef.frameImportant[15] = true;
            TileDef.frameImportant[16] = true;
            TileDef.frameImportant[17] = true;
            TileDef.frameImportant[18] = true;
            TileDef.frameImportant[19] = true;
            TileDef.frameImportant[20] = true;
            TileDef.frameImportant[21] = true;
            TileDef.frameImportant[24] = true;
            TileDef.frameImportant[26] = true;
            TileDef.frameImportant[27] = true;
            TileDef.frameImportant[28] = true;
            TileDef.frameImportant[29] = true;
            TileDef.frameImportant[31] = true;
            TileDef.frameImportant[33] = true;
            TileDef.frameImportant[34] = true;
            TileDef.frameImportant[35] = true;
            TileDef.frameImportant[42] = true;
            TileDef.frameImportant[50] = true;
            TileDef.frameImportant[55] = true;
            TileDef.frameImportant[61] = true;
            TileDef.frameImportant[71] = true;
            TileDef.frameImportant[72] = true;
            TileDef.frameImportant[73] = true;
            TileDef.frameImportant[74] = true;
            TileDef.frameImportant[77] = true;
            TileDef.frameImportant[78] = true;
            TileDef.frameImportant[79] = true;
            TileDef.frameImportant[81] = true;
            TileDef.frameImportant[82] = true;
            TileDef.frameImportant[83] = true;
            TileDef.frameImportant[84] = true;
            TileDef.frameImportant[85] = true;
            TileDef.frameImportant[86] = true;
            TileDef.frameImportant[87] = true;
            TileDef.frameImportant[88] = true;
            TileDef.frameImportant[89] = true;
            TileDef.frameImportant[90] = true;
            TileDef.frameImportant[91] = true;
            TileDef.frameImportant[92] = true;
            TileDef.frameImportant[93] = true;
            TileDef.frameImportant[94] = true;
            TileDef.frameImportant[95] = true;
            TileDef.frameImportant[96] = true;
            TileDef.frameImportant[97] = true;
            TileDef.frameImportant[98] = true;
            TileDef.frameImportant[99] = true;
            TileDef.frameImportant[101] = true;
            TileDef.frameImportant[102] = true;
            TileDef.frameImportant[103] = true;
            TileDef.frameImportant[104] = true;
            TileDef.frameImportant[105] = true;
            TileDef.frameImportant[100] = true;
            TileDef.frameImportant[106] = true;
            TileDef.frameImportant[110] = true;
            TileDef.frameImportant[113] = true;
            TileDef.frameImportant[114] = true;
            TileDef.frameImportant[125] = true;
            TileDef.frameImportant[126] = true;
            TileDef.frameImportant[128] = true;
            TileDef.frameImportant[129] = true;
            TileDef.frameImportant[132] = true;
            TileDef.frameImportant[133] = true;
            TileDef.frameImportant[134] = true;
            TileDef.frameImportant[135] = true;
            TileDef.frameImportant[141] = true;
        }
        static void InitSolid()
        {
            TileDef.solid[232] = true;
            TileDef.solid[239] = true;
            TileDef.solid[170] = true;
            TileDef.solid[221] = true;
            TileDef.solid[229] = true;
            TileDef.solid[230] = true;
            TileDef.solid[222] = true;
            TileDef.solid[223] = true;
            TileDef.solid[224] = true;
            TileDef.solid[225] = true;
            TileDef.solid[226] = true;
            TileDef.solid[235] = true;
            TileDef.solid[191] = true;
            TileDef.solid[211] = true;
            TileDef.solid[208] = true;
            TileDef.solid[192] = true;
            TileDef.solid[193] = true;
            TileDef.solid[194] = true;
            TileDef.solid[195] = true;
            TileDef.solid[200] = true;
            TileDef.solid[203] = true;
            TileDef.solid[204] = true;
            TileDef.solid[189] = true;
            TileDef.solid[190] = true;
            TileDef.solid[198] = true;
            TileDef.solid[206] = true;
            TileDef.solid[248] = true;
            TileDef.solid[249] = true;
            TileDef.solid[250] = true;
            TileDef.solid[251] = true;
            TileDef.solid[252] = true;
            TileDef.solid[253] = true;
            TileDef.solid[202] = true;
            TileDef.solid[188] = true;
            TileDef.solid[179] = true;
            TileDef.solid[180] = true;
            TileDef.solid[181] = true;
            TileDef.solid[182] = true;
            TileDef.solid[183] = true;
            TileDef.solid[196] = true;
            TileDef.solid[197] = true;
            TileDef.solid[175] = true;
            TileDef.solid[176] = true;
            TileDef.solid[177] = true;
            TileDef.solid[162] = true;
            TileDef.solid[163] = true;
            TileDef.solid[164] = true;
            TileDef.solid[234] = true;
            TileDef.solid[137] = true;
            TileDef.solid[160] = true;
            TileDef.solid[161] = true;
            TileDef.solid[145] = true;
            TileDef.solid[146] = true;
            TileDef.solid[147] = true;
            TileDef.solid[148] = true;
            TileDef.solid[138] = true;
            TileDef.solid[140] = true;
            TileDef.solid[151] = true;
            TileDef.solid[152] = true;
            TileDef.solid[153] = true;
            TileDef.solid[154] = true;
            TileDef.solid[155] = true;
            TileDef.solid[156] = true;
            TileDef.solid[157] = true;
            TileDef.solid[158] = true;
            TileDef.solid[159] = true;
            TileDef.solid[127] = true;
            TileDef.solid[130] = true;
            TileDef.solid[107] = true;
            TileDef.solid[108] = true;
            TileDef.solid[111] = true;
            TileDef.solid[109] = true;
            TileDef.solid[110] = false;
            TileDef.solid[112] = true;
            TileDef.solid[116] = true;
            TileDef.solid[117] = true;
            TileDef.solid[123] = true;
            TileDef.solid[118] = true;
            TileDef.solid[119] = true;
            TileDef.solid[120] = true;
            TileDef.solid[121] = true;
            TileDef.solid[122] = true;
            TileDef.solid[150] = true;
            TileDef.solid[199] = true;
            TileDef.solid[0] = true;
            TileDef.solid[1] = true;
            TileDef.solid[2] = true;
            TileDef.solid[3] = false;
            TileDef.solid[4] = false;
            TileDef.solid[5] = false;
            TileDef.solid[6] = true;
            TileDef.solid[7] = true;
            TileDef.solid[8] = true;
            TileDef.solid[9] = true;
            TileDef.solid[166] = true;
            TileDef.solid[167] = true;
            TileDef.solid[168] = true;
            TileDef.solid[169] = true;
            TileDef.solid[10] = true;
            TileDef.solid[11] = false;
            TileDef.solid[19] = true;
            TileDef.solid[22] = true;
            TileDef.solid[23] = true;
            TileDef.solid[25] = true;
            TileDef.solid[30] = true;
            TileDef.solid[37] = true;
            TileDef.solid[38] = true;
            TileDef.solid[39] = true;
            TileDef.solid[40] = true;
            TileDef.solid[41] = true;
            TileDef.solid[43] = true;
            TileDef.solid[44] = true;
            TileDef.solid[45] = true;
            TileDef.solid[46] = true;
            TileDef.solid[47] = true;
            TileDef.solid[48] = true;
            TileDef.solid[53] = true;
            TileDef.solid[54] = true;
            TileDef.solid[56] = true;
            TileDef.solid[57] = true;
            TileDef.solid[58] = true;
            TileDef.solid[59] = true;
            TileDef.solid[60] = true;
            TileDef.solid[63] = true;
            TileDef.solid[64] = true;
            TileDef.solid[65] = true;
            TileDef.solid[66] = true;
            TileDef.solid[67] = true;
            TileDef.solid[68] = true;
            TileDef.solid[75] = true;
            TileDef.solid[76] = true;
            TileDef.solid[70] = true;
        }

        unsafe static LoadError ReadV1(ref WorldFile ret, BinBuffer bb)
        {
            // please help me, I'm starting to code C/C++ in C#

            InitFrameImportant();
            InitSolid();

            int ver = ret.version;
            if (ver > 102 /* 1.2.4.1 */)
                return LoadError.InvalidVersion;

            #region global stuff
            ret.name = bb.ReadString();
            ret.ID = bb.ReadInt();
            ret.bounds = new Vector4(bb.ReadInt(), bb.ReadInt(), bb.ReadInt(), bb.ReadInt());
            ret.size = new Point(bb.ReadInt(), bb.ReadInt());
            ret.moonType = ver >= 63 ? bb.ReadByte() : backupRand.Next(Main.maxMoons);

            if (ver >= 44)
            {
                for (int i = 0; i < ret.treeX.Length; i++)
                    ret.treeX[i] = bb.ReadInt();
                for (int i = 0; i < ret.treeStyle.Length; i++)
                    ret.treeStyle[i] = bb.ReadInt();
            }
            if (ver >= 60)
            {
                for (int i = 0; i < ret.caveBackX.Length; i++)
                    ret.caveBackX[i] = bb.ReadInt();
                for (int i = 0; i < ret.caveBackStyle.Length; i++)
                    ret.caveBackStyle[i] = bb.ReadInt();

                ret.iceBackStyle = bb.ReadInt();

                if (ver >= 61)
                {
                    ret.jungleBackStyle = bb.ReadInt();
                    ret.hellBackStyle = bb.ReadInt();
                }
            }
            else
            {
                #region create random background styles
                const int maxValue = 8;

                if (ret.size.X == 4200)
                {
                    ret.caveBackX[0] = backupRand.Next(ret.size.X / 4, ret.size.X / 4 * 3);

                    ret.caveBackX[1] = ret.caveBackX[2] = ret.size.X;

                    ret.caveBackStyle[0] = backupRand.Next(maxValue);

                    do
                        ret.caveBackStyle[1] = backupRand.Next(maxValue);
                    while (ret.caveBackStyle[1] == ret.caveBackStyle[0]);
                }
                else if (ret.size.X == 6400)
                {
                    ret.caveBackX[0] = backupRand.Next(ret.size.X / 3 - ret.size.X / 5, ret.size.X / 3 + ret.size.X / 5);
                    ret.caveBackX[1] = backupRand.Next(ret.size.X / 3 * 2 - ret.size.X / 5, ret.size.X / 3 * 2 + ret.size.X / 5);

                    ret.caveBackX[2] = ret.size.X;

                    ret.caveBackStyle[0] = backupRand.Next(maxValue);

                    do
                        ret.caveBackStyle[1] = backupRand.Next(maxValue);
                    while (ret.caveBackStyle[1] == ret.caveBackStyle[0]);

                    do
                        ret.caveBackStyle[2] = backupRand.Next(maxValue);
                    while (ret.caveBackStyle[2] == ret.caveBackStyle[0] || ret.caveBackStyle[2] == ret.caveBackStyle[1]);
                }
                else
                {
                    ret.caveBackX[0] = backupRand.Next(ret.size.X / 4 - ret.size.X / 8, ret.size.X / 4 + ret.size.X / 8);
                    ret.caveBackX[1] = backupRand.Next(ret.size.X / 2 - ret.size.X / 8, ret.size.X / 2 + ret.size.X / 8);
                    ret.caveBackX[2] = backupRand.Next(ret.size.X / 4 * 3 - ret.size.X / 8, ret.size.X / 4 * 3 + ret.size.X / 8);

                    ret.caveBackStyle[0] = backupRand.Next(maxValue);

                    do
                        ret.caveBackStyle[1] = backupRand.Next(maxValue);
                    while (ret.caveBackStyle[1] == ret.caveBackStyle[0]);

                    do
                        ret.caveBackStyle[2] = backupRand.Next(maxValue);
                    while (ret.caveBackStyle[2] == ret.caveBackStyle[0] || ret.caveBackStyle[2] == ret.caveBackStyle[1]);

                    do
                        ret.caveBackStyle[3] = backupRand.Next(maxValue);
                    while (ret.caveBackStyle[3] == ret.caveBackStyle[0] || ret.caveBackStyle[3] == ret.caveBackStyle[1] || ret.caveBackStyle[3] == ret.caveBackStyle[2]);
                }

                ret.iceBackStyle = backupRand.Next(4);
                ret.hellBackStyle = backupRand.Next(3);
                ret.jungleBackStyle = backupRand.Next(2);
                #endregion
            }

            ret.spawn = new Point(bb.ReadInt(), bb.ReadInt());

            ret.surface = bb.ReadDouble();
            ret.rockLayer = bb.ReadDouble();
            ret.time = bb.ReadDouble();
            ret.day = bb.ReadBool();
            ret.moonPhase = bb.ReadInt() % 8;
            ret.bloodMoon = bb.ReadBool();

            if (ver >= 70)
                ret.eclipse = bb.ReadBool();

            ret.dungeon = new Point(bb.ReadInt(), bb.ReadInt());

            ret.crimson = ver >= 56 ? bb.ReadBool() : false;

            ret.DefeatedEoC = bb.ReadBool();
            ret.DefeatedEoW = bb.ReadBool();
            ret.DefeatedSkeletron = bb.ReadBool();

            if (ver >= 66)
                ret.DefeatedQueenBee = bb.ReadBool();
            if (ver >= 44)
            {
                ret.DefeatedDestroyer = bb.ReadBool();
                ret.DefeatedTwins = bb.ReadBool();
                ret.DefeatedSkeletronPrime = bb.ReadBool();
                bool downedAnyHM = bb.ReadBool(); // pretty redundant
            }
            if (ver >= 64)
            {
                ret.DefeatedPlantera = bb.ReadBool();
                ret.DefeatedGolem = bb.ReadBool();
            }
            if (ver >= 29)
            {
                ret.SavedGoblin = bb.ReadBool();
                ret.SavedWizard = bb.ReadBool();

                if (ver >= 34)
                {
                    ret.SavedMechanic = bb.ReadBool();

                    if (ver >= 80)
                        ret.SavedStylist = bb.ReadBool();
                }
            }
            if (ver >= 32)
                ret.DefeatedClown = bb.ReadBool();
            if (ver >= 37)
                ret.DefeatedFrostLegion = bb.ReadBool();
            if (ver >= 56)
                ret.DefeatedPirates = bb.ReadBool();

            ret.SmashedShadowOrb = bb.ReadBool();
            ret.MeteorHasLanded = bb.ReadBool();
            ret.shadowOrbsSmashed = bb.ReadByte();

            if (ver >= 23)
            {
                ret.altarsSmashed = bb.ReadInt();
                ret.Hardmode = bb.ReadBool();
            }

            ret.invasionDelay = bb.ReadInt();
            ret.invasionSize = bb.ReadInt();
            ret.invasionType = bb.ReadInt();
            ret.invasionX = bb.ReadDouble();

            if (ver >= 53)
            {
                ret.Raining = bb.ReadBool();
                ret.rainTime = bb.ReadInt();
                ret.maxRain = bb.ReadFloat();
            }
            if (ver >= 54)
            {
                ret.ore1Type = bb.ReadInt();
                ret.ore2Type = bb.ReadInt();
                ret.ore3Type = bb.ReadInt();
            }
            else if (ver >= 32 && ret.altarsSmashed == 0)
                ret.ore1Type = ret.ore2Type = ret.ore3Type = -1;
            else
            {
                ret.ore1Type = 107;
                ret.ore2Type = 108;
                ret.ore3Type = 111;
            }

            if (ver >= 55)
                for (int i = 0; i < 3; i++)
                    ret.bgStyles[i] = bb.ReadByte();
            if (ver >= 60)
            {
                for (int i = 3; i < 8; i++)
                    ret.bgStyles[i] = bb.ReadByte();

                ret.cloudBgActive = bb.ReadInt();
            }
            else
                ret.cloudBgActive = -backupRand.Next(8640, 86400);

            if (ver >= 62)
            {
                ret.cloudAmt = bb.ReadShort();
                ret.windSpeed = bb.ReadFloat();
            }
            else
            {
                ret.cloudAmt = (short)backupRand.Next(10, Main.cloudLimit);

                while (ret.windSpeed == 0f)
                    ret.windSpeed = WorldGen.genRand.Next(-100, 101) / 100f;
            }
            #endregion

            for (int i = 0; i < ret.bgStyles.Length; i++)
                WorldGen.setBG(i, ret.bgStyles[i]);

            ret.InitTiles();

            #region tiles
            // warning: lunacy ahead

            fixed (Tile* basePtr = &ret.tiles[0, 0])
            {
                var t = basePtr; // don't check the bounds over and over again. much faster.

                for (int i = 0; i < ret.size.X + ret.size.Y; i++)
                {
                    long offset = t - basePtr;
                    long x = offset % ret.size.X;
                    long y = (offset - x) / ret.size.X;

                    byte* compressed = (byte*)(t + t->CompressedDataOffset);

                    if (t->Active = bb.ReadBool())
                    {
                        t->type = ver > 77 ? bb.ReadUShort() : bb.ReadByte();

                        if (t->type == 127)
                            t->Active = false;

                        if (ver <= 72 && (t->type >= 35 || t->type <= 36 || t->type >= 170 || t->type <= 172))
                        {
                            t->frameX = bb.ReadShort();
                            t->frameY = bb.ReadShort();
                        }
                        else if (TileDef.frameImportant[t->type])
                        {
                            if (ver < 28 && t->type == 4)
                                t->frameX = t->frameY = 0;
                            else if (ver < 40 && t->type == 19)
                                t->frameX = t->frameY = 0;
                            else
                            {
                                t->frameX = bb.ReadShort();
                                t->frameY = bb.ReadShort();

                                if (t->type == 144)
                                    t->frameY = 0;
                            }
                        }
                        else
                            t->frameX = t->frameY = -1;

                        if (ver >= 84 && bb.ReadBool())
                            t->Colour = bb.ReadByte();

                        if (ver <= 25)
                            bb.ReadBool();

                        if (bb.ReadBool())
                        {
                            t->wall = bb.ReadByte();

                            if (ver >= 48 && bb.ReadBool())
                                t->WallColour = bb.ReadByte();
                        }
                        if (bb.ReadBool())
                        {
                            t->liquid = bb.ReadByte();

                            t->Lava = bb.ReadBool();

                            if (ver >= 51)
                                t->Honey = bb.ReadBool();
                        }
                        if (ver >= 33)
                            t->Wire = bb.ReadBool();
                        if (ver >= 43)
                        {
                            t->Wire2 = bb.ReadBool();
                            t->Wire3 = bb.ReadBool();
                        }
                        if (ver >= 41)
                        {
                            t->HalfBrick = bb.ReadBool() && !TileDef.solid[t->type];

                            if (ver >= 49)
                            {
                                t->Slope = bb.ReadByte();

                                if (!TileDef.solid[t->type])
                                    t->Slope = 0;
                            }
                        }
                        if (ver >= 42)
                        {
                            t->Actuator = bb.ReadBool();
                            t->Inactive = bb.ReadBool();
                        }

                        if (ver >= 25)
                            bb.ReadShort();
                    }

                    t += sizeof(Tile);
                }
            }
            #endregion

            #region chests
            int chestSize = ver < 58 ? 20 : 40;

            for (int i = 0; i < ret.chests.Length; i++)
            {
                if (!bb.ReadBool())
                    continue;

                ret.chests[i] = new Chest()
                {
                    position = new Point(bb.ReadInt(), bb.ReadInt())
                };

                if (ver >= 85)
                    ret.chests[i].name = bb.ReadString();

                for (int j = 0; j < ret.chests[i].items.Length; j++)
                {
                    ret.chests[i].items[j] = new Item();

                    if (j < chestSize)
                    {
                        Item it = ret.chests[i].items[j];

                        int stack = ver >= 59 ? bb.ReadShort() : bb.ReadByte();
                        if (stack <= 0)
                            continue;

                        if (ver < 38)
                        {
                            // screw this
                            throw new FormatException("World format too old.");
                        }

                        it.netID = bb.ReadInt();
                        it.stack = stack;
                        it.prefix = bb.ReadByte();
                    }
                }
            }
            #endregion

            #region signs
            for (int i = 0; i < ret.signs.Length; i++)
            {
                if (!bb.ReadBool())
                    continue;

                ret.signs[i] = new Sign()
                {
                    text = bb.ReadString(),
                    position = new Point(bb.ReadInt(), bb.ReadInt())
                };
            }
            #endregion

            #region town npcs
            while (bb.ReadBool())
            {
                TownNPC n = new TownNPC()
                {
                    occupation = bb.ReadString()
                };

                if (ver >= 83)
                    n.name = bb.ReadString();

                n.position = new Vector2(bb.ReadFloat(), bb.ReadFloat());
                n.homeless = bb.ReadBool();
                n.homeTile = new Point(bb.ReadInt(), bb.ReadInt());

                NPC npc = new NPC();
                npc.SetDefaults(n.occupation);
                n.type = npc.type;

                ret.townNPCs.Add(n);
            }

            if (ver >= 31 && ver <= 83)
            {
                string
                    merchantName = bb.ReadString(),
                    nurseName = bb.ReadString(),
                    armsDealerName = bb.ReadString(),
                    dryadName = bb.ReadString(),
                    guideName = bb.ReadString(),
                    clothierName = bb.ReadString(),
                    demolitionistName = bb.ReadString(),
                    goblinName = bb.ReadString(),
                    wizardName = bb.ReadString(),

                    mechanicName = String.Empty,
                    truffleName = String.Empty,
                    steampunkerName = String.Empty,
                    dyeTraderName = String.Empty,
                    partyGirlName = String.Empty,
                    cyborgName = String.Empty,
                    painterName = String.Empty,
                    witchDoctorName = String.Empty,
                    pirateName = String.Empty,
                    anglerName = String.Empty;

                if (ver >= 35)
                {
                    mechanicName = bb.ReadString();

                    if (ver >= 65)
                    {
                        truffleName = bb.ReadString();
                        steampunkerName = bb.ReadString();
                        dyeTraderName = bb.ReadString();
                        partyGirlName = bb.ReadString();
                        cyborgName = bb.ReadString();
                        painterName = bb.ReadString();
                        witchDoctorName = bb.ReadString();
                        pirateName = bb.ReadString();

                        if (ver >= 79)
                            anglerName = bb.ReadString();
                    }
                }

                for (int i = 0; i < ret.townNPCs.Count; i++)
                {
                    TownNPC n = ret.townNPCs[i];

                    switch (n.occupation.ToLowerInvariant()) // can't remember if it's First second or First Second...
                    {
                        case "merchant":
                            n.name = merchantName;
                            break;
                        case "nurse":
                            n.name = merchantName;
                            break;
                        case "arms dealer":
                            n.name = armsDealerName;
                            break;
                        case "dryad":
                            n.name = dryadName;
                            break;
                        case "guide":
                            n.name = guideName;
                            break;
                        case "clothier":
                            n.name = clothierName;
                            break;
                        case "demolitionist":
                            n.name = demolitionistName;
                            break;
                        case "goblin tinkerer":
                            n.name = goblinName;
                            break;
                        case "wizard":
                            n.name = wizardName;
                            break;
                        case "mechanic":
                            n.name = mechanicName;
                            break;
                        case "truffle":
                            n.name = truffleName;
                            break;
                        case "steampunker":
                            n.name = steampunkerName;
                            break;
                        case "dye trader":
                            n.name = dyeTraderName;
                            break;
                        case "party girl":
                            n.name = partyGirlName;
                            break;
                        case "cyborg":
                            n.name = cyborgName;
                            break;
                        case "painter":
                            n.name = painterName;
                            break;
                        case "witch doctor":
                            n.name = witchDoctorName;
                            break;
                        case "pirate":
                            n.name = pirateName;
                            break;
                        case "angler":
                            n.name = anglerName;
                            break;
                    }
                }
                // apply names..........................................
            }
            #endregion

            if (ver < 7)
                return LoadError.Success;

            // this is not a checksum, red.
            return bb.ReadBool() && (bb.ReadString() == ret.name || bb.ReadInt() == ret.ID)
                ? LoadError.Success
                : LoadError.InvalidChecksum;
        }
    }
}

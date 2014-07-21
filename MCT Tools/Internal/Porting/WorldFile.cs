using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PoroCYon.XnaExtensions;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.Tools.Internal.Porting
{
    using BitsByte = TAPI.BitsByte;

    enum CollisionType : sbyte
    {
        Unknown = -1,
        Inactive,
        Solid,
        Half,
        Slope // 3 or higher (slope type)
    }
    enum BlockType : byte
    {
        Normal,
        Half,
        Slope // 2 or higher (slope type)
    }
    enum LiquidType : byte
    {
        Water,
        Lava,
        Honey
    }

    struct Tile : ICloneable<Tile>
    {
        public ushort type;
        public byte wall, liquid;
        BitsByte bb1, bb2, bb3, bb4, bb5;
        public short frameX, frameY;

        short Word
        {
            get
            {
                return (short)(bb1 << 8 | bb2);
            }
            set
            {
                bb1 = (byte)((value & 0xFF00) >> 8);
                bb2 = (byte)(value & 0x00FF);
            }
        }

        public CollisionType CollisionType
        {
            get
            {
                if (!Active)
                    return CollisionType.Inactive;
                if (HalfBrick)
                    return CollisionType.Half;
                if (Slope > 0)
                    return (CollisionType)(Slope + (byte)CollisionType.Half);

                return CollisionType.Unknown;
            }
        }

        public Tile Copy()
        {
            return this; // it does not return this instance, but a copy...
        }
        public object Clone()
        {
            return this;
        }

        public int WallFrameX
        {
            get
            {
                return (bb4 & 15) * 36;
            }
            set
            {
                bb4 = (byte)((bb4 & 240) | (value / 36 & 15));
            }
        }
        public int WallFrameY
        {
            get
            {
                return (bb5 & 7) * 36;
            }
            set
            {
                bb2 = (byte)((bb2 & 248) | (value / 36 & 7));
            }
        }

        public byte FrameNumber
        {
            get
            {
                return (byte)((bb4 & 48) >> 4);
            }
            set
            {
                bb4 = (byte)((bb4 & 207) | ((value & 3) << 4));
            }
        }
        public byte WallFrameNumber
        {
            get
            {
                return (byte)((bb4 & 192) >> 6);
            }
            set
            {
                bb4 = (byte)((bb4 & 63) | ((value & 3) << 6));
            }
        }

        public bool TopSlope
        {
            get
            {
                return (Slope & 3) != 0;
            }
        }
        public bool BottomSlope
        {
            get
            {
                return Slope >= 3 && Slope < 5;
            }
        }
        public bool LeftSlope
        {
            get
            {
                return (Slope & 6) != 0;
            }
        }
        public bool RightSlope
        {
            get
            {
                return Slope == 1 || Slope == 3;
            }
        }

        public byte Slope
        {
            get
            {
                return (byte)((Word & 28672) >> 12);
            }
            set
            {
                Word = (short)((Word & 36863) | ((value & 7) << 12));
            }
        }

        public BlockType BlockType
        {
            get
            {
                if (HalfBrick)
                    return BlockType.Half;
                if (Slope > 0)
                    return (BlockType)(Slope + 1);

                return BlockType.Normal;
            }
        }

        public byte Colour
        {
            get
            {
                return (byte)(bb2 & 31);
            }
            set
            {
                bb2 = (byte)((bb2 & 224) | Math.Min(value, (byte)30));
            }
        }
        public byte WallColour
        {
            get
            {
                return (byte)(bb3 & 31);
            }
            set
            {
                bb3 = (byte)((bb3 & 224) | Math.Min(value, (byte)30));
            }
        }

        public bool Lava
        {
            get
            {
                return bb3[5];
            }
            set
            {
                bb3[5] = value;
                bb3[6] = false;
            }
        }
        public bool Honey
        {
            get
            {
                return bb3[6];
            }
            set
            {
                bb3[5] = false;
                bb3[6] = value;
            }
        }

        public LiquidType LiquidType
        {
            get
            {
                return (LiquidType)((bb3 & 96) >> 5);
            }
            set
            {
                switch (value)
                {
                    case LiquidType.Honey:
                        Honey = true;
                        break;
                    case LiquidType.Lava:
                        Lava = true;
                        break;
                    case LiquidType.Water:
                        bb3 &= 159;
                        break;
                }

                throw new ArgumentOutOfRangeException("value");
            }
        }

        public bool CheckingLiquid
        {
            get
            {
                return bb5[3];
            }
            set
            {
                bb5[3] = value;
            }
        }
        public bool SkipLiquid
        {
            get
            {
                return bb5[4];
            }
            set
            {
                bb5[4] = value;
            }
        }

        public bool Wire
        {
            get
            {
                return bb2[7];
            }
            set
            {
                bb2[7] = value;
            }
        }
        public bool Wire2
        {
            get
            {
                return bb1[0];
            }
            set
            {
                bb1[0] = value;
            }
        }
        public bool Wire3
        {
            get
            {
                return bb1[1];
            }
            set
            {
                bb1[1] = value;
            }
        }
        public bool HalfBrick
        {
            get
            {
                return bb1[2];
            }
            set
            {
                bb1[2] = value;
            }
        }
        public bool Actuator
        {
            get
            {
                return bb1[3];
            }
            set
            {
                bb1[3] = value;
            }
        }
        public bool NActive
        {
            get
            {
                return (Word & 96) == 32;
            }
        }
        public bool Inactive
        {
            get
            {
                return bb2[6];
            }
            set
            {
                bb2[6] = value;
            }
        }
        public bool Active
        {
            get
            {
                return bb2[5];
            }
            set
            {
                bb2[5] = value;
            }
        }

        public int CompressedDataOffset
        {
            get
            {
                return sizeof(ushort) + 2 * sizeof(byte);
            }
        }
    }
    class Chest
    {
        public Point position;
        public Item[] items = new Item[40];
        public string name;
    }
    class Sign
    {
        public Point position;
        public string text;
    }
    class TownNPC
    {
        public string occupation;
        public string name;
        public Vector2 position;
        public bool homeless;
        public Point homeTile;
    }

    class WorldFile
    {
        public int version;

        public string name;
        public int ID;
        public Vector4 bounds;
        public Point size;
        public int moonType;
        public int[] treeX = new int[3];
        public int[] treeStyle = new int[4];
        public int[] caveBackX = new int[3];
        public int[] caveBackStyle = new int[4];
        public int iceBackStyle, jungleBackStyle, hellBackStyle;
        public Point spawn;
        public double surface, rockLayer;
        public double time;
        public bool day;
        public int moonPhase;
        public bool bloodMoon, eclipse;
        public Point dungeon;
        public bool crimson;

        BitsByte defeatedNPCs1; // EoC, EoW, skeleton, queen bee, destroyer, twins, skeletron prime, plantera
        BitsByte defeatedNPCs2; // golem, goblins, clown, frost legion, pirates
        BitsByte savedNPCs; // goblin, wizard, mechanic, stylist
        BitsByte worldThings; // smashed shadow orb, meteor, hardmode, raining

        public byte shadowOrbsSmashed;
        public int altarsSmashed;
        public int invasionDelay, invasionSize, invasionType;
        public double invasionX;
        public int rainTime;
        public float maxRain;
        public int ore1Type, ore2Type, ore3Type; // hardmode ores, not copper, silver, ...
        public byte[] bgStyles = new byte[8];
        public float cloudBgActive;
        public short cloudAmt;
        public float windSpeed;

        public Tile[,] tiles;
        public Chest[] chests = new Chest[1000];
        public Sign [] signs  = new Sign [1000];
        public List<TownNPC> townNPCs = new List<TownNPC>();

        public bool DefeatedEoC
        {
            get
            {
                return defeatedNPCs1[0];
            }
            set
            {
                defeatedNPCs1[0] = value;
            }
        }
        public bool DefeatedEoW
        {
            get
            {
                return defeatedNPCs1[1];
            }
            set
            {
                defeatedNPCs1[1] = value;
            }
        }
        public bool DefeatedSkeletron
        {
            get
            {
                return defeatedNPCs1[2];
            }
            set
            {
                defeatedNPCs1[2] = value;
            }
        }
        public bool DefeatedQueenBee
        {
            get
            {
                return defeatedNPCs1[3];
            }
            set
            {
                defeatedNPCs1[3] = value;
            }
        }
        public bool DefeatedDestroyer
        {
            get
            {
                return defeatedNPCs1[4];
            }
            set
            {
                defeatedNPCs1[4] = value;
            }
        }
        public bool DefeatedTwins
        {
            get
            {
                return defeatedNPCs1[5];
            }
            set
            {
                defeatedNPCs1[5] = value;
            }
        }
        public bool DefeatedSkeletronPrime
        {
            get
            {
                return defeatedNPCs1[6];
            }
            set
            {
                defeatedNPCs1[6] = value;
            }
        }
        public bool DefeatedPlantera
        {
            get
            {
                return defeatedNPCs1[7];
            }
            set
            {
                defeatedNPCs1[7] = value;
            }
        }
        public bool DefeatedGolem
        {
            get
            {
                return defeatedNPCs2[0];
            }
            set
            {
                defeatedNPCs2[0] = value;
            }
        }
        public bool DefeatedGoblins
        {
            get
            {
                return defeatedNPCs2[1];
            }
            set
            {
                defeatedNPCs2[1] = value;
            }
        }
        public bool DefeatedClown
        {
            get
            {
                return defeatedNPCs2[2];
            }
            set
            {
                defeatedNPCs2[2] = value;
            }
        }
        public bool DefeatedFrostLegion
        {
            get
            {
                return defeatedNPCs2[3];
            }
            set
            {
                defeatedNPCs2[3] = value;
            }
        }
        public bool DefeatedPirates
        {
            get
            {
                return defeatedNPCs2[4];
            }
            set
            {
                defeatedNPCs2[4] = value;
            }
        }

        public bool SavedGoblin
        {
            get
            {
                return savedNPCs[0];
            }
            set
            {
                savedNPCs[0] = value;
            }
        }
        public bool SavedWizard
        {
            get
            {
                return savedNPCs[1];
            }
            set
            {
                savedNPCs[1] = value;
            }
        }
        public bool SavedMechanic
        {
            get
            {
                return savedNPCs[2];
            }
            set
            {
                savedNPCs[2] = value;
            }
        }
        public bool SavedStylist
        {
            get
            {
                return savedNPCs[3];
            }
            set
            {
                savedNPCs[3] = value;
            }
        }

        public bool SmashedShadowOrb
        {
            get
            {
                return worldThings[0];
            }
            set
            {
                worldThings[0] = value;
            }
        }
        public bool MeteorHasLanded
        {
            get
            {
                return worldThings[1];
            }
            set
            {
                worldThings[1] = value;
            }
        }
        public bool Hardmode
        {
            get
            {
                return worldThings[2];
            }
            set
            {
                worldThings[2] = value;
            }
        }
        public bool Raining
        {
            get
            {
                return worldThings[3];
            }
            set
            {
                worldThings[3] = value;
            }
        }

        public void InitTiles()
        {
            tiles = new Tile[size.X, size.Y];
        }
    }
}

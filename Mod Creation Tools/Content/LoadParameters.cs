using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using TAPI;

namespace PoroCYon.MCT.Content
{
    /// <summary>
    /// Common parameter values for ObjectLoader methods
    /// </summary>
    public class LoadParameters
    {
        /// <summary>
        /// The <see cref="TAPI.ModBase"/> of the parameters
        /// (not required for some methods)
        /// </summary>
        public ModBase ModBase;
        /// <summary>
        /// The mod assembly of the parameters
        /// (not required for some methods)
        /// </summary>
        public Assembly Assembly;
        /// <summary>
        /// The name of the object to be added
        /// </summary>
        public string Name;
        /// <summary>
        /// The name of the code class (which is in the given assembly)
        /// (not required for some methods)
        /// </summary>
        public string SubClassTypeName;

        /// <summary>
        /// Creates a new instance of the LoadParameters class
        /// </summary>
        /// <param name="name">Sets the Name field</param>
        /// <param name="base">Sets the ModBase field</param>
        /// <param name="subClassName">Sets the SubClassTypeName field</param>
        /// <param name="asm">Sets the Assembly field</param>
        public LoadParameters(string name, ModBase @base, string subClassName, Assembly asm = null)
        {
            Name = name;
            ModBase = @base;
            SubClassTypeName = subClassName;
			Assembly = (asm ?? @base.code) ?? @base.GetType().Assembly;
        }
    }

    /// <summary>
    /// Parameter values for armor items
    /// </summary>
    public class ArmorParameters
    {
        /// <summary>
        /// The texture of the helmet. null for no helmet.
        /// </summary>
        public Texture2D HeadTexture;
        /// <summary>
        /// The texture of the chainmail. null for no chainmail.
        /// </summary>
        public Texture2D BodyTexture;
        /// <summary>
        /// The texture of the chainmail for female players. null for no alternate texture.
        /// </summary>
        public Texture2D FemaleBodyTexture;
        /// <summary>
        /// The texture of the greaves. null for no greaves.
        /// </summary>
        public Texture2D LegsTexture;

        /// <summary>
        /// Creates a new instance of the ArmorParameters class
        /// </summary>
        /// <param name="head">Sets the HeadTexture field. null for no helmet.</param>
        /// <param name="body">Sets the BodyTexture field. null for no chainmail.</param>
        /// <param name="legs">Sets the LegsTexture field. null for no greaves.</param>
        /// <param name="femaleBody">Sets the FemaleBodyTexture field. null for no alternate texture.</param>
        public ArmorParameters(Texture2D head, Texture2D body, Texture2D legs, Texture2D femaleBody = null)
        {
            HeadTexture = head;
            BodyTexture = body;
            FemaleBodyTexture = femaleBody;
            LegsTexture = legs;
        }
    }
    /// <summary>
    /// Retur values for armor items
    /// </summary>
    public class ArmorReturnValues
    {
        /// <summary>
        /// The ID of the helmet. -1 for no helmet.
        /// </summary>
        public int HeadID;
        /// <summary>
        /// The ID of the chainmail. -1 for no chainmail.
        /// </summary>
        public int BodyID;
        /// <summary>
        /// The ID of the greaves. -1 for no greaves.
        /// </summary>
        public int LegsID;

        /// <summary>
        /// Creates a new instance of the ArmorReturnValues class
        /// </summary>
        /// <param name="head">The ID of the helmet. -1 for no helmet.</param>
        /// <param name="body">The ID of the chainmail. -1 for no chainmail.</param>
        /// <param name="legs">The ID of the greaves. -1 for no greaves.</param>
        public ArmorReturnValues(int head, int body, int legs)
        {
            HeadID = head;
            BodyID = body;
            LegsID = legs;
        }
    }

    /// <summary>
    /// Parameter values for Tiles
    /// </summary>
    public class TileParameters
    {
        /// <summary>
        /// The texture of the <see cref="Terraria.Tile" /> to add
        /// </summary>
        public Texture2D Texture;

        #region lots of stuff
        #region ints
        /// <summary>
        /// The width of the <see cref="Terraria.Tile" />
        /// </summary>
        public int Width;
        /// <summary>
        /// The height of the <see cref="Terraria.Tile" />
        /// </summary>
        public int Height;
        /// <summary>
        /// The frame width of the <see cref="Terraria.Tile" />
        /// </summary>
        public int FrameWidth;
        /// <summary>
        /// The frame height of the <see cref="Terraria.Tile" />
        /// </summary>
        public int FrameHeight;
        /// <summary>
        /// The amoutn of columns on the tile sheet
        /// </summary>
        public int SheetColumns;
        /// <summary>
        /// The amoutn of rows on the tile sheet
        /// </summary>
        public int SheetRows;
        /// <summary>
        /// The chance the tile has to sparkle
        /// </summary>
        public int ShineChance;
        /// <summary>
        /// The current frame
        /// </summary>
        public int Frame;
        /// <summary>
        /// The total amount of frames
        /// </summary>
        public int FrameCounter;
        #endregion

        #region bools
        int boolValues = 0;

        bool this[int index]
        {
            get
            {
                if (index < 0 || index > 31)
                    throw new IndexOutOfRangeException("An int holds only 32 bits!");

                return (boolValues & (1 << index)) != 0;
            }
            set
            {
                if (index < 0 || index > 31)
                    throw new IndexOutOfRangeException("An int holds only 32 bits!");

                if (value)
                    boolValues |= (byte)(1 << index);
                else
                    boolValues &= (byte)(1 << index);
            }
        }

        // byte 1
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> is solid or not
        /// </summary>
        public bool Solid
        {
            get
            {
                return this[0];
            }
            set
            {
                this[0] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> has a solid top or not (like Wooden Planks)
        /// </summary>
        public bool SolidTop
        {
            get
            {
                return this[1];
            }
            set
            {
                this[1] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> should only change frame when told or when tried to mine it
        /// </summary>
        public bool FrameImportant
        {
            get
            {
                return this[2];
            }
            set
            {
                this[2] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> is destroyed by 1 'tick' of the tool or not
        /// </summary>
        public bool BreaksFast
        {
            get
            {
                return this[3];
            }
            set
            {
                this[3] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> is mineable by a pickaxe or not
        /// </summary>
        public bool BreaksByPic
        {
            get
            {
                return this[4];
            }
            set
            {
                this[4] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> is mineable by an axe or not
        /// </summary>
        public bool BreaksByAxe
        {
            get
            {
                return this[5];
            }
            set
            {
                this[5] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> is mineable by a hammer or not
        /// </summary>
        public bool BreaksByHammer
        {
            get
            {
                return this[6];
            }
            set
            {
                this[6] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> breaks when hit with a melee item (sword, tool, ...), Projectile, ...
        /// </summary>
        public bool BreaksByCut
        {
            get
            {
                return this[7];
            }
            set
            {
                this[7] = value;
            }
        }

        // byte 2
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> breaks when it touches water or not
        /// </summary>
        public bool BreaksByWater
        {
            get
            {
                return this[8];
            }
            set
            {
                this[8] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> breaks when it touches lava or not
        /// </summary>
        public bool BreaksByLava
        {
            get
            {
                return this[9];
            }
            set
            {
                this[9] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> counts as a table or not
        /// </summary>
        public bool Table
        {
            get
            {
                return this[10];
            }
            set
            {
                this[10] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> counts as a rope or not
        /// </summary>
        public bool Rope
        {
            get
            {
                return this[11];
            }
            set
            {
                this[11] = value;
            }
        }
        /// <summary>
        /// Wether other tiles can be attached to <see cref="Terraria.Tile" /> tile or not
        /// </summary>
        public bool NoAttach
        {
            get
            {
                return this[12];
            }
            set
            {
                this[12] = value;
            }
        }
        /// <summary>
        /// Wether this <see cref="Terraria.Tile" /> counts as a dungeon tile or not
        /// </summary>
        public bool Dungeon
        {
            get
            {
                return this[13];
            }
            set
            {
                this[13] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> blocks any light or not (including sunlight)
        /// </summary>
        public bool BlocksAnyLight
        {
            get
            {
                return this[14];
            }
            set
            {
                this[14] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> blocks sunlight or not
        /// </summary>
        public bool BlocksSunlight
        {
            get
            {
                return this[15];
            }
            set
            {
                this[15] = value;
            }
        }

        // byte 3
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> merges with bricks
        /// </summary>
        public bool Brick
        {
            get
            {
                return this[16];
            }
            set
            {
                this[16] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> merges with moss
        /// </summary>
        public bool Moss
        {
            get
            {
                return this[17];
            }
            set
            {
                this[17] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> merges with stone
        /// </summary>
        public bool Stone
        {
            get
            {
                return this[18];
            }
            set
            {
                this[18] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> merges with dirt
        /// </summary>
        public bool Dirt
        {
            get
            {
                return this[19];
            }
            set
            {
                this[19] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> merges with sand or not
        /// </summary>
        public bool Sand
        {
            get
            {
                return this[20];
            }
            set
            {
                this[20] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> burns when you walk on it or not
        /// </summary>
        public bool Flame
        {
            get
            {
                return this[21];
            }
            set
            {
                this[21] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> counts as an alchemy recepient or not (like the Bowl, Vase, ...)
        /// </summary>
        public bool AlchemyFlower
        {
            get
            {
                return this[22];
            }
            set
            {
                this[22] = value;
            }
        }
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> glows or not
        /// </summary>
        public bool Glows
        {
            get
            {
                return this[23];
            }
            set
            {
                this[23] = value;
            }
        }

        // byte 4 *foreveralone*
        /// <summary>
        /// Wether the <see cref="Terraria.Tile" /> shines or not
        /// </summary>
        public bool Shines
        {
            get
            {
                return this[24];
            }
            set
            {
                this[24] = value;
            }
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// The type of the buff - buff, debuff, or a weapon buff
    /// This type is marked as Flags.
    /// </summary>
    [Flags]
    public enum BuffType : int
    {
        /// <summary>
        /// The buff is a positive buff
        /// </summary>
        Buff = 0,
        /// <summary>
        /// The buff is a negative buff and its effects cannot be cancelled by the player
        /// </summary>
        Debuff = 1,
        /// <summary>
        /// The buff boosts weapon stats
        /// </summary>
        WeaponBuff = 2
    }
    /// <summary>
    /// Parameter values for Buffs
    /// </summary>
    public class BuffParameters
    {
        /// <summary>
        /// The icon texture of the Buff to add
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// The description of the Buff to add
        /// </summary>
        public string Tip;
        /// <summary>
        /// Wether the Buff indicates that the player is having a pet that is meant for vanity purposes or not
        /// </summary>
        public bool VanityPet;
        /// <summary>
        /// Wether the Buff indicates that the player is having a pet that gives off light or not
        /// </summary>
        public bool LightPet;

        /// <summary>
        /// The type of the Buff to add
        /// </summary>
        public BuffType Type;

        /// <summary>
        /// Creates a new instance of the BuffParameters clas
        /// </summary>
        /// <param name="type">The type of the buff</param>
        /// <param name="texture">The texture of the buff</param>
        /// <param name="tip">The tooltip of the buff</param>
        /// <param name="vanityPet">Wether the buff functions as a vanity pet icon or not</param>
        /// <param name="lightPet">Wether the buff functions as a light-emitting pet icon or not</param>
        public BuffParameters(BuffType type, Texture2D texture, string tip, bool vanityPet, bool lightPet)
        {
            Type = type;
            Texture = texture;
            Tip = tip;
            VanityPet = vanityPet;
            LightPet = lightPet;
        }
    }
}

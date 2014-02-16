using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAPI.SDK.Content
{
    /// <summary>
    /// Common parameter values for ObjectLoader methods
    /// </summary>
    public struct LoadParameters
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
        /// Creates a new instance of the LoadParameters structure
        /// </summary>
        /// <param name="name">Sets the Name field</param>
        /// <param name="base">Sets the ModBase field</param>
        /// <param name="subClassName">Sets the SubClassTypeName field</param>
        /// <param name="asm">Sets the Assembly field</param>
        public LoadParameters(string name, ModBase @base, string subClassName, Assembly asm)
        {
            Name = name;
            ModBase = @base;
            SubClassTypeName = subClassName;
            Assembly = asm;
        }
    }

    /// <summary>
    /// Parameter values for armor items
    /// </summary>
    public struct ArmorParameters
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
        /// Creates a new instance of the ArmorParameters sturcture
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
    public struct ArmorReturnValues
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
        /// Creates a new instance of the ArmorReturnValues structure
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
    public struct TileParameters
    {
        /// <summary>
        /// The texture of the <see cref="TAPI.Tile" /> to add
        /// </summary>
        public Texture2D Texture;

        #region lots of stuff
        /// <summary>
        /// The width of the <see cref="TAPI.Tile" />
        /// </summary>
        public int Width;
        /// <summary>
        /// The height of the <see cref="TAPI.Tile" />
        /// </summary>
        public int Height;
        /// <summary>
        /// The frame width of the <see cref="TAPI.Tile" />
        /// </summary>
        public int FrameWidth;
        /// <summary>
        /// The frame height of the <see cref="TAPI.Tile" />
        /// </summary>
        public int FrameHeight;
        /// <summary>
        /// The amoutn of columns on the tile sheet
        /// </summary>
        public int SheetColumns;
        /// <summary>
        /// The amoutn of rows on the tile sheet
        /// </summary>
        public int SheetLines;
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

        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> is solid or not
        /// </summary>
        public bool Solid;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> has a solid top or not (like Wooden Planks)
        /// </summary>
        public bool SolidTop;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> should only change frame when told or when tried to mine it
        /// </summary>
        public bool FrameImportant;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> is destroyed by 1 'tick' of the tool or not
        /// </summary>
        public bool BreaksFast;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> is mineable by a pickaxe or not
        /// </summary>
        public bool BreaksByPic;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> is mineable by an axe or not
        /// </summary>
        public bool BreaksByAxe;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> is mineable by a hammer or not
        /// </summary>
        public bool BreaksByHammer;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> breaks when hit with a melee item (sword, tool, ...), Projectile, ...
        /// </summary>
        public bool BreaksByCut;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> breaks when it touches water or not
        /// </summary>
        public bool BreaksByWater;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> breaks when it touches lava or not
        /// </summary>
        public bool BreaksByLava;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> counts as a table or not
        /// </summary>
        public bool Table;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> counts as a rope or not
        /// </summary>
        public bool Rope;
        /// <summary>
        /// Wether other tiles can be attached to <see cref="TAPI.Tile" /> tile or not
        /// </summary>
        public bool NoAttach;
        /// <summary>
        /// Wether this <see cref="TAPI.Tile" /> counts as a dungeon tile or not
        /// </summary>
        public bool Dungeon;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> blocks any light or not (including sunlight)
        /// </summary>
        public bool BlocksAnyLight;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> blocks sunlight or not
        /// </summary>
        public bool BlocksSunlight;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> merges with bricks
        /// </summary>
        public bool Brick;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> merges with moss
        /// </summary>
        public bool Moss;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> merges with stone
        /// </summary>
        public bool Stone;
        /// <summary>
        /// Wether the Tiel merges with dirt
        /// </summary>
        public bool Dirt;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> merges with sand or not
        /// </summary>
        public bool Sand;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> burns when you walk on it or not
        /// </summary>
        public bool Flame;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> counts as an alchemy recepient or not (like the Bowl, Vase, ...)
        /// </summary>
        public bool AlchemyFlower;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> glows or not
        /// </summary>
        public bool Glows;
        /// <summary>
        /// Wether the <see cref="TAPI.Tile" /> shines or not
        /// </summary>
        public bool Shines;
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
    public struct BuffParameters
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
    }
}

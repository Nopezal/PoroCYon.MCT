using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using PoroCYon.MCT.Tools.Validation;
using PoroCYon.MCT.Tools.Validation.Entities;

namespace PoroCYon.MCT.Tools
{
    /// <summary>
    /// A complete mod.
    /// </summary>
    public class ModData
    {
        internal List<Item> items = new List<Item>();
        internal List<NPC> npcs = new List<NPC>();
        internal List<Projectile> projs = new List<Projectile>();
        internal List<Tile> tiles = new List<Tile>();
        internal List<Wall> walls = new List<Wall>();

        internal List<JsonFile> jsons = new List<JsonFile>();
        internal Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

        /// <summary>
        /// Gets the ModInfo of the mod.
        /// </summary>
        public ModInfo Info
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the mod options of the mod.
        /// </summary>
        public ModOptions Options
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the craft groups of the mod.
        /// </summary>
        public CraftGroups CraftGroups
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets all items in the mod.
        /// </summary>
        public ReadOnlyCollection<Item> Items
        {
            get
            {
                return items.AsReadOnly();
            }
        }
        /// <summary>
        /// Gets all NPCs in the mod.
        /// </summary>
        public ReadOnlyCollection<NPC> NPCs
        {
            get
            {
                return npcs.AsReadOnly();
            }
        }
        /// <summary>
        /// Gets all projectiles in the mod.
        /// </summary>
        public ReadOnlyCollection<Projectile> Projs
        {
            get
            {
                return projs.AsReadOnly();
            }
        }
        /// <summary>
        /// Gets all tiles in the mod.
        /// </summary>
        public ReadOnlyCollection<Tile> Tiles
        {
            get
            {
                return tiles.AsReadOnly();
            }
        }
        /// <summary>
        /// Gets all walls in the mod.
        /// </summary>
        public ReadOnlyCollection<Wall> Walls
        {
            get
            {
                return walls.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets all JSON files in the mod.
        /// </summary>
        public ReadOnlyCollection<JsonFile> JSONs
        {
            get
            {
                return jsons.AsReadOnly();
            }
        }
        /// <summary>
        /// Gets all non-JSON files in the mod.
        /// </summary>
        public IDictionary<string, byte[]> Files
        {
            get
            {
                return files;
            }
        }
        /// <summary>
        /// Gets the assembly of the mod.
        /// </summary>
        public Assembly Assembly
        {
            get;
            internal set;
        }
    }
}

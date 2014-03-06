using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;

namespace TAPI.SDK
{
    public sealed class MBase : ModBase
    {
        public MBase()
            : base()
        {

        }

        public override void OnLoad()
        {
            base.OnLoad();

            // hacky stuff #2

            // remove from list, but the file still exists

            // this is so it's using the .dll (shared objects) instead from some byte array (not so shared objects),
            // but that would make the modinfo display chrash. Adding this .tapi file and removing it from the list by code fixes it.

            modInfo = new ModInfo();
            modInterfaces = new List<ModInterface>();
            modItems = new List<ModItem>();
            modNPCs = new List<ModNPC>();
            modPlayers = new List<ModPlayer>();
            modPrefixes = new List<ModPrefix>();
            modProjectiles = new List<ModProjectile>();
            modWorlds = new List<ModWorld>();

            int tempIndex = modIndex;
            modIndex = -1;
            modName = "";
            fileName = "";
            textures = new Dictionary<string, Texture2D>();
            files = new Dictionary<string, byte[]>();
            code = null;

            Mods.loadOrder.RemoveAt(modIndex);

            Mods.modBases.Remove(this);
        }
    }
}

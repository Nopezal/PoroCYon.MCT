using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.MCT.ModControlling;
using TAPI;

namespace PoroCYon.MCT
{
    public sealed class MBase : ModBase
    {
        public MBase()
            : base()
        {

        }

        [CallPriority(Single.PositiveInfinity)]
        public override void OnAllModsLoaded()
        {
            base.OnAllModsLoaded();

            // hacky stuff #2

            // remove from list, but the file still exists

            // this is so it's using the .dll (shared objects) instead from some byte array (not so shared objects),
            // but that would make the modinfo display chrash. Adding this .tapi file and removing it from the list by code fixes it.

            ModController.UnloadMod(this);

            // idem here (see Mct.cs)
            /*
            modInfo = new ModInfo();
            modInterfaces = new List<ModInterface>();
            modItems = new List<ModItem>();
            modNPCs = new List<ModNPC>();
            modPlayers = new List<ModPlayer>();
            modPrefixes = new List<ModPrefix>();
            modProjectiles = new List<ModProjectile>();
            modWorlds = new List<ModWorld>();

            int tempIndex = modIndex;

            Mods.loadOrder.RemoveAt(tempIndex);
            Mods.loadOrderBackup.RemoveAt(tempIndex);

            Mods.dlls.Remove(GetType().Assembly);
            Mods.modJsons  .Remove(modName);
            Mods.modOptions.Remove(modName);

            modIndex = -1;
            modName = "";
            fileName = "";
            textures.Clear();
            files.Clear();
            code = null;

            Mods.modBases.Remove(this);
            */
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LitJson;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.ModControlling
{
    // Also hacky stuff. Lots of it.

    /// <summary>
    /// How to handle ModEntity attachments when there is already a ModEntity attached.
    /// </summary>
    public enum AttachMode
    {
        /// <summary>
        /// Set the ModEntity if there is no ModEntity attached. Otherwise, throw an exception.
        /// </summary>
        New,
        /// <summary>
        /// Overwrite the ModEntity if there is already one attached.
        /// </summary>
        Overwrite,
        /// <summary>
        /// Append the ModEntity to the array if there is already one attached.
        /// </summary>
        Append
    }

    /// <summary>
    /// Controls loaded mods and mod sets.
    /// </summary>
    /// <remarks>Mod set control is planned, and will be implemented when tAPI implements them.</remarks>
    public static class ModController
    {
        /// <summary>
        /// Gets the <see cref="Type" /> representation of <see cref="ModBase" />.
        /// </summary>
        public readonly static Type ModBaseType = typeof(ModBase);

        readonly static Type GlobalModAttrType = typeof(GlobalModAttribute);

        /// <summary>
        /// Loads a mod.
        /// </summary>
        /// <param name="asm">The assembly that contains the code of the mod.</param>
        /// <param name="info">The <see cref="ModInfo" /> of the mod, formatted as JSON.</param>
        /// <param name="baseType">The <see cref="Type" /> representing the <see cref="ModBase" /> of the mod.</param>
        /// <param name="index">Where to insert the mod in the list. Use -1 to append it to the list. Default is -1.</param>
        /// <param name="displayName">The display name of the mod. Use null to use the Name of <paramref name="asm" />. Default is null.</param>
        /// <param name="files">The dictionary of files of the mod, with as key the file name, and as value its binary content. Directories are separated by a single forward-slash ('/') character. Use null for an empty collection. Default is null.</param>
        /// <returns>The ModBase.</returns>
        public static ModBase LoadMod(Assembly asm, JsonData info, Type baseType, int index = -1, string displayName = null, Dictionary<string, byte[]> files = null)
        {
            if (!baseType.IsSubclassOf(ModBaseType) || baseType == ModBaseType)
                throw new ArgumentException(baseType + " is not a ModBase!");

            if (index == -1)
                index = Mods.modBases.Count;
            if (index < 0 && index > Mods.modBases.Count)
                throw new ArgumentOutOfRangeException("index");

            files = files ?? new Dictionary<string, byte[]>();

            displayName = displayName ?? asm.GetName().Name;

            List<Assembly> dlls = new List<Assembly>();

            #region load dllrefs
            if (info.Has("dllReferences"))
            {
                JsonData @ref = info["dllReferences"];

                for (int i = 0; i < @ref.Count; i++)
                {
                    string refStr = (string)@ref[i];
                    Assembly refAsm = null;

                    if (files.ContainsKey(refStr))
                        refAsm = Assembly.Load(files[refStr]);
                    else
                        try
                        {
                            refAsm = Assembly.Load(AssemblyName.GetAssemblyName(refStr));
                        }
                        catch
                        {
                            refAsm = Assembly.LoadFrom(refStr);
                        }

                    if (refAsm != null)
                    {
                        Mods.dlls.Add(refAsm);
                        dlls.Add(refAsm);
                    }
                }
            }
            #endregion

            ModBase mod;

            try
            {
                mod = (ModBase)Activator.CreateInstance(baseType);
            }
            catch (ReflectionTypeLoadException rtle)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(rtle.Message);

                for (int i = 0; i < rtle.LoaderExceptions.Length; i++)
                    sb.AppendLine(rtle.LoaderExceptions[i].ToString());

                throw new Mods.LoadException(sb.ToString());
            }
            catch (Exception e)
            {
                throw new Mods.LoadException("Could not load mod: " + Environment.NewLine + e.ToString());
            }

            mod.code = asm;
            mod.fileName = displayName + ".tapi";
            mod.modName = (string)info["internalName"];
            mod.modInfo = new ModInfo(info);

            mod.dlls = dlls;

            mod.modIndex = index;

            Mods.modJsons.Add(mod.modName, info);

            mod.files = files;

            for (int i = index + 1; i < Mods.modBases.Count; i++)
                Mods.modBases[i].modIndex++;

            // :O
            Mods.loadOrder.Insert(index, mod.modName);
            Mods.modBases .Insert(index, mod        );

            #region instantiate mod*
            foreach (Type t in ReflectionHelper.FindSubclasses(asm, typeof(ModPlayer)))
                mod.modPlayers.Add((ModPlayer)ReflectionHelper.Instantiate(t, new[] { typeof(ModBase), typeof(Player) }, new[] { mod, null }));
            foreach (Type t in ReflectionHelper.FindSubclasses(asm, typeof(ModWorld)))
                mod.modWorlds.Add((ModWorld)ReflectionHelper.Instantiate(t, new[] { typeof(ModBase) }, new[] { mod }));
            foreach (Type t in ReflectionHelper.FindSubclasses(asm, typeof(ModInterface)))
                mod.modInterfaces.Add((ModInterface)ReflectionHelper.Instantiate(t, new[] { typeof(ModBase) }, new[] { mod }));
            foreach (Type t in ReflectionHelper.FindSubclasses(asm, typeof(ModPrefix)))
                mod.modPrefixes.Add((ModPrefix)ReflectionHelper.Instantiate(t, new[] { typeof(ModBase) }, new[] { mod }));

            foreach (Type t in ReflectionHelper.FindSubclasses(asm, typeof(ModItem)))
                if (t.IsDefined(GlobalModAttrType, true))
                    mod.modItems.Add((ModItem)ReflectionHelper.Instantiate(t, new[] { typeof(ModBase), typeof(Item) }, new[] { mod, null }));
            foreach (Type t in ReflectionHelper.FindSubclasses(asm, typeof(ModNPC)))
                if (t.IsDefined(GlobalModAttrType, true))
                    mod.modNPCs.Add((ModNPC)ReflectionHelper.Instantiate(t, new[] { typeof(ModBase), typeof(NPC) }, new[] { mod, null }));
            foreach (Type t in ReflectionHelper.FindSubclasses(asm, typeof(ModProjectile)))
                if (t.IsDefined(GlobalModAttrType, true))
                    mod.modProjectiles.Add((ModProjectile)ReflectionHelper.Instantiate(t, new[] { typeof(ModBase), typeof(Projectile) }, new[] { mod, null }));

            foreach (Type t in ReflectionHelper.FindSubclasses(asm, typeof(ModTileType)))
                if (t.IsDefined(GlobalModAttrType, true))
                    mod.modTileTypes.Add((ModTileType)ReflectionHelper.Instantiate(t, new[] { typeof(ModBase) }, new[] { mod }));

            Mods.globalModPlayers.AddRange(mod.modPlayers);
            Mods.globalModWorlds.AddRange(mod.modWorlds);
            Mods.globalModInterfaces.AddRange(mod.modInterfaces);
            Mods.globalModPrefixes.AddRange(mod.modPrefixes);

            Mods.globalModItems.AddRange(mod.modItems);
            Mods.globalModNPCs.AddRange(mod.modNPCs);
            Mods.globalModProjectiles.AddRange(mod.modProjectiles);

            TileDef.codeGlobal.AddRange(mod.modTileTypes);

            Defs.FillCallPriorities(baseType);

            foreach (var m in mod.modPlayers)
                Defs.FillCallPriorities(m.GetType());
            foreach (var m in mod.modWorlds)
                Defs.FillCallPriorities(m.GetType());
            foreach (var m in mod.modInterfaces)
                Defs.FillCallPriorities(m.GetType());
            foreach (var m in mod.modPrefixes)
                Defs.FillCallPriorities(m.GetType());

            foreach (var m in mod.modItems)
                Defs.FillCallPriorities(m.GetType());
            foreach (var m in mod.modNPCs)
                Defs.FillCallPriorities(m.GetType());
            foreach (var m in mod.modProjectiles)
                Defs.FillCallPriorities(m.GetType());

            // seems to chrash?
            //TileDef.FillCodeHandlers();
            #endregion

            ModsLoadContent.Load(asm, mod);
            mod.OnLoad();

            mod.modPrefixes[0].Init(null);

            mod.SetTimesLoaded(mod.GetTimesLoaded() + 1);

            Mods.loadOrderBackup = new List<string>(Mods.loadOrder);

            // keep these commented
            //Hooks.Setup();
            //Mods.SetModOptions();
            //Mods.SaveModState(Mods.pathDirModsUnsorted);
            //Recipe.FixMaterials();

            return mod;
        }
        /// <summary>
        /// Unloads a mod.
        /// </summary>
        /// <param name="base">The mod to unload.</param>
        public static void UnloadMod(ModBase @base)
        {
            @base.OnUnload();

            ModsLoadContent.Unload(@base);

            @base.modInfo = default(ModInfo);

            @base.modInterfaces  = null;
            @base.modItems       = null;
            @base.modNPCs        = null;
            @base.modPlayers     = null;
            @base.modPrefixes    = null;
            @base.modProjectiles = null;
            @base.modWorlds      = null;

            int tempIndex = @base.modIndex;

            for (int i = tempIndex; i < Mods.modBases.Count; i++)
                Mods.modBases[i].modIndex--;

            Mods.loadOrder.RemoveAt(tempIndex);
            if (Mods.loadOrderBackup.Contains(@base.modName))
                Mods.loadOrderBackup.RemoveAt(tempIndex);

            Mods.modJsons  .Remove(@base.modName);
            Mods.modOptions.Remove(@base.modName);

            @base.modIndex = -1;
            @base.modName = @base.fileName = null;
            @base.textures.Clear();
            @base.files.Clear();
            @base.code = null;

            Mods.modBases.Remove(@base);
        }
    }

    /// <summary>
    /// Provides extension methods for mod hackery.
    /// </summary>
    public static class ModExtensions
    {
        readonly static FieldInfo timesLoadedInfo
            = ModController.ModBaseType.GetField("timesLoaded", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Gets the value of the internal field 'timesLoaded' of a <see cref="ModBase" /> instance.
        /// </summary>
        /// <param name="base">The <see cref="ModBase" /> which timesLoaded value to fetch.</param>
        /// <returns>The value of the internal field 'timesLoaded' as a 32-bit signed integer.</returns>
        public static int  GetTimesLoaded(this ModBase @base)
        {
            return (int)timesLoadedInfo.GetValue(@base);
        }
        /// <summary>
        /// Sets the value of the internal field 'timesLoaded' of a <see cref="ModBase" /> instance.
        /// </summary>
        /// <param name="base">The <see cref="ModBase" /> which timesLoaded value to get.</param>
        /// <param name="value">The new value of <paramref name="base" />.timesLoaded.</param>
        public static void SetTimesLoaded(this ModBase @base, int value)
        {
            timesLoadedInfo.SetValue(@base, value);
        }

        /// <summary>
        /// Attaches a ModEntity to a CodableEntity.
        /// </summary>
        /// <param name="entity">The CodableEntity where the ModEntity will be attached to.</param>
        /// <param name="toAttach">The ModEntity to attach to the CodableEntity.</param>
        /// <param name="mode">How to handle attached ModEntities. Default is <see cref="AttachMode" />.Append.</param>
        public static void AttachModEntity(this CodableEntity entity, ModEntity toAttach, AttachMode mode = AttachMode.Append)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (toAttach == null)
                throw new ArgumentNullException("toAttach");

            if (entity.subClass != null)
                switch (mode)
                {
                    case AttachMode.Append:
                        if (entity.subClass != null)
                        {
                            Array.Resize(ref entity.allSubClasses, entity.allSubClasses.Length + 1);
                            entity.allSubClasses[entity.allSubClasses.Length - 1] = toAttach;
                        }
                        else
                        {
                            entity.subClass = toAttach;
                            entity.subClassName = toAttach.GetType().Name;
                            entity.modBase = toAttach.modBase;
                        }
                        break;
                    case AttachMode.New:
                        throw new InvalidOperationException("There is already a ModEntity attached!");
                    case AttachMode.Overwrite:
                        entity.subClass = toAttach;
                        entity.subClassName = toAttach.GetType().Name;
                        entity.modBase = toAttach.modBase;
                        break;
                }
            else
            {
                entity.subClass = toAttach;
                entity.modBase = toAttach.modBase;
            }
        }
        /// <summary>
        /// Removes a ModEntity from a CodableEntity.
        /// </summary>
        /// <param name="entity">The CodableEntity where the ModEntity will be removed from.</param>
        /// <param name="removeIndex">The index of the entity to remove. Use -1 to remove the default field. Default is -1.</param>
        /// <returns>True if the ModEntity was removed, false otherwise. No exceptions are thrown.</returns>
        public static bool RemoveModEntity(this CodableEntity entity, int removeIndex = -1)
        {
            if (removeIndex == -1)
            {
                entity.subClass = null;
                entity.subClassName = String.Empty;
                entity.modBase = null;
            }
            else if (removeIndex >= 0 && removeIndex < entity.allSubClasses.Length)
            {
                List<ModEntity> list = new List<ModEntity>(entity.allSubClasses.Take(removeIndex));
                list.AddRange(entity.allSubClasses.Skip(removeIndex + 1));

                entity.allSubClasses = list.ToArray();
            }
            else
                return false;

            return true;
        }
    }
}

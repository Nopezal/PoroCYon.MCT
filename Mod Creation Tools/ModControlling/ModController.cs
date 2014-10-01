using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using LitJson;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.ModControlling
{
	// Also hacky stuff. Lots of it.

	/// <summary>
	/// Contains the global mod classes of a mod.
	/// </summary>
	public sealed class ModClasses
	{
#pragma warning disable 1591
		public ModInterface ModInterface = null;

		public List<ModItem      > GlobalItems = new List<ModItem      >();
		public List<ModNPC       > GlobalNPCs  = new List<ModNPC       >();
		public List<ModProjectile> GlobalProjs = new List<ModProjectile>();
		public List<ModTileType  > GlobalTiles = new List<ModTileType  >();
		public List<ModPlayer    > Players     = new List<ModPlayer    >();
		public List<ModPrefix    > Prefixes    = new List<ModPrefix    >();
		public List<ModWorld     > Worlds      = new List<ModWorld     >();
#pragma warning restore 1591
	}

	/// <summary>
	/// Provides methods to load and manipulate mods.
	/// </summary>
	public static class ModController
	{
		readonly static MethodInfo
			SoundDefLoadInfo = typeof(SoundDef).GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod);

		internal static void CheckModBaseAndInfo(Mod mod, ModBase @base)
		{
			if (mod == null || @base == null || mod.modBase == null || @base.mod == null || mod.ModInfo == null)
				throw new ArgumentException("Mod & ModBase are not correctly configured.");
		}

		internal static List<T> InstantiateAndReturnTypes<T>(Assembly asm  )
			where T : class
		{
			List<T> ret = new List<T>();

			foreach (Type t in asm.GetTypes())
				if (t.IsSubclassOf(typeof(T)))
				{
					T c = Activator.CreateInstance(t, t.IsNotPublic) as T;

					if (c == null)
						continue;

					ret.Add(c);
				}

			return ret;
		}
		internal static List<T> InstantiateAndReturnTypes<T>(ModBase  @base, bool requiresGlobalMod = false)
			where T : class
		{
			List<T> ret = new List<T>();

			foreach (Type t in @base.GetType().Assembly.GetTypes())
				if (t.IsSubclassOf(typeof(T)))
				{
					if (requiresGlobalMod && t.GetCustomAttributes(typeof(GlobalModAttribute), true).Length == 0)
						continue;

					T c = Activator.CreateInstance(t, t.IsNotPublic) as T;

					if (c == null)
						continue;

					c.GetType().GetField("modBase").SetValue(c, @base);

					ret.Add(c);
				}

			return ret;
		}

		internal static void SoundDefLoad(ModBase @base)
		{
			SoundDefLoadInfo.Invoke(null, new[] { @base });
		}

		internal static void  LoadClasses(Mod mod)
		{
			CheckModBaseAndInfo(mod, mod.modBase);

			mod.modInterface = InstantiateAndReturnTypes<ModInterface>(mod.modBase).FirstOrDefault();

			mod.modBase.modItemTemplates       = InstantiateAndReturnTypes<ModItem      >(mod.modBase, true);
			mod.modBase.modNPCTemplates        = InstantiateAndReturnTypes<ModNPC       >(mod.modBase, true);
			mod.modBase.modPlayerTemplates     = InstantiateAndReturnTypes<ModPlayer    >(mod.modBase);
			mod.modBase.modPrefixTemplates     = InstantiateAndReturnTypes<ModPrefix    >(mod.modBase);
			mod.modBase.modProjectileTemplates = InstantiateAndReturnTypes<ModProjectile>(mod.modBase, true);
			mod.modBase.modTileTypeTemplates   = InstantiateAndReturnTypes<ModTileType  >(mod.modBase, true);
			mod.modBase.modWorldTemplates      = InstantiateAndReturnTypes<ModWorld     >(mod.modBase);
		}
		internal static void ApplyClasses(Mod mod, ModClasses classes)
		{
			CheckModBaseAndInfo(mod, mod.modBase);

			mod.modInterface = classes.ModInterface;

			mod.modBase.modItemTemplates       = classes.GlobalItems;
			mod.modBase.modNPCTemplates        = classes.GlobalNPCs ;
			mod.modBase.modProjectileTemplates = classes.GlobalProjs;
			mod.modBase.modTileTypeTemplates   = classes.GlobalTiles;
			mod.modBase.modPlayerTemplates     = classes.Players    ;
			mod.modBase.modPrefixTemplates     = classes.Prefixes   ;
			mod.modBase.modWorldTemplates      = classes.Worlds     ;
		}
		internal static void Setup       (Mod mod)
		{
			SetupNoContent(mod);

			if (!Main.dedServ)
			{
					SoundDefLoad(mod.modBase);
				WavebankDef.Load(mod.modBase);
				GoreDef    .Load(mod.modBase);
			}

			ModJsonHandler.Handle(mod.modBase);
		}
		internal static void SetupNoContent(Mod mod)
		{
			CheckModBaseAndInfo(mod, mod.modBase);

			List<ModBase     > mBases  = new List<ModBase     >();
			List<ModInterface> mUIs    = new List<ModInterface>();
			List<ModWorld    > mWorlds = new List<ModWorld    >();
			List<ModPrefix   > mPfixes = new List<ModPrefix   >();

			mBases.Add(mod.modBase);

			if (mod.modInterface != null)
				mUIs.Add(mod.modInterface);

			if (mod.modBase.modWorldTemplates .Count > 0)
				mWorlds.AddRange(mod.modBase.modWorldTemplates );
			if (mod.modBase.modPrefixTemplates.Count > 0)
				mPfixes.AddRange(mod.modBase.modPrefixTemplates);

			Hooks.Base     .Setup(mBases );
			Hooks.Interface.Setup(mUIs   );
			Hooks.World    .Setup(mWorlds);
			Hooks.Prefixes .Setup(mPfixes);
		}

		internal static void LoadModInternal(Mod mod, ModBase @base)
		{
			CheckModBaseAndInfo(mod, @base);

			Mods.mods.Add(mod);

			Setup(mod);

			@base.OnLoad();

			@base.SetTimesLoaded(@base.GetTimesLoaded() + 1);
		}

		public static void LoadMod(Mod mod, ModBase @base)
		{
			mod.enabled = true;

			mod.modBase = @base;
			@base.mod = mod;

			LoadModInternal(mod, @base);
		}
        public static void LoadMod(Mod mod, ModBase @base, ModClasses classes)
		{
			mod.enabled = true;

			mod.modBase = @base;
			@base.mod = mod;

			ApplyClasses(mod, classes);

			LoadMod(mod, @base);
		}
		public static void LoadMod(string   basePath, JsonData info, Texture2D icon, ModBase @base, ModClasses classes)
		{
			Mod m = new Mod(basePath);

			m.SetModInfo(info);
			m.SetIcon   (icon);

			LoadClasses(m);

			LoadMod(m, @base, classes);
		}
		public static void LoadMod(Assembly asm     , JsonData info, Texture2D icon)
		{
			Mod m = new Mod(asm.Location);

			ModBase @base = InstantiateAndReturnTypes<ModBase>(asm).FirstOrDefault() ?? new ModBase();

            m.SetModInfo(info);
			m.SetIcon   (icon);

			LoadClasses(m);

			LoadMod(m, @base);
		}
		public static void LoadMod(Assembly asm     , JsonData info, Texture2D icon, ModClasses classes)
		{
			Mod m = new Mod(asm.Location);

			ModBase @base = InstantiateAndReturnTypes<ModBase>(asm).FirstOrDefault() ?? new ModBase();

            m.SetModInfo(info);
			m.SetIcon   (icon);

			ApplyClasses(m, classes);

			LoadMod(m, @base);
		}
		public static void LoadMod(Assembly asm     , JsonData info, Texture2D icon, ModClasses classes, ModBase @base)
		{
			Mod m = new Mod(asm.Location);

			m.SetModInfo(info);
			m.SetIcon(icon);

			ApplyClasses(m, classes);

			LoadMod(m, @base);
		}
	}

	/// <summary>
	/// Provides extension methods for mod hackery.
	/// </summary>
	public static class ModExtensions
	{
		readonly static FieldInfo
			timesLoadedInfo
				= typeof(ModBase).GetField("timesLoaded", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic),
			modInfoInfo	// kinda inconvenient
				= typeof(Mod    ).GetField("_modInfo"   , BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic),
			iconInfo
				= typeof(Mod    ).GetField("_icon"      , BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

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
		/// Sets the <see cref="Mod.ModInfo" /> property.
		/// </summary>
		/// <param name="mod">The Mod which ModInfo should be set.</param>
		/// <param name="value">The new value of the ModInfo property.</param>
		public static void SetModInfo    (this Mod mod, JsonData value)
		{
			modInfoInfo.SetValue(mod, value);
		}
		/// <summary>
		/// Sets the <see cref="Mod.Icon" /> property.
		/// </summary>
		/// <param name="mod">The Mod which Icon should be set.</param>
		/// <param name="value">The new value of the Icon property.</param>
		public static void SetIcon(this Mod mod, Texture2D value)
		{
			if (Main.dedServ)
				return;

			iconInfo.SetValue(mod, value);
		}

		/// <summary>
		/// Attaches a ModEntity to a CodableEntity.
		/// </summary>
		/// <typeparam name="T">The type of the ModEntity (Item, NPC, etc).</typeparam>
		/// <param name="e">The CodableEntity.</param>
		/// <param name="toAdd">The ModEntity to detach.</param>
		public static void AttachModEntity<T>(this CodableEntity e, ModEntity<T> toAdd)
			where T : class
		{
			if ((FieldInfo fi = e.GetType().GetField("modEntities")) != null
					&& fi.FieldType.GetGenericTypeDefinition() == typeof(List<>)
					&& fi.FieldType.GetGenericArguments()[0].IsSubclassOf(typeof(ModEntity<T>)))
				fi.FieldType.GetMethod("Add", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).Invoke(e, new[] { toAdd });
			else
				throw new ArgumentException("This CodableEntity does not have a modEntities list.", "e");
		}
		/// <summary>
		/// Detaches a ModEntity from a CodableEntity.
		/// </summary>
		/// <typeparam name="T">The type of the ModEntity (Item, NPC, etc).</typeparam>
		/// <param name="e">The CodableEntity.</param>
		/// <param name="toRemove">The ModEntity to detach.</param>
		public static void DetachModEntity<T>(this CodableEntity e, ModEntity<T> toRemove)
			where T : class
		{
			if ((FieldInfo fi = e.GetType().GetField("modEntities")) != null
					&& fi.FieldType.GetGenericTypeDefinition() == typeof(List<>)
					&& fi.FieldType.GetGenericArguments()[0].IsSubclassOf(typeof(ModEntity<T>)))
				fi.FieldType.GetMethod("Remove", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).Invoke(e, new[] { toRemove });
			else
				throw new ArgumentException("This CodableEntity does not have a modEntities list.", "e");
		}
	}
}

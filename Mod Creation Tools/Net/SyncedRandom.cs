using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Terraria;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Internal.ModClasses;

namespace PoroCYon.MCT.Net
{
    /// <summary>
    /// A synced Pseudo-Random Number Generator.
    /// Values are global.
    /// </summary>
    public sealed class SyncedRandom : Random, IDisposable
    {
        internal static WrapperDictionary<string, int> rands = new WrapperDictionary<string, int>();
        internal static WrapperDictionary<string, int> refs = new WrapperDictionary<string, int>();
        static Dictionary<int, Random> cachedRands = new Dictionary<int, Random>();

        int seed;

        /// <summary>
        /// The group name of the <see cref="PoroCYon.MCT.Net.SyncedRandom"/> instance
        /// </summary>
        public string GroupName
        {
            get;
            private set;
        }

        [TargetedPatchingOptOut(Consts.TPOOReason)]
        internal SyncedRandom(int seed)
            : base(seed)
        {

        }
        internal SyncedRandom(int seed, string group, bool sync = true)
            : base(seed)
        {
            this.seed = seed;
            GroupName = group;
            rands[group] = seed;
            refs[group]++;

            if (Main.netMode != 0 && sync)
                NetHelper.SendModData(MctMod.instance, InternalNetMessages.SyncRandom_CTOR, GroupName, seed);
        }

        /// <summary>
        /// Creates and returns a new instance of the SyncedRandom class.
        /// Environment.TickCount is used as seed value.
        /// </summary>
        /// <param name="groupName">The group name of the random instance.</param>
        /// <returns>A new instance of the SyncedRandom class.</returns>
        [TargetedPatchingOptOut(Consts.TPOOReason)]
        public static SyncedRandom CreateNew(string groupName)
        {
            return CreateNew(Environment.TickCount, groupName);
        }
        /// <summary>
        /// Creates and returns a new instance of the SyncedRandom class.
        /// </summary>
        /// <param name="seed">The seed value of the random instance.</param>
        /// <param name="groupName">The group name of the random instance.</param>
        /// <returns>A new instance of the SyncedRandom class.</returns>
        public static SyncedRandom CreateNew(int seed, string groupName)
        {
            return rands.ContainsKey(groupName) ? new SyncedRandom(rands[groupName]) { GroupName = groupName } : new SyncedRandom(seed, groupName);
        }

        /// <summary>
        /// Disposes the SyncedRandom instance by sending a NetMessage that decreases the group reference counter on the server/other clients,
        /// decreases the group reference counter, and checks wether the group's random instance can be removed or not.
        /// </summary>
        ~SyncedRandom()
        {
            DecreaseRef();
        }
        /// <summary>
        /// Disposes the SyncedRandom instance by sending a NetMessage that decreases the group reference counter on the server/other clients,
        /// decreases the group reference counter, and checks wether the group's random instance can be removed or not.
        /// </summary>
        public void Dispose()
        {
            DecreaseRef();
            GC.SuppressFinalize(this);
        }

        [TargetedPatchingOptOut(Consts.TPOOReason)]
        void DecreaseRef()
        {
            if (Main.netMode != 0)
                NetHelper.SendModData(MctMod.instance, InternalNetMessages.SyncRandom_DTOR, seed);

            refs[GroupName]--;

            if (refs[GroupName] <= 0)
            {
                rands.Remove(GroupName);
                refs .Remove(GroupName);
            }
        }

        /// <summary>
        /// Generates a nonnegative integral value
        /// </summary>
        /// <returns>A nonnegative integral value</returns>
        [TargetedPatchingOptOut(Consts.TPOOReason)]
        public override int Next()
        {
            return Next(0, Int32.MaxValue);
        }
        /// <summary>
        /// Generates a nonnegative integral value that is smaller than the specified maximum value 
        /// </summary>
        /// <param name="maxValue">The specified maximum value</param>
        /// <returns>An integral value that is equal to or greater than 0 and smaller than the specified maximum value</returns>
        [TargetedPatchingOptOut(Consts.TPOOReason)]
        public override int Next(int maxValue)
        {
            return Next(0, maxValue);
        }
        /// <summary>
        /// Generates an integral value that is equal to or bigger than the specified minimum value and smaller than the specified maximum value 
        /// </summary>
        /// <param name="minValue">The specified minimum value</param>
        /// <param name="maxValue">The specified maximum value</param>
        /// <returns>An integral value that is equal to or greater than the specified minimum value and smaller than the specified maximum value</returns>
        public override int Next(int minValue, int maxValue)
        {
            int ret = GetCached(seed).Next(minValue, maxValue);

            Sync();

            return ret;
        }
        /// <summary>
        /// Returns a decimal value equal to or bigger than 0 and smaller than 1
        /// </summary>
        /// <returns>A decimal value</returns>
        public override double NextDouble()
        {
            double ret = GetCached(seed).NextDouble();

            Sync();

            return ret;
        }
        /// <summary>
        /// Fills an array of bytes with random values
        /// </summary>
        /// <param name="array">The array to fill</param>
        public override void NextBytes(byte[] array)
        {
            GetCached(seed).NextBytes(array);

            Sync();
        }

        /// <summary>
        /// Creates a sample value equal to or greater than 0 and smaller than 1
        /// </summary>
        /// <returns>A sample value</returns>
        protected override double Sample()
        {
            double ret = GetCached(seed).NextDouble();

            Sync();

            return ret;
        }

        [TargetedPatchingOptOut(Consts.TPOOReason)]
        void Sync()
        {
            if (Main.netMode != 0)
                NetHelper.SendModData(MctMod.instance, InternalNetMessages.SyncRandom_Sync, seed);
        }

        internal static void Reset()
        {
            rands.Clear();
            refs.Clear();
            cachedRands.Clear();
        }
        [TargetedPatchingOptOut(Consts.TPOOReason)]
        internal static Random GetCached (int seed)
        {
            if (cachedRands.ContainsKey(seed))
                return cachedRands[seed];
            else
            {
                Random r = new Random(seed);
                cachedRands.Add(seed, r);
                return r;
            }
        }
        internal static void RemoveCached(int seed)
        {
            cachedRands.Remove(seed);
        }
    }
}

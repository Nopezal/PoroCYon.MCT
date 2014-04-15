using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TAPI;
using PoroCYon.MCT.Internal;

namespace PoroCYon.MCT.Net
{
    /// <summary>
    /// A synced Pseudo-Random Number Generator.
    /// Values are global.
    /// </summary>
    public sealed class SyncedRandom : Random, IDisposable
    {
        internal static WrapperDictionary<string, Random> rands = new WrapperDictionary<string, Random>();
        internal static WrapperDictionary<string, int> refs = new WrapperDictionary<string, int>();

        /// <summary>
        /// The group name of the <see cref="PoroCYon.MCT.Net.SyncedRandom"/> instance
        /// </summary>
        public string GroupName
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PoroCYon.MCT.Net.SyncedRandom"/> class.
        /// </summary>
        /// <param name="groupName">The group name of the <see cref="PoroCYon.MCT.Net.SyncedRandom"/> instance (instances of the same group name return the same results)</param>
        public SyncedRandom(string groupName)
            : this((int)DateTime.Now.Ticks, groupName)
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="PoroCYon.MCT.Net.SyncedRandom"/> class.
        /// </summary>
        /// <param name="seed">The seed of the PRNG</param>
        /// <param name="groupName">The group name of the <see cref="PoroCYon.MCT.Net.SyncedRandom"/> instance (instances of the same group name return the same results)</param>
        public SyncedRandom(int seed, string groupName)
            : base(seed)
        {
            GroupName = groupName;

            rands[GroupName] = new Random(seed);

            refs[GroupName]++;

            if (Main.netMode != 0)
                NetHelper.SendModData("PoroCYon.MCT", InternalNetMessages.SyncRandom_CTOR, GroupName, seed);
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
        }

        void DecreaseRef()
        {
            if (Main.netMode != 0)
                NetHelper.SendModData("PoroCYon.MCT", InternalNetMessages.SyncRandom_DTOR, GroupName);

            refs[GroupName]--;

            if (refs[GroupName] <= 0)
                rands.Remove(GroupName);
        }

        /// <summary>
        /// Generates a nonnegative integral value
        /// </summary>
        /// <returns>A nonnegative integral value</returns>
        public override int Next()
        {
            int ret = rands[GroupName].Next();

            Sync();

            return ret;
        }
        /// <summary>
        /// Generates a nonnegative integral value that is smaller than the specified maximum value 
        /// </summary>
        /// <param name="maxValue">The specified maximum value</param>
        /// <returns>An integral value that is equal to or greater than 0 and smaller than the specified maximum value</returns>
        public override int Next(int maxValue)
        {
            int ret = rands[GroupName].Next(maxValue);

            Sync();

            return ret;
        }
        /// <summary>
        /// Generates an integral value that is equal to or bigger than the specified minimum value and smaller than the specified maximum value 
        /// </summary>
        /// <param name="minValue">The specified minimum value</param>
        /// <param name="maxValue">The specified maximum value</param>
        /// <returns>An integral value that is equal to or greater than the specified minimum value and smaller than the specified maximum value</returns>
        public override int Next(int minValue, int maxValue)
        {
            int ret = rands[GroupName].Next(minValue, maxValue);

            Sync();

            return ret;
        }
        /// <summary>
        /// Returns a decimal value equal to or bigger than 0 and smaller than 1
        /// </summary>
        /// <returns>A decimal value</returns>
        public override double NextDouble()
        {
            double ret = rands[GroupName].NextDouble();

            Sync();

            return ret;
        }
        /// <summary>
        /// Fills an array of bytes with random values
        /// </summary>
        /// <param name="array">The array to fill</param>
        public override void NextBytes(byte[] array)
        {
            rands[GroupName].NextBytes(array);

            Sync();
        }

        /// <summary>
        /// Creates a sample value equal to or bigger than 0 and smaller than 1
        /// </summary>
        /// <returns>A sample value</returns>
        protected override double Sample()
        {
            double ret = rands[GroupName].NextDouble();

            Sync();

            return ret >= 1d || ret < 0d || Double.IsNaN(ret) || Double.IsNegativeInfinity(ret) || Double.IsPositiveInfinity(ret)
                ? base.Sample() : ret;
        }

        void Sync()
        {
            if (Main.netMode == 0)
                return;

            NetHelper.SendModData("PoroCYon.MCT", InternalNetMessages.SyncRandom_Sync, GroupName, rands[GroupName], refs[GroupName]);
        }

        internal static void Reset()
        {
            SyncedRandom.rands.Clear();
            SyncedRandom.refs.Clear();
        }
    }
}

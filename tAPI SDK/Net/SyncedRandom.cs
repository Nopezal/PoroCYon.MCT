using System;
using System.Collections.Generic;
using System.Linq;
using TAPI.SDK.Internal;

namespace TAPI.SDK.Net
{
    /// <summary>
    /// A synced Pseudo-Random Number Generator.
    /// Values are global.
    /// </summary>
    public sealed class SyncedRandom : Random
    {
        internal static WrapperDictionary<string, Random> rands = new WrapperDictionary<string, Random>();
        internal static WrapperDictionary<string, int> refs = new WrapperDictionary<string, int>();

        /// <summary>
        /// The group name of the <see cref="TAPI.SDK.Net.SyncedRandom"/> instance
        /// </summary>
        public string GroupName
        {
            get;
            private set;
        }

        public SyncedRandom(string groupName)
            : this((int)DateTime.Now.Ticks, groupName)
        {

        }
        public SyncedRandom(int seed, string groupName)
            : base(seed)
        {
            GroupName = groupName;

            rands[GroupName] = new Random(seed);

            refs[GroupName]++;

            if (Main.netMode != 0)
                NetMessageHelper.SendModData("TAPI.SDK", InternalNetMessages.SyncRandom_CTOR, GroupName, seed);
        }

        ~SyncedRandom()
        {
            if (Main.netMode != 0)
                NetMessageHelper.SendModData("TAPI.SDK", InternalNetMessages.SyncRandom_DTOR, GroupName);

            refs[GroupName]--;

            if (refs[GroupName] <= 0)
                rands.Remove(GroupName);
        }

        public override int Next()
        {
            int ret = rands[GroupName].Next();

            Sync();

            return ret;
        }
        public override int Next(int minValue)
        {
            int ret = rands[GroupName].Next(minValue);

            Sync();

            return ret;
        }
        public override int Next(int minValue, int maxValue)
        {
            int ret = rands[GroupName].Next(minValue, maxValue);

            Sync();

            return ret;
        }
        public override double NextDouble()
        {
            double ret = rands[GroupName].NextDouble();

            Sync();

            return ret;
        }
        public override void NextBytes(byte[] array)
        {
            rands[GroupName].NextBytes(array);

            Sync();
        }

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

            NetMessageHelper.SendModData("TAPI.SDK", InternalNetMessages.SyncRandom_Sync, GroupName, rands[GroupName], refs[GroupName]);
        }
    }
}

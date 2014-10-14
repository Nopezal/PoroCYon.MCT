using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.MCT.Net;
using TAPI;

namespace PoroCYon.MCT.Internal.ModClasses
{
    sealed class MNet : ModNet
    {
        [CallPriority(Single.PositiveInfinity)]
        public override void NetReceive(BinBuffer bb, int msg)
        {
            base.NetReceive(bb, msg);

            switch ((InternalNetMessages)msg)
            {
                case InternalNetMessages.SyncRandom_Sync:
                    SyncedRandom.GetCached(bb.ReadInt()).NextDouble();
                    break;
                case InternalNetMessages.SyncRandom_CTOR:
                    {
                        string group = bb.ReadString();
                        int seed = bb.ReadInt();

                        SyncedRandom.rands[group] = seed;
                        SyncedRandom.refs[group]++;
                    }
                    break;
                case InternalNetMessages.SyncRandom_DTOR:
                    {
                        int seed = bb.ReadInt();
                        string group = null;

                        foreach (var kvp in SyncedRandom.rands)
                            if (kvp.Value == seed)
                                group = kvp.Key;

                        if (group == null)
                            return;

                        SyncedRandom.refs[group]--;
                        if (SyncedRandom.refs[group] <= 0)
                        {
                            SyncedRandom.rands.Remove(group);
                            SyncedRandom.refs.Remove(group);
                            SyncedRandom.RemoveCached(seed);
                        }
                    }
                    break;
            }
        }
    }
}

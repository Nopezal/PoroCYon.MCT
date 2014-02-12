using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.Internal
{
    static class IEConsts // internal enum constants
    {
        internal const int ENUM_OFFSET = 128;
    }

    enum InternalNetMessages : int
    {
        SyncRandom_Sync = IEConsts.ENUM_OFFSET + 0,
        SyncRandom_CTOR = IEConsts.ENUM_OFFSET + 1,
        SyncRandom_DTOR = IEConsts.ENUM_OFFSET + 2
    }
    enum InternalModMessages : int
    {

    }
}

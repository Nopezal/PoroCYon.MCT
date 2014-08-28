using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PoroCYon.MCT
{
    // oh noes!
    static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
    }
}

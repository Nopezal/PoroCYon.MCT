using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TAPI.SDK.Debugger
{
    using Debugger_ = System.Diagnostics.Debugger;

    static class Program
    {
        [STAThread]
        static void Main()
        {
            if (!Debugger_.IsAttached)
            {
                MessageBox.Show("You need to run the debugger via Visual Studio!", "tAPI SDK Debugger", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // omits steam check etc.
            new Main().Run();
        }
    }
}

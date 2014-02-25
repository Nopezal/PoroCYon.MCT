using System;
using System.Windows;
using System.Windows.Controls;

namespace TAPI.SDK.Installer
{
    /// <summary>
    /// Enumeration is marked as Flags
    /// </summary>
    [Flags]
    public enum VSVersion : uint
    {
        VCSExpress = 1,
        VisualStudio10 = 2,

        WDExpress11 = 4,
        VisualStudio11 = 8,

        WDExpress12 = 16,
        VisualStudio12 = 32
    }

    public partial class VsVersions : UserControl
    {
        internal static VSVersion
            vsVersions = 0,
            canInstallVS = 0;

        public VsVersions()
        {
            InitializeComponent();

            vsVersions = 0;

            VSVersion latest = 0, latestExpress = 0;
            for (VSVersion ver = VSVersion.VCSExpress; ver <= VSVersion.VisualStudio12; ver = (VSVersion)((uint)ver * 2u))
            {
                FromVSVersion(ver).IsEnabled = false;

                if ((canInstallVS & ver) == ver)
                {
                    vsVersions |= ver;
                    FromVSVersion(ver).IsEnabled = true;
                    latest = ver;
                    if (ver == VSVersion.VCSExpress || ver == VSVersion.WDExpress11 || ver == VSVersion.WDExpress12)
                        latestExpress = ver;
                }
            }

            if (latest != 0)
                FromVSVersion(latest).IsChecked = true;
            if (latestExpress != 0)
                FromVSVersion(latestExpress).IsChecked = true;
        }

        CheckBox FromVSVersion(VSVersion version)
        {
            switch (version)
            {
                case VSVersion.VCSExpress:
                    return VCs10;
                case VSVersion.VisualStudio10:
                    return Vs10;
                case VSVersion.WDExpress11:
                    return WdE11;
                case VSVersion.VisualStudio11:
                    return Vs11;
                case VSVersion.WDExpress12:
                    return WdE12;
                case VSVersion.VisualStudio12:
                    return Vs12;
            }

            throw new ArgumentOutOfRangeException("version");
        }
    }
}

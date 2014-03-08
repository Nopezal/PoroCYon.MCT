using System;
using System.Windows;
using System.Windows.Controls;

namespace PoroCYon.MCT.Installer
{
    /// <summary>
    /// Enumeration is marked as Flags
    /// </summary>
    [Flags]
    public enum VSVersion : byte
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
            ChosenVersions = 0,
            PossibleVersions = 0;

        public VsVersions()
        {
            InitializeComponent();

            ChosenVersions = 0;

            VSVersion latest = 0, latestExpress = 0;
            for (VSVersion ver = VSVersion.VCSExpress; ver <= VSVersion.VisualStudio12; ver = (VSVersion)((byte)ver * 2))
                if (FromVSVersion(ver).IsEnabled = ((PossibleVersions & ver) != 0))
                {
                    latest = ver;
                    if (ver == VSVersion.VCSExpress || ver == VSVersion.WDExpress11 || ver == VSVersion.WDExpress12)
                        latestExpress = ver;
                }

            VCs10.Checked += (s, e) => ChosenVersions |= VSVersion.VCSExpress;
            VCs10.Unchecked += (s, e) => ChosenVersions ^= VSVersion.VCSExpress;

            Vs10.Checked += (s, e) => ChosenVersions |= VSVersion.VisualStudio10;
            Vs10.Unchecked += (s, e) => ChosenVersions ^= VSVersion.VisualStudio10;

            WdE11.Checked += (s, e) => ChosenVersions |= VSVersion.WDExpress11;
            WdE11.Unchecked += (s, e) => ChosenVersions ^= VSVersion.WDExpress11;

            Vs11.Checked += (s, e) => ChosenVersions |= VSVersion.VisualStudio11;
            Vs11.Unchecked += (s, e) => ChosenVersions ^= VSVersion.VisualStudio11;

            WdE12.Checked += (s, e) => ChosenVersions |= VSVersion.WDExpress12;
            WdE12.Unchecked += (s, e) => ChosenVersions ^= VSVersion.WDExpress12;

            Vs12.Checked += (s, e) => ChosenVersions |= VSVersion.VisualStudio12;
            Vs12.Unchecked += (s, e) => ChosenVersions ^= VSVersion.VisualStudio12;

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

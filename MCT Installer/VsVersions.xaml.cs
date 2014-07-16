using System;
using System.Windows;
using System.Windows.Controls;

namespace PoroCYon.MCT.Installer
{
    /// <summary>
    /// Enumeration is marked as Flags
    /// </summary>
    [Flags]
    public enum VsVersion : byte
    {
        VCSExpress = 1,
        VisualStudio10 = 2,

        WDExpress11 = 4,
        VisualStudio11 = 8,

        WDExpress12 = 16,
        VisualStudio12 = 32,

        VisualStudio14CTP = 64
    }

    public partial class VsVersions : UserControl
    {
        internal static VsVersion
            ChosenVersions = 0,
            PossibleVersions = 0;

        public VsVersions()
        {
            InitializeComponent();

            ChosenVersions = 0;

            VsVersion latest = 0, latestExpress = 0;
            for (VsVersion ver = VsVersion.VCSExpress; ver <= VsVersion.VisualStudio14CTP; ver = (VsVersion)((byte)ver * 2))
                if (FromVsVersion(ver).IsEnabled = ((PossibleVersions & ver) != 0))
                {
                    latest = ver;
                    if (ver == VsVersion.VCSExpress || ver == VsVersion.WDExpress11 || ver == VsVersion.WDExpress12)
                        latestExpress = ver;
                }

            VCs10.Checked += (s, e) => ChosenVersions |= VsVersion.VCSExpress;
            VCs10.Unchecked += (s, e) => ChosenVersions ^= VsVersion.VCSExpress;

            Vs10.Checked += (s, e) => ChosenVersions |= VsVersion.VisualStudio10;
            Vs10.Unchecked += (s, e) => ChosenVersions ^= VsVersion.VisualStudio10;

            WdE11.Checked += (s, e) => ChosenVersions |= VsVersion.WDExpress11;
            WdE11.Unchecked += (s, e) => ChosenVersions ^= VsVersion.WDExpress11;

            Vs11.Checked += (s, e) => ChosenVersions |= VsVersion.VisualStudio11;
            Vs11.Unchecked += (s, e) => ChosenVersions ^= VsVersion.VisualStudio11;

            WdE12.Checked += (s, e) => ChosenVersions |= VsVersion.WDExpress12;
            WdE12.Unchecked += (s, e) => ChosenVersions ^= VsVersion.WDExpress12;

            Vs12.Checked += (s, e) => ChosenVersions |= VsVersion.VisualStudio12;
            Vs12.Unchecked += (s, e) => ChosenVersions ^= VsVersion.VisualStudio12;

            Vs14CTP.Checked += (s, e) => ChosenVersions |= VsVersion.VisualStudio14CTP;
            Vs14CTP.Unchecked += (s, e) => ChosenVersions ^= VsVersion.VisualStudio14CTP;

            if (latest != 0)
                FromVsVersion(latest).IsChecked = true;
            if (latestExpress != 0)
                FromVsVersion(latestExpress).IsChecked = true;
        }

        CheckBox FromVsVersion(VsVersion version)
        {
            switch (version)
            {
                case VsVersion.VCSExpress:
                    return VCs10;
                case VsVersion.VisualStudio10:
                    return Vs10;
                case VsVersion.WDExpress11:
                    return WdE11;
                case VsVersion.VisualStudio11:
                    return Vs11;
                case VsVersion.WDExpress12:
                    return WdE12;
                case VsVersion.VisualStudio12:
                    return Vs12;
                case VsVersion.VisualStudio14CTP:
                    return Vs14CTP;
            }

            throw new ArgumentOutOfRangeException("version");
        }
    }
}

using System;
using System.Windows;

namespace TAPI.SDK.Installer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Resources.MergedDictionaries.Add(LoadComponent(new Uri("PresentationFramework.AeroLite;V4.0.0.0;31bf3856ad364e35;component\\themes/aerolite.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);

            base.OnStartup(e);
        }
    }
}

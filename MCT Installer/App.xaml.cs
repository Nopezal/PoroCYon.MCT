﻿using System;
using System.Reflection;
using System.Windows;

namespace PoroCYon.MCT.Installer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Resources.MergedDictionaries.Add(LoadComponent(new Uri(
                    "PresentationFramework.AeroLite;V4.0.0.0;31bf3856ad364e35;component\\themes/aerolite.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
            }
            catch { } // assembly not found

            base.OnStartup(e);
        }
    }
}

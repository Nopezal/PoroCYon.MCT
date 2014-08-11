using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Xml;

namespace PoroCYon.MCT.Internal.Versioning
{
    static class UpdateChecker
    {
        internal static bool
            CheckForUpdates = true,
            LastUpdateAvailable = false;

        static WebClient client = new WebClient();

        internal static bool GetIsUpdateAvailable()
        {
            if (!CheckForUpdates)
                return LastUpdateAvailable = false;

            // this might be useful
            if (!NetworkInterface.GetIsNetworkAvailable())
                return LastUpdateAvailable = false;

            try
            {
                XmlNode downloaded = GetXml().ChildNodes[1].ChildNodes[0];

                uint version_num = UInt32.Parse(downloaded.Attributes["Number"].Value);
                string version_str = downloaded.Attributes["String"].Value;
                Version version = new Version(downloaded.Attributes["Version"].Value);

                return LastUpdateAvailable = (version_num > MctConstants.VERSION_NUM || version_str != MctConstants.VERSION_STRING || version > MctConstants.VERSION);
            }
            catch
            {
                return LastUpdateAvailable = false;
            }
        }
        internal static XmlDocument GetXml()
        {
            XmlDocument xd = new XmlDocument();

            xd.LoadXml(client.DownloadString("https://dl.dropboxusercontent.com/u/151130168/MCT/Version.xml"));

            return xd;
        }
    }
}

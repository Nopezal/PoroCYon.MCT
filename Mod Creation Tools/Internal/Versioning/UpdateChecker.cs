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
            CheckForUpdates     = true ,
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

                Version version = new Version(downloaded.Attributes["Version"].Value);

                return LastUpdateAvailable = version > MctConstants.VERSION;
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

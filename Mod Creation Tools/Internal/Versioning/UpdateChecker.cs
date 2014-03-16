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
        internal static bool IsUpdateAvailable()
        {
            // this might be useful
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            try
            {
                XmlDocument xd = new XmlDocument();

                xd.LoadXml(new WebClient().DownloadString("https://dl.dropboxusercontent.com/u/151130168/MCT/Version.xml"));

                XmlNode downloaded = xd.ChildNodes[1].ChildNodes[0];

                uint version_num = UInt32.Parse(downloaded.Attributes["Number"].Value);
                string version_str = downloaded.Attributes["String"].Value;
                Version version = new Version(downloaded.Attributes["Version"].Value);

                return version_num > MctConstants.VERSION_NUM || version_str != MctConstants.VERSION_STRING || version > MctConstants.VERSION;
            }
            catch
            {
                return false;
            }
        }
    }
}

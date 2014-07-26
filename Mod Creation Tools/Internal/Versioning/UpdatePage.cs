using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using PoroCYon.Extensions;
using TAPI;
using PoroCYon.MCT.UI.MenuItems;
using Terraria;

namespace PoroCYon.MCT.Internal.Versioning
{
    class UpdatePage : Page
    {
        internal UpdatePage()
            : base()
        {
            MenuAnchor
                aLeft = new MenuAnchor()
                {
                    anchor = new Vector2(0f, 0f),
                    offset = new Vector2(150f),
                    offset_button = new Vector2(0f, 50f)
                },
                aRight = new MenuAnchor()
                {
                    anchor = new Vector2(0.5f, 0f),
                    offset = new Vector2(-200f, 40f),
                    offset_button = new Vector2(0f, 50f)
                };

            anchors.AddRange(new List<MenuAnchor>() { aLeft, aRight });

            // ---

            buttons.Add(new MenuButton(0, "UPDATE", "", "This will close tAPI", () =>
            {
                Process.Start("https://dl.dropboxusercontent.com/u/151130168/MCT/MCT%20Installer.exe");
                API.main.Exit();
            }).SetScale(3f).Where(mb => mb.SetAutomaticPosition(aLeft, 0)));

            buttons.Add(new MenuButton(0, "Don't update", "Main Menu").Where(mb => mb.SetAutomaticPosition(aLeft, 1)));

            // ---

            string text = "";

            XmlNode
                version = UpdateChecker.GetXml().ChildNodes[1],
                current = version.ChildNodes[0];

            text += "Current: " + MctConstants.VERSION_STRING + "\n";
            text += "New: " + current.Attributes["String"].Value + "\n";

            foreach (XmlNode xn in version.ChildNodes[1])
            {
                if (xn.NodeType != XmlNodeType.Element || new Version(xn.Attributes["Version"].Value) < MctConstants.VERSION)
                    continue;

                text += "v" + xn.Attributes["Version"].Value + "\n";

                foreach (XmlNode Xn in xn.ChildNodes)
                    text += "  -" + Xn.InnerText + "\n";
            }

            buttons.Add(new MenuButton(0, text, "", "Changelog", () => { }).Where(mb =>
            {
                mb.SetAutomaticPosition(aRight, 0);
                mb.canMouseOver = false;
            }).SetSize(Main.fontMouseText.MeasureString(text)));
        }
    }
}

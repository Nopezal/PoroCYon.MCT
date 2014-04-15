﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.Internal.Versioning
{
    using Size = System.Drawing.Size;
    using Color = System.Drawing.Color;
    using Point = System.Drawing.Point;

    static class UpdateBoxInjector
    {
        static ElementHost host;
        static Form mainForm;

        internal static void Inject()
        {
            mainForm = Form.FromHandle(API.main.Window.Handle) as Form; // the game window as a Form

            mainForm.Controls.Add(host = new ElementHost()
            {
                BackColor = Color.SkyBlue,
                BackColorTransparent = true,

                Location = new Point(Main.screenWidth / 2 - 250, Main.screenHeight / 2 - 150),

                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,

                Size = new Size(500, 300),

                Child = new UpdateBox()
            });
        }

        internal static void RemoveInjection()
        {
            mainForm.Controls.Remove(host);
        }
    }
}

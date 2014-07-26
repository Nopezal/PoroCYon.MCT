using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.Extensions;
using TAPI;

namespace PoroCYon.MCT.UI.Interface
{
    /// <summary>
    /// Encapsulates a CustomUI in an InterfaceLayer
    /// </summary>
    public class LayerUI : InterfaceLayer
    {
        /// <summary>
        /// The CustomUI the LayerUI contains
        /// </summary>
        public CustomUI CustomUI
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of the LayerUI class
        /// </summary>
        /// <param name="ui">The CustomUI the LayerUI contains</param>
        /// <param name="name">
        /// The name of the LayerUI.
        /// Default is the calling assembly's display name, two semicolons and the full name of the type of <paramref name="ui" />
        /// </param>
        public LayerUI(CustomUI ui, string name = null)
            : base(name ?? Assembly.GetCallingAssembly().FullName + "::" + ui.GetType())
        {
            CustomUI = ui;
        }

        /// <summary>
        /// Draws the InterfaceLayer
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the InterfaceLayer</param>
        protected override void OnDraw(SpriteBatch sb)
        {
            CustomUI.Draw(sb);
        }

        internal void Update()
        {
            if (CustomUI.IsVisible)
                CustomUI.Update();
        }
    }
}

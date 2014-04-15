using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Geometry;
using Terraria;
using TAPI;
using PoroCYon.MCT.ObjectModel;
using PoroCYon.MCT.UI.Interface.Controls.Primitives;

namespace PoroCYon.MCT.UI.Interface
{
    /// <summary>
    /// A button with text
    /// </summary>
    public class TextButton : Button, ITextObject
    {
        /// <summary>
        /// The size of the TextButton
        /// </summary>
        public Vector2? Size = null;

        /// <summary>
        /// The font of the TextButton
        /// </summary>
        public SpriteFont Font
        {
            get;
            set;
        }

        /// <summary>
        /// The text drawn by the TextButton
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// The hitbox of the TextButton
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X - 8, (int)Position.Y - 8,
                    (int)(Scale.X * (Size.HasValue ? Size.Value.X : Font.MeasureString(Text).X)) + 16,
                    (int)(Scale.Y * (Size.HasValue ? Size.Value.Y : Font.MeasureString(Text).Y)) + 16);
            }
        }

        /// <summary>
        /// Creates a new instance of the TextButton class
        /// </summary>
        public TextButton()
            : this("Button")
        {

        }
        /// <summary>
        /// Creates a new instance of the TextButton class
        /// </summary>
        /// <param name="text">The text of the TextButton</param>
        public TextButton(string text)
            : base()
        {
            Text = text;
            Font = Main.fontMouseText;
        }

        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (HasBackground)
                DrawBackground(sb);

            DrawOutlinedString(sb, Font, Text, Position + (Size.HasValue ? Size.Value / 2f - Font.MeasureString(Text) / 2f : Vector2.Zero), Colour);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI;
using Terraria;
using PoroCYon.MCT.ObjectModel;

namespace PoroCYon.MCT.UI.Interface.Controls
{
    /// <summary>
    /// A block of text
    /// </summary>
    public class TextBlock : Control, ITextObject
    {
        /// <summary>
        /// The size of the TextBlock
        /// </summary>
        public Vector2? Size = null;

        /// <summary>
        /// The font of the text
        /// </summary>
        public SpriteFont Font
        {
            get;
            set;
        }

        /// <summary>
        /// The displayed text
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// The hitbox of the control
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
        /// Creates a new instance of the TextBlock class
        /// </summary>
        public TextBlock()
            : base()
        {
            Text = "";
            Font = Main.fontMouseText;
        }
        /// <summary>
        /// Creates a new instance of the TextBlock class
        /// </summary>
        /// <param name="text">The text of the TextBlock</param>
        public TextBlock(string text)
            : this()
        {
            Text = text;
        }

        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (HasBackground)
                DrawBackground(sb);

            DrawOutlinedString(sb, Font, Text, Position + (Size.HasValue ? Size.Value / 2f - Font.MeasureString(Text) / 2f : Vector2.Zero), Colour);
        }
    }
}

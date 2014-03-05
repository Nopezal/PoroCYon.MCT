using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.ObjectModel;
using TAPI.SDK.UI.Interface.Controls.Primitives;

namespace TAPI.SDK.UI.Interface.Controls
{
    /// <summary>
    /// A block of text
    /// </summary>
    public class TextBlock : Control, ITextObject
    {
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
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(Scale.X * Font.MeasureString(Text).X), (int)(Scale.Y * Font.MeasureString(Text).Y));
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

            DrawBackground(sb);

            DrawOutlinedString(sb, Font, Text, Colour);
        }
    }
}

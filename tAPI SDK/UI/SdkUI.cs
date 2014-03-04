using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAPI.SDK.UI
{
    /// <summary>
    /// The global UI class, provides various things regarding drawing.
    /// </summary>
    public static class SdkUI
    {
        /// <summary>
        /// Gets the <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> used by the game
        /// </summary>
        public static SpriteBatch SharedSpriteBatch
        {
            get
            {
                return Constants.mainInstance.spriteBatch;
            }
        }
        /// <summary>
        /// A 1-by-1, white pixel (#FFFFFF00)
        /// </summary>
        public static Texture2D WhitePixel
        {
            get;
            internal set;
        }

        /// <summary>
        /// Draws a string with an outline
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the string</param>
        /// <param name="font">The font of the string to draw</param>
        /// <param name="text">The text to draw</param>
        /// <param name="position">The position of the string to draw</param>
        /// <param name="foreground">The foreground colour of the string to draw</param>
        /// <param name="background">The background colour of the string to draw; default is #000000</param>
        /// <param name="offset">The offset of the outlines; default is 1</param>
        /// <param name="scale">The scale of the string to draw; default is (1, 1)</param>
        /// <param name="rotation">The rotation of the string to draw; default is 0</param>
        /// <param name="origin">The origin of the rotation of the string to draw; default is (0, 0)</param>
        /// <param name="spriteEffects">The sprite effect of the string to draw; default is None</param>
        /// <param name="layerDepth">The layer depth of the string to draw; default is 0</param>
        public static void DrawOutlinedString(SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color foreground, Color? background = null, float offset = 2f,
            Vector2? scale = null, float rotation = 0f, Vector2 origin = default(Vector2), SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0f)
        {
            foreach (Vector2 v in new Vector2[] { new Vector2(offset, 0f), new Vector2(0f, offset), new Vector2(-offset, 0f), new Vector2(0f, -offset) })
                sb.DrawString(font, text, position + v, background ?? new Color(0, 0, 0, 0), rotation, origin, scale ?? new Vector2(1f), spriteEffects, layerDepth);

            sb.DrawString(font, text, position, foreground, rotation, origin, scale ?? new Vector2(1f), spriteEffects, layerDepth);
        }

        /// <summary>
        /// Draws text at the mouse
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="rare">The colour (Item rarity) of the text to draw</param>
        /// <param name="diff">No idea</param>
        public static void MouseText(string text, int rare = 0, byte diff = 0)
        {
            Constants.mainInstance.MouseText(text, rare, diff);
        }
        /// <summary>
        /// Draws Item information at the mouse
        /// </summary>
        /// <param name="i">The Item to get the info from</param>
        public static void MouseText(Item i)
        {
            Main.mouseItem = (Item)i.Clone();

            MouseText(i.AffixName() + (i.stack > 1 ? " (" + i.stack + ")" : ""));

            Main.mouseItem = new Item();
        }
    }
}

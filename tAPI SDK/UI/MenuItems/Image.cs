using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Graphics;
using TAPI.SDK.ObjectModel;

namespace TAPI.SDK.UI.MenuItems
{
    /// <summary>
    /// An image. Uses MenuButton.size as scale
    /// </summary>
    public class Image : Control, IImageObject
    {
        AnimatedGif gif = null;
        Texture2D tex = null;

        /// <summary>
        /// Wether the Image can be used as a button or not.
        /// </summary>
        public bool IsButton = true;

        /// <summary>
        /// Wether the image is a gif or not
        /// </summary>
        public bool IsGif
        {
            get;
            private set;
        }

        /// <summary>
        /// The picture of the IImageObject
        /// </summary>
        public object Picture
        {
            get
            {
                return IsGif ? gif : tex as object;
            }
            set
            {
                if (value is Texture2D)
                {
                    IsGif = false;
                    tex = value as Texture2D;
                    gif = null;
                }
                else if (value is AnimatedGif)
                {
                    IsGif = true;
                    tex = null;
                    gif = value as AnimatedGif;
                }
                else
                    throw new ArgumentException("value should be a Texture2D or an AnimatedGif", "value");

                size = picAsTex.Size() + new Vector2(16f);
            }
        }

        Texture2D picAsTex
        {
            get
            {
                return IsGif ? gif.Frames[gif.Current] : tex;
            }
        }

        /// <summary>
        /// Creates a new instance of the Image class
        /// </summary>
        /// <param name="image">The picture of the Image, as a Texture2D</param>
        public Image(Texture2D image)
            : base()
        {
            Picture = image;
        }
        /// <summary>
        /// Creates a new instance of the Image class
        /// </summary>
        /// <param name="image">The picture of the Image, as an AnimatedGif</param>
        public Image(AnimatedGif image)
            : base()
        {
            Picture = image;
        }

        /// <summary>
        /// Gets wether the mouse hovers over the MenuButton or not
        /// </summary>
        /// <param name="mouse">The current mouse position</param>
        /// <returns>true if the MenuButton is hovered, false otherwise.</returns>
        public override bool MouseOver(Vector2 mouse)
        {
            return base.MouseOver(mouse) && IsButton;
        }

        /// <summary>
        /// Before the Control is drawn
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        protected override void PreDraw(SpriteBatch sb)
        {
            DrawBackground(sb);

            base.PreDraw(sb);
        }
        /// <summary>
        /// Draws the Image
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Image</param>
        protected override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (Picture == null)
                return;

            if (Picture is Texture2D)
                sb.Draw(Picture as Texture2D, position + new Vector2(8f), null, colorText, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
            else if (Picture is AnimatedGif)
                sb.Draw(Picture as AnimatedGif, position + new Vector2(8f), null, colorText, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
            else
                throw new BadImageFormatException("This should never happen."); // a pun, I guess?
        }
    }
}

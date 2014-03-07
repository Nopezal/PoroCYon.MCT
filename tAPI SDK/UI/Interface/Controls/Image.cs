using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Graphics;
using TAPI.SDK.ObjectModel;
using TAPI.SDK.UI.Interface.Controls.Primitives;

namespace TAPI.SDK.UI.Interface.Controls
{
    /// <summary>
    /// An image
    /// </summary>
    public class Image : Control, IImageObject
    {
        Texture2D texture = null;
        AnimatedGif gif = null;

        Texture2D picAsTex
        {
            get
            {
                return IsGif ? gif.Frames[gif.Current] : texture;
            }
        }

        /// <summary>
        /// Wether the image is a .gif or not
        /// </summary>
        public bool IsGif
        {
            get;
            private set;
        }

        /// <summary>
        /// The picture shown
        /// </summary>
        public object Picture
        {
            get
            {
                return IsGif ? gif as object : texture;
            }
            set
            {
                if (value is AnimatedGif)
                {
                    IsGif = true;

                    gif = value as AnimatedGif;

                    texture.Dispose();
                    texture = null;
                }
                else if (value is Texture2D)
                {
                    IsGif = false;

                    foreach (Texture2D t in gif.Frames)
                        t.Dispose();
                    gif = null;

                    texture = value as Texture2D;
                }
                else
                    throw new ArgumentException("value has to be an AnimatedGif xor a Texture2D", "Image.set_Picture: value");
            }
        }

        ///// <summary>
        ///// The drawing origin of the control
        ///// </summary>
        //public override Vector2 Origin
        //{
        //    get
        //    {
        //        // this might be a good idea
        //        if (IsGif)
        //        {
        //            if (gif == null)
        //                return Vector2.Zero;
        //        }
        //        else if (texture == null)
        //            return Vector2.Zero;

        //        return new Vector2(picAsTex.Width, picAsTex.Height) * Scale;
        //    }
        //}

        /// <summary>
        /// The hitbox of the control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                // this might be a good idea
                if (IsGif)
                {
                    if (gif == null)
                        return Rectangle.Empty;
                }
                else if (texture == null)
                    return Rectangle.Empty;

                return new Rectangle((int)Position.X - 8, (int)Position.Y - 8, (int)(picAsTex.Width * Scale.X) + 16, (int)(picAsTex.Height * Scale.Y) + 16);
            }
        }

        /// <summary>
        /// Creates a new instance of the Image class
        /// </summary>
        /// <param name="tex">the picture of the Image</param>
        public Image(Texture2D tex)
            : base()
        {
            Picture = tex;
        }
        /// <summary>
        /// Creates a new instance of the Image class
        /// </summary>
        /// <param name="gif">the picture of the Image</param>
        public Image(AnimatedGif gif)
            : base()
        {
            Picture = gif;
        }

        /// <summary>
        /// Draws the control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (HasBackground)
                DrawBackground(sb);

            // this might be a good idea
            if (IsGif)
            {
                if (gif == null)
                    return;
            }
            else if (texture == null)
                return;

            if (IsGif)
                sb.Draw(gif, Position, null, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
            else
                sb.Draw(texture, Position, null, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
        }
    }
}

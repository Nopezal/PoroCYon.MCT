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
    /// An image that serves as a button
    /// </summary>
    public class ImageButton : Button, IImageObject
    {
        Texture2D tex = null;
        AnimatedGif gif = null;

        Texture2D picAsTex
        {
            get
            {
                return IsGif ? gif.Frames[gif.Current] : tex;
            }
        }

        /// <summary>
        /// Wether the ImageButton is a gif or not
        /// </summary>
        public bool IsGif
        {
            get;
            private set;
        }

        /// <summary>
        /// The picture of the ImageButton
        /// </summary>
        public object Picture
        {
            get
            {
                return IsGif ? gif as object : tex;
            }
            set
            {
                if (value is AnimatedGif)
                {
                    IsGif = true;

                    gif = value as AnimatedGif;

                    if (tex != null)
                        tex.Dispose();
                    tex = null;
                }
                else if (value is Texture2D)
                {
                    IsGif = false;

                    if (gif != null)
                        foreach (Texture2D t in gif.Frames)
                            t.Dispose();
                    gif = null;

                    tex = value as Texture2D;
                }
                else
                    throw new ArgumentException("value has to be (an AnimatedGif ^ a Texture2D)", "Image.set_Picture: value");
            }
        }

        ///// <summary>
        ///// The drawing origin of the Control
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
        //        else if (tex == null)
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
                else if (tex == null)
                    return Rectangle.Empty;

                return new Rectangle((int)Position.X, (int)Position.Y, (int)(picAsTex.Width * Scale.X), (int)(picAsTex.Height * Scale.Y));
            }
        }

        /// <summary>
        /// Creates a new instance of the ImageButton class
        /// </summary>
        /// <param name="tex">The picture of the ImageButton</param>
        public ImageButton(Texture2D tex)
            : base()
        {
            Picture = tex;
        }
        /// <summary>
        /// Creates a new instance of the ImageButton class
        /// </summary>
        /// <param name="gif">The picture of the ImageButton</param>
        public ImageButton(AnimatedGif gif)
            : base()
        {
            Picture = gif;
        }

        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the control</param>
        public override void Draw(SpriteBatch sb)
        {
            // this might be a good idea
            if (IsGif)
            {
                if (gif == null)
                    return;
            }
            else if (tex == null)
                return;

            if (IsGif)
                sb.Draw(gif, Position, null, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
            else
                sb.Draw(tex, Position, null, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

            base.Draw(sb);
        }
    }
}

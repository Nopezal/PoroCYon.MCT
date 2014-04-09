using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Graphics;
using PoroCYon.MCT.ObjectModel;

namespace PoroCYon.MCT.UI.Interface.Controls
{
    /// <summary>
    /// An image
    /// </summary>
    public class Image : Control, IImageObject
    {
        /// <summary>
        /// Gets the Picture as a Texture2D, even if it's an AnimatedGif
        /// </summary>
        protected Texture2D PicAsTexture
        {
            get
            {
                return IsGif ? ((AnimatedGif)Picture.Item).Frames[((AnimatedGif)Picture.Item).Current] : ((Texture2D)Picture.Item);
            }
        }

        /// <summary>
        /// Wether the ImageButton is a gif or not
        /// </summary>
        public bool IsGif
        {
            get
            {
                return Picture.UsedObjectNum == 1;
            }
        }

        /// <summary>
        /// The picture of the ImageButton
        /// </summary>
        public Union<Texture2D, AnimatedGif> Picture
        {
            get;
            set;
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
                if (Picture.Item == null)
                    return Rectangle.Empty;

                return new Rectangle((int)Position.X - 8, (int)Position.Y - 8, (int)(PicAsTexture.Width * Scale.X) + 16, (int)(PicAsTexture.Height * Scale.Y) + 16);
            }
        }

        /// <summary>
        /// Creates a new instance of the Image class
        /// </summary>
        /// <param name="tex">the picture of the Image</param>
        public Image(Texture2D tex)
            : base()
        {
            Picture = new Union<Texture2D, AnimatedGif>(tex, null);
        }
        /// <summary>
        /// Creates a new instance of the Image class
        /// </summary>
        /// <param name="gif">the picture of the Image</param>
        public Image(AnimatedGif gif)
            : base()
        {
            Picture = new Union<Texture2D, AnimatedGif>(null, gif);
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
            if (Picture.Item == null)
                return;

            sb.Draw(Picture, Position, null, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
        }
    }
}

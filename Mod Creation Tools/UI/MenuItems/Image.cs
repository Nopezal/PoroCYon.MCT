using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.Extensions;
using PoroCYon.Extensions.Xna.Graphics;
using PoroCYon.MCT.ObjectModel;

namespace PoroCYon.MCT.UI.MenuItems
{
    using ImageUnion = Union<Texture2D, AnimatedGif>;

    /// <summary>
    /// An image. Uses MenuButton.size as scale
    /// </summary>
    public class Image : Control, IImageObject
    {
        /// <summary>
        /// Wether the Image can be used as a button or not.
        /// </summary>
        public bool IsButton = true;

        /// <summary>
        /// Wether the image is a gif or not
        /// </summary>
        public bool IsGif
        {
            get
            {
                return Picture.UsedObjectNum == 1;
            }
        }

        /// <summary>
        /// The picture of the IImageObject
        /// </summary>
        public ImageUnion Picture
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Picture as a Texture2D, even if it's an AnimatedGif.
        /// </summary>
        protected Texture2D PicAsTexture
        {
            get
            {
                return IsGif ? ((AnimatedGif)Picture.Item).Frames[((AnimatedGif)Picture.Item).Current] : ((Texture2D)Picture.Item);
            }
        }

        /// <summary>
        /// Creates a new instance of the Image class
        /// </summary>
        /// <param name="image">The picture of the Image, as a Texture2D</param>
        public Image(Texture2D image)
            : base()
        {
            Picture = new ImageUnion(image, null);
        }
        /// <summary>
        /// Creates a new instance of the Image class
        /// </summary>
        /// <param name="image">The picture of the Image, as an AnimatedGif</param>
        public Image(AnimatedGif image)
            : base()
        {
            Picture = new ImageUnion(null, image);
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

            sb.Draw(Picture, position + new Vector2(8f), null, colorText, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
        }
    }
}

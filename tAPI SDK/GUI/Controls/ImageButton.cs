using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Graphics;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
    public class ImageButton : Button, IImageControl
    {
        Texture2D texture = null;
        AnimatedGif gif = null;

        Texture2D picAsTex
        {
            get
            {
                return IsGif ? gif.Frames[0] : texture;
            }
        }

        public bool IsGif
        {
            get;
            private set;
        }
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
                    throw new ArgumentException("value has to be (an AnimatedGif ^ a Texture2D)", "Image.set_Picture: value");
            }
        }

        public override Vector2 Origin
        {
            get
            {
                // this might be a good idea
                if (IsGif)
                {
                    if (gif == null)
                        return Vector2.Zero;
                }
                else if (texture == null)
                    return Vector2.Zero;

                return new Vector2(picAsTex.Width, picAsTex.Height) * Scale + padding;
            }
        }
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

                return new Rectangle((int)Position.X - (int)padding.X, (int)Position.Y - (int)padding.Y,
                    (int)(picAsTex.Width * Scale.X) + (int)padding.X, (int)(picAsTex.Height * Scale.Y) + (int)padding.Y);
            }
        }

        public ImageButton()
            : base()
        {
            IsGif = false;
        }
        public ImageButton(Texture2D tex)
            : base()
        {
            Picture = tex;
        }
        public ImageButton(AnimatedGif gif)
            : base()
        {
            Picture = gif;
        }

        public override void Draw(SpriteBatch sb)
        {
            // this might be a good idea
            if (IsGif)
            {
                if (gif == null)
                    return;
            }
            else if (texture == null)
                return;

            if (IsGif)
                sb.Draw(gif, Position, Frame, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
            else
                sb.Draw(texture, Position, Frame, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

            base.Draw(sb);
        }
    }
}

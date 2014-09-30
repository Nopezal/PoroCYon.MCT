using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.UI.Interface.Controls
{
    /// <summary>
    /// A combination of three sliders forming a colour chooser with H, S and L channels.
    /// </summary>
    public class ColourChooser : Control
    {
        Slider h, s, l;

        static Texture2D colourBG
        {
            get
            {
                return Main.colorBarTexture;
            }
        }
        static Texture2D hueBG
        {
            get
            {
                return API.main.hueTexture;
            }
        }

        /// <summary>
        /// The colour value of the ColourChooser, with flipped A channel.
        /// </summary>
        public Color Value
        {
            get
            {
                Color c = Main.hslToRgb(h.Value, s.Value, l.Value);
                c.A = 0;
                return c;
            }
            set
            {
                Vector3 val = Main.rgbToHsl(value);

                h.Value = val.X;
                s.Value = val.Y;
                l.Value = val.Z;
            }
        }

        /// <summary>
        /// Creates a new instance of the ColourChooser class
        /// </summary>
        public ColourChooser()
            : this(Color.White)
        {

        }
        /// <summary>
        /// Creates a new instance of the ColourChooser class
        /// </summary>
        /// <param name="defaultColour">The colour value of the ColourChooser</param>
        public ColourChooser(Color defaultColour)
            : base()
        {
            Vector3 v = Main.rgbToHsl(defaultColour);

            h = new Slider(v.X, 0f, 1f)
            {
                Background = hueBG
            };
            s = new Slider(v.Y, 0f, 1f)
            {
                Background = colourBG,
                Position = new Vector2(0f, 24f)
            };
            l = new Slider(v.Z, 0f, 1f)
            {
                Background = colourBG,
                Position = new Vector2(0f, 48f)
            };
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            h.Position += Position;
            h.Update();
            h.Position -= Position;

            s.Position += Position;
            s.Update();
            s.Position -= Position;

            l.Position += Position;
            l.Update();
            l.Position -= Position;
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

            h.Position += Position;
            h.Draw(sb);
            h.Position -= Position;

            s.Position += Position;
            s.Draw(sb);
            s.Position -= Position;

            l.Position += Position;
            l.Draw(sb);
            l.Position -= Position;
        }
    }
}

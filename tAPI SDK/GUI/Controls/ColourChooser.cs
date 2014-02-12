using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
	public class ColourChooser : Control
	{
		Slider h, s, l;

		static Texture2D colourBG
		{
			get
			{
                return Constants.mainInstance.colorBarTexture;
			}
		}
        static Texture2D hueBG
        {
            get
            {
                return Constants.mainInstance.hueTexture;
            }
        }

        public Color Value
        {
            get
            {
                return Main.hslToRgb(h.Value, s.Value, l.Value);
            }
            set
            {
                Vector3 val = Main.rgbToHsl(value);

                h.Value = val.X;
                s.Value = val.Y;
                l.Value = val.Z;
            }
        }

        public ColourChooser()
        {
            h = new Slider(0f, 1f)
            {
                Background = hueBG
            };
            s = new Slider(0f, 1f)
            {
                Background = colourBG,
                Position = new Vector2(0f, 24f)
            };
            l = new Slider(0f, 1f)
            {
                Background = colourBG,
                Position = new Vector2(0f, 48f)
            };
        }
        public ColourChooser(Color @default)
            : this()
        {
            Vector3 val = Main.rgbToHsl(@default);

            h.Value = val.X;
            s.Value = val.Y;
            l.Value = val.Z;
        }

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
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

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

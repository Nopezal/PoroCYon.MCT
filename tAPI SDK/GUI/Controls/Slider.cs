using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.Input;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
	public enum SliderOrientation
	{
		Horizontal,
		Vertical
	}

	public class Slider : Control
	{
        float value = 0f, min = 0f, max = 100f;

		public SliderOrientation Orientation;
		public bool Reversed = false;

		public Texture2D Background = null;
        public Color SliderColour = Color.White;

        public Action<Slider, float> OnValueChanged = null;
        public static Action<Slider, float> GlobalValueChanged = null;

        public Texture2D DrawnBackground
        {
            get
            {
                return Background ?? Constants.mainInstance.colorBarTexture;
            }
        }

        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, 16 + (int)(DrawnBackground.Width * Scale.X), 16 + (int)(DrawnBackground.Height * Scale.Y));
            }
        }
        public Rectangle SliderHitbox
        {
            get
            {
                Vector2 Size = new Vector2(slider.Width, slider.Height) * Scale;
                if (Orientation == SliderOrientation.Vertical)
                    Size = new Vector2(Size.Y, Size.X);

                float add = MathHelper.Lerp(0f, DrawnBackground.Width * Scale.X, (Value - Min) / Max);
                Vector2 Pos = Position + new Vector2(8f) + (Orientation == SliderOrientation.Horizontal ? new Vector2(add, 0f) : new Vector2(0f, add));

                return new Rectangle((int)Pos.X, (int)Pos.Y, (int)Size.X, (int)Size.Y);
            }
        }

		static Texture2D slider
		{
			get
			{
                return Constants.mainInstance.colorSliderTexture;
			}
		}

        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value < min || value > max)
                    throw new ArgumentOutOfRangeException("value");
            }
        }
        public float Min
        {
            get
            {
                return min;
            }
            set
            {
                if (value > this.value || value > max)
                    throw new ArgumentOutOfRangeException("value");
            }
        }
        public float Max
        {
            get
            {
                return max;
            }
            set
            {
                if (value < this.value || value < min)
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        public Slider()
            : base()
        {

        }
        public Slider(float value)
            : this()
        {
            Value = value;
        }
        public Slider(float min, float max)
            : this()
        {
            Min = min;
            Max = max;

            Value = MathHelper.Clamp(Value, Min, Max);
        }
        public Slider(float value, float min, float max)
            : this(min, max)
        {
            Value = value;
        }

		public override void Update()
		{
			base.Update();

            if (GInput.Mouse.Left && GInput.Mouse.Rectangle.Intersects(Hitbox)) // not SliderHitbox, so you can insta-jump to a value instead of dragging
            {
                float val = MathHelper.Lerp(Min, Max, Orientation == SliderOrientation.Horizontal
                    ? (GInput.Mouse.Position.X - Position.X) / Hitbox.Width : (GInput.Mouse.Position.Y - Position.Y) / Hitbox.Height);

                if (val != Value)
                {
                    float old = Value;
                    Value = val;

                    ValueChanged(old);

                    if (OnValueChanged == null)
                        OnValueChanged(this, old);
                    if (GlobalValueChanged == null)
                        GlobalValueChanged(this, old);
                }
            }
		}
		public override void Draw(SpriteBatch sb)
		{
			base.Draw(sb);

            DrawBackground(sb);

            // welp

            sb.Draw(DrawnBackground, Position + new Vector2(8f, 8f + Hitbox.Height / 2f - (DrawnBackground.Width / 2f) * Scale.Y), null, Colour,
                Rotation + (Orientation == SliderOrientation.Horizontal ? 0f : MathHelper.PiOver2), Origin,
                Orientation == SliderOrientation.Horizontal ? Scale : new Vector2(Scale.Y, Scale.X), SpriteEffects.None, 0f);

            sb.Draw(slider, Position + new Vector2(8f, Hitbox.Height / 2f - (DrawnBackground.Width / 2f) * Scale.Y) + (Orientation == SliderOrientation.Horizontal
                ? new Vector2(MathHelper.Lerp(0f, DrawnBackground.Width * Scale.X, (Value - Min) / Max), 0f)
                : new Vector2(0f, MathHelper.Lerp(0f, DrawnBackground.Height * Scale.Y, (Value - Min) / Max))), null, SliderColour,
                Rotation + (Orientation == SliderOrientation.Horizontal ? 0f : MathHelper.PiOver2), Origin,
                Orientation == SliderOrientation.Horizontal ? Scale : new Vector2(Scale.Y, Scale.X), SpriteEffects.None, 0f);
		}

        protected virtual void ValueChanged(float oldValue) { }
	}
}

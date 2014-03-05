using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.Input;
using TAPI.SDK.UI.Interface.Controls.Primitives;

namespace TAPI.SDK.UI.Interface.Controls
{
    /// <summary>
    /// The orientation of a Slider
    /// </summary>
    public enum SliderOrientation
    {
        /// <summary>
        /// The slider is horizontal
        /// </summary>
        Horizontal,
        /// <summary>
        /// The slider is vertical
        /// </summary>
        Vertical
    }

    /// <summary>
    /// A value represented with a slider which can be changed
    /// </summary>
    public class Slider : Focusable
    {
        float value = 0f, min = 0f, max = 100f;

        Texture2D background = null;

        /// <summary>
        /// The orientation of the slider. Default is horizontal.
        /// </summary>
        public SliderOrientation Orientation = SliderOrientation.Horizontal;
        /// <summary>
        /// Wether the slider is reversed or not
        /// </summary>
        public bool Reversed = false;

        static Texture2D slider
        {
            get
            {
                return Constants.mainInstance.colorSliderTexture;
            }
        }

        /// <summary>
        /// The background texture of the slider
        /// </summary>
        public Texture2D Background
        {
            get
            {
                return background ?? Constants.mainInstance.colorBarTexture;
            }
            set
            {
                background = value;
            }
        }

        /// <summary>
        /// The hitbox of the control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, 16 + (int)(Background.Width * Scale.X), 16 + (int)(Background.Height * Scale.Y));
            }
        }
        /// <summary>
        /// The hitbox of the slider
        /// </summary>
        public Rectangle SliderHitbox
        {
            get
            {
                Vector2 Size = new Vector2(slider.Width, slider.Height) * Scale;
                if (Orientation == SliderOrientation.Vertical)
                    Size = new Vector2(Size.Y, Size.X);

                float add = MathHelper.Lerp(0f, Background.Width * Scale.X, (Value - Min) / Max);
                Vector2 Pos = Position + new Vector2(8f) + (Orientation == SliderOrientation.Horizontal ? new Vector2(add, 0f) : new Vector2(0f, add));

                return new Rectangle((int)Pos.X, (int)Pos.Y, (int)Size.X, (int)Size.Y);
            }
        }

        /// <summary>
        /// The value of the slider
        /// </summary>
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

                float old = this.value;

                this.value = value;

                ValueChanged(old);
            }
        }
        /// <summary>
        /// The minimum value of the slider
        /// </summary>
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

                min = value;
            }
        }
        /// <summary>
        /// The maximum value of the slider
        /// </summary>
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

                max = value;
            }
        }

        /// <summary>
        /// When the value of the slider is changed.
        /// </summary>
        public Action<Slider, float> OnValueChanged = null;
        /// <summary>
        /// When the value of a slider is changed.
        /// </summary>
        public static Action<Slider, float> GlobalValueChanged = null;

        /// <summary>
        /// Creates a new instance of the Slider class
        /// </summary>
        public Slider()
            : this(0f, 0f,  100f)
        {

        }
        /// <summary>
        /// Creates a new instance of the Slider class
        /// </summary>
        /// <param name="value">The value of the Slider</param>
        public Slider(float value)
            : this(value, 0f, 100f)
        {

        }
        /// <summary>
        /// Creates a new instance of the Slider class
        /// </summary>
        /// <param name="min">The minimum value of the Slider</param>
        /// <param name="max">The maximum value of the Slider</param>
        public Slider(float min, float max)
            : this(0f, min, max)
        {

        }
        /// <summary>
        /// Creates a new instance of the Slider class
        /// </summary>
        /// <param name="value">The value of the Slider</param>
        /// <param name="min">The minimum value of the Slider</param>
        /// <param name="max">v</param>
        public Slider(float value, float min, float max)
            : base()
        {
            this.min = min;
            this.max = max;

            Value = value;

            // check for argumentoutofrange
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Updates the control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (IsFocused)
            {
                float val = MathHelper.Lerp(Min, Max, Orientation == SliderOrientation.Horizontal
                    ? (GInput.Mouse.Position.X - Position.X) / Hitbox.Width : (GInput.Mouse.Position.Y - Position.Y) / Hitbox.Height);

                if (Reversed)
                    val = max - value;

                if (val != Value)
                {
                    float old = Value;
                    Value = val;

                    ValueChanged(old);
                }
            }
        }
        /// <summary>
        /// Draws the control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            DrawBackground(sb);

            sb.Draw(Background, Position + new Vector2(8f, 8f + Hitbox.Height / 2f - (Background.Width / 2f) * Scale.Y), null, Colour,
                Rotation + (Orientation == SliderOrientation.Horizontal ? 0f : MathHelper.PiOver2), Origin,
                Orientation == SliderOrientation.Horizontal ? Scale : new Vector2(Scale.Y, Scale.X), SpriteEffects.None, 0f);

            float val = MathHelper.Lerp(0f, Background.Width * Scale.X, (Value - Min) / Max);
            if (Reversed)
                val = max - value;

            sb.Draw(slider, Position + new Vector2(8f, Hitbox.Height / 2f - (Background.Width / 2f) * Scale.Y)
                + (Orientation == SliderOrientation.Horizontal ? new Vector2(val, 0f) : new Vector2(0f, val)), null, SecondaryColour,
                Rotation + (Orientation == SliderOrientation.Horizontal ? 0f : MathHelper.PiOver2), Origin,
                Orientation == SliderOrientation.Horizontal ? Scale : new Vector2(Scale.Y, Scale.X), SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Changes the value of the Slider
        /// </summary>
        /// <param name="oldValue">The old slider value</param>
        protected virtual void ValueChanged(float oldValue)
        {
            if (OnValueChanged == null)
                OnValueChanged(this, oldValue);
            if (GlobalValueChanged == null)
                GlobalValueChanged(this, oldValue);
        }
    }
}

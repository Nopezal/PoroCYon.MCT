using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.MCT.Input;
using PoroCYon.MCT.UI.Interface.Controls.Primitives;
using Terraria;

namespace PoroCYon.MCT.UI.Interface.Controls
{
    /// <summary>
    /// The orientation a ScrollBar can have
    /// </summary>
    public enum ScrollBarOrientation
    {
        /// <summary>
        /// The ScrollBar is horizontal
        /// </summary>
        Horizontal,
        /// <summary>
        /// The ScrollBar is vertical
        /// </summary>
        Vertical
    }

    /// <summary>
    /// A bar used for scrolling. Supports the scroll wheel.
    /// </summary>
    public class ScrollBar : Focusable
    {
        float val = 0f, min = 0f, max = 100f;
        ScrollBarOrientation orientation = ScrollBarOrientation.Horizontal;

        /// <summary>
        /// The size of the ScrollBar
        /// </summary>
        public Vector2 Size;
        /// <summary>
        /// The amount ScrollBar.Value increases or decreases when the scroll wheel is used.
        /// </summary>
        public float MouseScrollStep = 1f;

        /// <summary>
        /// Gets or sets the value of the ScrollBar
        /// </summary>
        public float Value
        {
            get
            {
                return val;
            }
            set
            {
                val = MathHelper.Clamp(value, min, max);
            }
        }
        /// <summary>
        /// Gets or sets the minimum value of the ScrollBar
        /// </summary>
        public float MinValue
        {
            get
            {
                return min;
            }
            set
            {
                if (value >= max)
                    throw new ArgumentOutOfRangeException("value");

                val = Math.Max(val, min = value);
            }
        }
        /// <summary>
        /// Gets or sets the maximum value of the ScrollBar
        /// </summary>
        public float MaxValue
        {
            get
            {
                return max;
            }
            set
            {
                if (value <= min)
                    throw new ArgumentOutOfRangeException("value");

                val = Math.Min(val, max = value);
            }
        }

        /// <summary>
        /// The orientation of the ScrollBar
        /// </summary>
        public ScrollBarOrientation Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                if (value != orientation)
                {
                    orientation = value;

                    Size = new Vector2(Size.Y, Size.X);
                }
            }
        }
        /// <summary>
        /// Wether the ScrollBar is reversed or not
        /// </summary>
        public bool IsReversed = false;

        /// <summary>
        /// Gets the Hitbox of the Control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(Size.X * Scale.X), (int)(Size.Y * Scale.Y));
            }
        }

        /// <summary>
        /// When the Value of the ScrollBar is changed
        /// </summary>
        public Action<ScrollBar, float, float> OnValueChanged;
        /// <summary>
        /// When the Value of a ScrollBar is changed
        /// </summary>
        public static Action<ScrollBar, float, float> GlobalValueChanged;

        /// <summary>
        /// Creates a new instance of the ScrollBar class
        /// </summary>
        public ScrollBar()
            : this(0f, 0f, 10f, ScrollBarOrientation.Horizontal)
        {

        }
        /// <summary>
        /// Creates a new instance of the ScrollBar class
        /// </summary>
        /// <param name="value">The value of the ScrollBar</param>
        public ScrollBar(float value)
            : this(value, 0f, 10f, ScrollBarOrientation.Horizontal)
        {

        }
        /// <summary>
        /// Creates a new instance of the ScrollBar class
        /// </summary>
        /// <param name="minValue">The minimum value of the ScrollBar</param>
        /// <param name="maxValue">The maximum value of the ScrollBar</param>
        public ScrollBar(float minValue, float maxValue)
            : this(minValue, minValue, maxValue, ScrollBarOrientation.Horizontal)
        {

        }
        /// <summary>
        /// Creates a new instance of the ScrollBar class
        /// </summary>
        /// <param name="orientation">The orientation of the ScrollBar</param>
        /// <param name="isReversed">Wether the ScrollBar is reversed or not</param>
        public ScrollBar(ScrollBarOrientation orientation, bool isReversed = false)
            : this(0f, 0f, 10f, orientation, isReversed)
        {

        }
        /// <summary>
        /// Creates a new instance of the ScrollBar class
        /// </summary>
        /// <param name="value">The value of the ScrollBar</param>
        /// <param name="minValue">The minimum value of the ScrollBar</param>
        /// <param name="maxValue">The maximum value of the ScrollBar</param>
        public ScrollBar(float value, float minValue, float maxValue)
            : this(value, minValue, maxValue, ScrollBarOrientation.Horizontal)
        {

        }
        /// <summary>
        /// Creates a new instance of the ScrollBar class
        /// </summary>
        /// <param name="value">The value of the ScrollBar</param>
        /// <param name="minValue">The minimum value of the ScrollBar</param>
        /// <param name="maxValue">The maximum value of the ScrollBar</param>
        /// <param name="orientation">The orientation of the ScrollBar</param>
        /// <param name="isReversed">Wether the ScrollBar is reversed or not</param>
        public ScrollBar(float value, float minValue, float maxValue, ScrollBarOrientation orientation, bool isReversed = false)
        {
            val = value;
            min = minValue;
            max = maxValue;
            this.orientation = orientation;
            isReversed = IsReversed;

            Size = orientation == ScrollBarOrientation.Horizontal ? new Vector2(100f, 16f) : new Vector2(16f, 100f);
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (IsFocused && GInput.Mouse.ScrollWheel != 0)
            {
                float old = val;
                Value += (GInput.Mouse.ScrollWheel / 120f) * MouseScrollStep;

                ValueChanged(old, val);

                Main.PlaySound(12);
            }

            if (IsHovered && IsFocused && (GInput.Mouse.Left || ForceFocus))
            {
                float old = val;

                Value = MathHelper.Clamp(
                    orientation == ScrollBarOrientation.Horizontal
                        ? (GInput.Mouse.Position.X - (Position.X + 4f + Scale.X / 2)) / (Size.X - 8f)
                        : (GInput.Mouse.Position.Y - (Position.Y + 4f + Scale.Y / 2)) / (Size.Y - 8f),
                    0f, 1f) * (max - min);

                ValueChanged(old, val);
            }
        }
        /// <summary>
        /// Draws the Contorl
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            DrawBackground(sb, Hitbox);

            float pxSize = Math.Max(4f, (orientation == ScrollBarOrientation.Horizontal ? Size.X : Size.Y) / (max - min));
            Vector2
                offset = orientation == ScrollBarOrientation.Horizontal
                ? new Vector2(    (val / (max - min)) * (Size.X - 16f) + 8f, 7f)
                : new Vector2(7f, (val / (max - min)) * (Size.Y - 16f) + 8f),
                scale  = orientation == ScrollBarOrientation.Horizontal ? new Vector2(pxSize, 8f) : new Vector2(8f, pxSize);
            
            if (IsReversed)
                if (orientation == ScrollBarOrientation.Horizontal)
                    offset.X = Size.X - offset.X;
                else
                    offset.Y = Size.Y - offset.Y;

            if (orientation == ScrollBarOrientation.Horizontal)
            {
                if (offset.X > (Size.X - 16f) + 8f - pxSize)
                    offset.X = (Size.X - 16f) + 8f - pxSize;
            }
            else if (offset.Y > (Size.Y - 16f) + 8f - pxSize)
                     offset.Y = (Size.Y - 16f) + 8f - pxSize;

            sb.Draw(MctUI.WhitePixel, Position + offset * Scale, null, Colour, Rotation, Origin, scale * Scale, SpriteEffects, LayerDepth);
        }

        /// <summary>
        /// Changes the value of the ScrollBar
        /// </summary>
        /// <param name="oldValue">The old Value of the ScrollBar</param>
        /// <param name="newValue">The new Value of the ScrollBar</param>
        protected virtual void ValueChanged(float oldValue, float newValue)
        {
            if (OnValueChanged != null)
                OnValueChanged(this, oldValue, newValue);
            if (GlobalValueChanged != null)
                GlobalValueChanged(this, oldValue, newValue);
        }
    }
}

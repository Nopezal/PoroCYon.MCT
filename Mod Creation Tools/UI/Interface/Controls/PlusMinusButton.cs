using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.Extensions;
using PoroCYon.Extensions.Xna.Geometry;
using Terraria;
using PoroCYon.MCT.Input;

namespace PoroCYon.MCT.UI.Interface.Controls
{
    /// <summary>
    /// A control with a value which can be edited with a + and - button
    /// </summary>
    public class PlusMinusButton : TextBlock
    {
        int cd = 7;

        /// <summary>
        /// Gets the text and the value, if it should be shown.
        /// </summary>
        public string TextWithValue
        {
            get
            {
                return Text + (ShowValue ? (Text.IsEmpty() ? "" : ": ") + Value : "");
            }
        }

        /// <summary>
        /// Called when the value of the PlusMinusButton has changed
        /// </summary>
        public Action<PlusMinusButton, float, float> OnValueChanged;
        /// <summary>
        /// Called when the value of a PlusMinusButton has changed
        /// </summary>
        public static Action<PlusMinusButton, float, float> GlobalValueChanged;

        /// <summary>
        /// The value of the PlusMinusButton
        /// </summary>
        public float Value = 0;
        /// <summary>
        /// The step size of the PlusMinusButton
        /// </summary>
        public float Step = 1f;
        /// <summary>
        /// Wether to draw the value after the text or not
        /// </summary>
        public bool ShowValue = true;

        /// <summary>
        /// The hitbox of the Control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                Rectangle ret = new Rectangle((int)Position.X - 8, (int)Position.Y - 8,
                    (int)(Scale.X * (Size.HasValue ? Size.Value.X : Font.MeasureString(TextWithValue).X)) + 16,
                    (int)(Scale.Y * (Size.HasValue ? Size.Value.Y : Font.MeasureString(TextWithValue).Y)) + 16);

                ret.Width += 32;

                return ret;
            }
        }
        /// <summary>
        /// The Hitbox of the '+' button
        /// </summary>
        public Rectangle IncreaseHitbox
        {
            get
            {
                Rectangle ret = Hitbox;

                ret.X += Hitbox.Width + 40 - 28 - 48;
                ret.Y += 8;
                ret.Width = 24;
                ret.Height -= 16;

                return ret;
            }
        }
        /// <summary>
        /// The Hitbox of the '-' button
        /// </summary>
        public Rectangle DecreaseHitbox
        {
            get
            {
                Rectangle ret = Hitbox;

                ret.X += Hitbox.Width + 40 - 28 - 24;
                ret.Y += 8;
                ret.Width = 24;
                ret.Height -= 16;

                return ret;
            }
        }

        /// <summary>
        /// Creates a new instance of the PlusMinusButton class
        /// </summary>
        public PlusMinusButton()
            : this(0f, 1f, "")
        {

        }
        /// <summary>
        /// Creates a new instance of the PlusMinusButton class
        /// </summary>
        /// <param name="text">The text of the PlusMinusButton</param>
        public PlusMinusButton(string text)
            : this(0f, 1f, text)
        {

        }
        /// <summary>
        /// Creates a new instance of the PlusMinusButton class
        /// </summary>
        /// <param name="value">The value of the PlusMinusButton</param>
        public PlusMinusButton(float value)
            : this(value, 1f, "")
        {

        }
        /// <summary>
        /// Creates a new instance of the PlusMinusButton class
        /// </summary>
        /// <param name="value">The value of the PlusMinusButton</param>
        /// <param name="step">The step size of the PlusMinusButton</param>
        public PlusMinusButton(float value, float step)
            : this(value, step, "")
        {

        }
        /// <summary>
        /// Creates a new instance of the PlusMinusButton class
        /// </summary>
        /// <param name="value">The value of the PlusMinusButton</param>
        /// <param name="text">The text of the PlusMinusButton</param>
        public PlusMinusButton(float value, string text)
            : this(value, 1f, text)
        {

        }
        /// <summary>
        /// Creates a new instance of the PlusMinusButton class
        /// </summary>
        /// <param name="value">The value of the PlusMinusButton</param>
        /// <param name="step">The step size of the PlusMinusButton</param>
        /// <param name="text">The text of the PlusMinusButton</param>
        public PlusMinusButton(float value, float step, string text)
            : base()
        {
            Value = value;
            Step = step;
            Text = text;
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (cd > 0)
                cd--;

            if (GInput.Mouse.Rectangle.Intersects(IncreaseHitbox) && GInput.Mouse.Left && cd <= 0)
            {
                GInput.BlockMouse = true;

                cd = 7;

                Value += Step;
                ValueChanged(Value - Step, Value);

                Main.PlaySound(12);
            }
            if (GInput.Mouse.Rectangle.Intersects(DecreaseHitbox) && GInput.Mouse.Left && cd <= 0)
            {
                GInput.BlockMouse = true;

                cd = 7;

                Value -= Step;
                ValueChanged(Value + Step, Value);

                Main.PlaySound(12);
            }
        }
        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            string oldText = Text;

            Text = TextWithValue;

            base.Draw(sb);

            Text = oldText;

            DrawBackground(sb, IncreaseHitbox);
            DrawBackground(sb, DecreaseHitbox);

            MctUI.DrawOutlinedString(sb, Font, "+", IncreaseHitbox.Centre() - Font.MeasureString("+") / 2f + new Vector2(0f, 4f), Colour);
            MctUI.DrawOutlinedString(sb, Font, "-", DecreaseHitbox.Centre() - Font.MeasureString("-") / 2f + new Vector2(0f, 4f), Colour);
        }

        /// <summary>
        /// Changes the value of the PlsuMinusButton
        /// </summary>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        protected virtual void ValueChanged(float oldValue, float newValue)
        {
            if (OnValueChanged != null)
                OnValueChanged(this, oldValue, newValue);
            if (GlobalValueChanged != null)
                GlobalValueChanged(this, oldValue, newValue);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.UI.Interface.Controls.Primitives;

namespace TAPI.SDK.UI.Interface.Controls
{
    /// <summary>
    /// A control with a value which can be edited with a + and - button
    /// </summary>
    public class PlusMinusButton : TextBlock
    {
        TextButton incr, decr;

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
        /// Creates a new instance of the PlusMinusButton class
        /// </summary>
        public PlusMinusButton()
            : this(0f, 1f)
        {

        }
        /// <summary>
        /// Creates a new instance of the PlusMinusButton class
        /// </summary>
        /// <param name="value">The value of the PlusMinusButton</param>
        public PlusMinusButton(float value)
            : this(value, 1f)
        {

        }
        /// <summary>
        /// Creates a new instance of the PlusMinusButton class
        /// </summary>
        /// <param name="value">The value of the PlusMinusButton</param>
        /// <param name="step">The step size of the PlusMinusButton</param>
        public PlusMinusButton(float value, float step)
            : base()
        {
            incr = new TextButton("+")
            {
                StayFocused = true
            };
            incr.OnClicked += (b) =>
            {
                Value += Step;
            };

            decr = new TextButton("-")
            {
                StayFocused = true
            };
            decr.OnClicked += (b) =>
            {
                Value += Step;
            };
        }

        /// <summary>
        /// The hitbox of the Control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                Rectangle ret = base.Hitbox;

                ret.Width += incr.Hitbox.Width + decr.Hitbox.Width + 16;

                return ret;
            }
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            incr.Position += Position + new Vector2(Hitbox.Width - decr.Hitbox.Width - 16f, 0f);
            incr.Update();
            incr.Position -= Position + new Vector2(Hitbox.Width - decr.Hitbox.Width - 16f, 0f);

            decr.Position += Position + new Vector2(Hitbox.Width - 8f, 0f);
            decr.Update();
            decr.Position -= Position + new Vector2(Hitbox.Width - 8f, 0f);
        }
        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            (this as Control).Draw(sb); // skip TextBlock.Draw

            if (HasBackground)
                DrawBackground(sb);

            DrawOutlinedString(sb, Font, Text + (ShowValue ? ": " + Value : ""), Colour);

            incr.Position += Position + new Vector2(Hitbox.Width - decr.Hitbox.Width - 16f, 0f);
            incr.Draw(sb);
            incr.Position -= Position + new Vector2(Hitbox.Width - decr.Hitbox.Width - 16f, 0f);

            decr.Position += Position + new Vector2(Hitbox.Width - 8f, 0f);
            decr.Draw(sb);
            decr.Position -= Position + new Vector2(Hitbox.Width - 8f, 0f);
        }
    }
}

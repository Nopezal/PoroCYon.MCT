using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Geometry;
using TAPI.SDK.Input;

namespace TAPI.SDK.UI.MenuItems
{
    /// <summary>
    /// A checkbox
    /// </summary>
    public class CheckBox : Control
    {
        /// <summary>
        /// Called when the CheckBox is checked
        /// </summary>
        public Action<CheckBox> OnChecked;
        /// <summary>
        /// Called when the CheckBox is unchecked
        /// </summary>
        public Action<CheckBox> OnUnchecked;
        /// <summary>
        /// Called when a CheckBox is checked
        /// </summary>
        public static Action<CheckBox> GlobalChecked;
        /// <summary>
        /// Called when a CheckBox is unchecked
        /// </summary>
        public static Action<CheckBox> GlobalUnchecked;

        /// <summary>
        /// The character to display in the box when the CheckBox is checked
        /// </summary>
        public char DisplayChar = 'X';

        bool isChecked = false;
        /// <summary>
        /// Wether the CheckBox is checked or not
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (isChecked = value)
                    Checked();
                else
                    Unchecked();
            }
        }

        /// <summary>
        /// Creates a new instance of the CheckBox class
        /// </summary>
        public CheckBox()
            : base()
        {
            canMouseOver = true;
            Text = "CheckBox";
        }
        /// <summary>
        /// Creates a new instance of the CheckBox class
        /// </summary>
        /// <param name="isChecked">Wether the CheckBox is checked or not</param>
        public CheckBox(bool isChecked)
            : this()
        {
            this.isChecked = isChecked;
        }
        /// <summary>
        /// Creates a new instance of the CheckBox class
        /// </summary>
        /// <param name="text">The text of the CheckBox</param>
        public CheckBox(string text)
            : this()
        {
            Text = text;
        }
        /// <summary>
        /// Creates a new instance of the CheckBox class
        /// </summary>
        /// <param name="isChecked">Wether the CheckBox is checked or not</param>
        /// <param name="text">The text of the CheckBox</param>
        public CheckBox(bool isChecked, string text)
            : this(isChecked)
        {
            Text = text;
        }

        /// <summary>
        /// The hitbox of the checkable box
        /// </summary>
        public Rectangle BoxHitbox
        {
            get
            {
                Vector2 charSize = Font.MeasureString(DisplayChar.ToString()) * Scale;

                return new Rectangle((int)position.X + 8, (int)position.Y + 8, (int)charSize.X, (int)charSize.Y);
            }
        }

        /// <summary>
        /// Called before the Control is drawn
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the control</param>
        protected override void PreDraw(SpriteBatch sb)
        {
            DrawBackground(sb);

            base.PreDraw(sb);
        }
        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the control</param>
        protected override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            Vector2 charSize = Font.MeasureString(DisplayChar.ToString());

            // draw box

            // top, horizontal
            sb.Draw(SdkUI.WhitePixel, BoxHitbox.Position(), null, MouseOver ? colorMouseOver : colorText, Rotation, Origin,
                new Vector2(charSize.X + 6f, 2f) * Scale, SpriteEffects, LayerDepth);
            // bottom, horizontal
            sb.Draw(SdkUI.WhitePixel, BoxHitbox.Position() + new Vector2(0f, charSize.Y - 4f) * Scale, null, MouseOver ? colorMouseOver : colorText, Rotation, Origin,
                new Vector2(charSize.X + 8f, 2f) * Scale, SpriteEffects, LayerDepth);

            // left, vertical
            sb.Draw(SdkUI.WhitePixel, BoxHitbox.Position(), null, MouseOver ? colorMouseOver : colorText, Rotation, Origin,
                new Vector2(2f, charSize.Y - 4f) * Scale, SpriteEffects, LayerDepth);
            // right, vertical
            sb.Draw(SdkUI.WhitePixel, BoxHitbox.Position() + new Vector2(charSize.X + 6f, 0f) * Scale, null, MouseOver ? colorMouseOver : colorText, Rotation, Origin,
                new Vector2(2f, charSize.Y - 4f) * Scale, SpriteEffects, LayerDepth);

            if (IsChecked)
                SdkUI.DrawOutlinedString(sb, Font, DisplayChar.ToString(), BoxHitbox.Position() + new Vector2(4f, 2f), MouseOver ? colorMouseOver : colorText, null, 2f, Scale, Rotation, Origin, SpriteEffects, LayerDepth);
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (MouseOver && Main.mouseLeft && Main.mouseLeftRelease)
            {
                IsChecked = !IsChecked;
                Main.PlaySound(12);
            }
        }

        /// <summary>
        /// Checks the Control
        /// </summary>
        protected virtual void Checked()
        {
            if (OnChecked != null)
                OnChecked(this);
            if (GlobalChecked != null)
                GlobalChecked(this);
        }
        /// <summary>
        /// Unchecks the Control
        /// </summary>
        protected virtual void Unchecked()
        {
            if (OnUnchecked != null)
                OnUnchecked(this);
            if (GlobalUnchecked != null)
                GlobalUnchecked(this);
        }
    }
}

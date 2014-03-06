using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.UI.Interface.Controls.Primitives;

namespace TAPI.SDK.UI.Interface.Controls
{
    /// <summary>
    /// A checkable button used in a list of options
    /// </summary>
    public class RadioButton : Checkable
    {
        static bool preventStackOverflow = false;

        /// <summary>
        /// The group name of the RadioButton
        /// </summary>
        public string GroupName = "";

        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        public RadioButton()
            : this(null, false, "RadioButton")
        {

        }
        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        /// <param name="isChecked">Wether the RadioButton is checked or not</param>
        public RadioButton(bool isChecked)
            : this(null, isChecked, "RadioButton")
        {

        }
        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        /// <param name="groupName">The group name of the RadioButton</param>
        public RadioButton(string groupName)
            : this(groupName, "RadionButton")
        {

        }
        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        /// <param name="groupName">The group name of the RadioButton</param>
        /// <param name="isChecked">Wether the RadioButton is checked or not</param>
        public RadioButton(string groupName, bool isChecked)
            : this(groupName, isChecked, "RadioButton")
        {

        }
        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        /// <param name="isChecked">Wether the RadioButton is checked or not</param>
        /// <param name="text">The text of the RadioButton</param>
        public RadioButton(bool isChecked, string text)
            : this(null, isChecked, text)
        {

        }
        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        /// <param name="groupName">The group name of the RadioButton</param>
        /// <param name="text">The text of the RadioButton</param>
        public RadioButton(string groupName, string text)
            : this(groupName, false, text)
        {

        }
        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        /// <param name="groupName">The group name of the RadioButton</param>
        /// <param name="isChecked">Wether the RadioButton is checked or not</param>
        /// <param name="text">The text of the RadioButton</param>
        public RadioButton(string groupName, bool isChecked, string text)
            : base()
        {
            GroupName = groupName;
            IsChecked = isChecked;
            Text = text;
        }

        /// <summary>
        /// Checks the checkable
        /// </summary>
        protected override void Check()
        {
            if (!Parent.IsAlive || preventStackOverflow)
                return;

            base.Check();

            if (!String.IsNullOrEmpty(GroupName))
            {
                preventStackOverflow = true;

                foreach (Control c in Parent.Target.Controls)
                    if (c is RadioButton && c != this /* we might want to prevent this */)
                    {
                        RadioButton rb = c as RadioButton;

                        if (rb.GroupName == GroupName)
                            rb.IsChecked = false;
                    }

                preventStackOverflow = false;
            }
        }
        /// <summary>
        /// Unchecks the checkable
        /// </summary>
        protected override void Uncheck()
        {
            if (preventStackOverflow)
                return;

            base.Uncheck();

            if (!String.IsNullOrEmpty(GroupName))
            {
                preventStackOverflow = true;

                IsChecked = true; // prevent that no radiobuttons in the group are checked

                preventStackOverflow = false;
            }
        }

        /// <summary>
        /// Draws the control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            Vector2 charSize = Font.MeasureString(CheckChar.ToString());

            DrawBackground(sb);

            DrawOutlinedString(sb, Font, Text, TextPosition, Colour);

            // draw box

            // top, horizontal
            sb.Draw(SdkUI.WhitePixel, BoxPosition, null, Colour, Rotation, Origin,
                new Vector2(charSize.X + 6f, 2f) * Scale, SpriteEffects, LayerDepth);
            // bottom, horizontal
            sb.Draw(SdkUI.WhitePixel, BoxPosition + new Vector2(0f, charSize.Y - 4f) * Scale, null, Colour, Rotation, Origin,
                new Vector2(charSize.X + 8f, 2f) * Scale, SpriteEffects, LayerDepth);

            // left, vertical
            sb.Draw(SdkUI.WhitePixel, BoxPosition, null, Colour, Rotation, Origin,
                new Vector2(2f, charSize.Y - 4f) * Scale, SpriteEffects, LayerDepth);
            // right, vertical
            sb.Draw(SdkUI.WhitePixel, BoxPosition + new Vector2(charSize.X + 6f, 0f) * Scale, null, Colour, Rotation, Origin,
                new Vector2(2f, charSize.Y - 4f) * Scale, SpriteEffects, LayerDepth);

            if (IsChecked)
                SdkUI.DrawOutlinedString(sb, Font, CheckChar.ToString(), BoxPosition + new Vector2(4f), Colour, null, 1f, Scale, Rotation, Origin, SpriteEffects, LayerDepth);
        }
    }
}

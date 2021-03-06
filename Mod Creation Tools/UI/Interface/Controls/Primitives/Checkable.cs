﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using PoroCYon.MCT.ObjectModel;

namespace PoroCYon.MCT.UI.Interface.Controls.Primitives
{
    /// <summary>
    /// The location of the box of the checkable
    /// </summary>
    public enum BoxLocation
    {
        /// <summary>
        /// At the left of the text
        /// </summary>
        Left,
        /// <summary>
        /// At the right of the text
        /// </summary>
        Right,
        /// <summary>
        /// Ontop of the text
        /// </summary>
        Top,
        /// <summary>
        /// Under the text
        /// </summary>
        Bottom
    }

    /// <summary>
    /// A control that can be checked and unchecked
    /// </summary>
    public abstract class Checkable : Focusable, ITextObject
    {
        /// <summary>
        /// The character displayed when the Checkable is checked
        /// </summary>
        public char DisplayChar = 'X';

        /// <summary>
        /// Wether the Checkable is checked or not
        /// </summary>
        public bool IsChecked = false;

        /// <summary>
        /// The location of the box of the Checkable
        /// </summary>
        public BoxLocation Location = BoxLocation.Left;

        /// <summary>
        /// The text of the Checkable
        /// </summary>
        public string Text
        {
            get;
            set;
        }
        /// <summary>
        /// The font of the Checkable
        /// </summary>
        public SpriteFont Font
        {
            get;
            set;
        }

        /// <summary>
        /// The hitbox of the Control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                Rectangle ret = new Rectangle((int)Position.X - 8, (int)Position.Y - 8,
                    (int)(Font.MeasureString(Text).X * Scale.X) + 16, (int)(Font.MeasureString(Text).Y * Scale.Y) + 16);

                switch (Location)
                {
                    case BoxLocation.Left:
                    case BoxLocation.Right:
                        ret.Width += (int)(16f * Scale.X) + 8;
                        break;
                    case BoxLocation.Bottom:
                    case BoxLocation.Top:
                        ret.Height += (int)(16f * Scale.Y) + 8;
                        break;
                }

                return ret;
            }
        }

        /// <summary>
        /// The position of the box of the Checkable
        /// </summary>
        public virtual Vector2 BoxPosition
        {
            get
            {
                Vector2 pos = Position;

                switch (Location)
                {
                    case BoxLocation.Left:
                        // nothing
                        break;
                    case BoxLocation.Right:
                        pos.X += Font.MeasureString(Text).X * Scale.X + 8f;
                        break;
                    case BoxLocation.Top:
                    case BoxLocation.Bottom:
                        pos.X += Hitbox.Width / 2f - 8f;

                        if (Location == BoxLocation.Bottom)
                            pos.Y += Font.MeasureString(Text).Y * Scale.Y + 8f;
                        break;
                }

                return pos;
            }
        }
        /// <summary>
        /// The position of the text of the Checkable
        /// </summary>
        public virtual Vector2 TextPosition
        {
            get
            {
                Vector2 pos = Position;

                switch (Location)
                {
                    case BoxLocation.Left:
                        pos.X += Font.MeasureString(DisplayChar.ToString()).X * Scale.X + 16f;
                        break;
                    case BoxLocation.Top:
                        pos.Y += Font.MeasureString(DisplayChar.ToString()).Y * Scale.Y + 16f;
                        break;
                    case BoxLocation.Bottom:
                    case BoxLocation.Right:
                        // nothing
                        break;
                }

                return pos;
            }
        }
        /// <summary>
        /// The position of the check box
        /// </summary>
        public virtual Rectangle BoxHitbox
        {
            get
            {
                return new Rectangle((int)BoxPosition.X, (int)BoxPosition.Y,
                    (int)(Font.MeasureString(DisplayChar.ToString()).X * Scale.X), (int)(Font.MeasureString(DisplayChar.ToString()).Y * Scale.X));
            }
        }

        /// <summary>
        /// When the Checkable is checked
        /// </summary>
        public Action<Checkable> OnChecked;
        /// <summary>
        /// When the Checkable is unchecked
        /// </summary>
        public Action<Checkable> OnUnchecked;
        /// <summary>
        /// When a Checkable is checked
        /// </summary>
        public static Action<Checkable> GlobalChecked;
        /// <summary>
        /// When a Checkable is unchecked
        /// </summary>
        public static Action<Checkable> GlobalUnchecked;

        /// <summary>
        /// Creates a new instance of the Checkable class
        /// </summary>
        public Checkable()
            : base()
        {
            StayFocused = false;
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (IsFocused && !OldIsFocused)
                if (IsChecked = !IsChecked)
                    Check();
                else
                    Uncheck();
        }

        /// <summary>
        /// Checks the checkable
        /// </summary>
        protected virtual void Check()
        {
            Main.PlaySound("Vanilla:menuTick");

            if (OnChecked != null)
                OnChecked(this);
            if (GlobalChecked != null)
                GlobalChecked(this);
        }
        /// <summary>
        /// Unchecks the checkable
        /// </summary>
        protected virtual void Uncheck()
        {
            Main.PlaySound("Vanilla:menuTick");

            if (OnUnchecked != null)
                OnUnchecked(this);
            if (GlobalUnchecked != null)
                GlobalUnchecked(this);
        }
    }
}

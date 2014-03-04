using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Geometry;
using TAPI.SDK.Input;
using TAPI.SDK.ObjectModel;

namespace TAPI.SDK.UI.MenuItems
{
    /// <summary>
    /// A MenuButton that provides basic events for initialization, updating and drawing, and properties for rotation, effects, etc.
    /// If you want to have Init called, put it in a Page object.
    /// </summary>
    public abstract class Control : MenuButton, ITextObject
    {
        /// <summary>
        /// Called when the Control is initialized
        /// </summary>
        public Action<Control> OnInit;
        /// <summary>
        /// Called when the Control is updated
        /// </summary>
        public Action<Control> OnUpdate;
        /// <summary>
        /// Called when the Control is drawn
        /// </summary>
        public Action<Control, SpriteBatch> OnDraw;
        /// <summary>
        /// Called before the Control is drawn
        /// </summary>
        public Action<Control, SpriteBatch> OnPreDraw;
        /// <summary>
        /// Called when a Control is initialized
        /// </summary>
        public static Action<Control> GlobalInit;
        /// <summary>
        /// Called when a Control is drawn
        /// </summary>
        public static Action<Control> GlobalUpdate;
        /// <summary>
        /// Called before a Control is drawn
        /// </summary>
        public static Action<Control, SpriteBatch> GlobalDraw;
        /// <summary>
        /// Called before a Control is drawn
        /// </summary>
        public static Action<Control, SpriteBatch> GlobalPreDraw;

        Vector2? origin = null, scl = null;
        SpriteFont font;

        /// <summary>
        /// The rotation of the Control
        /// </summary>
        public float Rotation = 0f;
        /// <summary>
        /// The effects of the Control
        /// </summary>
        public SpriteEffects SpriteEffects = SpriteEffects.None;
        /// <summary>
        /// The layer depth of the Control
        /// </summary>
        public float LayerDepth = 0f;

        /// <summary>
        /// The text of the ITextObject
        /// </summary>
        public virtual string Text
        {
            get
            {
                return displayText;
            }
            set
            {
                displayText = value;
            }
        }
        /// <summary>
        /// The font of the IFontObject
        /// </summary>
        public virtual SpriteFont Font
        {
            get
            {
                return font ?? (fontType == 1 ? Main.fontDeathText : Main.fontMouseText);
            }
            set
            {
                font = value;

                if (value == Main.fontDeathText)
                    fontType = 1;
                else if (value == Main.fontMouseText)
                    fontType = 0;
            }
        }

        /// <summary>
        /// The origin of the Control
        /// </summary>
        public virtual Vector2 Origin
        {
            get
            {
                return origin ?? Vector2.Zero;//(Hitbox.Size() / (Scale * 2f));
            }
            set
            {
                origin = Single.IsNaN(value.X) && Single.IsNaN(value.Y) ? null : new Vector2?(value);
            }
        }
        /// <summary>
        /// The scale of the Control
        /// </summary>
        public virtual Vector2 Scale
        {
            get
            {
                return scl ?? new Vector2(scale);
            }
            set
            {
                scl = value;
                if (value.X == value.Y)
                    scale = value.X;
                else
                    scale = value.Length();
            }
        }

        /// <summary>
        /// The hitbox of the Control
        /// </summary>
        public virtual Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, (int)(size.X * Scale.X), (int)(size.Y * Scale.Y));
            }
        }
        /// <summary>
        /// Wether the mouse hovers over the Control or not
        /// </summary>
        public new bool MouseOver
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of the Control class
        /// </summary>
        public Control()
            : base(0, "", "", "")
        {
            With(w => w.Click = () => { if (!w.leadsTo.IsEmpty()) Menu.MoveTo(w.leadsTo); }); // IMPORTANT!

            base.Update += () => Update();

            canMouseOver = false;

            colorFrame = colorMouseOver = new Color(255, 255, 255, 0);
            colorText = new Color(127, 127, 127, 0);
        }

        /// <summary>
        /// Draws the MenuButton
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the MenuButton</param>
        /// <param name="mouseOver">Wether the mouse hovers over the MenuButton or not</param>
        public override void Draw(SpriteBatch sb, bool mouseOver)
        {
            MouseOver = mouseOver;

            PreDraw(sb);

            base.Draw(sb, mouseOver);

            Draw(sb);
        }

        internal void CallInit()
        {
            Init();
        }

        /// <summary>
        /// Initializes the Control
        /// </summary>
        protected virtual void Init()
        {
            if (OnInit != null)
                OnInit(this);
            if (GlobalInit != null)
                GlobalInit(this);
        }
        /// <summary>
        /// Updates the Control
        /// </summary>
        protected new virtual void Update()
        {
            if (OnUpdate != null)
                OnUpdate(this);
            if (GlobalUpdate != null)
                GlobalUpdate(this);
        }
        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        protected virtual void Draw(SpriteBatch sb)
        {
            if (OnDraw != null)
                OnDraw(this, sb);
            if (GlobalDraw != null)
                GlobalDraw(this, sb);
        }
        /// <summary>
        /// Before the Control is drawn
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        protected virtual void PreDraw(SpriteBatch sb)
        {
            if (OnPreDraw != null)
                OnPreDraw(this, sb);
            if (GlobalPreDraw != null)
                GlobalPreDraw(this, sb);
        }

        /// <summary>
        /// Draws a blue background around the Control. Call this in PreDraw.
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the background</param>
        protected void DrawBackground(SpriteBatch sb)
        {
            DrawBackground(sb, Hitbox);
        }
        /// <summary>
        /// Draws a blue background around the <paramref name="bg"/>. Call this in PreDraw.
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the background</param>
        /// <param name="bg">The region of the background</param>
        protected void DrawBackground(SpriteBatch sb, Rectangle bg)
        {
            Drawing.DrawBlueBox(sb, bg.X, bg.Y, bg.Width, bg.Height, MouseOver ? 0.85f : 0.75f);

            //Rectangle?
            //    topLeft = new Rectangle(0, 0, 16, 16),
            //    topRight = new Rectangle(484, 0, 16, 16),
            //    bottomLeft = new Rectangle(0, 484, 16, 16),
            //    bottomRight = new Rectangle(484, 484, 16, 16);

            //int a = MouseOver ? 200 : 150;
            //Color
            //    corner = new Color(255, 255, 255, a),
            //    border = new Color(18, 18, 38, a),
            //    inner = new Color(63, 65, 151, a);

            //// corners
            //sb.Draw(Main.inventoryBackTexture, position, topLeft, corner);
            //sb.Draw(Main.inventoryBackTexture, position + Main.inventoryBackTexture.Size() - new Vector2(8f, 0f), topRight, corner, Rotation, Origin, 1f, SpriteEffects, LayerDepth);
            //sb.Draw(Main.inventoryBackTexture, position + Main.inventoryBackTexture.Size() - new Vector2(0f, 80f), bottomLeft, corner, Rotation, Origin, 1f, SpriteEffects, LayerDepth);
            //sb.Draw(Main.inventoryBackTexture, position + Main.inventoryBackTexture.Size() - new Vector2(8f), bottomRight, corner, Rotation, Origin, 1f, SpriteEffects, LayerDepth);

            //// borders
            //sb.Draw(SdkUI.WhitePixel, position + new Vector2(8f, 0f), null, border, Rotation, Origin, new Vector2(bg.Width - 16f, 2f), SpriteEffects, LayerDepth);
            //sb.Draw(SdkUI.WhitePixel, position + new Vector2(0f, 8f), null, border, Rotation, Origin, new Vector2(2f, bg.Height - 16f), SpriteEffects, LayerDepth);
            //sb.Draw(SdkUI.WhitePixel, position + new Vector2(8f, bg.Height - 2f), null, border, Rotation, Origin, new Vector2(bg.Width - 16f, 2f), SpriteEffects, LayerDepth);
            //sb.Draw(SdkUI.WhitePixel, position + new Vector2(bg.Width - 2f, 8f), null, border, Rotation, Origin, new Vector2(2f, bg.Height - 16f), SpriteEffects, LayerDepth);

            //// inner (centre)
            //sb.Draw(SdkUI.WhitePixel, position + new Vector2(8f), null, inner, Rotation, Origin, Main.inventoryBackTexture.Size() - new Vector2(16f), SpriteEffects, LayerDepth);

            //// inner (missing parts)
            //sb.Draw(SdkUI.WhitePixel, position + new Vector2(8f, 2f), null, inner, Rotation, Origin, new Vector2(bg.Width - 16f, 6f), SpriteEffects, LayerDepth);
            //sb.Draw(SdkUI.WhitePixel, position + new Vector2(8f, bg.Height - 8f), null, inner, Rotation, Origin, new Vector2(bg.Width - 16f, 6f), SpriteEffects, LayerDepth);
            //sb.Draw(SdkUI.WhitePixel, position + new Vector2(2f, 8f), null, inner, Rotation, Origin, new Vector2(6f, bg.Height - 16f), SpriteEffects, LayerDepth);
            //sb.Draw(SdkUI.WhitePixel, position + new Vector2(bg.Width - 8f, 8f), null, inner, Rotation, Origin, new Vector2(6f, bg.Height - 16f), SpriteEffects, LayerDepth);
        }
        /// <summary>
        /// Draws an outlined string with the properties of the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the text</param>
        /// <param name="font">The SpriteFont used to draw the text</param>
        /// <param name="text">The text to draw</param>
        /// <param name="foreground">The foreground colour of the text</param>
        /// <param name="background">The outline colour of the text</param>
        /// <param name="offset">The offset of the outlines</param>
        protected void DrawOutlinedString(SpriteBatch sb, SpriteFont font, string text, Color foreground, Color? background = null, float offset = 1f)
        {
            SdkUI.DrawOutlinedString(sb, font, text, position, foreground, background, offset, Scale, Rotation, Origin, SpriteEffects, LayerDepth);
        }
    }
}

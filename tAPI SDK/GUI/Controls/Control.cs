using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Geometry;
using TAPI.SDK.Input;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
    /// <summary>
    /// Enumeration has the Flags attribute
    /// </summary>
    [Flags]
    public enum Visibility : int
    {
        None = 0,

        Menu = 1,
        IngameNoInv = 2,
        IngameInv = 4,
        Ingame = IngameInv | IngameNoInv,

        All = Menu | Ingame
    }

    public abstract class Control : ModableObject, IDisposable, ICloneable<Control>
    {
        protected static readonly Vector2 padding = new Vector2(8f);

        internal bool Destroyed = false;

        public bool IsDrawnAfter = false, HasBackground = true, Enabled = true;

		/// <summary>
		/// Null is TAPI.SDK.SdkUI
		/// </summary>
        public WeakReference<IControlParent> Parent; // WeakReference so we don't create a memory leak

        public int ID = -1;

        /// <summary>
        /// Relative if the control is in a ControlContainer
        /// </summary>
        public Vector2 Position = new Vector2();
        /// <summary>
        /// Sometimes used a size (eg. Window)
        /// </summary>
        public Vector2 Scale = new Vector2(1f);
        public Color Colour = Color.White;
        /// <summary>
        /// Might not be used by every type of Control
        /// </summary>
        public Color SecondaryColour = Color.White;
        public float Rotation = 0f;
        public SpriteEffects SpriteEffects = SpriteEffects.None;
        /// <summary>
        /// Might not be used by every type of Control
        /// </summary>
        public Rectangle? Frame = null;
        public float LayerDepth = 0f;
        public string Tooltip = "";
        /// <summary>
        /// Default is Visibility.IngameInv
        /// </summary>
        public Visibility Visibility = Visibility.IngameInv;

        public Action<Control> OnInit, OnUpdate;
        public Action<Control, SpriteBatch> OnDraw;
        /// <summary>
        /// Null as second argument is TAPI.SDK.GUI.Interface
        /// </summary>
        public Action<Control, IControlParent> OnAdded;
        /// <summary>
        /// Null as second argument is TAPI.SDK.GUI.Interface
        /// </summary>
        public Action<Control, IControlParent> OnRemoved;
        public static Action<Control> GlobalInit, GlobalUpdate;
        public static Action<Control, SpriteBatch> GlobalDraw;
        /// <summary>
        /// Null as second argument is TAPI.SDK.GUI.Interface
        /// </summary>
        public static Action<Control, IControlParent> GlobalAdded;
        /// <summary>
        /// Null as second argument is TAPI.SDK.GUI.Interface
        /// </summary>
        public static Action<Control, IControlParent> GlobalRemoved;

        public virtual Vector2 Origin
        {
            get
            {
                return new Vector2(Hitbox.Width, Hitbox.Height) / 2f + padding;
            }
        }
        /// <summary>
        /// If not overridden in a deriving class, it uses the Scale fields as width/height
        /// </summary>
        public virtual Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Scale.X, (int)Scale.Y);
            }
        }
		public virtual Vector2 DrawnPosition
		{
			get
			{
				return Hitbox.Position();
			}
		}
        /// <summary>
        /// Gets wether the control is visible or not
        /// </summary>
        public virtual bool IsVisible
        {
            get
            {
                return (Visibility & SdkUI.CurrentVisibility) != 0;
            }
        }

        public virtual void Init()
        {
            if (OnInit != null)
                OnInit(this);
            if (GlobalInit != null)
                GlobalInit(this);
        }
        public virtual void Update()
        {
            if (OnUpdate != null)
                OnUpdate(this);
            if (GlobalUpdate != null)
                GlobalUpdate(this);
        }
        public virtual void Draw(SpriteBatch sb)
        {
            if (OnDraw != null)
                OnDraw(this, sb);
            if (GlobalDraw != null)
                GlobalDraw(this, sb);

            if (!String.IsNullOrEmpty(Tooltip) && GInput.Mouse.Rectangle.Intersects(Hitbox))
                Constants.mainInstance.MouseText(Tooltip);
        }

        ~Control()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);

			GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool forced) { }

        protected void DrawBackground(SpriteBatch sb)
        {
            if (!HasBackground)
                return;

			DrawBackground(sb, Hitbox);
        }
		protected void DrawBackground(SpriteBatch sb, Rectangle bg)
		{
			Rectangle?
				topLeft = new Rectangle(0, 0, 8, 8),
				topRight = new Rectangle(492, 0, 8, 8),
				bottomLeft = new Rectangle(0, 492, 8, 8),
				bottomRight = new Rectangle(492, 492, 8, 8);

			int a = GInput.Mouse.Rectangle.Intersects(bg) ? 200 : 150;
			Color
				corner = new Color(255, 255, 255, a),
				border = new Color(18, 18, 38, a),
				inner = new Color(63, 65, 151, a);

			// corners
			sb.Draw(Main.chatBackTexture, Position, topLeft, corner);
			sb.Draw(Main.chatBackTexture, Position + bg.Size() - new Vector2(8f, 0f), topRight, corner, Rotation, Origin, 1f, SpriteEffects, LayerDepth);
			sb.Draw(Main.chatBackTexture, Position + bg.Size() - new Vector2(0f, 80f), bottomLeft, corner, Rotation, Origin, 1f, SpriteEffects, LayerDepth);
			sb.Draw(Main.chatBackTexture, Position + bg.Size() - new Vector2(8f), bottomRight, corner, Rotation, Origin, 1f, SpriteEffects, LayerDepth);

			// borders
			sb.Draw(SdkUI.WhitePixel, Position + new Vector2(8f, 0f), null, border, Rotation, Origin, new Vector2(bg.Width - 16f, 2f), SpriteEffects, LayerDepth);
			sb.Draw(SdkUI.WhitePixel, Position + new Vector2(0f, 8f), null, border, Rotation, Origin, new Vector2(2f, bg.Height - 16f), SpriteEffects, LayerDepth);
			sb.Draw(SdkUI.WhitePixel, Position + new Vector2(8f, bg.Height - 2f), null, border, Rotation, Origin, new Vector2(bg.Width - 16f, 2f), SpriteEffects, LayerDepth);
			sb.Draw(SdkUI.WhitePixel, Position + new Vector2(bg.Width - 2f, 8f), null, border, Rotation, Origin, new Vector2(2f, bg.Height - 16f), SpriteEffects, LayerDepth);

			// inner (centre)
			sb.Draw(SdkUI.WhitePixel, Position + new Vector2(8f), null, inner, Rotation, Origin, bg.Size() - new Vector2(16f), SpriteEffects, LayerDepth);

			// inner (missing parts)
			sb.Draw(SdkUI.WhitePixel, Position + new Vector2(8f, 2f), null, inner, Rotation, Origin, new Vector2(bg.Width - 16f, 6f), SpriteEffects, LayerDepth);
			sb.Draw(SdkUI.WhitePixel, Position + new Vector2(8f, bg.Height - 8f), null, inner, Rotation, Origin, new Vector2(bg.Width - 16f, 6f), SpriteEffects, LayerDepth);
			sb.Draw(SdkUI.WhitePixel, Position + new Vector2(2f, 8f), null, inner, Rotation, Origin, new Vector2(6f, bg.Height - 16f), SpriteEffects, LayerDepth);
			sb.Draw(SdkUI.WhitePixel, Position + new Vector2(bg.Width - 8f, 8f), null, inner, Rotation, Origin, new Vector2(6f, bg.Height - 16f), SpriteEffects, LayerDepth);
		}
        protected void DrawOutlinedString(SpriteBatch sb, SpriteFont font, string text, Color foreground, Color? background = null, float offset = 1f)
        {
            SdkUI.DrawOutlinedString(sb, font, text, Position, foreground, background, offset, Scale, Rotation, Origin, SpriteEffects, LayerDepth);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public Control Copy()
        {
            return (Control)MemberwiseClone();
        }

        public void Destroy()
        {
            Destroyed = true;
            Enabled = false;
        }
    }
}

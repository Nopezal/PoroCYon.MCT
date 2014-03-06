using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Geometry;
using TAPI.SDK.Input;
using TAPI.SDK.ObjectModel;

namespace TAPI.SDK.UI.Interface.Controls
{
    /// <summary>
    /// A drawable and interactable interface element
    /// </summary>
    public abstract class Control : ModableObject, ICloneable<Control>
    {
        /// <summary>
        /// The parent of the control, as a weak reference.
        /// </summary>
        public WeakReference<IControlParent> Parent; // To avoid a memory leak (indirect circular reference: parent -> list -> control -> parent...)

        Vector2? origin = null;

        internal bool Destroyed = false;

        internal static bool listening = false;

        /// <summary>
        /// Wether a control is listening to the keyboard or not
        /// </summary>
        protected static bool SomethingIsListening
        {
            get
            {
                return listening;
            }
            set
            {
                listening = value;
            }
        }

        #region fields
        /// <summary>
        /// Wether the control has a background or not
        /// </summary>
        public bool HasBackground = true;
        /// <summary>
        /// Wether the control can be updated and drawn or not
        /// </summary>
        public bool Enabled = true;
        /// <summary>
        /// The absolute position of the Control
        /// </summary>
        public Vector2 Position = new Vector2();
        /// <summary>
        /// The scale of the control. Sometimes used as size (eg. Window)
        /// </summary>
        public Vector2 Scale = new Vector2(1f);
        /// <summary>
        /// The colour of the control. Default is #FFFFFF00
        /// </summary>
        public Color Colour = new Color(255, 255, 255, 255);
        /// <summary>
        /// The secondary colour of the control. Not used for all types of controls. Default is #00000000
        /// </summary>
        public Color SecondaryColour = new Color(0, 0, 0, 255);
        /// <summary>
        /// The rotation of the control
        /// </summary>
        public float Rotation = 0f;
        /// <summary>
        /// The sprite effects of the control
        /// </summary>
        public SpriteEffects SpriteEffects = SpriteEffects.None;
        /// <summary>
        /// The layer depth of the control
        /// </summary>
        public float LayerDepth = 0f;
        /// <summary>
        /// The tooltip of the control
        /// </summary>
        public string Tooltip = "";
        /// <summary>
        /// The visibility of the control
        /// </summary>
        public Visibility Visibility = Visibility.All;
        #endregion

        #region events
        /// <summary>
        /// Called when the control's parent is inited
        /// </summary>
        public Action<Control> OnInit;
        /// <summary>
        /// Called when the control is updated
        /// </summary>
        public Action<Control> OnUpdate;
        /// <summary>
        /// Called when the control is drawn
        /// </summary>
        public Action<Control, SpriteBatch> OnDraw;
        /// <summary>
        /// Called when the control is added to a list
        /// </summary>
        public Action<Control, IControlParent> OnAdded;
        /// <summary>
        /// Called when the control is removed from a list
        /// </summary>
        public Action<Control, IControlParent> OnRemoved;

        /// <summary>
        /// Called when a control's parent is inited
        /// </summary>
        public static Action<Control> GlobalInit;
        /// <summary>
        /// Called when a control is updated
        /// </summary>
        public static Action<Control> GlobalUpdate;
        /// <summary>
        /// Called when a control is drawn
        /// </summary>
        public static Action<Control, SpriteBatch> GlobalDraw;
        /// <summary>
        /// Called when a control is added to a list
        /// </summary>
        public static Action<Control, IControlParent> GlobalAdded;
        /// <summary>
        /// Called when a control is removed from a list
        /// </summary>
        public static Action<Control, IControlParent> GlobalRemoved;
        #endregion

        #region properties
        /// <summary>
        /// The index of the control in Parent.Target.Controls
        /// </summary>
        public int ID
        {
            get;
            internal set;
        }
        /// <summary>
        /// The drawing origion of the Control
        /// </summary>
        public virtual Vector2 Origin
        {
            get
            {
                return origin ?? Vector2.Zero;//new Vector2(Hitbox.Width, Hitbox.Height) / 2f;
            }
            set
            {
                origin = Single.IsNaN(value.X) && Single.IsNaN(value.Y) ? null : new Vector2?(value);
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
        /// <summary>
        /// The draw position of the Control
        /// </summary>
        public virtual Vector2 DrawnPosition
        {
            get
            {
                return Position + Origin;//Hitbox.Position();
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
        #endregion

        /// <summary>
        /// Creates a new instance of the Control class
        /// </summary>
        public Control()
        {
            ID = -1;
        }

        /// <summary>
        /// Initializes the control
        /// </summary>
        public virtual void Init()
        {
            if (OnInit != null)
                OnInit(this);
            if (GlobalInit != null)
                GlobalInit(this);
        }
        /// <summary>
        /// Updates the control
        /// </summary>
        public virtual void Update()
        {
            if (OnUpdate != null)
                OnUpdate(this);
            if (GlobalUpdate != null)
                GlobalUpdate(this);
        }
        /// <summary>
        /// Draws the control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the control</param>
        public virtual void Draw(SpriteBatch sb)
        {
            if (OnDraw != null)
                OnDraw(this, sb);
            if (GlobalDraw != null)
                GlobalDraw(this, sb);

            if (!String.IsNullOrEmpty(Tooltip) && GInput.Mouse.Rectangle.Intersects(Hitbox))
                SdkUI.MouseText(Tooltip);
        }

        /// <summary>
        /// Draws a blue background behind the control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the background</param>
        protected void DrawBackground(SpriteBatch sb)
        {
            if (!HasBackground)
                return;

            DrawBackground(sb, Hitbox);
        }
        /// <summary>
        /// Draws a blue background with the given bounds
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the background</param>
        /// <param name="bg">The bounds of the background to draw</param>
        protected void DrawBackground(SpriteBatch sb, Rectangle bg)
        {
            Drawing.DrawBlueBox(sb, bg.X, bg.Y, bg.Width, bg.Height, GInput.Mouse.Rectangle.Intersects(bg) ? 0.85f : 0.75f);
        }
        /// <summary>
        /// Draws an outlined string with the control's properties as paramters
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the string</param>
        /// <param name="font">The font of the text to draw</param>
        /// <param name="text">The text to draw</param>
        /// <param name="foreground">The foreground colour of the text to draw</param>
        /// <param name="background">The outline colour</param>
        /// <param name="offset">The offset of the outlines</param>
        protected void DrawOutlinedString(SpriteBatch sb, SpriteFont font, string text, Color foreground, Color? background = null, float offset = 2f)
        {
            SdkUI.DrawOutlinedString(sb, font, text, Position, foreground, background ?? SecondaryColour, offset, Scale, Rotation, Origin, SpriteEffects, LayerDepth);
        }

        /// <summary>
        /// Creates a memberwise clone of the object
        /// </summary>
        /// <returns>A memberwise clone of the object</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
        /// <summary>
        /// Creates a memberwise clone of the object
        /// </summary>
        /// <returns>A memberwise clone of the object</returns>
        public Control Copy()
        {
            return (Control)MemberwiseClone();
        }

        /// <summary>
        /// Destroys the control by removing it safely from it's parent's list.
        /// </summary>
        public void Destroy()
        {
            Destroyed = true;
            Enabled = false;
        }
    }
}

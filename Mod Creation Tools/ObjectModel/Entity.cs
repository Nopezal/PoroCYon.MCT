using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI;

namespace PoroCYon.MCT.ObjectModel
{
    /// <summary>
    /// A base for an entity. Provides position, velocity, and drawing properties, as well as initializing/updating/drawing events.
    /// </summary>
    public abstract class Entity : ModableObject, IDisposable
    {
        Vector2? origin = null;

        /// <summary>
        /// The position of the Entity
        /// </summary>
        public Vector2 Position = Vector2.Zero;
        /// <summary>
        /// The velocity of the Entity
        /// </summary>
        public Vector2 Velocity = Vector2.Zero;
        /// <summary>
        /// The scale of the Entity
        /// </summary>
        public Vector2 Scale = new Vector2(1f);
        /// <summary>
        /// The rotation of the Entity
        /// </summary>
        public float Rotation = 0f;
        /// <summary>
        /// The layer depth of the Entity
        /// </summary>
        public float LayerDepth = 0f;
        /// <summary>
        /// The sprite effects of the Entity
        /// </summary>
        public SpriteEffects SpriteEffects = SpriteEffects.None;
        /// <summary>
        /// The texture of the Entity
        /// </summary>
        public Texture2D Texture;
        /// <summary>
        /// The colour of the Entity
        /// </summary>
        public Color Colour = new Color(255, 255, 255, 0);

        /// <summary>
        /// Called when the Entity is initialized
        /// </summary>
        public Action<Entity> OnInit;
        /// <summary>
        /// Called when the Entity is updated
        /// </summary>
        public Action<Entity> OnUpdate;
        /// <summary>
        /// Called when the Entity is drawn
        /// </summary>
        public Action<Entity, SpriteBatch> OnDraw;
        /// <summary>
        /// Called when an Entity is initialized
        /// </summary>
        public static Action<Entity> GlobalInit;
        /// <summary>
        /// Called when an Entity is updated
        /// </summary>
        public static Action<Entity> GlobalUpdate;
        /// <summary>
        /// Called when an Entity is drawn
        /// </summary>
        public static Action<Entity, SpriteBatch> GlobalDraw;

        /// <summary>
        /// Finalizes the Entity
        /// </summary>
        ~Entity()
        {
            Dispose(false);
        }
        /// <summary>
        /// Disposes the Entity
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Disposes the Entity
        /// </summary>
        /// <param name="forced">Wether Dispose is called from IDisposable.Dispose or not</param>
        protected virtual void Dispose(bool forced)
        {

        }

        /// <summary>
        /// The hitbox of the Entity
        /// </summary>
        public virtual Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * Scale.X), (int)(Texture.Height * Scale.Y));
            }
        }
        /// <summary>
        /// The drawing origin of the Entity
        /// </summary>
        public virtual Vector2 Origin
        {
            get
            {
                return origin ?? new Vector2(Texture.Width, Texture.Height) / 2f;
            }
            set
            {
                origin = Single.IsNaN(value.X) && Single.IsNaN(value.Y) ? null : new Vector2?(value);
            }
        }
        /// <summary>
        /// The screen position of the Entity
        /// </summary>
        public virtual Vector2 ScreenPositon
        {
            get
            {
                return Position - Main.screenPosition;
            }
            set
            {
                Position = value + Main.screenPosition;
            }
        }
        /// <summary>
        /// The screen hitbox of the Entity
        /// </summary>
        public virtual Rectangle ScreenHitbox
        {
            get
            {
                return new Rectangle((int)Position.X - (int)Main.screenPosition.X, (int)Position.Y -
                    (int)Main.screenPosition.Y, (int)(Texture.Width * Scale.X), (int)(Texture.Height * Scale.Y));
            }
        }

        /// <summary>
        /// Initializes the Entity
        /// </summary>
        public virtual void Init()
        {
            if (OnInit != null)
                OnInit(this);
            if (GlobalInit != null)
                GlobalInit(this);
        }
        /// <summary>
        /// Updates the Entity
        /// </summary>
        public virtual void Update()
        {
            if (OnUpdate != null)
                OnUpdate(this);
            if (GlobalUpdate != null)
                GlobalUpdate(this);
        }
        /// <summary>
        /// Draws the Entity
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Entity</param>
        public virtual void Draw(SpriteBatch sb)
        {
            if (OnDraw != null)
                OnDraw(this, sb);
            if (GlobalDraw != null)
                GlobalDraw(this, sb);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.GUI.Controls.Primitives;
using TAPI.SDK.Input;

namespace TAPI.SDK.GUI.Controls
{
    /// <summary>
    /// An <see cref="TAPI.Item"/> slot
    /// </summary>
    public class ItemContainer : Button
    {
        Texture2D tex = Main.inventoryBackTexture;
        int invBackNum = 1;

        public int InventoryBackTextureNum
        {
            get
            {
                return invBackNum;
            }
            set
            {
                if (value < 1 || value > 12)
                    throw new ArgumentOutOfRangeException("value");

                switch (invBackNum = value)
                {
                    case 1:
                        tex = Main.inventoryBackTexture;
                        break;
                    case 2:
                        tex = Main.inventoryBack2Texture;
                        break;
                    case 3:
                        tex = Main.inventoryBack3Texture;
                        break;
                    case 4:
                        tex = Main.inventoryBack4Texture;
                        break;
                    case 5:
                        tex = Main.inventoryBack5Texture;
                        break;
                    case 6:
                        tex = Main.inventoryBack6Texture;
                        break;
                    case 7:
                        tex = Main.inventoryBack7Texture;
                        break;
                    case 8:
                        tex = Main.inventoryBack8Texture;
                        break;
                    case 9:
                        tex = Main.inventoryBack9Texture;
                        break;
                    case 10:
                        tex = Main.inventoryBack10Texture;
                        break;
                    case 11:
                        tex = Main.inventoryBack11Texture;
                        break;
                    case 12:
                        tex = Main.inventoryBack12Texture;
                        break;
                }
            }
        }

        /// <summary>
        /// The Item the ItemContainer contains
        /// </summary>
        public Item ContainedItem
        {
            get;
            protected set;
        }

        /// <summary>
        /// When ContainedItem is changed
        /// </summary>
        public Action<ItemContainer, Item, Item> OnItemChanged;
        /// <summary>
        /// When ContainedItem is changed
        /// </summary>
        public static Action<ItemContainer, Item, Item> GlobalItemChanged;

        /// <summary>
        /// When trying to change ContainedItem
        /// </summary>
        public Func<ItemContainer, Item, bool> OnTrySetItem;
        /// <summary>
        /// When trying to change ContainedItem
        /// </summary>
        public static Func<ItemContainer, Item, bool> GlobalTrySetItem;

        /// <summary>
        /// Creates a new instance of the ItemContainer class
        /// </summary>
        public ItemContainer()
            : this(new Item())
        {

        }
        /// <summary>
        /// Creates a new instance of the ItemContainer class
        /// </summary>
        /// <param name="i">Sets the ContainedItem field</param>
        public ItemContainer(Item i)
            : base()
        {
            ContainedItem = (Item)i.Clone();
        }

        /// <summary>
        /// The hitbox of the Control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y,
                    (int)(tex.Width * Scale.X), (int)(tex.Height * Scale.Y));
            }
        }

        /// <summary>
        /// When the Button is clicked
        /// </summary>
        protected override void Clicked()
        {
            base.Clicked();

            Item
                tempMouse = (Item)Main.mouseItem.Clone(),
                oldContained = (Item)ContainedItem.Clone();

            if (TrySetItem(tempMouse))
            {
                bool run = true;

                if (OnTrySetItem != null)
                    run &= OnTrySetItem(this, tempMouse);
                if (GlobalTrySetItem != null)
                    run &= GlobalTrySetItem(this, tempMouse);

                if (!run)
                    return;
            }

            Main.mouseItem = (Item)ContainedItem.Clone();
            ContainedItem = (Item)tempMouse.Clone();

            ItemChanged(oldContained, ContainedItem);

            if (OnItemChanged != null)
                OnItemChanged(this, oldContained, ContainedItem);
            if (GlobalItemChanged != null)
                GlobalItemChanged(this, oldContained, ContainedItem);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            sb.Draw(tex, Position, null, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
            //DrawBackground(sb);

            if (ContainedItem.IsBlank())
                return;

            Vector2 scl = Scale;

            Texture2D tex = ContainedItem.GetTexture();

            if (tex.Width > Hitbox.Width || tex.Height > Hitbox.Height)
                scl *= new Vector2(tex.Width > tex.Height ? Hitbox.Width / tex.Width : Hitbox.Height / tex.Height);

            Color c = ContainedItem.GetTextureColor();

            sb.Draw(tex, new Vector2(Position.X + 26f * Main.inventoryScale - tex.Width * 0.5f * scl.X,
                Position.Y + 26f * Main.inventoryScale - tex.Height * 0.5f * scl.Y),
                null, ContainedItem.GetAlpha(c), 0f, default(Vector2), scl, SpriteEffects.None, 0f);

            c = Color.White;

            if (ContainedItem.color != default(Color))
                sb.Draw(tex, new Vector2(Position.X + 26f * Main.inventoryScale - tex.Width * 0.5f * scl.X,
                    Position.Y + 26f * Main.inventoryScale - tex.Height * 0.5f * scl.Y),
                    null, ContainedItem.GetColor(Color.White), 0f, default(Vector2), scl, SpriteEffects.None, 0f);

            if (ContainedItem.stack > 1)
                sb.DrawString(Main.fontItemStack, ContainedItem.stack.ToString(), new Vector2(Position.X + 10f * Main.inventoryScale,
                    Position.Y + 26f * Main.inventoryScale), Color.White, 0f, default(Vector2), scl, SpriteEffects.None, 0f);

            if (GInput.Mouse.Rectangle.Intersects(Hitbox) && ContainedItem != null)
            {
                Main.toolTip = (Item)ContainedItem.Clone();
                Constants.mainInstance.MouseText(ContainedItem.AffixName() + (ContainedItem.stack > 1 ? " (" + ContainedItem.stack + ")" : ""), ContainedItem.rare);
                Main.toolTip = new Item();
            }
        }

        /// <summary>
        /// When ContainedItem is changed
        /// </summary>
        /// <param name="old">The old ContainedItem</param>
        /// <param name="new">The new ContainedItem</param>
        protected virtual void ItemChanged(Item old, Item @new) { }
        /// <summary>
        /// When trying to change ContainedItem
        /// </summary>
        /// <param name="trying">The Item that might be the new ContainedItem</param>
        /// <returns>true if ContainedItem can be changed, false otherwise.</returns>
        protected virtual bool TrySetItem(Item trying) { return true; }
    }
}

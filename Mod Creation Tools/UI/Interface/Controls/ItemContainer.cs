using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using Terraria;
using TAPI;
using PoroCYon.MCT.UI.Interface.Controls.Primitives;

namespace PoroCYon.MCT.UI.Interface.Controls
{
    using ItemSlot = TAPI.Interface.ItemSlot;

    /// <summary>
    /// An Item slot
    /// </summary>
    public class ItemContainer : Focusable
    {
        class ItemSlotWrapper : ItemSlot
        {
            WeakReference<ItemContainer> parent;

            internal ItemSlotWrapper(ItemContainer container, ModBase @base, string type, int index)
                : base(@base, type, index, (s, it) => { container.ContainedItem = it; }, (s) => { return container.ContainedItem; })
            {
                parent = new WeakReference<ItemContainer>(container);
            }

            public override bool AllowsItem(Item item)
            {
                if (!parent.IsAlive)
                    return base.AllowsItem(item);

                return parent.Target.CanSetItem(item);
            }
            public override void DrawItemSlotBackground(SpriteBatch sb)
            {
                if (!parent.IsAlive)
                {
                    base.DrawItemSlotBackground(sb);
                    return;
                }

                if (TAPI.Hooks.Interface.PreDrawItemSlotBackground(sb, this))
                    sb.Draw(parent.Target.bgTex, pos, null, Main.inventoryBack, 0f, default(Vector2), Main.inventoryScale, SpriteEffects.None, 0f);
                TAPI.Hooks.Interface.PostDrawItemSlotBackground(sb, this);
            }
        }

        Texture2D bgTex = Main.inventoryBackTexture;
        int invBackNum = 1;
        Item item = new Item();
        ItemSlotWrapper slot;

        /// <summary>
        /// Gets or sets [n] where [n] is Main.inventoryBack[n]Texture
        /// </summary>
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
                        bgTex = Main.inventoryBackTexture;
                        break;
                    case 2:
                        bgTex = Main.inventoryBack2Texture;
                        break;
                    case 3:
                        bgTex = Main.inventoryBack3Texture;
                        break;
                    case 4:
                        bgTex = Main.inventoryBack4Texture;
                        break;
                    case 5:
                        bgTex = Main.inventoryBack5Texture;
                        break;
                    case 6:
                        bgTex = Main.inventoryBack6Texture;
                        break;
                    case 7:
                        bgTex = Main.inventoryBack7Texture;
                        break;
                    case 8:
                        bgTex = Main.inventoryBack8Texture;
                        break;
                    case 9:
                        bgTex = Main.inventoryBack9Texture;
                        break;
                    case 10:
                        bgTex = Main.inventoryBack10Texture;
                        break;
                    case 11:
                        bgTex = Main.inventoryBack11Texture;
                        break;
                    case 12:
                        bgTex = Main.inventoryBack12Texture;
                        break;
                }
            }
        }

        /// <summary>
        /// The Item the ItemContainer contains
        /// </summary>
        public Item ContainedItem
        {
            get
            {
                return item;
            }
            set
            {
                ItemChanged(item, value);
                item = value;
            }
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
        /// When ContainedItem.stack is changed
        /// </summary>
        public Action<ItemContainer, int, int> OnStackChanged;
        /// <summary>
        /// When ContainedItem.stack is changed
        /// </summary>
        public static Action<ItemContainer, int, int> GlobalStackChanged;

        /// <summary>
        /// When trying to change ContainedItem
        /// </summary>
        public Func<ItemContainer, Item, bool> OnCanSetItem;
        /// <summary>
        /// When trying to change ContainedItem
        /// </summary>
        public static Func<ItemContainer, Item, bool> GlobalCanSetItem;

        /// <summary>
        /// The hitbox of the Control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y,
                    (int)(bgTex.Width * Scale.X * Main.inventoryScale), (int)(bgTex.Height * Scale.Y * Main.inventoryScale));
            }
        }

        /// <summary>
        /// Creates a new instance of the ItemContainer class
        /// </summary>
        public ItemContainer(ModBase @base, int index = 0)
            : this(new Item(), @base)
        {

        }
        /// <summary>
        /// Creates a new instance of the ItemContainer class
        /// </summary>
        /// <param name="i">Sets the ContainedItem field</param>
        /// <param name="base">The calling mod's ModBase</param>
        /// <param name="index">The index of the Item slot</param>
        /// <param name="type">The type of the ItemContainer, used to identify it.</param>
        public ItemContainer(Item i, ModBase @base, int index = 0, string type = null)
            : base()
        {
            item = i;

            slot = new ItemSlotWrapper(this, @base, type ?? "ItemContainerImpl", index);
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            int oldStack = item.stack;

            slot.Update(Position);

            if (item.stack != oldStack)
                StackChanged(oldStack, item.stack);
        }
        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (HasBackground)
                DrawBackground(sb);

            slot.Draw(sb);
        }

        /// <summary>
        /// When ContainedItem is changed
        /// </summary>
        /// <param name="old">The old ContainedItem</param>
        /// <param name="new">The new ContainedItem</param>
        protected virtual void ItemChanged(Item old, Item @new)
        {
            if (OnItemChanged != null)
                OnItemChanged(this, old, @new);
            if (GlobalItemChanged != null)
                GlobalItemChanged(this, old, @new);
        }
        /// <summary>
        /// When ContainedItem.stack is changed
        /// </summary>
        /// <param name="old">The old stack</param>
        /// <param name="new">The new stack</param>
        protected virtual void StackChanged(int old, int @new)
        {
            if (OnStackChanged != null)
                OnStackChanged(this, old, @new);
            if (GlobalStackChanged != null)
                GlobalStackChanged(this, old, @new);
        }
        /// <summary>
        /// When trying to change ContainedItem
        /// </summary>
        /// <param name="trying">The Item that might be the new ContainedItem</param>
        /// <returns>true if ContainedItem can be changed, false otherwise.</returns>
        protected virtual bool CanSetItem(Item trying)
        {
            bool ret = true;

            if (OnCanSetItem != null)
                ret &= OnCanSetItem(this, trying);
            if (GlobalCanSetItem != null)
                ret &= GlobalCanSetItem(this, trying);

            return ret;
        }
    }
}

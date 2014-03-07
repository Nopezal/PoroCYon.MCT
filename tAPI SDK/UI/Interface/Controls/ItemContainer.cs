using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TAPI.SDK.Input;
using TAPI.SDK.UI.Interface.Controls.Primitives;

namespace TAPI.SDK.UI.Interface.Controls
{
    /// <summary>
    /// An Item slot
    /// </summary>
    public class ItemContainer : Focusable
    {
        Texture2D bgTex = Main.inventoryBackTexture;
        int invBackNum = 1;
        Item item = new Item();

        /// <summary>
        /// Wether the ItemContainer should show the Item's stack number or not
        /// </summary>
        public bool ShowStackNumber = true;
        /// <summary>
        /// Wether you can use shift+click to sell the Item or not
        /// </summary>
        public bool IsSellable = false;
        /// <summary>
        /// Wether you can use shift+click to trash the Item or not
        /// </summary>
        public bool IsTrashable = false;
        /// <summary>
        /// Wether you can use the item slot as trash
        /// </summary>
        public bool IsTrash = false;

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
                ItemChanged(ContainedItem, value);
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
            item = (Item)i.Clone();
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (IsHovered)
            {
                if (IsFocused)
                {
                    if (GInput.Keyboard.IsKeyDown(Keys.LeftShift))
                    {
                        if (Main.npcShop > 0 && IsSellable)
                        {
                            if (Main.localPlayer.SellItem(ContainedItem.value, ContainedItem.stack))
                            {
                                Item old = (Item)ContainedItem.Clone();

                                Constants.mainInstance.shop[Main.npcShop].AddShop(ContainedItem);
                                ContainedItem.SetDefaults(0);

                                ItemChanged(old, ContainedItem);

                                Main.PlaySound(18, -1, -1, 1);
                            }
                            else if (ContainedItem.value == 0)
                            {
                                Item old = (Item)ContainedItem.Clone();

                                Constants.mainInstance.shop[Main.npcShop].AddShop(ContainedItem);
                                ContainedItem.SetDefaults(0);

                                ItemChanged(old, ContainedItem);

                                Main.PlaySound(7, -1, -1, 1);
                            }
                        }
                        else if (IsTrashable && !ContainedItem.IsBlank())
                        {
                            Item old = (Item)ContainedItem.Clone();

                            Main.trashItem = (Item)ContainedItem.Clone();
                            ContainedItem.SetDefaults(0);

                            ItemChanged(old, ContainedItem);

                            Main.PlaySound(7, -1, -1, 1);
                        }
                    }
                    else if (Main.localPlayer.itemAnimation <= 0 && Main.localPlayer.itemTime == 0)
                    {
                        if (IsTrash && !Main.mouseItem.IsBlank())
                        {
                            Item old = (Item)ContainedItem.Clone();

                            ContainedItem.SetDefaults(0);

                            ItemChanged(old, ContainedItem);
                        }

                        if (CanSetItem(Main.mouseItem))
                        {
                            Item temp = Main.mouseItem;
                            Main.mouseItem = ContainedItem;

                            ContainedItem = temp;

                            if (ContainedItem.IsBlank())
                                item = new Item();

                            if (Main.mouseItem.IsTheSameAs(ContainedItem) && ContainedItem.stack != ContainedItem.maxStack
                                && Main.mouseItem.stack != Main.mouseItem.maxStack)
                            {
                                int diff = Main.mouseItem.maxStack - ContainedItem.stack;
                                ContainedItem.stack += diff;
                                Main.mouseItem.stack -= diff;

                                if (Main.mouseItem.stack <= 0)
                                    Main.mouseItem.SetDefaults(0);

                                StackChanged(ContainedItem.stack - diff, ContainedItem.stack);
                            }

                            if (Main.mouseItem.IsBlank())
                                Main.mouseItem = new Item();

                            if (!Main.mouseItem.IsBlank() || !ContainedItem.IsBlank())
                                Main.PlaySound(7);
                        }
                    }
                }
                if (GInput.Mouse.Right && !IsTrash)
                    if (!GInput.OldMouse.Right)
                    {
                        if (ContainedItem.type == 1774)
                        {
                            ContainedItem.stack--;

                            if (ContainedItem.stack == 0)
                                ContainedItem.SetDefaults(0);

                            StackChanged(ContainedItem.stack + 1, ContainedItem.stack);

                            Main.PlaySound(7, -1, -1, 1);
                            Main.stackSplit = 30;
                            Main.localPlayer.openGoodieBag();
                        }
                        else if (ContainedItem.type == 1869)
                        {
                            ContainedItem.stack--;

                            if (ContainedItem.stack == 0)
                                ContainedItem.SetDefaults(0);

                            StackChanged(ContainedItem.stack + 1, ContainedItem.stack);

                            Main.PlaySound(7, -1, -1, 1);
                            Main.stackSplit = 30;
                            Main.localPlayer.openPresent();
                        }
                        else if (ContainedItem.type >= 599 && ContainedItem.type <= 601)
                        {
                            Main.PlaySound(7, -1, -1, 1);
                            Main.stackSplit = 30;

                            WeightedRandom<Tuple<int, int, int>> rand = new WeightedRandom<Tuple<int, int, int>>(Main.rand);

                            if (Main.hardMode)
                                rand.Add(new Tuple<int, int, int>(602, 1, 1), 1);
                            rand.Add(new Tuple<int, int, int>(586, 20, 50), 7);
                            rand.Add(new Tuple<int, int, int>(591, 20, 50), 7);

                            Tuple<int, int, int> result = rand.Get();

                            Item old = (Item)item.Clone();

                            ContainedItem.SetDefaults(result.Item1);

                            if (result.Item2 < result.Item3)
                                ContainedItem.stack = Main.rand.Next(result.Item2, result.Item3);

                            ItemChanged(old, ContainedItem);
                        }
                        else if (ContainedItem.maxStack == 1)
                        {
                            Item old = (Item)item.Clone();

                            ContainedItem = ContainedItem.dye > 0 ? Main.dyeSwap(ContainedItem) : Main.armorSwap(ContainedItem);

                            ItemChanged(old, ContainedItem);
                        }

                        Recipe.FindRecipes();
                    }
                    else if (Main.stackSplit <= 1 && !ContainedItem.IsBlank() && ContainedItem.maxStack > 1
                        && (Main.mouseItem.IsTheSameAs(ContainedItem) || Main.mouseItem.IsBlank())
                        && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.IsBlank()))
                    {
                        if (Main.mouseItem.IsBlank())
                        {
                            Main.mouseItem = (Item)ContainedItem.Clone();
                            Main.mouseItem.stack = 0;
                        }

                        Main.mouseItem.stack++;
                        ContainedItem.stack--;

                        if (ContainedItem.stack == 0)
                            ContainedItem = new Item();

                        StackChanged(ContainedItem.stack + 1, ContainedItem.stack);

                        Main.PlaySound(12, -1, -1, 1);
                        Main.stackSplit = Main.stackSplit == 0 ? 15 : Main.stackDelay;
                    }
            }
        }
        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            sb.Draw(bgTex, Position, null, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

            if (ContainedItem.IsBlank())
                return;

            Vector2 itemScl = Scale;

            Texture2D itemTex = ContainedItem.GetTexture();

            if (itemTex.Width > 32 || itemTex.Height > 32)
                itemScl *= itemTex.Width > itemTex.Height ? 32f / itemTex.Width : 32f / itemTex.Height;

            itemScl *= Main.inventoryScale;

            Color itemClr = ContainedItem.GetTextureColor();

            sb.Draw(itemTex, Position + new Vector2(26f) * Main.inventoryScale - itemTex.Size() / 2f * itemScl,
                null, ContainedItem.GetAlpha(itemClr), Rotation, Origin, itemScl, SpriteEffects, LayerDepth);

            if (ContainedItem.color != default(Color))
                sb.Draw(itemTex, Position + new Vector2(26f) * Main.inventoryScale - itemTex.Size() / 2f * itemScl,
                    null, ContainedItem.GetColor(Color.White), Rotation, Origin, itemScl, SpriteEffects, LayerDepth);

            if (ContainedItem.stack > 1 && ShowStackNumber)
                sb.DrawString(Main.fontItemStack, ContainedItem.stack.ToString(),
                    new Vector2(Position.X + 10f * Main.inventoryScale, Position.Y + 26f * Main.inventoryScale),
                    Color.White, 0f, default(Vector2), itemScl, SpriteEffects.None, 0f);

            if (GInput.Mouse.Rectangle.Intersects(Hitbox))
                SdkUI.MouseText(ContainedItem);
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

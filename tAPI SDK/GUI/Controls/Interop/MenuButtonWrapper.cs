using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.Input;

namespace TAPI.SDK.GUI.Controls.Interop
{
	/// <summary>
	/// A MenuButton as a Control
	/// This class cannot be inherited.
	/// </summary>
	public sealed class MenuButtonWrapper : Control
	{
		public MenuButton MenuButton;

		static int ButtonLock
		{
			get
			{
				return (int)Assembly.GetEntryAssembly().GetType("TAPI.Menu").GetField("buttonLock", BindingFlags.Static | BindingFlags.Public).GetValue(null);
			}
			set
			{
				Assembly.GetEntryAssembly().GetType("TAPI.Menu").GetField("buttonLock", BindingFlags.Static | BindingFlags.Public).SetValue(null, value);
			}
		}

		public MenuButtonWrapper()
			: this(null)
		{

		}
		public MenuButtonWrapper(MenuButton menuButton)
		{
			MenuButton = menuButton;
		}

		public override void Draw(SpriteBatch sb)
		{
			if (MenuButton == null)
				return;

			int buttonLock = ButtonLock; // we only want to get this once

			bool foundHighlight = false;

			Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY);
			Update();

			int count = 0;

			MenuButton.whoAmi = count++;
			MenuButton.Update();

			if (foundHighlight)
				MenuButton.Draw(sb, false);
			else
			{
				if (MenuButton.canMouseOver && !MenuButton.disabled && (buttonLock == -1 || buttonLock == MenuButton.whoAmi))
					foundHighlight = MenuButton.MouseOver(mouse);

				if (MenuButton.whoAmi == buttonLock)
					foundHighlight = true;

				MenuButton.Draw(sb, foundHighlight);

				if (foundHighlight && Main.mouseLeft)
				{
					if (MenuButton.buttonLock)
						ButtonLock = MenuButton.whoAmi;

					if (Main.mouseLeftRelease)
					{
						MenuButton.framesHeld = 0;
						MenuButton.Click();
					}
					else
					{
						MenuButton.framesHeld++;
						MenuButton.ClickHold();
					}
				}
			}

			base.Draw(sb);
		}

		protected override void Dispose(bool forced)
		{
			if (MenuButton is ControlWrapper)
				((ControlWrapper)MenuButton).Dispose();
		}

		public static explicit operator MenuButton(MenuButtonWrapper wrapper)
		{
			return wrapper.MenuButton;
		}
		public static explicit operator MenuButtonWrapper(MenuButton button)
		{
			return new MenuButtonWrapper(button);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls.Interop
{
	/// <summary>
	/// A MenuPage as a Control
	/// This class cannot be inherited.
	/// </summary>
	public sealed class MenuPageWrapper : Control
	{
		public MenuPage MenuPage;

		public MenuPageWrapper()
			: this(null)
		{

		}
		public MenuPageWrapper(MenuPage page)
		{
			MenuPage = page;
		}

		public override void Draw(SpriteBatch sb)
		{
			if (MenuPage != null)
				MenuPage.Draw(sb);

			base.Draw(sb);
		}

		protected override void Dispose(bool forced)
		{
			if (MenuPage is ControlContainerWrapper)
				((ControlContainerWrapper)MenuPage).ControlContainer.Dispose();

			base.Dispose(forced);
		}

		public static explicit operator MenuPage(MenuPageWrapper wrapper)
		{
			return wrapper.MenuPage;
		}
		public static explicit operator MenuPageWrapper(MenuPage page)
		{
			return new MenuPageWrapper(page);
		}
	}
}

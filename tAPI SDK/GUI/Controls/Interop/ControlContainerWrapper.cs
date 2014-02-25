using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls.Interop
{
	/// <summary>
	/// A ContorlContainer as a MenuPage
	/// This class cannot be inherited
	/// </summary>
    [ComVisible(false)]
	public sealed class ControlContainerWrapper : MenuPage, IDisposable, ICloneable<ControlContainerWrapper>
	{
		bool inited = false;

		public ControlContainer ControlContainer;

		public ControlContainerWrapper()
			: this((ControlContainer)null)
		{
			// 'null' will give an ambiguous match between the other contructors
		}
		public ControlContainerWrapper(params Control[] controls)
			: this(new ControlGroup(controls))
		{

		}
		public ControlContainerWrapper(ControlContainer container)
			: base()
		{
			Update += () =>
			{
				if (ControlContainer == null)
					return;

				if (!inited)
				{
					ControlContainer.Init();

					inited = true;
				}

				ControlContainer.Update();
			};

			ControlContainer = container;
		}

		public override void Draw(SpriteBatch sb)
		{
			if (ControlContainer != null)
				ControlContainer.Draw(sb);

			base.Draw(sb);
		}

		~ControlContainerWrapper()
		{
			if (ControlContainer != null)
				ControlContainer.Dispose();
		}
		public void Dispose()
		{
			if (ControlContainer != null)
				ControlContainer.Dispose();

			GC.SuppressFinalize(this);
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
		public ControlContainerWrapper Copy()
		{
			return (ControlContainerWrapper)MemberwiseClone();
		}

		public static explicit operator ControlContainer(ControlContainerWrapper wrapper)
		{
			return wrapper.ControlContainer;
		}
		public static explicit operator ControlContainerWrapper(ControlContainer container)
		{
			return new ControlContainerWrapper(container);
		}
		public static explicit operator ControlContainerWrapper(Control[] controls)
		{
			return new ControlContainerWrapper(controls);
		}
	}
}

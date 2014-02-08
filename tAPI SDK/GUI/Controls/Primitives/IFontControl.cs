using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAPI.SDK.GUI.Controls.Primitives
{
    public interface IFontControl
    {
        SpriteFont Font
        {
            get;
            set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAPI.SDK.ObjectModel
{
    /// <summary>
    /// An object that holds a font
    /// </summary>
    public interface IFontObject
    {
        /// <summary>
        /// The font of the IFontObject
        /// </summary>
        SpriteFont Font
        {
            get;
            set;
        }
    }
}

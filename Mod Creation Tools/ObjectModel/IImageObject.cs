using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Graphics;

namespace PoroCYon.MCT.ObjectModel
{
    /// <summary>
    /// An object that holds an image
    /// </summary>
    public interface IImageObject
    {
        /// <summary>
        /// The image of the IImageObject
        /// </summary>
        Union<Texture2D, AnimatedGif> Picture
        {
            get;
            set;
        }
    }
}

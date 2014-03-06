using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Graphics;

namespace TAPI.SDK.ObjectModel
{
    /// <summary>
    /// An object that holds an image
    /// </summary>
    public interface IImageObject
    {
        /// <summary>
        /// Wether the image is a gif or not
        /// </summary>
        bool IsGif
        {
            get;
        }

        /// <summary>
        /// The image of the IImageObject
        /// </summary>
        object Picture
        {
            get;
            set;
        }
    }
}

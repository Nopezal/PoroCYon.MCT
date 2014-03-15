using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Graphics;

namespace PoroCYon.MCT.UI
{
    /// <summary>
    /// Provides default parameters for extension methods defined in <see cref="PoroCYon.XnaExtensions.Graphics.Extensions"/>
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Crops a Texture2D
        /// </summary>
        /// <param name="toCrop">The Texture2D to crop</param>
        /// <param name="newSize">The region of the cropped Texture2D</param>
        /// <returns>A cropped version of <paramref name="toCrop"/></returns>
        public static Texture2D Crop(this Texture2D toCrop, Rectangle newSize)
        {
            return toCrop.Crop(newSize, MctUI.SharedGraphicsDevice);
        }
        /// <summary>
        /// Creates a Texture2D from a GraphicsDevice
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice used to create a Texture2D</param>
        /// <param name="width">The width of the Texture2D</param>
        /// <param name="height">The height of the Texture2D</param>
        /// <returns>A new Texture2D instance</returns>
        public static Texture2D CreateTexture(this GraphicsDevice graphicsDevice, int width, int height)
        {
            return new Texture2D(graphicsDevice, width, height);
        }
        /// <summary>
        /// Creates a Texture2D from a two-dimentional Color array
        /// </summary>
        /// <param name="data">The texels of the Texture2D to create</param>
        /// <returns>A Texture2D from the two-dimentional Color array</returns>
        public static Texture2D CreateTexture(this Color[,] data)
        {
            return data.CreateTexture(MctUI.SharedGraphicsDevice);
        }
        /// <summary>
        /// Creates a Texture2D from a one-dimentional Color array
        /// </summary>
        /// <param name="data">The texels of the Texture2D to create</param>
        /// <param name="width">The width of the Texture2D to create</param>
        /// <param name="height">The height of the Texture2D to create</param>
        /// <returns>A Texture2D from the one-dimentional Color array</returns>
        public static Texture2D CreateTexture(this Color[] data, int width, int height)
        {
            return data.CreateTexture(width, height, MctUI.SharedGraphicsDevice);
        }
    }
}

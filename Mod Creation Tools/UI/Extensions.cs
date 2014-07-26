using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.Extensions;
using PoroCYon.Extensions.Xna.Graphics;

namespace PoroCYon.MCT.UI
{
    /// <summary>
    /// Provides default parameters for extension methods defined in <see cref="PoroCYon.Extensions.Xna.Graphics.Extensions"/>
    /// </summary>
    public static class Extensions
    {
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

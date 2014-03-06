using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TAPI.SDK.Input
{
    /// <summary>
    /// Custom structure to make calculations with the Keyboard easier
    /// </summary>
    public struct KeyHandler
    {
        /// <summary>
        /// All keys of the keyboard which are currently pressed
        /// </summary>
        public Keys[] Keys
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets wether a key is pressed
        /// </summary>
        /// <param name="k">The key to check</param>
        /// <returns>true if the key is pressed, false otherwise.</returns>
        public bool this[Keys k]
        {
            get
            {
                return IsKeyDown(k);
            }
        }

        /// <summary>
        /// Checks wether a key is pressed or not
        /// </summary>
        /// <param name="k">The key to check</param>
        /// <returns>true if the key is pressed, false otherwise.</returns>
        public bool IsKeyDown(Keys k)
        {
            return Keys.Contains(k);
        }
        /// <summary>
        /// Checks wether a key is pressed or not
        /// </summary>
        /// <param name="k">The key to check</param>
        /// <returns>true if the key is not pressed, false otherwise.</returns>
        public bool IsKeyUp(Keys k)
        {
            return !Keys.Contains(k);
        }

        /// <summary>
        /// Checks wether the user just pressed a key or not
        /// </summary>
        /// <param name="k">The key to check</param>
        /// <returns>true if the key is toggled, false otherwise</returns>
        public bool KeyJustPressed(Keys k)
        {
            return GInput.Keyboard.IsKeyDown(k) && !GInput.OldKeyboard.IsKeyDown(k);
        }
        /// <summary>
        /// Checks wether the user just released a key or not
        /// </summary>
        /// <param name="k">The key to check</param>
        /// <returns>true if the key is toggled, false otherwise</returns>
        public bool KeyJustLifted(Keys k)
        {
            return !GInput.Keyboard.IsKeyDown(k) && GInput.OldKeyboard.IsKeyDown(k);
        }

        /// <summary>
        /// Creates the current KeyHandler state from XNA data
        /// </summary>
        /// <param name="index">The index of the keyboard to check</param>
        /// <returns>The current KeyHandler state</returns>
        public static KeyHandler GetState(PlayerIndex index = PlayerIndex.One)
        {
            return new KeyHandler()
            {
                // holding an array (pointer collection) instead of a structure -> less memory usage
                Keys = (from k in Keyboard.GetState(index).GetPressedKeys()
                        where k != Microsoft.Xna.Framework.Input.Keys.None select k).ToArray()
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.UI.Interface.Controls.Primitives
{
    /// <summary>
    /// A Focusable that listens to an input
    /// </summary>
    public abstract class ListeningControl : Focusable
    {
        /// <summary>
        /// When a ListeningControl got input
        /// </summary>
        public static Action<ListeningControl, object> GlobalInputGot;

        /// <summary>
        /// Wether the ListeningControl listens to the Keyboard or not
        /// </summary>
        protected bool ListensToKeyboard = false;

        bool listen = false;

        /// <summary>
        /// Wether the ListeningControl is listening or not
        /// </summary>
        protected bool Listening
        {
            get
            {
                return listen;
            }
            set
            {
                SomethingIsListening = (listen = value) && ListensToKeyboard;
            }
        }

        /// <summary>
        /// Creates a new instance of the ListeningControl class
        /// </summary>
        public ListeningControl()
            : base()
        {

        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (listen)
                SomethingIsListening = ListensToKeyboard;
        }
    }

    /// <summary>
    /// A Focusable that listens to a certain type of input
    /// </summary>
    /// <typeparam name="T">The type of the input</typeparam>
    public abstract class ListeningControl<T> : ListeningControl
    {
        /// <summary>
        /// When the ListeningControl got input
        /// </summary>
        public Action<ListeningControl<T>, T> OnInputGot;

        /// <summary>
        /// Creates a new instance of the ListeningControl class
        /// </summary>
        public ListeningControl()
            : base()
        {

        }

        /// <summary>
        /// Gets input. Must be called manually.
        /// </summary>
        /// <param name="got">The input that the ListeningControl got</param>
        protected virtual void GotInput(T got)
        {
            if (OnInputGot != null)
                OnInputGot(this, got);

            if (GlobalInputGot != null)
                GlobalInputGot(this, got as object);
        }
    }
}

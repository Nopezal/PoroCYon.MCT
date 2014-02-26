using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.GUI.Controls.Primitives
{
    public abstract class ListeningControl : Focusable
    {
        bool listening;

        public bool Listening
        {
            get
            {
                return listening && IsFocused;
            }
            protected set
            {
                listening = IsFocused = value;
            }
        }

        public Action<ListeningControl, object> OnInputGot;
        public static Action<ListeningControl, object> GlobalInputGot;

        public ListeningControl()
            : base()
        {

        }

        protected override void FocusGot()
        {
            Listening = true;
        }
        protected override void FocusLost()
        {
            Listening = false;
        }

        protected virtual void GotInput(object got)
        {
            if (OnInputGot != null)
                OnInputGot(this, got);
            if (GlobalInputGot != null)
                GlobalInputGot(this, got);
        }
    }
    public abstract class ListeningControl<T> : ListeningControl
    {
        public Action<ListeningControl<T>, T> OnInputGot;
        //public static Action<ListeningControl<T>, T> GlobalInputGot;

        protected override void GotInput(object got)
        {
            GotInput((T)got);

            if (OnInputGot != null)
                OnInputGot(this, (T)got);
            //if (GlobalInputGot != null)
            //    GlobalInputGot(this, (T)got);

            base.GotInput(got);
        }

        protected virtual void GotInput(T got) { }
    }
}

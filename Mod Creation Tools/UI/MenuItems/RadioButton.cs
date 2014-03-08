using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.XnaExtensions;

namespace PoroCYon.MCT.UI.MenuItems
{
    /// <summary>
    /// A Radio button
    /// </summary>
    public class RadioButton : CheckBox, IDisposable
    {
        internal static Dictionary<string, List<RadioButton>> groups = new Dictionary<string, List<RadioButton>>();

        string grName = "";

        static bool preventSO = false;

        /// <summary>
        /// The group name of the radio button
        /// </summary>
        public string GroupName
        {
            get
            {
                return grName;
            }
            set
            {
                if (grName == value)
                    return;

                if (!grName.IsEmpty() && groups.ContainsKey(grName))
                    groups[grName].Remove(this);

                grName = value;

                if (!groups.ContainsKey(grName))
                    groups.Add(grName, new List<RadioButton>());

                groups[grName].Add(this);
            }
        }

        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        public RadioButton()
            : this(false)
        {

        }
        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        /// <param name="isChecked">Wether the radio button is checked or not</param>
        public RadioButton(bool isChecked)
            : base(isChecked)
        {

        }
        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        /// <param name="groupName">The group name of the radio button</param>
        public RadioButton(string groupName)
            : this(false, groupName)
        {

        }
        /// <summary>
        /// Creates a new instance of the RadioButton class
        /// </summary>
        /// <param name="isChecked">Wether the radio button is checked or not</param>
        /// <param name="groupName">The group name of the radio button</param>
        public RadioButton(bool isChecked, string groupName)
            : this(isChecked)
        {
            GroupName = groupName;
        }

        /// <summary>
        /// Finalizes the RadioButton instance.
        /// </summary>
        ~RadioButton()
        {
            Dispose(false);
        }
        /// <summary>
        /// Disposes the RadioButton instance.
        /// </summary>
        public void Dispose()
        {
            groups[grName].Remove(this);
            if (groups[grName].Count == 0)
                groups.Remove(grName);

            Dispose(true);

            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Called when the object is disposed
        /// </summary>
        /// <param name="forced">Wether the object is disposed through IDisposable.Dispose or not</param>
        protected virtual void Dispose(bool forced)
        {

        }

        /// <summary>
        /// Called when the CheckBox is checked
        /// </summary>
        protected override void Checked()
        {
            base.Checked();

            if (preventSO)
                return;

            if (!grName.IsEmpty())
                foreach (RadioButton rb in groups[grName])
                    if (rb != this)
                        rb.IsChecked = false;
        }
        /// <summary>
        /// Called when the CheckBox is unchecked
        /// </summary>
        protected override void Unchecked()
        {
            base.Unchecked();

            preventSO = true;

            if (!grName.IsEmpty())
                foreach (RadioButton rb in groups[grName])
                    if (rb != this)
                        if (rb.IsChecked)
                        {
                            preventSO = false;
                            return;
                        }

            IsChecked = true; // do not allow no RB to be checked

            preventSO = false;
        }
    }
}

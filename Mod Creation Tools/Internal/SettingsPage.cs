using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using PoroCYon.Extensions;
using TAPI;
using PoroCYon.MCT.Internal.ModClasses;
using PoroCYon.MCT.Internal.Versioning;
using PoroCYon.MCT.UI.MenuItems;

namespace PoroCYon.MCT.Internal
{
    sealed class SettingsPage : Page
    {
        CheckBox checkUpdates;

        public static SettingsPage PageInstance;

        /// <summary>
        /// Creates a new instance of the SettingsPage class
        /// </summary>
        public SettingsPage()
            : base()
        {
            OnEntry += () => checkUpdates.IsChecked = UpdateChecker.CheckForUpdates;
        }

        /// <summary>
        /// Initializes the Page
        /// </summary>
        protected override void Init()
        {
            base.Init();

            MenuAnchor
                aLeft = new MenuAnchor()
                {
                    anchor = new Vector2(0.5f, 0),
                    offset = new Vector2(-105, 200),
                    offset_button = new Vector2(0, 50)
                },
                aRight = new MenuAnchor()
                {
                    anchor = new Vector2(0.5f, 0),
                    offset = new Vector2(105, 200),
                    offset_button = new Vector2(0, 50)
                },
                aCentre = new MenuAnchor()
                {
                    anchor = new Vector2(0.5f, 0f),
                    offset = new Vector2(0f, 200f),
                    offset_button = new Vector2(0f, 50f)
                };

            anchors.AddRange(new List<MenuAnchor>() { aLeft, aRight, aCentre });

            int index = 0;

            buttons.Add(checkUpdates = new CheckBox(UpdateChecker.CheckForUpdates, "Automatically check\nfor updates")
            {
                anchor = new Vector2(0f),
                offset = new Vector2(300f, 200f),

                OnChecked = (cb) =>
                {
                    UpdateChecker.CheckForUpdates = true;
                },
                OnUnchecked = (cb) =>
                {
                    UpdateChecker.CheckForUpdates = false;
                }
            }
            .Where(cb =>
            {
                cb.SetAutomaticPosition(aCentre, index++);

                cb.SetSize(cb.size.Where(delegate(ref Vector2 v) { v.Y *= 2; }));

                index++;
            }));

            buttons.Add(new MenuButton(0, "Check for updates", "").Where(mb =>
            {
                mb.SetAutomaticPosition(aCentre, index++);

                mb.Click = () =>
                {
                    if (UpdateChecker.IsUpdateAvailable())
                        UpdateBoxInjector.Inject();
                    else
                        mb.description = "No update availables";
                };
            }));

            buttons.Add(new MenuButton(0, "Back", "Options").Where(mb =>
            {
                mb.SetAutomaticPosition(aCentre, ++index);
                index++;

                mb.Click += () =>
                {
                    FileStream fs = new FileStream(Mod.MCTDataFile, FileMode.Create);
                    Mod.WriteSettings(fs);
                    fs.Close();
                };
            }));
        }
    }
}

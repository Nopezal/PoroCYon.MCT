using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace PoroCYon.MCT
{
    /// <summary>
    /// An invasion. Does only manage active/inactive state and displayed text, not the NPCs spawning.
    /// </summary>
    public abstract class Invasion
    {
        internal static Dictionary<string, Invasion> invasions = new Dictionary<string, Invasion>();
        internal static Dictionary<int, string> invasionTypes = new Dictionary<int, string>();

        /// <summary>
        /// Gets the ID of the invasion.
        /// </summary>
        public int ID
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets wether the invasion is active or not.
        /// </summary>
        public bool IsActive
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the display name of the invasion.
        /// </summary>
        public abstract string DisplayName
        {
            get;
        }

        /// <summary>
        /// Gets the start text of the invasion. Must be invoked. The argument should be "east" or "west".
        /// </summary>
        public Func<string, string> StartText
        {
            get;
            protected set;
        }
        /// <summary>
        /// Gets the arrived text of the invasion.
        /// </summary>
        public virtual string ArrivedText
        {
            get
            {
                return "The " + DisplayName + " has arrived!";
            }
        }
        /// <summary>
        /// Gets the defeated text of the invasion.
        /// </summary>
        public virtual string DefeatedText
        {
            get
            {
                return "The " + DisplayName + " has been defeated!";
            }
        }

        /// <summary>
        /// Creates a new instance of the Invasion class.
        /// </summary>
        protected Invasion()
        {
            StartText = d => "A " + DisplayName + " is coming from the " + d + "!";
        }

        /// <summary>
        /// Gets an Invasion from the specified ID.
        /// </summary>
        /// <param name="id">The ID of the Invasion to get.</param>
        /// <returns>The Invasion from the specified ID</returns>
        public static Invasion FromID(int id)
        {
            return FromInternalName(invasionTypes[id]);
        }
        /// <summary>
        /// Gets an Invasion from the specified internal name.
        /// </summary>
        /// <param name="name">The internal name the Invasion to get.</param>
        /// <returns>The Invasion from the specified internal name</returns>
        public static Invasion FromInternalName(string name)
        {
            return invasions[name];
        }

        /// <summary>
        /// Starts the invasion.
        /// </summary>
        public virtual void Start()
        {
            Main.StartInvasion(ID);
        }
        /// <summary>
        /// Stops the invasion. (all of them, actually)
        /// </summary>
        public virtual void Stop()
        {
            World.StopInvasions();
        }
    }

    class GoblinArmyInv : Invasion
    {
        public override string DisplayName
        {
            get
            {
                return "Goblin army";
            }
        }
    }
    class FrostLegionInv : Invasion
    {
        public override string DisplayName
        {
            get
            {
                return "Frost Legion";
            }
        }
    }
    class PiratesInv : Invasion
    {
        public override string DisplayName
        {
            get
            {
                return "Pirates";
            }
        }

        public override string ArrivedText
        {
            get
            {
                return "The " + DisplayName + " have arrived!";
            }
        }
        public override string DefeatedText
        {
            get
            {
                return "The " + DisplayName + " have been defeated!";
            }
        }

        internal PiratesInv()
            : base()
        {
            StartText = d => DisplayName + " are coming from the " + d + "!";
        }
    }
}

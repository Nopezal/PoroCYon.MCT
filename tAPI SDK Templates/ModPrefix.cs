using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAPI.$safeprojectname$
{
    public class ModPrefix : TAPI.ModPrefix
    {
        public ModPrefix(ModBase @base, Prefix p)
            : base(@base, p)
        {

        }

        /// <summary>
        /// Called when a Prefix is applyed to an Item
        /// </summary>
        /// <param name="i">The Item where the Prefix is applied to</param>
        public override void ApplyToItem(Item i)
        {
            base.ApplyToItem(i);

            // TODO: add your ApplyToItem logic here
        }
        /// <summary>
        /// Called when a Prefix is applyed to a Player
        /// </summary>
        /// <param name="p">The Player where the Prefix is applied to</param>
        public override void ApplyToPlayer(Player p)
        {
            base.ApplyToPlayer(p);

            // TODO: add your ApplyToPlayer logic here
        }
    }
}

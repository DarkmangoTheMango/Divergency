using Divergency.Content.Events.LivingCore.InfiniteRoomParts;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;


namespace Divergency.Content.Events.LivingCore.InfiniteRoomParts.Kill
{
    public class BlockersSidePattern : Pattern
    {
        public override bool DoKill(List<ActivePattern> activePatterns)
        {
            int blockerCountL = 0;
            int blockerCountR = 0;

            foreach (ActivePattern ap in activePatterns)
            {
                if (ap.unit.fullName == "Divergency/CoreBlockadeRight")
                {
                    if (ap.position.X <= 0)
                        return true;
                    blockerCountL++;
                }

                if (ap.unit.fullName == "Divergency/CoreBlockadeLeft")
                {
                    if (ap.position.X >= 0)
                        return true;
                    blockerCountR++;
                }
            }

            if (blockerCountL > 1 || blockerCountR > 1)
                return true;

            return false;
        }
    }
}

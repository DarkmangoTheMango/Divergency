using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;


namespace Divergency.Content.Events.LivingCore.Rooms.InfiniteRoomParts.Kill
{
    public class BlockersSidePattern : Pattern
    {
        public override bool DoKill(List<ActivePattern> activePatterns)
        {
            int blockerCountL = 0;
            int blockerCountR = 0;

            foreach (ActivePattern ap in activePatterns)
            {
                if (ap.unit.id == ModContent.NPCType<CoreBlockadeRight>())
                {
                    if (ap.position.X <= 0)
                        return true;
                    blockerCountL++;
                }

                if (ap.unit.id == ModContent.NPCType<CoreBlockadeLeft>())
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

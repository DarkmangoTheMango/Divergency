using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore.Rooms.InfiniteRoomParts.Kill
{
    public class CorennectorLimitPattern : Pattern
    {
        public override bool DoKill(List<ActivePattern> activePatterns)
        {
            int npcCount = 0;
            int corennectorCount = 0;
            foreach (ActivePattern ap in activePatterns)
            {
                if (ap.unit.id == ModContent.NPCType<Corennector>())
                    corennectorCount++;
                else
                    npcCount++;
            }

            if (npcCount / 5 < corennectorCount) // amount of units pr corennector... as a minimum, that is
                return true;

            return false;
        }
    }
}

using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore.Rooms.InfiniteRoomParts.Kill
{
    public class SeerCapPattern : Pattern
    {
        public override bool DoKill(List<ActivePattern> activePatterns)
        {
            int under = 0;
            int over = 0;

            foreach (ActivePattern ap in activePatterns)
            {
                if (ap.unit.id == ModContent.NPCType<Underseer>())
                    under++;
                else if (ap.unit.id == ModContent.NPCType<Overseer>())
                    over++;
            }

            if (under > 1 || over > 1)
                return true;

            return false;
        }
    }
}

using Divergency.Content.Events.LivingCore.InfiniteRoomParts;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;


namespace Divergency.Content.Events.LivingCore.InfiniteRoomParts.Kill
{
    public class OverAndUnderPattern : Pattern
    {
        public override bool DoKill(List<ActivePattern> activePatterns)
        {
            foreach (ActivePattern ap in activePatterns)
            {
                if (ap.unit.fullName == "Divergency/Overseer" && ap.position.Y > -80)
                    return true;
                if (ap.unit.fullName == "Divergency/Underseer" && ap.position.Y < -20)
                    return true;
            }

            return false;
        }
    }
}

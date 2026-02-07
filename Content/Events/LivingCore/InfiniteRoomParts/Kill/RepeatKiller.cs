using Divergency.Content.Events.LivingCore.InfiniteRoomParts;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore.InfiniteRoomParts.Kill
{
    public class RepeatKiller : Pattern
    {
        public override bool DoKill(List<ActivePattern> activePatterns)
        {
            string lastName = "";
            for (int i = 0; i < activePatterns.Count; i++)
            {
                ActivePattern ap = activePatterns[i];
                if (ap.unit.fullName == lastName)
                {
                    if (activePatterns.Count > i + 1)
                    {
                        if (activePatterns[i + 1].unit.fullName == lastName) // allow three or more in a row
                        {
                            lastName = "";
                            continue;
                        }
                    }

                    return true;
                }
                lastName = ap.unit.fullName;
            }

            return false;
        }
    }
}

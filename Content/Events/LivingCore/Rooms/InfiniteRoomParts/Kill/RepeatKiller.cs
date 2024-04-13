using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore.Rooms.InfiniteRoomParts.Kill
{
    public class RepeatKiller : Pattern
    {
        public override bool DoKill(List<ActivePattern> activePatterns)
        {
            int lastID = 0;
            for (int i = 0; i < activePatterns.Count; i++)
            {
                ActivePattern ap = activePatterns[i];
                if (ap.unit.id == lastID)
                {
                    if (activePatterns.Count > i + 1)
                    {
                        if (activePatterns[i + 1].unit.id == lastID) // allow three or more in a row
                        {
                            lastID = 0;
                            continue;
                        }
                    }

                    return true;
                }
                lastID = ap.unit.id;
            }

            return false;
        }
    }
}

using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore.InfiniteRoomParts
{
    public class ByUnitPattern : Pattern
    {
        public static List<ActivePattern> offsetPositions = new List<ActivePattern>()
        {
            new ActivePattern("Divergency/CoreBlockadeLeft", new Vector2(90, 0)), // the wrong sided one gets deleted...
            new ActivePattern("Divergency/CoreBlockadeRight", new Vector2(90, 0)),
            new ActivePattern("Divergency/Corennector", new Vector2(120, 0)), // behind and above \/
            new ActivePattern("Divergency/Corennector", new Vector2(0, -120)),
        };

    public override float costMultiplier => 1.8f;

        public override List<List<ActivePattern>> Possibilities(List<ActivePattern> activePatterns)
        {
            List<List<ActivePattern>> ret = new List<List<ActivePattern>>();


            foreach (ActivePattern ap in activePatterns)
            {
                foreach (ActivePattern justData in offsetPositions)
                {
                    List<ActivePattern> napl = new List<ActivePattern>();

                    Vector2 p = justData.position;

                    if (ap.position.X < 0)
                    {
                        napl.Add(new ActivePattern(justData.unit, ap.position + new Vector2(-p.X, p.Y)));
                    }
                    else if (ap.position.X > 0)
                    {
                        napl.Add(new ActivePattern(justData.unit, ap.position + new Vector2(p.X, p.Y)));
                    }

                    ret.Add(napl);
                }
            }


            return ret; // all possible units to add...
        }
    }
}

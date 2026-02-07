using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Divergency.Content.Events.LivingCore.InfiniteRoomParts
{
    public class CenterPosPattern : Pattern
    {
        public static List<Vector2> positions = new List<Vector2>() {
            new Vector2(0, 0),
            new Vector2(0, -100),
            new Vector2(0, -150),
            new Vector2(0, -200),
            new Vector2(0, -300),
        };
        public override float costMultiplier => 1.00001f;
        public override List<List<ActivePattern>> Possibilities(List<ActivePattern> activePatterns)
        {
            List<List<ActivePattern>> ret = new List<List<ActivePattern>>();


            foreach (Unit u in Unit.units)
            {
                foreach (Vector2 v in positions)
                {
                    List<ActivePattern> toAdd = new List<ActivePattern>();
                    toAdd.Add(new ActivePattern(u, v));
                    ret.Add(toAdd);
                }
            }


            return ret; // all possible units to add...
        }
    }
}

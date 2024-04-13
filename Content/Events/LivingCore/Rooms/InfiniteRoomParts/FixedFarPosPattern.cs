using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore.Rooms.InfiniteRoomParts
{
    public class FixedFarPosPattern : Pattern
    {
		public static List<Vector2> positions = new List<Vector2>() {
			new Vector2(-300, 0),
            new Vector2(-250, -200),
            new Vector2(-100, -200),
            new Vector2(-360, -80),
        };

        public override List<List<ActivePattern>> Possibilities(List<ActivePattern> activePatterns)
        {
            List<List<ActivePattern>> ret = new List<List<ActivePattern>>();


            foreach (Unit u in Unit.units)
            {
                foreach (Vector2 v in positions)
                {
                    List<ActivePattern> toAdd1 = [new ActivePattern(u, v)];
                    List<ActivePattern> toAdd2 = [new ActivePattern(u, new Vector2(-v.X, v.Y))];
                    ret.Add(toAdd1);
                    ret.Add(toAdd2);
                }
            }


            return ret; // all possible units to add...
        }
    }
}

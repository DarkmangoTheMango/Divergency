using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;


namespace Divergency.Content.Events.LivingCore.Rooms.InfiniteRoomParts
{
    public class MirrorPattern : Pattern
    {
        public override float costMultiplier => 1.4f;

        public override List<List<ActivePattern>> Possibilities(List<ActivePattern> activePatterns)
        {
            List<List<ActivePattern>> ret = new List<List<ActivePattern>>();

            foreach (ActivePattern ap in activePatterns)
            {
                List<ActivePattern> napList = new List<ActivePattern>() { }; // nap -> new active pattern
                List<ActivePattern> napList2x = new List<ActivePattern>() { }; // nap -> new active pattern

                napList.Add(new ActivePattern(ap.unit, ap.position));
                Vector2 mirrorPos = new Vector2(-ap.position.X, ap.position.Y);
                napList.Add(new ActivePattern(ap.unit, mirrorPos));

                napList2x.Add(new ActivePattern(ap.unit, ap.position));
                napList2x.Add(new ActivePattern(ap.unit, mirrorPos));
                Vector2 mirrorPos2 = new Vector2(ap.position.X, -ap.position.Y);
                Vector2 mirrorPos3 = new Vector2(-ap.position.X, -ap.position.Y);
                napList2x.Add(new ActivePattern(ap.unit, mirrorPos2));
                napList2x.Add(new ActivePattern(ap.unit, mirrorPos3));

                ret.Add(napList);
                ret.Add(napList2x);
            }


            return ret; // all possible units to add...
        }
    }
}

using Divergency.Content.Events.LivingCore.InfiniteRoomParts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Composition.Convention;

namespace Divergency.Content.Events.LivingCore.InfiniteRoomParts.Kill
{
    public class PersonalSpacePatter : Pattern
    {
        public override bool DoKill(List<ActivePattern> activePatterns)
        {
            foreach (ActivePattern ap1 in activePatterns)
            {
                foreach (ActivePattern ap2 in activePatterns)
                {
                    if (ap1 != ap2)
                    {
                        if (Vector2.Distance(ap1.position, ap2.position) < ap1.unit.space + ap2.unit.space)
                            return true;
                    }
                }
            }
            return false;
        }
    }
}

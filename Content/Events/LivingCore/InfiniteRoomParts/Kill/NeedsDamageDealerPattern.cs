using Divergency.Content.Events.LivingCore.InfiniteRoomParts;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;


namespace Divergency.Content.Events.LivingCore.InfiniteRoomParts.Kill
{
    public class NeedsDamageDealerPattern : Pattern
    {
        public override bool DoKill(List<ActivePattern> activePatterns)
        {
            if (activePatterns.Count == 0)
                return false;

            List<string> needsOneOf = new List<string>() { "Divergency/Sage", "Divergency/Coreling", "Divergency/LivingCoreSaw",
                "Divergency/Corelossus", "Divergency/CoreElemental"};

            return !activePatterns.Select(x => { return x.unit.fullName; }).Intersect(needsOneOf).Any();
        }
    }
}

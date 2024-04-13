using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;


namespace Divergency.Content.Events.LivingCore.Rooms.InfiniteRoomParts.Kill
{
    public class NeedsDamageDealerPattern : Pattern
    {
        public override bool DoKill(List<ActivePattern> activePatterns)
        {
            if (activePatterns.Count == 0)
                return false;

            List<int> needsOneOf = new List<int>() { ModContent.NPCType<Sage>(), ModContent.NPCType<Coreling>(), ModContent.NPCType<LivingCoreSaw>(),
                ModContent.NPCType<Corelossus>(), ModContent.NPCType<CoreElemental>()};

            return !activePatterns.Select(x => { return x.unit.id; }).Intersect(needsOneOf).Any();
        }
    }
}

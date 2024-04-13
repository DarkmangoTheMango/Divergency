using Divergency.Content.NPCs.LivingGrove;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore.Rooms.InfiniteRoomParts
{
    public class Unit
    {
        // need to have a lot of things that will increase cost...
        public static List<Unit> units = new List<Unit>() {
            new Unit(ModContent.NPCType<Coreling>(), 0.9f, 10f),

            new Unit(ModContent.NPCType<CoreBlockadeRight>(), 3f, 4f), // you can get them early
            new Unit(ModContent.NPCType<CoreBlockadeLeft>(), 3f, 4f),
            new Unit(ModContent.NPCType<CoreBlockadeRight>(), 20f, 4f), // but they are not ignored late
            new Unit(ModContent.NPCType<CoreBlockadeLeft>(), 20f, 4f),

            // new Unit(ModContent.NPCType<LivingCoreSaw>(), 3f, 10f),
            new Unit(ModContent.NPCType<Corelossus>(), 5f, 18f),
            // new Unit(ModContent.NPCType<CoreElemental>(), 6f, 10f),
            new Unit(ModContent.NPCType<Overseer>(), 7f, 20f),
            new Unit(ModContent.NPCType<Underseer>(), 7f, 20f),
            new Unit(ModContent.NPCType<Corennector>(), 15f, 24f),
            new Unit(ModContent.NPCType<Sage>(), 25f, 40f),
        };
        
        public int id;
        public float cost;
        public float space;

        public Unit(int id, float cost, float space) { this.id = id; this.cost = cost; this.space = space; }
    }
}

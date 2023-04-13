using Divergency.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Buffs
{

    public class Earrape : ModBuff
    {
        public float oldDefense;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earraped");
            Description.SetDefault("I can't hear you it's to dark in here!");

            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<EarrapeNPC>().Earraped = true;
        }
    }

    public class EarrapeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool Earraped;
        

        public int damage;
        public override void ResetEffects(NPC npc)
        {
            Earraped = false;
        }
        public override void AI(NPC npc)
        {
            if (Main.rand.NextBool(4) && Earraped) { Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<NoteDust>(), npc.velocity.X * 1f, npc.velocity.Y * 1f, 0, default, 1f).noGravity = true; }

        }
    }
}
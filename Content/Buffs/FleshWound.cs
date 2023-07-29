using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Buffs
{
    public class FleshWound : ModBuff
	{
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<FleshWoundNPC>().FleshWound = true;
			npc.defense -= 10;
		}

		public override void SetStaticDefaults()
		{
			//.setdefault("Flesh Wound");
			////.setdefault("Slowly losing life \nReduced defense");

			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}
    }

	public class FleshWoundNPC : GlobalNPC
	{
        public override bool InstancePerEntity => true;

        public bool FleshWound;

		public override void ResetEffects(NPC npc) => FleshWound = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
			if (FleshWound)
			{
				if (npc.lifeRegen > 0) { npc.lifeRegen = 0; }

				npc.lifeRegen -= 5;

				if (damage < 1) { damage = 1; }
			}
		}

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
			if (FleshWound)
			{
				if (Main.rand.NextBool(4)) { Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default, 1f); }
			}
		}
    }

	public class FleshWoundIcon : ModItem
    {
		public override string Texture => "Divergency/Content/Buffs/FleshWound";
	}
}

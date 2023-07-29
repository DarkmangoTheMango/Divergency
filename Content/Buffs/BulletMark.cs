using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Buffs
{
    public class BulletMark : ModBuff
	{
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<BulletMarkNPC>().bulletMark = true;
		}

		public override void SetStaticDefaults()
		{
			//.setdefault("Bullet Mark");
			////.setdefault("Deadly Napalm is stuck to you");

			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}
    }

	public class BulletMarkNPC : GlobalNPC
	{
        public override bool InstancePerEntity => true;

        public bool bulletMark;

		public override void ResetEffects(NPC npc) => bulletMark = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
			if (bulletMark)
			{
				if (npc.lifeRegen > 0) { npc.lifeRegen = 0; }

				npc.lifeRegen -= 20;

				if (damage < 1) { damage = 1; }
			}
		}

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
			if (bulletMark)
			{
				if (Main.rand.NextBool(4)) { Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.GemSapphire, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default, 1f).noGravity = true; }
			}
		}
    }

	public class BulletMarkIcon : ModItem
    {
		public override string Texture => "Divergency/Content/Buffs/BulletMark";
	}
}

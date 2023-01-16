using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Divergency.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Buffs
{
	public class CoreInfection : ModBuff
	{
		public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<CoreInfectionNPC>().Infected = true;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shred");
			Description.SetDefault("Rapidly losing life");

			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

	}

	public class CoreInfectionNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool Infected;

		public override void ResetEffects(NPC npc) => Infected = false;


		public override void DrawEffects(NPC npc, ref Color drawColor)
		{
			if (Infected)
			{
				if (Main.rand.NextBool(4)) { Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.GemEmerald, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default, 1f).noGravity = true; }

				Lighting.AddLight(npc.Center, 0.2f, 2.00f, 0.08f);
			}
		}
		public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
		{
			if (item.DamageType == DamageClass.Summon && Infected)
			{
				npc.StrikeNPC(5, 0, 0, true);
			}
		}
		public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
		{
			if (projectile.DamageType == DamageClass.Summon && Infected)

				npc.StrikeNPC(5, 0, 0, true);
		}
	}




	public class CoreInfectionIcon : ModItem
	{
		public override string Texture => "Divergency/Content/Buffs/Shred";
	}
}

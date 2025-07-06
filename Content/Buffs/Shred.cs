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
	public class Shred : ModBuff
	{
		public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<ShredNPC>().shred = true;

		public override void SetStaticDefaults()
		{
			//.setdefault("Shred");
			////.setdefault("Rapidly losing life");

			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}
    }

	public class ShredNPC : GlobalNPC
	{
        public override bool InstancePerEntity => true;

        public bool shred;

		public override void ResetEffects(NPC npc) => shred = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
			if (shred)
			{
				if (npc.lifeRegen > 0) { npc.lifeRegen = 0; }

				npc.lifeRegen -= 20;

				if (damage < 1) { damage = 1; }
			}
		}

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
			if (shred)
			{
				if (Main.rand.NextBool(4)) { Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<ShredBlood>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default, 1f).noGravity = true; }

				Lighting.AddLight(npc.Center, 0.2f, 0, 0.08f);
			}
		}

        public override void OnKill(NPC npc)
		{
            if (shred)
			{
				Player player = Main.LocalPlayer;
				CameraSystem.ScreenShake(5);

				SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Impacts/FleshyExplosion"), npc.Center);
				for (int i = 0; i < 20; i++) { Dust.NewDustPerfect(npc.Center, ModContent.DustType<ShredBlood>(), Main.rand.NextVector2Circular(1f, 1f) * 10f, 50, default, 2f).noGravity = true; }

                if (npc.noGravity) { Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, ((npc.rotation + MathHelper.PiOver2).ToRotationVector2() * -10f).RotatedByRandom(Main.rand.NextFloat(2f)), ModContent.ProjectileType<SavageDagger>(), 10, 2f, player.whoAmI); }
				else if (!npc.noGravity) { Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, new Vector2(npc.direction * -10f, Main.rand.NextFloat(-1f, 1f)), ModContent.ProjectileType<SavageDagger>(), 10, 2f, player.whoAmI); }
			}
		}
    }

	public class ShredIcon : ModItem
    {
		public override string Texture => "Divergency/Content/Buffs/Shred";
	}
}

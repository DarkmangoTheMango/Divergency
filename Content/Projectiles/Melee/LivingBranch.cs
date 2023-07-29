using Divergency.Common.Helpers;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Melee
{
    public class LivingBranch : ModProjectile
    {
        public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = new Vector2(54);
			Projectile.hide = true;

			Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			if (Projectile.ai[0] == 1f)
			{
				int npcIndex = (int)Projectile.ai[1];

				if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
				{
					if (Main.npc[npcIndex].behindTiles) { behindNPCsAndTiles.Add(index); }
					else { behindNPCsAndTiles.Add(index); }

					return;
				}
			}

			behindProjectiles.Add(index);
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			width = height = 10;
			return true;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (targetHitbox.Width > 8 && targetHitbox.Height > 8) { targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8); }
			return projHitbox.Intersects(targetHitbox);
		}

        public override void Kill(int timeLeft)
		{
			NPC npc = Main.npc[targetIdentity];
			Player player = Main.player[Projectile.owner];

			//npc.StrikeNPC(player.GetWeaponDamage(player.HeldItem, false) * 3, 4.5f, 0, true, false, false);
			npc.SimpleStrikeNPC(player.GetWeaponDamage(player.HeldItem, false),0,true, 0);
			SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

			for (int i = 0; i < 10; i++)
			{
				Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Leaf>(), 0f, 0f, 0, default, 1f);
				Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture, 0f, 0f, 0, default, 1f);
			}

			for (int k = 0; k < 2; k++) { Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, Mod.Find<ModGore>("LivingBranch" + k.ToString()).Type, 1f); }
		}

		public bool sticking
		{
			get => Projectile.ai[0] == 1f;
			set => Projectile.ai[0] = value ? 1f : 0f;
		}

		public int targetIdentity
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}
		
		const int maxCount = 5;
		readonly Point[] stickyCount = new Point[maxCount];

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
   
			sticking = true;
			targetIdentity = target.whoAmI;

			Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
			Projectile.netUpdate = true;

			Projectile.damage = 0;
			UpdateStickyJavelins(target);
		}

		void UpdateStickyJavelins(NPC target)
		{
			int currentJavelinIndex = 0;

			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile currentProjectile = Main.projectile[i];
				if (i != Projectile.whoAmI && currentProjectile.active && currentProjectile.owner == Main.myPlayer && currentProjectile.type == Projectile.type && currentProjectile.ModProjectile is LivingBranch javelinProjectile && javelinProjectile.sticking
					&& javelinProjectile.targetIdentity == target.whoAmI)
				{
					stickyCount[currentJavelinIndex++] = new Point(i, currentProjectile.timeLeft);
					if (currentJavelinIndex >= stickyCount.Length) { break; }
				}
			}

			if (currentJavelinIndex >= maxCount)
			{
				int oldJavelinIndex = 0;

				for (int i = 1; i < maxCount; i++) { if (stickyCount[i].Y < stickyCount[oldJavelinIndex].Y) { oldJavelinIndex = i; } }

				Main.projectile[stickyCount[oldJavelinIndex].X].Kill();
			}
		}

		const int maxTick = 45;
		const int alphaReduction = 25;

		public override void AI()
		{

			UpdateAlpha();
			if (sticking) StickyAI();
			else NormalAI();
		}

		void UpdateAlpha()
		{
			if (Projectile.alpha > 0) { Projectile.alpha -= alphaReduction; }
			if (Projectile.alpha < 0) { Projectile.alpha = 0; }
		}

		void NormalAI()
		{
			targetIdentity++;

			if (targetIdentity >= maxTick)
			{
				const float velXmult = 0.98f;
				const float velYmult = 0.35f;

				targetIdentity = maxTick;
				Projectile.velocity.X *= velXmult;
				Projectile.velocity.Y += velYmult;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
		}

		void StickyAI()
		{
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			const int aiFactor = 15;
			Projectile.localAI[0] += 1f;

			bool hitEffect = Projectile.localAI[0] % 30f == 0f;
			int projTargetIndex = (int)targetIdentity;

			if (Projectile.localAI[0] >= 60 * aiFactor || projTargetIndex < 0 || projTargetIndex >= 200) { Projectile.Kill(); }
			else if (Main.npc[projTargetIndex].active && !Main.npc[projTargetIndex].dontTakeDamage)
			{
				Projectile.Center = Main.npc[projTargetIndex].Center - Projectile.velocity * 2f;
				Projectile.gfxOffY = Main.npc[projTargetIndex].gfxOffY;
				if (hitEffect) { Main.npc[projTargetIndex].HitEffect(0, 1.0); }
			}
			else { Projectile.Kill(); }
		}
	}
}
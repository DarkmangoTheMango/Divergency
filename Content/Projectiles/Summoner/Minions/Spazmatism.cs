using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Summoner.Minions
{
    public class SpazmatismBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spazmatism");
			Description.SetDefault("Spazmatism will fight for you");

			Main.buffNoSave[Type] = true;
		}
	}

	public class SpazmatismMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spazmatism");
			Main.projFrames[Projectile.type] = 3;

			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public sealed override void SetDefaults()
		{
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true;
			Projectile.hostile = false;

			Projectile.Size = new Vector2(36f);
			Projectile.scale = 1f;

			Projectile.tileCollide = false;
			Projectile.ignoreWater = false;

			Projectile.aiStyle = -1;
			Projectile.extraUpdates = 1;
		}

        public override bool? CanCutTiles()
		{
			return true;
		}

		public override bool MinionContactDamage()
		{
			return true;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			if (!CheckActive(owner))
			{
				return;
			}

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		private bool CheckActive(Player owner)
		{
			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(ModContent.BuffType<SpazmatismBuff>());
				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<SpazmatismBuff>())) { Projectile.timeLeft = 2; }

			return true;
		}

		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
		{
			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f;

			float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX;

			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
			{
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player player = Main.player[Projectile.owner];

			if (Main.rand.NextBool(4)) { crit = true; }

			player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 5;

			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

			Projectile.velocity *= Projectile.velocity.SafeNormalize(Vector2.One) * 3f;
			Projectile.velocity = Projectile.velocity.RotatedByRandom(3.6f);

			int numberOfDusts = 20;

			for (int i = 0; i < numberOfDusts; i++) { Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfDusts * i)) * 2f, 0, default, 2f).noGravity = true; }
		}

        void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
		{
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			if (owner.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float distance = Vector2.Distance(npc.Center, Projectile.Center);

				if (distance < 2000f)
				{
					distanceFromTarget = distance;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];

					if (npc.CanBeChasedBy())
					{
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;

						if (((closest && inRange) || !foundTarget))
						{
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}

			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
		{
			float speed = 8f;
			float power = 20f;

			if (foundTarget)
			{
				if (distanceFromTarget > 40f)
				{
					Vector2 direction = targetCenter - Projectile.Center;
					direction.Normalize();
					direction *= speed;

					Projectile.velocity = (Projectile.velocity * (power - 1) + direction) / power;
				}
			}
			else
			{
				if (distanceToIdlePosition > 600f)
				{
					speed = 12f;
					power = 60f;
				}
				else
				{
					speed = 4f;
					power = 80f;
				}

				if (distanceToIdlePosition > 20f)
				{
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;

					Projectile.velocity = (Projectile.velocity * (power - 1) + vectorToIdlePosition) / power;
				}
				else if (Projectile.velocity == Vector2.Zero)
				{
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
		}

		private void Visuals()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			int frameSpeed = 5;

			Projectile.frameCounter++;

			if (Projectile.frameCounter >= frameSpeed)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;

				if (Projectile.frame >= Main.projFrames[Projectile.type]) { Projectile.frame = 0; }
			}

			Lighting.AddLight(Projectile.Center, new Color(98, 205, 123).ToVector3() * 0.78f);
		}

		public Trail trail;
		public Trail whiteTrail;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = texture = TextureAssets.Projectile[Projectile.type].Value;

			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int frameY = frameHeight * Projectile.frame;

			Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;
			Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			Color color = Projectile.GetAlpha(lightColor);
			Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

			if (trail == null)
			{
				trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(98, 205, 123, 100)));
				trail.drawOffset = Projectile.Size / 2f;

				whiteTrail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
				whiteTrail.drawOffset = Projectile.Size / 2f;
			}

			trail.Draw(Projectile.oldPos);
			whiteTrail.Draw(Projectile.oldPos);

			return false;
		}
	}
}
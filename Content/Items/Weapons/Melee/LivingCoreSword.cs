using Divergency.Assets.Particles;
using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class LivingCoreSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Core Sword");
            Tooltip.SetDefault("Every third attack spins instead of swinging");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.DamageType = DamageClass.Melee;
			Item.width = 36;
			Item.height = 44;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.reuseDelay = 20;
			Item.channel = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6.5f;
			Item.crit = 9;
			Item.shootSpeed = 14f;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<LivingCoreSwordPro>();
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Blue;
		}
	}

    public class LivingCoreSwordPro : ModProjectile
    {
        public override string Texture => "Divergency/Content/Items/Weapons/Melee/LivingCoreSword";

		enum AttackDirection : int
		{
			Down = 0,
			Up = 1,
			Reset = 2
		}

		private AttackDirection attackDirection = AttackDirection.Down;

		private bool initialized = false;

		private int attackDuration = 0;

		private float startRotation = 0f;

		private float endRotation = 0f;

		private bool facingRight;

		private float zRotation = 0;

		private float rotVel = 0f;

		private int growCounter = 0;

		private List<Vector2> cache;

		private List<float> oldRotation = new();
		private List<Vector2> oldPosition = new();

		private List<NPC> hit = new();

		Player Owner => Main.player[Projectile.owner];

		private bool FirstTickOfSwing => Projectile.ai[0] == 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frying Pan");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.Size = new Vector2(96);
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
			Projectile.extraUpdates = 3;
		}

		public override void AI()
		{
			Projectile.velocity = Vector2.Zero;
			Projectile.Center = Main.GetPlayerArmPosition(Projectile);

			Owner.heldProj = Projectile.whoAmI;

			if (FirstTickOfSwing)
			{
				hit = new List<NPC>();

				if (Owner.DirectionTo(Main.MouseWorld).X > 0)
					facingRight = true;
				else
					facingRight = false;

				float rot = Owner.DirectionTo(Main.MouseWorld).ToRotation();

				SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -1.0f }, Projectile.Center);

				if (!initialized)
				{
					initialized = true;
					endRotation = rot - 1f * Owner.direction;

					oldRotation = new List<float>();
					oldPosition = new List<Vector2>();
				}
				else
				{
                    if (attackDirection == AttackDirection.Down)
                    {
						attackDirection = AttackDirection.Up;
					}
					else
                    {
						attackDirection = AttackDirection.Down;
					}
				}

				startRotation = endRotation;

				switch (attackDirection)
				{
					case AttackDirection.Down:
						endRotation = rot + 2f * Owner.direction;
						attackDuration = 120;
						break;

					case AttackDirection.Up:
						endRotation = rot - 2f * Owner.direction;
						attackDuration = 120;
						break;
				}

				Projectile.ai[0] += 30f / attackDuration;
			}

			if (Projectile.ai[0] < 1)
			{
				Projectile.timeLeft = 50;
				Projectile.ai[0] += 1f / attackDuration;
				rotVel = Math.Abs(EaseProgress(Projectile.ai[0]) - EaseProgress(Projectile.ai[0] - 1f / attackDuration)) * 2;
			}
			else
			{
				rotVel = 0f;
				if (Main.mouseLeft)
				{
					Projectile.ai[0] = 0;
					return;
				}
			}

			float progress = EaseProgress(Projectile.ai[0]);

			Projectile.scale = MathHelper.Min(MathHelper.Min(growCounter++ / 30f, 1 + rotVel * 4), 1.3f);

			Projectile.rotation = MathHelper.Lerp(startRotation, endRotation, progress);

			Owner.ChangeDir(facingRight ? 1 : -1);

			float wrappedRotation = MathHelper.WrapAngle(Projectile.rotation);

			Owner.itemRotation = Projectile.rotation;
			Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation - (facingRight ? 0 : MathHelper.Pi));

			Owner.itemAnimation = Owner.itemTime = 2;

			float throwingAngle = MathHelper.WrapAngle(Owner.DirectionTo(Main.MouseWorld).ToRotation());

			if (Main.netMode != NetmodeID.Server)
			{
				ManageCaches();
			}

			oldRotation.Add(Projectile.rotation);
			oldPosition.Add(Projectile.Center);

			if (oldRotation.Count > 16)
				oldRotation.RemoveAt(0);
			if (oldPosition.Count > 16)
				oldPosition.RemoveAt(0);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (rotVel < 0.005f)
				return false;

			float collisionPoint = 0f;

			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + 42 * Projectile.rotation.ToRotationVector2(), 20, ref collisionPoint))
				return true;

			return false;
		}

		public override bool? CanHitNPC(NPC target)
		{
			if (hit.Contains(target))
				return false;

			return base.CanHitNPC(target);
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			hitDirection = Math.Sign(target.Center.X - Owner.Center.X);
		}

		public Trail trail;
		public Trail whiteTrail;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

			bool flip = false;
			SpriteEffects effects = SpriteEffects.None;

			var origin = new Vector2(0, tex.Height);

			Vector2 scaleVec = Vector2.One;

			for (int k = 16; k > 0; k--)
			{

				float progress = 1 - (float)((16 - k) / (float)16);
				Color color = lightColor * EaseFunction.EaseQuarticOut.Ease(progress) * 0.1f;
				if (k > 0 && k < oldRotation.Count)
					Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color, oldRotation[k] + 0.78f, origin, Projectile.scale * scaleVec, effects, 0f);
			}

			Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + 0.78f, origin, Projectile.scale * scaleVec, effects, 0f);

			Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

			if (trail == null)
			{
				trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(25f), (p) => Projectile.GetAlpha(new Color(79, 214, 126, 100)));
				trail.drawOffset = Projectile.Size / 2f;

				whiteTrail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(15f), (p) => Projectile.GetAlpha(new Color(158, 249, 255, 100)));
				whiteTrail.drawOffset = Projectile.Size / 2f;
			}

			trail.Draw(Projectile.oldPos);
			whiteTrail.Draw(Projectile.oldPos);

			return false;
		}

		private void ManageCaches()
		{
			Vector2 off = Projectile.rotation.ToRotationVector2() * 35;
			off.X *= (float)Math.Cos(zRotation);

			if (cache == null)
			{
				cache = new List<Vector2>();

				for (int i = 0; i < 60; i++)
				{
					cache.Add(Projectile.Center + off);
				}
			}

			cache.Add(Projectile.Center + off);

			while (cache.Count > 60)
			{
				cache.RemoveAt(0);
			}
		}

		private float EaseProgress(float input)
		{
			return attackDirection switch
			{
				AttackDirection.Down => EaseFunction.EaseCircularInOut.Ease(input),
				AttackDirection.Up => EaseFunction.EaseCircularInOut.Ease(input),
				_ => input,
			};
		}
	}
}
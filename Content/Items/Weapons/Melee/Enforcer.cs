using Divergency.Assets.Particles;
using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class Enforcer : ModItem
    {
        public int attackDirection = 1;
        public int AttackCounter = 1;
        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
      
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Enforcer");
            Tooltip.SetDefault("Direct hits deploy orbs, right click in order to call them back to the player" +
                "Orb daamage scales with your current health stat (not maximum)");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 50;
            Item.knockBack = 4f;

            Item.shoot = ModContent.ProjectileType<EnforcerPro>();
            Item.shootSpeed = 1f;

            Item.width = Item.height = 96;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void HoldItem(Player player)
        {
            if (player == Main.LocalPlayer)
            {
                if (player.ItemAnimationActive) { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation - MathHelper.PiOver2 * player.direction); }
                else { player.SetCompositeArmFront(false, default, default); }
            }
        }
        

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ProjectileID.None, damage, knockback, player.whoAmI, attackDirection, 0f);
                for (int k = 0; k < 30 ; k++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(player.Center + (vel * 100f), DustID.GemEmerald, vel * -5f, 0, default, Main.rand.NextFloat(0.5f, 1f));
                    dust.noGravity = true;
                }
            }
            else
            {
                attackDirection = -attackDirection;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EnforcerPro>(), damage, knockback, player.whoAmI, attackDirection, 0f);

            }



            return false;
        }
    }

    public class EnforcerPro : ModProjectile
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Enforcer");

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = new Vector2(90);

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        List<float> oldRotation = new List<float>();

        Vector2 direction;

        bool initialize = true;

        float maxTimeLeft;

        public float SwingDirection => Projectile.ai[0] * Math.Sign(direction.X);

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            if (initialize)
            {
                float attackSpeed = player.GetTotalAttackSpeed(DamageClass.Melee) - 1f;
                Projectile.timeLeft = (int)(player.HeldItem.useAnimation * (1f - attackSpeed));
                maxTimeLeft = Projectile.timeLeft;
                direction = Projectile.velocity;
                direction.Normalize();
                Projectile.rotation = Utils.ToRotation(direction);
                Projectile.netUpdate = true;

                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingStyleLight"), player.Center);

                initialize = false;
            }

            Projectile.Center = player.Center + direction * 45;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(3f * SwingDirection, -3f * SwingDirection, EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
            Projectile.scale = 1.5f + (float)Math.Sin(EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)) * MathHelper.Pi) * 0.6f * 0.6f;

            player.heldProj = Projectile.whoAmI;

            oldRotation.Add(Projectile.rotation);

            if (oldRotation.Count > 10) { oldRotation.RemoveAt(0); }

            //ParticleManager.NewParticle(player.Center + (Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(30f, 110f)), new Vector2(0f, Main.rand.NextFloat(1, 5)).RotatedBy(Projectile.rotation) * -SwingDirection, ParticleManager.NewInstance<StarParticle>(),
              //  new Color(0.50f, 2f, 0.5f, 0), 0.5f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, Projectile.velocity.RotateRandom(3) * Main.rand.NextFloat(7, 10), ModContent.ProjectileType<EnforcerOrb>(), 20, 0, Projectile.owner);

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame, 0, 0);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 drawPosition = player.Center + Projectile.rotation.ToRotationVector2() * 80f - Main.screenPosition;
            float rotation = Projectile.rotation + MathHelper.PiOver4 + (player.direction == -1 ? MathHelper.PiOver2 : 0f);


            SpriteEffects drawFlipped = player.direction == -1 ? SpriteEffects.FlipHorizontally : 0;
            Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, lightColor, rotation, origin, Projectile.scale, drawFlipped, 0f);



            for (int k = 10; k > 0; k--)
            {
                float progress = 1 - (float)(((float)(10 - k) / (float)10));
                Color color = lightColor * EaseFunction.EaseQuarticOut.Ease(progress) * 0.1f;

                if (Projectile.timeLeft < 20)
                {
                    color = Color.Lerp(color, Color.Transparent, 1f - (Projectile.timeLeft / 10f) * k);
                }

                color.A = 0;

                if (k > 0 && k < oldRotation.Count)
                {
                    Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, color, oldRotation[k] + MathHelper.PiOver4 + (player.direction == -1 ? MathHelper.PiOver2 : 0f), origin, Projectile.scale, drawFlipped,
                    0f);
                }
            }
            


            texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Star").Value;
            sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame, 0, 0);
            origin = sourceRectangle.Size() / 2f;
            drawPosition = player.Center + Projectile.rotation.ToRotationVector2() * 120f - Main.screenPosition;

            Color textureColor = Color.Transparent;

            if (Projectile.timeLeft <= maxTimeLeft / 2f)
            {
                textureColor = Color.Lerp(new Color(153, 255, 167, 50), Color.Transparent, 1f - (Projectile.timeLeft / 20f));
            }

            Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, textureColor, 0f, origin, Projectile.scale, drawFlipped, 0f);

            return false;
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            bool flip = false;
            SpriteEffects effects = SpriteEffects.None;

            Vector2 scaleVec = Vector2.One;

            for (int k = 16; k > 0; k--)
            {

                float progress = 1 - (float)((16 - k) / (float)16);
                Color color = lightColor * EaseFunction.EaseQuarticOut.Ease(progress) * 0.1f;
                if (k > 0 && k < oldRotation.Count)
                    Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, color, oldRotation[k] + MathHelper.PiOver4 + (player.direction == -1 ? MathHelper.PiOver2 : 0f), origin, Projectile.scale, drawFlipped, 0f);
            }

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            float collisionPoint = 0f;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, player.Center + ((96 * Projectile.scale) * Projectile.rotation.ToRotationVector2()), 20, ref collisionPoint)) { return true; }

            return false;
        }
    }
    public class EnforcerOrb : ModProjectile
    {
        public bool initialzed;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Orb");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(14);
            Projectile.scale = 1f;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 2000;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 4;



        }

        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            if (!initialzed)
            {

                Projectile.damage = 0;
            }

            Player player = Main.player[Projectile.owner];

            if (Main.mouseRight && Main.mouseRightRelease && player.HeldItem.type == ModContent.ItemType<Enforcer>())
            {
                initialzed = true;
                Projectile.velocity += Projectile.Center.DirectionTo(player.Center) * 3;


            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (initialzed)
            {
                // Projectile.Move(player.Center, 50);
                Projectile.damage = player.statLife / 8;


            }
            if (Projectile.active && Projectile.Hitbox.Intersects(player.Hitbox) && !player.dead)
            {

                player.Heal(1);
                Projectile.Kill();
                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { Volume = 0.8f, MaxInstances = 3 });

                for (int i = 0; i < 8; i++)
                {
                    Vector2 dir = Main.rand.NextVector2Unit() * 0.1f;

                    ParticleManager.NewParticle(Projectile.Center, dir * Main.rand.NextFloat(10, 25), ParticleManager.NewInstance<StarParticle>(), new Color(0.50f, 2f, 0.5f, 0), 0.3f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);

                }

            }
            if (!Particlespawned)
            {
                for (int i = 0; i < 2; i++)
                {
                    ParticleManager.NewParticle(Projectile.Center, new Vector2(0, 0), ParticleManager.NewInstance<BloomParticleProjectile>(), new Color(0.50f, 2f, 0.5f, 0), 0.03f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);

                }
                Particlespawned = true;
            }   

        }

        public Trail trail;
        public Trail trail2;
        private int initialDamage;

        public bool Particlespawned { get; private set; }

        public override bool PreDraw(ref Color lightColor)
        {



            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(25f), (p) => Projectile.GetAlpha(new Color(30, 220, 30, 100)));
                trail.drawOffset = Projectile.Size / 2f;
            }
            if (trail2 == null)
            {
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            trail2.Draw(Projectile.oldPos);

            return false;
        }
    }
}
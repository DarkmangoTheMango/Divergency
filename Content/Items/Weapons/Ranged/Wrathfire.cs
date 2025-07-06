using Divergency.Common.Helpers;
using Divergency.Content.Items.Weapons.Magic;
using Divergency.Content.Items.Weapons.Melee;
using Divergency.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stratum.Content.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Ranged
{
    public class Wrathfire : ModItem
    {
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(16, 16);
            Item.scale = 0.5f;

            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.damage = 120;
            Item.knockBack = 5;

            Item.shoot = ModContent.ProjectileType<WrathfirePro>();
            Item.shootSpeed = 1;

            Item.channel = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.UseSound = SoundID.Item73;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
    }

    public class WrathfirePro : ModProjectile
    {
        public override string Texture => "Divergency/Content/Items/Weapons/Ranged/Wrathfire";

        Player player => Main.player[Projectile.owner];

        Vector2 shakeOffset;

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanCutTiles() => false;

        float charge;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.Size = new (16);

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<Sawblade>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0, Projectile.whoAmI);
            AI();
        }

        public override void AI()
        {
            if (!player.channel)
                Projectile.Kill();

            Projectile.direction = Projectile.Center.X > player.Center.X ? 1 : -1;

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Projectile.direction;

            shakeOffset = Main.rand.NextVector2Circular(1, 1) * (Math.Clamp(charge, 0, 60) * 0.05f);

            Projectile.velocity += (player.DirectionTo(Main.MouseWorld) - Projectile.velocity) * 0.2f;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, false, true) + Projectile.velocity * 30f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            player.itemRotation = Projectile.rotation;
            player.SetDummyItemTime(2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2);

            Dust.NewDustPerfect(player.RotatedRelativePoint(player.MountedCenter, false, true) + Projectile.velocity * 90f, DustID.Terra, -Projectile.velocity * 3, 0, default, 1).noGravity = true;
            Lighting.AddLight(Projectile.Center, new Vector3(0, 1, 0) * 0.1f);

            charge++;

            if (charge == 120)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/RechargeDash"), player.Center);
            }

            if (charge >= 120 && !player.channel)
            {
                Projectile.Kill();
                Projectile.NewProjectile(Entity.GetSource_FromAI(), player.Center, Vector2.Normalize(Projectile.velocity) * 20, ModContent.ProjectileType<WrathBlast>(), 1, Projectile.knockBack, Projectile.owner, 0, Projectile.damage);
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot"), player.Center);
                CameraSystem.ScreenShake(5);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + shakeOffset;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Color color = Projectile.GetAlpha(lightColor);

            Vector2 drawOrigin = sourceRectangle.Size() / 2;

            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            //Draw glow effect

            texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

            color = Projectile.GetAlpha(Color.White);

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }

    public class WrathBlast : ModProjectile
    {
        float timer;

        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(16);
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            ParticleManager.NewParticle<StarParticle2>(Projectile.Center, Projectile.velocity.RotatedByRandom(0.2f) * -Main.rand.Next(1, 2), default, 0.2f);
            Projectile.velocity.Y += 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Entity.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WrathBlast2>(), (int)Projectile.ai[1], Projectile.knockBack, Projectile.owner);
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/WrathfireBlast"), Projectile.Center);
        }

        public Trail trail;
        public Trail trail2;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Light").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(60f), (p) => Projectile.GetAlpha(new Color(0, 255, 0, 0)) * (float)Math.Pow(1f - p, 2f));
                trail.drawOffset = Projectile.Size / 2f;
            }
            if (trail2 == null)
            {
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(60f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 0)) * (float)Math.Pow(1f - p, 2f));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos, timer);
            trail2.Draw(Projectile.oldPos, timer);
            timer -= 0.05f;

            return false;
        }
    }

    public class WrathBlast2 : ModProjectile
    {
        public override bool ShouldUpdatePosition() => false;

        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/AnimatedFire";

        public override bool? CanDamage() => Projectile.ai[0] >= 30;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;

            Projectile.Size = new Vector2(500);
            Projectile.scale = 1f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.Pi);
            Projectile.hide = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 20)
            {
                CombatText.NewText(new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, 0, 0), Color.Lime, "BOOM!", true);
                CameraSystem.ScreenShake(50, 0.95f, Projectile.Center);
                
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, Vector2.Zero, default, 10);
                ParticleManager.NewParticle<Flash>(Projectile.Center, Vector2.Zero, default, 0);

                for (int k = 0; k < 60; k++)
                    ParticleManager.NewParticle<StarParticle2>(Projectile.Center, Main.rand.NextVector2Circular(1, 1) * 60, default, 1);
            }

            if (++Projectile.ai[0] >= 20)
            {
                Projectile.hide = false;
                Projectile.scale += 0.1f;

                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type]) Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float t = (float)(Math.Sin(Main.GameUpdateCount * 0.5f) * 0.5f + 0.5f);
            Color interpolatedColor = Color.Lerp(new(0, 255, 0, 0), new(255, 255, 255, 0), t);

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, Projectile.GetAlpha(interpolatedColor), Projectile.rotation, sourceRectangle.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
using Divergency.Common.Helpers;
using Divergency.Content.Items.Weapons.Magic;
using Divergency.Content.Items.Weapons.Melee;
using Divergency.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ReLogic.Utilities;
using Steamworks;
using Stratum.Content.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
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
        SlotId soundSlot;

        Player player => Main.player[Projectile.owner];

        Vector2 shakeOffset;

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanCutTiles() => false;

        float charge;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
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

            shakeOffset = Main.rand.NextVector2Circular(1, 1) * (Math.Clamp(charge, 0, 30) * 0.05f);

            Projectile.velocity += (player.DirectionTo(Main.MouseWorld) - Projectile.velocity) * 0.2f;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, false, true) + Projectile.velocity * 30f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            player.itemRotation = Projectile.rotation;
            player.SetDummyItemTime(2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2 + MathHelper.PiOver4 * player.direction);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2);

            Dust.NewDustPerfect(player.RotatedRelativePoint(player.MountedCenter, false, true) + Projectile.velocity * 90f, DustID.Terra, -Projectile.velocity * 3, 0, default, 1).noGravity = true;
            Lighting.AddLight(Projectile.Center, new Vector3(0, 1, 0) * 0.1f);

            charge++;

            if (charge == 60)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/RechargeDash") { Volume = 0.3f }, player.Center);
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/WrathfireChargeReady"), player.Center);
                ParticleManager.NewParticle<SupernovaFlash>(Projectile.Center + Projectile.velocity * 60, Vector2.Zero, default, 0.5f);
            }

            if (charge >= 60 && !player.channel)
            {
                Projectile.Kill();
                Projectile.NewProjectile(Entity.GetSource_FromAI(), player.Center, Vector2.Normalize(Projectile.velocity) * 20, ModContent.ProjectileType<WrathBlast>(), 1, Projectile.knockBack, Projectile.owner, 0, Projectile.damage);
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot"), player.Center);
                CameraSystem.ScreenShake(5);
            }

            if (!SoundEngine.TryGetActiveSound(soundSlot, out _))
            {
                var tracker = new ProjectileAudioTracker(Projectile);

                soundSlot = SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/WrathfireCharge")
                {
                    IsLooped = true,
                    SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest,
                }, Projectile.Center, soundInstance =>
                {
                    soundInstance.Pitch = MathHelper.Lerp(-1f, 0f, Math.Clamp(charge, 0, 60) / 60f);
                    soundInstance.Position = Projectile.Center;
                    return tracker.IsActiveAndInGame() && player.active && Projectile.active;
                });
            }

            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame > Main.projFrames[Projectile.type] - 1)
                    Projectile.frame = 0;
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
        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
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
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.velocity.Y += 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Entity.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Supernova>(), (int)Projectile.ai[1], Projectile.knockBack, Projectile.owner);
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/WrathfireBlast"), Projectile.Center);
        }

        static Color LerpThreeColors(Color colorA, Color colorB, Color colorC, float progress)
        {
            if (progress < 0.5f)
            {
                float t = progress / 0.5f;
                return Color.Lerp(colorA, colorB, t);
            }
            else
            {
                float t = (progress - 0.5f) / 0.5f;
                return Color.Lerp(colorB, colorC, t);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.oldPos[1] == Vector2.Zero)
                return false;

            Texture2D Texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/CrimeAgainstHumanity").Value;

            VertexStrip strip = new();

            strip.PrepareStripWithProceduralPadding(
                positions: Projectile.oldPos,
                rotations: Projectile.oldRot,
                colorFunction: progress =>
                    LerpThreeColors(
                        new Color(255, 255, 255, 0),
                        new Color(128, 255, 0, 0),
                        new Color(0, 128, 128, 0),
                        progress
                        ),
                widthFunction: progress => 30,
                offsetForAllPositions: Projectile.Size / 2f - Main.screenPosition,
                includeBacksides: false,
                tryStoppingOddBug: true
            );

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            strip.DrawTrail();

            return false;
        }
    }

    public class Supernova : ModProjectile
    {
        const int MaxTime = 40;
        const int EaseInDuration = 20;
        const int FadeInDelay = 10;
        const int FadeOutStart = 20;
        const float MaxScale = 3f;
        const float BaseRadius = 240f;
    
        float intensity;

        public override string Texture => "Divergency/Assets/Textures/Noise/TurbulentNoise";

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(BaseRadius * 2f);
            Projectile.scale = 1f;
            Projectile.timeLeft = MaxTime;

            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ParticleManager.NewParticle<SupernovaFlash>(Projectile.Center, Vector2.Zero, default, 1);

            CameraSystem.ScreenShake(20, 0.95f, Projectile.Center);

            SpawnParticles();

            Projectile.scale = 0f;
        }
    
        void SpawnParticles()
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(10f, 20f);
                ParticleManager.NewParticle<CoreSparkle>(Projectile.Center, velocity, default, 4);
            }
        }

        public override void AI()
        {
            float scaleLerp = MathHelper.Clamp((MaxTime - Projectile.timeLeft) / (float)(MaxTime - EaseInDuration), 0f, 1f);

            Projectile.scale = MathF.Sin(scaleLerp * MathHelper.PiOver2) * 1;

            int fadeElapsed = Math.Max(0, MaxTime - Projectile.timeLeft - FadeInDelay);
            float fadeProgress = MathHelper.Clamp(fadeElapsed / (float)(MaxTime - FadeInDelay), 0f, 1f);

            intensity = MathHelper.Lerp(0.1f, 1f, fadeProgress);

            float fadeOutProgress = MathHelper.Clamp((Projectile.timeLeft - FadeOutStart) / (float)(MaxTime - FadeOutStart), 0f, 1f);
            float easedFade = MathF.Sin(fadeOutProgress * MathHelper.PiOver2);

            Lighting.AddLight(Projectile.Center, new Color(0, 255, 0).ToVector3() * 5f * easedFade * Projectile.scale);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float radius = Projectile.scale * BaseRadius;
            hitbox = new Rectangle((int)(Projectile.Center.X - radius), (int)(Projectile.Center.Y - radius), (int)(radius * 2f), (int)(radius * 2f));
        }

        public override bool CanHitPlayer(Player target)
        {
            if (intensity >= 0.5f)
                return false;

            float radius = Projectile.scale * BaseRadius;
            float distance = Vector2.Distance(target.Center, Projectile.Center);

            return distance <= radius;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, Divergency.Supernova.Value, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Bounds, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.94f, SpriteEffects.None, 0);

            Divergency.Supernova.Value.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            Divergency.Supernova.Value.Parameters["intensity"].SetValue(intensity);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
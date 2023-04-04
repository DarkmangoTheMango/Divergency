using Divergency.Common.Helpers;
using Divergency.Content.Items.Weapons.LivingCore;
using Divergency.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Summoner.Minions
{
    public class CoreprismProj : ModProjectile
    {
        public bool AuraSpawned { get; private set; }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coreprism");
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 7;

        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 20;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.gfxOffY = -30;
        }
        int frameTimer;
        int timer;
        int timer2;
        private bool collided;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Player player = Main.LocalPlayer;
            Projectile.spriteDirection = (int)Projectile.ai[0];
            Projectile.velocity.Y += 1;
            if (!CheckActive(owner))
                return;
            if (collided)
            {
                timer++;
                if (timer == 150)
                {
                    timer = 0;
                   DivergencyDraw.SpawnCirclePulse(Projectile.Center, Color.LimeGreen, 0.5f);
                }
                timer2++;
                if (timer2 == 30)
                {
                    ParticleManager.NewParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(-12, 12), 0), new Vector2(0, -0.3f), ParticleManager.NewInstance<BloomParticle2>(), new Color(0.50f, 2f, 0.5f, 0), Main.rand.NextFloat(0.008f, 0.015f), Layer:Particle.Layer.BeforeProjectiles);
                    ParticleManager.NewParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(-12, 12), 0), new Vector2(0, -0.5f), ParticleManager.NewInstance<BloomParticle2>(), new Color(0.50f, 2f, 0.5f, 0), Main.rand.NextFloat(0.008f, 0.015f), Layer: Particle.Layer.BeforeProjectiles);
                    ParticleManager.NewParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(-12, 12), 0), new Vector2(0, -0.2f), ParticleManager.NewInstance<BloomParticle2>(), new Color(0.50f, 2f, 0.5f, 0), Main.rand.NextFloat(0.008f, 0.015f), Layer: Particle.Layer.BeforeProjectiles);
                    ParticleManager.NewParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(-12,12),0), new Vector2(0,-0.7f) , ParticleManager.NewInstance<BloomParticle2>(), new Color(0.50f, 2f, 0.5f, 0), Main.rand.NextFloat(0.008f, 0.015f), Layer: Particle.Layer.BeforeProjectiles);

                    timer2 = 0;
                }

                if (Projectile.frame != 6 && collided)
                {
                    frameTimer++;
                }
                if (frameTimer == 6)
                {
                    Projectile.frame++;
                    frameTimer = 0;
                }


                if (!AuraSpawned && Projectile.owner == Main.myPlayer)
                {
                    AuraSpawned = true;

                }
                if (player.Distance(Projectile.Center) < 500)
                {
                    player.AddBuff(BuffID.DryadsWard, 1);
                }
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y > 0)
                Projectile.velocity.Y = 0;
            collided = true;
            return false;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                Projectile.timeLeft = 3;
            }

            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture, position - new Vector2(0,10), sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }



    }
}
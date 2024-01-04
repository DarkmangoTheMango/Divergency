using Divergency.Common.Helpers;
using Divergency.Content.Buffs;
using Divergency.Content.Items.Weapons.LivingCore;
using Divergency.Content.Particles;
using Divergency.Content.Tiles.LivingGrove.CorePuzzle;
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
            //.setdefault("Coreprism");
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
        int soundtimer = 9;

        private bool collided;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Player player = Main.LocalPlayer;

            for (int io = 0; io < Main.maxNPCs; io++)
            {
                NPC target = Main.npc[io];
                if (target.Distance(Projectile.Center) < 500 && !target.HasBuff(ModContent.BuffType<CoreInfection>()) && !target.HasBuff(ModContent.BuffType<CoreInfectionII>()))
                {
                    target.AddBuff(ModContent.BuffType<CoreInfection>(), 5);
                }
            }
            if (player != null)
                Projectile.spriteDirection = (int)Projectile.ai[0];
            Projectile.velocity.Y += 1;
            if (!CheckActive(owner))
                return;
            if (collided)
            {



                timer++;
                if (timer == 200)
                {
                    timer = 0;
                    DivergencyDraw.SpawnCirclePulse(Projectile.Center, Color.LimeGreen, 0.6f);
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse, Projectile.Center);
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                        ParticleManager.NewParticle(Projectile.Center, speed * Main.rand.NextFloat(10, 25), ParticleManager.NewInstance<StarParticle>(), new Color(0, 200, 0, 0), 0.4f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);

                    }

                }
                timer2++;
                if (timer2 == 30)
                {
                    ParticleManager.NewParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(-12, 12), 0), new Vector2(0, -0.3f), ParticleManager.NewInstance<BloomParticle2>(), new Color(0.50f, 2f, 0.5f, 0), Main.rand.NextFloat(0.008f, 0.015f), Layer: Particle.Layer.BeforeProjectiles);
                    ParticleManager.NewParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(-12, 12), 0), new Vector2(0, -0.5f), ParticleManager.NewInstance<BloomParticle2>(), new Color(0.50f, 2f, 0.5f, 0), Main.rand.NextFloat(0.008f, 0.015f), Layer: Particle.Layer.BeforeProjectiles);
                    ParticleManager.NewParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(-12, 12), 0), new Vector2(0, -0.2f), ParticleManager.NewInstance<BloomParticle2>(), new Color(0.50f, 2f, 0.5f, 0), Main.rand.NextFloat(0.008f, 0.015f), Layer: Particle.Layer.BeforeProjectiles);
                    ParticleManager.NewParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(-12, 12), 0), new Vector2(0, -0.7f), ParticleManager.NewInstance<BloomParticle2>(), new Color(0.50f, 2f, 0.5f, 0), Main.rand.NextFloat(0.008f, 0.015f), Layer: Particle.Layer.BeforeProjectiles);

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
                    SoundEngine.PlaySound(SoundID.WormDig, Projectile.Center);

                }


                if (!AuraSpawned && Projectile.owner == Main.myPlayer)
                {
                    AuraSpawned = true;

                }
                if (player.Distance(Projectile.Center) < 500)
                {
                    player.AddBuff(BuffID.Summoning, 1);
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
            Projectile.velocity.X = 0;
            if (collided == false)
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, Projectile.Center);
            }
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
        public Trail trail;

        public Trail trail2;

        float radius = 0;

        float timer3 = 0;

        public float timer4 = 0;

        float width = 10;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor);
            Color color1 = new Color(0.50f / 2, 2f / 3, 0.5f / 2, 0);

            Main.EntitySpriteDraw(texture, position - new Vector2(0, 10), sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, position - new Vector2(0, 10), sourceRectangle, color1, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);


            Texture2D texture2 = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Default").Value;

            if (trail == null)
            {
                trail = new Trail(texture2, Trail.DefaultPass, (p) => new Vector2(width), (p) => Projectile.GetAlpha(new Color(0, 255 / 1.2f, 0, 200)));
                trail.drawOffset = Projectile.Size / 2f;

                //trail2 = new Trail(texture2, Trail.DefaultPass, (p) => new Vector2(width / 1.9f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                //trail2.drawOffset = Projectile.Size / 2f;
            }

            int parts = 30;
            Vector2[] tests = new Vector2[parts];
            float[] rotations = new float[parts];

            for (int point = 0; point < parts; point++)
            {
                float rad = ((float)point / (parts - 1)) * MathHelper.TwoPi;

                tests[point] = Projectile.position + new Vector2(MathF.Cos(rad) * radius, MathF.Sin(rad) * radius);
                rotations[point] = rad;
            }

            timer3 -= 0.01f;

            trail.Draw(tests, rotations, timer3);
            // trail2.Draw(tests, rotations, timer3);


            return false;
        }



    }
}
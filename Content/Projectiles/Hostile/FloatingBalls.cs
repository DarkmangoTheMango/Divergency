using Divergency.Assets.Particles;
using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Hostile
{
    internal class FloatingBalls : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("BALLS HAHA SEX PENIS HAHA HA SEX");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25; // in SetStaticDefaults()
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        private int Counter { get { return (int)Projectile.ai[1]; } set { Projectile.ai[1] = value; } }
        private bool FistPhase { get { return (int)Projectile.ai[1] >= 0; } }
        private int Timer;

  
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
                
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.maxAI = 5;
        }

        public override void AI()
        {
            Timer++; 
            Projectile.rotation += Projectile.velocity.Length() * (Projectile.direction * 0.04f);

            if (Timer == 1)
            {
                ParticleManager.NewParticle(Projectile.Center, new Vector2(0, 0), ParticleManager.NewInstance<BloomParticleProjectile>(), new Color(0.50f, 2f, 0.5f, 0), 0.1f, Projectile.whoAmI, Layer: Particle.Layer.BeforeProjectiles);

            }

            Vector2 speed = Main.rand.NextVector2Unit() * 0.1f;

            ParticleManager.NewParticle(Projectile.Center, speed * 10, ParticleManager.NewInstance<StarParticle>(), new Color(0.50f, 2f, 0.5f, 0), 0.3f, Projectile.whoAmI);
            NPC owner = Main.npc[(int)Projectile.ai[4]];

            if (Counter == 0)
            { // first itteration of ai
                int dir = Main.rand.Next(2)*2-1;

                Projectile.position = owner.Center + new Vector2(20f, 0f)* dir;
                Projectile.velocity = new Vector2(dir*6f, (Main.rand.NextFloat()*2-1)*9f);
                Projectile.velocity.Normalize();
                Projectile.velocity *= 12f;
            }

            if (Counter >= 0)
                Counter++;
            else
                Counter--;

            if (FistPhase)
                FirstPhaseAI();
            else
                SecondPhaseAI();

        }
       
         public Trail trail;
        public Trail whiteTrail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(new Color(79, 214, 126, 25));

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(25f), (p) => Projectile.GetAlpha(new Color(79, 214, 126, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                whiteTrail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(15f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            whiteTrail.Draw(Projectile.oldPos);

            return false;
        }
        private void FirstPhaseAI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[4]];

            float speed = Projectile.velocity.Length();

            Vector2 flaotPoint = owner.Center + new Vector2(0f, -160f);
            Vector2 floatPointDiff = flaotPoint - Projectile.Center;

            // apply slow at center
            if ((floatPointDiff).Length() < 120f && Main.rand.NextBool())
                speed *= 0.83f;

            if (speed < 0.02f)
                speed = 0f;


            floatPointDiff.Normalize();
            Projectile.velocity += floatPointDiff;
            Projectile.velocity.Normalize();
            Projectile.velocity *= speed;

            if (speed == 0)
            {
                Counter = -1;
                Projectile.ai[2] = owner .Center.X - Projectile.Center.X;
                Projectile.velocity = new Vector2(0, 10f);
            }
        }

        private void SecondPhaseAI()
        {
            Projectile.width = Projectile.height = 20;

            if (Projectile.ai[0] != -1)
            {
                NPC owner = Main.npc[(int)Projectile.ai[4]];
                Vector2 targetPosition = Main.player[(int)Projectile.ai[3]].Center; // should be player
                Vector2 vecForRot = targetPosition - Projectile.Center;

                float rot = vecForRot.ToRotation() + MathHelper.PiOver2;
                targetPosition += new Vector2(MathF.Cos(rot) * Projectile.ai[2], MathF.Sin(rot) * Projectile.ai[2]);

                float speed = Projectile.velocity.Length();
                Vector2 targetPointDiff = targetPosition - Projectile.Center;

                if (Counter == -17)
                {
                    Projectile.ai[0] = -1;
                    Counter = -1;
                }

                targetPointDiff.Normalize();
                Projectile.velocity += targetPointDiff * 5f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= speed;
            }
            else
            {
                if (Counter == -120)
                {
                    Projectile.Kill();
                }
                if (Counter == -30)
                {
                    Projectile.tileCollide = true;
                }
            }
        }   
    }

}

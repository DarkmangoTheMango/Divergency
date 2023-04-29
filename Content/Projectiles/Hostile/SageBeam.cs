using Divergency.Content.Particles;
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
    internal class SageBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Sage Beam");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25; // in SetStaticDefaults()
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
       // private int Counter { get { return (int)Projectile.ai[1]; } set { Projectile.ai[1] = value; } }
        //private bool FistPhase { get { return (int)Projectile.ai[1] >= 0; } }
        private int Timer;
        private Vector2 unmodifiedVelocity;


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
            Projectile.rotation += Projectile.velocity.Length() * (Projectile.direction * 0.01f);

            if (Timer == 1)
            {
                ParticleManager.NewParticle(Projectile.Center, new Vector2(0, 0), ParticleManager.NewInstance<BloomParticleProjectile>(), new Color(0.50f, 2f, 0.5f, 0), 0.1f, Projectile.whoAmI, Layer: Particle.Layer.BeforeProjectiles);

            }
            if (Timer == 1)
            {
                unmodifiedVelocity = Projectile.velocity;
            }

            Projectile.velocity *= 1.001f;
            Projectile.velocity = unmodifiedVelocity.RotatedBy(Math.Sin((Timer) * 0.2f) * 0.2f);

            Vector2 speed = Main.rand.NextVector2Unit() * 0.1f;

            ParticleManager.NewParticle(Projectile.Center, speed * 10, ParticleManager.NewInstance<StarParticle>(), new Color(0.50f, 2f, 0.5f, 0), 0.3f, Projectile.whoAmI);
            NPC owner = Main.npc[(int)Projectile.ai[4]];

    

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

            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/MotionTrail").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(25f), (p) => Projectile.GetAlpha(new Color(79, 214, 126, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                whiteTrail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }

           // trail.Draw(Projectile.oldPos);
            whiteTrail.Draw(Projectile.oldPos);

            return false;
        }
    }


}

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
    internal class DirectionalAttack2 : ModProjectile
    {
        private int Counter { get { return (int)Projectile.ai[2]; } set { Projectile.ai[2] = value; } }
        private int Direction { get { return (int)Projectile.ai[1] % 4;} }
        private Vector2[] Directions = new Vector2[] {
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(-1, 0),
            new Vector2(0, -1),
        };

        public static int maxAI = 3;

        private int Delay = 65;
        private float StartDistance = 350;
        private int Timer;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("i have taken your family hostage");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25; // in SetStaticDefaults()
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }   
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 35;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                ParticleManager.NewParticle(Projectile.Center, new Vector2(0, 0), ParticleManager.NewInstance<BloomParticleProjectile>(), new Color(0.50f, 2f, 0.5f, 0), 0.1f, Projectile.whoAmI, Layer: Particle.Layer.BeforeProjectiles);

            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
            Projectile.spriteDirection = Projectile.direction;
            Player owner  = Main.player[(int)Projectile.ai[0]];

            if (Counter == Delay)
            {
                Counter = -1;
                Projectile.velocity = Directions[Direction] * 20f;

                for (int i = 0; i < 10; i++)
                {

                    Vector2 dir = Main.rand.NextVector2Unit() * 0.1f;

                    ParticleManager.NewParticle(Projectile.Center, dir * 10, ParticleManager.NewInstance<StarParticle>(), new Color(0.50f, 2f, 0.5f, 0), 0.3f, Projectile.whoAmI);
                }
            }
            else if (Counter >= 0)
            {
                Counter++;

                Vector2 dir = Directions[Direction];
                Projectile.rotation = dir.ToRotation()+MathHelper.PiOver2;
                Projectile.Center = owner .Center + -dir * StartDistance;

                // this is stand still
            }
            else if (Counter < 0)
            {
                Counter--;
                if (Counter < -120)
                    Projectile.Kill();

                // this is stand released
            }
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
    }
}

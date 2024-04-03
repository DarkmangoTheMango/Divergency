using Divergency.Content.Particles;
using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Content.Dusts;
using System.Threading;

namespace Divergency.Content.Projectiles.Melee
{
    public class SacrifeBallThing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Life Orb");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(14);
            Projectile.scale = 1f;
            Projectile.damage = 0;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player player = Main.LocalPlayer;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), new Vector2(Main.rand.NextFloat(-0.4f, 0.4f)), 0, Color.DarkRed, 0.3f).noGravity = true;

            if (fly)
            {
                timer++;
                if (timer == 60 )
                {
                    fly = false;
                }
            }
            else
            {
                Projectile.Move(player.Center, 10);
                if (Projectile.active && Projectile.Hitbox.Intersects(player.Hitbox) && !player.dead)
                {
                    Projectile.Kill();
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { Volume = 0.8f, MaxInstances = 3 });

             

                }
                if (!Particlespawned)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        ParticleManager.NewParticle(Projectile.Center, new Vector2(0, 0), ParticleManager.NewInstance<BloomParticleProjectile>(), new Color(2f, 0f, 0.1f, 0), 0.03f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);

                    }
                    Particlespawned = true;
                }
            }


        }

        public Trail trail;
        public Trail trail2;
        public bool Particlespawned { get; private set; }
        public bool fly = true;
        private int timer;

        public override bool PreDraw(ref Color lightColor)
        {



            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(25f), (p) => Projectile.GetAlpha(new Color(220, 0, 30, 100)));
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

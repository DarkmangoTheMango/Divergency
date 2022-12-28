using Divergency.Assets.Particles;
using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles
{
    public class LifeOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Orb");

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
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player player = Main.LocalPlayer;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.Move(player.Center, 22);
            if (Projectile.active && Projectile.Hitbox.Intersects(player.Hitbox) && !player.dead)
            {
                player.Heal(2);
                Projectile.Kill();
                for (int i = 0; i < 10; i++)
                {
                    Vector2 dir = Main.rand.NextVector2Unit() * 0.1f;

                    ParticleManager.NewParticle(Projectile.Center, dir * 20, ParticleManager.NewInstance<StarParticle>(), new Color(0.50f, 2f, 0.5f, 0), 0.4f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);

                }

            }
            if (!Particlespawned)
            {
                for (int i = 0; i < 2; i++)
                {
                    //ParticleManager.NewParticle(player.Center + (Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(30f, 110f)), new Vector2(0f, Main.rand.NextFloat(1, 5)).RotatedBy(Projectile.rotation), ParticleManager.NewInstance<BloomParticleProjectile>(), new Color(0.50f, 2f, 0.5f, 0), 0.2f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);

                }
                Particlespawned = true;
            }

        }

        public Trail trail;
        public Trail trail2;

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
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(6f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            trail2.Draw(Projectile.oldPos);

            return false;
        }
    }
}

using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Divergency.Content.Projectiles.Ranged
{
    public class CoreShrapnel : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.width = Projectile.height = 4;
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            if (Projectile.ai[0] >= 15f) { Projectile.velocity *= 0.95f; }

            if (Projectile.velocity.Length() < 0.1f) { Projectile.Kill(); }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            return base.OnTileCollide(oldVelocity);
        }

        public TrailRenderer trail;

        public override bool PreDraw(ref Color lightColor)
        {
            var trailTexture = Request<Texture2D>("Divergency/Assets/Textures/Trails/Default").Value;

            for (int i = 0; i < 3; i++)
            {
                if (trail == null)
                {
                    trail = new TrailRenderer(trailTexture, TrailRenderer.DefaultPass, (p) => new Vector2(8f), (p) => Projectile.GetAlpha(Color.LightYellow) * (float)Math.Pow(1f - p, 2f));
                    trail.drawOffset = Projectile.Size / 2f;
                }

                trail.Draw(Projectile.oldPos);
            }

            return true;
        }
    }
}

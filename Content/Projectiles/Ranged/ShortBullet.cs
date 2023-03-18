using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace Divergency.Content.Projectiles.Ranged
{
    public class LivingCoreShrapnel : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Shrapnel");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.width = Projectile.height = 8;
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 5;
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

        public Trail trail;
        public Trail whiteTrail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                whiteTrail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 247, 179, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }

            whiteTrail.Draw(Projectile.oldPos);

            return true;
        }
    }
}

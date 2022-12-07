using Divergency.Content.Dusts;
using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Divergency.Common.Players;

namespace Divergency.Content.Projectiles.Ranged
{
    public class CoreGrenade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.width = Projectile.height = 22;
            Projectile.scale = 1f;
            DrawOffsetX = 4;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (bounced)
            {
                Projectile.velocity *= 0.9f;
                Projectile.rotation += Projectile.velocity.Length() * (0.05f * Projectile.direction);

                Projectile.timeLeft -= 20;
            }
            else
            {
                Projectile.ai[0] += 1f;

                if (Projectile.ai[0] >= 30f)
                {
                    Projectile.ai[0] = 30f;
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;

                    Projectile.rotation += Projectile.velocity.Length() * (0.05f * Projectile.direction);
                }
                else
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }

                if (Projectile.velocity.Y > 16f) { Projectile.velocity.Y = 16f; }
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity = 8;

            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, Main.rand.NextVector2CircularEdge(1f, 1f) * 5, 0, default, 3f);
                dust.noGravity = true;

                dust = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Main.rand.NextVector2Circular(1f, 1f) * 10f, 0, default, 3f);
                dust.noGravity = true;

                dust = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, Projectile.velocity.SafeNormalize(Vector2.One) * Main.rand.NextFloat(-1f, -4f), 0, default, 2f);
                dust.noGravity = true;
            }

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<Smoke>(), Main.rand.NextVector2CircularEdge(1f, 1f) * 5, 0, Color.Gold, 3f);
                Dust.NewDustPerfect(Projectile.Center, DustType<Smoke>(), Projectile.velocity.SafeNormalize(Vector2.One) * Main.rand.NextFloat(-1f, -4f), 0, default, 2f);
            }

            Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, default(Vector2), Mod.Find<ModGore>("CoreGrenadeGore1").Type, 1f);

            int NumProjectiles = 3 + Main.rand.Next(3);

            for (int i = 0; i < NumProjectiles; i++)
            {
                Vector2 newVelocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(10));
                newVelocity *= 1f - Main.rand.NextFloat(0.5f);

                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.position, newVelocity * 3, ProjectileType<CoreShrapnel>(), (int)(Projectile.damage * 0.8f), 0f, player.whoAmI);
            }
            
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/LivingCoreShotgun"), Projectile.Center);
        }

        bool bounced = false;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) { Projectile.velocity.X = -oldVelocity.X; }
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) { Projectile.velocity.Y = -oldVelocity.Y; }

            bounced = true;

            return false;
        }

        public TrailRenderer trail;

        public override bool PreDraw(ref Color lightColor)
        {
            var trailTexture = Request<Texture2D>("Divergency/Assets/Textures/Trails/Default").Value;

            if (trail == null)
            {
                trail = new TrailRenderer(trailTexture, TrailRenderer.DefaultPass, (p) => new Vector2(5f), (p) => Projectile.GetAlpha(Color.Lime) * (float)Math.Pow(1f - p, 2f));
                trail.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);

            return true;
        }
    }
}

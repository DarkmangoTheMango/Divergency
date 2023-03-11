using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Ranged
{
    public class SapphireBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sapphire Bullet");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(14);
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 5;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.ai[0] += 1f;

            if (Projectile.ai[0] >= 15f)
            {
                Projectile.velocity *= 0.95f;
                Projectile.scale *= 0.98f;
                Projectile.alpha += 5;
            }

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.Center);

            for (int k = 0; k < 2; k++)
            {
                Vector2 perturbedSpeed = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30));
                float scale = 1f - (Main.rand.NextFloat() * 0.9f);

                Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.GemSapphire, (perturbedSpeed * scale) * -0.5f, 0, default, 1f);
                dust.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.Center);

            for (int k = 0; k < 2; k++)
            {
                Vector2 perturbedSpeed = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30));
                float scale = 1f - (Main.rand.NextFloat() * 0.9f);

                Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.GemSapphire, (perturbedSpeed * scale) * -0.5f, 0, default, 1f);
                dust.noGravity = true;
            }

            return base.OnTileCollide(oldVelocity);
        }

        public Trail trail;
        public Trail whiteTrail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Projectiles/Ranged/SapphireBulletGlow").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = new Color(0, 94, 255, 100);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            texture = TextureAssets.Projectile[Projectile.type].Value;

            frameHeight = texture.Height / Main.projFrames[Projectile.type];
            frameY = frameHeight * Projectile.frame;

            sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            origin = sourceRectangle.Size() / 2f;
            position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            color = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(0, 94, 255, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                whiteTrail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(158, 249, 255, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            whiteTrail.Draw(Projectile.oldPos);

            return false;
        }
    }
}

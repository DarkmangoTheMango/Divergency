using Divergency.Common.Helpers;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles
{
    public class SavageDagger : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Savage Dagger");

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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ShredBlood>(), new Vector2(Main.rand.NextFloat(-0.4f, 0.4f)), 0, default, 0.5f).noGravity = true;

            Projectile.ai[0] += 1f;

            if (Projectile.ai[0] >= 15f)
            {
                Projectile.velocity *= 0.95f;
                Projectile.scale *= 0.98f;
                Projectile.alpha += 5;
            }

            if (Projectile.alpha >= 255) { Projectile.Kill(); }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.Center);

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Impacts/Fleshy"), target.Center);
            target.AddBuff(ModContent.BuffType<Shred>(), 60);

            for (int i = 0; i < 10; i++) { Dust.NewDustPerfect(target.Center, ModContent.DustType<ShredBlood>(), Main.rand.NextVector2Circular(1f, 1f) * 5f, 0, default, 2f).noGravity = true; }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.Center);

            for (int k = 0; k < 2; k++)
            {
                Vector2 perturbedSpeed = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30));
                float scale = 1f - (Main.rand.NextFloat() * 0.9f);

                Dust dust = Dust.NewDustPerfect(Projectile.position, ModContent.DustType<ShredBlood>(), (perturbedSpeed * scale) * -0.5f, 0, default, 1f);
                dust.noGravity = true;
            }

            return base.OnTileCollide(oldVelocity);
        }

        public Trail trail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Projectiles/SavageDaggerGlow").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = new Color(222, 0, 13, 100);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            texture = texture = TextureAssets.Projectile[Projectile.type].Value;

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
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(222, 0, 13, 100)));
                trail.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);

            return false;
        }
    }
}

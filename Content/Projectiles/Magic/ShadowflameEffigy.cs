using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Magic
{
    public class ShadowflameEffigy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Shadowflame Effigy");

            Main.projFrames[Projectile.type] = 4;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 0.9f;
            Projectile.Size = new Vector2(32);
            Projectile.alpha = 255;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X >= 0f) ? 1 : -1;

            Lighting.AddLight(Projectile.Center, new Color(241, 150, 255).ToVector3());

            Projectile.alpha -= 10;

            if (Projectile.alpha <= 0) { Projectile.alpha = 0; }

            if (Projectile.spriteDirection == -1) { Projectile.rotation += MathHelper.Pi; }

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4) { Projectile.frame = 0; }
            }

            if (Projectile.ai[1] == 1)
            {
                Projectile.scale = 1.2f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.DD2_SkeletonDeath, Projectile.position);

            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, Main.rand.NextVector2Circular(1f, 1f) * 10, 0, default, 2f);
                dust.noGravity = true;
            }

            if (Projectile.ai[1] == 1)
            {
                CameraSystem.ScreenShake(5);
            }
        }

        public Trail trail;

        float timer;

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1) { spriteEffects = SpriteEffects.FlipHorizontally; }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Color color = Projectile.GetAlpha(lightColor);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            float offsetX = 30f;
            drawOrigin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Shadow").Value;

            for (int k = 0; k < 3; k++)
            {
                if (trail == null)
                {
                    trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(60f), (p) => Projectile.GetAlpha(new Color(220, 82, 255, 100)) * (float)Math.Pow(1f - p, 2f));
                    trail.drawOffset = Projectile.Size / 2f;
                }

                trail.Draw(Projectile.oldPos, timer);
                timer -= 0.01f;
            }

            return false;
        }
    }
}
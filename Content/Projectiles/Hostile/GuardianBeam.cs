using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Hostile
{
    public class GuardianBeam : ModProjectile
    {
        public override void Kill(int timeLeft) => SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.width = Projectile.height = 16;
            Projectile.scale = 1f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        public override void AI() { Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(-0.4f, 0.4f)), 0, default, 1.2f).noGravity = true; }

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
            Color color = Projectile.GetAlpha(new Color(109, 223, 94, 25));

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
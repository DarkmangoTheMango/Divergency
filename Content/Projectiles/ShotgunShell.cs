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
using System;
using System.Security.Permissions;

namespace Divergency.Content.Projectiles
{
    public class ShotgunShell : ModProjectile
    {
        float maxVelocity = 10f;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.Size = new(6);
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.5f;
            Projectile.rotation += Projectile.velocity.X * 0.05f;
            Projectile.velocity.X *= 0.98f;

            if (Projectile.penetrate <= 2)
            {
                Projectile.alpha += 5;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate >= 2)
            {
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    Projectile.velocity.Y = -oldVelocity.Y * 0.5f;

                Projectile.penetrate--;
            }
            else
            {
                Projectile.velocity.Y = 0f;
                Projectile.velocity.X *= 0.9f;
            }
            
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

        }

        public override void Kill(int timeLeft)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            Vector2 origin = rectangle.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Color drawColor = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture, drawPosition, rectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}

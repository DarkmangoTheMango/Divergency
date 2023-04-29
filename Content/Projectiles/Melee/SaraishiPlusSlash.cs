using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Melee
{
    public class SaraishiPlusSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Aku Saraishi Slash");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 2f;
            Projectile.Size = new Vector2(90);
            Projectile.alpha = 255;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X >= 0f) ? 1 : -1;

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 15f)
            {
                Projectile.scale *= 0.9f;
                Projectile.velocity *= 0.7f;

                Projectile.alpha += 25;
                if (Projectile.alpha >= 255) { Projectile.Kill(); }
            }
            else
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha <= 0) { Projectile.alpha = 0; }
            }
        }

         public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[0] = 15;
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<SaraishiStrike>(), Projectile.damage, 0f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(new Color(255, 255, 255, 100));

            if (Projectile.ai[1] == 1)
            {
                Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.FlipVertically, 0);
            }
            else
            {
                Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}
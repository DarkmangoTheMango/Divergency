using Divergency.Common.Helpers;
using Divergency.Content.Items.Weapons.LivingCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Magic
{
    public class LivingBolt : ModProjectile
    {
        float timer;

        public override void Kill(int timeLeft) => SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(54);
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Projectile.velocity.RotatedByRandom(0.1f), 0, new Color(109, 223, 94), 1.2f).noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 5) { Projectile.Kill(); }
            else
            {
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) { Projectile.velocity.X = -oldVelocity.X; }
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) { Projectile.velocity.Y = -oldVelocity.Y; }

                int numberOfDusts = 20;
                float radius = 2;

                for (int i = 0; i < numberOfDusts; i++) { Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfDusts * i)) * radius, 0, new Color(109, 223, 94), 1.2f).noGravity = true; }

                Projectile.oldPos[0] = Projectile.position;
            }

            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 16;

            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Vector2 position = target.Center + Main.rand.NextVector2Circular(1f, 1f) * target.width;

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), position, Vector2.Zero, ModContent.ProjectileType<LivingCoreSpearDamage>(), 0, 0f, Projectile.owner);

            for (int k = 0; k < 5; k++)
            {
                float speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.PortalBoltTrail, new Vector2(0f, speed), 0, new Color(109, 223, 94), 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.PortalBoltTrail, new Vector2(speed, 0f), 0, new Color(109, 223, 94), 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.PortalBoltTrail, new Vector2(0f, -speed), 0, new Color(109, 223, 94), 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.PortalBoltTrail, new Vector2(-speed, 0f), 0, new Color(109, 223, 94), 1.2f).noGravity = true;
            }
        }

        public Trail trail;
        public Trail whiteTrail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Star").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Color color = Projectile.GetAlpha(new Color(109, 223, 94, 0));

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, timer, origin, Projectile.scale + (float)Math.Sin(timer) * 0.5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, -timer / 2, origin, (Projectile.scale * 0.6f) + (float)Math.Sin(timer) * 0.5f, SpriteEffects.None, 0);

            timer += 0.1f;

            if (timer >= MathHelper.Pi)
            {
                timer = 0f;
            }

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                position = Projectile.oldPos[k] + origin - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                color = Projectile.GetAlpha(new Color(109, 223, 94, 0)) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(texture, position, sourceRectangle, color, 0, origin, Projectile.scale - (k * 0.05f) - 0.2f, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
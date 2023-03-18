using Divergency.Content.Particles;
using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Content.Items.Weapons.LivingCore;

namespace Divergency.Content.Projectiles.Magic
{
    public class LivingLeaf : ModProjectile
    {
        public override void Kill(int timeLeft) => SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Bolt");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(16);
            Projectile.scale = 0.3f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, new Vector2(Main.rand.NextFloat(-0.4f, 0.4f)), 0, default, 1.2f).noGravity = true;

            Projectile.rotation += 0.1f;

            Projectile.velocity.Y += 0.1f;
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

                for (int i = 0; i < numberOfDusts; i++) { Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfDusts * i)) * radius, 0, default, 1.2f).noGravity = true; }

                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Projectile.oldPos[k] = Projectile.position;
                }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Vector2 position = target.Center + Main.rand.NextVector2Circular(1f, 1f) * target.width;

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), position, Vector2.Zero, ModContent.ProjectileType<LivingCoreSpearDamage>(), 0, 0f, Projectile.owner);

            for (int k = 0; k < 5; k++)
            {
                float speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(0f, speed), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(speed, 0f), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(0f, -speed), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(-speed, 0f), 0, default, 1.2f).noGravity = true;
            }
        }

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
            Color color = Projectile.GetAlpha(new Color(109, 223, 94, 0));

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(40f), (p) => Projectile.GetAlpha(new Color(109, 223, 94, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                whiteTrail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            whiteTrail.Draw(Projectile.oldPos);

            return false;
        }
    }
}
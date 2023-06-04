using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Magic
{
    public class CorescillationProj : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";
        public override void SetStaticDefaults()
        {
            //.setdefault("Shadowflame Effigy");


            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
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

            Projectile.alpha -= 20;

            if (Projectile.alpha <= 0) { Projectile.alpha = 0; }

            if (Projectile.spriteDirection == -1) { Projectile.rotation += MathHelper.Pi; }

            

         
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);

            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemEmerald, Main.rand.NextVector2Circular(1f, 1f) * 30, 0, default, 2f);
                 Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(1f, 1f) * 10, 0, new Color(109, 223, 94), 1f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 10, 0, new Color(109, 223, 94), 1f);


                dust.noGravity = true;
            }
            DivergencyDraw.SpawnRing(Projectile.Center, new Color(109, 223, 94));
           
                player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 4;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<LivingExplosion>(), Projectile.damage,Projectile.knockBack / 2, Projectile.owner);
        }

        public Trail trail;

        float timer;
        float trailtimer;
        public override bool PreDraw(ref Color lightColor)
        {
     
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Beam").Value;

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
            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Light").Value;

            for (int k = 0; k < 3; k++)
            {
                if (trail == null)
                {
                    trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(60f), (p) => Projectile.GetAlpha(new Color(109, 223, 94, 100)) * (float)Math.Pow(1f - p, 2f));
                    trail.drawOffset = Projectile.Size / 2f;
                }

                trail.Draw(Projectile.oldPos, trailtimer);
                trailtimer -= 0.01f;
            }

            return true;
        }
        public class LivingExplosion : ModProjectile
        {
            public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalLeafShot;

            public override void SetDefaults()
            {
                Projectile.penetrate = -1;
                Projectile.DamageType = DamageClass.Magic;
                Projectile.friendly = true;
                Projectile.hostile = false;

                Projectile.width = Projectile.height = 130;
                Projectile.scale = 1f;
                Projectile.alpha = 255;
                Projectile.penetrate = -1;
                Projectile.tileCollide = false;
                Projectile.ignoreWater = false;


                Projectile.timeLeft = 5;
            }
        }
    }
}
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
using Divergency.Content.Tiles.LivingGrove.CorePuzzle;
using System.ComponentModel.DataAnnotations;
using Terraria.Enums;

namespace Divergency.Content.Projectiles.Hostile
{
    internal class CircleBounceAttack : ModProjectile
    {
        private int TargetPlayer { get { return (int)Projectile.ai[0]; } }
        private bool WithinCircle { get { return Projectile.ai[1] == 0; } set { Projectile.ai[1] = value ? 0f : 1f; } }

        public override void SetStaticDefaults()
        {
            //.setdefault("skill issue");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25; // in SetStaticDefaults()
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }   
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 35;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2400;
        }

        static private float range = 500f;

        public override void AI()
        {
            Player target = Main.player[TargetPlayer];

            if (Vector2.Distance(target.Center, Projectile.Center) > range) // is not in circle
            {
                if (WithinCircle) // if it was in last frame
                {
                    WithinCircle = false;
                    Projectile.velocity = Vector2.Normalize(target.Center + Main.rand.NextVector2Circular(range * 0.1f, range * 0.1f)
                        - Projectile.Center) * Projectile.velocity.Length();
                }
            }
            else
            {
                WithinCircle = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
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
            Color color = Projectile.GetAlpha(new Color(79, 214, 126, 25));

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

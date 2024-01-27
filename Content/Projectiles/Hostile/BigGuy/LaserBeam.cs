using Divergency.Content.Dusts;
using Divergency.Content.Events.LivingCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Hostile.BigGuy
{
    public class LaserBeam : ModProjectile
    {
        private const int maxDistance = 2000;
        private const int beamWidth = 20;

        private int distance = 0;

        private Player targetPlayer => Main.player[(int)Projectile.ai[0]];
        private NPC owner => Main.npc[(int)Projectile.ai[1]];
        private Vector2 origin {
            get {
                Vector2 pos = owner.position;

                Vector2 eyeOffset = ownerEyeOffset[0];//[(owner.frame.Y / 58)]; // so like, this shit no work...
                if (owner.spriteDirection == -1)
                    eyeOffset.X = 374 - eyeOffset.X;

                return pos + eyeOffset;
            }
        }

        private Vector2[] ownerEyeOffset = new Vector2[] {
            new Vector2(147, 49),
            new Vector2(141, 56),
            new Vector2(133, 63),
            new Vector2(137, 59),
            new Vector2(141, 56),
            new Vector2(151, 147),
        };

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.width = Projectile.height = 64;
            Projectile.scale = 1f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Base = (Texture2D)ModContent.Request<Texture2D>(Texture);

            Main.EntitySpriteDraw(Base, origin - Main.screenPosition, new Rectangle(0, 0, (distance < 64 ? 64 : distance), 64), Color.White, Projectile.rotation, new Vector2(0, 32), 1f, SpriteEffects.FlipHorizontally);

            return false;
        }

        public override void AI()
        {
            Projectile.timeLeft = 10;

            // make it rotate smootly towards player...
            Projectile.rotation = ((targetPlayer.Center - origin) / 400).ToRotation();

            CalculateDistance();
            CastLights();
            //Projectile.Kill();

            Vector2 end = origin + Projectile.rotation.ToRotationVector2() * (distance < 64 ? 64 : distance);

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustPerfect(end, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 10, 0, Color.LimeGreen, 2f) ;
                dust.noGravity = true;

            }
        }
        private void CastLights()
        {
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Utils.PlotTileLine(origin, origin + Projectile.rotation.ToRotationVector2() * (distance < 64 ? 64 : distance), beamWidth, DelegateMethods.CastLight);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), origin,
                origin + Projectile.rotation.ToRotationVector2() * (distance < 64 ? 64 : distance), beamWidth, ref point);
        }

        private void CalculateDistance()
        {
            for (distance = 0; distance <= maxDistance; distance += 5)
            {
                Vector2 end = origin + Projectile.rotation.ToRotationVector2() * distance;
                if (!Collision.CanHit(origin, 1, 1, end, 1, 1))
                {
                    distance -= 5;
                    return;
                }
            }
        }
    }
}

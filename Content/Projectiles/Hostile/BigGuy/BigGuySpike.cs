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
using System.Threading;
using Terraria.DataStructures;

namespace Divergency.Content.Projectiles.Hostile.BigGuy
{
    internal class BigGuySpike : ModProjectile
    {


        public override void SetStaticDefaults()
        {
            //.setdefault("skill issue");
            Main.projFrames[Projectile.type] = 8;
          
        }
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 75;
            Projectile.damage = 50;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1600;
            
        }
        Rectangle hitboxExtension;
        public int timer;
        public int maxInstances;
        public override void AI()
        {
            if (!initialized)
            {
                Projectile.frame = -1;
                initialDamage = Projectile.damage;
                Projectile.damage = 0;
               if (Projectile.ai[1] == 1)
                {
                    SpikeRetreatCounter = 210;
                    maxInstances = 300;
                }

                if (Projectile.ai[2] == 2)
                {
                    SpikeRetreatCounter = 230;
                    maxInstances = 1;
                }


                initialized = true;
            }
            Projectile.velocity.Y += 10;
            if (collided)
            {
                timer++;
                 Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                int frameHeight = texture.Height / Main.projFrames[Projectile.type];
                hitboxExtension = new Rectangle((int)Projectile.Top.X, (int)Projectile.Top.Y - 165, texture.Width / 5, 240);

            }
            if (timer <= 120 && timer > 0)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] == 30)
                Dust.QuickDustLine(Projectile.BottomLeft,Projectile.BottomRight, DustID.GemEmerald, Color.LimeGreen);

            }
            if (timer == 120)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/groundattack") with { Pitch = Main.rand.NextFloat(-0.3f, 0.3f), MaxInstances = maxInstances }, Projectile.Center);

            }
            if (timer > 120)
            {
                Projectile.damage = initialDamage;
                if (hitboxExtension.Intersects(Main.LocalPlayer.Hitbox) && Projectile.frame >= 3 && Projectile.frame <= 6)
                {
                    Main.LocalPlayer.Hurt(PlayerDeathReason.ByCustomReason("womp womp"), Projectile.damage, 0);
                }
                if (timer <= 149 || timer >= SpikeRetreatCounter)
                Projectile.frameCounter++;
            }
            
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8) { Projectile.Kill(); }
            }
            if (Projectile.frame == 8)
            {
                Projectile.Kill();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            collided = true;
            return false;
        }
        public Trail trail;
        public Trail whiteTrail;
        private bool collided;
        private int initialDamage;
        private bool initialized;

        public int SpikeRetreatCounter = 400;

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

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(50f, -82), sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

           
            return false;
        }
    }
}

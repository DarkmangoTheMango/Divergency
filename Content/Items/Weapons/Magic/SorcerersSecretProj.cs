using Divergency.Common.Helpers;
using Divergency.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Magic
{
    public class SorcerersSecretProj : ModProjectile
    {
        public int HitEnemy;
        public NPC BackTarget;
        public int FlyBack = 0;
        public bool ParticleSpawned;
        public float velY;
        public Vector2 unmodifiedVelocity;
        public bool TimerActive = false;
        public bool SoundPlayed;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Oculomeye");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
  
        }

        public override void SetDefaults()
        {
            Projectile.damage = 3;
            Projectile.width = 32;
            Projectile.height = 32;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.timeLeft = 200;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float Timer2
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            TimerActive = true;
            Projectile.alpha = 150;
            HitEnemy++;
    
                BackTarget = target;
            
            if (HitEnemy == 2)
            {
                Projectile.velocity *= 0.95f;
            }
            Projectile.damage *= 2;
        }

        public override void AI()
        {
            Vector3 RGB = new Vector3(1.28f, 0, 1.24f);
            float multiplier = 0.5f;
            float max = 1f;
            float min = 0.5f;
            RGB *= multiplier;
            if (RGB.X > max)
            {
                multiplier = 0.5f;
            }
            if (RGB.X < min)
            {
                multiplier = 1.5f;
            }
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            if (TimerActive)
            {
                Timer2++;
            }
     

            Timer++;
            Player player = Main.player[Projectile.owner];

            if (!ParticleSpawned)
            {
                ParticleSpawned = true;
            }

            if (Timer == 1)
            {
                unmodifiedVelocity = Projectile.velocity;
            }

            Projectile.velocity = unmodifiedVelocity.RotatedBy(Math.Sin((Timer - 40) * 0.3f) * 0.3f);

            if (Timer2 >= 60 && HitEnemy >= 1)
            {
                FlyBack++;
            }
            if (FlyBack >= 1)
            {
                if (BackTarget != null && BackTarget.active && HitEnemy == 1)
                {
                    Projectile.velocity = Vector2.Normalize(Projectile.DirectionTo(BackTarget.Center)) * 40f;
                    if (!SoundPlayed)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Items/thej")
                        {
                            Volume = 0.9f,
                            PitchVariance = 0.2f,
                            
                        });

                        SoundPlayed = true;
                    }
                }
            }
            if (FlyBack >= 2)
            {
                Projectile.penetrate = 1;
                Projectile.velocity = Projectile.oldVelocity;
                Projectile.velocity *= 0.97f;
                if (Projectile.timeLeft <= 40)
                {
                    Projectile.alpha += 10;
                }
            }
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;

            Projectile.rotation = Projectile.velocity.ToRotation();
            // Since our sprite has an orientation, we need to adjust rotation to compensate for the draw flipping
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
                // For vertical sprites use MathHelper.PiOver2
            }
                   // ParticleManager.NewParticle(Projectile.Center, Projectile.velocity * Main.rand.NextFloat(10, 25), ParticleManager.NewInstance<StarParticle>(), new Color(0.50f, 2f, 0.5f, 0), 0.3f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);

            if (FlyBack == 0)
            {
                Projectile.frame = 0;
            }
            else
            {
                Projectile.frame = 1;
            }
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 7; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                ParticleManager.NewParticle(Projectile.Center, speed * Main.rand.NextFloat(10, 25), ParticleManager.NewInstance<StarParticle>(), new Color(153, 50, 204, 0), 0.3f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);
            }
        }
       
        public Trail trail;
        public Trail trail2;


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(30f), (p) => Projectile.GetAlpha(new Color(153, 50, 204, 100)));
                trail.drawOffset = Projectile.Size / 2f;
            }
            if (trail2 == null)
            {
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            trail2.Draw(Projectile.oldPos);

            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                // Getting texture of projectile
                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

                // Calculating frameHeight and current Y pos dependence of frame
                // If texture without animation frameHeight is always texture.Height and startY is always 0
                int frameHeight = texture.Height / Main.projFrames[Projectile.type];
                int startY = frameHeight * Projectile.frame;

                // Get this frame on texture
                Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

                // Alternatively, you can skip defining frameHeight and startY and use this:
                // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

                Vector2 origin = sourceRectangle.Size() / 2f;

                // If image isn't centered or symmetrical you can specify origin of the sprite
                // (0,0) for the upper-left corner
                float offsetX = 0f;
                origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

                // If sprite is vertical
                // float offsetY = 20f;
                // origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);

                // Applying lighting and draw current frame

                Color drawColor = Projectile.GetAlpha(lightColor);
                Main.EntitySpriteDraw(texture,
                    Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                    sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }
}
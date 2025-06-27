using Divergency.Common.Helpers;
using Divergency.Content.Items.Weapons.Summoner;
using Divergency.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Summoner.Minions
{ 
    public class Corewhack_Summon : ModProjectile
    {
        private bool CanShoot
        {
            get { return Projectile.ai[0] == 0; }
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true; //This is necessary for right-click targeting
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // in SetStaticDefaults()
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0.5f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.1f;
        }

        public override void AI()
        {
            Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);
            float multiplier = 0.3f;
            float max = 2.25f;
            float min = 1.0f;
            RGB *= multiplier;
            if (RGB.X > max)
            {
                multiplier = 0.2f;
            }
            if (RGB.X < min)
            {
                multiplier = 0.5f;
            }
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            if (Projectile.ai[0] > 0)
                Projectile.ai[0]--;

            Projectile.timeLeft = 100;
            Player owner  = Main.player[Projectile.owner];

            if (!owner .HasBuff<CorewhackBuff>())
            {
                Projectile.Kill();
            }

            int target = owner .MinionAttackTargetNPC;
            Vector2 offest = Vector2.Zero;
            if (target == -1)
            {
                offest = owner.Center;
                drawTimer = 0;
            }
            else
                offest = Main.npc[target].Center;

            int totalMinions = owner .ownedProjectileCounts[Projectile.type];

            int minionPos = owner .numMinions; // Projectile.minionPos + 1
            if (target >= 1)
            {
                float minionOffset = (float)minionPos / (float)totalMinions * MathF.PI * 2 + (float)Main.time / 10f;
                Projectile.rotation = minionOffset;

                Projectile.Center = offest + minionOffset.ToRotationVector2() * (100f + (12f * totalMinions));

                Projectile.Center = offest + minionOffset.ToRotationVector2() * (100f + (12f * totalMinions));
            }
            else
            {
                float minionOffset = (float)minionPos / (float)totalMinions * MathF.PI * 2 + (float)Main.time / 80f;
                Projectile.rotation = minionOffset;

                Projectile.Center = offest + minionOffset.ToRotationVector2() * (target + (4f * totalMinions));

                Projectile.Center = offest + minionOffset.ToRotationVector2() * (60f + (4f * totalMinions));
            }

          

            Vector2 vel = -Projectile.rotation.ToRotationVector2() * 10f;

            if (target != -1 && CanShoot && Collision.CanHit(Projectile, Main.npc[target]))
            {

                // shoot target

                Projectile.ai[0] = 60;

                Terraria.Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel * 2, ModContent.ProjectileType<Corewhack_Summon_Shot>(), Projectile.damage, 0f, Projectile.owner);
            }

        }
        public Trail prim;
        public Trail prim2;
        private int drawTimer;

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];

            int target = owner.MinionAttackTargetNPC;
            Vector2 offest = Vector2.Zero;
            if (target == -1)
                offest = owner.Center;
            else
                offest = Main.npc[target].Center;

            var TrailTex = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;
            Color color = Color.Multiply(new(0.50f, 2.05f, 0.5f, 0), 80);
            if (prim == null)
            {
                prim = new Trail(TrailTex, Trail.DefaultPass, (p) => new Vector2(10f) * (1f - p), (p) => Projectile.GetAlpha(Color.LimeGreen) * 0.9f * (float)Math.Pow(1f - p, 2f));
                prim.drawOffset = Projectile.Size / 2f;
            }
            if (prim2 == null)
            {
                prim2 = new Trail(TrailTex, Trail.DefaultPass, (p) => new Vector2(5f) * (1f - p), (p) => Projectile.GetAlpha(Color.White) * 0.9f * (float)Math.Pow(1f - p, 2f));
                prim2.drawOffset = Projectile.Size / 2f;
            }
            if (target >= 1)
            {
                drawTimer++;
                if (drawTimer > 5)
                {
                    prim.Draw(Projectile.oldPos);
                    prim2.Draw(Projectile.oldPos);
                }
               
            }
            


            return true;
        }
    }

    
    public class Corewhack_Summon_Shot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.6f;

        }
         public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = Main.rand.NextVector2Unit() * 0.5f;
                ParticleManager.NewParticle(Projectile.Center, dir * 5, ParticleManager.NewInstance<StarParticle>(), new Color(0.50f, 2f, 0.5f, 0), 0.5f);
            }
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * (0.05f * Projectile.direction);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
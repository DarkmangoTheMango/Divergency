using Divergency.Common.Helpers;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.Audio;   
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Ranged
{
    internal class LivingCoreArrow : ModProjectile
    {

  

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = Terraria.ID.ProjAIStyleID.Arrow;
            Projectile.damage = 10;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.arrow = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 2;


            Projectile.timeLeft = 240;

      
        }

        public override void AI()
        {
            
            if (Projectile.ai[1] == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dir = Main.rand.NextVector2Unit() * 0.1f;


                    
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 10, 0, default, 0.5f);
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), dir * 2, 0, new Color(109, 223, 94), 0.8f);

                        dust.noGravity = true;
                    


                }
            }

            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Utils.PlotTileLine(Projectile.position, Projectile.position - (Projectile.velocity.SafeNormalize(Vector2.Zero) * 24f), 0, DelegateMethods.CastLight);

            if (Projectile.ai[1] == 0)
            {
                if (Projectile.ai[0] < 0)
                {
                    Projectile.ai[0] -= 1;
                    if (Projectile.ai[0] < -20)
                    {
                        Projectile.ai[0]++;
                        //Projectile.Kill();
                        //Terraria.Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -20f), ModContent.ProjectileType<LivingCoreTip>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 20);

                        Projectile.position -= (Projectile.rotation - MathF.PI / 2).ToRotationVector2() * 60f; // size

                        float rotate = -(Projectile.rotation > MathF.PI ? -(Projectile.rotation - MathF.PI) : Projectile.rotation);

                        if (MathF.Abs(rotate) > 0.2f) // maxRotateVal
                            rotate = rotate / MathF.Abs(rotate) * 0.2f; // maxRotateVal

                        Projectile.rotation += rotate;

                        Projectile.rotation = Projectile.rotation > MathF.PI * 2f ? 0 : Projectile.rotation;

                        Projectile.position += (Projectile.rotation - MathF.PI / 2).ToRotationVector2() * 60f; // size

                        if (Projectile.rotation == 0)
                        {
                            Projectile.ai[0] = 300;
                            Projectile.ai[1] = 1;
                            Projectile.ai[2] = 12;

                            Projectile.tileCollide = true; // might not want this, idk
                        }
                    }
                }
            }
            else if (Projectile.ai[1] == 1)
            { 
                if (Projectile.ai[2] == 0)
                {
                    NPC closestNPC = FindClosestNPC(2000f);
                    float startrot = 0;
                    if (closestNPC != null)
                        startrot = (closestNPC.position - Projectile.position).ToRotation();
                    else
                        startrot = Main.rand.NextFloat() * MathF.PI * 2;

                    Projectile.ai[0]--;

                    Projectile.position -= (Projectile.rotation - MathF.PI / 2).ToRotationVector2() * 24f; // size

                    Projectile.rotation += 0.4f; //Projectile.ai[0]/4;

                    Projectile.position += (Projectile.rotation - MathF.PI / 2).ToRotationVector2() * 24f; // size

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = Main.rand.NextVector2Unit() * Main.rand.NextFloat();
                    }

                    if (Projectile.ai[0] == 0)
                    {
                        Projectile.ai[1] = 2;

                       

                        float rot = 0;
                        if (closestNPC != null)
                            rot = (closestNPC.position - Projectile.position).ToRotation();
                        else
                            rot = Main.rand.NextFloat() * MathF.PI * 2;

                        Projectile.rotation = rot;

                        Projectile.velocity = Projectile.rotation.ToRotationVector2() * 20f;
                        Projectile.rotation += MathF.PI / 2;
                    }
                }
                else
                    Projectile.ai[2]--;
            }
            //else if (Projectile.ai[1] == 2)


        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];

                if (target.CanBeChasedBy())
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center) * (target.boss ? 0.1f : 1);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public override bool PreAI()
        {
            if (Projectile.ai[1] == 0)
            {
                if (!(Projectile.ai[0] >= 0))
                    AI();
                else
                    Projectile.velocity.Y += 0.2f; // drags it down, remove to make it work like normal arrow

                return Projectile.ai[0] >= 0;
            }

            AI();
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dir = (-oldVelocity).RotatedBy(Main.rand.NextFloat() * MathF.PI - MathF.PI / 2);
                dir.Normalize();
                dir *= 0.3f;
            }

            if (Projectile.ai[1] != 0)
                return true;

            Projectile.ai[0] = -1;

            Projectile.position += oldVelocity;
            Projectile.velocity = new Vector2(0, 0);
            Projectile.tileCollide = false;

            return false;
        }
    

        public Trail trail;
        public Trail whiteTrail;


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Projectiles/Ranged/LivingCoreArrowGlow").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Color.White;
            Texture2D texture2 = ModContent.Request<Texture2D>("Divergency/Content/Projectiles/Ranged/LivingCoreArrowGlow2").Value;

            frameHeight = texture.Height / Main.projFrames[Projectile.type];
            frameY = frameHeight * Projectile.frame;

            sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            origin = sourceRectangle.Size() / 2f;
            position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            color = Projectile.GetAlpha(lightColor);


            

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            texture = TextureAssets.Projectile[Projectile.type].Value;

            frameHeight = texture.Height / Main.projFrames[Projectile.type];
            frameY = frameHeight * Projectile.frame;

            sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            origin = sourceRectangle.Size() / 2f;
            position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            color = Color.White;
            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

           

            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(0, 255, 0, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                whiteTrail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(158, 249, 255, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }
          
                trail.Draw(Projectile.oldPos);
                whiteTrail.Draw(Projectile.oldPos);
            
            

            return false;
        }

    }
}

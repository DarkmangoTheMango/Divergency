using Divergency.Common.Helpers;
using Divergency.Content.Dusts;
using log4net.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;   
using Terraria.GameContent;
using Terraria.GameContent.Biomes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Ranged
{
    internal class LivingCoreArrow : ModProjectile
    {
        bool lineLineCol(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float x1 = p1.X;
            float y1 = p1.Y;
            float x2 = p2.X;
            float y2 = p2.Y;
            float x3 = p3.X;
            float y3 = p3.Y;
            float x4 = p4.X;
            float y4 = p4.Y;

            // calculate the distance to intersection point
            float uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            float uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            // if uA and uB are between 0-1, lines are colliding
            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {

                // optionally, draw a circle where the lines meet
                float intersectionX = x1 + (uA * (x2 - x1));
                float intersectionY = y1 + (uA * (y2 - y1));

                return true;
            }
            return false;
        }


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Arrow;
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

        private enum stage
        {
            InAir,
            InGround,
            ExitingGround,
            Turning,
            SecondInAir,
        }
        private int StageProgress
        {
            get { return (int)Projectile.ai[2]; }
            set { Projectile.ai[2] = value; }
        }
        private stage Stage {
            get { return (stage)Projectile.ai[1]; }
            set { Projectile.ai[1] = (int)value; }
        }


        public override bool PreAI()
        {
            /*
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
            */

            //DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            //Utils.PlotTileLine(Projectile.position, Projectile.position - (Projectile.velocity.SafeNormalize(Vector2.Zero) * 24f), 0, DelegateMethods.CastLight);

            if (!(Stage == stage.InAir || Stage == stage.SecondInAir))
                Projectile.timeLeft = 240;

            switch (Stage)
            {
                case (stage.InAir):
                    return base.PreAI();

                case (stage.SecondInAir):
                    Projectile.position -= Projectile.velocity;
                    break;

                case (stage.InGround):
                    float diff = (Projectile.rotation - (Projectile.velocity.ToRotation() + MathF.PI / 2f)) % (MathF.PI * 2);
                    Console.WriteLine(diff);

                    diff = MathF.Abs(diff - MathF.PI * 2) > diff ? diff : diff - MathF.PI * 2;

                    float rotSpeed = 0.2f;

                    if (diff > 0)
                    {
                        Projectile.rotation -= rotSpeed;
                        if ((diff - rotSpeed) < 0)
                            Projectile.rotation = (Projectile.velocity.ToRotation() + MathF.PI / 2f);
                    }
                    else if (diff < 0)
                    {
                        Projectile.rotation += rotSpeed;
                        if ((diff + rotSpeed) > 0)
                            Projectile.rotation = (Projectile.velocity.ToRotation() + MathF.PI / 2f);
                    }

                    StageProgress = StageProgress + 1;
                    if (StageProgress > 20 && diff == 0f)
                    {
                        Stage = stage.ExitingGround;
                        StageProgress = 0;
                    }
                    break;

                case (stage.ExitingGround):
                    StageProgress = StageProgress + 1;
                    Projectile.position += (Projectile.rotation + MathF.PI / 2f).ToRotationVector2() * 14f;
                    if (StageProgress == 8)
                    {
                        Stage = stage.Turning;
                        StageProgress = 0;
                    }

                    break;

                case (stage.Turning):
                    NPC close = FindClosestNPC(1000f);

                    if (close != null)
                    {
                        float tRot = 0;
                        for (int i = 0; i < 20; i++)
                        {
                            tRot = (close.position - Projectile.position).ToRotation() + MathF.PI / 2f;
                            if (Projectile.rotation + 10 > tRot + 10)
                            {
                                Projectile.position -= (Projectile.rotation).ToRotationVector2() * 0.4f;
                                Projectile.rotation -= 0.01f;
                            }
                            else
                            {
                                Projectile.position += (Projectile.rotation).ToRotationVector2() * 0.4f;
                                Projectile.rotation += 0.01f;
                            }
                        }

                        if (MathF.Abs(Projectile.rotation - tRot) < 0.01)
                        {
                            Projectile.tileCollide = true;
                            Stage = stage.SecondInAir;
                            
                            for (int i = 0; i < Projectile.oldPos.Length; i++)
                            {
                                Projectile.oldPos[i] = Projectile.position; // Clear trail
                            }

                            Projectile.velocity = (Projectile.rotation + MathF.PI / 2f).ToRotationVector2() * 20f;
                        }
                    }
                    else
                        Projectile.Kill();

                    break;
            }

            /*
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

            */

            Projectile.position -= Projectile.velocity;
            return false;
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {


            /*
            for (int i = 0; i < 10; i++)
            {
                Vector2 dir = (-oldVelocity).RotatedBy(Main.rand.NextFloat() * MathF.PI - MathF.PI / 2);
                dir.Normalize();
                dir *= 0.3f;
            }
            */

            if (Stage == stage.SecondInAir)
                return true;

            /*
            if (oldVelocity.X > Projectile.velocity.X)
                Console.WriteLine(1);
            else if (oldVelocity.X < Projectile.velocity.X)
                Console.WriteLine(2);
            else if (oldVelocity.Y > Projectile.velocity.Y)
                Console.WriteLine(3);
            else if (oldVelocity.Y < Projectile.velocity.Y)
                Console.WriteLine(4);
            */

            while (Projectile.rotation < 0)
                Projectile.rotation += MathF.PI*2f;

            float speed = Projectile.velocity.Length();

            if (oldVelocity.X > Projectile.velocity.X)
                Projectile.velocity = new Vector2(speed, 0f);
            else if (oldVelocity.X < Projectile.velocity.X)
                Projectile.velocity = new Vector2(-speed, 0f);
            else if (oldVelocity.Y > Projectile.velocity.Y)
                Projectile.velocity = new Vector2(0f, speed);
            else if (oldVelocity.Y < Projectile.velocity.Y)
                Projectile.velocity = new Vector2(0f, -speed);

            Stage = stage.InGround;

            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            //Projectile.velocity *= 0f;
            //Projectile.position -= oldVelocity;

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

            if (Stage == stage.SecondInAir) // maby somehow let this look wilder? idk
            {
                trail.Draw(Projectile.oldPos);
                whiteTrail.Draw(Projectile.oldPos);
            }



            return false;
        }

    }
}

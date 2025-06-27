using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Summoner.Minions
{
    public class CoreTurbineProj : ModProjectile
    {
        public bool AuraSpawned { get; private set; }
        public Projectile cachedProjectile { get; private set; }
        public bool spawned { get; private set; }

        public override void SetStaticDefaults()
        {
            //.setdefault("Coreprism");
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

        }
        public override void SetDefaults()
        {
            Projectile.width = 57;
            Projectile.height = 72;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
        }

        int frameTimer;
        int timer;
        int timer2;
        int soundtimer = 9;

        private bool collided;

       
       
        public override void AI()
        {

            Player owner = Main.player[Projectile.owner];
            Player player = Main.LocalPlayer;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC target = Main.npc[owner.MinionAttackTargetNPC];
            if (Projectile.Center.Distance(target.Center) > 100 && target.active)
            {
                if (eyescale > 2)
                eyescale += 0.1f; 
            }
               
                }
            if (player != null)
                Projectile.spriteDirection = (int)Projectile.ai[0];
            Projectile.velocity.Y += 1;
            if (!CheckActive(owner))
                return;
            if (collided)
            {
                radius += (100 - radius) / 5f;

                if (radius >= 35)
                {
                    width += (0 - width) / 5f;
                }

                timer++;

                //if (Projectile.frame != 6 && collided)
                //{
                 //   frameTimer++;
                //}
                //if (frameTimer == 6)
                //{
                  //  Projectile.frame++;
                    //frameTimer = 0;
                    //SoundEngine.PlaySound(SoundID.WormDig, Projectile.Center);

               // }
              
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y > 0)
                Projectile.velocity.Y = 0;
            Projectile.velocity.X = 0;
            if (collided == false)
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, Projectile.Center);
            }
            collided = true;
            return false;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                Projectile.timeLeft = 3;
            }

            return true;
        }
        public Trail trail;

        public Trail trail2;

        float radius = 0;

        float timer3 = 0;

        public float timer4 = 0;

        float width = 10;
        private float eyescale = 0.8f;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D OrbTex = ModContent.Request<Texture2D>("Divergency/Content/Projectiles/Summoner/Minions/CoreTurbineCore").Value;
            Texture2D EyeTex = ModContent.Request<Texture2D>("Divergency/Content/Projectiles/Summoner/Minions/CoreTurbineEye").Value;
            Texture2D GlowTex = ModContent.Request<Texture2D>("Divergency/Assets/Textures/WhiteGlow").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Rectangle sourceRectangle2 = new Rectangle(0, frameY, OrbTex.Width, OrbTex.Height);

            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor);
            Color color1 = Color.White;
            Color color2 = new Color(0.50f / 2, 2f / 3, 0.5f / 2, 0);

            Main.EntitySpriteDraw(texture, position - new Vector2(0, 10), sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, position - new Vector2(0, 10), sourceRectangle, color1, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);


            Texture2D texture2 = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Default").Value;

            if (trail == null)
            {
                trail = new Trail(texture2, Trail.DefaultPass, (p) => new Vector2(width), (p) => Projectile.GetAlpha(new Color(0, 255 / 1.2f, 0, 200)));
                trail.drawOffset = Projectile.Size / 2f;

                //trail2 = new Trail(texture2, Trail.DefaultPass, (p) => new Vector2(width / 1.9f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                //trail2.drawOffset = Projectile.Size / 2f;
            }

            int parts = 30;
            Vector2[] tests = new Vector2[parts];
            float[] rotations = new float[parts];

            for (int point = 0; point < parts; point++)
            {
                float rad = ((float)point / (parts - 1)) * MathHelper.TwoPi;

                tests[point] = Projectile.position + new Vector2(MathF.Cos(rad) * radius, MathF.Sin(rad) * radius);
                rotations[point] = rad;
            }

            timer3 -= 0.01f;

            trail.Draw(tests, rotations, timer3);
            // trail2.Draw(tests, rotations, timer3);

            //glow:
            // Main.EntitySpriteDraw(GlowTex, position - new Vector2(-25, 45), sourceRectangle2, color2, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);

      

            //orb:
          //  Main.EntitySpriteDraw(OrbTex, position - new Vector2(-5, 45), sourceRectangle, color, Projectile.rotation, origin, 0.9f, SpriteEffects.None, 0);
            //Main.EntitySpriteDraw(OrbTex, position - new Vector2(-5, 45), sourceRectangle, color1, Projectile.rotation, origin, 0.9f, SpriteEffects.None, 0);

            //eye:
            Main.EntitySpriteDraw(OrbTex, position - new Vector2(-5, 45), sourceRectangle, color, Projectile.rotation, origin, eyescale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(OrbTex, position - new Vector2(-5, 45), sourceRectangle, color1, Projectile.rotation, origin, eyescale, SpriteEffects.None, 0);
            Main.NewText(eyescale);
            return false;
        }



    }

   
}
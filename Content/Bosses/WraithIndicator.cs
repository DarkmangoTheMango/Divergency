using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Divergency.Content.Events.LivingCore;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;


namespace Divergency.Content.Bosses
{
    public class WraithIndicator : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public Projectile cachedProjectile { get; private set; }
        public bool spawned { get; private set; }
        public NPC cachedNPC { get; private set; }
        private Vector2 clawPos1;
        private Vector2 clawPos2;
        private Vector2 clawPos3;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 36;
            Projectile.hide = false;
            Projectile.scale = 3f;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        Vector2 originPos;
        public override void AI()
        {
            if (!spawned)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is WraithHand)
                    {
                        cachedNPC = taggedNPC;
                    }
                }
               
                spawned = true;
            }

            if (!cachedNPC.active)
            {
                Projectile.active = false;
            }
            if (spawned)
            {
                clawPos1 = Projectile.Center;
                clawPos2 = Projectile.BottomRight;
                clawPos3 = Projectile.TopLeft;
                Projectile.Center = cachedNPC.Center;
                Projectile.localAI[0] += .007f;
                Projectile.rotation += Projectile.localAI[0];

                for (int k = 0; (k < 10); k++)
                {
                    Dust.NewDustPerfect(clawPos1 + new Vector2(Main.rand.NextFloat(-1,1)), DustID.PortalBoltTrail, Projectile.velocity /5, 0, new Color(0, 223, 0), 1.4f).noGravity = true;
                    Dust.NewDustPerfect(clawPos2 + new Vector2(Main.rand.NextFloat(-1, 1)), DustID.PortalBoltTrail, Projectile.velocity /5 , 0, new Color(0, 223, 0), 1.4f).noGravity = true;
                    Dust.NewDustPerfect(clawPos3 + new Vector2(Main.rand.NextFloat(-1, 1)), DustID.PortalBoltTrail, Projectile.velocity / 5, 0, new Color(0, 223, 0), 1.4f).noGravity = true;
                }


                Projectile.scale -= 0.15f;
                Projectile.velocity = cachedNPC.velocity;
               
                // else
                // {

                ///Projectile.scale -= .06f;
                //    Projectile.velocity *= .9f;

                if (Projectile.scale <= .01f) 
                Projectile.scale = 0;
            }
           
        }
        public Trail trail;
        public Trail trail2;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/TripleFire").Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }), Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }), -Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale, 0, 0);
            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(32f), (p) => Projectile.GetAlpha(new Color(30, 220, 30, 100)));
                trail.drawOffset = Projectile.Size / 2f;
            }
            if (trail2 == null)
            {
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(32f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }
        
            trail.Draw(Projectile.oldPos);
            trail2.Draw(Projectile.oldPos);
 
            
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                //SoundEngine.PlaySound(SoundID.Shimmer2,Projectile.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, originPos.DirectionFrom(Projectile.Center) * (7 + Projectile.ai[0]), ModContent.ProjectileType<Thorn_ClawSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[1], 0, Projectile.ai[2]);
            }
        }
    }
    public class WraithIndicator2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public Projectile cachedProjectile { get; private set; }
        public bool spawned { get; private set; }
        public NPC cachedNPC { get; private set; }
        private Vector2 clawPos1;
        private Vector2 clawPos2;
        private Vector2 clawPos3;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40;
            Projectile.hide = false;
            Projectile.scale = 3f;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        Vector2 originPos;
        public override void AI()
        {
            if (!spawned)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is WraithHand)
                    {
                        cachedNPC = taggedNPC;
                    }
                }

                spawned = true;
            }

            if (!cachedNPC.active)
            {
                Projectile.active = false;
            }
            if (spawned)
            {
                clawPos1 = Projectile.Center;
                clawPos2 = Projectile.BottomRight;
                clawPos3 = Projectile.TopLeft;
                Projectile.Center = cachedNPC.Center;
                Projectile.localAI[0] += .007f;
                Projectile.rotation += Projectile.localAI[0];
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                for (int k = 0; (k < 5); k++)
                {

                    Dust dust = Dust.NewDustPerfect(Projectile.Center + (velocity * 110f), ModContent.DustType<Glow>(), velocity * -5f, 0, Color.LimeGreen, 0.4f);
                    dust.noGravity = true;

                }


                Projectile.scale -= 0.15f;
                Projectile.velocity = cachedNPC.velocity;

                // else
                // {

                ///Projectile.scale -= .06f;
                //    Projectile.velocity *= .9f;

                if (Projectile.scale <= .01f)
                    Projectile.scale = 0;
            }

        }
       
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/TripleFire").Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }), Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }), -Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale, 0, 0);



            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                //SoundEngine.PlaySound(SoundID.Shimmer2,Projectile.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    //Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, originPos.DirectionFrom(Projectile.Center) * (7 + Projectile.ai[0]), ModContent.ProjectileType<Thorn_ClawSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[1], 0, Projectile.ai[2]);
                }
        }
    }
}


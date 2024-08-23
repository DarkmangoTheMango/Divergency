using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Divergency.Common.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Divergency.Content.Dusts;
using Divergency.Common.Players;
using Terraria.Audio;
using System.Drawing.Imaging;
using Divergency.Content.NPCs.LivingGrove;
using Terraria.GameContent.Bestiary;
using Divergency.Content.Projectiles;
using Divergency.Content.Particles;
using ParticleLibrary;



namespace Divergency.Content.NPCs.LivingGrove
{
    public class Markscore : ModNPC
    {
      
        private bool spawned;
        private bool enrage;

        enum State
        {
            walking,
            mines,
            threeshot,
            airshot,
            dash, 
            jump,

        }
        State state = State.walking;
        private Vector2 oldtargetpos;

        public int ShotCount { get; private set; }

        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 150;
            NPC.damage = 10;
            NPC.defense = 2;
            NPC.knockBackResist = 0.7f;

            NPC.Size = new Vector2(40, 100);
            NPC.scale = 1f;

            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = Item.sellPrice(0, 0, 0, 90);

            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("snipey")
            });
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
            }
            if (spawned)
            {
                float radius = 0.3f;

                if (NPC.GetLifePercent() < 0.25)
                {
                    Dust.NewDustPerfect(NPC.Top, DustID.PortalBoltTrail, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / 1)) * radius, 0, new Color(109, 223, 94), 1f).noGravity = true;
                    Dust.NewDustPerfect(NPC.Top, ModContent.DustType<Glow>(), Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / 1)) * radius, 0, new Color(109, 223, 94), 0.2f).noGravity = true;
                    enrage = true;
                }
                NPC.TargetClosest(true);
                Player target = Main.player[NPC.target];

                float direction = NPC.direction;
                NPC.spriteDirection = -NPC.direction;

                if (state == State.walking)
                {
                    NPC.ai[0]++;
                    NPC.velocity.Y += 0.2f;
                    if (target.Distance(NPC.Center) <= 300)
                    {
                        if (!enrage)
                        {
                            NPC.velocity.X = 2f * NPC.spriteDirection;
                        }
                        else
                        {
                            NPC.velocity.X = 3f * -NPC.spriteDirection;

                        }
                    }
                    if (target.Distance(NPC.Center) >= 300) 
                    {
                        NPC.velocity *= 0.9f;
                    }

                    if (NPC.ai[0] == 240)
                    {
                        NPC.ai[0] = 0;
                        if (Main.rand.NextBool(2))
                        {
                            state = State.dash;
                        }
                        else
                        {
                            state = State.jump;
                        }
                    }   

                }

                if (state == State.jump)
                {
                    if (NPC.ai[0] == 1)
                    {
                        NPC.velocity.Y = -10;

                    }
                    NPC.ai[0]++;
                    
                    if (NPC.ai[0] == 30)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            state = State.airshot;
                        }
                        else
                        {
                            state = State.threeshot;
                        }
                        NPC.ai[0] = 0;
                    }


                }
                if (state == State.dash)
                {
                    //Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0), ModContent.ProjectileType<DashVisualsMarkscore>(), 0, 0);

                    NPC.velocity.X += 30f * Main.rand.NextFloat(-1,1);
                    NPC.ai[0] = 0;

                    if (Main.rand.NextBool(2))
                    {
                        state = State.airshot;
                    }
                    else
                    {
                        state = State.threeshot;
                    }

                }
                Vector2 shotVector;
                Vector2 shotPosition;

                if (state == State.threeshot)
                {
                    Vector2 speed = NPC.DirectionTo(oldtargetpos);

                    NPC.ai[0]++; 
                    if (NPC.ai[0] == 19)
                    {
                        oldtargetpos = target.Center;

                    }
                    if (NPC.ai[0] >= 20)
                    {

                        NPC.velocity.Y = 0;
                        NPC.velocity.X = 0;

                        for (int i = 0; i < 4   ; i++)
                        {


                            Dust.QuickDustLine(NPC.Center, NPC.Center + speed.RotatedBy(i / 2.5) * 1200, DustID.GemEmerald,Color.LimeGreen);
                            Dust.QuickDustLine(NPC.Center, NPC.Center + speed.RotatedBy(i / -2.5) * 1200, DustID.GemEmerald, Color.LimeGreen);


                        }
                    }
                    else
                    {
                        NPC.velocity *= 0.9f;

                    }
                    if (NPC.ai[0] == 90)
                    {

                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, speed.RotatedBy(i / 2.5) * 7, ModContent.ProjectileType<MarkscoreArrow>(), 20, 1);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, speed.RotatedBy(i / -2.5) * 7, ModContent.ProjectileType<MarkscoreArrow>(), 20, 1);

                            

                        }
                    }
                    if (NPC.ai[0] == 95)
                    {

                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, speed.RotatedBy(i / 2.5) * 7, ModContent.ProjectileType<MarkscoreArrow>(), 20, 1);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, speed.RotatedBy(i / -2.5) * 7, ModContent.ProjectileType<MarkscoreArrow>(), 20, 1);


                        }
                    }
                    if (NPC.ai[0] == 100)
                    {

                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, speed.RotatedBy(i / 2.5) * 7, ModContent.ProjectileType<MarkscoreArrow>(), 20, 1);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, speed.RotatedBy(i / -2.5) * 7, ModContent.ProjectileType<MarkscoreArrow>(), 20, 1);

                            NPC.ai[0] = 0;
                            if (!enrage)
                            {
                                state = State.walking;

                            }
                            else
                            {
                                if (Main.rand.NextBool(2))
                                {
                                    state = State.dash;
                                }
                                else
                                {
                                    state = State.jump;
                                }
                            }
                        }
                    }


                }
                if (state == State.airshot)
                {
                    NPC.velocity.X *= 0.9f;

                 
                        Vector2 speed = NPC.DirectionTo(oldtargetpos);

                    NPC.ai[0]++;
                    NPC.ai[1]++;

                    Vector2 speed2 = Main.rand.NextVector2Circular(1f, 1f);
                    
                    if (NPC.ai[0] < 160)
                    {
                        Main.rand.NextFloat(MathHelper.TwoPi);

                       
                            ParticleManager.NewParticle<FlareLineParticle>(NPC.Center + new Vector2(0f, 0f).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)), new Vector2(Main.rand.NextFloat(2f, 5f), -Main.rand.NextFloat(2f, 5f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi) + MathHelper.Pi), new Color(0.50f, 2.05f, 0.5f, 0), 0.5f, Main.rand.NextFloat(0.8f, 1.1f));
                        
                    }
                    if (NPC.ai[0] == 180)
                    {
                        NPC.velocity.Y = 0;
                        NPC.velocity.X = 0;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (speed2 * 100f), (NPC.DirectionTo(target.Center) * 10f).RotatedByRandom(0.9f), ModContent.ProjectileType<MarkscoreArrowHoming>(), NPC.damage, 0f, 0);
                        Dust dust2 = Dust.NewDustPerfect(NPC.Center + (speed2 * 200f), DustID.GemEmerald, speed2 * 2f, 0, default, 1f);
                        NPC.ai[0] = 160;
                    }
                  
                    if (NPC.ai[1] == 361)
                    {
                        NPC.ai[0] = 0; 
                        NPC.ai[1] = 0;
                        if (!enrage)
                        {
                            state = State.walking;

                        }
                        else
                        {
                            if (Main.rand.NextBool(2))
                            {
                                state = State.dash;
                            }
                            else
                            {
                                state = State.jump;
                            }
                        }
                    }


                }
            }


        }

        public override void HitEffect(NPC.HitInfo hit)
        {

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }


            if (NPC.life <= 0)
            {
                //Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, -3f)), Mod.Find<ModGore>("Acorn").Type, 1f);
            }
        }



    }
   
    

    public class MarkscoreArrow : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            //.setdefault("Living Shrapnel");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.damage = 10;
            Projectile.width = Projectile.height = 15;
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, new Vector2(Main.rand.NextFloat(-0.4f, 0.4f)), 0, Color.LimeGreen, 0.5f).noGravity = true;


        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            Vector2 spawnPosition = Projectile.Center;

            Main.rand.NextFloat(MathHelper.TwoPi);

            for (int i = 0; i < 15; i++)
            {
                ParticleManager.NewParticle<FlareLineParticle>(spawnPosition + new Vector2(0f, 0f).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)), new Vector2(Main.rand.NextFloat(2f, 5f), -Main.rand.NextFloat(2f, 5f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi) + MathHelper.Pi), new Color(0.50f, 2.05f, 0.5f, 0), 0.5f, Main.rand.NextFloat(0.8f, 1.1f));
            }

            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f) * 5, ModContent.ProjectileType<MarkscoreArrowExplosion>(), 20, 1);
            DivergencyDraw.SpawnExplosion(Projectile.Center, Color.LimeGreen, ModContent.DustType<Glow>(), 1, scale: 0.7f, noDust: true, dustScale: 0); ;


            return base.OnTileCollide(oldVelocity);
        }

        public Trail trail;
        public Trail whiteTrail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(10, 255, 10, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                whiteTrail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 247, 179, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }

            whiteTrail.Draw(Projectile.oldPos);

            return true;
        }
    }
    public class MarkscoreArrowExplosion : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void SetStaticDefaults()
        {
            //.setdefault("Living Shrapnel");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.damage = 10;
            Projectile.width = Projectile.height = 100;
            Projectile.scale = 1.3f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 1;
            Projectile.aiStyle = -1;
        }

     
    }

    public class MarkscoreArrowHoming : ModProjectile
    {
    
        public override void SetStaticDefaults()
        {
            //.setdefault("Living Shrapnel");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.damage = 10;
            Projectile.width = Projectile.height = 15;
            Projectile.scale = 1f;
           
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
        }
        public Player target;
        public override void AI()
        {
            if (!spawned)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is Markscore && Projectile.Distance(taggedNPC.Center) < 500)
                    {
                        cachedNPC = taggedNPC;
                    }
                }

                spawned = true;
            }

            if (!cachedNPC.active)
            {
                Projectile.Kill();
            }
            Player player = Main.LocalPlayer;

            Projectile.ai[0] += 1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, new Vector2(Main.rand.NextFloat(-0.4f, 0.4f)), 0, Color.LimeGreen, 0.5f).noGravity = true;

            if (Projectile.ai[0] < 720)
            {

                Projectile.Move(player.Center, 0.001f);

            }
            else
            {
                Projectile.Move(player.Center, Projectile.ai[0] / 20);
            }

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            Vector2 spawnPosition = Projectile.Center;

            Main.rand.NextFloat(MathHelper.TwoPi);

            for (int i = 0; i < 15; i++)
            {
                ParticleManager.NewParticle<FlareLineParticle>(spawnPosition + new Vector2(0f, 0f).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)), new Vector2(Main.rand.NextFloat(2f, 5f), -Main.rand.NextFloat(2f, 5f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi) + MathHelper.Pi), new Color(0.50f, 2.05f, 0.5f, 0), 0.5f, Main.rand.NextFloat(0.8f, 1.1f));
            }

            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f) * 5, ModContent.ProjectileType<MarkscoreArrowExplosion>(), 20, 1);
            DivergencyDraw.SpawnExplosion(Projectile.Center, Color.LimeGreen, ModContent.DustType<Glow>(), 1, scale: 0.7f, noDust: true, dustScale: 0); ;


        }


        public Trail trail;
        public Trail whiteTrail;
        private bool spawned;
        private NPC cachedNPC;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(10, 255, 10, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                whiteTrail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 247, 179, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }

            whiteTrail.Draw(Projectile.oldPos);

            return true;
        }
    }
}




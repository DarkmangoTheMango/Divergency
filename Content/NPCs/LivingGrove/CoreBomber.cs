using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Divergency.Content.Events.LivingCore;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Net.Security;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{

    public class CoreBomber  : ModNPC
    {
        

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Scale = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            


        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 75;
            NPC.damage = 20;
            NPC.defense = 8;
            NPC.knockBackResist = 0f;

            NPC.noTileCollide = false;

            NPC.scale = 0.2f;
            NPC.Size = new Vector2 (150, 150);

            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Kaboom")
            });
        }
        bool secondPhase = false; 
        enum ActionState
        {
            Moving,
            Throwing,
            Igniting,
            RapidFire

        }
        private int teleport;

        public bool TpBack { get; private set; }
        public float throwCounter = 180f;

        ActionState state = ActionState.Moving;

        public float Phase;
        private bool initialize;

        public override void AI()
        {
            if (!initialize)
            {

                initialize = true;

            }
            else
            {
                Player player = Main.player[NPC.target];

                if (NPC.HasValidTarget && !player.dead)
                {

                    NPC.TargetClosest();
                }


              

                NPC.TargetClosest(true);

                if (state == ActionState.Moving)
                {
                    NPC.Move(player.Center, 3, Main.rand.NextFloat(70, 130));
                    NPC.ai[0]++;
                    if (NPC.ai[0] == throwCounter)
                    {
                        NPC.ai[0] = 0;

                        if (NPC.GetLifePercent() < 0.25)
                        {
                            throwCounter = 45;
                            

                        }
                        state = ActionState.Throwing;
                       

                    }
                   
                    float radius = 0.3f;
                    if (NPC.GetLifePercent() < 0.25)
                    {
                        Dust.NewDustPerfect(NPC.Center, DustID.PortalBoltTrail, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / 1 )) * radius, 0, new Color(109, 223, 94), 1f).noGravity = true;
                        Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Glow>(), Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / 1 )) * radius, 0, new Color(109, 223, 94), 0.2f).noGravity = true;

                    }
                }
                if (state == ActionState.Throwing)
                {
                    SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = 1f }, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(player.Center) * 7f, ModContent.ProjectileType<CoreBomb>(), NPC.damage / 4, 3f, 0);
                    
                    NPC.velocity -= NPC.DirectionTo(player.Center);

                    state = ActionState.Moving;
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
                //NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<SageDeath>());
            }
        }


        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.hide)
            {
                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                //Texture2D glow = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/BigGuyGlow").Value;


                Texture2D a = TextureAssets.Npc[Type].Value;
                //Main.EntitySpriteDraw(a, NPC.VisualPosition - screenPos + new Vector2(-30, 34), NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, 1f, effects, 0);
               // Main.EntitySpriteDraw(glow, NPC.VisualPosition - screenPos + new Vector2(-30, 34), NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, 1f, effects, 0);

            }
            return true;
        }
    }

    public class CoreBomb : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";

        public bool kaboom { get; private set; }

        float timer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.Size = new Vector2(14);
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 1180;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
        }

       
        public override void AI()
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Projectile.velocity.RotatedByRandom(0.1f), 0, new Color(109, 223, 94), 1.2f).noGravity = true;

            }
            Projectile.velocity *= 0.99f;

            if (Projectile.velocity.X <= 0.08f && Projectile.timeLeft < 1000)
            {
                Projectile.velocity *= 0.1f;
                if (!kaboom)
                {
                    DivergencyDraw.SpawnExplosionIndicator(Projectile.Center, Color.LimeGreen, 0.4f);
                    Projectile.timeLeft = 180;
                    kaboom = true;
                }
                Vector2 spawnPosition = Projectile.Center;

                float rotation = Main.rand.NextFloat(MathHelper.TwoPi);

                for (int i = 0; i < 2; i++)
                {
                    ParticleManager.NewParticle<FlareLineParticle>(spawnPosition + new Vector2(128f, 0f).RotatedBy(rotation), new Vector2(4f, 0f).RotatedBy(rotation + MathHelper.Pi), new Color(0.50f, 2.05f, 0.5f, 0), 0.9f, Main.rand.NextFloat(0.8f, 1.1f));
                }

                if (Projectile.timeLeft == 1)
                {
                    Player player = Main.LocalPlayer;
                    player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 12;
                    
                    DivergencyDraw.SpawnExplosion(Projectile.Center, Color.LimeGreen, DustID.PortalBoltTrail, 0);
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                        Dust dust = Dust.NewDustPerfect(Projectile.Center + (velocity * 120f), ModContent.DustType<Glow>(), velocity * 15f, 0, Color.LimeGreen, 0.6f);
                    }

                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0), ModContent.ProjectileType<CoreElementalDeathProj>(), Projectile.damage * 2, 1.2f);
                    player.GetModPlayer<FlashPlayer>().intensity += 40;

                }


            }

        
    }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 5) { Projectile.Kill(); }
            else
            {
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) { Projectile.velocity.X = -oldVelocity.X; }
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) { Projectile.velocity.Y = -oldVelocity.Y; }

                int numberOfDusts = 5;
                float radius = 2;

                for (int i = 0; i < numberOfDusts; i++) { Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfDusts * i)) * radius, 0, new Color(109, 223, 94), 1.2f).noGravity = true; }

                Projectile.oldPos[0] = Projectile.position;
            }

            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 16;

            return true;
        }

        public Trail trail;
        public Trail whiteTrail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Star").Value;
            Texture2D Radius = ModContent.Request<Texture2D>("Divergency/Assets/Textures/GlowRing").Value;



            Vector2 origin = texture.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            
            Color colorRadius = Projectile.GetAlpha(new Color(109, 223, 94, Projectile.timeLeft));

            Color color = Projectile.GetAlpha(new Color(109, 223, 94, 0));

            Main.EntitySpriteDraw(texture, position, null, color, timer, origin, 1 + (float)Math.Sin(timer) * 0.5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, position, null, color, -timer / 2, origin, (1 * 0.6f) + (float)Math.Sin(timer) * 0.5f, SpriteEffects.None, 0);

            timer += 0.1f;

            if (timer >= MathHelper.Pi)
            {
                timer = 0f;
            }

            

            return false;
        }
    }
    public class FlashPlayer : ModPlayer
    {
        public int intensity = 1;

        public override void PreUpdate()
        {
            if (intensity > 1)
            {
                intensity -= 5;
            }
            if (intensity <= 2)
                intensity = 1;
        }
    }


}


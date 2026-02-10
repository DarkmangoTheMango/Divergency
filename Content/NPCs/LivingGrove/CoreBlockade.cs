using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Divergency.Content.Events.LivingCore;
using Divergency.Content.Items.Accessories;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Melee;
using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using System.Net.Security;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{

    public class CoreBlockadeRight : ModNPC
    {

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Scale = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            Main.npcFrameCount[NPC.type] = 4;



        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 75;
            NPC.damage = 20;
            NPC.defense = 8;
            NPC.knockBackResist = 0f;

            NPC.noTileCollide = false;

            NPC.scale = 1;
            NPC.Size = new Vector2(40, 88);

            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.behindTiles = true;
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
            Floating,
            Falling,
            Blocking

        }
        private int teleport;

        public bool TpBack { get; private set; }
        public float throwCounter = 180f;

        ActionState state = ActionState.Falling;

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

                if (state == ActionState.Floating)
                {
                    NPC.ai[0]++;

                    NPC.Move(player.Center - new Vector2(-200, 150), 40, 20); //for making them spawn right and left


                    if (NPC.ai[0] == 1)
                    {
                        state = ActionState.Falling;
                        NPC.ai[0] = 0;
                    }
                }


                if (state == ActionState.Falling)
                {
                    NPC.ai[0]++;
                    NPC.velocity.X = 0;
                    if (NPC.ai[0] == 0)
                    {
                        NPC.velocity.Y = 0;
                    }
                    if (NPC.ai[0] < 40)
                    {
                        NPC.velocity.Y -= 0.2f;
                    }
                    if (NPC.ai[0] > 40)
                    {
                        NPC.velocity.Y += 3f;
                    }

                    if (NPC.collideY)
                    {
                        CameraSystem.ScreenShake(12);
                        NPC.Center += new Vector2(0, 5);
                        Vector2 spawnPosition = NPC.Center;

                        Main.rand.NextFloat(MathHelper.TwoPi);

                        for (int i = 0; i < 100; i++)
                        {
                            ParticleManager.NewParticle<FlareLineParticle>(spawnPosition + new Vector2(0f, 0f).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)), new Vector2(Main.rand.NextFloat(5f, 12f), -Main.rand.NextFloat(5f, 12f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi) + MathHelper.Pi), new Color(0.50f, 2.05f, 0.5f, 0), 1.1f, Main.rand.NextFloat(0.8f, 1.1f));
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -450), new Vector2(0), ModContent.ProjectileType<BlockadeRight>(), 0, 0, 0);
                        state = ActionState.Blocking;
                    }
                }
                if (state == ActionState.Blocking)
                {
                    Vector2 spawnPosition = NPC.Center;
                    
                    ParticleManager.NewParticle<FlareLineParticleCurved>(spawnPosition, new Vector2(0, -3), new Color(0.50f, 2.05f, 0.5f, 0), 1.1f, Main.rand.NextFloat(0.8f, 1.1f));
                    //ParticleManager.NewParticle<FlareLineParticle>(spawnPosition + new Vector2(Main.rand.NextFloat(-30, 30), 0), new Vector2(0, -4), new Color(0.50f, 2.05f, 0.5f, 0), 1.1f, Main.rand.NextFloat(0.8f, 1.1f));
                    

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
            NPC.spriteDirection = -NPC.direction;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * 4)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.hide)
            {
                var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Texture2D tex = Terraria.GameContent.TextureAssets.Npc[Type].Value;
                var fadeMult = 1f / NPCID.Sets.TrailCacheLength[Type];
                if (state == ActionState.Falling)
                {
                    for (int i = 0; i < NPC.oldPos.Length; i++)
                    {
                        Main.spriteBatch.Draw(tex, NPC.oldPos[i] - Main.screenPosition + NPC.Size / 2 + new Vector2(0, 0), NPC.frame, Color.DarkGreen * (1f - fadeMult * i), NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0f);
                    }
                }

                Texture2D a = TextureAssets.Npc[Type].Value;
                Main.EntitySpriteDraw(a, NPC.Center - screenPos + new Vector2(0, 0), NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, 1f, effects, 0);
                Texture2D glow = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/CoreBlockadeGlow").Value;
                Main.EntitySpriteDraw(glow, NPC.Center - screenPos + new Vector2(0, 0), NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, 1f, effects, 0);
            }
            return false;
        }
    }

    public class CoreBlockadeLeft : ModNPC
    {


        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Scale = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            Main.npcFrameCount[NPC.type] = 4;



        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 75;
            NPC.damage = 20;
            NPC.defense = 8;
            NPC.knockBackResist = 0f;

            NPC.noTileCollide = false;

            NPC.scale = 1;
            NPC.Size = new Vector2(40, 88);

            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.behindTiles = true;
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
            Floating,
            Falling,
            Blocking

        }
        private int teleport;

        public bool TpBack { get; private set; }
        public float throwCounter = 180f;

        ActionState state = ActionState.Falling;

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

                if (state == ActionState.Floating)
                {
                    NPC.ai[0]++;

                    NPC.Move(player.Center - new Vector2(200, 150), 40, 20); //for making them spawn right and left


                    if (NPC.ai[0] == 1)
                    {
                        state = ActionState.Falling;
                        NPC.ai[0] = 0;
                    }
                }


                if (state == ActionState.Falling)
                {
                    NPC.ai[0]++;
                    NPC.velocity.X = 0;
                    if (NPC.ai[0] == 0)
                    {
                        NPC.velocity.Y = 0;
                    }
                    if (NPC.ai[0] < 40)
                    {
                        NPC.velocity.Y -= 0.2f;
                    }
                    if (NPC.ai[0] > 40)
                    {
                        NPC.velocity.Y += 3f;
                    }

                    if (NPC.collideY)
                    {
                        CameraSystem.ScreenShake(12);
                        NPC.Center += new Vector2(0, 5);
                        Vector2 spawnPosition = NPC.Center;

                        Main.rand.NextFloat(MathHelper.TwoPi);

                        for (int i = 0; i < 100; i++)
                        {
                            ParticleManager.NewParticle<FlareLineParticle>(spawnPosition + new Vector2(0f, 0f).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)), new Vector2(Main.rand.NextFloat(5f, 12f), -Main.rand.NextFloat(5f, 12f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi) + MathHelper.Pi), new Color(0.50f, 2.05f, 0.5f, 0), 1.1f, Main.rand.NextFloat(0.8f, 1.1f));
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -450), new Vector2(0), ModContent.ProjectileType<BlockadeLeft>(), 0, 0, 0);
                        state = ActionState.Blocking;
                    }
                }
                if (state == ActionState.Blocking)
                {
                    Vector2 spawnPosition = NPC.Center;

                    ParticleManager.NewParticle<FlareLineParticleCurved>(spawnPosition, new Vector2(0, -3), new Color(0.50f, 2.05f, 0.5f, 0), 1.1f, Main.rand.NextFloat(0.8f, 1.1f));
                    //ParticleManager.NewParticle<FlareLineParticle>(spawnPosition + new Vector2(Main.rand.NextFloat(-30, 30), 0), new Vector2(0, -4), new Color(0.50f, 2.05f, 0.5f, 0), 1.1f, Main.rand.NextFloat(0.8f, 1.1f));


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
            NPC.spriteDirection = -NPC.direction;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * 4)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.hide)
            {
                var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Texture2D tex = Terraria.GameContent.TextureAssets.Npc[Type].Value;
                var fadeMult = 1f / NPCID.Sets.TrailCacheLength[Type];
                if (state == ActionState.Falling)
                {
                    for (int i = 0; i < NPC.oldPos.Length; i++)
                    {
                        Main.spriteBatch.Draw(tex, NPC.oldPos[i] - Main.screenPosition + NPC.Size / 2 + new Vector2(0, 0), NPC.frame, Color.DarkGreen * (1f - fadeMult * i), NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0f);
                    }
                }

                Texture2D a = TextureAssets.Npc[Type].Value;
                Main.EntitySpriteDraw(a, NPC.Center - screenPos + new Vector2(0, 0), NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, 1f, effects, 0);

                Texture2D glow = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/CoreBlockadeGlow").Value;
                Main.EntitySpriteDraw(glow, NPC.Center - screenPos + new Vector2(0, 0), NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, 1f, effects, 0);


            }
            return false;
        }
    }
    public class BlockadeRight : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/BlockadeLaser";
        public override void SetStaticDefaults()
        {
            //.setdefault("Pulse");
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 1000;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            Projectile.hide = true;
        }
        public override void AI()
        {
            Projectile.timeLeft = 100;
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC taggedNPC = Main.npc[k];

                if (taggedNPC.active && taggedNPC.ModNPC is CoreBlockadeRight)
                {
                    cachedNPC = taggedNPC;
                }
            }
            for (int p = 0; p < Main.maxNetPlayers; p++)
            {
                Player player = Main.player[p];
                if (player.Hitbox.Intersects(Projectile.Hitbox))
                {
                    player.velocity.X += cachedNPC.direction * 5;
                }

            }
            for (int d = 0; d < Main.maxProjectiles; d++)
            {
                Projectile projectile = Main.projectile[d];
                if (projectile.type == ModContent.ProjectileType<CoreDashLeftProjectile>() || projectile.type == ModContent.ProjectileType<CoreDashRightProjectile>())
                {
                    for (int p = 0; p < Main.maxNetPlayers; p++)
                    {
                        Player player = Main.player[p];
                    }
                    projectile.Kill();


                }
                if (Projectile.Hitbox.Intersects(projectile.Hitbox) && Projectile.type != projectile.type && projectile.type == ProjectileID.Hook || (projectile.type == ProjectileID.AmberHook || projectile.type == ProjectileID.AntiGravityHook
                  || projectile.type == ProjectileID.BatHook || projectile.type == ProjectileID.CandyCaneHook || projectile.type == ProjectileID.ChristmasHook || projectile.type == ProjectileID.DualHookBlue || projectile.type == ProjectileID.DualHookRed || projectile.type == ProjectileID.FishHook || projectile.type == ProjectileID.GemHookAmethyst
                  || projectile.type == ProjectileID.GemHookDiamond || projectile.type == ProjectileID.GemHookEmerald || projectile.type == ProjectileID.GemHookRuby || projectile.type == ProjectileID.GemHookSapphire || projectile.type == ProjectileID.GemHookTopaz
                  || projectile.type == ProjectileID.IlluminantHook || projectile.type == ProjectileID.LunarHookNebula || projectile.type == ProjectileID.LunarHookSolar || projectile.type == ProjectileID.LunarHookStardust || projectile.type == ProjectileID.LunarHookVortex || projectile.type == ProjectileID.QueenSlimeHook
                  || projectile.type == ProjectileID.SlimeHook || projectile.type == ProjectileID.SquirrelHook || projectile.type == ProjectileID.StaticHook || projectile.type == ProjectileID.TendonHook || projectile.type == ProjectileID.ThornHook || projectile.type == ProjectileID.TrackHook || projectile.type == ProjectileID.WoodHook || projectile.type == ProjectileID.WormHook))
                {
                    projectile.Kill();
                }

            }



            if (!cachedNPC.active)
            {
                Projectile.Kill();
            }

        }
       
        public Color color;
        private NPC cachedNPC;

        public override bool PreDraw(ref Color lightColor)
        {
            
            return false;
        }
    }
    public class BlockadeLeft : ModProjectile
    {
        private NPC cachedNPC;

        public override string Texture => "Divergency/Assets/Textures/BlockadeLaser";
        public override void SetStaticDefaults()
        {
            //.setdefault("Pulse");
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 1000;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            Projectile.hide = true;
        }
        public override void AI()
        {
            Projectile.timeLeft = 100;
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC taggedNPC = Main.npc[k];

                if (taggedNPC.active && taggedNPC.ModNPC is CoreBlockadeLeft)
                {
                    cachedNPC = taggedNPC;
                }
            }
            for (int p = 0; p < Main.maxNetPlayers; p++)
            {
                Player player = Main.player[p];
                if (player.Hitbox.Intersects(Projectile.Hitbox))
                {
                    player.velocity.X += cachedNPC.direction * 5;
                }

            }
            for (int d = 0; d < Main.maxProjectiles; d++)
            {
                Projectile projectile = Main.projectile[d];
                if (projectile.type == ModContent.ProjectileType<CoreDashLeftProjectile>() || projectile.type == ModContent.ProjectileType<CoreDashRightProjectile>())
                {
                    for (int p = 0; p < Main.maxNetPlayers; p++)
                    {
                        Player player = Main.player[p];
                    }
                    projectile.Kill();



                }
                if (Projectile.Hitbox.Intersects(projectile.Hitbox) && Projectile.type != projectile.type && projectile.type == ProjectileID.Hook || (projectile.type == ProjectileID.AmberHook || projectile.type == ProjectileID.AntiGravityHook
                    || projectile.type == ProjectileID.BatHook || projectile.type == ProjectileID.CandyCaneHook || projectile.type == ProjectileID.ChristmasHook || projectile.type == ProjectileID.DualHookBlue || projectile.type == ProjectileID.DualHookRed || projectile.type == ProjectileID.FishHook || projectile.type == ProjectileID.GemHookAmethyst
                    || projectile.type == ProjectileID.GemHookDiamond || projectile.type == ProjectileID.GemHookEmerald || projectile.type == ProjectileID.GemHookRuby || projectile.type == ProjectileID.GemHookSapphire || projectile.type == ProjectileID.GemHookTopaz
                    || projectile.type == ProjectileID.IlluminantHook || projectile.type == ProjectileID.LunarHookNebula || projectile.type == ProjectileID.LunarHookSolar || projectile.type == ProjectileID.LunarHookStardust || projectile.type == ProjectileID.LunarHookVortex || projectile.type == ProjectileID.QueenSlimeHook
                    || projectile.type == ProjectileID.SlimeHook || projectile.type == ProjectileID.SquirrelHook || projectile.type == ProjectileID.StaticHook || projectile.type == ProjectileID.TendonHook || projectile.type == ProjectileID.ThornHook || projectile.type == ProjectileID.TrackHook || projectile.type == ProjectileID.WoodHook || projectile.type == ProjectileID.WormHook))
                {
                    projectile.Kill();
                }
                    

            }


            if (!cachedNPC.active)
            {
                Projectile.Kill();
            }

        }
    }


}


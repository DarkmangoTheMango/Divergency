using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Accessories;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Magic;
using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ParticleLibrary;
using System;
using System.Net.Security;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Divergency.Content.NPCs.LivingGrove
{

    public class BigGuy : ModNPC
    {


        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Scale = 0.2f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            Main.npcFrameCount[NPC.type] = 6;
            //.setdefault("Core Sage");


        }

        public override void SetDefaults()
        {
            Console.WriteLine("a");
            NPC.lifeMax = 1000;
            NPC.damage = 10;
            NPC.defense = 8;
            NPC.knockBackResist = 0f;

            NPC.noTileCollide = false;

            NPC.scale = 1f;
            NPC.Size = new Vector2(150, 150);

            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.boss = true;

            Music = MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/CoreMiniboss");

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("A powerful warlock that can focus the energy radiating from living crystals in powerful magic.")
            });
        }
        bool secondPhase = false; 
        enum ActionState
        {
            Idling,
            Lazer1,
            Lazer2,
            Projectiles,
            Thorns1,
            Thorns2,
            Thorns3,
            Roar




        }
        private int teleport;

        public bool TpBack { get; private set; }

         ActionState state = ActionState.Idling;
        public bool enraged;
        public float Phase;
        private bool initialize;
        public float addDistance = 90;
        private bool spawned = false;
        public int screamtimer;
        Rectangle hitboxExtension;
        private bool reverse;
        private double framespeed;
        private int repeatCounter;
        private float pitch;

        public override void AI()
        {
            Console.WriteLine("ai");

            Player player = Main.player[NPC.target];
            hitboxExtension = new Rectangle((int)NPC.Top.X, (int)NPC.Top.Y, 300, 400);
            if (hitboxExtension.Intersects(player.Hitbox))
            {
                screamtimer++;
                if (screamtimer == 15)
                {
                    player.velocity.X -= 30;
                    player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 25;
                    DivergencyDraw.SpawnRing(NPC.Top, Color.LimeGreen, 0.13f * 2f, 0.9f * 2, 2 * 2);
                    DivergencyDraw.SpawnRing(NPC.Top, Color.LimeGreen, 0.13f * 2, 0.9f * 2, 2 * 2);
                    DivergencyDraw.SpawnRing(NPC.Top, Color.LimeGreen, 0.13f * 2, 0.9f * 2, 2 * 2);
                    SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/growl") with { Pitch = Main.rand.NextFloat(-0.3f, 0.3f), MaxInstances = 1 }, NPC.Center);
                    enraged = true;
                    screamtimer = 0;

                }

            }

            for (int d = 0; d < Main.maxProjectiles; d++)
            {
                Projectile projectile = Main.projectile[d];

                if (projectile.type == ProjectileID.Hook || (projectile.type == ProjectileID.AmberHook || projectile.type == ProjectileID.AntiGravityHook
                  || projectile.type == ProjectileID.BatHook || projectile.type == ProjectileID.CandyCaneHook || projectile.type == ProjectileID.ChristmasHook || projectile.type == ProjectileID.DualHookBlue || projectile.type == ProjectileID.DualHookRed || projectile.type == ProjectileID.FishHook || projectile.type == ProjectileID.GemHookAmethyst
                  || projectile.type == ProjectileID.GemHookDiamond || projectile.type == ProjectileID.GemHookEmerald || projectile.type == ProjectileID.GemHookRuby || projectile.type == ProjectileID.GemHookSapphire || projectile.type == ProjectileID.GemHookTopaz
                  || projectile.type == ProjectileID.IlluminantHook || projectile.type == ProjectileID.LunarHookNebula || projectile.type == ProjectileID.LunarHookSolar || projectile.type == ProjectileID.LunarHookStardust || projectile.type == ProjectileID.LunarHookVortex || projectile.type == ProjectileID.QueenSlimeHook
                  || projectile.type == ProjectileID.SlimeHook || projectile.type == ProjectileID.SquirrelHook || projectile.type == ProjectileID.StaticHook || projectile.type == ProjectileID.TendonHook || projectile.type == ProjectileID.ThornHook || projectile.type == ProjectileID.TrackHook || projectile.type == ProjectileID.WoodHook || projectile.type == ProjectileID.WormHook))
                {
                    projectile.Kill();
                }
            }
                if (!initialize)
                {

                    initialize = true;

                }
                else
                {
                    if (NPC.life <= NPC.lifeMax / 2)
                    {
                        enraged = true;
                    }

                    if (NPC.HasValidTarget && !player.dead)
                    {

                        NPC.TargetClosest();
                    }

                    NPC.velocity.Y = 0f;



                    NPC.TargetClosest(true);


                    if (state == ActionState.Idling)
                    {
                        NPC.ai[0]++;

                        if (NPC.ai[0] == 120)
                        {
                            WeightedRandom<ActionState> attack = new WeightedRandom<ActionState>();
                            attack.Add(ActionState.Thorns1, 1f);
                            attack.Add(ActionState.Thorns2, 1f);
                            attack.Add(ActionState.Thorns3, 1f);
                            state = attack.Get();

                            NPC.ai[0] = 0;

                        }
                    }

                    //////THORNS AFTER ANOTHER :)
                    if (state == ActionState.Thorns1)
                    {

                        NPC.ai[0]++;
                        if (NPC.ai[0] == 25)
                        {

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(addDistance, 0), Vector2.Zero, ModContent.ProjectileType<Projectiles.Hostile.BigGuy.BigGuySpike>(), NPC.damage, 0, ai1: 1);
                            if (reverse)
                            {
                                addDistance -= 90;

                            }
                            else
                            {
                                addDistance += 90;

                            }
                            NPC.ai[0] = 0;
                        }
                        if (enraged)
                        {
                            if (addDistance == 1440)
                            {
                                reverse = true;
                            }

                            if (addDistance == 0)
                            {
                                if (Main.rand.NextBool(2))
                                {
                                    state = ActionState.Lazer1;
                                }
                                else
                                {
                                    state = ActionState.Projectiles;
                                }
                                NPC.ai[0] = 0;
                                addDistance = 0;
                                reverse = false;
                            }
                        }
                        else
                        {
                            if (addDistance == 1440)
                            {
                                if (Main.rand.NextBool(2))
                                {
                                    state = ActionState.Lazer1;
                                }
                                else
                                {
                                    state = ActionState.Projectiles;
                                }
                                NPC.ai[0] = 0;
                                addDistance = 0;
                            }
                        }
                    }
                    ////THORNS SET POS

                    if (state == ActionState.Thorns2)
                    {
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 180)
                        {
                            for (int i = 0; i < 9; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(addDistance, 0), Vector2.Zero, ModContent.ProjectileType<Projectiles.Hostile.BigGuy.BigGuySpike>(), NPC.damage, 0, ai2: 2);
                                addDistance += 180;

                            }
                        }
                        if (NPC.ai[0] == 300)
                        {
                            addDistance = 90;
                            for (int i = 0; i < 8; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(addDistance, 0), Vector2.Zero, ModContent.ProjectileType<Projectiles.Hostile.BigGuy.BigGuySpike>(), NPC.damage, 0, ai2: 2);
                                addDistance += 180;

                            }
                        }
                        if (enraged)
                        {
                            if (NPC.ai[0] == 420)
                            {
                                addDistance = 0;

                                for (int i = 0; i < 9; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(addDistance, 0), Vector2.Zero, ModContent.ProjectileType<Projectiles.Hostile.BigGuy.BigGuySpike>(), NPC.damage, 0, ai2: 2);
                                    addDistance += 180;

                                }
                            }
                            if (NPC.ai[0] == 540)
                            {
                                addDistance = 90;
                                for (int i = 0; i < 8; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(addDistance, 0), Vector2.Zero, ModContent.ProjectileType<Projectiles.Hostile.BigGuy.BigGuySpike>(), NPC.damage, 0, ai2: 2);
                                    addDistance += 180;


                                }
                            }

                            if (NPC.ai[0] == 600)
                            {
                                if (Main.rand.NextBool(2))
                                {
                                    state = ActionState.Lazer1;
                                }
                                else
                                {
                                    state = ActionState.Projectiles;
                                }
                                NPC.ai[0] = 0;
                                addDistance = 0;
                            }
                        }
                        else
                        {
                            if (NPC.ai[0] == 360)
                            {
                                if (Main.rand.NextBool(2))
                                {
                                    state = ActionState.Lazer1;
                                }
                                else
                                {
                                    state = ActionState.Projectiles;
                                }
                                NPC.ai[0] = 0;
                                addDistance = 0;
                            }
                        }


                    }
                    ////RANDOM THORNS
                


                if (state == ActionState.Thorns3)
                {
                    NPC.ai[0]++;
                    NPC.ai[1]++; //real timer
                    if (enraged)
                    {
                        if (NPC.ai[1] == 100)
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(Main.rand.NextFloat(20, 1440), 0), Vector2.Zero, ModContent.ProjectileType<Projectiles.Hostile.BigGuy.BigGuySpike>(), NPC.damage, 0, ai2: 2);

                            }
                            NPC.ai[1] = 0;
                        }
                    }
                    else
                    {
                        if (NPC.ai[1] == 120)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(Main.rand.NextFloat(20, 1440), 0), Vector2.Zero, ModContent.ProjectileType<Projectiles.Hostile.BigGuy.BigGuySpike>(), NPC.damage, 0, ai2: 2);

                            }
                            NPC.ai[1] = 0;
                        }
                    }


                    if (NPC.ai[0] == 1000)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            state = ActionState.Lazer1;
                        }
                        else
                        {
                            state = ActionState.Projectiles;
                        }
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;

                        addDistance = 0;
                    }


                }

                ///FOLOWWING LASER

                if (state == ActionState.Lazer1)
                {
                    NPC.ai[1]++; //real timer

                    if (NPC.ai[0] < 120)
                    {
                        if (NPC.ai[1] == 30)
                        {
                            DivergencyDraw.SpawnRingReverse(NPC.Center, Color.LimeGreen, 0.13f / 5, 0.9f / 5, 2 / 5);
                            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/BeamWindUpSmall") with { Pitch = pitch, MaxInstances = 10 }, NPC.Center);
                            pitch += 0.1f;
                            NPC.ai[1] = 10;
                        }
                    }

                    NPC.ai[0]++;
                    if (NPC.ai[0] == 120)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/BeamWindUp") with { Pitch = pitch, MaxInstances = 10 }, NPC.Center);
                        pitch = 0;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Hostile.BigGuy.LaserBeam>(), NPC.damage, 0, ai0: Main.LocalPlayer.whoAmI, ai1: NPC.whoAmI);
                    }
                    if (NPC.ai[0] == 160)
                    {

                       


                    }

                    if (NPC.ai[0] == 180)
                    {

                        if (repeatCounter < 3 && enraged)
                        {

                            state = ActionState.Lazer1;
                            repeatCounter++;
                            NPC.ai[0] = 100;

                        }
                        else
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            repeatCounter = 0;
                            state = ActionState.Idling;

                        }



                    }
                }
                if (state == ActionState.Projectiles)
                {
                    NPC.ai[2]++;
                    NPC.ai[0]++;

                    if (NPC.ai[0] < 120)
                    {
                        if (NPC.ai[2] == 30)
                        {
                            DivergencyDraw.SpawnRingReverse(NPC.Center, Color.LimeGreen, 0.13f / 5, 0.9f / 5, 2 / 5);
                            NPC.ai[2] = 0;
                        }
                    }
                    if (NPC.ai[0] > 120)
                    {
                        NPC.ai[1]++; //real timer

                        if (!enraged)
                        {
                            if (NPC.ai[1] == 25)
                            {
                                Vector2 newVelocity = new Vector2(3).RotatedByRandom(MathHelper.ToRadians(30));

                                // Decrease velocity randomly for nicer visuals.
                                newVelocity *= 1.5f - Main.rand.NextFloat(0.4f);

                                // Create a projectile.
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Top, NPC.DirectionTo(player.Center) * newVelocity, ModContent.ProjectileType<CorescillationProj2>(), NPC.damage, 0);
                                NPC.ai[1] = 0; //real timer

                            }
                        }
                        else
                        {
                            if (NPC.ai[1] == 15)
                            {
                                Vector2 newVelocity = new Vector2(4).RotatedByRandom(MathHelper.ToRadians(30));

                                // Decrease velocity randomly for nicer visuals.
                                newVelocity *= 1.9f - Main.rand.NextFloat(0.5f);

                                // Create a projectile.
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Top, NPC.DirectionTo(player.Center) * newVelocity, ModContent.ProjectileType<CorescillationProj2>(), NPC.damage, 0);
                                NPC.ai[1] = 0; //real timer

                            }
                        }

                    }
                    if (NPC.ai[0] >= 480)
                    {
                        NPC.ai[0] = 0; //real timer
                        NPC.ai[1] = 0; //real timer
                        NPC.ai[2] = 0;

                        state = ActionState.Idling;
                    }


                }
            }
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            npcHitbox.X = (int)(NPC.Size.X / 2 - 150 / 2);
            npcHitbox.Y = (int)(NPC.Size.Y / 2 - 150 / 2);
            return false;
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
            NPC.frameCounter++;
            if (enraged)
            {
                framespeed = 6;
            }
            else
            {
                framespeed = 9;
            }
            if (NPC.frameCounter >= framespeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * 6)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.hide)
            {
                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Texture2D glow = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/BigGuyGlow").Value;


                Texture2D a = TextureAssets.Npc[Type].Value;
                Main.EntitySpriteDraw(a, NPC.VisualPosition - screenPos + new Vector2(-30, 34), NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, 1f, effects, 0);
                Main.EntitySpriteDraw(glow, NPC.VisualPosition - screenPos + new Vector2(-30, 34), NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, 1f, effects, 0);

            }
            return false;
        }
    }

}
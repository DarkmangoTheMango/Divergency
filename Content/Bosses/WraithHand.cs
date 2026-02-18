using System;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Divergency.Content.NPCs.LivingGrove;
using Divergency.Content.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Divergency.Common.Helpers;
using Divergency.Content.Projectiles.Hostile;
using Terraria.Utilities;
using System.Diagnostics.Metrics;
using Divergency.Content.Buffs;



namespace Divergency.Content.Bosses
{
    public class WraithHand : ModNPC
    {
        public static float Magnitude(Vector2 mag)
        {
            return (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);
        }




        public float State = 0;

        public float AI_Timer;
        public float Timer;
        private bool SoundPlayed;
        private int frametimer;

        

        public override void SetStaticDefaults()
        {

            Main.npcFrameCount[NPC.type] = 4; // make sure to set this for your modNPCs.
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 500;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 90;
            NPC.height = 110;
            NPC.value = Item.buyPrice(0, 3, 0, 0);
            // NPC.dontTakeDamage = true;
            NPC.friendly = false;
            //NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            //NPC.dontTakeDamageFromHostiles = true;
            NPC.behindTiles = false;
            NPC.ShowNameOnHover = false;


        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
      

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("hands down")
            });
        }
        public NPC cachedNPC;
        private int initialdamage;
        private bool spawned;

        private float Y;
        private int X;

        private Vector2 TopLeft;
        private Vector2 TopRight;
        private Vector2 BottomRight;
        private Vector2 BottomLeft;

        
        public override void AI()

        {
            if (!spawned)
            {
                bool found = false;

                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is WraithBody)
                    {
                        cachedNPC = taggedNPC;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    NPC.active = false;
                    return;
                }
                initialdamage = NPC.damage;

                TopLeft = cachedNPC.Center + new Vector2(-700, -400);
                TopRight = cachedNPC.Center + new Vector2(700, -500);
                BottomLeft = cachedNPC.Center + new Vector2(-700, 400);
                BottomRight = cachedNPC.Center + new Vector2(700, 400);

                spawned = true;
            }
            if (!cachedNPC.active)
            {
                NPC.active = false;
            }

            if (spawned)
            {
                // DONT FORGET TO MAKE INVINCIBLE PLAYEr

                if (cachedNPC.ai[3] == 3)
                {
                    Vector2 WraithPos = Vector2.Zero;
                    Vector2 moveTo = TopLeft;
                    
                    if (NPC.ai[0] == 0)
                    {
                        WraithPos = TopLeft;
                        NPC.knockBackResist = 0.1f;

                    }
                    if (NPC.ai[0] == 60)
                    {
                        NPC.damage = initialdamage;
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Wraith/Indicator")

                        {
                            Pitch = Main.rand.NextFloat(1f),
                            Volume = 0.7f,
                            MaxInstances = 5,

                        });
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0), ModContent.ProjectileType<WraithIndicator>(), 0, 0);
                            
                    }


                    Player player = Main.player[NPC.target];
                    if (!incircle)
                    { NPC.ai[0]++; }

                    //circle melee
                    if (NPC.Distance(player.Center) >  140)
                    {
                        incircle = false;
                        NPC.immortal = true;
                    }
                    else
                    {
                        incircle = true;
                        NPC.immortal = false;

                    }
                    if (NPC.ai[0] == 120)
                    {
                      
                        moveTo = TopRight;
                       
         
                            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = Main.rand.NextFloat(0.1f, 2f), MaxInstances = 10 }, NPC.Center);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(player.Center) * 7f, ModContent.ProjectileType<GuardianBeam>(), NPC.damage, 3f, 0);
                        NPC.ai[0] = 0;

                    }

                    moveTo = NPC.position;

              
             

                    Vector2 vector; float speed; float turnResistance = 10f; bool toNPC = false;

                    NPC.spriteDirection = -NPC.direction;
                    if (NPC.direction == 1)
                    {
                        NPC.rotation += 0.03f;

                    }
                    else
                    {
                        NPC.rotation += 0.03f;

                    }
                    speed = 0;

                    Vector2 move = moveTo - NPC.Center;
                    float magnitude = Magnitude(move);
                    if (magnitude > speed)
                    {
                        move *= speed / magnitude;
                    }

                    move = (NPC.velocity * turnResistance + move) / (turnResistance + 2f);
                    magnitude = Magnitude(move);
                    if (magnitude > speed)
                    {
                        move *= speed / magnitude;
                    }

                    NPC.velocity = move;
                }


                if (cachedNPC.ai[3] == 0)
                {
                    NPC.knockBackResist = 0f;

                    NPC.ai[0] = 0;
                    NPC.frame.Y = 0 * NPC.frame.Height;
                    Vector2 vector; float speed; float turnResistance = 10f; bool toNPC = false;
                    Vector2 WraithPos = cachedNPC.Center;
                    NPC.spriteDirection = -NPC.direction;
                    NPC.rotation = 0;
                    speed = NPC.Distance(WraithPos) / 6;

                    Vector2 moveTo = cachedNPC.Center + new Vector2(-50 * cachedNPC.direction, 0);
                    Vector2 move = moveTo - NPC.Center;
                    float magnitude = Magnitude(move);
                    if (magnitude > speed)
                    {
                        move *= speed / magnitude;
                    }

                    move = (NPC.velocity * turnResistance + move) / (turnResistance + 2f);
                    magnitude = Magnitude(move);
                    if (magnitude > speed)
                    {
                        move *= speed / magnitude;
                    }

                    NPC.velocity = move;
                    NPC.TargetClosest(true);
                }
                if (cachedNPC.ai[3] == 1)
                {
                    Player player = Main.player[NPC.target];


                    NPC.rotation = NPC.velocity.ToRotation();
                    NPC.frame.Y = 0 * NPC.frame.Height;

                    NPC.ai[0]++;
                    NPC.TargetClosest(true);
                    NPC.rotation = NPC.velocity.ToRotation();

                    NPC.velocity *= 0.76f;

                    if (NPC.ai[0] == 1)
                    {
                        NPC.damage = 0;
                        NPC.velocity += NPC.DirectionTo(player.Center) * 205;



                    }
                    if (NPC.ai[0] == 10)
                    {
                        NPC.damage = initialdamage;
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Wraith/Indicator")

                        {
                            Pitch = Main.rand.NextFloat(1f),
                            Volume = 0.7f,
                            MaxInstances = 5,

                        });
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0), ModContent.ProjectileType<WraithIndicator>(), 0, 0);
                    }
                    {
                        if (NPC.ai[0] == 15 || NPC.ai[0] == 20)
                        {
                            SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Wraith/Indicator")

                            {
                                Pitch = Main.rand.NextFloat(1f, 3f),
                                Volume = 0.7f,
                                MaxInstances = 5,

                            });
                        }
                    }


                }
                if (cachedNPC.ai[3] == 2)
                {
                    Player player = Main.player[NPC.target];

                    NPC.ai[0]++;
                    if (NPC.ai[0] == 1)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0), ModContent.ProjectileType<WraithIndicator2>(), 0, 0);

                    }
                    NPC.frame.Y = 0 * NPC.frame.Height;
                    Vector2 vector; float speed; float turnResistance = 10f; bool toNPC = false;
                    Vector2 WraithPos = cachedNPC.Center;
                    NPC.spriteDirection = -NPC.direction;
                    if (NPC.direction == 1)
                    {
                        NPC.rotation = 45;

                    }
                    else
                    {
                        NPC.rotation = 45;

                    }
                    speed = NPC.Distance(WraithPos) / 2;

                    Vector2 moveTo = cachedNPC.Center + new Vector2(X, Y);
                    Vector2 move = moveTo - NPC.Center;
                    float magnitude = Magnitude(move);
                    if (magnitude > speed)
                    {
                        move *= speed / magnitude;
                    }

                    move = (NPC.velocity * turnResistance + move) / (turnResistance + 2f);
                    magnitude = Magnitude(move);
                    if (magnitude > speed)
                    {
                        move *= speed / magnitude;
                    }

                    NPC.velocity = move;
                    NPC.TargetClosest(true);

                    //movement
                    if (NPC.ai[0] < 55)
                    {
                        Y = -100;
                        X = -50 * cachedNPC.direction;
                    }
                    
                    if (NPC.ai[0] > 55 && NPC.ai[0] <= 90)
                    {
                        Y = 100;
                        X = -50 * cachedNPC.direction;

                    }

                    //sounds 
                    if (NPC.ai[0] == 50)
                    {
                        NPC.damage = initialdamage;
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Wraith/Indicator")

                        {
                            Pitch = Main.rand.NextFloat(1f),
                            Volume = 0.7f,
                            MaxInstances = 5,

                        });
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0), ModContent.ProjectileType<WraithIndicator>(), 0, 0);
                    }
                    {
                        if (NPC.ai[0] == 55 || NPC.ai[0] == 60)
                        {
                            SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Wraith/Dash")

                            {
                                Pitch = Main.rand.NextFloat(0.5f, 2f),
                                Volume = 0.675f,
                                MaxInstances = 5,

                            });
                            NPC.velocity.X += 100 * NPC.direction;
                            NPC.velocity.Y -= 10 * NPC.direction;

                            SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Wraith/Indicator")

                            {
                                Pitch = Main.rand.NextFloat(1f, 3f),
                                Volume = 0.7f,
                                MaxInstances = 5,

                            });
                        }
                    }
                }
            if (cachedNPC.ai[3] == 4)
                {
                    Player player = Main.player[NPC.target];

                    NPC.ai[0]++;
                    if (NPC.ai[0] == 1)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0), ModContent.ProjectileType<WraithIndicator2>(), 0, 0);
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Wraith/Indicator")

                        {
                            Pitch = Main.rand.NextFloat(1f, 3f),
                            Volume = 0.7f,
                            MaxInstances = 5,

                        });
                    }
                    NPC.frame.Y = 0 * NPC.frame.Height;
                    Vector2 vector; float speed; float turnResistance = 10f; bool toNPC = false;
                    Vector2 WraithPos = cachedNPC.Center;
                    NPC.spriteDirection = -NPC.direction;
                    NPC.rotation = cachedNPC.rotation;
                    speed = NPC.Distance(WraithPos);

                    Vector2 moveTo = cachedNPC.Center + new Vector2(NPC.direction * 120, 0); ;
                    Vector2 move = moveTo - NPC.Center;
                    float magnitude = Magnitude(move);
                    if (magnitude > speed)
                    {
                        move *= speed / magnitude;
                    }

                    move = (NPC.velocity * turnResistance + move) / (turnResistance + 2f);
                    magnitude = Magnitude(move);
                 
                    
                    NPC.velocity = move;
                    NPC.TargetClosest(true);
                }
                
                    
                






            }

        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (cachedNPC?.active == true)
            {
                if (cachedNPC.ai[3] == 3)
                {
                    return false;

                }
                else
                {
                    return true;

                }
            }

            return false;
        }
        public override void FindFrame(int frameHeight)
        {


        }
        private byte _hitShake;
        private int counter;
        private int counter1;

        public bool incircle { get; private set; }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<CoreBurn>(), 120);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            byte shake = (byte)MathHelper.Clamp(hit.Damage / 8, 4, 10);
            if (shake > _hitShake)
            {
                _hitShake = shake;
            }

            if (NPC.life <= 0)
            {
                cachedNPC.ai[4] = 2; //hand is dead
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Bosses/WraithHand").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            if (_hitShake > 0)
            {
                position += new Vector2(Main.rand.Next(-_hitShake, _hitShake), Main.rand.Next(-_hitShake, _hitShake));
                _hitShake--;
            }
            if (NPC.ai[3] == 1)
            {
                spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
                var fadeMult = 1f / NPCID.Sets.TrailCacheLength[Type];
                for (int i = 0; i < NPC.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(texture, NPC.oldPos[i] - Main.screenPosition + NPC.Size / 2 + new Vector2(0, 0), NPC.frame, Color.White * (1f - fadeMult * i), NPC.rotation, NPC.Size / 2, NPC.scale, spriteEffects, 0f);
                }
            }
            else
            {
                spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 1f);

            }
            
                
            
                return false;
        }
    }
}
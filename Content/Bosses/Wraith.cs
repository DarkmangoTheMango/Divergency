
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Common;
using Divergency.Content.Projectiles;
using Divergency.Common.Helpers;
using Terraria.Utilities;
using Mono.Cecil;
using Terraria.ModLoader.UI.ModBrowser;
using Divergency.Content.Particles;
using Divergency.Content.Dusts;
using Divergency.Common.Players;


namespace Divergency.Content.Bosses
{
    public class Wraith : ModNPC
    {
        //main body, responsible for body attacks
        //npc ai 0 is the normal timer
        //npc ai 2 is secondary timer
        //npc ai 3 are hand commands // 1 move forward the player and slash back, gain trail
                                     // 2 is prepare and slash in a bow
        //ai 4 is the command to regenerate her hand when shes in floating phase
        //ai4 = 4 or higher is for hand movement while breath
        private int frame = 0;
        private int frameTimer = 0;
        public float State = 0;
        public byte phase = 1;
        private int framerate;
        public bool openMouth;
        public bool closeMouth;
        private byte _hitShake;
        private bool spawned;
        private bool frameXMove;
        private int counter;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 45;
            NPCID.Sets.TrailingMode[NPC.type] = 3;

            Main.npcFrameCount[NPC.type] = 25; // make sure to set this for your modNPCs.
      
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 5000;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.knockBackResist = 0f;
            NPC.width = 122;
            NPC.height = 144;
            // NPC.dontTakeDamage = true;
            NPC.friendly = false;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            
            //NPC.dontTakeDamageFromHostiles = true;
            NPC.behindTiles = false;

            Music = MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/DuelOfTheRoots");

        }
        private enum Phase
        {
            Float,
            HandDash,
            HandDash2,
            RegenerateHand,
            FireBreathFollowUp,
            FireBreath,
            

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Bosses/Wraith").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ?   SpriteEffects.None : SpriteEffects.FlipVertically;
            if (_hitShake > 0)
            {
                position += new Vector2(Main.rand.Next(-_hitShake, _hitShake), Main.rand.Next(-_hitShake, _hitShake));
                _hitShake--;
            }
                spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return false;
        }
        public override void FindFrame(int blabla)
        {   
            int frameHeight = 180;
            int frameWidth = 180;
            NPC.frame.Width = 180;
            NPC.frame.Height = 180;

            //idle anime
            if (State == (float)Phase.Float)
            {
                framerate = 5;
                NPC.frameCounter++;

                if (NPC.frameCounter >= framerate)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.X += frameHeight;

                    if  (NPC.frame.X >= frameWidth * 4)
                    {
                        NPC.frame.X = 0 * frameHeight;
                    }
                }

            }
            if (State == (float)Phase.RegenerateHand && !laugh)
            {
                NPC.frame.X = NPC.frame.Width * 3;
                NPC.frame.Y = 4 * frameHeight;
            }

            if (laugh)
            {
                NPC.frame.Y = 900;
                NPC.frameCounter++;

                if (NPC.frameCounter >= framerate)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.X += frameWidth;

                    if (NPC.frame.X >= frameWidth * 4)
                    {
                        NPC.frame.X = 2 * frameWidth;
                    }
                }
            }

            //mouth open
            if (openMouth)
            {
                
                if (NPC.frame.X == 2 * frameWidth && NPC.frame.Y == 360)
                {
                    openMouth = false;
                }
                else
                {
                    NPC.frameCounter++;

                    if (NPC.frameCounter >= framerate)
                    {
                        NPC.frameCounter = 0;

                        NPC.frame.X += frameWidth;



                        if (NPC.frame.X >= frameWidth * 4 && NPC.frame.Y < 360)
                        {
                            NPC.frame.X = 0 * frameWidth;
                            NPC.frame.Y += 180;
                        }   
                    }
                
                    


                }
            }
            if (closeMouth)
            {

                if (NPC.frame.X == 1 * frameWidth && NPC.frame.Y == 720)
                {
                    closeMouth = false;
                }
                else
                {
                    NPC.frameCounter++;

                    if (NPC.frameCounter >= framerate)
                    {
                        NPC.frameCounter = 0;

                        NPC.frame.X += frameWidth;



                        if (NPC.frame.X >= frameWidth * 4)
                        {
                            NPC.frame.X = 0 * frameWidth;
                            NPC.frame.Y += 180;
                        }
                    }




                }
                
            }
         
          



        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            //SoundEngine.PlaySound(SoundID.Item34 with { Volume = 1f, Pitch = Main.rand.NextFloat(0.5f, 2f), MaxInstances = 400 });
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Wraith/WraithHurt")

            {
                Pitch = Main.rand.NextFloat(0.5f, 2f),
                Volume = 0.5f,
                MaxInstances = 5,

            });
            byte shake = (byte)MathHelper.Clamp(hit.Damage / 8, 4, 10);
            if (shake > _hitShake)
            {
                _hitShake = shake;
            }
        }
        public override void AI()
        {
            NPC.TargetClosest();
            if (!spawned)
            {
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WraithCore>(), 0, NPC.whoAmI);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WraithHand>(), 0, NPC.whoAmI);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WraithBody>(), 0, NPC.whoAmI);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BodyOrb>(), 0, NPC.whoAmI);
              
                    spawned = true;
            }
            else
            {
                switch (State)
                {
                    case (float)Phase.Float:
                        Float();
                        break;
                    case (float)Phase.HandDash:
                        HandDash();
                        break;
                    case (float)Phase.HandDash2:
                        HandDash2();
                        break;
                    case (float)Phase.RegenerateHand:
                        RegenerateHand();
                        break;
                    case (float)Phase.FireBreathFollowUp:
                        FireBreathFollowUp();
                        break;
                    case (float)Phase.FireBreath:
                        FireBreath();
                            break;

                }
            }
          
            if (NPC.life <= NPC.lifeMax / 2)
            {
                phase = 2;
            }

        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (State == (float)Phase.Float || noContactDamage)
            {
                return false;

            }
            else
            {
                return true;

            }
        }
    
        private void Float()
        {
            NPC.immortal = false;
            NPC.oldRot[0] = NPC.velocity.ToRotation();
            for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            {
                NPC.oldRot[i] = NPC.oldRot[i - 1];
            }
            NPC.ai[0]++;
            Player player = Main.player[NPC.target];

            NPC.direction = NPC.spriteDirection = (NPC.velocity.X >= 0f) ? 1 : -1;

            NPC.ai[3] = 0;

            NPC.rotation = NPC.velocity.ToRotation();       
            if (NPC.ai[4] == 2)
            {
                HandDead = true;
            }
            if (HandDead)
            {
                State = (float)Phase.RegenerateHand;
                NPC.frame.X = 0;
                NPC.frame.Y = 0;
            }
            if (phase == 2)
            {
                NPC.Move(player.Center, 4f);
            }
            else
            {
                    NPC.Move(player.Center, 3f);
            }
            if (NPC.ai[0] == 120)
            {
                WeightedRandom<Phase> phase = new WeightedRandom<Phase>();
                phase.Add(Phase.HandDash, 1f);
                phase.Add(Phase.HandDash2, 2f);
                phase.Add(Phase.FireBreath, 0.8f);

                //phase.Add(Phase.FireBreathFollowUp, 1);
                State = (float)phase.Get();
                NPC.ai[0] = 0;

            }

        }
        float maxSpeed = 2f;
        private bool noContactDamage;
        private bool laugh;

        public bool HandDead { get; private set; }

        private void HandDash()
        {
            Player target = Main.player[NPC.target];
            NPC.ai[0]++;
            if (NPC.ai[0] == 1)
            {
                openMouth = true;
                NPC.frame.X += 180;
                framerate = 5;
            }
            if (NPC.ai[0] < 90 || (NPC.ai[0] > 180 && NPC.ai[0] < 210)) //move a bit back if the player is too close
            {
                if (NPC.Distance(target.Center) <= 400f) { NPC.velocity -= NPC.DirectionTo(target.Center) * 0.1f; }
                else { NPC.velocity *= 0.99f; }
                if (NPC.Center.Y >= target.Center.Y) { NPC.velocity.Y -= 0.1f; }

                if (NPC.velocity.X >= maxSpeed) { NPC.velocity.X = maxSpeed; }
                if (NPC.velocity.X <= -maxSpeed) { NPC.velocity.X = -maxSpeed; }
                if (NPC.velocity.Y >= maxSpeed) { NPC.velocity.Y = maxSpeed; }
                if (NPC.velocity.Y <= -maxSpeed) { NPC.velocity.Y = -maxSpeed; }

            }
            else
            {
                NPC.direction = NPC.spriteDirection = (NPC.velocity.X >= 0f) ? 1 : -1;


                NPC.rotation = NPC.velocity.ToRotation();
                NPC.velocity += NPC.DirectionTo(target.Center)  / 35;
                NPC.velocity *= 0.93f;
            }
            if (NPC.ai[0] == 60)
            {
                NPC.ai[3] = 1;

            }
            if (NPC.ai[0] == 90 || NPC.ai[0] == 150 || NPC.ai[0] == 260 || NPC.ai[0] == 310 || NPC.ai[0] == 360 || NPC.ai[0] == 390)
            {
                noContactDamage = false;

                if (NPC.ai[0] == 90 || NPC.ai[0] == 150)
                {
                    NPC.velocity += NPC.DirectionTo(target.Center) * 25;

                }
                if (NPC.ai[0] == 260 || NPC.ai[0] == 310 || NPC.ai[0] == 360 || NPC.ai[0] == 390 || NPC.ai[0] == 420)
                {
                    NPC.velocity += NPC.DirectionTo(target.Center) * 3;
                }

                closeMouth = true;
                NPC.ai[3] = 0;
                SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Wraith/Dash")

                {
                    Pitch = Main.rand.NextFloat(0.5f, 2f),
                    Volume = 0.5f,
                    MaxInstances = 5,

                });

            }
            if (NPC.ai[0] == 120 || NPC.ai[0] == 250 || NPC.ai[0] == 300 || NPC.ai[0] == 350 || NPC.ai[0] == 380)
            {
                noContactDamage = true;

                NPC.ai[3] = 1;
                NPC.frame.X = 180;
                NPC.frame.Y = 0;
                openMouth = true;
            }
            if (NPC.ai[0] == 180)
            {
              
            }

      
            if (phase == 1)
            {
                if (NPC.ai[0] == 240)
                {

                    State = (float)Phase.Float;
                    NPC.frame.X = 0;
                    NPC.frame.Y = 0;
                    openMouth = false;
                    closeMouth = false;
                    NPC.ai[0] = 0;

                }
            }
            else
            {
                if (NPC.ai[0] == 450)
                {

                    State = (float)Phase.Float;
                    NPC.frame.X = 0;
                    NPC.frame.Y = 0;
                    openMouth = false;
                    closeMouth = false;
                    NPC.ai[0] = 0;

                }
            }
           



            NPC.oldRot[0] = NPC.velocity.ToRotation();
            for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            {
                NPC.oldRot[i] = NPC.oldRot[i - 1];
            }

        }
        private void HandDash2()
        {

            Player target = Main.player [NPC.target];
            NPC.ai [0]++;
            if (NPC.ai[0]  == 1)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Wraith/Indicator")

                {
                    Pitch = Main.rand.NextFloat(1f, 3f),
                    Volume = 0.7f,
                    MaxInstances = 5,

                });
            
                    NPC.ai[3] = 2;
               
  

            }
            if (NPC.ai[0] == 60)
            {
                NPC.velocity += NPC.DirectionTo(target.Center) * 13.5f;
                noContactDamage = false;

            }
            if (NPC.ai[0] > 59)
            {
                NPC.velocity *= 0.93f;
                NPC.frame.Y = 1080;
                NPC.frame.X = 360;

            }
            else
            {
                NPC.frame.Y = 1080;
                NPC.frame.X = 180;
                NPC.Move(target.Center, 4.6f);
                noContactDamage = true;

            }
            if (NPC.ai[0] == 90)
            {

                if (phase == 2)
                {
                    State = (float)Phase.FireBreathFollowUp;
                }
                else
                {
                    State = (float)Phase.Float;

                }
                 NPC.frame.X = 0;
                NPC.frame.Y = 0;
                openMouth = false;
                closeMouth = false;
                NPC.ai[0] = 0;
                NPC.ai[3] = 0;


            }
        }
        private void RegenerateHand()
        {
            NPC.immortal = false;
            regenerating = true;
            Player target = Main.player[NPC.target];
            NPC.direction = NPC.spriteDirection = (NPC.velocity.X >= 0f) ? 1 : -1;
            
            NPC.Move(target.Center, 0.2f);
            NPC.ai[0]++;
            Vector2 spawnPosition = new Vector2((int)NPC.Center.X + (75 * NPC.direction), (int)NPC.Center.Y);
            NPC.rotation = NPC.velocity.ToRotation();
         
            NPC.velocity *= 0.9f;
            if (NPC.ai[0] < 360)
            {

                NPC.ai[4] = 1;

                float rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < 2; i++)
                {
                    ParticleManager.NewParticle<FlareLineParticle>(spawnPosition + new Vector2(128f, 0f).RotatedBy(rotation), new Vector2(4f, 0f).RotatedBy(rotation + MathHelper.Pi), new Color(0.50f, 2.05f, 0.5f, 0), 0.9f, Main.rand.NextFloat(0.8f, 1.1f));
                }

                float radius = 2;
                int numberOfDusts = 20;
                for (int i = 0; i < 1; i++)
                {
                    Dust dust = Dust.NewDustPerfect(spawnPosition, ModContent.DustType<Glow>(), new Vector2(4f, 0f).RotatedBy(rotation + MathHelper.Pi), 0, default, 2f);
                    Dust.NewDustPerfect(spawnPosition, ModContent.DustType<Glow>(), new Vector2(1f, 0f).RotatedBy(rotation + MathHelper.Pi), 0, new Color(109, 223, 94), 1.4f);

                    dust.noGravity = true;
                }
            }
            if (NPC.ai[0] == 360)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, spawnPosition);
                regenerating = false;
                for (int i = 0; i < 25; i++)
                {
                    Dust dust = Dust.NewDustPerfect(spawnPosition, DustID.GemEmerald, Main.rand.NextVector2Circular(1f, 1f) * 30, 0, default, 2f);
                    Dust.NewDustPerfect(spawnPosition, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(1f, 1f) * 10, 0, new Color(109, 223, 94), 1f);
                    Dust.NewDustPerfect(spawnPosition, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 10, 0, new Color(109, 223, 94), 1f);


                    dust.noGravity = true;
                }
                DivergencyDraw.SpawnRing(spawnPosition, new Color(109, 223, 94));

                HandDead = false;
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (75 * NPC.direction), (int)NPC.Center.Y, ModContent.NPCType<WraithHand>(), 0, NPC.whoAmI);
                NPC.frame.X = 360;
                NPC.frame.Y = 900;
                NPC.frameCounter = 0;
                framerate = 5;

                laugh = true;
         

            }
            if (NPC.ai[0] == 480)
            {
                State = (float)Phase.Float;
                NPC.frame.X = 0;
                NPC.frame.Y = 0;
                openMouth = false;
                closeMouth = false;
                laugh = false;
                NPC.ai[0] = 0;
                NPC.ai[4] = 0;
            }


        }

        int duration = 40;


        float progress = 50; float progress2 = 50;
        private bool regenerating;
        private int slowdowntimerminus;
        private int slowdowntimer;

        private void FireBreathFollowUp()
        {
            Player target = Main.player[NPC.target];
            //NPC.Move(target.Center, 2f);

            if (NPC.ai[0] == 1)
            {
                openMouth = true;
                NPC.frame.X += 180;
                framerate = 3;
            }

            if (progress > 0 && NPC.ai[0] >= 30)
            {
                progress -= 0.8f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Lerp(0, -0.9f * NPC.direction, EaseFunction.EaseQuinticOut.Ease((progress) / duration));

            }
            if (NPC.ai[2] == 4)
            {

                NPC.ai[2] = 0;
                SoundEngine.PlaySound(SoundID.Item34 with { Volume = 1f, Pitch = Main.rand.NextFloat(0.5f, 2f), MaxInstances = 400 });

                for (int i = 0; i < 2; i++)
                {

                    ParticleManager.NewParticle(NPC.Center + new Vector2(Main.rand.NextFloat(5)), (NPC.rotation.ToRotationVector2() * Main.rand.NextFloat(14, 16)).RotatedByRandom(MathHelper.ToRadians(10)) , ParticleManager.NewInstance<CoreFireParticle>(), Color.Purple, 1.6f);
                }
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(Main.rand.NextFloat(10)), NPC.rotation.ToRotationVector2() * Main.rand.NextFloat(10, 14), ModContent.ProjectileType<FireBreathWraith>(), 30, 0);

                Dust dust = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Smoke>(), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, -1f)) * 5, 0, Color.LimeGreen, 0.9f);
                dust.noGravity = true;

                dust = Dust.NewDustPerfect(NPC.Center, DustID.TerraBlade, Main.rand.NextVector2Circular(1f, 1f) * 5, 0, default, 2f);
                dust.noGravity = true;
            }

            if (progress <= 0)
            {
                progress2 -= 0.8f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Lerp(-0.9f * NPC.direction, 0, EaseFunction.EaseQuinticOut.Ease((progress2) / duration));

            }
            if (progress2 <= -5)
            {

                State = (float)Phase.Float;


                NPC.frame.X = 0;
                NPC.frame.Y = 0;
                openMouth = false;
                closeMouth = false;
                progress = 50;
                progress2 = 50;
                NPC.ai[0] = 0;
                NPC.ai[2] = 0;

            }

            float halfDuration = duration * 0.5f;

            NPC.direction = NPC.spriteDirection = (NPC.velocity.X >= 0f) ? 1 : -1;
           
            NPC.ai[0]++;
            if (progress < 50)
            NPC.ai[2]++;

        }
        private void FireBreath()
        {
            Player target = Main.player[NPC.target];
            if (NPC.direction == 1)
            {
                if (slowdowntimerminus < 60)
                    slowdowntimerminus++;
                if (slowdowntimerminus > 0 && slowdowntimer > 0)
                    slowdowntimer--;


                Main.NewText(slowdowntimerminus);
            }
            if (NPC.direction == -1)
            {
                if (slowdowntimer < 60)
                    slowdowntimer++;
                if (slowdowntimer > 0 && slowdowntimerminus > 0)
                    slowdowntimerminus--;
                Main.NewText(slowdowntimer);

            }
            if (NPC.direction == -1)
            {
                NPC.Move(target.Center, 0.2f / (slowdowntimerminus) / 2);

            }
            if (NPC.direction == 1)
            {
                NPC.Move(target.Center, 0.2f / (slowdowntimer) / 2);

            }
            NPC.oldRot[0] = NPC.velocity.ToRotation();
            for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            {
                NPC.oldRot[i] = NPC.oldRot[i - 1];
            }

            NPC.direction = NPC.spriteDirection = (NPC.velocity.X >= 0f) ? 1 : -1;

            NPC.rotation = NPC.velocity.ToRotation();
            NPC.Move(target.Center, 1.75f);
            NPC.ai[0]++;
            NPC.ai[2]++;
            NPC.immortal = true;
            if (NPC.ai[0] == 1)
            {
                NPC.ai[3] = 3;
                DivergencyDraw.ProxRing(NPC.Center, Color.LimeGreen, 0.7f);

                openMouth = true;
                NPC.frame.X += 180;
                framerate = 3;
            }
            if (NPC.ai[2] == 4)
            {

                NPC.ai[2] = 0;
                SoundEngine.PlaySound(SoundID.Item34 with { Volume = 0.5f, Pitch = Main.rand.NextFloat(0.5f, 2f), MaxInstances = 400 });

                for (int i = 0; i < 2; i++)
                {
                 

                    ParticleManager.NewParticle(NPC.Center + new Vector2(Main.rand.NextFloat(5)), (NPC.rotation.ToRotationVector2() * Main.rand.NextFloat(14, 16)).RotatedByRandom(MathHelper.ToRadians(10)), ParticleManager.NewInstance<CoreFireParticle>(), Color.Purple, 1.6f);
                }
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(Main.rand.NextFloat(10)), NPC.rotation.ToRotationVector2() * Main.rand.NextFloat(6, 10), ModContent.ProjectileType<FireBreathWraith>(), 30, 0);

                Dust dust = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Smoke>(), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, -1f)) * 5, 0, Color.LimeGreen, 0.9f);
                dust.noGravity = true;

                dust = Dust.NewDustPerfect(NPC.Center, DustID.TerraBlade, Main.rand.NextVector2Circular(1f, 1f) * 5, 0, default, 2f);
                dust.noGravity = true;
            }
        
            if (NPC.ai[0] == 9999)
            {
                State = (float)Phase.Float;

                NPC.frame.X = 0;
                NPC.frame.Y = 0;
                openMouth = false;
                closeMouth = false;
      
                NPC.ai[0] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;

            }

            //hand movement


            //hand dead detection works during this phase too
            if (NPC.ai[4] == 2)
            {
                HandDead = true;
                NPC.ai[0] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
            }
            if (HandDead)
            {
                State = (float)Phase.RegenerateHand;
                NPC.frame.X = 0;
                NPC.frame.Y = 0;
          
            }

        }
    }
}

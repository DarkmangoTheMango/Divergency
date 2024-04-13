using Divergency.Common.Helpers;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Weapons.LivingCore;
using Divergency.Content.Projectiles.Hostile;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{

    public class Corennector : ModNPC
    {
        int startingFrame;

        int endingFrame;

        int framerate;

        int combatFrame = 6;

        float maxSpeed = 2f;

        float attackTimer;

        float attackCooldown = 30f;

        bool attacking;

        enum State
        {
            Connect,
            Idle,
            Death

        }

        State state = State.Connect;

        public bool TargetFound { get; private set; }
    

        public int counter { get; private set; }
        public Vector2 NPCCenter { get; private set; }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        List<NPC> cachedNPCs = new List<NPC>();
        List<float> floats = new List<float>();
        Vector2 MiddlePoint;
        private float timer;

        public override void SetDefaults()
        {
            NPC.lifeMax = 999;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0.2f;
            NPC.immortal = true;
            NPC.noTileCollide = true;

            NPC.scale = 0.9f;
            NPC.Size = new Vector2(116f);

            NPC.HitSound = SoundID.DD2_WitherBeastHurt;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("The most annoying creature in the groves :kojima:")
            });
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * 5)
                    NPC.frame.Y = 0;
            }
        }
        public override void AI()
        {
            if (NPC.ai[0] == 20)
            {
              //      NPC.ai[0] = 0;
            }
            Player target = Main.player[NPC.target];
            Vector2 VinePosTop = NPC.Center;
            Vector2 VinePosBottomRight = NPC.Center;
            Vector2 VinePosBottomLeft = NPC.Center;


            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;

            Lighting.AddLight(NPC.Center, new Color(79, 214, 126).ToVector3());

            if (state == State.Connect)
            {
                if (AllSheildsDown())
                {
                    NPC.ai[3] = 1;
                   
                  
                        NPC.immortal = false;
                        NPC.StrikeInstantKill();
                    
                

                    foreach (NPC targetNPC in cachedNPCs)
                    {
                     
                        targetNPC.GetGlobalNPC<CorennectorNPC>().connected = false;
                        targetNPC.immortal = false;

                    }

                }
                else
                {
                    NPC.ai[0]++;
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        NPC taggedNPC = Main.npc[k];

                        if (!taggedNPC.friendly &&!taggedNPC.immortal && taggedNPC.active && taggedNPC.ModNPC is not Corennector && taggedNPC.ModNPC is not CoreElementalHand && taggedNPC.ModNPC is not Corennector && taggedNPC.ModNPC is not CoreElementalBody && taggedNPC.Distance(NPC.Center) < 1000 && !cachedNPCs.Contains(taggedNPC) && NPC.ai[0] == 20)
                        {
                            cachedNPCs.Add(taggedNPC);
                            foreach (NPC targetNPC in cachedNPCs)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = 1f }, NPC.Center);

                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetNPC.Center) * 5, ModContent.ProjectileType<CorennectorProj>(), 0, 0, ai0: cachedNPCs.Count);
                                if (targetNPC.GetGlobalNPC<CorennectorNPC>().ShieldHP <= 0)
                                {
                                    NPC.active = false;
                                }
                            }



                        }
                    }


                    NPC.ai[1]++;
                    if (NPC.ai[1] == 120)
                    {
                        DivergencyDraw.SpawnRing(NPC.Center, Color.LightGreen);
                        NPC.ai[1] = 0;
                    }

                    NPC.rotation += NPC.velocity.X * 0.05f;
                    var averagePos = Vector2.Zero;


                    List<Vector2> Points = new List<Vector2>() { NPCCenter };

                    foreach (var npc in cachedNPCs)
                    {
                        averagePos += npc.Center;
                    }

                    if (cachedNPCs.Count > 0)
                    {
                        // check if count is NOT 0 otherwise it'll crash
                        averagePos /= cachedNPCs.Count;
                    }

                    float[] Lengths = new float[Points.Count];

                    for (int i = 0; i < Points.Count; i++)
                        Lengths[i] = (Points[i] - averagePos).Length();

                    float maxLength = Lengths.Max();
                    Vector2 RPCenter = new Vector2();

                    float totalWeight = 0;

                    for (int i = 0; i < Points.Count; i++)
                    {
                        RPCenter += Points[i] * (Lengths[i] / maxLength);
                        totalWeight += (Lengths[i] / maxLength);
                    }

                    RPCenter /= totalWeight;


                    NPC.velocity += NPC.DirectionTo(averagePos) / 100;
                }
               

          
            }

            
            //FOUND ENEMIES END
            
                
               
         
        }


        private bool AllSheildsDown()
        {
            if (cachedNPCs.Count == 0)
                return false;

            foreach (NPC npc in cachedNPCs)
            {
                if (npc.GetGlobalNPC<CorennectorNPC>().ShieldHP > 0) // or however you do it 
                    return false;
            }

            return true;
        }



        public override void OnKill()
        {
            for (int i = 0; i < 20  ; i++)
            {
              //  Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LivingShard>(), Main.rand.NextVector2Circular(1f, 1f) * 8f, 0, default, 2f);
                //Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CradleWoodFurniture>(), Main.rand.NextVector2Circular(1f, 1f) * 8f, 0, default, 2f);
                Dust.NewDustPerfect(NPC.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(1f, 1f) * 20f, 0, Color.LimeGreen, 2f);

            }
            for (int ii = 0; ii < 40 ; ii++)
            {
                Dust.NewDustPerfect(NPC.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(1f, 1f) * 20f, 0, Color.LimeGreen, 1f);

            }

            if (Main.netMode != NetmodeID.Server) { Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, -3f)), Mod.Find<ModGore>("CorennectorCorpse").Type, 0.9f); }
        }






        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Star = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Star").Value;

            int frameHeight = Star.Height / Main.npcFrameCount[NPC.type];
            int frameY = frameHeight * NPC.frame.Y;

            Rectangle sourceRectangle = new Rectangle(0, frameY, Star.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);

            Color color = NPC.GetAlpha(new Color(109, 223, 94, 0));


            timer += 0.1f;

            if (timer >= MathHelper.Pi)
            {
                timer = 0f;
            }

            Texture2D tex = Terraria.GameContent.TextureAssets.Npc[Type].Value;
            var fadeMult = 1f / NPCID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, NPC.oldPos[i] - Main.screenPosition + NPC.Size / 2, NPC.frame, Color.DarkGreen * (1f - fadeMult * i), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            }

            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/Corennector").Value;
            Texture2D glow = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/CorennectorGlow").Value;


         

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, NPC.position - Main.screenPosition + NPC.Size / 2, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            spriteBatch.Draw(glow, NPC.position - Main.screenPosition + NPC.Size / 2, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            //eye
   

            return false;
        }
    }
    public class CorennectorProj : ModProjectile
    {
        private const string ChainTexturePath = "Divergency/Content/NPCs/LivingGrove/CorennectorChain"; // The folder path to the flail chain sprite
        private const string ChainTextureExtraPath = "Divergency/Content/NPCs/LivingGrove/CorennectorChainExtra";  // This texture and related code is optional and used for a unique effect
        private const string ChainDryTexturePath = "Divergency/Content/NPCs/LivingGrove/CorennectorChainDry"; // The folder path to the flail chain sprite

        public bool spawned { get; private set; }
        public NPC cachedNPC { get; private set; }
        public bool targetfound { get; private set; }
        public float timer { get; private set; }

        public Vector2 destination;

        private NPC cachedTarget;
        private NPC cachedTarget2;
        private bool dried;

        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 999999999;
            Projectile.hide = true;
        }
        public override void AI()
        {

            if (!spawned)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is Corennector)
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
            if (!targetfound)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC target = Main.npc[k];

                    if (target.active && target.ModNPC is not Corennector && target.ModNPC is not CoreElementalHand && target.ModNPC is not CoreElementalBody && Projectile.Hitbox.Intersects(target.Hitbox))
                    {
                        targetfound = true;
                        cachedTarget = target;
                        target.GetGlobalNPC<CorennectorNPC>().connected = true;
                        target.GetGlobalNPC<CorennectorNPC>().ShieldHP += 150;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(),target.Center, new Vector2(0), ModContent.ProjectileType<CorennectorVisuals>(), 0, 0);
                    }



                }
            }
            if (targetfound)
            {
                if(cachedNPC.ai[3] == 1)
                {
                  
                     Projectile.Kill(); 
                }
                else
                {
                    Projectile.Center = cachedTarget.Center;
                }
                if (cachedTarget.GetGlobalNPC<CorennectorNPC>().ShieldHP <= 0)
                {
                    dried = true;
                }
            }
          
        }
        public override bool PreDraw(ref Color lightColor)
        {
            


            Vector2 Origin = Vector2.Zero;
      
                Origin = cachedNPC.Center; //determine where the chains spawns TODO offsets
            

            // This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.

            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);

            Asset<Texture2D> chainTextureExtra = ModContent.Request<Texture2D>(ChainTextureExtraPath); // This texture and related code is optional and used for a unique effect
            if (dried)
            {
                chainTexture = ModContent.Request<Texture2D>(ChainDryTexturePath);

            }
            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center; /// top bottom etc ------------------------------------------------------------------ TO DOOOOOOOOOOOOOOOOOOOOOOOOOOOOO __________________________________________
            Vector2 vectorFromProjectileToPlayerArms = Origin.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 32; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
            }
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (chainLengthRemainingToDraw > 0f)
            {
                // This code gets the lighting at the current tile coordinates
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

              
                

                // Here, we draw the chain texture at the coordinates
                Main.spriteBatch.Draw(chainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }

        
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
    }
    public class CorennectorNPC : GlobalNPC
    {
        public bool connected;
        public float ShieldHP =  1; //trololo
        public override bool InstancePerEntity => true;

        public override void AI(NPC npc)
        {
            if (connected)
            {
                npc.immortal = true;

            }
     

        }
        public override bool PreAI(NPC npc)
        {
            if (connected)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(npc.Center + (velocity * 50f), DustID.PortalBoltTrail, velocity * -3f, 0, Color.LimeGreen, 0.6f);
                dust.noGravity = true;
            }
            return base.PreAI(npc);
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (connected)
            {
                ShieldHP -= hit.Damage;
                if (projectile.DamageType == DamageClass.Melee && projectile.DamageType == DamageClass.SummonMeleeSpeed)
                {
                    ShieldHP -= hit.Damage * 2;

                }
            }
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            ShieldHP -= hit.Damage;
            if (item.DamageType == DamageClass.Melee)
            {
                ShieldHP -= hit.Damage * 2;
            }
        }

    }
    public class CorennectorVisuals : ModProjectile
    {

       // public override string Texture => "Divergency/Assets/Textures/Shockwave";

        public override string Texture => "Divergency/Assets/Textures/Ring";

        public bool spawned { get; private set; }
        public NPC cachedNPC { get; private set; }

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public Trail trail3;

        public Trail trail4;
        float radius = 0;

        float timer = 0;

        float width = 40;
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 0f;
            Projectile.Size = new Vector2(2);
            Projectile.alpha = 0;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (!spawned)
            {

                spawned = true;
              
            }

           
            if (spawned)
            {
                radius += (90 - radius) / 4f;
           
            }
            Projectile.scale += 0.05f;

            Projectile.alpha += 10;
            if (Projectile.alpha >= 255) { Projectile.Kill(); }
        }
        public Trail trail;
        public Trail trail2;
        private int initialDamage;
        public float timer2;

        public override bool PreDraw(ref Color lightColor)
        {



            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Default").Value;
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Default").Value;

            for (int k = 0; k < 3; k++)
            {
                if (trail == null)
                {
                    trail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(30, 220, 30, 100)) * (float)Math.Pow(1f - p, 2f));
                    trail.drawOffset = Projectile.Size / 2f;
                }

                trail.Draw(Projectile.oldPos, timer);
                timer2 -= 0.01f;
            }
            if (trail2 == null)
            {
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            trail2.Draw(Projectile.oldPos);

            if (trail3 == null)
            {
                trail3 = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(width), (p) => Projectile.GetAlpha(new Color(0, 255, 0, 0)));
                trail3.drawOffset = Projectile.Size / 2f;

                trail4 = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(width / 1.9f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail4.drawOffset = Projectile.Size / 2f;
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

            timer -= 0.01f;

            trail3.Draw(tests, rotations, timer);
            trail4.Draw(tests, rotations, timer);

            return false;
        }
    }
}
        








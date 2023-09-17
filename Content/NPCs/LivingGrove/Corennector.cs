using Divergency.Common.Helpers;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Weapons.LivingCore;
using Divergency.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
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
        public NPC cachedNPC { get; private set; }
        public NPC cachedNPC2 { get; private set; }
        public NPC cachedNPC3 { get; private set; }

        public int counter { get; private set; }
        public Vector2 NPCCenter { get; private set; }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        List<NPC> cachedNPCs = new List<NPC>();
        Vector2 MiddlePoint;
        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
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
                if (counter == 0)
                {
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        NPC taggedNPC = Main.npc[k];

                        if (taggedNPC.active && taggedNPC.ModNPC is not Corennector && taggedNPC.Distance(NPC.Center) < 1000)
                        {
                            cachedNPCs.Add(taggedNPC);
                            
                        }
                    }
                    counter++;

                }


                //FOUND ENEMIES END

                //START CONNECTING
                if (counter == 1)
                {
                    NPC.ai[0]++;

                    if (NPC.ai[0] == 20)
                    {
                        foreach (NPC targetNPC in cachedNPCs)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = 1f }, NPC.Center);

                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetNPC.Center) * 2, ModContent.ProjectileType<CorennectorProj>(), 0, 0, ai0: 1);
                        }
                    }

                    NPC.rotation += NPC.velocity.X * 0.04f;
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


                    NPC.velocity += NPC.DirectionTo(averagePos) / 70;

                }
            }
         

        }


        public override void OnKill()
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LivingShard>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CradleWoodFurniture>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
            }

            // if (Main.netMode != NetmodeID.Server) { Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, -3f)), Mod.Find<ModGore>("GuardianCorpse").Type, 1f); }
        }






        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);

            Texture2D tex = Terraria.GameContent.TextureAssets.Npc[Type].Value;
            var fadeMult = 1f / NPCID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, NPC.oldPos[i] - Main.screenPosition + NPC.Size / 2, NPC.frame, Color.DarkGreen * (1f - fadeMult * i), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            }

            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/Corennector").Value;
            Texture2D glow = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/CorennectorGlow").Value;


            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, NPC.position - Main.screenPosition + NPC.Size / 2, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            spriteBatch.Draw(glow, NPC.position - Main.screenPosition + NPC.Size / 2, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            //trail
          
            return false;
        }
    }
    public class CorennectorProj : ModProjectile
    {
        private const string ChainTexturePath = "Divergency/Content/NPCs/LivingGrove/CorennectorChain"; // The folder path to the flail chain sprite
        private const string ChainTextureExtraPath = "Divergency/Content/NPCs/LivingGrove/CorennectorChainExtra";  // This texture and related code is optional and used for a unique effect

        public bool spawned { get; private set; }
        public NPC cachedNPC { get; private set; }
        public bool targetfound { get; private set; }
        public Vector2 destination;

        private NPC cachedTarget;
        private NPC cachedTarget2;

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

                    if (target.active && target.ModNPC is not Corennector && Projectile.Hitbox.Intersects(target.Hitbox))
                    {
                        targetfound = true;
                        cachedTarget = target;
                        target.GetGlobalNPC<CorennectorNPC>().connected = true;
                    }
                  


                }
            }
            if (targetfound)
            {
                Projectile.Center = cachedTarget.Center;
                
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 Origin = Vector2.Zero;
            if (Projectile.ai[0] == 1)
            {
                Origin = cachedNPC.Center; //determine where the chains spawns TODO offsets
            }
     
            // This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.


            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
            Asset<Texture2D> chainTextureExtra = ModContent.Request<Texture2D>(ChainTextureExtraPath); // This texture and related code is optional and used for a unique effect

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
        public override bool InstancePerEntity => true;

        public override void AI(NPC npc)
        {
            if (connected)
            {
                npc.immortal = true;
            }
        }

    }
    public class CorennectorVisuals : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";
        //emit those rings and make the glow
    }
    public class CorennectorDust : ModDust
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";

        //todo
        //1. make them spin in random angles
    }
}





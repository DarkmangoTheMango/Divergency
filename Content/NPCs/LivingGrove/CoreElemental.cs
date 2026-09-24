using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Bosses;
using Divergency.Content.Dusts;
using Divergency.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{
    public class CoreElemental : ModNPC
    {
        enum State
        {
            spawn,
            moving,
            dashcharge,
            dashing,
            death
        }
        State state = State.spawn;

        public int dashcooldown { get; private set; }
        public int initialDamage { get; private set; }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 150;
            NPC.damage = 50;
            NPC.defense = 15;
            NPC.knockBackResist = 0.6f;

            NPC.noTileCollide = true;

            NPC.scale = 1f;
            NPC.Size = new Vector2(52f, 32f);

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
                new FlavorTextBestiaryInfoElement("He just took 3 gas station dick pills")
            });
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            if (NPC.Center.Distance(target.Center) < 150)
            {
                NPC.knockBackResist = 2;
                NPC.defense = 0;
            }
            else
            {
                NPC.knockBackResist = 0.6f;
                NPC.defense = 15;
            };
            if (state == State.spawn)
            {
                //spawm limbs
                initialDamage = NPC.damage;

                NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<CoreElementalBody>(), 0);
                NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<CoreElementalHand>(), 0);
                NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<CoreElementalHand>(), 0, 1);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0), ModContent.ProjectileType<CoreElementalTrail>(), 0, 0);
                state = State.moving;
            }
            if (state == State.moving && NPC.velocity.X < 0.2f && dashcooldown == 0)
            {
                state = State.dashcharge;
            }
            if (state == State.moving)
            {
                NPC.damage = 0;

                NPC.ai[0] = 0;
                NPC.TargetClosest(true);
                NPC.Move(target.Center, 9f, 50);
                NPC.rotation = NPC.velocity.X * 0.1f;
                if (dashcooldown > 0)
                    dashcooldown--;
            }
            if (state == State.dashing)
            {

                NPC.ai[0]++;
                NPC.TargetClosest(true);
                NPC.Move(target.Center, 9f, 50);
                NPC.rotation = NPC.velocity.X * 0.1f;
                if (dashcooldown > 0)
                    dashcooldown--;
                if (NPC.ai[0] == 45)
                {
                    state = State.moving;
                }
            }

            if (state == State.dashcharge)
            {
                NPC.damage = initialDamage;

                NPC.rotation = NPC.velocity.X * 0.1f;
                NPC.TargetClosest(true);
                NPC.ai[0]++;
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(NPC.Center + (velocity * 80f), ModContent.DustType<Glow>(), velocity * -5f, 0, Color.LimeGreen, 0.7f);
                //SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack, NPC.Center); NEED SUITABLE SOUND 

                dust.noGravity = true;
                if (NPC.ai[0] < 80)
                    NPC.velocity /= 1.2f;
                if (NPC.ai[0] == 80)
                {
                    SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/Dash") with { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, NPC.Center);
                    NPC.velocity += NPC.Center.DirectionTo(target.Center) * 12;
                    dashcooldown = 120;
                    state = State.dashing;
                    NPC.ai[0] = 0;
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
                NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<CoreElementalDeath>());
            }
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/CoreElemental").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, position, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return false;
        }
    }

    public class CoreElementalHand : ModNPC
    {
        public bool spawned { get; private set; }
        public Projectile cachedProjectile { get; private set; }
        public NPC cachedNPC { get; private set; }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1;
            NPC.damage = 0;
            NPC.defense = 5;
            NPC.knockBackResist = 0f;
            NPC.width = 10;
            NPC.height = 50;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.dontTakeDamage = true;
            NPC.friendly = false;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.behindTiles = false;
            NPC.ShowNameOnHover = false;

        }
        public override void AI()
        {

            if (!spawned)
            {
                bool found = false;

                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is CoreElemental)
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

                spawned = true;
            }

            if (!cachedNPC.active)
            {
                NPC.active = false;
            }
            if (spawned)
            {
                NPC.rotation = NPC.velocity.X * 0.1f;

                if (NPC.ai[0] == 0)
                {
                    NPC.Move(cachedNPC.Center + new Vector2(-18, 12), 25f, 20);

                }
                else
                {
                    NPC.Move(cachedNPC.Center + new Vector2(18, 12), 25f, 20);

                }
            }
        }
       
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/CoreElementalHandGlow").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.ai[0] > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

           // spriteBatch.Draw(texture, position, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return true;
        }
    }
    public class CoreElementalBody : ModNPC
    {
        public bool spawned { get; private set; }
        public Projectile cachedProjectile { get; private set; }
        public NPC cachedNPC { get; private set; }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1;
            NPC.damage = 0;
            NPC.defense = 5;
            NPC.knockBackResist = 0f;
            NPC.width = 10;
            NPC.height = 50;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.dontTakeDamage = true;
            NPC.friendly = false;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.behindTiles = false;
            NPC.ShowNameOnHover = false;

        }
        public override void AI()
        {

            if (!spawned)
            {
                bool found = false;

                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is CoreElemental)
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

                spawned = true;
            }

            if (!cachedNPC.active)
            {
                NPC.active = false;
            }
            if (spawned)
            {
                NPC.rotation = NPC.velocity.X * 0.1f;

                
                    NPC.Move(cachedNPC.Center + new Vector2(0, 18), 30f, 20);

                
                
            }
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/CoreElementalBodyGlow").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

           // spriteBatch.Draw(texture, position, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return true;
        }
    }
    public class CoreElementalDeath : ModNPC
    {
        public override string Texture => "Divergency/Content/NPCs/LivingGrove/CoreElemental";
        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
            NPC.knockBackResist = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;

            NPC.noTileCollide = true;
            NPC.damage = 100;
            NPC.scale = 1f;
            NPC.Size = new Vector2(0, 0);
            NPC.friendly = true;
            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;
        }

        public override void AI()
        {
            if (NPC.ai[0] >= 60f)
            {
                NPC.velocity.Y += 0.1f;
            }
            NPC.TargetClosest(true);
            Player target = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.05f;

            NPC.ai[0]++;
            NPC.ai[1]++;

           
            if (NPC.ai[0] == 120f)
            {
                DivergencyDraw.SpawnExplosion(NPC.Center, Color.LimeGreen, ModContent.DustType<Glow>(), 7);
                DivergencyDraw.SpawnRing(NPC.Center,Color.LimeGreen, 0.13f * 1.18f,0.9f * 1.18f, 2* 1.18f);
                DivergencyDraw.SpawnRing(NPC.Center, Color.LimeGreen, 0.13f * 1.1f, 0.9f * 1.1f, 2 * 1.1f);
                DivergencyDraw.SpawnRing(NPC.Center, Color.LimeGreen, 0.13f * 1.05f, 0.9f * 1.05f, 2 * 1.05f);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0), ModContent.ProjectileType<CoreElementalDeathProj>(), 120, 2f);
                float radius = 2;
                int numberOfDusts = 20;
                CameraSystem.ScreenShake(10);

                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 25, 0, default, 2f);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Glow>(), Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfDusts * i)) * radius, 0, new Color(109, 223, 94), 1.4f);

                    dust.noGravity = true;
                }

                NPC.active = false;
            }

          
            else
            {

                if (NPC.ai[1] >= 5f)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, Color.LimeGreen, 0.7f);
                    }
                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (Main.rand.NextVector2Circular(1f, 1f) * 40f), Vector2.Zero, ModContent.ProjectileType<SageDeathBomb>(), 0, 0f, 0, Main.rand.NextFloat(0f, 360f), 1f);

                    NPC.ai[1] = 0f;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/CoreElemental").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, position, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return false;
        }
    }
    public class CoreElementalDeathProj : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Size = new Vector2(300, 300);
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 2;
        }        

    }
}
    
    

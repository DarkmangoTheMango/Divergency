using Divergency.Content.Dusts;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{
    [AutoloadBossHead]

    public class Coreling : ModNPC
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
            attacking,
            moving
        }

        State state = State.moving;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.damage = 30;
            NPC.defense = 5;
            NPC.knockBackResist = 0.2f;

            NPC.noTileCollide = true;

            NPC.scale = 1f;
            NPC.Size = new Vector2(44f, 54f);

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
                new FlavorTextBestiaryInfoElement("The most common creature in The Living Grove. Protecting it from weak adventurers and taking care of the tree itself. They have been observed displaying behavior similar to bees...")
            });
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];

            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.1f;

            Lighting.AddLight(NPC.Center, new Color(79, 214, 126).ToVector3());

            if (NPC.ai[0] >= 240f)
            {
                state = State.attacking;

                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(NPC.Center + (velocity * 100f), DustID.TerraBlade, velocity * -5f, 0, default, 1f);
                dust.noGravity = true;

                NPC.velocity *= 0.98f;

                if (attacking)
                {
                    SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = 1f }, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(target.Center) * 10f, ModContent.ProjectileType<GuardianBeam>(), NPC.damage, 3f, 0);

                    NPC.velocity -= NPC.DirectionTo(target.Center) * 3f;

                    attacking = false;
                }
            }
            else
            {
                if (NPC.Distance(target.Center) >= 300f) { NPC.velocity += NPC.DirectionTo(target.Center) * 0.05f; }
                else if (NPC.Distance(target.Center) <= 100f) { NPC.velocity -= NPC.DirectionTo(target.Center) * 0.05f; }
                else { NPC.velocity *= 0.98f; }

                if (NPC.Center.Y >= target.Center.Y) { NPC.velocity.Y -= 0.05f; }

                state = State.moving;
            }

            if (NPC.velocity.X >= maxSpeed) { NPC.velocity.X = maxSpeed; }
            if (NPC.velocity.X <= -maxSpeed) { NPC.velocity.X = -maxSpeed; }
            if (NPC.velocity.Y >= maxSpeed) { NPC.velocity.Y = maxSpeed; }
            if (NPC.velocity.Y <= -maxSpeed) { NPC.velocity.Y = -maxSpeed; }

            NPC.ai[0]++;
        }

        public override void OnKill()
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LivingShard>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CradleWoodFurniture>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
            }

            if (Main.netMode != NetmodeID.Server) { Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, -3f)), Mod.Find<ModGore>("GuardianCorpse").Type, 1f); }
        }

        public override void FindFrame(int frameHeight)
        {
            if (state == State.moving)
            {
                startingFrame = 0;
                endingFrame = 3;
                framerate = 5;

                NPC.frameCounter += (NPC.velocity.Length() * 0.1f) + 0.6f;

                if (NPC.frameCounter >= framerate)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > endingFrame * frameHeight) { NPC.frame.Y = startingFrame * frameHeight; }
                }
            }

            if (state == State.attacking)
            {
                startingFrame = 5;
                endingFrame = 9;
                framerate = 5;

                NPC.frameCounter++;

                if (NPC.frameCounter >= framerate)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y == combatFrame * frameHeight) { attacking = true; }

                    if (NPC.frame.Y > endingFrame * frameHeight)
                    {
                        NPC.frame.Y = endingFrame * frameHeight;
                        NPC.ai[0] = 0f;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/Coreling").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return true;
        }
    }
    public class CorelingCutscene : ModNPC
    {
        int startingFrame;

        int endingFrame;

        int framerate;

        int combatFrame = 6;

        float maxSpeed = 2f;

        float attackTimer;

        float attackCooldown = 30f;

        bool attacking;

        bool ActiveCutscene = true;

        enum State
        {
            cutscene,
            attacking,
            moving
        }
        State state = State.moving;

        public override string Texture => "Divergency/Content/NPCs/LivingGrove/Coreling";

        public override void SetStaticDefaults()
        {
            //.setdefault("Core Sage");
            Main.npcFrameCount[NPC.type] = 10;

        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.damage = 30;
            NPC.defense = 20;
            NPC.knockBackResist = 0.2f;

            NPC.noTileCollide = true;

            NPC.scale = 1f;
            NPC.Size = new Vector2(44f, 54f);

            NPC.HitSound = SoundID.DD2_WitherBeastHurt;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];


            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.1f;

            Lighting.AddLight(NPC.Center, new Color(79, 214, 126).ToVector3());
            if (ActiveCutscene)
            {
                target.stoned = true;
                state = State.cutscene;
                if (NPC.ai[0] == 1f)
                {
                    CameraSystem.Zoom(NPC.Center, 240);
                }
                if (NPC.ai[0] > 75f && NPC.ai[0] < 200f && alpha < 1)
                {
                    alpha += 0.1f;
                }
                if (NPC.ai[0] > 200f)
                {
                    alpha -= 0.2f;
                }

                if (NPC.ai[0] == 239f)
                {
                    ActiveCutscene = false;
                    NPC.ai[0] = 0;

                }

                NPC.ai[0]++;

            }
            else
            {
                if (NPC.ai[0] >= 240f)
                {
                    state = State.attacking;

                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(NPC.Center + (velocity * 100f), DustID.TerraBlade, velocity * -5f, 0, default, 1f);
                    dust.noGravity = true;

                    NPC.velocity *= 0.98f;

                    if (attacking)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = 1f }, NPC.Center);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(target.Center) * 10f, ModContent.ProjectileType<GuardianBeam>(), NPC.damage, 3f, 0);

                        NPC.velocity -= NPC.DirectionTo(target.Center) * 3f;

                        attacking = false;
                    }
                }
                else
                {
                    if (NPC.Distance(target.Center) >= 300f) { NPC.velocity += NPC.DirectionTo(target.Center) * 0.05f; }
                    else if (NPC.Distance(target.Center) <= 100f) { NPC.velocity -= NPC.DirectionTo(target.Center) * 0.05f; }
                    else { NPC.velocity *= 0.98f; }

                    if (NPC.Center.Y >= target.Center.Y) { NPC.velocity.Y -= 0.05f; }

                    state = State.moving;
                }

                if (NPC.velocity.X >= maxSpeed) { NPC.velocity.X = maxSpeed; }
                if (NPC.velocity.X <= -maxSpeed) { NPC.velocity.X = -maxSpeed; }
                if (NPC.velocity.Y >= maxSpeed) { NPC.velocity.Y = maxSpeed; }
                if (NPC.velocity.Y <= -maxSpeed) { NPC.velocity.Y = -maxSpeed; }

                NPC.ai[0]++;


            }
        }
        public override void OnKill()
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LivingShard>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CradleWoodFurniture>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
            }

            if (Main.netMode != NetmodeID.Server) { Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, -3f)), Mod.Find<ModGore>("GuardianCorpse").Type, 1f); }
        }
        public override void FindFrame(int frameHeight)
        {
            if (state == State.moving || state == State.cutscene)
            {
                startingFrame = 0;
                endingFrame = 3;
                framerate = 5;

                NPC.frameCounter += (NPC.velocity.Length() * 0.1f) + 0.6f;

                if (NPC.frameCounter >= framerate)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > endingFrame * frameHeight) { NPC.frame.Y = startingFrame * frameHeight; }
                }
            }

            if (state == State.attacking)
            {
                startingFrame = 5;
                endingFrame = 9;
                framerate = 5;

                NPC.frameCounter++;

                if (NPC.frameCounter >= framerate)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y == combatFrame * frameHeight) { attacking = true; }

                    if (NPC.frame.Y > endingFrame * frameHeight)
                    {
                        NPC.frame.Y = endingFrame * frameHeight;
                        NPC.ai[0] = 0f;
                    }
                }
            }
        }
        float alpha;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
           
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/Coreling").Value;
            Texture2D CutsceneIcon = ModContent.Request<Texture2D>("Divergency/Assets/Textures/CorelingIcon").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);

            spriteBatch.Draw(CutsceneIcon, position - new Vector2(0, 50), null, color * alpha, NPC.rotation, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);

            return true;
        }
    }
}
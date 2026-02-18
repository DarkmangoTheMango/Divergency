using Divergency.Common.Helpers;
using Divergency.Content.Biomes;
using Divergency.Content.Dusts;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Hostile;
using ParticleLibrary;
using System;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace Divergency.Content.NPCs.LivingGrove;

public class Corelossus : ModNPC
{
    #region Fields

    Player Target => Main.player[NPC.target];

    private enum State
    {
        Spawning,
        Moving,
        Attacking,
        Dying
    }

    private State state
    {
        get;
        set;
    } = State.Spawning;

    #endregion Fields

    #region Initialization

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;

        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
        {
            Velocity = 1f,
            Direction = 1
        };

        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }

    public override void SetDefaults()
    {
        NPC.aiStyle = -1;
        AIType = -1;

        NPC.Size = new(82);

        NPC.lifeMax = Main.expertMode ? Main.masterMode ? 400 : 300 : 200;
        NPC.defense = 25;

        NPC.noTileCollide = true;
        NPC.noGravity = true;

        NPC.HitSound = SoundID.DD2_WitherBeastHurt;
        NPC.DeathSound = SoundID.DD2_WitherBeastDeath;

        NPC.value = Item.buyPrice(0, 0, 8, 0);

        SpawnModBiomes =
        [
            ModContent.GetInstance<LivingCoreBiome>().Type
        ];
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(
        [
            new FlavorTextBestiaryInfoElement($"Mods.{Mod.Name}.Bestiary.{Name}")
        ]);
    }

    #endregion Initialization

    #region Networking

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write((int)state);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        state = (State)reader.ReadInt32();
    }

    #endregion Networking

    #region Behavior

    public override void OnSpawn(IEntitySource source)
    {

    }

    public override void OnKill()
    {
        state = State.Dying;

        if (Main.netMode != NetmodeID.Server)
        {
            for (int i = 0; i < 3; i++)
                for (int k = 0; k < 5; k++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>($"Coreling{k}").Type, 1f);

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LivingShard>(), Main.rand.NextVector2Circular(1f, 1f) * 8f, 0, default, 1f);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CradleWoodFurniture>(), Main.rand.NextVector2Circular(1f, 1f) * 8f, 0, default, 1f);
            }
        }
    }

    public override void AI()
    {
        if (!NPC.HasValidTarget)
            NPC.TargetClosest();

        switch (state)
        {
            case State.Spawning:
                State_Spawning();
                break;
            case State.Moving:
                State_Moving();
                break;
            case State.Attacking:
                State_Attacking();
                break;
            case State.Dying:
                State_Dying();
                break;
            default:
                break;
        }

        NPC.direction = NPC.Center.X > Target.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
        NPC.velocity *= 0.98f;
        NPC.rotation = Math.Clamp(NPC.velocity.X * 0.1f, -1, 1);
        NPC.ai[0]++;

        Lighting.AddLight(NPC.Center, new Color(109, 223, 94).ToVector3() * 0.4f);
    }

    private void State_Spawning()
    {
        NPC.dontTakeDamage = true;
        NPC.ShowNameOnHover = false;

        NPC.scale = MathHelper.Lerp(0, 1, EaseFunction.EaseCircularOut.Ease(NPC.ai[0] / 60));

        Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

        ParticleManager.NewParticle<CoreSparkle>(NPC.Center + (velocity * 100f), velocity * -5f, default, 1f);

        if (NPC.ai[0] >= 60)
        {
            NPC.ai[0] = 0;

            for (int k = 0; k < 20; k++)
                ParticleManager.NewParticle<CoreSparkle>(NPC.Center, Main.rand.NextVector2Circular(1f, 1f) * 10f, default, 1f);

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Spawn"), NPC.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                state = State.Moving;
                NPC.netUpdate = true;
            }
        }
    }

    private void State_Moving()
    {
        NPC.dontTakeDamage = false;
        NPC.ShowNameOnHover = true;

        float acceleration = 0.05f;

        if (NPC.Distance(Target.Center) > 300)
            NPC.velocity += NPC.DirectionTo(Target.Center) * acceleration;
        else if (NPC.Distance(Target.Center) < 100)
            NPC.velocity += NPC.DirectionFrom(Target.Center) * acceleration;

        if (NPC.Center.Y > Target.Center.Y)
            NPC.velocity.Y -= acceleration;

        if (NPC.ai[0] >= 240)
        {
            NPC.ai[0] = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                state = State.Attacking;
                NPC.netUpdate = true;
            }
        }
    }

    private void State_Attacking()
    {
        NPC.dontTakeDamage = false;
        NPC.ShowNameOnHover = true;

        if (NPC.ai[0] < 30)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

            ParticleManager.NewParticle<CoreSparkle>(NPC.Center + (velocity * 100f), velocity * -5f, default, 1f);
        }

        if (NPC.ai[0] == 30)
        {
            for (int k = 0; k < 3; k++)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).RotatedByRandom(0.3f) * 5f, ModContent.ProjectileType<GuardianBeam>(), 20, 0, Main.myPlayer);

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot").WithPitchOffset(0.5f), NPC.Center);

            NPC.velocity += NPC.DirectionFrom(Target.Center) * 3f;
        }

        if (NPC.ai[0] >= 40)
        {
            NPC.ai[0] = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                state = State.Moving;
                NPC.netUpdate = true;
            }
        }
    }

    private void State_Dying()
    {
        NPC.dontTakeDamage = true;
        NPC.ShowNameOnHover = true;
    }

    #endregion Behavior

    #region Drawing

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter += (NPC.velocity.Length() * 0.1f) + 0.6f;

        if (NPC.frameCounter >= 5)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y += frameHeight;

            if (NPC.frame.Y > 3 * frameHeight)
                NPC.frame.Y = 0 * frameHeight;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.IsABestiaryIconDummy)
        {
            DrawBody(screenPos, drawColor);
            return false;
        }

        switch (state)
        {
            case State.Spawning:
                DrawGlow(screenPos, drawColor);
                break;
            case State.Moving:
                DrawBody(screenPos, drawColor);
                break;
            case State.Attacking:
                DrawBody(screenPos, drawColor);
                break;
            case State.Dying:
                break;
            default:
                break;
        }

        return false;
    }

    private void DrawGlow(Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow2").Value;

        Vector2 position = NPC.Center - screenPos;

        SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        Main.EntitySpriteDraw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, spriteEffects, 1f);
    }

    private void DrawBody(Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

        Vector2 position = NPC.Center - screenPos;

        SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        Main.EntitySpriteDraw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, spriteEffects, 1f);
        Main.EntitySpriteDraw(glowTexture, position, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, spriteEffects, 1f);
    }

    #endregion Drawing
}

/*public class Corelossus : ModNPC
{
    int startingFrame;

    int endingFrame;

    int framerate;

    float maxSpeed = 2f;

    float attackTimer;

    float attackCooldown = 30f;

    bool attacking;

    enum State
    {
        attacking,
        moving
    }

    State state = 0;

    public bool initialize { get; private set; }
    public int initialDamage { get; private set; }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;

        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Velocity = 0f };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
    }

    public override void SetDefaults()
    {
        NPC.lifeMax = 200;
        NPC.damage = 40;
        NPC.defense = 25;
        NPC.knockBackResist = 0.1f;

        NPC.noTileCollide = true;

        NPC.scale = 1f;
        NPC.Size = new Vector2(82f, 86f);

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
            new FlavorTextBestiaryInfoElement("A trio of Guardians, fuesed by the roots from which they emerged...")
        });
    }

    public override void AI()
    {
        if (!initialize)
        {
            initialize = true;
            initialDamage = NPC.damage;
        }
        if (initialize)
        {
            Player target = Main.player[NPC.target];

            NPC.TargetClosest(true);

            NPC.rotation = NPC.velocity.X * 0.1f;

            Lighting.AddLight(NPC.Center, new Color(79, 214, 126).ToVector3());

            if (NPC.ai[0] >= 360f)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = 0.5f }, NPC.Center);

                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (NPC.DirectionTo(target.Center) * 10f).RotatedByRandom(0.3f), ModContent.ProjectileType<GuardianBeam>(), initialDamage, 3f, 0);
                }

                NPC.velocity -= NPC.DirectionTo(target.Center) * 3f;

                NPC.ai[0] = 0f;
            }
            else if (NPC.ai[0] >= 240f)
            {
                state = State.attacking;

                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(NPC.Center + (velocity * 100f), DustID.TerraBlade, velocity * -5f, 0, default, 1f);
                dust.noGravity = true;

                NPC.velocity *= 0.98f;
            }
            else
            {
                state = State.moving;

                if (NPC.Distance(target.Center) >= 400f) { NPC.velocity += NPC.DirectionTo(target.Center) * 0.05f; }
                else if (NPC.Distance(target.Center) <= 200f) { NPC.velocity -= NPC.DirectionTo(target.Center) * 0.05f; }
                else { NPC.velocity *= 0.98f; }

                if (NPC.Center.Y >= target.Center.Y) { NPC.velocity.Y -= 1f; }
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
            Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LivingShard>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 2f);
            Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CradleWoodFurniture>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 2f);
        }

        if (Main.netMode != NetmodeID.Server) { for (int i = 0; i < 3; i++) { Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, -3f)), Mod.Find<ModGore>("GuardianCorpse").Type, 1f); } }
    }

    public override void FindFrame(int frameHeight)
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

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/Corelossus").Value;

        Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
        Color color = Color.White;

        SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
        return true;
    }
}*/
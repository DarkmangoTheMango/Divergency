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

public class Coreling : ModNPC
{
    #region Fields

    Player Target => Main.player[NPC.target];

    private enum State
    {
        Moving,
        Attacking
    }

    private State state
    {
        get;
        set;
    } = State.Moving;

    #endregion Fields

    #region Initialization

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 10;
        
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

        NPC.Size = new(44);

        NPC.lifeMax = Main.expertMode ? Main.masterMode ? 120 : 90 : 60;
        NPC.defense = 12;

        NPC.noTileCollide = true;
        NPC.noGravity = true;

        NPC.HitSound = SoundID.DD2_WitherBeastHurt;
        NPC.DeathSound = SoundID.DD2_WitherBeastDeath;

        NPC.value = Item.buyPrice(0, 0, 5, 0);

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
        if (Main.netMode != NetmodeID.Server)
        {
            for (int k = 0; k < 5; k++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>($"{Name}{k}").Type, 1f);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LivingShard>(), Main.rand.NextVector2Circular(1f, 1f) * 5f, 0, default, 1f);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CradleWoodFurniture>(), Main.rand.NextVector2Circular(1f, 1f) * 5f, 0, default, 1f);
            }
        }
    }

    public override void AI()
    {
        if (!NPC.HasValidTarget)
            NPC.TargetClosest();

        switch (state)
        {
            case State.Moving:
                State_Moving();
                break;
            case State.Attacking:
                State_Attacking();
                break;
            default:
                break;
        }

        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.ModNPC is Coreling && npc.ModNPC != this && NPC.Distance(npc.Center) <= 30f)
                NPC.velocity += NPC.DirectionFrom(npc.Center) * 0.1f;

        NPC.direction = NPC.Center.X > Target.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
        NPC.velocity *= 0.98f;
        NPC.rotation = Math.Clamp(NPC.velocity.X * 0.1f, -1, 1);
        NPC.ai[0]++;

        Lighting.AddLight(NPC.Center, new Color(109, 223, 94).ToVector3() * 0.2f);
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
            ParticleManager.NewParticle<CoreSparkle>(NPC.Center, Vector2.Zero, default, 3f);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center) * 5f, ModContent.ProjectileType<GuardianBeam>(), 20, 0, Main.myPlayer);

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot").WithPitchOffset(1f), NPC.Center);

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

    #endregion Behavior

    #region Drawing

    public override void FindFrame(int frameHeight)
    {
        switch (state)
        {
            case State.Moving:
                NPC.frameCounter += (NPC.velocity.Length() * 0.1f) + 0.6f;

                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0 * frameHeight;
                }
                break;
            case State.Attacking:
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > 9 * frameHeight)
                        NPC.frame.Y = 9 * frameHeight;
                }
                break;
            default:
                break;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

        Vector2 position = NPC.Center - screenPos;

        SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        Main.EntitySpriteDraw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, spriteEffects, 1f);
        Main.EntitySpriteDraw(glowTexture, position, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, spriteEffects, 1f);

        return false;
    }

    #endregion Drawing
}
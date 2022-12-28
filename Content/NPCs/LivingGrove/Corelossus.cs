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

    public class Corelossus : ModNPC
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
            Player target = Main.player[NPC.target];

            NPC.TargetClosest(true);

            NPC.rotation = NPC.velocity.X * 0.1f;

            Lighting.AddLight(NPC.Center, new Color(79, 214, 126).ToVector3());

            if (NPC.ai[0] >= 360f)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = 0.5f }, NPC.Center);

                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (NPC.DirectionTo(target.Center) * 10f).RotatedByRandom(0.3f), ModContent.ProjectileType<GuardianBeam>(), NPC.damage, 3f, 0);
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

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/´Corelossus_Glow").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, position, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
        }
    }

    public class CorelossusSpawner : ModItem
    {
        public override bool AltFunctionUse(Player player) => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guardian Cluster");
            Tooltip.SetDefault("Summons a Guardian Cluster");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Items/SwingStyleNotYippee");
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.type == ModContent.NPCType<Coreling>()) { npc.active = false; }
                }
            }
            else { if (player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server) { NPC.NewNPC(Terraria.Entity.GetSource_NaturalSpawn(), (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, ModContent.NPCType<Corelossus>()); } }

            return true;
        }
    }
}
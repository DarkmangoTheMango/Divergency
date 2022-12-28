using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;


namespace Divergency.Content.NPCs.Forest
{
    public class Acrid : ModNPC
    {
        float aiEvent = 240f;

        float aiInterval = 60f;

        int state = 0;

        int startingFrame;

        int endingFrame;

        int framerate;

        enum State
        {
            attacking,
            screaming
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 23;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 20;
            NPC.damage = 10;
            NPC.defense = 2;
            NPC.knockBackResist = 0.7f;

            NPC.Size = new Vector2(31, 36);
            NPC.scale = 1f;

            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = Item.sellPrice(0, 0, 0, 90);

            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Known for its high-pitched cry, This monster lurks in the trees of the forest, waiting for it's prey to appear. Studiest show no signs of remorse for their targets, some even say they are the spawn of the devil himself...")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(true);

            float direction = NPC.direction * 0.1f;

            if (NPC.velocity.X >= 2f) { NPC.velocity.X = 2f; }
            if (NPC.velocity.X <= -2f) { NPC.velocity.X = -2f; }

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.05f;

            if (NPC.ai[0] >= aiEvent + aiInterval)
            {
                state = (int)State.attacking;
                NPC.ai[0] = 0f;
            }
            else if (NPC.ai[0] == aiEvent) { state = (int)State.screaming; }
            else if (NPC.ai[0] >= aiEvent - aiInterval)
            {
                state = (int)State.screaming;
                NPC.velocity.X *= 0.9f;
            }
            else
            {
                state = (int)State.attacking;

                NPC.velocity.X += direction;
                if (NPC.Center.Distance(Main.player[NPC.target].Center) <= 100f && Collision.SolidTiles(NPC.position, NPC.width, NPC.height)) { NPC.velocity.Y -= 5f; }
            }

            NPC.ai[0]++;
        }

        public override void OnKill() { if (Main.netMode != NetmodeID.Server) { Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, -3f)), Mod.Find<ModGore>("Acorn").Type, 1f); } }

        public override void FindFrame(int frameHeight)
        {
            if (state == (int)State.attacking)
            {
                startingFrame = 0;
                endingFrame = 9;
                framerate = 5;

                NPC.frameCounter += (NPC.velocity.Length() * 1f);

                if (NPC.frameCounter >= framerate)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > endingFrame * frameHeight) { NPC.frame.Y = startingFrame * frameHeight; }
                }
            }

            if (state == (int)State.screaming)
            {
                startingFrame = 10;
                endingFrame = 22;
                framerate = 5;

                NPC.frameCounter++;

                if (NPC.frameCounter >= framerate)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y == 14 * frameHeight) { SoundEngine.PlaySound(SoundID.DeerclopsScream with { Volume = 0.75f, Pitch = 1.3f }, NPC.Center); }
                    
                    if (NPC.frame.Y >= endingFrame * frameHeight) { NPC.frame.Y = endingFrame * frameHeight; }
                }
            }
        }
    }
}




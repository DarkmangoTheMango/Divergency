using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
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

                    if (NPC.frame.Y == 14 * frameHeight)
                    {
                        Vector2 pos = NPC.position;
                        for (int i = 0; i < 2; i++)
                        {
                            for (i = -5; i <= 5; i++)
                            {
                                bool success = TryFindTreeTop(pos + new Vector2(i * 16f, 0f), out Vector2 result);
                                NPC.NewNPC(null, (int)(result.X + Main.rand.NextFloat(-32f, 33f)), (int)(result.Y + Main.rand.NextFloat(-64f, 1f)), ModContent.NPCType<Acrid>());
                                SoundEngine.PlaySound(SoundID.NPCHit2 with { Volume = 0.75f, Pitch = 1.3f }, NPC.Center);

                                for (int i2 = 0; i2 < 10; i2++)
                                {
                                    if (Main.netMode != NetmodeID.Server)
                                    {
                                        Vector2 perturbedSpeed = NPC.velocity.RotatedByRandom(MathHelper.ToRadians(20));

                                        float scale = 1f - (Main.rand.NextFloat() * 0.75f);
                                        perturbedSpeed *= scale;

                                        Dust dust = Dust.NewDustDirect(pos + new Vector2(i * 16f, 0f), NPC.width, NPC.height, DustID.WoodFurniture, 0, 0, 100, default, 2f);
                                        dust.noGravity = true;
                                        dust.velocity *= 2f;
                                        dust = Dust.NewDustDirect(new Vector2(result.X + Main.rand.NextFloat(-32f, 33f),(result.Y + Main.rand.NextFloat(-64f, 1f))), NPC.width, NPC.height, DustID.WoodFurniture, 0f, 0f, 1000, default, 2f);
                                    }
                                }

                            }
                        }
                        SoundEngine.PlaySound(SoundID.DeerclopsScream with { Volume = 0.75f, Pitch = 1.3f }, NPC.Center); 
                    }
                    
                    if (NPC.frame.Y >= endingFrame * frameHeight) { NPC.frame.Y = endingFrame * frameHeight; }
                }
            }
        }
        public bool TryFindTreeTop(Vector2 position, out Vector2 result)
        {
            if (Main.tile[(int)position.X / 16, (int)position.Y / 16].TileType == TileID.Trees)
            {
                // Origin position, in tile format.
                int x = (int)(position.X / 16);
                int y = (int)(position.Y / 16);

                // Position being checked;

                int checkX = x;
                int checkY = y;

                // Checking up to a maximum of 30 tiles.
                for (int b = 0; b < 30; b++)
                {
                    // If this position is in the world, and if the tile is a Tree tile.
                    if (WorldGen.InWorld(checkX, y) && Main.tile[checkX, checkY].TileType == TileID.Trees)
                    {
                        // Checking if the tile's frames are within the range of tile frames used for the invisible tree top tiles.
                        if (Main.tile[checkX, checkY].TileFrameX == 22 && Main.tile[checkX, checkY].TileFrameY >= 198)
                        {
                            //Dust.QuickBox(new Vector2(checkX * 16, checkY * 16), new Vector2((checkX * 16) + 16, (checkY * 16) + 16), 10, Color.Yellow, null);
                            result = new Vector2(checkX * 16, checkY * 16);
                            return true;
                        }
                        // Otherwise, its a success, since it's still a tree tile. Just not the one we're looking for.
                        //Dust.QuickBox(new Vector2(checkX * 16, checkY * 16), new Vector2((checkX * 16) + 16, (checkY * 16) + 16), 10, Color.Green, null);
                        checkY--;
                    }
                    else
                    {
                        // If the tile isn't what we're looking for and since we're only iterating upwards, logically this means its useless to continue.
                        //Dust.QuickDustLine(new Vector2(checkX * 16, checkY * 16), new Vector2((checkX * 16) + 16, (checkY * 16) + 16), 5f, Color.Red);
                        //Dust.QuickDustLine(new Vector2(checkX * 16, (checkY * 16) + 16), new Vector2((checkX * 16) + 16, checkY * 16), 5f, Color.Red);
                        break;
                    }
                }
            }

            result = default;
            return false;
        }
    }
}




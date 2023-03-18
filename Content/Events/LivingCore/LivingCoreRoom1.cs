
using Divergency.Content.Particles;
using Divergency.Content.NPCs.LivingGrove;
using Divergency.Content.Tiles.LivingGrove;
using Divergency.Events.LivingCore;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore
{
    public static class LivingCoreRoom1
    {
        public static int Kills;
        public static int KillsNeeded;
        public static float Progress { get => (float)Kills / KillsNeeded; }
        public static int KillsRemaining { get => KillsNeeded - Kills; }

        public static int Timer;
        public static int SpawnTimer;
        public static WaveState State;

        public enum WaveState
        {
            Intermission,
            WaveOne,
            WaveTwo,
            WaveThree,
            WaveFour,
            Cleared
        }

        public static void Update()
        {
            Timer++;

            Rectangle textPosition = new(LivingCoreEvent.X * 16 + 24, LivingCoreEvent.Y * 16 + 24, 0, 0);

            // Wave handling
            if (KillsRemaining == 16 && State != WaveState.WaveOne)
            {
                if (SpawnTimer == 0)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "WAVE 1!", true, false);
                    State = WaveState.Intermission;
                }
                if (SpawnTimer == 100)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "3!", false, false);
                }
                if (SpawnTimer == 160)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "2!", false, false);
                }
                if (SpawnTimer == 220)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "1!", false, false);
                }
                if (SpawnTimer == 280)
                {
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300), ModContent.NPCType<Sage>());
                   // NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300), ModContent.NPCType<Coreling>());
                    //NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300), ModContent.NPCType<Coreling>());
                   // NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300), ModContent.NPCType<Coreling>());

                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300), Vector2.Zero, Color.White, 1f, 1.05f);
                   // ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300), Vector2.Zero, Color.White, 1f, 1.05f);
                    //ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300), Vector2.Zero, Color.White, 1f, 1.05f);
                   // ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300), Vector2.Zero, Color.White, 1f, 1.05f);

                    SpawnTimer = 0;
                    State = WaveState.WaveOne;
                }
            }
            else if (KillsRemaining == 12 && State != WaveState.WaveTwo)
            {
                if (SpawnTimer == 0)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "WAVE 2!", true, false);
                    State = WaveState.Intermission;
                }
                if (SpawnTimer == 100)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "3!", false, false);
                }
                if (SpawnTimer == 160)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "2!", false, false);
                }
                if (SpawnTimer == 220)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "1!", false, false);
                }
                if (SpawnTimer == 280)
                {
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300), ModContent.NPCType<Coreling>());
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300), ModContent.NPCType<Coreling>());
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300), ModContent.NPCType<Coreling>());
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300), ModContent.NPCType<Coreling>());

                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300), Vector2.Zero, Color.White, 1f, 1.05f);
                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300), Vector2.Zero, Color.White, 1f, 1.05f);
                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300), Vector2.Zero, Color.White, 1f, 1.05f);
                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300), Vector2.Zero, Color.White, 1f, 1.05f);

                    SpawnTimer = 0;
                    State = WaveState.WaveTwo;
                }
            }
            else if (KillsRemaining == 8 && State != WaveState.WaveThree)
            {
                if (SpawnTimer == 0)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "WAVE 3!", true, false);
                    State = WaveState.Intermission;
                }
                if (SpawnTimer == 100)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "3!", false, false);
                }
                if (SpawnTimer == 160)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "2!", false, false);
                }
                if (SpawnTimer == 220)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "1!", false, false);
                }
                if (SpawnTimer == 280)
                {
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300), ModContent.NPCType<Coreling>());
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300), ModContent.NPCType<Coreling>());
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300), ModContent.NPCType<Coreling>());
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300), ModContent.NPCType<Coreling>());

                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300), Vector2.Zero, Color.White, 1f, 1.05f);
                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300), Vector2.Zero, Color.White, 1f, 1.05f);
                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300), Vector2.Zero, Color.White, 1f, 1.05f);
                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300), Vector2.Zero, Color.White, 1f, 1.05f);

                    SpawnTimer = 0;
                    State = WaveState.WaveThree;
                }
            }
            else if (KillsRemaining == 4 && State != WaveState.WaveFour)
            {
                if (SpawnTimer == 0)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "WAVE 4!", true, false);
                    State = WaveState.Intermission;
                }
                if (SpawnTimer == 100)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "3!", false, false);
                }
                if (SpawnTimer == 160)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "2!", false, false);
                }
                if (SpawnTimer == 220)
                {
                    CombatText.NewText(textPosition, Color.LightGreen, "1!", false, false);
                }
                if (SpawnTimer == 280)
                {
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300), ModContent.NPCType<Coreling>());
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300), ModContent.NPCType<Coreling>());
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300), ModContent.NPCType<Coreling>());
                    NPC.NewNPCDirect(null, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300), ModContent.NPCType<Coreling>());

                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300), Vector2.Zero, Color.White, 1f, 1.05f);
                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300), Vector2.Zero, Color.White, 1f, 1.05f);
                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300), Vector2.Zero, Color.White, 1f, 1.05f);
                    ParticleManager.NewParticle<ResetParticle>(new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300), Vector2.Zero, Color.White, 1f, 1.05f);

                    SpawnTimer = 0;
                    State = WaveState.WaveFour;
                }
            }
            else if (KillsRemaining == 0)
            {
                State = WaveState.Cleared;

                LivingCoreEvent.End();
            }

            if (SpawnTimer > 100)
            {
                if (SpawnTimer % 5 == 0)
                {
                    float angle1 = Main.rand.NextFloat(MathHelper.TwoPi);
                    float angle2 = Main.rand.NextFloat(MathHelper.TwoPi);
                    float angle3 = Main.rand.NextFloat(MathHelper.TwoPi);
                    float angle4 = Main.rand.NextFloat(MathHelper.TwoPi);

                    for (int i = 0; i < 2; i++)
                    {
                        ParticleManager.NewParticle<FlareLineParticle>(new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300) + new Vector2(128f, 0f).RotatedBy(angle1), new Vector2(4f, 0f).RotatedBy(angle1 + MathHelper.Pi), new(0.50f, 2.05f, 0.5f, 0), 1f, Main.rand.NextFloat(0.8f, 1.1f));
                        ParticleManager.NewParticle<FlareLineParticle>(new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300) + new Vector2(128f, 0f).RotatedBy(angle2), new Vector2(4f, 0f).RotatedBy(angle2 + MathHelper.Pi), new(0.50f, 2.05f, 0.5f, 0), 1f, Main.rand.NextFloat(0.8f, 1.1f));
                        ParticleManager.NewParticle<FlareLineParticle>(new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300) + new Vector2(128f, 0f).RotatedBy(angle3), new Vector2(4f, 0f).RotatedBy(angle3 + MathHelper.Pi), new(0.50f, 2.05f, 0.5f, 0), 1f, Main.rand.NextFloat(0.8f, 1.1f));
                        ParticleManager.NewParticle<FlareLineParticle>(new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300) + new Vector2(128f, 0f).RotatedBy(angle4), new Vector2(4f, 0f).RotatedBy(angle4 + MathHelper.Pi), new(0.50f, 2.05f, 0.5f, 0), 1f, Main.rand.NextFloat(0.8f, 1.1f));
                    }

                  
                }
            }

            if (State == WaveState.Intermission)
            {
                SpawnTimer++;
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;

                if (player.active && player.dead)
                {
                    LivingCoreEvent.End();
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            Texture2D wave1 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1").Value;
            Texture2D wave2 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar2").Value;
            Texture2D wave3 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar3").Value;
            Texture2D wave4 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar4").Value;

            Texture2D altar = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1").Value;

            Vector2 offscreen = new(Main.offScreenRange);
            Vector2 position = new Vector2(LivingCoreEvent.X * 16f, LivingCoreEvent.Y * 16f) - Main.screenPosition + offscreen;

            if (KillsRemaining > 12)
            {
                spriteBatch.Draw(wave1, position - wave1.Size() + new Vector2(0,15), Color.White);
            }
            else if (KillsRemaining > 8)
            {
                spriteBatch.Draw(wave2, position - wave2.Size(), Color.White);
            }
            else if (KillsRemaining > 4)
            {
                spriteBatch.Draw(wave3, position - wave3.Size(), Color.White);
            }
            else if (KillsRemaining > 0)
            {
                spriteBatch.Draw(wave4, position - wave4.Size(), Color.White);
            }

            if (SpawnTimer >= 100)
            {
                Texture2D glow = ModContent.Request<Texture2D>("Divergency/Assets/Textures/ParticleTextures/SoftCircle").Value;
                Texture2D star = ModContent.Request<Texture2D>("Divergency/Assets/Textures/ParticleTextures/Star").Value;

                float alpha = (SpawnTimer - 100) / 180f;

                spriteBatch.Draw(glow, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300) - Main.screenPosition, glow.Bounds, new Color(0.50f, 2.05f, 0.5f, 0) * alpha, 0f, glow.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(glow, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300) - Main.screenPosition, glow.Bounds, new Color(0.50f, 2.05f, 0.5f, 0) * alpha, 0f, glow.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(glow, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300) - Main.screenPosition, glow.Bounds, new Color(0.50f, 2.05f, 0.5f, 0) * alpha, 0f, glow.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(glow, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300) - Main.screenPosition, glow.Bounds, new Color(0.50f, 2.05f, 0.5f, 0) * alpha, 0f, glow.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

                spriteBatch.Draw(star, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y - 300) - Main.screenPosition, star.Bounds, new Color(0.50f, 2.05f, 0.5f, 0) * alpha, 0f, star.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(star, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y - 300) - Main.screenPosition, star.Bounds, new Color(0.50f, 2.05f, 0.5f, 0) * alpha, 0f, star.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(star, new Vector2(LivingCoreEvent.Position.X - 300, LivingCoreEvent.Position.Y + 300) - Main.screenPosition, star.Bounds, new Color(0.50f, 2.05f, 0.5f, 0) * alpha, 0f, star.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(star, new Vector2(LivingCoreEvent.Position.X + 300, LivingCoreEvent.Position.Y + 300) - Main.screenPosition, star.Bounds, new Color(0.50f, 2.05f, 0.5f, 0) * alpha, 0f, star.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(altar, LivingCoreEvent.Position, Color.White);
        }

        public static void Begin(int i, int j)
        {
            NPC.NewNPCDirect(null, LivingCoreEvent.Position + new Vector2(24f), ModContent.NPCType<LivingCoreEventHandler>());

            int left = i - Main.tile[i, j].TileFrameX / 18;
            int top = j - Main.tile[i, j].TileFrameY / 18;

            WorldGen.PlaceTile(left - 74, top - 10, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left - 74, top - 11, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left - 74, top - 12, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left - 74, top - 13, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left - 74, top - 14, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left - 74, top - 15, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left - 74, top - 16, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left - 74, top - 17, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left - 74, top - 18, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left - 74, top - 19, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left - 74, top - 20, ModContent.TileType<CradleWood>());

            WorldGen.PlaceTile(left + 74, top, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top - 1, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top - 2, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top - 3, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top - 4, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top - 5, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top - 6, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top - 7, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top - 8, ModContent.TileType<CradleWood>());

            WorldGen.PlaceTile(left + 74, top + 1, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top + 2, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top + 3, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top + 4, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top + 5, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top + 6, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top + 7, ModContent.TileType<CradleWood>());
            WorldGen.PlaceTile(left + 74, top + 8, ModContent.TileType<CradleWood>());

            State = WaveState.Intermission;

            Kills = 0;
            KillsNeeded = 16;
            Timer = 0;
            SpawnTimer = 0;
        }

        public static void End()
        {
            Kills = 0;
            KillsNeeded = 0;
            Timer = 0;
            SpawnTimer = 0;


            State = WaveState.Intermission;

            int left = LivingCoreEvent.X - LivingCoreEvent.Altar.TileFrameX / 18;
            int top = LivingCoreEvent.Y - LivingCoreEvent.Altar.TileFrameY / 18;

            WorldGen.KillTile(left - 74, top - 10);
            WorldGen.KillTile(left - 74, top - 11);
            WorldGen.KillTile(left - 74, top - 12);
            WorldGen.KillTile(left - 74, top - 13);
            WorldGen.KillTile(left - 74, top - 14);
            WorldGen.KillTile(left - 74, top - 15);
            WorldGen.KillTile(left - 74, top - 16);
            WorldGen.KillTile(left - 74, top - 17);
            WorldGen.KillTile(left - 74, top - 18);
            WorldGen.KillTile(left - 74, top - 19);
            WorldGen.KillTile(left - 74, top - 20);

            WorldGen.KillTile(left + 74, top);
            WorldGen.KillTile(left + 74, top - 1);
            WorldGen.KillTile(left + 74, top - 2);
            WorldGen.KillTile(left + 74, top - 3);
            WorldGen.KillTile(left + 74, top - 4);
            WorldGen.KillTile(left + 74, top - 5);
            WorldGen.KillTile(left + 74, top - 6);
            WorldGen.KillTile(left + 74, top - 7);

            WorldGen.KillTile(left + 74, top + 1);
            WorldGen.KillTile(left + 74, top + 2);
            WorldGen.KillTile(left + 74, top + 3);
            WorldGen.KillTile(left + 74, top + 4);
            WorldGen.KillTile(left + 74, top + 5);
            WorldGen.KillTile(left + 74, top + 6);
            WorldGen.KillTile(left + 74, top + 7);
            WorldGen.KillTile(left + 74, top + 8);
        }
    }
}


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Divergency;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Divergency.Content.Tiles.LivingGrove;
using Divergency.Events.LivingCore;
using Divergency.Content.Particles;
using Divergency.Common.Helpers;
using Terraria.ID;
using ReLogic.Content;

namespace Divergency.Content.Events.LivingCore
{
    public class Reward
    {
        public Reward(int id, string texturePath) { this.id = id; this.texturePath = texturePath;  }
        public int id;
        public string texturePath;
    }

    public abstract class LivingCoreRoom
    {
        static Effect rewardEffect;
        static Matrix view = Matrix.CreateTranslation(0, 0, -600);

        public static void Setup()
        {
            rewardEffect = ModContent.Request<Effect>("Divergency/Content/Effects/3DShader", AssetRequestMode.ImmediateLoad).Value;
            rewardEffect.Parameters["View"].SetValue(view);
        }

        List<NPC> currentNPCs = new List<NPC>();

        public int Kills = 0;
        public float Progress => (float)TotalKills / (TotalEnemies);
        private int KillsRemaining { get => CurWaveObject != null ? CurWaveObject.enemies.Length - Kills : 0; }

        public static bool hasBeenCleared = false;

        private int Timer = 0;
        private int SpawnTimer = 0;
        private int CurWave = 0;

        private int TotalEnemies = 0;
        private int TotalKills = 0;

        private bool Intermission = false;

        private Wave CurWaveObject;

        private string[] Textures = new string[] {
            "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1",
            "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar2",
            "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar3",
            "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar4"
        };

        private int[] savedTiles = new int[0];

        public LivingCoreRoom()
        {
            TotalEnemies = 0;

            int curWaveTest = 1;
            Wave wave = getWave(curWaveTest);

            while (wave != null)
            {
                TotalEnemies += wave.enemies.Length;
                curWaveTest++;
                wave = getWave(curWaveTest);
            }
        }

        public virtual int Music { get { return 0; } }
        
        public virtual List<Reward> Rewards { get { return new List<Reward> { }; } }

        public virtual Vector2[] BlockingBlocks { get { return new Vector2[] {}; } }

        public virtual Wave? getWave(int wave)
        {
            return new Wave("Wave #&¤%!", new Instance[] {});
        }

        public virtual int getWaves()
        {
            return 0;
        }

        private void updateAltarReward()
        {
            int visualWave = CurWave - 1;
            if (visualWave < 0)
                visualWave = 0;
            if (visualWave >= Textures.Length)
                visualWave = Textures.Length - 1;

            Texture2D altarWave = ModContent.Request<Texture2D>(Textures[visualWave]).Value;
            Vector2 position = new Vector2(LivingCoreEvent.X * 16f, LivingCoreEvent.Y * 16f) - Main.screenPosition + altarWave.Size() / 2;
            
            Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);
			float multiplier = 0.4f;
			RGB *= multiplier;

			Lighting.AddLight(position, RGB.X, RGB.Y, RGB.Z);
        }

        private void drawAltarReward(Vector2 pos)
        {
            pos = pos - Main.ScreenSize.ToVector2() / 2;
            Matrix view = Matrix.CreateLookAt(new Vector3(0f, 0f, 644f * (Main.screenHeight / 1080f)), Vector3.Zero, Vector3.Up);
            rewardEffect.Parameters["View"].SetValue(view);

            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, rewardEffect, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < Rewards.Count; i++)
            {
                Matrix projection = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0.1f, 1000f);
                rewardEffect.Parameters["Projection"].SetValue(projection);
                // only needs to be done on resize, no?

                Matrix model = Matrix.CreateScale(1f);
                // model *= Matrix.CreateTranslation(0, 0, -29);

                // model *= Matrix.CreateTranslation(globalOffset);

                // model *= Matrix.CreateScale(scale);

                // model *= Matrix.CreateRotationX(rotation.X);
                // model *= Matrix.CreateRotationY(rotation.Y);
                // model *= Matrix.CreateRotationZ(rotation.Z);

                // model *= Matrix.CreateTranslation(localOffset);

                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Rewards[i].texturePath);

                int width, height;
                width = texture.Width;
                height = texture.Height;

                Rectangle sourceRectangle = new Rectangle(0, 0, width, height);
                Vector2 origin = new Vector2(width / 2f, height / 2f);

                int offset = (i - (Rewards.Count / 2));

                float r = (MathF.PI * (4f/3f) * (float)i / (float)Rewards.Count + Main.GlobalTimeWrappedHourly * 0f) % MathF.PI;

                float addY = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) * 10;
                float addX = offset * 60f;

                Vector2 target = pos + new Vector2(0f, -80f + addY);

                model *= Matrix.CreateRotationY(MathF.PI / 2);

                model *= Matrix.CreateTranslation(100f, 0f, 0f);

                model *= Matrix.CreateRotationY(r);

                model *= Matrix.CreateTranslation(target.X, -target.Y, 0f);

                // model.Translation = model.Translation - new Vector3(0f, 0f, model.Translation.Z);

                rewardEffect.Parameters["Model"].SetValue(model);

                Main.EntitySpriteDraw(texture,
                    Vector2.Zero, sourceRectangle,
                    Color.White, -MathF.PI / 4f, origin, 0.8f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
        }

        public void Update()
        {
            Timer++;

            Rectangle textPosition = new(LivingCoreEvent.X * 16 + 24, LivingCoreEvent.Y * 16 + 24, 0, 0);

            //Console.WriteLine(Timer + " | " + SpawnTimer + " | " + CurWave + " | " + KillsRemaining + " | " + TotalEnemies);

            if (SpawnTimer == 0 && KillsRemaining == 0 || KeybindSystem.Begin.JustPressed)
            {
                if (KeybindSystem.Begin.JustPressed)
                    killSpawnedEnemies();

                CurWave++;

                if (CurWave == getWaves() + 1)
                {
                    LivingCoreEvent.End();
                    return;
                }

                Kills = 0;
                CurWaveObject = getWave(CurWave);

                if (CurWaveObject == null)
                {
                    LivingCoreEvent.End();
                    return;
                }

                CombatText.NewText(textPosition, Color.LightGreen, CurWaveObject.name, true, false);
                Intermission = true;
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
                foreach (Instance instance in CurWaveObject.enemies)
                {
                    Vector2 spawnPosition = LivingCoreEvent.Center + instance.SpawnOffset;

                    currentNPCs.Add(NPC.NewNPCDirect(null, spawnPosition, instance.NPCID));

                    // ParticleManager.NewParticle<ResetParticle>(spawnPosition, Vector2.Zero, Color.White, 1f, 1.05f);
                    DivergencyDraw.SpawnExplosion(spawnPosition, Color.LimeGreen, DustID.GemEmerald, scale: 1.2f);

                }

                SpawnTimer = 0;
                Intermission = false;
            }

            if (SpawnTimer > 100)
            {
                foreach (Instance instance in CurWaveObject.enemies)
                {
                    Vector2 spawnPosition = LivingCoreEvent.Center + instance.SpawnOffset;

                    float rotation = Main.rand.NextFloat(MathHelper.TwoPi);

                    for (int i = 0; i < 2; i++)
                    {
                        ParticleManager.NewParticle<FlareLineParticle>(spawnPosition + new Vector2(128f, 0f).RotatedBy(rotation), new Vector2(4f, 0f).RotatedBy(rotation + MathHelper.Pi), new Color(0.50f, 2.05f, 0.5f, 0), 1f, Main.rand.NextFloat(0.8f, 1.1f));
                    }

                   // ParticleManager.NewParticle<CrystalParticle>(spawnPosition, Main.rand.NextVector2Circular(3f, 3f), Color.Purple, Main.rand.NextFloat(0.5f, 0.75f), 1f);
                }
            }

            if (Intermission)
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
                    return;
                }
            }

            clearList();
            if (!LivingCoreEvent.HasRoomBeenCleared(this.GetType()))
                updateAltarReward();
        }

        private void clearList()
        {
            foreach (NPC npc in currentNPCs)
            {
                if (npc.active == false)
                {
                    currentNPCs.Remove(npc);
                    clearList();
                    Kills++;
                    TotalKills++;
                    return;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D altarWave = default;

            int visualWave = CurWave - 1;
            if (visualWave < 0)
                visualWave = 0;
            if (visualWave >= Textures.Length)
                visualWave = Textures.Length-1;

            altarWave = ModContent.Request<Texture2D>(Textures[visualWave]).Value;

            Texture2D altar = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1").Value;

            Vector2 offscreen = new(Main.offScreenRange);
            Vector2 position = new Vector2(LivingCoreEvent.X * 16f, LivingCoreEvent.Y * 16f) - Main.screenPosition;

            spriteBatch.Draw(altarWave, position + new Vector2(3,15), Color.White);

            if (SpawnTimer >= 100)
            {
                Texture2D glow = ModContent.Request<Texture2D>("Divergency/Assets/Textures/ParticleTextures/SoftCircle").Value;
                Texture2D star = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Star").Value;

                float alpha = (SpawnTimer - 100) / 180f;

                foreach (Instance instance in CurWaveObject.enemies)
                {
                    Vector2 spawnPosition = LivingCoreEvent.Center + instance.SpawnOffset;

                    spriteBatch.Draw(glow, spawnPosition - Main.screenPosition, glow.Bounds, new Color(0.50f, 2.05f, 0.5f, 0) * alpha, 0f, glow.Size() * 0.5f, 0.4f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(star, spawnPosition - Main.screenPosition, star.Bounds, new Color(0.50f, 2.05f, 0.5f, 0) * alpha, 0f, star.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.Draw(altar, LivingCoreEvent.Position + new Vector2(3, 50), Color.White);

            // texture for reward

            if (!LivingCoreEvent.HasRoomBeenCleared(this.GetType()))
                drawAltarReward(position + altarWave.Size() / 2);
        }



        public void Begin(int i, int j)
        {
            NPC.NewNPCDirect(null, LivingCoreEvent.Position + new Vector2(24f), ModContent.NPCType<LivingCoreEventHandler>());

            int left = i - Main.tile[i, j].TileFrameX / 18;
            int top = j - Main.tile[i, j].TileFrameY / 18;

            savedTiles = new int[BlockingBlocks.Length];
            int counter = 0;
            foreach (Vector2 vec in BlockingBlocks)
            {
                savedTiles[counter] = WorldGen.TileType(left - (int)vec.X, top - (int)vec.Y);
                WorldGen.KillTile(left - (int)vec.X, top - (int)vec.Y, noItem: true);
                WorldGen.PlaceTile(left - (int)vec.X, top - (int)vec.Y, ModContent.TileType<CradleWood>());
                counter++;
            }

            Kills = 0;
            Timer = 0;
            CurWave = 0;
            SpawnTimer = 0;
        }

        public virtual void HasEnded() { }
        public void End()
        {
            HasEnded();

            if (CurWave == getWaves() + 1)
            {
                Texture2D altarWave = ModContent.Request<Texture2D>(Textures[CurWave - 2]).Value;
                Vector2 position = new Vector2(LivingCoreEvent.X * 16f, LivingCoreEvent.Y * 16f) + altarWave.Size() / 2;

                bool roomCleared = LivingCoreEvent.HasRoomBeenCleared(this.GetType());

                Console.WriteLine(roomCleared);

                if (!roomCleared)
                {
                    LivingCoreEvent.RoomCleared(this.GetType());
                    // Item.NewItem(null, position, RewardID);
                }

                Main.NewText("Cleared!");
            }

            Kills = 0;
            Timer = 0;
            CurWave = 0;
            SpawnTimer = 0;

            killSpawnedEnemies();

            int left = LivingCoreEvent.X - LivingCoreEvent.Altar.TileFrameX / 18;
            int top = LivingCoreEvent.Y - LivingCoreEvent.Altar.TileFrameY / 18;

            int counter = 0;
            foreach (Vector2 vec in BlockingBlocks)
            {
                if (savedTiles[counter] != -1)
                {
                    WorldGen.KillTile(left - (int)vec.X, top - (int)vec.Y, noItem: true);
                    WorldGen.PlaceTile(left - (int)vec.X, top - (int)vec.Y, savedTiles[counter]);
                }
                else
                    WorldGen.KillTile(left - (int)vec.X, top - (int)vec.Y, noItem: true);


                counter++;
            }
        }

        private void killSpawnedEnemies()
        {
            foreach (NPC npc in currentNPCs)
            {
                npc.active = false;
            }
        }
    }
}


using Divergency;
using Divergency.Common.Helpers;
using Divergency.Content.Particles;
using Divergency.Content.Tiles.LivingGrove;
using Divergency.Events.LivingCore;
using Divergency.Tiles.LivingTree;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using rail;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Divergency.Content.Events.LivingCore
{
    public class Reward
    {
        public Reward(TagCompound item, string texturePath) { this.item = item; this.texturePath = texturePath;  }
        public TagCompound item;
        public string texturePath;
        public TagCompound Save()
        {
            return new TagCompound
            {
                ["item"] = item,
                ["texturePath"] = texturePath
            };
        }

        public static Reward Load(TagCompound tag)
        {
            return new Reward(tag.GetCompound("item"), tag.GetString("texturePath"));
        }
    }

    public class LivingCoreRoom(LivingCoreAltarTileEntity roomBase = null)
    {
        static Effect rewardEffect;
        static Matrix view = Matrix.CreateTranslation(0, 0, -600);

        public LivingCoreAltarTileEntity RoomBase = roomBase;

        public virtual bool Valid
        {
            get {
                foreach (Wave wave in RoomBase.Waves)
                {
                    foreach (Instance enemy in wave.enemies)
                    {
                        if (enemy.NPCID == -1)
                        {
                            Console.WriteLine($"'{enemy.FullName}' is not a valid name/vanila Mob ID.");
                            return false; // an invalid npc.
                        }
                    }
                }
                return true;  }
        }

        private Vector3 lerp(Vector3 start, Vector3 stop, float t, bool curve = true)
        {
            if (curve)
                t = MathF.Pow(t, 2);
            return start * (1f - t) + stop * t;
        }
        private float lerp(float start, float stop, float t, bool curve = true)
        {
            if (curve)
                t = MathF.Pow(t, 2);
            return start * (1f - t) + stop * t;
        }

        public static void Setup()
        {
            rewardEffect = ModContent.Request<Effect>("Divergency/Content/Effects/3DShader", AssetRequestMode.ImmediateLoad).Value;
            rewardEffect.Parameters["View"].SetValue(view);
        }

        List<NPC> currentNPCs = new List<NPC>();

        public int Kills = 0;
        public float Progress => (float)TotalKills / (TotalEnemies);
        private int KillsRemaining { get => CurWaveObject != null ? CurWaveObject.enemies.Count - Kills : 0; }

        public static bool hasBeenCleared = false;

        private int Timer = 0;
        private int SpawnTimer = 0;
        private int CurWave = 0;

        private bool rewardPhase = false;
        private float rewardTransition = 0f;

        private int TotalEnemies = 0;
        private int TotalKills = 0;

        private bool Intermission = false;

        private Wave CurWaveObject;

        private int hoverReward = -1;

        private float rewardLastTransition = -1f;

        private string[] Textures = new string[] {
            "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1",
            "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar2",
            "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar3",
            "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar4"
        };

        private int[] savedTiles = new int[0];

        public virtual int Music { get { return MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/LivingGroveBattle1"); } }
        
        public virtual List<Reward> Rewards { get { return RoomBase.Rewards; } }

        public virtual List<Point16> BlockingBlocks { get { return RoomBase.BlockingBlocks; } }

        public virtual Wave? getWave(int wave)
        {
            if (wave > RoomBase.Waves.Count)
                return null;

            return RoomBase.Waves[wave-1];
        }

        public virtual int getWaves()
        {
            return RoomBase.Waves.Count - (Math.Max(0, RoomBase.ClaimedRewards.Select(t => t ? 0 : 1).Sum() - 1));
        }

        private void updateAltarReward()
        {
            // no clue what this is for...
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

        private Matrix FromEuler(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Matrix result = Matrix.CreateScale(1f);

            Matrix translationMatrix = Matrix.CreateTranslation(position);
            Matrix rotXMatrix = Matrix.CreateRotationX(rotation.X);
            Matrix rotYMatrix = Matrix.CreateRotationY(rotation.Y);
            Matrix rotZMatrix = Matrix.CreateRotationZ(rotation.Z);
            Matrix scaleMatrix = Matrix.CreateScale(scale);

            Matrix rotationMatrix = Matrix.Multiply(rotXMatrix, Matrix.Multiply(rotYMatrix, rotZMatrix));

            result = scaleMatrix * rotationMatrix * translationMatrix;

            return result;
        }

        private float RewardH = 0f;
        private void drawAltarReward()
        {
            Vector2 pos = LivingCoreEvent.Center - Main.screenPosition + new Vector2(0f, 24f) - Main.ScreenSize.ToVector2() / 2;
            Matrix view = Matrix.CreateLookAt(new Vector3(0f, 0f, 644f * (Main.screenHeight / 1080f)), Vector3.Zero, Vector3.Up);
            rewardEffect.Parameters["View"].SetValue(view);

            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, rewardEffect, Main.GameViewMatrix.TransformationMatrix);

            if (!rewardPhase)
            {
                for (int i = 0; i < Rewards.Count; i++)
                {
                    Color c = Color.White;

                    RewardH = Main.GlobalTimeWrappedHourly * 4 / Rewards.Count;
                    float r = (RewardH + MathF.PI * i / Rewards.Count) % MathF.PI;
                    float x = MathF.Cos(r);
                    float y = MathF.Sin(r);

                    Matrix projection = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0.1f, 1000f);
                    rewardEffect.Parameters["Projection"].SetValue(projection);
                    // only needs to be done on resize, no?

                    Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Rewards[i].texturePath);

                    int width, height;
                    width = texture.Width;
                    height = texture.Height;

                    Rectangle sourceRectangle = new Rectangle(0, 0, width, height);
                    Vector2 origin = new Vector2(width / 2f, height / 2f);

                    float addY = MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 10;
                    // float addX = offset * 60f;

                    Vector2 target = pos + new Vector2(0f, -80f + addY);

                    float hpi = MathF.PI / 2f;
                    float xrot = 0f;
                    /*
                    if (r < hpi)
                        xrot = hpi - r;
                    else
                        xrot = r - hpi;
                    */

                    float dist = Rewards.Count * 20;
                    rewardEffect.Parameters["Model"].SetValue(FromEuler(new Vector3(target.X + x * dist, -target.Y, y * dist), new Vector3(xrot, r + MathF.PI / 2f, 0f), new Vector3(1f, 1f, 1f)));

                    if (RoomBase.ClaimedRewards.Count < i || RoomBase.ClaimedRewards[i] == true) // if it has been claimed
                        c = Color.Gray;

                    Main.EntitySpriteDraw(texture,
                        Vector2.Zero, sourceRectangle,
                        c, -MathF.PI / 4f, origin, 0.8f, SpriteEffects.None, 0);
                }
            }
            else
            {
                if (rewardLastTransition < 0f)
                {
                    for (int i = 0; i < Rewards.Count; i++)
                    {
                        float r = (RewardH + MathF.PI * i / Rewards.Count) % MathF.PI;
                        float x = MathF.Cos(r);
                        float y = MathF.Sin(r);

                        Matrix projection = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0.1f, 1000f);
                        rewardEffect.Parameters["Projection"].SetValue(projection);
                        // only needs to be done on resize, no?

                        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Rewards[i].texturePath);

                        int width, height;
                        width = texture.Width;
                        height = texture.Height;

                        Rectangle sourceRectangle = new Rectangle(0, 0, width, height);
                        Vector2 origin = new Vector2(width / 2f, height / 2f);

                        float addY = MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 10;
                        // float addX = offset * 60f;

                        Vector2 target = pos + new Vector2(0f, -80f + addY);

                        float hpi = MathF.PI / 2f;
                        float xrot = 0f;
                        /*
                        if (r < hpi)
                            xrot = hpi - r;
                        else
                            xrot = r - hpi;
                        */

                        float dist = 80f + Rewards.Count * 20;

                        Vector3 prePos = new Vector3(target.X + x * dist, -target.Y, y * dist);
                        Vector3 preRot = new Vector3(xrot, r + MathF.PI / 2f, 0f);
                        float rr = MathF.PI / 2f;

                        if (Rewards.Count > 1)
                            rr = MathF.PI * ((float)i / (Rewards.Count - 1));
                        
                        float rx = MathF.Cos(rr);
                        float ry = MathF.Sin(rr);
                        Vector3 tPos = new Vector3(target.X + rx * dist, -target.Y + ry * dist, 0f);
                        Vector3 tRot = new Vector3(0f, 0f, 0f);
                        if (preRot.Y > MathF.PI / 2f)
                            tRot.Y = MathF.PI;

                        rewardEffect.Parameters["Model"].SetValue(FromEuler(
                            lerp(prePos, tPos, rewardTransition),
                            lerp(preRot, tRot, rewardTransition),
                            new Vector3(1f, 1f, 1f)));

                        Color c = Color.White;

                        float multW = 1f / MathF.Sqrt(2);
                        Vector2 playerM = Main.MouseScreen;
                        Vector2 TopLeftReward = target + Main.ScreenSize.ToVector2() / 2 + new Vector2(rx * dist, -ry * dist) - new Vector2(width / 2f * multW, height / 2f);

                        if (RoomBase.ClaimedRewards.Count < i || RoomBase.ClaimedRewards[i] == true) // if it has been claimed
                            c = Color.Gray;

                        if (playerM.X > TopLeftReward.X && playerM.Y > TopLeftReward.Y &&
                            playerM.X < TopLeftReward.X + width * multW && playerM.Y < TopLeftReward.Y + height)
                        {
                            hoverReward = i;
                            c = Color.Yellow;

                            if (RoomBase.ClaimedRewards.Count < i || RoomBase.ClaimedRewards[i] == true) // if it has been claimed
                                c = new Color(128, 128, 0, 255);
                        }

                        Main.EntitySpriteDraw(texture,
                            Vector2.Zero, sourceRectangle,
                            c, -MathF.PI / 4f, origin, 0.8f, SpriteEffects.None, 0);
                    }
                }
                else
                {
                    Color c = Color.White;

                    Matrix projection = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0.1f, 1000f);
                    rewardEffect.Parameters["Projection"].SetValue(projection);

                    Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Rewards[hoverReward].texturePath);

                    int width, height;
                    width = texture.Width;
                    height = texture.Height;

                    Rectangle sourceRectangle = new Rectangle(0, 0, width, height);
                    Vector2 origin = new Vector2(width / 2f, height / 2f);

                    Vector2 target = pos + new Vector2(0f, -80f);

                    float dist = 80f + Rewards.Count * 20;

                    float rr = MathF.PI / 2f;

                    if (Rewards.Count > 1)
                        rr = MathF.PI * ((float)hoverReward / (Rewards.Count - 1));

                    float rx = MathF.Cos(rr);
                    float ry = MathF.Sin(rr);
                    Vector3 tPos = new Vector3(target.X + rx * dist, -target.Y + ry * dist, 0f);

                    Vector3 t2Pos = new Vector3(pos.X, pos.Y, 0f);

                    rewardEffect.Parameters["Model"].SetValue(FromEuler(
                        lerp(tPos, t2Pos, rewardLastTransition),
                        new Vector3(0f, 0f, 0f),
                        new Vector3(1f, 1f, 1f)));

                    if (RoomBase.ClaimedRewards.Count < hoverReward || RoomBase.ClaimedRewards[hoverReward] == true) // if it has been claimed
                        c = Color.Gray;

                    Main.EntitySpriteDraw(texture,
                        Vector2.Zero, sourceRectangle,
                        c, -MathF.PI / 4f, origin, 0.8f, SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
        }

        public void Update()
        {
            Timer++;

            Rectangle textPosition = new(LivingCoreEvent.X * 16 + 24, LivingCoreEvent.Y * 16 + 24, 0, 0);

            //Console.WriteLine(Timer + " | " + SpawnTimer + " | " + CurWave + " | " + KillsRemaining + " | " + TotalEnemies);

            LivingCoreEvent.End();
            return;

            if (rewardPhase)
            {
                rewardTransition += 0.01f;
                if (rewardTransition >= 1f)
                    rewardTransition = 1f; // should be 1, testing

                if (rewardLastTransition >= 0f)
                {
                    rewardLastTransition += 0.05f;
                    if (rewardLastTransition >= 1f)
                    {
                        LivingCoreEvent.End();
                    }
                }

                return;
            }

            if (SpawnTimer == 0 && KillsRemaining == 0)
            {
                CurWave++;

                if (CurWave == getWaves() + 1)
                {
                    LivingCoreEvent.PreEnd();
                    return;
                }

                Kills = 0;
                CurWaveObject = getWave(CurWave);

                if (CurWaveObject == null || CurWaveObject.enemies.Count == 0)
                {
                    LivingCoreEvent.PreEnd();
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

            drawAltarReward();
        }



        public void Begin(int i, int j)
        {
            int curWaveTest = 1;
            Wave wave = getWave(curWaveTest);

            TotalEnemies = 0;

            while (wave != null)
            {
                TotalEnemies += wave.enemies.Count;
                curWaveTest++;
                wave = getWave(curWaveTest);
            }

            NPC.NewNPCDirect(null, LivingCoreEvent.Position + new Vector2(24f), ModContent.NPCType<LivingCoreEventHandler>());

            int left = i - Main.tile[i, j].TileFrameX / 18;
            int top = j - Main.tile[i, j].TileFrameY / 18;

            /*
            savedTiles = new int[BlockingBlocks.Length];
            int counter = 0;
            foreach (Vector2 vec in BlockingBlocks)
            {
                savedTiles[counter] = WorldGen.TileType(left - (int)vec.X, top - (int)vec.Y);
                WorldGen.KillTile(left - (int)vec.X, top - (int)vec.Y, noItem: true);
                WorldGen.PlaceTile(left - (int)vec.X, top - (int)vec.Y, ModContent.TileType<CradleWood>());
                counter++;
            }
            */

            Kills = 0;
            Timer = 0;
            CurWave = 0;
            SpawnTimer = 0;
            rewardPhase = false;
            rewardTransition = 0f;
            hoverReward = -1;
            rewardLastTransition = -1f;
        }

        public virtual void HasEnded() { }

        public void PreEnd()
        {
            rewardPhase = true;
        }

        public void RequestReward()
        {
            if (rewardPhase && rewardTransition == 1f && hoverReward != -1)
            {
                rewardLastTransition = 0f;
            }
        }
        
        public void End()
        {
            HasEnded();

            if (hoverReward != -1)
            {
                if (RoomBase.ClaimedRewards.Count < hoverReward || RoomBase.ClaimedRewards[hoverReward] == false)
                {
                    Item item = ItemIO.Load(Rewards[hoverReward].item);
                    Item.NewItem(null, LivingCoreEvent.Center, item);
                    Main.NewText("Cleared!");
                }
                else
                {
                    // TODO: drop currency?
                }

                RoomBase.ClaimedRewards[hoverReward] = true;
            }

            Kills = 0;
            Timer = 0;
            CurWave = 0;
            SpawnTimer = 0;
            rewardPhase = false;
            rewardTransition = 0f;
            hoverReward = -1;
            rewardLastTransition = -1f;

            killSpawnedEnemies();

            /*
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
            */

            foreach (Point16 vec in BlockingBlocks)
            {
                Point16 pos = RoomBase.Position + vec;
                WorldGen.KillTile(pos.X, pos.Y, noItem: true);
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

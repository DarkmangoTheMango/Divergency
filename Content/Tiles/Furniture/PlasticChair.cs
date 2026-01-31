using Divergency.Content.Particles;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Divergency.Content.Tiles.Furniture
{
    public class PlasticChair : ModTile
    {
        public const int NextStyleHeight = 40;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSatOnForNPCs[Type] = true;
            TileID.Sets.CanBeSatOnForPlayers[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

            DustType = DustID.Asphalt;
            AdjTiles = [TileID.Chairs];

            AddMapEntry(new Color(214, 215, 207), Language.GetText("MapObject.Chair"));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance); // Avoid being able to trigger it from long range
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int left = i - (tile.TileFrameX / 18) % 2;
            int top = j - (tile.TileFrameY / 18) % 2;
            int bottom = top + 1;

            bool facingRight = tile.TileFrameX >= 36;

            info.TargetDirection = facingRight ? 1 : -1;

            int seatX = facingRight ? left + 1 : left;

            info.AnchorTilePosition = new Point(seatX, bottom);

            info.DirectionOffset = info.RestingEntity is Player ? 6 : 2;
            info.VisualOffset = new Vector2(facingRight ? -10 : -10, 0);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
                player.GetModPlayer<StormPlayer>().Motivated = true;
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
                return;

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<PlasticChairItem>();

            if (Main.tile[i, j].TileFrameX / 18 < 1)
                player.cursorItemIconReversed = true;
        }
    }

    public class PlasticChairItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PlasticChair>());
        }
    }

    public class StormPlayer : ModPlayer
    {
        public bool Motivated;

        public override void PostUpdate()
        {
            if (!Player.sitting.isSitting)
                Motivated = false;
        }
    }

    public class StormSkyScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override int Music => MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/BuryTheLight");

        public override void SpecialVisuals(Player player, bool isActive) => player.ManageSpecialBiomeVisuals("Divergency:StormSky", isActive);

        public override bool IsSceneEffectActive(Player player) => player.GetModPlayer<StormPlayer>().Motivated;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Filters.Scene["Divergency:StormSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(Color.Transparent).UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance["Divergency:StormSky"] = new StormSky();
            }
        }
    }

    public class StormSky : CustomSky
    {
        public class Particle(Vector2 position, Vector2 velocity, int lifetime)
        {
            public int TimeLeft;
            public int Lifetime = lifetime;
            public int ID = Particles.Count;
            public Vector2 Velocity = velocity;
            public Vector2 Position = position;
        }

        public static List<Particle> Particles
        {
            get;
            internal set;
        } = [];


        private bool Active
        {
            get;
            set;
        }

        private float Intensity
        {
            get;
            set;
        }

        public override float GetCloudAlpha() => MathHelper.Lerp(1, 0, Intensity);

        public override Color OnTileColor(Color inColor) => inColor * MathHelper.Lerp(1, 0.1f, Intensity);

        public override bool IsActive() => Active || Intensity > 0;

        public override void Activate(Vector2 position, params object[] args)
        {
            Active = true;
        }

        public override void Deactivate(params object[] args)
        {
            Active = false;
        }

        public override void Reset()
        {
            Active = false;
        }

        public override void Update(GameTime gameTime)
        {

            for (int k = 0; k < 20; k++)
                Particles.Add(new Particle(new Vector2(Main.screenWidth * Main.rand.NextFloat(-1, 1), -Main.screenHeight), Vector2.UnitY.RotatedBy(-0.1f) * 200, 100));

            UpdateParticles();

            if (Main.gamePaused)
                return;

            if (Active && Intensity < 1f)
                Intensity += 0.01f;
            else if (!Active && Intensity > 0)
                Intensity -= 0.01f;
        }

        private static void UpdateParticles()
        {
            for (int k = 0; k < Particles.Count; k++)
            {
                var particle = Particles[k];

                particle.TimeLeft++;
                particle.Position += particle.Velocity;
            }

            Particles.RemoveAll(p => p.TimeLeft >= p.Lifetime);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            DrawBackground(spriteBatch);
            DrawParticles(spriteBatch);
        }

        private void DrawBackground(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Noise/CloudyNoise").Value;

            Effect shader = Divergency.Storm.Value;

            Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Noise/TurbulentNoise").Value;

            shader.Parameters["uTime"]?.SetValue((float)Main.timeForVisualEffects * 0.0005f);
            shader.Parameters["alpha"]?.SetValue(Intensity);
            shader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, shader, Main.UIScaleMatrix);

            spriteBatch.Draw(texture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        }

        private void DrawParticles(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Beam").Value;

            for (int k = 0; k < Particles.Count; k++)
            {
                var particle = Particles[k];

                spriteBatch.Draw(texture, particle.Position, texture.Bounds, new Color(90, 150, 255, 0) * 0.1f * Intensity, particle.Velocity.ToRotation() + MathHelper.PiOver2, texture.Size() * 0.5f, new Vector2(0.4f, 10), SpriteEffects.None, 0f);
            }
        }
    }
}


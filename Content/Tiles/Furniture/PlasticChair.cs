using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using ParticleLibrary;
using Divergency.Content.Dusts;

namespace Divergency.Content.Tiles.Furniture
{
    public class PlasticChair : ModTile
    {
        public const int NextStyleHeight = 40; // Calculated by adding all CoordinateHeights + CoordinatePaddingFix.Y applied to all of them + 2

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            //TileID.Sets.HasOutlines[Type] = true;
            //TileID.Sets.CanBeSatOnForNPCs[Type] = true; // Facilitates calling ModifySittingTargetInfo for NPCs
            TileID.Sets.CanBeSatOnForPlayers[Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

            DustType = ModContent.DustType<Smoke>();

            // Names
            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Chair"));

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
            // The following 3 lines are needed if you decide to add more styles and stack them vertically
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);

        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            // Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<>());
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance); // Avoid being able to trigger it from long range
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            // It is very important to know that this is called on both players and NPCs, so do not use Main.LocalPlayer for example, use info.restingEntity

            int left = i - Main.tile[i, j].TileFrameX / 18;
            int right = i + Main.tile[i, j].TileFrameX / 18;

            int top = j - Main.tile[i, j].TileFrameY / 18;


            Vector2 pos = new Vector2(left * 16f + 32f, top * 16f + 8f);
            Tile tile = Framing.GetTileSafely(left, top);

            //info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
            info.VisualOffset = new Vector2(-10, 0); // Defaults to (0,0)

            info.TargetDirection = -1;
            if (tile.TileFrameX != 0)
            {
                info.TargetDirection = 1; // Facing right if sat down on the right alternate (added through addAlternate in SetStaticDefaults earlier)
            }


            // The anchor represents the bottom-most tile of the chair. This is used to align the entity hitbox
            // Since i and j may be from any coordinate of the chair, we need to adjust the anchor based on that

            info.AnchorTilePosition.X = left; // Our chair is only 1 wide, so nothing special required
            info.AnchorTilePosition.Y = top;




            if (tile.TileFrameY % NextStyleHeight == 0)
            {
                info.AnchorTilePosition.Y++; // Here, since our chair is only 2 tiles high, we can just check if the tile is the top-most one, then move it 1 down

            }
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            { // Avoid being able to trigger it from long range
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
                player.GetModPlayer<ChairPlayer>().Motivated = true;

            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            { // Match condition in RightClick. Interaction should only show if clicking it does something
                return;
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<PlasticChairItem>();

            if (Main.tile[i, j].TileFrameX / 18 < 1)
            {
                player.cursorItemIconReversed = true;
            }
        }
    }
    public class PlasticChairItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ////.setdefault("Forsakened, I am awakened...");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PlasticChair>());
            Item.value = 150;
            Item.maxStack = 9999;
            Item.width = 12;
            Item.height = 30;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
                
        }
    }
    public class ChairPlayer : ModPlayer
    {
        public bool Motivated;
        public int timer;
        private int timer2;

        public override void PostUpdate()
        {
            if (!Player.sitting.isSitting)
            {
                Motivated = false;
                Main.StopRain();
            }
            if (Motivated && Player.sitting.isSitting)
            {
                timer++;
                timer2++;
                Main.StartRain();
                Main.maxRaining = 0.5f;
                Main.windSpeedCurrent = 0.5f;
                Main.UseStormEffects = true;
                if (timer == 10)
                {
                    for (int j = 0; j < 3; j++)

                    {
                        Vector2 ParticleLoc = Player.Center + Main.rand.NextVector2Circular(1, 1f) * 1000;
                        ParticleManager.NewParticle(ParticleLoc - new Vector2(1500, 0), new Vector2(Player.direction * -9, 0), ParticleManager.NewInstance<WindParticle>(), new(2.55f, 2.55f, 2.55f, 0), 0.2f, Main.rand.NextFloat(0.5f, 2f), Layer: Particle.Layer.BeforeNPCsBehindTiles);


                    }
                    timer = 0;

                }
                if (timer2 == 60)
                {
                    Main.NewLightning();
                    timer2 = 0;
                }
       

            }
            
        }
    }
    public class VergilEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/BuryTheLight");

        public const string ScreenFilterKey = "Divergency: VergilFilter";
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment; // We have set the SceneEffectPriority to be BiomeLow for purpose of example, however default behavour is BiomeLow.
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float r = 1.64f;
                float g = 2.11f;
                float b = 2.38f;
                r += 0.5f;
                g += 0.5f;
                Filters.Scene[ScreenFilterKey] = new Filter(new ScreenShaderData("FilterMoonLord").UseColor(r / 2, g / 2, b), EffectPriority.High); 


                // To bind a screen shader, use this.
                // EffectPriority should be set to whatever you think is reasonable.   


            }
        }
        public override void Unload()
        {
        }
        public override bool IsSceneEffectActive(Player player)
        {
            return player.sitting.isSitting && player.GetModPlayer<ChairPlayer>().Motivated;
        }
        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
            {
                if (!Filters.Scene[ScreenFilterKey].Active)
                {
                    Filters.Scene.Activate(ScreenFilterKey, player.Center);
                }
            }
            else if (Filters.Scene[ScreenFilterKey].Active)
            {
                Filters.Scene.Deactivate(ScreenFilterKey);
            }
        }

    }
    public class WindParticle : Particle
    {
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/SoftCircle";

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = 200;
            opacity = 125;
            layer = Layer.BeforeNPCsBehindTiles;
        }

        public override void AI()
        {
            rotation = velocity.ToRotation();

            Scale = ai[0] == 0f ? 1f : ai[0];
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, color * alpha, rotation, texture.Size() * 0.5f, Scale * new Vector2(0.1f, 0.005f), SpriteEffects.None, 0f);

            return false;
        }
    }
}


//using Divergency;
//using Divergency.Content.Tiles.LivingGrove;
//using Microsoft.Xna.Framework;
//using SubworldLibrary;
//using System.Collections.Generic;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.GameContent.Creative;
//using Terraria.ID;
//using Terraria.IO;
//using Terraria.ModLoader;
//using Terraria.WorldBuilding;

//public class ExampleSubworld : Subworld
//{
//    public override int Width => 1000;
//    public override int Height => 1000;

//    public override bool ShouldSave => true;
//    public override bool NoPlayerSaving => true;

//    public override List<GenPass> Tasks => new List<GenPass>()
//    {
//        new ExampleGenPass()
//    };

//    // Sets the time to the middle of the day whenever the subworld loads
//    public override void OnLoad()
//    {
//        Main.dayTime = true;
//        Main.time = 27000;
//    }
    
//}

//public class ExampleGenPass : GenPass
//{
//    //TODO: remove this once tML changes generation passes
//    public ExampleGenPass() : base("goofy ahh", 1) { }

//    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
//    {
//        progress.Message = "goofy ahh"; // Sets the text displayed for this pass
//        Main.worldSurface = Main.maxTilesY - 42; // Hides the underground layer just out of bounds
//        Main.rockLayer = Main.maxTilesY; // Hides the cavern layer way out of bounds
//        for (int i = 0; i < Main.maxTilesX; i++)
//        {
//            for (int j = 0; j < Main.maxTilesY; j++)
//            {
//                progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
//                Tile tile = Main.tile[i, j];
//                tile.HasTile = true;
//                tile.TileType = (ushort)ModContent.TileType<CradleWood>();
//            }
//        }
//    }
//    public class UpdateSubworldSystem : ModSystem
//    {
//        public override void PreUpdateWorld()
//        {
//            if (SubworldSystem.IsActive<ExampleSubworld>())
//            {
//                // Update mechanisms
//                Wiring.UpdateMech();

//                // Update tile entities
//                TileEntity.UpdateStart();
//                foreach (TileEntity te in TileEntity.ByID.Values)
//                {
//                    te.Update();
//                }
//                TileEntity.UpdateEnd();

//                // Update liquid
//                if (++Liquid.skipCount > 1)
//                {
//                    Liquid.UpdateLiquid();
//                    Liquid.skipCount = 0;
//                }
//            }
//        }
//    }
//    internal class LivingCorePortal : ModItem
//    {
//        public override string Texture => "Divergency/Assets/Textures/Star";

//        public override void SetStaticDefaults()
//        {
//            //.setdefault("Living Core Portal");
//            ////.setdefault("Used for crafting living core items");
//            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
//        }   

//        public override void SetDefaults()
//        {
//            Item.consumable = true;
//            Item.maxStack = 999;
//            Item.value = Item.sellPrice(0, 0, 0, 0);
//            Item.rare = ItemRarityID.Green;

//            Item.width = Item.height = 16;
//            Item.scale = 1f;

//            Item.useTime = 10;
//            Item.useAnimation = 15;
//            Item.useStyle = ItemUseStyleID.Swing;
//            Item.autoReuse = true;
//            Item.useTurn = true;
//            Item.shoot = ProjectileID.FireArrow;

//        }
//        public override void OnConsumeItem(Player player)
//        {
//            SubworldSystem.Enter<ExampleSubworld>();
//        }
//        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
//        {
//            SubworldSystem.Enter<ExampleSubworld>();

//            return base.Shoot(player, source, position, velocity, type, damage, knockback);

//        }
//    }
//}


using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class LivingCrystalSmall1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            DustType = ModContent.DustType<LivingShard>();
            HitSound = SoundID.DD2_WitherBeastCrystalImpact;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(109, 225, 90));
        }
        public override bool KillSound(int i, int j, bool fail)
        {
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastHurt, new Vector2(i, j).ToWorldCoordinates());
                return false;
            }

            return base.KillSound(i, j, fail);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.025f;
            g = 0.1f;
            b = 0.045f;
        }
    }

    public class LivingCrystalSmall2 : LivingCrystalSmall1
    {

    }

    public class LivingCrystalSmall3 : LivingCrystalSmall1
    {

    }

    public class LivingCrystalTestItem3 : ModItem
    {
        public override string Texture => "Divergency/Assets/Textures/Star";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Crystal Test Item 3");
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Orange;

            Item.createTile = ModContent.TileType<LivingCrystalSmall1>();
            Item.placeStyle = 0;

            Item.width = Item.height = 16;
            Item.scale = 1f;

            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
        }
    }

    public class LivingCrystalTestItem4 : ModItem
    {
        public override string Texture => "Divergency/Assets/Textures/Star";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Crystal Test Item 4");
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Orange;

            Item.createTile = ModContent.TileType<LivingCrystalSmall2>();
            Item.placeStyle = 0;

            Item.width = Item.height = 16;
            Item.scale = 1f;

            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
        }
    }

    public class LivingCrystalTestItem5 : ModItem
    {
        public override string Texture => "Divergency/Assets/Textures/Star";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Crystal Test Item 5");
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Orange;

            Item.createTile = ModContent.TileType<LivingCrystalSmall3>();
            Item.placeStyle = 0;

            Item.width = Item.height = 16;
            Item.scale = 1f;

            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
        }
    }
}

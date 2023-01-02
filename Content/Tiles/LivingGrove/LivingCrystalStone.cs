using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Content.Dusts;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class LivingCrystalStone : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileMerge[Type][ModContent.TileType<CradleWood>()] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = ModContent.DustType<LivingShard>();
            ItemDrop = ModContent.ItemType<LivingCrystalStoneItem>();
            HitSound = SoundID.DD2_WitherBeastCrystalImpact;

            AddMapEntry(new Color(79, 214, 126));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 79f * 0.2f;
            g = 214f * 0.2f;
            b = 126f * 0.2f;
        }
    }

    public class LivingCrystalStoneItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Crystal Stone");
            Tooltip.SetDefault("Used for crafting living core items");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.createTile = ModContent.TileType<LivingCrystalStone>();
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
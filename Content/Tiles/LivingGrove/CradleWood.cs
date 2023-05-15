using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Content.Dusts;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class CradleWood : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = ModContent.DustType<CradleWoodFurniture>();
           // ItemDrop = ModContent.ItemType<CradleWoodItem>();
            HitSound = new SoundStyle("Divergency/Assets/Sounds/Tiles/CradleWoodHit") with { PitchVariance = 0.1f };

            AddMapEntry(new Color(81, 44, 57));
        }
    }

    public class CradleWoodItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Cradle Wood");
            ////.setdefault("Used for crafting living core items");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.createTile = ModContent.TileType<CradleWood>();
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
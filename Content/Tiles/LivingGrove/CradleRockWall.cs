using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class CradleRockWallTile : ModWall
    {

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;

            DustType = 7;
            //ItemDrop = ModContent.ItemType<Items.Placeable.ExampleWall>();
            DustType = 7;

            AddMapEntry(new Color(30, 55, 59));
        }

    }


    internal class CradleRockWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 500;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.value = 1000;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.White;
            Item.createWall = ModContent.WallType<CradleRockWallTile>();

        }
    }
}


using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class LivingCrystalStone : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileMerge[Type][ModContent.TileType<CradleWood>()] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;

            DustType = ModContent.DustType<LivingShard>();
            //ItemDrop = ModContent.ItemType<LivingCrystalStoneItem>();
            HitSound = SoundID.DD2_WitherBeastCrystalImpact;

            AddMapEntry(new Color(79, 214, 126));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 79f * 0.9f;
            g = 214f * 0.9f;
            b = 126f * 0.9f;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            Texture2D tex = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/LivingCrystalStoneGlow").Value;
            int height = tile.TileFrameY == 36 ? 18 : 16;
            if (tile.Slope == 0 && !tile.IsHalfBlock)
            {
              Main.spriteBatch.Draw(tex, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }

    public class LivingCrystalStoneItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Living Crystal Stone");
            ////.setdefault("Used for crafting living core items");

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
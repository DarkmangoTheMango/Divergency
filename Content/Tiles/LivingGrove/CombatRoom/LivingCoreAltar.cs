using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Divergency.Common.Helpers;
using Divergency.Content.Events.LivingCore;
using System;

namespace Divergency.Tiles.LivingTree
{
    public class LivingCoreAltarTile1 : ModTile
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            Main.tileBouncy[Type] = true;

            Main.tileLighted[Type] = false;
            Main.tileAxe[Type] = false;
            Main.tileBrick[Type] = false;
            Main.tileHammer[Type] = false;
            Main.tileAlch[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 85, 60), Language.GetText("Living Core Altar"));
            DustType = 7;
        }

        public override bool RightClick(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18;
            int top = j - Main.tile[i, j].TileFrameY / 18;

            LivingCoreEvent.Begin(left, top, new Content.Events.LivingCore.Rooms.FirstRoom());
            // TODO: Netsync begin signal

            return true;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1").Value;
            Color color = Lighting.GetColor(i,j);

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                if ((LivingCoreEvent.X != i && LivingCoreEvent.Y != j))
                {
                    spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(0, 15), color);
                    Main.tileHammer[Type] = false;
                }
            }

            return false;
        }
    }

    public class LivingCoreAltar : ModItem
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1";

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.value = 1000;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.White;
            Item.createTile = ModContent.TileType<LivingCoreAltarTile1>();
        }
    }
}

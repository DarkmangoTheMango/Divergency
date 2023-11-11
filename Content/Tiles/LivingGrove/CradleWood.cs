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
using System.Threading;
using Humanizer;
using static log4net.Appender.ColoredConsoleAppender;
using System.Collections.Generic;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class CradleWood : ModTile
    {
        private float timer = 0;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = ModContent.DustType<CradleWoodFurniture>();
            //ItemDrop = ModContent.ItemType<CradleWoodItem>();
            HitSound = new SoundStyle("Divergency/Assets/Sounds/Tiles/CradleWoodHit") with { PitchVariance = 0.1f };

            AddMapEntry(new Color(81, 44, 57));
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {

            Vector2 pos = new Vector2(i * 16, j * 16);

            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            Texture2D tex = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CradleWoodGlow").Value;
            int height = tile.TileFrameY == 36 ? 18 : 16;

            const float GlowDistanceSq = 96 * 96;
            var glowFactor = 0f;
            for (var ii = 0; ii < Main.maxPlayers; ii++)
            {
                var player = Main.player[ii];
                if (!player.active)
                    continue;

                var amt = 1f - Math.Clamp(Main.player[ii].DistanceSQ(new Point(i, j).ToWorldCoordinates()) / GlowDistanceSq, 0, 1);

                if ((glowFactor += amt) >= 1.0f)    
                {
                    break;
                }
            }
            if (tile.Slope == 0 && !tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(tex, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White * glowFactor * 1f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
           



         
            
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
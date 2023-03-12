using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Color = Microsoft.Xna.Framework.Color;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class FancyCoreCrystal : ModTile
    {
        int timer = 300;
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            
            DustType = ModContent.DustType<LivingShard>();
            HitSound = SoundID.DD2_WitherBeastCrystalImpact;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(109, 225, 90));
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {

            Texture2D tex = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/FancyCoreCrystal").Value;
            Texture2D tex1 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/FancyCoreCrystal1").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/FancyCoreCrystal2").Value;
            Texture2D tex3 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/FancyCoreCrystal3").Value;
            Texture2D tex4 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/FancyCoreCrystal4").Value;

            int left = i - Main.tile[i, j].TileFrameX / 18;
            int top = j - Main.tile[i, j].TileFrameY / 18;
            Vector2 pos = new Vector2(left * 16 + 16f, top * 16 + 16f);

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
      
                timer--;
                
                Color color = Color.Lerp(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), Color.Multiply(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), 0.5f), MathF.Sin(timer));

                spriteBatch.Draw(tex, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(-10, -20), color);
                spriteBatch.Draw(tex1, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(-10, -20), color);
                spriteBatch.Draw(tex2, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(-10, -20), color);
                spriteBatch.Draw(tex3, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(-10, -20), color);
                spriteBatch.Draw(tex4, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(-10, -20), color);


            }
        }
        public override bool KillSound(int i, int j, bool fail)
        {
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, new Vector2(i, j).ToWorldCoordinates());
                return false;
            }

            return base.KillSound(i, j, fail);
        }

      
    }

  

    public class FancyCoreCrystalItem : ModItem
    {
        public override string Texture => "Divergency/Assets/Textures/Star";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("fance");
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Orange;

            Item.createTile = ModContent.TileType<FancyCoreCrystal>();
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

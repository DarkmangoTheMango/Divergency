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
    public class CradleVineTile : ModTile
    {

        public override void SetStaticDefaults()
        {
            //Main.tileCut[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = false;

            HitSound = SoundID.Grass;
            DustType = DustID.Ash;

            AddMapEntry(new Color(73, 74, 89));
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Framing.GetTileSafely(i, j + 1);
            if (tile.HasTile && tile.TileType == Type)
            {
                WorldGen.KillTile(i, j + 1);
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            int type = -1;
            if (tileAbove.HasTile && tileAbove.Slope != SlopeType.SlopeUpLeft && tileAbove.Slope != SlopeType.SlopeUpRight)
            {
                type = tileAbove.TileType;
            }

            if (type == ModContent.TileType<CradleWood>() || type == Type || type == ModContent.TileType<CradleWood>())
            {
                return true;
            }

            WorldGen.KillTile(i, j);
            return true;
        }

       
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
            Rectangle realSource = source;

            float xOff = GetOffset(i, j, tile.TileFrameX); //Sin offset.
            Vector2 drawPos = ((new Vector2(i, j)) * 16) - Main.screenPosition;

            Color col = Lighting.GetColor(i, j, Color.White);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            spriteBatch.Draw(ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CradleVineTile").Value, drawPos + zero - new Vector2(xOff, 5), realSource, new Color(col.R, col.G, col.B, 255), 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            return false;
        }
        public float GetOffset(int i, int j, int frameX, float sOffset = 0f)
        {
            float sin = (float)Math.Sin((Main.time + (i * 24) + (j * 19)) * (0.04f * (!Lighting.NotRetro ? 0f : 1)) + sOffset) * 1.4f;
            if (Framing.GetTileSafely(i, j - 1).TileType != Type) 
                sin *= 0.50f;
            else if (Framing.GetTileSafely(i, j - 2).TileType != Type)
                sin *= 0.75f;
            else if (Framing.GetTileSafely(i, j - 3).TileType != Type)
                sin *= 1.1f;

            return sin;
        }


    }
    internal class CradleVine : ModItem
    {

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
            Item.createTile = ModContent.TileType<CradleVineTile>();
        }
    }
}

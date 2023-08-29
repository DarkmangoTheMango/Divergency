using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Content.Dusts;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class CoreTreeMultitile : ModTile
    {

    
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 18;
            TileObjectData.newTile.Height = 19;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<LivingShard>();
            HitSound = SoundID.ScaryScream;
            AddMapEntry(new Color(151, 107, 75), Language.GetText("MapObject.koga shenanigans"));
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return true;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];

            if (tile == null || !tile.HasTile) { return;}

            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CoreTreeTrunk").Value;

            Vector2 offScreen = new Vector2(Main.offScreenRange);
            Vector2 globalPosition = p.ToWorldCoordinates(0f, 0f);
            Vector2 position = globalPosition + offScreen - Main.screenPosition;
            Color color = Lighting.GetColor(i, j, Color.White);

          //  Main.EntitySpriteDraw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            //END OF TRUNK
        }

    }
}
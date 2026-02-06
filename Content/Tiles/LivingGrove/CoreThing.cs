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
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Divergency.Common.Helpers.SwordAnimator;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class CoreThing : ModTile
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CoreThing";
        public const int FrameWidth = 16 * 19;
        public const int FrameHeight = 16 * 11;
        public const int HorizontalFrames = 1;
        public const int VerticalFrames = 1;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 19;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16,16,16,16,16};
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 0;
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
        Color[] colorsSliced = new Color[4];



        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) { Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j); }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];

            if (tile == null || !tile.HasTile) { return; }

            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CoreThing").Value;

            Texture2D glow = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CoreThingGlow").Value;

            Vector2 offScreen = new Vector2(Main.offScreenRange);
            Vector2 globalPosition = p.ToWorldCoordinates(0f, 0f);
            Vector2 position = globalPosition + offScreen - Main.screenPosition - new Vector2(0, 0);
            Color color = Lighting.GetColor(i, j);
            Lighting.GetColor4Slice(i, j, ref colorsSliced);

            Main.EntitySpriteDraw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(glow, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);



        }

    }
}
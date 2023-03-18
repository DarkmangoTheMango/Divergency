using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class LivingCombatAltar : ModTile
    {
        public const int FrameWidth = 16 * 3;
        public const int FrameHeight = 16 * 4;
        public const int HorizontalFrames = 1;
        public const int VerticalFrames = 1;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            
            DustType = ModContent.DustType<LivingShard>();
            HitSound = SoundID.DD2_WitherBeastCrystalImpact;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 4;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(109, 225, 90), Language.GetText("Combat Altar"));
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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.05f;
            g = 0.2f;
            b = 0.09f;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) { Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j); }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new Vector2(Main.offScreenRange);

            if (Main.drawToScreen) { offScreen = Vector2.Zero; }

            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];

            if (tile == null || !tile.HasTile) { return; }

            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/LivingCombatAltarGlow").Value;

            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly / 2f) * 0.6f;

            Vector2 globalPosition = p.ToWorldCoordinates(0f, 0f);
            Vector2 position = globalPosition + offScreen - Main.screenPosition + (new Vector2(50f, 74f) / 2f);
            Vector2 origin = texture.Size() / 2f;

            spriteBatch.Draw(texture, position + (Vector2.UnitX * 5 * scale), null, Color.White * 0.33f, 0f, origin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position + (Vector2.UnitX * -5 * scale), null, Color.White * 0.33f, 0f, origin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position + (Vector2.UnitY * 5 * scale), null, Color.White * 0.33f, 0f, origin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position + (Vector2.UnitY * -5 * scale), null, Color.White * 0.33f, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }

    public class LivingCombatAltarItem : ModItem
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/LivingCombatAltar";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("(Remove) Living Combat Altar");
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Orange;

            Item.createTile = ModContent.TileType<LivingCombatAltar>();
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

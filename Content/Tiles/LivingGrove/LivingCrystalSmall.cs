using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class LivingCrystalSmall1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;

            DustType = ModContent.DustType<LivingShard>();
            HitSound = SoundID.DD2_WitherBeastCrystalImpact;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(109, 225, 90));
        }
        public override bool KillSound(int i, int j, bool fail)
        {
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastHurt, new Vector2(i, j).ToWorldCoordinates());
                return false;
            }

            return base.KillSound(i, j, fail);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.025f;
            g = 0.1f;
            b = 0.045f;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];

            if (tile == null || !tile.HasTile) { return false; }

            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/GradientPillar").Value;

            Vector2 offScreen = new Vector2(Main.offScreenRange);
            Vector2 globalPosition = p.ToWorldCoordinates(0f, 0f);
            Vector2 position = globalPosition + offScreen - Main.screenPosition + new Vector2(0f, -100f + 16f);
            Color color = new Color(0.05f, 0.2f, 0.08f, 0f) * (2 * (((float)Math.Sin(Main.GameUpdateCount * 0.02f) + 4) / 4));

            Main.EntitySpriteDraw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Vector2 pos = new Vector2(i, j) * 16;
            Lighting.AddLight(pos, new Vector3(0.1f, 0.32f, 0.5f) * 0.35f);

            if (Main.rand.NextBool(50))
            {
                if (!Main.tile[i, j - 1].HasTile)
                {
                    Dust.NewDustPerfect(pos + new Vector2(Main.rand.NextFloat(0, 16), Main.rand.NextFloat(-32, -16)),
                        ModContent.DustType<LivingShardGlow>(), new Vector2(Main.rand.NextFloat(-0.02f, 0.02f), -Main.rand.NextFloat(0.1f, 0.36f)), 0, new Color(0.05f, 0.2f, 0.08f, 0f), Main.rand.NextFloat(0.25f, 0.5f));
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];

            if (tile == null || !tile.HasTile) { return; }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 offScreen = new Vector2(Main.offScreenRange);
            Vector2 globalPosition = p.ToWorldCoordinates(0f, 0f);
            Vector2 position = globalPosition + offScreen - Main.screenPosition;
            Color color = Color.White;

            Main.EntitySpriteDraw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }

    public class LivingCrystalSmall2 : LivingCrystalSmall1
    {

    }

    public class LivingCrystalSmall3 : LivingCrystalSmall1
    {

    }

    public class LivingCrystalTestItem3 : ModItem
    {
        public override string Texture => "Divergency/Assets/Textures/Star";

        public override void SetStaticDefaults()
        {
            //.setdefault("Living Crystal Test Item 3");
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Orange;

            Item.createTile = ModContent.TileType<LivingCrystalSmall1>();
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

    public class LivingCrystalTestItem4 : ModItem
    {
        public override string Texture => "Divergency/Assets/Textures/Star";

        public override void SetStaticDefaults()
        {
            //.setdefault("Living Crystal Test Item 4");
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Orange;

            Item.createTile = ModContent.TileType<LivingCrystalSmall2>();
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

    public class LivingCrystalTestItem5 : ModItem
    {
        public override string Texture => "Divergency/Assets/Textures/Star";

        public override void SetStaticDefaults()
        {
            //.setdefault("Living Crystal Test Item 5");
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Orange;

            Item.createTile = ModContent.TileType<LivingCrystalSmall3>();
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

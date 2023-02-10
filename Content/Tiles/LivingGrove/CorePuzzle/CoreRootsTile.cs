using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using Divergency.Assets.Particles;

namespace Divergency.Content.Tiles.LivingGrove.CorePuzzle
{
    public class CoreRootsTile : ModTile
    {
        private static bool ChangeTexture;
        private Vector2 zero = Vector2.Zero;
        private bool AlreadyDrawn;

        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            Main.tileLighted[Type] = false;
          
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorBottom = default(AnchorData);
            TileObjectData.newTile.AnchorTop = default(AnchorData);
            TileObjectData.newTile.AnchorWall = true;

            TileObjectData.addTile(Type);
            Main.tileBouncy[Type] = true;

            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
            DustType = 7;

        }
     
       

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            //Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeable.Furniture.MinionBossTrophy>());
        }
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CorePuzzle/CoreRootsTile").Value;
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                if (!Main.tileLighted[Type])
                {
                    spriteBatch.Draw(tex, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, Color.White);
                    AlreadyDrawn = true;
                }
                
            }

            return false;

        }

    }
    internal class CoreRoots : ModItem
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CorePuzzle/CoreRootsTile";

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
            Item.createTile = ModContent.TileType<CoreRootsTile>();
        }
    }
}

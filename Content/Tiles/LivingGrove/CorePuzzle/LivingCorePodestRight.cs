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
using Divergency.Content.Particles;

namespace Divergency.Content.Tiles.LivingGrove.CorePuzzle
{
    public class LivingCorePodestTileRight : ModTile
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CorePuzzle/LivingCorePodestRight";

        private static bool ChangeTexture;
        private Vector2 zero = Vector2.Zero;
        private bool AlreadyDrawn;

        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            Main.tileLighted[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.Width = 2; // unless it's already 2 tiles wide
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 2);

            TileObjectData.addTile(Type);
            Main.tileBouncy[Type] = true;

            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Podest"));
            DustType = 7;

        }
        public override bool RightClick(int i, int j)
        {
            Vector2 speed = new Vector2(10f, 0f);

            int left = i - Main.tile[i, j].TileFrameX / 18;
            int top = j - Main.tile[i, j].TileFrameY / 18;


            Vector2 pos = new Vector2(left * 16f + 32f, top * 16f + 8f);
            Player player = Main.LocalPlayer;
            if (!Main.tileLighted[Type])
            {
                if (player.GetModPlayer<CorePuzzle>().LivingCoreAmount != 0)
                {
                    player.GetModPlayer<CorePuzzle>().LivingCoreAmount--;

                    Projectile.NewProjectile(null, pos, speed, ModContent.ProjectileType<PodestProjectile>(), 0, 0);
                    Main.tileLighted[Type] = true;


                }
                else
                {

                }
            }
        

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                //NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, left, top);
            }


            //if (!ChangeTexture)
            //   ChangeTexture = true;
            //else
            //   ChangeTexture = false;


            return true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {

            if (Main.tileLighted[Type])
            {


                r = 1.45f;
                g = 2.55f;
                b = 0.94f;
            }
            else if (!Main.tileLighted[Type])
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }

        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            //Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeable.Furniture.MinionBossTrophy>());
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CorePuzzle/LivingCorePodestRight").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CorePuzzle/LivingCorePodestRightCharged").Value;
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                if (!Main.tileLighted[Type])
                {
                    spriteBatch.Draw(tex, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(0, 13), Color.White);
                    AlreadyDrawn = true;
                    int left = i - Main.tile[i, j].TileFrameX / 18;
                    int top = j - Main.tile[i, j].TileFrameY / 18;
                    Vector2 pos = new Vector2(left * 16f + 32f, top * 16f + 8f);

                    //Item.NewItem(null, pos, ModContent.ItemType<Pebble>());

                }
                else if (Main.tileLighted[Type])
                {
                    spriteBatch.Draw(tex2, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(0,9), Color.White);
                    AlreadyDrawn = true;

                }
            }

            return false;

        }

    }
    internal class LivingCorePodestRight : ModItem
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CorePuzzle/LivingCorePodestRightCharged";

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
            Item.createTile = ModContent.TileType<LivingCorePodestTileRight>();
        }
    }
}

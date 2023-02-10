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
    public class XORCoreTile : ModTile
    {

        private static bool ChangeTexture;
        private Vector2 zero = Vector2.Zero;
        private bool AlreadyDrawn;
        private int timer;
        private bool Shoot;

        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;

            Main.tileLighted[Type] = false;
            Main.tileBouncy[Type] = false;


            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);

            TileObjectData.newTile.AnchorBottom = default(AnchorData);
            TileObjectData.newTile.AnchorTop = default(AnchorData);
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 85, 60), Language.GetText("XOR Tile"));
            DustType = 7;

        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {

            if (ChangeTexture)
            {


                r = 1.45f;
                g = 2.55f;
                b = 0.94f;
            }
            else
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
            Texture2D tex = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CorePuzzle/XORCoreTile").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CorePuzzle/XORCoreTileCharged1").Value;
            Texture2D tex3 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CorePuzzle/XORCoreTileCharged2").Value;
            int left = i - Main.tile[i, j].TileFrameX / 18;
            int top = j - Main.tile[i, j].TileFrameY / 18;
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 pos = new Vector2(left * 16 + 16f, top * 16 + 16f);
            Vector2 speed = new Vector2(3, 0);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                if (!Main.tileLighted[Type])
                {
                    spriteBatch.Draw(tex, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero - new Vector2(12,0), Color.White);
                }
                else if (Main.tileLighted[Type] && !Main.tileBouncy[Type])
                {
                    spriteBatch.Draw(tex2, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero - new Vector2(12, 0), Color.White);
                    Shoot = true;
                    if (Shoot)
                    {
                  
                        for (int jo = 0; jo < 1; jo++)
                        {
                            Vector2 dir = Main.rand.NextVector2Unit() * 0.1f;


                            ParticleManager.NewParticle(pos, dir * 10, ParticleManager.NewInstance<BloomParticle2>(), new Color(0.50f, 2f, 0.5f, 0), 0.032f);



                        }
                        timer++;
                        if (timer == 60)
                        {
                            Projectile.NewProjectile(null, pos, speed, ModContent.ProjectileType<GateProjectile>(), 0, 0);
                            timer = 0;
                            Main.tileLighted[Type] = false;
                            Main.tileBouncy[Type] = false;
                            Shoot = false;
                        }
                    }
                }
                if (Main.tileBouncy[Type] && Main.tileLighted[Type])
                {
                    spriteBatch.Draw(tex3, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, Color.White);
                    Shoot = false;
                    timer = 0;
                }
            }

            return false;

        }

    }
    internal class XORCore : ModItem
    {
         public override string Texture => "Divergency/Content/Tiles/LivingGrove/CorePuzzle/XORCoreTileCharged1";

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
            Item.createTile = ModContent.TileType<XORCoreTile>();
        }
    }
}

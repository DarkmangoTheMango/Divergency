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
using Divergency.Common.Players;

namespace Divergency.Content.Tiles.LivingGrove.CorePuzzle
{
    public class CoreResetTileDrop2 : ModTile
    {
        private static bool ChangeTexture;
        private Vector2 zero = Vector2.Zero;
        private bool AlreadyDrawn;
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CorePuzzle/CoreResetTile";

        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            Main.tileLighted[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);

            TileObjectData.addTile(Type);
            Main.tileBouncy[Type] = false;

            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
            DustType = 7;

        }
        public override bool RightClick(int i, int j)
        {
            Vector2 pos = new Vector2(i * 16, j * 16);

            Vector2 speed = new Vector2(-10f, 0f);

     

            Main.tileLighted[ModContent.TileType<XORCoreTile>()] = false;
            Main.tileBouncy[ModContent.TileType<XORCoreTile>()] = false;

                
            Main.tileLighted[ModContent.TileType<ANDCoreTile>()] = false;
            Main.tileBouncy[ModContent.TileType<ANDCoreTile>()] = false;

            Main.tileLighted[ModContent.TileType<CoreRootsTile>()] = false;
            Main.tileLighted[ModContent.TileType<CoreRootsTile1>()] = false;

            Main.tileLighted[ModContent.TileType<CoreRootsTile2>()] = false;
            Main.tileLighted[ModContent.TileType<CoreDoublerDownLeftTile>()] = false;
            Main.tileLighted[ModContent.TileType<CoreDoublerLeftUpTile>()] = false;
            Main.tileLighted[ModContent.TileType<CoreDoublerRightDownTile>()] = false;
            Main.tileLighted[ModContent.TileType<CoreDoublerUpRightTile>()] = false;

            Main.tileLighted[ModContent.TileType<LivingCorePodestTileLeft>()] = false;
            Main.tileLighted[ModContent.TileType<LivingCorePodestTileRight>()] = false;
            Main.tileLighted[ModContent.TileType<LivingCorePodestTileUp>()] = false;

            Player player = Main.LocalPlayer;
            player.GetModPlayer<CorePuzzle>().LivingCoreAmount = 1;
            ParticleManager.NewParticle(player.Center, player.velocity * 3, ParticleManager.NewInstance<LivingCoreInsertParticle>(), Color.Purple, 1f);
            ParticleManager.NewParticle(player.Center, player.velocity * 3, ParticleManager.NewInstance<LivingCoreInsertParticle2>(), Color.Purple, 1f);
      
                ParticleManager.NewParticle(player.Center, player.velocity * 0, ParticleManager.NewInstance<ResetParticle>(), Color.Purple, 8f);


            CameraSystem.ScreenShake(20);




            SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Tiles/Reset")

            {
                Pitch = Main.rand.NextFloat(1f),
                Volume = 1f,
                MaxInstances = 1,

            });
            player.GetModPlayer<CorePuzzle>().LivingCoreAmount = 2;

            for (int jo = 0; jo < 1000; jo++)
            {
                Vector2 dir = Main.rand.NextVector2Unit() * 0.1f;

                ParticleManager.NewParticle(pos, dir * 400, ParticleManager.NewInstance<BloomParticle2>(), new Color(0.50f, 2f, 0.5f, 0), 0.1f);

            }
            for (int io = 0; io < Main.maxProjectiles; io++)
            {
                Projectile proj = Main.projectile[io];
                if (proj.type == ModContent.ProjectileType<PodestProjectile
                    >())
                {
                    proj.Kill();
                }
            }

            return true;
        }
      

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            //Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeable.Furniture.MinionBossTrophy>());
        }
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 20;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CorePuzzle/CoreResetTile").Value;
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                if (!Main.tileLighted[Type])
                {
                    spriteBatch.Draw(tex, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(0, 15), Color.White);
                    AlreadyDrawn = true;
                }
       
            }

            return false;

        }
        public int FindEmptySlot()
        {
            for (int i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++)
            {
                Item item = Main.player[Main.myPlayer].inventory[i];
                if (!item.active)
                {
                    return i;
                }
            }
            return -1;
        }
    }
    internal class CoreResetDrop2 : ModItem
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CorePuzzle/CoreResetTile";

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
            Item.createTile = ModContent.TileType<CoreResetTileDrop2>();
        }
    }

}

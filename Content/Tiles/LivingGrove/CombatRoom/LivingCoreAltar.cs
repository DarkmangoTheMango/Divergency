using Divergency.Common.Helpers;
using Divergency.Content.Events.LivingCore;
using Divergency.Content.Items.Weapons.LivingCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Divergency.Tiles.LivingTree
{
    public class LivingCoreAltarTile : ModTile
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1";

        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            Main.tileBouncy[Type] = true;

            Main.tileLighted[Type] = false;
            Main.tileAxe[Type] = false;
            Main.tileBrick[Type] = false;
            Main.tileHammer[Type] = false;
            Main.tileAlch[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 85, 60), Language.GetText("Living Core Altar"));
            DustType = 7;
        }

        public override bool RightClick(int i, int j)
        {
            Console.WriteLine("a");
            int left = i - Main.tile[i, j].TileFrameX / 18;
            int top = j - Main.tile[i, j].TileFrameY / 18;

            if (TileEntity.ByPosition.TryGetValue(new Point16(left, top), out TileEntity te) && te is LivingCoreAltarTileEntity altarEntity)
            {
                Console.WriteLine("b");
                Console.WriteLine(altarEntity.Waves.Count);
                if (altarEntity.Waves.Count > 0)
                    LivingCoreEvent.Begin(left, top, new LivingCoreRoom(altarEntity));
                else
                    LivingCoreEvent.Begin(left, top, new InfiniteRoom());

                // TODO: Netsync begin signal
            }

            return true;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1").Value;
            Color color = Lighting.GetColor(i,j);

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                if ((LivingCoreEvent.X != i && LivingCoreEvent.Y != j))
                {
                    spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X + 3, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(0, 15), color);
                    Main.tileHammer[Type] = false;
                }
            }

            return false;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Main.tile[i, j];

            int left = i - (tile.TileFrameX / 18);
            int top = j - (tile.TileFrameY / 18);

            ModContent.GetInstance<LivingCoreAltarTileEntity>().Place(left, top);
        }
    }

    public class LivingCoreAltarTileEntity : ModTileEntity
    {
        public List<Reward> Rewards = [];
        public List<bool> ClaimedRewards = [];

        public List<Wave> Waves = [];

        public string MusicPath = "Divergency/Assets/Sounds/Music/LivingGroveBattle1";

        public Vector2[] BlockingBlocks => [];

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];

            Console.WriteLine("ffff");
            Console.WriteLine(tile.TileType);
            return tile.HasTile && tile.TileType == ModContent.TileType<LivingCoreAltarTile>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
                return -1;
            }
            return Place(i, j);
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("MusicPath", MusicPath);
            tag.Add("ClaimedRewards", ClaimedRewards);

            tag.Add("Rewards", Rewards.Select(r => r.Save()).ToList());
            tag.Add("Waves", Waves.Select(w => w.Save()).ToList());
        }

        public override void LoadData(TagCompound tag)
        {
            MusicPath = tag.GetString("MusicPath");
            ClaimedRewards = tag.GetList<bool>("ClaimedRewards").ToList();

            if (tag.ContainsKey("Rewards"))
            {
                Rewards = tag.GetList<TagCompound>("Rewards").Select(Reward.Load).ToList();
            }

            if (tag.ContainsKey("Waves"))
            {
                Waves = tag.GetList<TagCompound>("Waves").Select(Wave.Load).ToList();
            }
        }
    }

    public class LivingCoreAltar : ModItem
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CombatRoom/LivingCoreAltar1";

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
            Item.createTile = ModContent.TileType<LivingCoreAltarTile>();
        }
    }
}

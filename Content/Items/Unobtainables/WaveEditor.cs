using Divergency.Content.Particles.ParticleSystems;
using Divergency.Content.UI;
using Divergency.Tiles.LivingTree;
using Humanizer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Unobtainables
{
    public class WaveEditor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 1; // 20?
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.autoReuse = false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer || Main.dedServ)
                return false;

            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];

            int left = Player.tileTargetX - (tile.TileFrameX / 18);
            int top = Player.tileTargetY - (tile.TileFrameY / 18);

            if (TileEntity.ByPosition.TryGetValue(new Point16(left, top), out TileEntity te) && te is LivingCoreAltarTileEntity altarEntity)
                ModContent.GetInstance<WaveEditorUI>().OpenMenu(altarEntity);

            return false;
        }
    }
}
using Divergency.Content.NPCs.Forest;
using Microsoft.Xna.Framework;
using System.Drawing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Common.Players
{
    public class TreePlayer : ModPlayer
    {
    
            public bool treeCheck;
            public int treeNear;
            public override void PreUpdate()
            {
                if (treeNear >= 1)
                {
                    treeNear--;
                }

                // Origin position, in tile format.
                int x = (int)(Player.position.X / 16);
                int y = (int)(Player.position.Y / 16);

                // Position being checked;



                int checkX = x;
                int checkY = y;
                if (WorldGen.InWorld(checkX, y) && Main.tile[checkX, checkY].TileType == TileID.Trees)
                {
                    Player.GetModPlayer<TreePlayer>().treeNear = 300;
                }

            }
            public override void ResetEffects()
            {
                treeCheck = false;
            }

        
    }
}
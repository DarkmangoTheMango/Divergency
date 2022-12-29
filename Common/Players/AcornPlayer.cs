using Divergency.Content.NPCs.Forest;
using Microsoft.Xna.Framework;
using System.Drawing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Common.Players
{
    public class AcornPlayer : ModPlayer
    {
        public bool checkTree = true;
        public int Spawned = 120;


        public bool TryFindTreeTop(Vector2 position, out Vector2 result)
        {

            if (Main.tile[(int)position.X / 16, (int)position.Y / 16].TileType == TileID.Trees)
            {
                // Origin position, in tile format.
                int x = (int)(position.X / 16);
                int y = (int)(position.Y / 16);

                // Position being checked;



                int checkX = x;
                int checkY = y;
                if (WorldGen.InWorld(checkX, y) && Main.tile[checkX, checkY].TileType == TileID.Trees)
                {
                    Player.GetModPlayer<TreePlayer>().treeNear = 600;
                }

                // Checking up to a maximum of 30 tiles.
                for (int b = 0; b < 30; b++)
                {
                    // If this position is in the world, and if the tile is a Tree tile.
                    if (WorldGen.InWorld(checkX, y) && Main.tile[checkX, checkY].TileType == TileID.Trees)
                    {
                        // Checking if the tile's frames are within the range of tile frames used for the invisible tree top tiles.
                        if (Main.tile[checkX, checkY].TileFrameX == 22 && Main.tile[checkX, checkY].TileFrameY >= 198)
                        {
                            //Dust.QuickBox(new Vector2(checkX * 16, checkY * 16), new Vector2((checkX * 16) + 16, (checkY * 16) + 16), 10, Color.Yellow, null);
                            result = new Vector2(checkX * 16, checkY * 16);
                            return true;
                        }
                        // Otherwise, its a success, since it's still a tree tile. Just not the one we're looking for.
                        //Dust.QuickBox(new Vector2(checkX * 16, checkY * 16), new Vector2((checkX * 16) + 16, (checkY * 16) + 16), 10, Color.Green, null);
                        checkY--;
                    }
                    else
                    {
                        // If the tile isn't what we're looking for and since we're only iterating upwards, logically this means its useless to continue.
                        //Dust.QuickDustLine(new Vector2(checkX * 16, checkY * 16), new Vector2((checkX * 16) + 16, (checkY * 16) + 16), 5f, Color.Red);
                        //Dust.QuickDustLine(new Vector2(checkX * 16, (checkY * 16) + 16), new Vector2((checkX * 16) + 16, checkY * 16), 5f, Color.Red);
                        break;
                    }
                }
            }

            result = default;
            return false;
        }


        public override void PreUpdate()
        {
            if (Spawned > 0)
            {
                Spawned--;
            }
            if (Main.rand.NextBool(15) && Main.dayTime && !Main.IsItAHappyWindyDay && Spawned == 0 && Player.ZoneForest && checkTree)
            {
                if (TryFindTreeTop(Player.Center, out Vector2 result))
                {
                    NPC.NewNPC(null, (int)(result.X + Main.rand.NextFloat(-32f, 33f)), (int)(result.Y + Main.rand.NextFloat(-64f, 1f)), ModContent.NPCType<Acrid>());
                    Spawned = 1200;
                }
            }
            else if (Main.rand.NextBool(10) && Main.dayTime && (Main.IsItAHappyWindyDay || Main.IsItStorming) && Spawned == 0 && checkTree)
            {
                if (TryFindTreeTop(Player.Center, out Vector2 result))
                {
                    NPC.NewNPC(null, (int)(result.X + Main.rand.NextFloat(-32f, 33f)), (int)(result.Y + Main.rand.NextFloat(-64f, 1f)), ModContent.NPCType<Acrid>());
                    Spawned = 1200;
                }
            }

        }
    }
}
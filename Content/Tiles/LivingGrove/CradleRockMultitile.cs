using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Content.Dusts;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using System;

namespace Divergency.Content.Tiles.LivingGrove
{
    public class LargeCradleRockTile1 : ModTile
    {

    
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 7;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(2, 1);
            TileObjectData.newTile.DrawYOffset = 10;
            TileObjectData.addTile(Type);
             DustType = ModContent.DustType<LivingShard>();
            HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            AddMapEntry(new Color(151, 107, 75), Language.GetText("MapObject.Let's rock"));
        }
 
    }
}
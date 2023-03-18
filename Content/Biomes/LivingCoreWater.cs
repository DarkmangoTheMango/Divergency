using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using Terraria.GameContent;
using ReLogic.Content;

namespace Divergency.Content.Biomes
{
    public class LivingCoreWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.GetInstance<LivingCoreWaterfall>().Slot;


        public override int GetSplashDust()
        {
            return DustID.GemEmerald;
        }
        public override int GetDropletGore()
        {
            return GoreID.WaterDripJungle;
        }
        Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);

     

        public override Color BiomeHairColor()
        {
            return Color.LimeGreen;
        }
        public override byte GetRainVariant()
        {
            return (byte)Main.rand.Next(3);
        }

        public override Asset<Texture2D> GetRainTexture()
        {
            return ModContent.Request<Texture2D>("DivergencyAssets/Textures/Empty");


        }
    }
}
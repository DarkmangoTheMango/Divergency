using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Divergency.Common.Systems
{
    public class TestingSS : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "test",
                    delegate
                    {
                        ShaderTest(Main.spriteBatch);

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        
        private static Texture2D Pixel = ModContent.Request<Texture2D>("Divergency/Assets/Textures/WhitePixel", AssetRequestMode.ImmediateLoad).Value;
        private static Effect myEffect = ModContent.Request<Effect>("Divergency/Content/Effects/Testing", AssetRequestMode.ImmediateLoad).Value;
        private void ShaderTest(SpriteBatch spriteBatch) // also updates
        {
            /*
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, myEffect, Main.UIScaleMatrix);

            Rectangle rec = new Rectangle(Main.screenWidth / 4, Main.screenHeight / 4, Main.screenWidth / 2, Main.screenHeight / 2);
            spriteBatch.Draw(Pixel, rec, Color.Gray);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
            */
        }
    }
}
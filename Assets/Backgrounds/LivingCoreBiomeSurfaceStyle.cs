
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.GameContent;
using Divergency.Common.Helpers;
using Divergency.Content.NPCs.LivingGrove;

namespace Divergency.Assets.Backgrounds
{
    public class LivingCoreBiomeSurfaceStyle : ModSurfaceBackgroundStyle
    {
        public int flashIntensity;
        // Use this to keep far Backgrounds like the mountains.
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public override int ChooseFarTexture()
        {
            return -1;
        }

        public override int ChooseMiddleTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceGlowMid");
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceGlowClose");
        }
        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            flashIntensity = player.GetModPlayer<FlashPlayer>().intensity;
             float a = 2800f;
            float b = 1750f;
            int[] textureSlots = new int[] {
            BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceFar"),
            BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceMid"),
            BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceClose"),
        };

            float[] bgParallax = new float[] {
            0.15f, // Far
            0.2f, // Mid
            0.37f, // Close
                 };
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceGlowMid").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceGlowClose").Value;


            int length = textureSlots.Length;
            for (int i = 0; i < textureSlots.Length; i++)
            {
                //float bgParallax = 0.37f + 0.2f - (0.1f * (length - i));
                int textureSlot = textureSlots[i];
                Main.instance.LoadBackground(textureSlot);
                float bgScale = 1.4f;
                int bgW = (int)(Main.backgroundWidth[textureSlot] * bgScale);
                SkyManager.Instance.DrawToDepth(spriteBatch, 1f / bgParallax[i]);
                float screenOff = typeof(Main).GetFieldValue<float>("screenOff", Main.instance);
                float scAdj = typeof(Main).GetFieldValue<float>("scAdj", Main.instance);
                int bgStart = (int)(-Math.IEEERemainder(Main.screenPosition.X * bgParallax[i], bgW) - (bgW / 2));
                int bgStartMid = (int)(-Math.IEEERemainder(Main.screenPosition.X * 0.2f, bgW) - (bgW / 2));
                int bgStartClose = (int)(-Math.IEEERemainder(Main.screenPosition.X * 0.37f, bgW) - (bgW / 2));

                int bgTop = (int)((-Main.screenPosition.Y + screenOff / 2f) / (Main.worldSurface * 16.0) * a + b) + (int)scAdj - (length * 150);
                if (Main.gameMenu)
                {
                    bgTop = 320;
                }
                Color backColor = typeof(Main).GetFieldValue<Color>("ColorOfSurfaceBackgroundsModified", Main.instance);
                int bgLoops = Main.screenWidth / bgW + 2;
                if (Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
                {

                    for (int k = 0; k < bgLoops; k++)
                    {
                        spriteBatch.Draw(texture,
                            new Vector2(bgStartMid + bgW * k, MathHelper.Clamp(bgTop, -100, 0)),
                            new Rectangle(0, 0, Main.backgroundWidth[textureSlot], Main.backgroundHeight[textureSlot]),
                            Color.White, 0f, default, bgScale, SpriteEffects.None, 0f);

                    }
                   
                    for (int k = 0; k < bgLoops; k++)
                    {
                        spriteBatch.Draw(TextureAssets.Background[textureSlot].Value,
                            new Vector2(bgStart + bgW * k, MathHelper.Clamp(bgTop, -100, 0)),
                            new Rectangle(0, 0, Main.backgroundWidth[textureSlot], Main.backgroundHeight[textureSlot]),
                            backColor * flashIntensity, 0f, default, bgScale, SpriteEffects.None, 0f);;

                    }
                    for (int k = 0; k < bgLoops; k++)
                    {
                        spriteBatch.Draw(texture2,
                            new Vector2(bgStartClose + bgW * k, MathHelper.Clamp(bgTop, -100, 0)),
                            new Rectangle(0, 0, Main.backgroundWidth[textureSlot], Main.backgroundHeight[textureSlot]),
                            Color.White, 0f, default, bgScale, SpriteEffects.None, 0f);

                    }

                }

            }
            return false;
        }
    }

}





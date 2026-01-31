using Divergency.Common.Helpers;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

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
            float a = 2800f;
            float b = 1750f;

            int[] textureSlots = new int[] {
        BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceFar"),
        BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceMid"),
        BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceClose"),
    };

            float[] bgParallax = new float[] { 0.15f, 0.2f, 0.37f };

            // Glow overlays
            Texture2D glowMid = ModContent.Request<Texture2D>("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceGlowMid").Value;
            Texture2D glowClose = ModContent.Request<Texture2D>("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceGlowClose").Value;

            int length = textureSlots.Length;
            for (int i = 0; i < length; i++)
            {

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

                int bgTop = (int)((-Main.screenPosition.Y + screenOff / 2f) / (Main.worldSurface * 16.0) * a + b)
                            + (int)scAdj - (length * 150);

                if (Main.gameMenu)
                    bgTop = 320;

                Color backColor = typeof(Main).GetFieldValue<Color>("ColorOfSurfaceBackgroundsModified", Main.instance);

                int bgLoops = Main.screenWidth / bgW + 2;

                if (Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
                {
                    for (int k = 0; k < bgLoops; k++)
                    {
                        Vector2 pos = new Vector2(bgStart + bgW * k, MathHelper.Clamp(bgTop, -100, 0));

                        Texture2D baseTex = TextureAssets.Background[textureSlot].Value;
                        Rectangle frame = new Rectangle(0, 0, Main.backgroundWidth[textureSlot], Main.backgroundHeight[textureSlot]);

                        spriteBatch.Draw(baseTex, pos, frame, backColor, 0f, default, bgScale, SpriteEffects.None, 0f);

                        Texture2D glowTex = null;

                        if (i == 1)
                            glowTex = glowMid;
                        if (i == 2)
                            glowTex = glowClose;

                        if (glowTex != null)
                            spriteBatch.Draw(glowTex, pos, frame, Color.White * 1f, 0f, default, bgScale, SpriteEffects.None, 0f);
                    }

                }
            }

            return false;
        }
    }
}





/*using Divergency.Common.Helpers;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Divergency.Assets.Backgrounds
{
    public class LivingCoreBiomeSurfaceStyle : ModSurfaceBackgroundStyle
    {
        public class Cinder
        {
            public int Time;
            public int Lifetime;
            public int IdentityIndex;
            public float Scale;
            public float Depth;
            public float Rotation;
            public Color DrawColor;
            public Vector2 Velocity;
            public Vector2 Center;

            public Cinder(int lifetime, int identity, float depth, float rotation, Color color, Vector2 startingPosition, Vector2 startingVelocity)
            {
                Lifetime = lifetime;
                IdentityIndex = identity;
                Depth = depth;
                Rotation = rotation;
                DrawColor = color;
                Center = startingPosition;
                Velocity = startingVelocity;
            }
        }

        public static List<Cinder> Cinders
        {
            get;
            internal set;
        } = new();

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
            if (Main.rand.NextBool(4))
            {
                int lifetime = Main.rand.Next(200, 300);
                float depth = Main.rand.NextFloat(1.8f, 5f);
                Vector2 startingPosition = new Vector2(Main.screenWidth * Main.rand.NextFloat(-0.1f, 1.1f), Main.screenHeight * 1.05f);
                Vector2 startingVelocity = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.9f, 0.9f)) * 4f;
                Color cinderColor = new Color(109, 223, 94, 0);
                Cinders.Add(new Cinder(lifetime, Cinders.Count, depth, Main.rand.NextFloat(MathHelper.TwoPi), cinderColor, startingPosition, startingVelocity));
            }

            for (int i = 0; i < Cinders.Count; i++)
            {
                Cinders[i].Scale = Utils.GetLerpValue(Cinders[i].Lifetime, Cinders[i].Lifetime / 3, Cinders[i].Time, true);
                Cinders[i].Scale *= MathHelper.Lerp(1f, 1.2f, Cinders[i].IdentityIndex % 6f / 6f);
                if (Cinders[i].IdentityIndex % 13 == 12)
                    Cinders[i].Scale *= 2f;

                float flySpeed = MathHelper.Lerp(3.2f, 14f, Cinders[i].IdentityIndex % 21f / 21f);

                float wiggleX = (float)Math.Sin(Main.timeForVisualEffects * 0.01f + Cinders[i].Depth) * 1.5f;

                Cinders[i].Velocity = new Vector2(wiggleX, -2f);

                Cinders[i].Time++;
                Cinders[i].Rotation += Cinders[i].Velocity.X * 0.01f;
                Cinders[i].Center += Cinders[i].Velocity;
            }

            Cinders.RemoveAll(c => c.Time >= c.Lifetime);

            //break

            Player player = Main.LocalPlayer;
            float a = 2800f;
            float b = 1750f;

            int[] textureSlots = new int[] {
        BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceFar"),
        BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceMid"),
        BackgroundTextureLoader.GetBackgroundSlot("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceClose"),
    };

            float[] bgParallax = new float[] { 0.15f, 0.2f, 0.37f };

            // Glow overlays
            Texture2D glowMid = ModContent.Request<Texture2D>("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceGlowMid").Value;
            Texture2D glowClose = ModContent.Request<Texture2D>("Divergency/Assets/Backgrounds/LivingCoreBiomeSurfaceGlowClose").Value;

            int length = textureSlots.Length;
            for (int i = 0; i < length; i++)
            {

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

                int bgTop = (int)((-Main.screenPosition.Y + screenOff / 2f) / (Main.worldSurface * 16.0) * a + b)
                            + (int)scAdj - (length * 150);

                if (Main.gameMenu)
                    bgTop = 320;

                Color backColor = typeof(Main).GetFieldValue<Color>("ColorOfSurfaceBackgroundsModified", Main.instance);

                int bgLoops = Main.screenWidth / bgW + 2;

                if (Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
                {
                    for (int k = 0; k < bgLoops; k++)
                    {
                        Vector2 pos = new Vector2(bgStart + bgW * k, MathHelper.Clamp(bgTop, -100, 0));

                        // 1. Draw the base background first (always visible)
                        spriteBatch.Draw(TextureAssets.Background[textureSlot].Value,
                            pos,
                            new Rectangle(0, 0, Main.backgroundWidth[textureSlot], Main.backgroundHeight[textureSlot]),
                            Color.White, 0f, default, bgScale, SpriteEffects.None, 0f);
                    }
                }

                if (i == 1)
                {
                    // Draw cinders.
                    Texture2D cinderTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/MicroBloom").Value;
                    for (int k = 0; k < Cinders.Count; k++)
                    {
                        Vector2 drawPosition = Cinders[k].Center;
                        spriteBatch.Draw(cinderTexture, drawPosition, null, Cinders[k].DrawColor with { A = 0 } * 0.5f, Cinders[k].Rotation, cinderTexture.Size() * 0.5f, Cinders[k].Scale, 0, 0f);
                        spriteBatch.Draw(cinderTexture, drawPosition, null, Color.White with { A = 0 }, Cinders[k].Rotation, cinderTexture.Size() * 0.5f, Cinders[k].Scale * 0.5f, 0, 0f);
                    }
                }
            }

            return false;
        }
    }
}




*/
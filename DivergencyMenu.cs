using Divergency.Assets.Backgrounds;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace Divergency;

public class DivergencyMenu : ModMenu
{
    public override Asset<Texture2D> Logo => Mod.Assets.Request<Texture2D>("Assets/Textures/Title");

    public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<LivingCoreBiomeSurfaceStyle>();

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/MainMenu");

    public override void Update(bool isOnTitleScreen)
    {
        Main.time = 27000;
        Main.dayTime = true;
    }

    public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
    {
        drawColor = new Color(255, 255, 255);

        Texture2D glowTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Title_Glow").Value;

        spriteBatch.Draw(glowTexture, logoDrawCenter, glowTexture.Bounds, drawColor with { A = 0 } * 0.8f, logoRotation, glowTexture.Size() * 0.5f, logoScale, SpriteEffects.None, 1);

        Texture2D logoTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Title").Value;

        float backInterpolant = Main.GlobalTimeWrappedHourly * 0.5f % 1f;
        float backScale = logoScale * MathHelper.Lerp(1f, 1.1f, backInterpolant);

        spriteBatch.Draw(logoTexture, logoDrawCenter, logoTexture.Bounds, drawColor * (float)Math.Pow(1f - backInterpolant, 0.46f) * 0.5f, logoRotation, logoTexture.Size() * 0.5f, backScale, SpriteEffects.None, 1);

        return base.PreDrawLogo(spriteBatch, ref logoDrawCenter, ref logoRotation, ref logoScale, ref drawColor);
    }
}

public class StormMenu : ModMenu
{
    public class Particle(Vector2 position, Vector2 velocity, int lifetime)
    {
        public int TimeLeft;
        public int Lifetime = lifetime;
        public int ID = Particles.Count;
        public Vector2 Velocity = velocity;
        public Vector2 Position = position;
    }

    public static List<Particle> Particles
    {
        get;
        internal set;
    } = [];

    public override string DisplayName => "Bury The Light";

    public override int Music => MusicID.OtherworldlyNight;

    public override void Update(bool isOnTitleScreen)
    {
        Main.time = 27000;
        Main.dayTime = true;

        for (int k = 0; k < 20; k++)
            Particles.Add(new Particle(new Vector2(Main.screenWidth * Main.rand.NextFloat(-1, 1), -Main.screenHeight), Vector2.UnitY.RotatedBy(-0.1f) * 200, 100));

        UpdateParticles();
    }

    private static void UpdateParticles()
    {
        for (int k = 0; k < Particles.Count; k++)
        {
            var particle = Particles[k];

            particle.TimeLeft++;
            particle.Position += particle.Velocity;
        }

        Particles.RemoveAll(p => p.TimeLeft >= p.Lifetime);
    }

    public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/TitleAlt").Value;
        Texture2D glowTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/TitleAlt_Glow").Value;

        DrawBackground(spriteBatch);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

        spriteBatch.Draw(glowTexture, logoDrawCenter, glowTexture.Bounds, Color.White with { A = 0 } * 0.8f, logoRotation, glowTexture.Size() * 0.5f, logoScale, SpriteEffects.None, 1);
        spriteBatch.Draw(texture, logoDrawCenter + Main.rand.NextVector2Circular(1, 1) * 4, texture.Bounds, Color.White * 0.5f, logoRotation, texture.Size() * 0.5f, logoScale, SpriteEffects.None, 1);
        spriteBatch.Draw(texture, logoDrawCenter, texture.Bounds, Color.White, logoRotation, texture.Size() * 0.5f, logoScale, SpriteEffects.None, 1);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

        DrawParticles(spriteBatch);

        return false;
    }

    private static void DrawBackground(SpriteBatch spriteBatch)
    {

        Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Noise/CloudyNoise").Value;

        Effect shader = Divergency.Storm.Value;

        Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Noise/TurbulentNoise").Value;

        shader.Parameters["uTime"]?.SetValue((float)Main.timeForVisualEffects * 0.0005f);
        shader.Parameters["alpha"]?.SetValue(1);
        shader.CurrentTechnique.Passes[0].Apply();

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, shader, Main.UIScaleMatrix);

        spriteBatch.Draw(texture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
    }

    private static void DrawParticles(SpriteBatch spriteBatch)
    {
        Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Beam").Value;

        for (int k = 0; k < Particles.Count; k++)
        {
            var particle = Particles[k];

            spriteBatch.Draw(texture, particle.Position, texture.Bounds, new Color(90, 150, 255, 0) * 0.1f, particle.Velocity.ToRotation() + MathHelper.PiOver2, texture.Size() * 0.5f, new Vector2(0.4f, 10), SpriteEffects.None, 0f);
        }
    }
}
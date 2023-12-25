using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Examples;
using ParticleLibrary.Utilities;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Particles.ParticleSystems;

//
// Summary:
//     This class demonstrates a good way to manage your GPU particle systems. It's
//     a good idea to centralize your GPU particle systems, as you'll want to reuse
//     them as much as possible.
public class ParticleSystemManager : ModSystem
{
    public static QuadParticleSystem ExampleQuadSystem { get; private set; }
    public static QuadParticleSystemSettings ExampleQuadSettings { get; private set; }
    public static QuadParticle CoreGroveVisualBackGround { get; private set; } //
    public static QuadParticle CoreGroveVisualForeground { get; private set; } // core biome visuals
    public static PointParticleSystem ExamplePointSystem { get; private set; }
    public static PointParticleSystemSettings ExamplePointSettings { get; private set; }
    public static PointParticle ExamplePointParticle { get; private set; }

    public static ParticleSystemWrapper<QuadParticle> ExampleWrappedQuadParticleSystem { get; private set; }

    public override void OnModLoad()
    {
        if (Main.netMode is NetmodeID.Server)
        {
            return;
        }

        // Demonstrates creating a Quad particle system.
        ExampleQuadSettings = new(ModContent.Request<Texture2D>("ParticleLibrary/Assets/Textures/Star", AssetRequestMode.ImmediateLoad).Value, 3000, 600, Layer.BeforeInterface, blendState: BlendState.AlphaBlend);
        ExampleQuadSystem = new QuadParticleSystem(ExampleQuadSettings);
        CoreGroveVisualBackGround = new()
        {
            StartColor = Color.Transparent.WithAlpha(0f),
            EndColor = Color.LimeGreen.WithAlpha(0f),
            Scale = new Vector2(Main.rand.NextFloat(0.5f, 1.5f)),
            Rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi + float.Epsilon),
            RotationVelocity = Main.rand.NextFloat(-0.1f, 0.1f + float.Epsilon),
            Depth = 1f + Main.rand.NextFloat(-0.2f, 0.2f + float.Epsilon),
            DepthVelocity = Main.rand.NextFloat(-0.002f, 0.002f + float.Epsilon)


        };


        ExampleQuadSettings = new(ModContent.Request<Texture2D>("ParticleLibrary/Assets/Textures/Star", AssetRequestMode.ImmediateLoad).Value, 3000, 600, Layer.BeforeInterface, blendState: BlendState.AlphaBlend);
        ExampleQuadSystem = new QuadParticleSystem(ExampleQuadSettings);
        CoreGroveVisualForeground = new()
        {
            StartColor = Color.Transparent.WithAlpha(0f),
            EndColor = Color.LimeGreen.WithAlpha(0f),
            Scale = new Vector2(Main.rand.NextFloat(0.5f, 1.5f)),
            Rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi + float.Epsilon),
            RotationVelocity = Main.rand.NextFloat(-0.1f, 0.1f + float.Epsilon),
            Depth = 1f + Main.rand.NextFloat(-0.1f, 0.2f + float.Epsilon),
            DepthVelocity = Main.rand.NextFloat(-0.001f, 0.001f + float.Epsilon)


        };

        // Demonstrates creating a Point particle system.
        ExamplePointSettings = new(500, 300);
        ExamplePointSystem = new PointParticleSystem(ExamplePointSettings);
        ExamplePointParticle = new()
        {
            StartColor = Color.White.WithAlpha(0f),
            EndColor = Color.Black.WithAlpha(0f),
            Depth = 1f + Main.rand.NextFloat(-0.1f, 0.1f + float.Epsilon),
            DepthVelocity = Main.rand.NextFloat(-0.001f, 0.001f - float.Epsilon)
        };

        // Demonstrates creating a wrapped particle system from a Quad particle system.
        // This can be useful for implementing custom functionality, such as embedding your system into a RenderTarget2D.
        ExampleWrappedQuadParticleSystem = new(ExampleQuadSystem, ExampleQuadSettings);
    }

    public override void Unload()
    {
        // Always make sure to dispose GPU particle systems when you're done with them!
        ExampleQuadSystem?.Dispose();
        ExampleQuadSystem = null;
        ExampleQuadSettings = null;
        CoreGroveVisualBackGround = null;

        ExamplePointSystem?.Dispose();
        ExamplePointSystem = null;
        ExamplePointSettings = null;
        ExamplePointParticle = null;

        ExampleWrappedQuadParticleSystem = null;
    }

    public class ParticleSystemWrapper<T>
        where T : GPUParticle
    {
        public IGPUParticleSystem<T> System { get; }
        public GPUParticleSystemSettings Settings { get; }

        public ParticleSystemWrapper(IGPUParticleSystem<T> system, GPUParticleSystemSettings settings)
        {
            System = system;
            Settings = settings;
        }

        /// <summary>
        /// A shorthand for accessing the <see cref="IGPUParticleSystem{T}.NewParticle(Vector2, Vector2, T, int?)"/> method.
        /// </summary>
        /// <param name="position">The location of the particle.</param>
        /// <param name="velocity">The velocity of the particle.</param>
        /// <param name="particle">The particle settings.</param>
        public void AddParticle(Vector2 position, Vector2 velocity, T particle)
        {
            System.NewParticle(position, velocity, particle);
        }
    }
}


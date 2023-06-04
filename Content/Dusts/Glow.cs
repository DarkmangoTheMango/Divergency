using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Dusts
{
    public class Glow : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 64, 64);

			dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Divergency.Instance.Assets.Request<Effect>("Content/Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
		}

		public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

		public override bool Update(Dust dust)
		{
			if (dust.customData is null)
			{
				dust.position -= Vector2.One * 32 * dust.scale;
				dust.customData = true;
			}

			Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

			dust.scale *= 0.95f;
			Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

			dust.rotation += 0.06f;
			dust.position += currentCenter - nextCenter;

			dust.shader.UseColor(dust.color);

			dust.position += dust.velocity;

			if (!dust.noGravity)
				dust.velocity.Y += 0.1f;

			dust.velocity *= 0.99f;
			dust.color *= 0.95f;

			if (!dust.noLight)
				Lighting.AddLight(dust.position, dust.color.ToVector3());

			if (dust.scale < 0.05f)
				dust.active = false;

			return false;
		}
	}
    public class CodeDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 64, 64);

            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Divergency.Instance.Assets.Request<Effect>("Content/Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= Vector2.One * 32 * dust.scale;
                dust.customData = true;
            }

            Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

            dust.scale *= 0.96f;
            Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

            dust.position += currentCenter - nextCenter;

            dust.shader.UseColor(dust.color);

            dust.position += dust.velocity;

            if (!dust.noGravity)
                dust.velocity.Y += 0.1f;

            dust.velocity *= 0.98f;
            dust.color *= 0.95f;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3());

            if (dust.scale < 0.05f)
                dust.active = false;

            return false;
        }
    }
    public class CodeDust1 : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 64, 64);

            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Divergency.Instance.Assets.Request<Effect>("Content/Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= Vector2.One * 32 * dust.scale;
                dust.customData = true;
            }

            Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

            dust.scale *= 0.96f;
            Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

            dust.position += currentCenter - nextCenter;

            dust.shader.UseColor(dust.color);

            dust.position += dust.velocity;

            if (!dust.noGravity)
                dust.velocity.Y += 0.1f;

            dust.velocity *= 0.98f;
            dust.color *= 0.95f;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3());

            if (dust.scale < 0.05f)
                dust.active = false;

            return false;
        }

    }

    public class GlowLine : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 128, 64);

            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Divergency.Instance.Assets.Request<Effect>("Content/Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= Vector2.One * 32 * dust.scale;
                dust.customData = true;
            }

            Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

            dust.scale *= 0.9f;
            Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

            dust.rotation = dust.velocity.ToRotation();
            dust.position += currentCenter - nextCenter;

            dust.shader.UseColor(dust.color);

            dust.position += dust.velocity;

            if (!dust.noGravity)
                dust.velocity.Y += 0.1f;

            dust.velocity *= 0.9f;
            dust.color *= 0.95f;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3());

            if (dust.scale < 0.04f)
                dust.active = false;

            return false;
        }
    }
}

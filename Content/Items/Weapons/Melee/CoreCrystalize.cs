using Divergency.Common.Helpers;
using System;
using System.Collections.Generic;
using Terraria.UI.Chat;

namespace Divergency.Content.Items.Weapons.Melee;

public class CoreCrystalize : ModItem
{
    private float SwingDir
    {
        get;
        set;
    } = 1;

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
        private set;
    } = [];

    public override bool MeleePrefix() => true;

    public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.Size = new(176);
        Item.scale = 1;

        Item.DamageType = DamageClass.Melee;
        Item.noMelee = true;
        Item.damage = 120;
        Item.knockBack = 5;

        Item.shoot = ModContent.ProjectileType<CoreCrystalizePro>();
        Item.shootSpeed = 1;

        Item.autoReuse = true;
        Item.noUseGraphic = true;
        Item.useTime = Item.useAnimation = 50;
        Item.useStyle = ItemUseStyleID.Shoot;

        Item.value = Item.sellPrice(0, 5, 0, 0);
        Item.rare = ItemRarityID.Green;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SwingDir = -SwingDir;

        Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, SwingDir);

        return false;
    }

    public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
    {
        for (int k = 0; k < Particles.Count; k++)
        {
            var particle = Particles[k];

            particle.TimeLeft++;
            particle.Position += particle.Velocity;
        }

        Particles.RemoveAll(p => p.TimeLeft >= p.Lifetime);

        if (line.Name == "ItemName" && line.Mod == "Terraria")
        {

            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Beam").Value;

            Vector2 position = new(line.X, line.Y);

            if (Main.rand.NextBool(30))
                Particles.Add(new Particle(new Vector2(62 + Main.rand.NextFloat(-60, 60), 11f), -Vector2.UnitY * 0.01f, 1400));

            Color darkColor = new(96, 214, 72);
            Color lightColor = new(96, 214, 72);

            float t = Main.GlobalTimeWrappedHourly * 0.8f % 1f;

            Vector2 pulsePosition = position - new Vector2(12.5f, 1.5f) * t;
            Vector2 scale = line.BaseScale * MathHelper.Lerp(1f, 1.2f, t);

            Color pulseColor = new Color(191, 255, 119, 0) * (1f - t);

            DrawParticles(darkColor, lightColor, position);

            Main.spriteBatch.Draw(texture, position + new Vector2(62, 11), texture.Bounds, darkColor with { A = 0 }, MathHelper.PiOver2, texture.Size() * 0.5f, new Vector2(1f, 3.5f), SpriteEffects.None, 0);

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, position, darkColor, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, position, darkColor with { A = 0 }, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, pulsePosition, pulseColor, line.Rotation, line.Origin, scale, line.MaxWidth, line.Spread);

            return false;
        }

        return true;
    }

    private static void DrawParticles(Color darkColor, Color lightColor, Vector2 position)
    {
        Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Dust").Value;

        for (int k = 0; k < Particles.Count; k++)
        {
            var particle = Particles[k];

            float t = 1f - (float)Particles[k].TimeLeft / Particles[k].Lifetime;
            float scale = MathF.Sin(EaseFunction.EaseQuadInOut.Ease(t) * MathF.PI);
            float alpha = MathF.Sin(EaseFunction.EaseQuadInOut.Ease(t));

            Main.spriteBatch.Draw(texture, particle.Position + position, texture.Bounds, darkColor with { A = 0 } * alpha, Particles[k].ID, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, particle.Position + position, texture.Bounds, lightColor with { A = 0 } * alpha, Particles[k].ID, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }
}
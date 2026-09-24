using Terraria.UI.Chat;

namespace Divergency.Content.Items.Weapons.Melee;

public class CoreCrystalize : ModItem
{
    private float SwingDir
    {
        get;
        set;
    } = 1;

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
        if (line.Name == "ItemName" && line.Mod == "Terraria")
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Beam").Value;

            Vector2 position = new(line.X, line.Y);
            Color baseColor = new(96, 214, 72);

            float t = Main.GlobalTimeWrappedHourly * 0.5f % 1f;
            float pulseScale = MathHelper.Lerp(1f, 1.2f, t);

            Color pulseColor = baseColor with { A = 0 } * (1 - t);

            Vector2 textSize = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);

            Vector2 centeredOrigin = textSize * 0.5f;

            Vector2 centeredPos = position + centeredOrigin;

            Main.spriteBatch.Draw(texture, centeredPos - new Vector2(0, 5), texture.Bounds, baseColor with { A = 0 }, MathHelper.PiOver2, texture.Size() * 0.5f, new Vector2(1, textSize.X * 0.025f), SpriteEffects.None, 0);

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, position, baseColor, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, position, baseColor with { A = 0 }, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, centeredPos, pulseColor, line.Rotation, centeredOrigin, line.BaseScale * pulseScale, line.MaxWidth, line.Spread);

            return false;
        }

        return true;
    }
}
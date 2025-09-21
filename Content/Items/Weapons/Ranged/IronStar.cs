using Divergency.Common.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Divergency.Content.Items.Weapons.Ranged;

public class IronStar : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Ranged;
        Item.damage = 40;
        Item.crit = 3;
        Item.knockBack = 4f;
        Item.noMelee = true;

        Item.shoot = ModContent.ProjectileType<IronStarPro>();
        Item.shootSpeed = 10f;

        Item.Size = new(16);

        Item.useTime = Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noUseGraphic = true;
        Item.autoReuse = true;

        Item.value = Item.sellPrice(0, 5, 0, 0);
        Item.rare = ItemRarityID.Green;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return true;
    }
}

public class IronStarPro : ModProjectile
{
    public override string Texture => "Divergency/Content/Items/Weapons/Ranged/IronStar";

    Player player => Main.player[Projectile.owner];

    float maxTimeLeft;

    public override bool ShouldUpdatePosition() => false;

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.friendly = true;

        Projectile.Size = Vector2.Zero;

        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;

        Projectile.aiStyle = -1;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.timeLeft = player.HeldItem.useAnimation;
        maxTimeLeft = Projectile.timeLeft;

        AI();
    }

    public override void AI()
    {
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

        Projectile.Center = player.Center;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(-0.6f * player.direction, 0f, EaseFunction.EaseQuinticOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));

        player.heldProj = Projectile.whoAmI;
    }

    float rot;

    public override bool PreDraw(ref Color lightColor)
    {
        rot += 0.1f;
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
        Vector2 origin = sourceRectangle.Size() / 2f;
        Vector2 drawPosition = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2) + new Vector2(15, -2 * player.direction).RotatedBy(Projectile.rotation) - Main.screenPosition;

        SpriteEffects drawFlipped = player.direction == -1 ? SpriteEffects.FlipVertically : 0;

        Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, lightColor, Projectile.rotation + rot, origin, Projectile.scale, drawFlipped, 0f);

        return false;
    }
}
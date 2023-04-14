using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Divergency.Content.Projectiles.Ranged.Doors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Ranged
{
    public class DoorLauncher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Door Launcher");
            Tooltip.SetDefault("Uses doors as ammo");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 40;
            Item.crit = 3;
            Item.knockBack = 4f;
            Item.noMelee = true;

            Item.useAmmo = ItemID.WoodenDoor;
            Item.shoot = 1;
            Item.shootSpeed = 10f;

            Item.Size = new Vector2(58, 30);
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void HoldItem(Player player)
        {
            if (player == Main.LocalPlayer)
            {
                if (player.ItemAnimationActive) { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation - MathHelper.PiOver2 * player.direction); }
                else { player.SetCompositeArmFront(false, default, default); }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rotation = velocity.ToRotation();

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity = 3f;

            Vector2 offset = new Vector2(0.8f, 0).RotatedBy(rotation);

            for (int k = 0; k < 15; k++)
            {
                Vector2 direction = offset.RotatedByRandom(0.4f);
                Dust.NewDustPerfect(position + offset * 70, ModContent.DustType<Glow>(), direction * Main.rand.NextFloat(8), 125, new Color(255, 108, 23), Main.rand.NextFloat(0.2f, 0.5f));
            }

            SoundEngine.PlaySound(SoundID.Item61, player.Center);

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DoorLauncherPro>(), 0, 0f, player.whoAmI);

            return false;
        }
    }

    public class DoorLauncherPro : ModProjectile
    {
        bool initialize = true;

        float maxTimeLeft;

        float widthMod = 1;
        
        float heightMod = 1;

        public override string Texture => "Divergency/Content/Items/Weapons/Ranged/DoorLauncher";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Door Launcher");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = Vector2.Zero;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            if (initialize)
            {
                Projectile.timeLeft = player.HeldItem.useAnimation;
                maxTimeLeft = Projectile.timeLeft;
                Projectile.netUpdate = true;
                initialize = false;
            }

            Projectile.Center = player.Center;
            Projectile.rotation = Projectile.velocity.ToRotation();

            widthMod = MathHelper.Lerp(0.75f, 1, EaseFunction.EaseQuinticInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
            heightMod = MathHelper.Lerp(1.25f, 1, EaseFunction.EaseQuinticInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));

            player.heldProj = Projectile.whoAmI;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = new Vector2(0, sourceRectangle.Size().Y / 2);

            Vector2 drawPosition = player.Center + Projectile.rotation.ToRotationVector2() * -20f - Main.screenPosition;

            SpriteEffects drawFlipped = player.direction == -1 ? SpriteEffects.FlipVertically : 0;

            Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, lightColor, Projectile.rotation, origin, new Vector2(widthMod, heightMod), drawFlipped, 0f);

            return false;
        }
    }
}

 
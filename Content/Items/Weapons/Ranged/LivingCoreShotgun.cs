using Divergency.Content.Dusts;
using Divergency.Content.Projectiles.Ranged;
using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Divergency.Common.Players;

namespace Divergency.Content.Items.Weapons.Ranged
{
    public class LivingCoreShotgun : ModItem
    {
        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);
        public override bool AltFunctionUse(Player player) => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Core Shotgun");
            Tooltip.SetDefault("<right> to eject a grenade");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 15;
            Item.crit = 3;
            Item.knockBack = 5f;
            Item.noMelee = true;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileType<CoreBuckshot>();
            Item.shootSpeed = 15f;

            Item.width = Item.height = 16;
            Item.scale = 1.4f;

            Item.useTime = Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Items/LivingCoreShotgun");
            Item.autoReuse = false;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = SoundID.Item61;
            }
            else
            {
                Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Items/LivingCoreShotgun");
            }

            return base.CanUseItem(player);
        }

        public override void HoldItem(Player player)
        {
            if (player == Main.LocalPlayer)
            {
                if (player.ItemAnimationActive && player.altFunctionUse != 2)
                {
                    if (player.itemAnimation < 10) { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation - MathHelper.PiOver2 * player.direction); }
                    else if (player.itemAnimation < 20) { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, player.itemRotation - MathHelper.PiOver2 * player.direction); }
                    else if (player.itemAnimation < 30) { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, player.itemRotation - MathHelper.PiOver2 * player.direction); }
                    else if (player.itemAnimation < 40) { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, player.itemRotation - MathHelper.PiOver2 * player.direction); }
                    else { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation - MathHelper.PiOver2 * player.direction); }

                    if (player.itemAnimation == 40) { SoundEngine.PlaySound(SoundID.Item149, player.position); }
                }
                else { player.SetCompositeArmFront(false, default, default); }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 52f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) { position += muzzleOffset; }

            for (int i = 0; i < 5; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                float scale = 1f - (Main.rand.NextFloat() * 0.9f);

                Dust dust = Dust.NewDustPerfect(position, DustID.Torch, (perturbedSpeed * scale) * 0.5f, 0, default, 3f);
                dust.noGravity = true;

                Dust.NewDustPerfect(position, DustType<Smoke>(), (perturbedSpeed * scale) * 0.5f, 0, Color.Gold, 1f);
            }

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity = 8;

            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectileDirect(source, position, velocity * 0.5f, ProjectileType<CoreGrenade>(), damage, knockback, player.whoAmI);
                return false;
            }
            else
            {
                type = ProjectileType<CoreBuckshot>();

                int NumProjectiles = 3 + Main.rand.Next(3);

                for (int i = 0; i < NumProjectiles; i++)
                {
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(20));
                    newVelocity *= 1f - Main.rand.NextFloat(0.5f);

                    Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                }

                return false;
            }
        }
    }
}
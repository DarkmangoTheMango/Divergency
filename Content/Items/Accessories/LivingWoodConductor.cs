
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using System;
using Terraria.Audio;

namespace Divergency.Content.Items.Accessories
{
    public class LivingWoodConductor : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("guh");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;


        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AcornDrop>().Acorns = true;

        }




    }

    public class AcornDrop : ModPlayer
    {
        public bool Acorns;
        public override void ResetEffects()
        {
            Acorns = false;
        }
            
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (Player.HeldItem.DamageType == DamageClass.Magic && Acorns)
            {
                if (Player.HeldItem.DamageType == DamageClass.Magic && Acorns && !target.immortal && !target.dontTakeDamage)
                {

                    for (int i = 0; i < Main.rand.Next(3, 4); i++)
                    {
                        Projectile.NewProjectile(target.GetSource_FromThis(), target.Center, new Vector2(Main.rand.Next(-20, 20) * 0.9f, Main.rand.Next(-10, 30)) * 0.9f, ModContent.ProjectileType<AcornProj>(), 12, 1f, Player.whoAmI);
                    }

                }
            }
        }
       

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {

            
            if (Player.HeldItem.DamageType == DamageClass.Magic && Acorns && proj.type != ModContent.ProjectileType<AcornProj>())
            {

                for (int i = 0; i < Main.rand.Next(0, 2); i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                    Projectile.NewProjectile(target.GetSource_FromThis(), target.Top, speed * 13, ModContent.ProjectileType<AcornProj>(), 10, 1f, Player.whoAmI);
                }

            }
        }
    }
    public class AcornProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Acorn;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acorn");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 15;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(10);
            Projectile.scale = 0.9f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.velocity.X *= 0.99f;
            for (int i = 0; i < Main.maxNetPlayers; i++)
            {
                Player player = Main.player[i];
                if (Projectile.active && Projectile.Hitbox.Intersects(player.Hitbox) && !player.dead && Projectile.Distance(Projectile.Center) < 15)
                {
                    player.Heal(1);
                    SoundEngine.PlaySound(SoundID.Item2 with { Volume = 0.8f, MaxInstances = 3 });
                    Projectile.Kill();
                }
         
            }
            Projectile.rotation += Projectile.velocity.Length() * (Projectile.direction * 0.04f);

        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) >= float.Epsilon) { Projectile.velocity.X = -oldVelocity.X * 1f; }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) >= float.Epsilon) { Projectile.velocity.Y = -oldVelocity.Y * 0.5f; }
            return false;
        }
    }
}
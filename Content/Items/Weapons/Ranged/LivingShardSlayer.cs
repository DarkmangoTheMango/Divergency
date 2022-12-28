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
using Divergency.Common.Players;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.UI;

namespace Divergency.Content.Items.Weapons.Ranged
{
    public class LivingShardSlayer : ModItem
    {
        public override Vector2? HoldoutOffset() => new Vector2(-12f, 0f);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (player.altFunctionUse == 2) { return false; }
            else { return true; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Core Slayer");
            Tooltip.SetDefault("Overheats when used enough\n<right> to throw the gun when overheated, detonating it on impact");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 8;
            Item.crit = 3;
            Item.knockBack = 8f;
            Item.noMelee = true;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<ShortBullet>();
            Item.shootSpeed = 15f;

            Item.width = Item.height = 16;
            Item.scale = 1.4f;

            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Items/LivingShardSlayer");
            Item.autoReuse = false;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override bool CanUseItem(Player Player)
        {
            if (overheat >= 5 && Player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<LivingShardSlayerPro>();
                Item.shootSpeed = 7f;

                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item1;
                Item.noUseGraphic = true;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<ShortBullet>();
                Item.shootSpeed = 15f;

                Item.useStyle = ItemUseStyleID.Shoot;
                Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Items/LivingShardSlayer");
                Item.noUseGraphic = false;
            }

            return Player.ownedProjectileCounts[ModContent.ProjectileType<LivingShardSlayerPro>()] < 1;
        }

        float overheat = 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (overheat == 4)
            {
                SoundEngine.PlaySound(SoundID.MaxMana, player.Center);

                for (int k = 0; k < 10; k++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(player.Center + (speed * 100f), DustID.TerraBlade, speed * -5f, 0, default, 1f);
                    dust.noGravity = true;
                }
            }

            if (overheat >= 5 && player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<LivingShardSlayerPro>(), damage, knockback, player.whoAmI);
                overheat = 0;
            }
            else
            {
                player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 8;

                if (type == ProjectileID.Bullet) { type = ModContent.ProjectileType<ShortBullet>(); }

                Vector2 offset = Vector2.Normalize(velocity) * 52f;

                if (Collision.CanHit(position, 0, 0, position + offset, 0, 0)) { position += offset; }

                for (int k = 0; k < 5; k++)
                {
                    Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                    float scale = 1f - (Main.rand.NextFloat() * 0.9f);

                    Dust dust = Dust.NewDustPerfect(position, DustID.Torch, (perturbedSpeed * scale) * 0.5f, 0, default, 3f);
                    dust.noGravity = true;

                    Dust.NewDustPerfect(position, ModContent.DustType<Smoke>(), (perturbedSpeed * scale) * 0.5f, 0, new Color(255, 217, 0), 1f);
                }

                for (int k = 0; k < 3; k++)
                {
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(20));
                    newVelocity *= 1f - Main.rand.NextFloat(0.5f);

                    Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                }

                Gore.NewGore(source, player.Center, new Vector2(player.direction * -1, -0.5f) * 2, Mod.Find<ModGore>("ShotgunShell").Type, 1f);

                overheat += 1;
            }

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LivingShard>()
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.IllegalGunParts)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LivingShardSlayerPro : ModProjectile
    {
        public override string Texture => "Divergency/Content/Items/Weapons/Ranged/LivingShardSlayer";

        public override bool? CanCutTiles() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Shard Slayer");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(22);
            Projectile.scale = 1.4f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        float timer = 0f;

        public override void AI()
        {

            Projectile.rotation += Projectile.velocity.Length() * (Projectile.direction * 0.04f);

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 30f) { Projectile.velocity.Y += 0.4f; }

            if (Projectile.ai[1] == 1)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(Projectile.Center + (speed * 100f), DustID.TerraBlade, speed * -5f, 0, default, 1f);
                dust.noGravity = true;

                Projectile.velocity.X *= 0.9f;

                timer++;

                if (timer >= 40f)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.velocity.X *= 0.98f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) >= float.Epsilon) { Projectile.velocity.X = -oldVelocity.X; }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) >= float.Epsilon) { Projectile.velocity.Y = -oldVelocity.Y * 0.2f; }

            if (Projectile.ai[1] == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/Fire"), Projectile.Center);
            }

            Projectile.ai[1] = 1;

            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[1] == 0)
            {
                Projectile.velocity.X = -Projectile.velocity.X;
                Projectile.velocity.Y = -Projectile.velocity.Y * 0.2f;

                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/Fire"), Projectile.Center);
            }

            Projectile.ai[1] = 1;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity = 8;

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/FireHit"), Projectile.Center);
            SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LivingShardSlayerExplosion>(), Projectile.damage * 4, Projectile.knockBack, Projectile.owner);

            for (int k = 0; k < 3; k++)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), Mod.Find<ModGore>("LivingShardSlayer" + k.ToString()).Type, 1.4f);
            }

            for (int k = 0; k < 20; k++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokeIncendiary>(), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, -1f)), 0, new Color(255, 217, 0), 3f);
                dust.noGravity = true;

                dust = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, Main.rand.NextVector2Circular(1f, 1f) * 10, 0, default, 2f);
                dust.noGravity = true;
            }

            for (int k = 0; k < 10; k++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(player.Center + (speed * 100f), DustID.TerraBlade, speed * -5f, 0, default, 1f);
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor);

            if (Projectile.ai[1] == 1)
            {
                position += Main.rand.NextVector2Circular(2f, 2f);
            }

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class LivingShardSlayerExplosion : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Shard Slayer");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(256);
            Projectile.scale = 1f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.timeLeft = 3;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360);
        }
    }
}
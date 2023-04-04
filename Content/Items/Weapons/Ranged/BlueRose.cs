using Divergency.Content.Particles;
using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Weapons.LivingCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Ranged
{
    public class BlueRose : ModItem, IReloadWeapon
    {
        int shotsLeft = 3;
        private bool noBullets;

        public string BulletTexture => "Divergency/Common/UI/BlueRoseBulletUI";

        public int GetRemainingBullets() => shotsLeft;

        public void Reload() => shotsLeft = 3;

        public override bool AltFunctionUse(Player player) => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Rose");
            Tooltip.SetDefault($"Press <right> to perform a Color Up, strengthening the next 3 shots [i:{ModContent.ItemType<BulletMarkIcon>()}]");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 40;
            Item.crit = 3;
            Item.knockBack = 4f;
            Item.noMelee = true;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<BlueRosePro>();
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

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.GetModPlayer<ReloadWeapon>().TryReload(player);
                noBullets = true;

            }
            else
            {
                noBullets = false;

            }
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (shotsLeft > 0)
            {
                type = ModContent.ProjectileType<HighShot>();
            }
            if (noBullets)
            {
                type = ProjectileID.None;
            }

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rotation = velocity.ToRotation();

            Vector2 offset = new Vector2(0.8f, 0).RotatedBy(rotation);

            if (player.altFunctionUse != 2) { if (shotsLeft > 0) { shotsLeft--; } }

            if (!noBullets)
            {
                for (int k = 0; k < 15; k++)
                {
                    Vector2 direction = offset.RotatedByRandom(0.4f);
                    Dust.NewDustPerfect(position + offset * 70, ModContent.DustType<Glow>(), direction * Main.rand.NextFloat(8), 125, new Color(255, 108, 23), Main.rand.NextFloat(0.2f, 0.5f));
                }

                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/BlueRoseShot") { PitchVariance = 0.1f }, player.Center);
            }

            if (shotsLeft > 0)
            {
                SoundEngine.PlaySound(SoundID.Item60, player.Center);
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity, Item.shoot, 0, 0f, player.whoAmI);

            return false;
        }
    }

    public class BlueRosePro : ModProjectile
    {
        bool initialize = true;

        float maxTimeLeft;

        public string BulletTexture => "Divergency/Common/UI/MuscoreUI_Bullet";

        public override string Texture => "Divergency/Content/Items/Weapons/Ranged/BlueRose";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Rose");
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(-0.5f * player.direction, 0f, EaseFunction.EaseQuinticOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));

            player.heldProj = Projectile.whoAmI;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 drawPosition = player.Center + Projectile.rotation.ToRotationVector2() * 30f - Main.screenPosition;

            SpriteEffects drawFlipped = player.direction == -1 ? SpriteEffects.FlipVertically : 0;

            Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, lightColor, Projectile.rotation, origin, Projectile.scale, drawFlipped, 0f);

            return false;
        }
    }

    public class HighShot : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Rose");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.width = Projectile.height = 8;
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 5;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 2;

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center + (Main.rand.NextVector2Circular(1f, 1f) * target.width), Vector2.Zero, ModContent.ProjectileType<BlueRoseFlash>(), 0, 0f, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center + (Main.rand.NextVector2Circular(1f, 1f) * target.width), Vector2.Zero, ModContent.ProjectileType<BlueRoseFlash2>(), 0, 0f, Projectile.owner);

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(target.position, target.width, target.height, DustID.Blood, target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 5f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, default, 2f);
                Dust.NewDustPerfect(Projectile.position, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 10f, 0, new Color(255, 108, 23), Main.rand.NextFloat(0.2f, 0.5f));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            return base.OnTileCollide(oldVelocity);
        }

        public Trail trail;
        public Trail whiteTrail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(255, 108, 23, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                whiteTrail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 247, 179, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            whiteTrail.Draw(Projectile.oldPos);

            return true;
        }
    }

    public class BlueRoseFlash : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Star";

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Rose");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 2f;
            Projectile.Size = new Vector2(100);
            Projectile.alpha = 0;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        bool initilize = true;

        public override void AI()
        {
            if (initilize)
            {
                Projectile.rotation = Main.rand.NextFloat(0f, 360f);

                initilize = false;
            }

            Projectile.scale *= 0.9f;

            Projectile.alpha += 25;
            if (Projectile.alpha >= 255) { Projectile.Kill(); }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class BlueRoseFlash2 : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Ring";

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Rose");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 0f;
            Projectile.Size = new Vector2(600);
            Projectile.alpha = 0;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.scale += 0.05f;

            Projectile.alpha += 10;
            if (Projectile.alpha >= 255) { Projectile.Kill(); }
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

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}

 
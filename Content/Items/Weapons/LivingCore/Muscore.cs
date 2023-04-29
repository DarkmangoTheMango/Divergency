using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.LivingCore
{
    public class Muscore : ModItem, IReloadWeapon
    {
        int shotsLeft = 3;

        public string BulletTexture => "Divergency/Common/UI/MuscoreUI_Bullet";

        public int GetRemainingBullets() => shotsLeft;

        public void Reload() => shotsLeft = 3;

        public override void SetStaticDefaults()
        {
            //.setdefault("Muscore");
            ////.setdefault("");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.damage = 30;
            Item.knockBack = 4f;

            Item.shoot = ModContent.ProjectileType<MuscorePro>();
            Item.shootSpeed = 15f;

            Item.width = Item.height = 96;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;
        }

   
        public override bool CanUseItem(Player player)
        {
            if (shotsLeft > 0) { return base.CanUseItem(player); }
            else { player.GetModPlayer<ReloadWeapon>().TryReload(player); }

            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            shotsLeft--;
            return true;
        }

        public override void HoldItem(Player player)
        {
            if (player == Main.LocalPlayer)
            {
                if (player.ItemAnimationActive) { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation - MathHelper.PiOver2 * player.direction); }
                else { player.SetCompositeArmFront(false, default, default); }
            }
        }
    }

    public class MuscorePro : ModProjectile, IReloadWeapon
    {
        public string BulletTexture => "Divergency/Common/UI/MuscoreUI_Bullet";

        public int GetRemainingBullets() => shotsLeft;

        public void Reload()
        {
            shotsLeft = 3;
        }

        public override void SetStaticDefaults()
        {
            //.setdefault("Muscore");
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1.2f;
            Projectile.Size = Vector2.Zero;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        int shotsLeft = 3;

        float holdoutOffset = 24f;

        bool initilize = true;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (initilize)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/MuscoreShoot"), player.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Projectile.velocity.SafeNormalize(Vector2.One) * 10f, ModContent.ProjectileType<MuscoreBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                initilize = false;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (player.direction > 0 ? MathHelper.PiOver2 : -MathHelper.PiOver2));

            if (player.noItems || player.CCed || player.dead || !player.active) { Projectile.Kill(); }

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float direction = 0f;

            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                direction = (Main.MouseWorld - player.Center).ToRotation();
            }

            Projectile.velocity = direction.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1) { Projectile.rotation = Projectile.velocity.ToRotation(); }
            else { Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi; }

            Projectile.Center = playerCenter + Projectile.velocity * holdoutOffset;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            if (++Projectile.frameCounter >= 6)
            {
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) { Projectile.Kill(); }
                Projectile.frameCounter = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

                int frameHeight = texture.Height / Main.projFrames[Projectile.type];
                int startY = frameHeight * Projectile.frame;

                Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
                Vector2 origin = sourceRectangle.Size() / 2f;
                float offsetX = 40f;
                origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

                Color drawColor = Projectile.GetAlpha(lightColor);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }

            return false;
        }

    }
    public class MuscoreBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Muscore Bullet");

            Main.projFrames[Projectile.type] = 1;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = new Vector2(16);

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }
    }

    public class ReloadWeapon : ModPlayer
    {
        public Item itemReloading = null;
        public int secondsToReload = 1;
        public int timeTillReload = 2;

        public int bulletsOnReload = 0;

        public bool canShoot()
        {
            return (itemReloading == null);
        }

        public override void PreUpdate()
        {
            if (itemReloading == null)
                return;

            if (Player.HeldItem != itemReloading)
                return;

            if (bulletsOnReload != (Player.HeldItem.ModItem as IReloadWeapon).GetRemainingBullets())
            {
                itemReloading = null;
                return;
            }

            timeTillReload--;

            if (timeTillReload == 0)
            {
                IReloadWeapon iweapon = itemReloading.ModItem as IReloadWeapon;

                iweapon.Reload();
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            Item weapon = Player.HeldItem;
            IReloadWeapon iweapon = weapon as IReloadWeapon;

            if (iweapon == null)
                return;

            if (KeybindSystem.Reload.JustPressed)
            {
                TryReload(Player);
            }
        }

        public void TryReload(Player plr)
        {
            Item weapon = plr.HeldItem;

            itemReloading = weapon;
            timeTillReload = secondsToReload * (60 / (int)plr.GetAttackSpeed(DamageClass.Ranged));
            bulletsOnReload = (weapon.ModItem as IReloadWeapon).GetRemainingBullets();
        }
    }

    internal interface IReloadWeapon
    {
        void Reload();
        int GetRemainingBullets();
        string BulletTexture { get; }
    }

    public class ItemSwapKeybindDraw : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            IReloadWeapon iweapon = drawInfo.drawPlayer.HeldItem.ModItem as IReloadWeapon;

            return iweapon != null;
        }

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.BeetleBuff);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            ReloadWeapon modPlr = drawInfo.drawPlayer.GetModPlayer<ReloadWeapon>();
            IReloadWeapon iReloadWeapon = drawInfo.drawPlayer.HeldItem.ModItem as IReloadWeapon;

            Texture2D stockTexture = (Texture2D)ModContent.Request<Texture2D>(iReloadWeapon.BulletTexture);

            int stocksLeft = iReloadWeapon.GetRemainingBullets();

            for (int i = 0; i < stocksLeft; i++)
            {
                Rectangle bulletRect = new Rectangle(0, 0, 12, 22);

                int spacing = 12;

                int position = -(((12 + spacing) * stocksLeft) / 2);
                position += (12 + spacing / 2) * i + spacing;

                drawInfo.DrawDataCache.Add(new DrawData(stockTexture, new Vector2(Main.screenWidth / 2 + position, Main.screenHeight / 2 - 60f), bulletRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0));
            }

            // draw reload ui

            if (modPlr.itemReloading != null)
            {
                //Texture2D LoadingBorder = (Texture2D)ModContent.Request<Texture2D>("Divergency/Common/UI/MuscoreUI_ReloadBar");
                Texture2D Pixel = (Texture2D)ModContent.Request<Texture2D>("Divergency/Assets/Textures/WhitePixel");

                int width = (int)MathF.Floor((float)86 * ((float)modPlr.timeTillReload / ((float)modPlr.secondsToReload * 60)));

                Rectangle back = new Rectangle(0, 0, 86, 28);
                Rectangle fill = new Rectangle(0, 0, width, 12);

                drawInfo.DrawDataCache.Add(new DrawData(Pixel, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 - 60f), fill, new Color(28, 51, 255), 0f, new Vector2(50f, 8f), 1f, SpriteEffects.None, 0));
                //drawInfo.DrawDataCache.Add(new DrawData(LoadingBorder, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 - 60f), back, Color.White, 0f, new Vector2(56f, 16f), 1f, SpriteEffects.None, 0));
            }
        }
    }
}

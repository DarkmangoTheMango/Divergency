using Divergency.Common.Helpers;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Weapons.LivingCore;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
namespace Divergency.Content.Items.Weapons.LivingCore
{
    public class CoreBarrage : ModItem, IReloadWeapon
    {
        public int StackSize => 6;
        public int shotsLeft = 3 * 6;
       
        public string BulletTexture => "Divergency/Common/UI/MuscoreUI_Bullet";

        public int GetRemainingBullets() => shotsLeft;

        public void Reload() => shotsLeft = 3 * StackSize;
      
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
            Item.damage = 28;
            Item.knockBack = 4f;

            Item.shoot = ModContent.ProjectileType<CoreBarrageProj>();
            Item.shootSpeed = 15f;

            Item.width = Item.height = 96;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.reuseDelay = 30;
        }

        public override bool CanUseItem(Player player)
        {
            if (shotsLeft > 0) { return base.CanUseItem(player); }
            else
            {

                if (player.GetModPlayer<CoreBarragePlayer>().ShotCounterTotal >= 18)
                {
                    shotsLeft = 1;
                    player.Heal(5);
              
                    player.GetModPlayer<CoreBarragePlayer>().ShotCounterTotal = 0;
                    
                }
                else if (player.GetModPlayer<CoreBarragePlayer>().ShotCounterTotal < 18)
                {
                    player.GetModPlayer<CoreBarragePlayer>().ShotCounterTotal = 0;
                    player.GetModPlayer<ReloadWeapon>().TryReload(player);

                }



            }

            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
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

    public class CoreBarrageProj : ModProjectile
    {
        private float MovementFactor = 24f;
        //public override string Texture => "DivergencyMod/Items/Weapons/Ranged/Muscore/Bullet";

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.damage = 1;

            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }

        public float Timer = 7;
        private bool hasShot = false;


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Timer++;
            if (player.noItems || player.CCed || player.dead || !player.active || !Main.mouseLeft || (player.HeldItem.ModItem as CoreBarrage) == null)
                Projectile.Kill();


            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                swordRotation = (Main.MouseWorld - player.Center).ToRotation();
            }
            Projectile.velocity = swordRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation();
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            Projectile.Center = playerCenter + Projectile.velocity * MovementFactor;// customization of the hitbox position

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            if (Timer == 10)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity * 5, ModContent.ProjectileType<CoreBarrageBullet>(), Projectile.damage + player.GetModPlayer<CoreBarragePlayer>().ShotCounterTotal, 2, Projectile.owner, Projectile.whoAmI);
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/MuscoreShoot"), player.Center);
                Timer = 0;

                (player.HeldItem.ModItem as CoreBarrage).shotsLeft--;
                hasShot = true;
            }
            if ((player.HeldItem.ModItem as CoreBarrage).shotsLeft % (player.HeldItem.ModItem as CoreBarrage).StackSize == 0 && hasShot)
            {
                Projectile.Kill();
            }


        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                // Getting texture of projectile
                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

                // Calculating frameHeight and current Y pos dependence of frame
                // If texture without animation frameHeight is always texture.Height and startY is always 0
                int frameHeight = texture.Height / Main.projFrames[Projectile.type];
                int startY = frameHeight * Projectile.frame;

                // Get this frame on texture
                Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

                // Alternatively, you can skip defining frameHeight and startY and use this:
                // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

                Vector2 origin = sourceRectangle.Size() / 2f;

                // If image isn't centered or symmetrical you can specify origin of the sprite
                // (0,0) for the upper-left corner
                float offsetX = 40f;
                origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

                // If sprite is vertical
                // float offsetY = 20f;
                // origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);

                // Applying lighting and draw current frame

                Color drawColor = Projectile.GetAlpha(lightColor);
                Main.EntitySpriteDraw(texture,
                    Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                    sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

    }
    public class CoreBarrageBullet : ModProjectile
    {
        public float timer;
        public float Timer;
        private Vector2 unmodifiedVelocity;

        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25; // in SetStaticDefaults()
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20; // The width of projectile hitbox
            Projectile.height = 20; // The height of projectile hitbox
            Projectile.damage = 50;
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.timeLeft = 1000; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 10;
            Projectile.penetrate = -1;

            Projectile.scale = 1f;
        }
        public override void AI()
        {
            Timer++;


            Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);
            float multiplier = 0.4f;
            float max = 1f;
            float min = 1.0f;
            RGB *= multiplier;
            if (RGB.X > max)
            {
                multiplier = 0.5f;
            }
            if (RGB.X < min)
            {
                multiplier = 1.5f;
            }
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            timer++;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
            Projectile.spriteDirection = Projectile.direction;

            if (timer == 7)
            {
                for (int j = 0; j < 1; j++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(Projectile.Center + (velocity * 15f), ModContent.DustType<Glow>(), velocity * 1.1f, 0, Color.LimeGreen, 0.1f);


                }
                timer = 2;
            }





        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            player.GetModPlayer<CoreBarragePlayer>().ShotCounterTotal++;
            Projectile.velocity = new Vector2(0, 0);
            Projectile.damage = 0;
            Projectile.timeLeft = 60;
            for (int j = 0; j < 5; j++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(Projectile.Center + (velocity), ModContent.DustType<Glow>(), velocity * 1.2f, 0, Color.LimeGreen, 1f);



            }
        }
        public Trail trail;
        public Trail whiteTrail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(0, 255, 0, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                whiteTrail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(5f), (p) => Projectile.GetAlpha(new Color(158, 249, 255, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            whiteTrail.Draw(Projectile.oldPos);


            return false;
        }

    }
    public class CoreBarragePlayer : ModPlayer
    {
        public int ShotCounterTotal;

      
    }

}

  
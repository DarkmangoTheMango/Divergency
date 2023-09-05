using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static Divergency.Content.Projectiles.Magic.CorescillationProj;
using static ParticleLibrary.Particle;

namespace Divergency.Content.Items.Weapons.LivingCore
{
    public class Muscore : ModItem, IReloadWeapon
    {
        int shotsLeft = 6;

        public string BulletTexture => "Divergency/Common/UI/MuscoreUI_Bullet";

        public int GetRemainingBullets() => shotsLeft;

        public void Reload() => shotsLeft = 6;

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

            Item.shoot = ModContent.ProjectileType<MuscoreProj>();
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

    public class MuscoreProj : ModProjectile, IReloadWeapon
    {
        int shotsLeft = 6;

        public string BulletTexture { get { return "DivergencyMod/Items/Weapons/Ranged/Muscore/Bullet"; } }
        public int GetRemainingBullets() { return shotsLeft; }
        public void Reload()
        {
            shotsLeft = 6;
        }
        private float MovementFactor = 24f;
        //public override string Texture => "DivergencyMod/Items/Weapons/Ranged/Muscore/Bullet";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
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
            Projectile.scale = 1.2f;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(Projectile.Center + (velocity * 80f), ModContent.DustType<Glow>(), velocity * 6f, 0, Color.LimeGreen, 0.8f);
            }
        }
        int frameTimer;
        bool Shooting;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Timer++;
            if (player.noItems || player.CCed || player.dead || !player.active)
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
            if (!Shooting)
            {
                frameTimer++;

                if (frameTimer == 1)
                {
                    Projectile.frame = 4;

                }
                if (frameTimer == 10)
                {
                    Projectile.frame++;
                    frameTimer = 2;
                }
                if (Projectile.frame == 10)
                {
                    Shooting = true;
                    frameTimer = 0;

                }
            }
            else
            {
                frameTimer++;

                if (frameTimer == 1)
                {
                    Projectile.frame = 0;

                }
                if (frameTimer == 7)
                {
                    Projectile.frame++;
                    frameTimer = 2;
                }
                if (Projectile.frame == 2 && frameTimer == 3)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity * 15, ModContent.ProjectileType<MuscoreBullet>(), Projectile.damage, 2, Projectile.owner, Projectile.whoAmI);
                    SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/MuscoreShoot"), player.Center);
                }
                if (Projectile.frame == 4)
                {

                    Projectile.Kill();

                }
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
    public class MuscoreBullet : ModProjectile
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
            Projectile.width = 10; // The width of projectile hitbox
            Projectile.height = 10; // The height of projectile hitbox
            Projectile.damage = 50;
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.timeLeft = 1000; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 5;

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
                for (int j = 0; j < 5; j++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(Projectile.Center + (velocity * 80f), ModContent.DustType<Glow>(), velocity * 2f, 0, Color.LimeGreen, 0.5f);


                }
                timer = 2;
            }





        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
         
            target.GetGlobalNPC<MuscoreBulletNPC>().hitBullets++;
            target.GetGlobalNPC<MuscoreBulletNPC>().timer = 180;

        }
        public Trail trail;
        public Trail whiteTrail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(0, 255, 0, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                whiteTrail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(158, 249, 255, 100)));
                whiteTrail.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            whiteTrail.Draw(Projectile.oldPos);


            return false;
        }

    }
    public class MuscoreBulletNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int hitBullets;
        public int timer;
        public override bool PreAI(NPC npc)
        {
            if (timer >= 1)
                timer--;

            if (timer == 0)
                hitBullets = 0;
            if (hitBullets == 2)
            {
                float angle1 = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(npc.Center + (velocity * 80f), ModContent.DustType<Glow>(), velocity * 6f, 0, Color.LimeGreen, 0.6f);


            }

            if (hitBullets == 3)
            {
                hitBullets = 0;

                Player player = Main.LocalPlayer;
                player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 12;

                DivergencyDraw.SpawnExplosion(npc.Center, Color.LimeGreen, DustID.TerraBlade, 0);
                for (int i = 0; i < 20; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(npc.Center + (velocity * 80f), ModContent.DustType<Glow>(), velocity * 6f, 0, Color.LimeGreen, 0.6f);
                }


                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, Vector2.Zero, ModContent.ProjectileType<LivingExplosion>(), 200    ,
                    5, Main.myPlayer);

                int numberDust = 5;

                for (int i = 0; i < numberDust; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(npc.Center + (velocity * 80f), ModContent.DustType<Glow>(), velocity * 6f, 0, Color.LimeGreen, 0.6f);
                }
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDustPerfect(npc.Center, ModContent.DustType<Smoke>(), Main.rand.NextVector2CircularEdge(1f, 1f) * 5, 0, Color.LimeGreen, 3f);
                    Dust.NewDustPerfect(npc.Center, ModContent.DustType<Smoke>(), npc.velocity.SafeNormalize(Vector2.One) * Main.rand.NextFloat(-1f, -4f), 0, Color.LimeGreen, 2f);

                }
            }
            return base.PreAI(npc);
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
            timeTillReload = secondsToReload * (100 / (int)plr.GetAttackSpeed(DamageClass.Ranged));
            bulletsOnReload = (weapon.ModItem as IReloadWeapon).GetRemainingBullets();
        }
    }

    internal interface IReloadWeapon
    {
        void Reload();
        int GetRemainingBullets();
        string BulletTexture { get; }
    }

    public class ItemSwapKeybindDraw : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            IReloadWeapon iweapon = Main.LocalPlayer.HeldItem.ModItem as IReloadWeapon;

            if (iweapon == null)
                return;

            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Divergency: General Reload Weapon UI",
                    delegate {
                        UIDraw(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            /*
            Console.WriteLine("START");
            foreach (var layer in layers)
            {
                Console.WriteLine(layer.Name);
            }
            Console.WriteLine("END");
            */
        }
        private void UIDraw(SpriteBatch spriteBatch)
        {
            ReloadWeapon modPlr = Main.LocalPlayer.GetModPlayer<ReloadWeapon>();
            IReloadWeapon iReloadWeapon = Main.LocalPlayer.HeldItem.ModItem as IReloadWeapon;

            Texture2D stockTexture = (Texture2D)ModContent.Request<Texture2D>(iReloadWeapon.BulletTexture);

            int stocksLeft = iReloadWeapon.GetRemainingBullets();

            Console.WriteLine("reach??");

            for (int i = 0; i < stocksLeft; i++)
            {
                Rectangle bulletRect = new Rectangle(0, 0, 12, 22);

                int spacing = 12;

                int position = -(((12 + spacing) * stocksLeft) / 2);
                position += (12 + spacing / 2) * i + spacing;

                spriteBatch.Draw(stockTexture, new Vector2(Main.screenWidth / 2 + position, Main.screenHeight / 2 - 60f), bulletRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }

            // draw reload ui

            if (modPlr.itemReloading != null)
            {
                //Texture2D LoadingBorder = (Texture2D)ModContent.Request<Texture2D>("Divergency/Common/UI/MuscoreUI_ReloadBar");
                Texture2D Pixel = (Texture2D)ModContent.Request<Texture2D>("Divergency/Assets/Textures/WhitePixel");

                int width = (int)MathF.Floor((float)86 * ((float)modPlr.timeTillReload / ((float)modPlr.secondsToReload * 60)));

                Rectangle back = new Rectangle(0, 0, 86, 28);
                Rectangle fill = new Rectangle(0, 0, width, 12);

                spriteBatch.Draw(Pixel, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 - 60f), fill, new Color(28, 51, 255), 0f, new Vector2(50f, 8f), 1f, SpriteEffects.None, 0);
                //drawInfo.DrawDataCache.Add(new DrawData(LoadingBorder, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 - 60f), back, Color.White, 0f, new Vector2(56f, 16f), 1f, SpriteEffects.None, 0));
            }
        }
    }
}

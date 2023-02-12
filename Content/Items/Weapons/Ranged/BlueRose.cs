using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Weapons.LivingCore;
using Divergency.Content.NPCs.LivingGrove;
using Divergency.Content.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

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
                type = ModContent.ProjectileType<BlueRoseBullet>();
            }
            if (noBullets)
            {
                type = ProjectileID.None;
            }

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 offset = Vector2.Normalize(velocity) * 52f;
            if (player.altFunctionUse != 2)
            {
                if (shotsLeft > 0)
                {
                    shotsLeft--;
                }
            }

            if (!noBullets)
            {
                for (int k = 0; k < 5; k++)
                {
                    Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(10));
                    float scale = 1f - (Main.rand.NextFloat() * 0.9f);

                    Dust dust = Dust.NewDustPerfect(position + offset - new Vector2(0, 13), DustID.Torch, (perturbedSpeed * scale) * 0.5f, 0, default, 3f);
                    dust.noGravity = true;
                }

                for (int k = 0; k < 10; k++)
                {

                    Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                    float scale = 1f - (Main.rand.NextFloat() * 0.9f);

                    Dust.NewDustPerfect(position + offset, ModContent.DustType<Smoke>(), (perturbedSpeed * scale) * 0.5f, 0, new Color(255, 217, 0), 0.4f);
                }

                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/BlueRoseColorShot") { PitchVariance = 0.1f }, player.Center);
            }
            Projectile.NewProjectile(source, position - new Vector2(0, 10), velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position + new Vector2(0, 0), velocity, type, damage, knockback, player.whoAmI);
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(-1f * player.direction, 0f, EaseFunction.EaseQuinticOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));

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
    public class ColorUpProj : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";

        private float timer;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.aiStyle = -1;
            Projectile.scale = 0.1f;

          
        }
        public override void AI()
        {
            timer--;
            if (timer <= 0f)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + (Main.rand.NextVector2Circular(1f, 1f) * 30f), Vector2.Zero, ModContent.ProjectileType<ColorUpExplosion>(), 20, 0f, 0, Main.rand.NextFloat(0f, 360f), 0f);
                for (int k = 0; k < 10; k++)
                {

                    Vector2 perturbedSpeed = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(360));
                    float scale = 1f - (Main.rand.NextFloat() * 0.9f);

                    Dust.NewDustPerfect(Projectile.position + (Main.rand.NextVector2Circular(1f, 1f) * 30f), DustID.Torch, (perturbedSpeed * scale) * 30f, 0, new Color(255, 217, 0), 1.3f);
                    DivergencyDraw.SpawnExplosion(Projectile.position + (Main.rand.NextVector2Circular(1f, 1f) * 30f),  Color.Orange, DustID.Torch, shakeAmount: -1f,1,1,0.05f,true);
                    
                }
                timer = 10f;
            }
        }
    }
    public class ColorUpExplosion : ModProjectile
    {
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.damage = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.05f;
            Projectile.Size = new Vector2(100, 100);
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0];
            Projectile.scale += 0.01f;

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;

                if (++Projectile.frame >= 5)
                {
                    Projectile.frame = 0;
                    Projectile.Kill();
                }
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

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
    public class BlueRoseBullet : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dead weight");

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

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            if (Projectile.ai[0] >= 15f) { Projectile.velocity *= 0.97f; }

            if (Projectile.velocity.Length() < 0.1f) { Projectile.Kill(); }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int k = 0; k < 2; k++)
            {
                Vector2 perturbedSpeed = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30));
                float scale = 1f - (Main.rand.NextFloat() * 0.9f);

                Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.Torch, (perturbedSpeed * scale) * -0.5f, 0, default, 3f);
                dust.noGravity = true;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ColorUpProj>(), 0, 0f, 0, Main.rand.NextFloat(0f, 360f), 0f);

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

}

 
using Divergency.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class Ripsaw : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("(Rework Pending) Ripsaw");
            Tooltip.SetDefault("'These filthy trees will shat themselves!'"
            +"\nPenetrates 3 enemy defense");

            ItemID.Sets.IsChainsaw[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 9;
            Item.axe = 5;
            Item.knockBack = 2f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<RipsawPro>();
            Item.shootSpeed = 50f;

            Item.width = Item.height = 16;
            Item.scale = 1.3f;

            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item23;
            Item.autoReuse = false;
            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.value = Item.sellPrice(0, 1, 40, 0);
            Item.rare = ItemRarityID.Blue;
        }
    }

    public class RipsawPro : ModProjectile
    {
        public override string Texture => "Divergency/Content/Items/Weapons/Melee/Ripsaw";

        private const int OFFSET = 70;
        private const int MAXCHARGE = 20;

        public ref float Charge => ref Projectile.ai[0];
        public ref float Angle => ref Projectile.ai[1];

        float oldAngle = 0f;

        public Vector2 direction = Vector2.Zero;

        private int counter;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ripsaw");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(22);
            Projectile.scale = 1.2f;
            Projectile.hide = true;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.ownerHitCheck = true;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            player.itemTime = 5;
            player.itemAnimation = 5;

            float shake = 0;

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Main.MouseWorld - (player.Center)).ToRotation();

                if (Math.Abs(oldAngle - Angle) > 0.1f)
                {
                    oldAngle = Angle;
                    Projectile.netUpdate = true;
                }
            }

            direction = Angle.ToRotationVector2();

            player.ChangeDir(direction.X > 0 ? 1 : -1);
            shake = 0.04f;

            if (counter % 30 == 1)
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);

            if (!player.channel)
            {
                Projectile.Kill();
            }

            Projectile.Center = player.Center + (direction * OFFSET * Main.rand.NextFloat(1 - shake, 1 + shake));
            Projectile.velocity = Vector2.Zero;
            player.itemRotation = direction.ToRotation();

            if (player.direction != 1)
                player.itemRotation -= MathHelper.Pi;

            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            player.heldProj = Projectile.whoAmI;

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, default, 1.2f);
            dust.noGravity = true;

            if (Charge >= MAXCHARGE)
            {
                Charge = MAXCHARGE;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Charge++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * OFFSET)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();

            Color color = new Color(255, 140, 0, 100);

            if (Player.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, color, direction.ToRotation(), origin, Projectile.scale + (Charge * 0.01f), effects1, 0.0f);
            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, color, direction.ToRotation() - MathHelper.Pi, origin, Projectile.scale + (Charge * 0.01f), effects1, 0.0f);
            }

            texture = TextureAssets.Projectile[Projectile.type].Value;

            if (Player.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation(), origin, Projectile.scale, effects1, 0.0f);

            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation() - MathHelper.Pi, origin, Projectile.scale, effects1, 0.0f);
            }

            return false;
        }
    }
}
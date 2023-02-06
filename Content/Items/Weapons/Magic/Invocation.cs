using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Magic
{
    public class Invocation : ModItem
    {
        public override Vector2? HoldoutOffset() => Vector2.Zero;

        public override bool CanUseItem(Player player) => player.statMana >= 20;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Commandant's Guide To Invocation");
            Tooltip.SetDefault("Chargable");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 25;
            Item.knockBack = 7f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<InvocationHoldout>();
            Item.shootSpeed = 15f;
            Item.channel = true;

            Item.width = Item.height = 16;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = Item.reuseDelay = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.useTurn = false;
            Item.mana = 10;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void HoldItem(Player player)
        {
            if (player.ItemAnimationActive) { player.manaRegenDelay = 120; }
        }
    }

    public class InvocationHoldout : ModProjectile
    {
        public override string Texture => "Divergency/Content/Items/Weapons/Magic/Invocation";

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = new Vector2(34);
            Projectile.hide = true;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        bool initilize = true;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (initilize)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationCharge"), player.Center);
                initilize = false;
            }

            if (Main.LocalPlayer == player)
            {
                Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld), 1f));
                Projectile.netUpdate = true;

                player.ChangeDir(Math.Sign(Projectile.velocity.X));
            }

            Projectile.rotation = MathHelper.WrapAngle(Projectile.velocity.ToRotation() + (Projectile.direction < 0 ? MathHelper.Pi : 0));

            player.itemRotation = Projectile.rotation;
            player.itemAnimation = player.itemTime = 2;
            player.heldProj = Projectile.whoAmI;

            Projectile.timeLeft = 2;
            Projectile.Center = player.MountedCenter;

            if (!player.channel) { Projectile.Kill(); }

            AbstractAI();
        }

        void AbstractAI()
        {
            Player player = Main.player[Projectile.owner];

            for (int k = 0; k < (Projectile.ai[0] / 40); k++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(player.Center + (velocity * 100f), DustID.GemAmethyst, velocity * -5f, 0, default, Main.rand.NextFloat(0.5f, 1f));
                dust.noGravity = true;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 80)
            {
                player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 5;

                for (int k = 0; k < 20; k++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, Main.rand.NextVector2CircularEdge(1f, 1f) * 5f, 0, default, 2f);
                    dust.noGravity = true;
                }

                player.channel = false;
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.ai[0] >= 80)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot"), player.Center);
                SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, player.Center);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 20f, ModContent.ProjectileType<ShadowflameEffigy>(), Projectile.damage * 4, Projectile.knockBack, Projectile.owner, 0f, 1);
                player.statMana -= player.GetManaCost(player.HeldItem);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, player.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<ShadowflameEffigy>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0);
            }

            player.manaRegenDelay = 120;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Star").Value;

            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame, 0, 0);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 drawPosition = player.Center + ((Projectile.rotation.ToRotationVector2() * 30f) * Projectile.direction) - Main.screenPosition;

            Color textureColor = new Color(241, 150, 255);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.Additive);

            Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, textureColor, Projectile.ai[0] * 0.02f, origin, Projectile.scale * (Projectile.ai[0] * 0.02f), SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, default);

            return false;
        }
    }
}
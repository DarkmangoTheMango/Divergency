using Divergency.Common.Helpers;
using Divergency.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Ranged
{
    public class Wrathfire : ModItem
    {
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(16, 16);
            Item.scale = 0.5f;

            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 1;
            Item.knockBack = 1;

            Item.axe = 11;
            Item.tileBoost = 1;

            Item.shoot = ModContent.ProjectileType<WrathfirePro>();
            Item.shootSpeed = 1;

            Item.channel = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.UseSound = SoundID.Item73;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
    }

    public class WrathfirePro : ModProjectile
    {
        public override string Texture => "Divergency/Content/Items/Weapons/Ranged/Wrathfire";

        Player player => Main.player[Projectile.owner];

        Vector2 shakeOffset;

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanCutTiles() => false;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.Size = new (16);

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<Sawblade>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0, Projectile.whoAmI);
            AI();
        }

        public override void AI()
        {
            if (!player.channel)
                Projectile.Kill();

            Projectile.direction = Projectile.Center.X > player.Center.X ? 1 : -1;

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Projectile.direction;

            Projectile.velocity += (player.DirectionTo(Main.MouseWorld) - Projectile.velocity) * 0.2f;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, false, true) + Projectile.velocity * 30f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            player.itemRotation = Projectile.rotation;
            player.SetDummyItemTime(2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2);

            Dust.NewDustPerfect(player.RotatedRelativePoint(player.MountedCenter, false, true) + Projectile.velocity * 90f, DustID.Terra, -Projectile.velocity * 3, 0, default, 1).noGravity = true;
            Lighting.AddLight(Projectile.Center, new Vector3(0, 1, 0) * 0.1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Color color = Projectile.GetAlpha(lightColor);

            Vector2 drawOrigin = sourceRectangle.Size() / 2;

            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            //Draw glow effect

            texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

            color = Projectile.GetAlpha(Color.White);

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }

    public class WrathBlast : ModProjectile
    {
        float timer;

        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void Kill(int timeLeft) => SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(50);
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        public Trail trail;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Shadow").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(60f), (p) => Projectile.GetAlpha(new Color(0, 255, 0, 0)) * (float)Math.Pow(1f - p, 2f));
                trail.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos, timer);
            timer -= 0.05f;

            return false;
        }
    }
}
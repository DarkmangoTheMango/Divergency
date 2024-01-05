using Divergency.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class Ripsaw : ModItem
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

            Item.shoot = ModContent.ProjectileType<RipsawPro>();
            Item.shootSpeed = 1;

            Item.channel = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.UseSound = SoundID.Item23;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
    }

    public class RipsawPro : ModProjectile
    {
        Player player => Main.player[Projectile.owner];

        Vector2 shakeOffset;

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanCutTiles() => false;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16, 16);
            Projectile.scale = 0.5f;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<Sawblade>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0, Projectile.whoAmI);
            AI();
        }

        public override void AI()
        {
            if (!player.channel)
                Projectile.Kill();

            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
                Projectile.soundDelay = 30;
            }

            Projectile.direction = Projectile.Center.X > player.Center.X ? 1 : -1;

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Projectile.direction;

            shakeOffset = Main.rand.NextVector2Circular(1, 1);

            Projectile.velocity += (player.DirectionTo(Main.MouseWorld) - Projectile.velocity) * 0.2f;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, false, true) + Projectile.velocity * 20f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            player.itemRotation = Projectile.rotation;
            player.SetDummyItemTime(2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + shakeOffset;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Color color = Projectile.GetAlpha(lightColor);

            Vector2 drawOrigin = sourceRectangle.Size() / 2;

            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }

    public class Sawblade : ModProjectile
    {
        Player player => Main.player[Projectile.owner];

        Projectile parent => Main.projectile[(int)Projectile.ai[2]];

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(100, 100);
            Projectile.scale = 0.5f;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (player.channel)
            {
                Projectile.velocity = parent.velocity;
                Projectile.Center = parent.Center + Projectile.velocity * 36;
            }
            else
                Projectile.Kill();

            Projectile.rotation += parent.direction * 0.2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

            for (int k = 0; k < 15; k++)
            {
                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(1, 1) * 10, ModContent.DustType<ThickBlood>(), new Vector2(hit.HitDirection, 0).RotatedByRandom(1) * Main.rand.NextFloat(1, 3), 0, default, Main.rand.NextFloat(1, 1.2f));

                if (k >= 12)
                {
                    ParticleManager.NewParticle<Spark>(target.Center, new Vector2(hit.HitDirection, 0).RotatedByRandom(0.5f) * Main.rand.NextFloat(8, 20), default, 1f, 1);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Color color = Projectile.GetAlpha(lightColor);

            Vector2 drawOrigin = sourceRectangle.Size() / 2;

            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }

    public class ThickBlood : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            if (dust.position.X > 16 && dust.position.Y > 16)
            {
                Tile tile = Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16];

                if (tile.HasTile && tile.BlockType == BlockType.Solid && Main.tileSolid[tile.TileType])
                {
                    dust.velocity.Y *= -0.5f;
                    dust.velocity.X *= 0.5f;
                    dust.alpha += 2;
                }
            }

            dust.position += dust.velocity;
            dust.velocity.Y += 0.2f;

            dust.rotation = dust.velocity.ToRotation();
            dust.scale *= 0.98f;

            if (dust.scale < 0.05f)
                dust.active = false;

            return false;
        }
    }
}
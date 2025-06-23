using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Ranged
{
    public class Bloomstick : ModItem
    {
        public override Vector2? HoldoutOffset() => new(-8, 0);

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 2;
            Item.crit = 3;
            Item.knockBack = 4f;
            Item.noMelee = true;
            Item.useAmmo = AmmoID.Bullet;

            Item.shoot = ModContent.ProjectileType<BloomstickBullet>();
            Item.shootSpeed = 17f;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Shotgun") with { PitchVariance = 0.15f };
            Item.autoReuse = true;

            Item.Size = new Vector2(60, 38);

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            const int numProjectiles = 8;

            Projectile.NewProjectile(Entity.GetSource_FromAI(), position - new Vector2(52, 0).RotatedBy(velocity.ToRotation()), new Vector2(-player.direction * Main.rand.NextFloat(2, 5), -Main.rand.NextFloat(5, 9)), ModContent.ProjectileType<ShotgunShell>(), 0, 0);
            ParticleManager.NewParticle<BloomstickFlash>(position - new Vector2(23, 0).RotatedBy(velocity.ToRotation()), velocity, default, 1f, 1);

            for (int i = 0; i < numProjectiles; i++)
            {
                float spreadAngle = 20f;
                const float velocityVariation = 0.5f;

                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(spreadAngle));
                newVelocity *= 1f - Main.rand.NextFloat(velocityVariation);

                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            const int numParticles = 8;

            for (int i = 0; i < numParticles; i++)
            {
                float spreadAngle = 20f;
                const float velocityVariation = 1f;

                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(spreadAngle));
                newVelocity *= 1f - Main.rand.NextFloat(velocityVariation);

                ParticleManager.NewParticle<Sparkle>(position, newVelocity, default, 1f, 1);
            }

            const int numParticles2 = 4;

            for (int i = 0; i < numParticles2; i++)
            {
                float spreadAngle = 20f;
                const float velocityVariation = 1f;

                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(spreadAngle));
                newVelocity *= 1f - Main.rand.NextFloat(velocityVariation);

                ParticleManager.NewParticle<BloomstickSmoke>(position, newVelocity * 0.2f, default, 1f, 1);
            }

            const int numDusts = 8;

            for (int i = 0; i < numDusts; i++)
            {
                float spreadAngle = 20f;
                const float velocityVariation = 1f;

                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(spreadAngle));
                newVelocity *= 1f - Main.rand.NextFloat(velocityVariation);

                Dust.NewDustPerfect(position, DustID.Smoke, newVelocity, 100, default, 1.5f).noGravity = true;
            }

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 2;
            
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 offset = new Vector2(52, 0).RotatedBy(velocity.ToRotation());

            if (Collision.CanHit(position, 0, 0, position + offset, 0, 0))
                position += offset;

            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<BloomstickBullet>();
        }
    }

    public class BloomstickBullet : ModProjectile
    {
        public override string Texture => TextureGrabber.Empty;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;

            Projectile.Size = new(8);
            Projectile.ignoreWater = true;

            Projectile.extraUpdates = 4;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 100;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.velocity.Length() <= 0.01f)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)Projectile.timeLeft / 100f).ToVector3() * 0.2f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            Lighting.AddLight(Projectile.Center, Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)Projectile.timeLeft / 100f).ToVector3() * 0.5f);

            return base.OnTileCollide(oldVelocity);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

        }

        public override void Kill(int timeLeft)
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/HalfLight").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            Vector2 origin = rectangle.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Color drawColor = Projectile.GetAlpha(Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)Projectile.timeLeft / 100f));

            Vector2 scale = new(0.2f, Projectile.velocity.Length() * 0.4f);

            Main.EntitySpriteDraw(texture, drawPosition, rectangle, drawColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0);

            drawColor = Projectile.GetAlpha(new Color(255, 255, 255, 0));

            scale = new(0.05f, Projectile.velocity.Length() * 0.4f);

            Main.EntitySpriteDraw(texture, drawPosition, rectangle, drawColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
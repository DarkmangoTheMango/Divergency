using Divergency.Common.Helpers;
using Divergency.Common.Helpers.SwordAnimator;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Weapons.Ranged;
using Divergency.Content.Particles;
using ParticleLibrary;
using ReLogic.Utilities;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class CoreCrystalize : ModItem
    {

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 35;
            Item.knockBack = 5f;

            Item.shoot = ModContent.ProjectileType<CoreCrystalizePro>();
            Item.shootSpeed = 1f;

            Item.Size = new(16);
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.channel = true;

            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
    }

    public class CoreCrystalizePro : ModProjectile
    {
        SlotId soundSlot;

        Player player => Main.player[Projectile.owner];

        Vector2 shakeOffset;

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanCutTiles() => false;

        float charge;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.Size = new(16);

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = MathHelper.PiOver2;

            //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<Sawblade>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0, Projectile.whoAmI);
            AI();
        }

        public override void AI()
        {
            if (!player.channel)
                Projectile.Kill();

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Projectile.direction;

            shakeOffset = Main.rand.NextVector2Circular(1, 1) * (Math.Clamp(charge, 0, 30) * 0.05f);

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, false, true) + Projectile.rotation.ToRotationVector2();
            Projectile.rotation = MathHelper.Lerp(MathHelper.PiOver2, -MathHelper.PiOver2, EaseFunction.EaseCubicInOut.Ease(Math.Clamp(charge, 0, 60) / 60f));

            player.itemRotation = Projectile.rotation;
            player.SetDummyItemTime(2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2);

            Lighting.AddLight(Projectile.Center, new Vector3(0, 1, 0) * 0.1f);

            charge++;

            if (!SoundEngine.TryGetActiveSound(soundSlot, out _))
            {
                var tracker = new ProjectileAudioTracker(Projectile);

                soundSlot = SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/WrathfireCharge")
                {
                    IsLooped = true,
                    SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest,
                }, Projectile.Center, soundInstance =>
                {
                    soundInstance.Pitch = MathHelper.Lerp(-1f, 0f, Math.Clamp(charge, 0, 60) / 60f);
                    soundInstance.Position = Projectile.Center;
                    return tracker.IsActiveAndInGame() && player.active && Projectile.active;
                });
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(80, -86 * player.direction).RotatedBy(Projectile.rotation) + shakeOffset;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);

            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, sourceRectangle.Size() * 0.5f, Projectile.scale, spriteEffects, 0);

            //Draw glow effect

            texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

            Color color = Projectile.GetAlpha(Color.White);

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, sourceRectangle.Size() * 0.5f, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}
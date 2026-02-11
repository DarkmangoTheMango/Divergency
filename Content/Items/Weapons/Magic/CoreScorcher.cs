using Divergency.Content.Projectiles;
using Divergency.Content.Projectiles.Ranged;
using ReLogic.Utilities;
using Terraria.Audio;

namespace Divergency.Content.Items.Weapons.Magic;

public class CoreScorcher : ModItem
{
    public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Magic;
        Item.damage = 25;
        Item.knockBack = 1f;
        Item.noMelee = true;
        Item.mana = 2;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.channel = true;
        Item.value = Item.sellPrice(0, 2, 50, 0);
        Item.rare = ItemRarityID.Green;

        Item.Size = new(110, 40);

        Item.shoot = ModContent.ProjectileType<CoreScorcherPro>();
        Item.shootSpeed = 1;

        Item.channel = true;
        Item.noUseGraphic = true;
        Item.useTime = Item.useAnimation = 5;
        Item.reuseDelay = 10;
        Item.useStyle = ItemUseStyleID.Shoot;

        Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Items/CoreScorcherStart") with { PauseBehavior = PauseBehavior.PauseWithGame };

        Item.value = Item.sellPrice(0, 5, 0, 0);
        Item.rare = ItemRarityID.Green;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        player.manaCost = 0f;

        return true;
    }
}

public class CoreScorcherPro : ModProjectile
{
    Player player => Main.player[Projectile.owner];

    Vector2 shakeOffset;

    SlotId soundSlot;

    public override string Texture => "Divergency/Content/Items/Weapons/Magic/CoreScorcher";

    public override bool ShouldUpdatePosition() => false;

    public override bool? CanCutTiles() => false;

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Projectile.Size = new(110);

        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = -1;

        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;

        Projectile.aiStyle = -1;
    }

    public override void OnSpawn(IEntitySource source)
    {
        SoundEngine.PlaySound(SoundID.Item73, player.Center);
        AI();
    }
    private float fade = 0f;
    private bool dying = false;

    public override void AI()
    {
        if (!player.CheckMana(2))
            Projectile.Kill();

        if (!player.channel && player.itemTime <= 2)
            Projectile.Kill();

        Projectile.direction = Projectile.Center.X > player.Center.X ? 1 : -1;

        player.heldProj = Projectile.whoAmI;
        player.ChangeDir(Projectile.direction);
        Projectile.spriteDirection = Projectile.direction;

        Projectile.velocity += (player.DirectionTo(Main.MouseWorld) - Projectile.velocity) * 0.2f;
        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
        Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, false, true) + Projectile.velocity * 45f;
        Projectile.rotation = Projectile.velocity.ToRotation();

        player.itemRotation = Projectile.rotation;

        if (player.channel)
            player.SetDummyItemTime(10);

        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2 + MathHelper.PiOver4 * player.direction);
        player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation * player.gravDir - MathHelper.PiOver2);

        Lighting.AddLight(Projectile.Center, new Vector3(0, 1, 0) * 0.1f);
        shakeOffset = Main.rand.NextVector2Circular(1, 1) * 2;

        if (!SoundEngine.TryGetActiveSound(soundSlot, out _))
        {
            var tracker = new ProjectileAudioTracker(Projectile);

            soundSlot = SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CoreScorcherLoop")
            {
                IsLooped = true,
                Pitch = -5,
                PauseBehavior = PauseBehavior.PauseWithGame,
                SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            }, Projectile.Center, soundInstance =>
            {
                soundInstance.Position = Projectile.Center;

                bool shouldBeActive = tracker.IsActiveAndInGame() && player.active && Projectile.active;

                if (!shouldBeActive)
                    dying = true;

                if (dying)
                    fade = MathHelper.Lerp(fade, 0f, 0.08f);
                else
                    fade = MathHelper.Lerp(fade, 1f, 0.08f);

                soundInstance.Volume = fade;
                soundInstance.Pitch = MathHelper.Lerp(soundInstance.Pitch, dying ? -1f : 0f, 0.1f);

                return fade > 0.01f;
            });
        }

        if (++Projectile.ai[0] >= 3)
        {
            player.CheckMana(2, true);

            Projectile.ai[0] = 0;


            Vector2 position = player.Center;

            if (Collision.CanHit(position, 16, 16, Projectile.Center + Projectile.velocity * 45f, 16, 16))
                position = Projectile.Center + Projectile.velocity * 45f;

            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), position, Projectile.velocity.RotatedByRandom(0.2f) * 20, ModContent.ProjectileType<CoreFlame>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
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

        texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

        color = Projectile.GetAlpha(Color.White);

        Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

        return false;
    }
}
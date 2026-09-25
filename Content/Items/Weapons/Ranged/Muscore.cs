using Divergency.Content.Particles;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.UI;

namespace Divergency.Content.Items.Weapons.Ranged;

public class Muscore : ModItem, IReloadWeapon
{
    int maxShotsLeft = 6;
    int shotsLeft = 6;

    public string BulletTexture => "Divergency/Common/UI/Reload_Muscore";

    public int StackSize => 1;

    public int GetRemainingBullets() => shotsLeft;
    public int GetMaxBullets() => maxShotsLeft;

    public void Reload() => shotsLeft = maxShotsLeft;

    public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.Size = new(16);
        Item.damage = 50;
        Item.knockBack = 3f;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = Item.useAnimation = 50;
        Item.autoReuse = true;
        Item.noUseGraphic = true;
        Item.shootSpeed = 10f;
        Item.shoot = ModContent.ProjectileType<MuscorePro>();

        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;

        Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Items/MuscoreShoot") with { PitchVariance = 0.1f };
    }

    public override bool CanUseItem(Player player)
    {
        if (shotsLeft > 0)
            return base.CanUseItem(player);
        else
            player.GetModPlayer<ReloadWeapon>().TryReload(player);

        return false;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        shotsLeft--;
        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<MuscoreBullet>(), damage, 1, player.whoAmI);
        return true;
    }
}

public class MuscoreBullet : ModProjectile
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Projectile.Size = new(32);
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = 3;
        Projectile.MaxUpdates = 15;
        Projectile.timeLeft = 20 * Projectile.MaxUpdates;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.hide = true;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.ai[0]++;

        if (Projectile.ai[0] > 7f && Projectile.numUpdates % 3 == 0)
        {
            float scale = MathHelper.Clamp(Projectile.ai[0] * 0.005f, 0f, 2f);

            ParticleManager.NewParticle<GreenSpark>(Projectile.Center, Projectile.velocity, new Color(0, 255, 0), 3f + scale);
            ParticleManager.NewParticle<GreenSpark>(Projectile.Center, Projectile.velocity, new Color(128, 255, 0), 1.5f + scale);
        }

        else if (Projectile.ai[0] == 7f)
        {
            for (int i = 0; i <= 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Terra, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(2f), 0, default, 1.5f).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.CoralTorch, Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(3f), 0, default, 1.5f).noGravity = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        for (int i = 0; i < 5; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Terra, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 2.1f), 0, default, 1.5f).noGravity = true;
            Dust.NewDustPerfect(Projectile.Center, DustID.CoralTorch, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 2.1f), 0, default, 1.5f).noGravity = true;
        }
    }
}

/*public class Muscore : ModItem, IReloadWeapon
{
    int maxShotsLeft = 6;
    int shotsLeft = 6;

    public string BulletTexture => "Divergency/Common/UI/Reload_Muscore";

    public int StackSize => 1;

    public int GetRemainingBullets() => shotsLeft;
    public int GetMaxBullets() => maxShotsLeft;

    public void Reload() => shotsLeft = maxShotsLeft;

    public override void SetStaticDefaults()
    {

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

public class MuscoreProj : ModProjectile
{
    int shotsLeft = 6;

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

                Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity * 80f, ModContent.DustType<Glow>(), velocity * 6f, 0, Color.LimeGreen, 0.8f);
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
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.velocity * 15, ModContent.ProjectileType<MuscoreBullet>(), Projectile.damage, 2, Projectile.owner, Projectile.whoAmI);
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
            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;

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

                Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity * 80f, ModContent.DustType<Glow>(), velocity * 2f, 0, Color.LimeGreen, 0.5f);


            }
            timer = 2;
        }





    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
     
        target.GetGlobalNPC<MuscoreBulletNPC>().hitBullets++;
        target.GetGlobalNPC<MuscoreBulletNPC>().timer = 180;

        for (int j = 0; j < 5; j++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

            Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity, ModContent.DustType<Glow>(), velocity * 2f, 0, Color.LimeGreen, 1f);


        }

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

            Dust dust = Dust.NewDustPerfect(npc.Center + velocity * 80f, ModContent.DustType<Glow>(), velocity * 6f, 0, Color.LimeGreen, 0.6f);


        }

        if (hitBullets == 3)
        {
            hitBullets = 0;

            Player player = Main.LocalPlayer;
            CameraSystem.ScreenShake(12);

            DivergencyDraw.SpawnExplosion(npc.Center, Color.LimeGreen, DustID.TerraBlade, 0);
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(npc.Center + velocity * 80f, ModContent.DustType<Glow>(), velocity * 6f, 0, Color.LimeGreen, 0.6f);
            }


            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, Vector2.Zero, ModContent.ProjectileType<LivingExplosion>(), 200    ,
                5, Main.myPlayer);

            int numberDust = 5;

            for (int i = 0; i < numberDust; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(npc.Center + velocity * 80f, ModContent.DustType<Glow>(), velocity * 6f, 0, Color.LimeGreen, 0.6f);
            }
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(npc.Center, ModContent.DustType<Smoke>(), Main.rand.NextVector2CircularEdge(1f, 1f) * 5, 0, Color.LimeGreen, 3f);
                Dust.NewDustPerfect(npc.Center, ModContent.DustType<Smoke>(), npc.velocity.SafeNormalize(Vector2.One) * Main.rand.NextFloat(-1f, -4f), 0, Color.LimeGreen, 2f);

            }
        }
        return base.PreAI(npc);
    }
}*/

public class ReloadWeapon : ModPlayer
{
    public Item itemReloading = null;
    public int secondsToReload = 1;
    public int timeTillReload = 2;

    public int bulletsOnReload = 0;

    public bool canShoot()
    {
        return itemReloading == null;
    }

    public override void PreUpdate()
    {
        if (itemReloading == null || Player.HeldItem.ModItem == null)
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
    int GetMaxBullets();
    string BulletTexture { get; }
    int StackSize { get; }
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
                delegate
                {
                    UIDraw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }

        Console.WriteLine("START");
        foreach (var layer in layers)
        {
            Console.WriteLine(layer.Name);
        }
        Console.WriteLine("END");
    }

    private static int prevBulletsLeft = -1;
    private static float[] animTimers = Array.Empty<float>();

    private static void UIDraw(SpriteBatch spriteBatch)
    {
        ReloadWeapon modPlr = Main.LocalPlayer.GetModPlayer<ReloadWeapon>();
        IReloadWeapon iReloadWeapon = Main.LocalPlayer.HeldItem.ModItem as IReloadWeapon;

        Texture2D fullTexture = (Texture2D)ModContent.Request<Texture2D>(iReloadWeapon.BulletTexture + "_Full");
        Texture2D emptyTexture = (Texture2D)ModContent.Request<Texture2D>(iReloadWeapon.BulletTexture + "_Empty");

        int bulletsLeft = iReloadWeapon.GetRemainingBullets();
        int maxBullets = iReloadWeapon.GetMaxBullets();

        // Ensure animTimers has correct length
        if (animTimers.Length != maxBullets)
            animTimers = new float[maxBullets];

        // Detect changes (bullet lost or gained)
        if (prevBulletsLeft != -1 && prevBulletsLeft != bulletsLeft)
        {
            // Losing bullets
            if (bulletsLeft < prevBulletsLeft)
            {
            }
            // Gaining bullets
            else
            {
            }
        }

        prevBulletsLeft = bulletsLeft;

        int spacingX = 16;
        int spacingY = 7;
        float scale = Main.UIScale;

        for (int i = 0; i < maxBullets; i++)
        {
            Texture2D stockTexture = (i + 1 > bulletsLeft ? emptyTexture : fullTexture);
            Rectangle bulletRect = new(0, 0, stockTexture.Width, stockTexture.Height);
            Vector2 origin = bulletRect.Size() * 0.5f;

            // total width of the row
            float totalWidth = maxBullets * spacingX;

            // start X so the row is centered
            float startX = (Main.screenWidth * 0.5f) - (totalWidth * 0.5f);

            float x = startX + i * spacingX + origin.X * scale;
            float y = (Main.screenHeight * 0.5f) - (spacingY * 0.5f);

            Vector2 position = new(x, 30 + y + spacingY + origin.Y * scale);
            position.Y += MathF.Sin(i + Main.GameUpdateCount * 0.05f) * 2f;

            // Animation scaling
            float anim = animTimers[i];
            if (anim > 0f)
            {
                // Pulse effect: scale up then down
                float pulse = 1f + MathF.Sin((0.3f - anim) * MathF.PI * 3f) * 0.25f;
                spriteBatch.Draw(
                    stockTexture,
                    position,
                    bulletRect,
                    Color.White,
                    0f,
                    origin,
                    scale * pulse,
                    SpriteEffects.None,
                    0f
                );

                animTimers[i] -= 1f / 60f; // assuming 60 FPS
                if (animTimers[i] < 0f)
                    animTimers[i] = 0f;
            }
            else
            {
                // Normal draw
                spriteBatch.Draw(
                    stockTexture,
                    position,
                    bulletRect,
                    Color.White,
                    0f,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}

public class Corrage : ModItem, IReloadWeapon
{
    int maxShotsLeft = 6 * 3;
    int shotsLeft = 6 * 3;

    public string BulletTexture => "Divergency/Common/UI/Reload_Muscore";

    public int StackSize => 1;

    public int GetRemainingBullets() => shotsLeft;
    public int GetMaxBullets() => maxShotsLeft;

    public void Reload() => shotsLeft = maxShotsLeft;

    public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.Size = new(16);
        Item.damage = 20;
        Item.knockBack = 3f;
        Item.DamageType = DamageClass.Ranged;

        Item.useTime = 5;
        Item.useAnimation = 30;
        Item.reuseDelay = 30;
        Item.useLimitPerAnimation = 6;
        Item.autoReuse = true;

        Item.shootSpeed = 10f;
        Item.shoot = ModContent.ProjectileType<MuscoreBullet>();

        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
    }

    public override bool CanUseItem(Player player)
    {
        if (shotsLeft > 0)
            return base.CanUseItem(player);
        else
            player.GetModPlayer<ReloadWeapon>().TryReload(player);

        return false;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        shotsLeft--;
        SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/MuscoreShoot") with { PitchVariance = 0.1f }, player.Center);
        return true;
    }
}
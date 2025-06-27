using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class HeavyMetal : ModItem
    {
        //public override bool AltFunctionUse(Player player) => true;

        //float ScaleEase(float cur, float max)
        //{
        //    float x = cur / max;
        //    return 1f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(1 - x) * MathHelper.Pi) * 0.6f * 0.6f;
        //}

        //float RotationEase(float cur, float max)
        //{
        //    float x = cur / max;
        //    return EaseFunction.EaseCircularInOut.Ease(x);
        //}

        //private int freezeFrames = -1;
        //void NPCHit(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        //{
        //    Player player = Main.player[projectile.owner];

        //    player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 7;

        //    if (Main.rand.NextBool(10))
        //    {
        //        target.AddBuff(BuffID.Confused, 600);
        //    }

        //    SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

        //    for (int i = 0; i < 20; i++)
        //    {
        //       // Dust.NewDust(target.position, target.width, target.height, DustID.Blood, target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, default, 2f);
        //    }
        //    projectile.damage = 0;

        //    if (freezeFrames == -1)
        //        freezeFrames = projectile.localNPCHitCooldown * 2;
        //}

        //private void Update(Projectile projectile)
        //{
        //    if (freezeFrames > -1)
        //    {
        //        freezeFrames--;

        //        if (freezeFrames > 0)
        //        {
        //            SwordProjectile proj = (projectile.ModProjectile as SwordProjectile);
        //            ISwordSwing SwingInfo = ModContent.GetModItem(proj.baseItem) as ISwordSwing;
        //            proj.FramesPassed-=1f / SwingInfo.Updates;
        //        }
        //    }
        //}

        //public int attackDirection = 1;
        //public int AttackCounter = 1;

        //public int Updates => 5;
        //public Action<Projectile, NPC, int, float, bool> OnHitNPC => NPCHit;
        //public string SwordTexture => "Divergency/Content/Items/Weapons/Melee/HeavyMetal";
        //public Vector2 Pivot => new Vector2(0, 30);

        //public TimedFunction[] SwingFunctions => new TimedFunction[]
        //{
        //    new TimedFunction(Update, 0, RunEveryFrame: true),
        //};

        //static void PlaySound(Projectile proj)
        //{
        //    Player player = Main.player[proj.owner];
        //    SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingHeavy") with { Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, player.Center);
        //}

        //private static TimedFunction[] timedFunctions = new TimedFunction[] { new TimedFunction(PlaySound, 0.5f) };
        //public Keyframes SwordFrames => new Keyframes(new SwordAnimation[]
        //{
        //    new SwordAnimation(-2f+MathF.PI/2, 0), // TimedFunction should be in here, not down below...
        //    new SwordAnimation(2f+MathF.PI/2, 80, 4, Flipped: true,FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
        //});
        
        //public float Width => MathF.Sqrt(MathF.Pow(10, 2)*2)+1f; // 12 is vertical width of blade

        //public SwordTrail[] SwordTrails => new SwordTrail[]
        //{
        //    new SwordTrail("Divergency/Assets/Textures/Trails/Stretched", 1),
        //};

        

        public override void SetStaticDefaults()
        {
            //.setdefault("Commandant's Blade");
            ////.setdefault($"Inflicts Flesh Wound [i:{ModContent.ItemType<FleshWoundIcon>()}]");
            

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.damage = 40;
            Item.knockBack = 12f;

            Item.shootSpeed = 20f;

            Item.shoot = ModContent.ProjectileType<MetalPipe>();
            Item.width = Item.height = 90;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void HoldItem(Player player)
        {
            if (player == Main.LocalPlayer)
            {
                if (player.ItemAnimationActive) { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation - MathHelper.PiOver2 * player.direction); }
                else { player.SetCompositeArmFront(false, default, default); }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 0f);

            return false;
        }
        public override bool CanUseItem(Player player)
        {
         
            return player.ownedProjectileCounts[Item.shoot] < 3;
        }
    }
    public class MetalPipe : ModProjectile
    {

        public override string Texture => "Divergency/Content/Items/Weapons/Melee/HeavyMetal";
        int timer = 0;
        bool collided;
        int counter;
            public override void SetStaticDefaults()
            {
                ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4; // The length of old position to be recorded
                ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
            }

            public override void SetDefaults()
            {
                Projectile.penetrate = 10;
                Projectile.DamageType = DamageClass.Ranged;
                Projectile.friendly = true;
                Projectile.hostile = false;

                Projectile.Size = new Vector2(30);
                Projectile.scale = 1f;
                Projectile.tileCollide = true;
                Projectile.ignoreWater = false;
                Projectile.timeLeft = 1200;
                Projectile.aiStyle = -1;
               
            }
            public override void AI()
            {
            timer++;
            if (counter == 2)
            {
                Projectile.Kill();
            }
            Projectile.velocity.Y += 0.2f;
            if (collided)
            {
                Projectile.rotation += Projectile.velocity.Length() * (Projectile.direction * 0.02f);

            }
            else
            {
               
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);

            }
  
            }
            public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
            {
                fallThrough = false;
                return true;
            }

            public override bool OnTileCollide(Vector2 oldVelocity)
            {  SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/metalpipe") { Volume = 1.5f, Pitch = Main.rand.NextFloat(-0.3f, 0.3f), MaxInstances = 99 }, Projectile.Center);
            collided = true;
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) >= float.Epsilon) { Projectile.velocity.X = -oldVelocity.X * 0.7f; }
            Projectile.penetrate--;

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) >= float.Epsilon) { Projectile.velocity.Y = -oldVelocity.Y * 0.5f; }
                return false;
            }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.penetrate--;
            //counter++;
            target.AddBuff(BuffID.Confused, 60);
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/metalpipe") { Volume = 3f, MaxInstances = 99 }, Projectile.Center) ;
            Projectile.velocity.X *=  -0.9f;
            Projectile.velocity.Y *= -1.3f;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {

                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
    

    
}
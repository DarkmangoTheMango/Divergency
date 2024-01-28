using Divergency.Common.Helpers;
using Divergency.Common.Helpers.SwordAnimator;
using Divergency.Common.Players;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Divergency.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class Sacresti : ModItem
    {

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {
            //.setdefault("Commandant's Blade");
            ////.setdefault($"Inflicts Flesh Wound [i:{ModContent.ItemType<FleshWoundIcon>()}]");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 50;
            Item.knockBack = 5f;

            Item.shoot = ModContent.ProjectileType<SacrestiSwing>();
            Item.shootSpeed = 1f;

            Item.width = Item.height = 90;
            Item.scale = 0.6f;

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
            Console.WriteLine("Made proj");
            SwordAnimator.Swing<SacrestiSwing>(player, damage, knockback);

            return false;
        }
    }

    public class SacrestiSwing : SwordSwing
    {
        float ScaleEase(float cur, float max)
        {
            float x = cur / max;
            return 0.666f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(1 - x) * MathHelper.Pi) * 0.6f * 0.6f;
        }

        float RotationEase(float cur, float max)
        {
            float x = cur / max;
            return EaseFunction.EaseCircularInOut.Ease(x);
        }
        bool knockbacked = false;

        private int freezeFrames = -1;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 1;

            if (currentCharge >= 60 && !knockbacked)
            {

                player.velocity.X *= -0.5f;
                player.velocity.Y *= -0.5f;

                knockbacked = true;
                projectile.damage = 0;
                target.velocity.Y -= 10;
            }

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 5f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 5f), 0, Color.DarkRed, 1f);
                ParticleManager.NewParticle<Spark>(target.Center, target.DirectionTo(player.Center) * -Main.rand.NextFloat(2f, 9f), new Color(255, 0, 0, 0), 1f, 1);

            }

            if (freezeFrames == -1)
                freezeFrames = 1;
        }


        public float currentCharge;
        private void Update(Projectile projectile)
        {
            timer++;
            Player player = Main.player[projectile.owner];
            if (freezeFrames > -1)
            {
                freezeFrames--;

                if (freezeFrames > 0)
                {
                    SwordSwing proj = (projectile.ModProjectile as SwordSwing);
                    proj.framesPassed -= 1f / proj.Updates;
                }





            }

            SwordSwing swing = (projectile.ModProjectile as SwordSwing);

            if (swing.Charge > 60 && swing.Charge < 90)
            {
                ParticleManager.NewParticle<Spark>(projectile.Center, (projectile.DirectionTo(player.Center) * -Main.rand.NextFloat(2f, swing.Charge / 4) * Main.rand.NextVector2Circular(2, 2)), new Color(255, 0, 0, 0), 0.4f, 1);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Glow>(), projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(10f, swing.Charge / 3), projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(10f, swing.Charge / 4), 0, Color.DarkRed, 0.4f);

            }
            if (swing.Charge >= 90)
            {
                ParticleManager.NewParticle<Spark>(projectile.Center, projectile.DirectionTo(player.Center) * -Main.rand.NextFloat(2, 2), new Color(255, 0, 0, 0), 1f, 1);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Glow>(), projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(1f, 2) * 0.8f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(0.8f - 0.2f) * MathHelper.Pi) * 0.5f * 0.5f, projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(2, 2), 0, Color.DarkRed, 1f);

            }
            if (swing.Charge == 90 && !dashed)
            {
                player.velocity += player.DirectionTo(Main.MouseWorld) * 15;
                player.SetImmuneTimeForAllTypes(75);

                dashed = true;
            }



            currentCharge = swing.Charge;


        }

        public int chargeEnder = 0;
        public int attackDirection = 1;
        public int AttackCounter = 1;
        public override int Updates => 10;
        public override string SwordTexture => "Divergency/Content/Items/Weapons/Melee/Sacresti";
        public override Vector2 Pivot => new Vector2(0, 60);
        public override float BuildInRotation => MathF.PI / 8;
        public override TimedFunction[] SwingFunctions => new TimedFunction[]
        {
            new TimedFunction(Update, 0, RunEveryFrame: true),
        };
        static void PlaySound(Projectile proj)
        {
            Player player = Main.player[proj.owner];
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingHeavy") with { Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, player.Center);
        }
        static void Dash(Projectile proj)
        {
            Player player = Main.player[proj.owner];
            //SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingHeavy") with { Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, player.Center);
            player.velocity.X += player.direction * 10;

        }

        private static TimedFunction[] timedFunctions = new TimedFunction[] { new TimedFunction(PlaySound, 0.5f) };

        public override Keyframes SwordFrames => new Keyframes(new SwordAnimation[]
        {
           new SwordAnimation(0,0),
           new SwordAnimation(2f+MathF.PI/2, 20, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(-2f+MathF.PI/2, 20, 4, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase, Flipped: false),
           new SwordAnimation(MathF.PI* 2f, 40, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(-MathF.PI* 2f, 40, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(5+MathF.PI* 2f, 40, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase, Flipped: false),
           new SwordAnimation(-2+MathF.PI/4, 25, FrameFunctions: timedFunctions,  ChargeAutoRelease: true, MaxCharge: 90f, RotationIn: RotationEase, ScaleMul: ScaleEase),




        });
        public override float Width => MathF.Sqrt(MathF.Pow(10, 2) * 2) + 1f; // 12 is vertical width of blade

        public override SwordTrail SwordTrail => new SwordTrail("Divergency/Assets/Textures/RedTrail", 147, TrailType.Sqrt);

        public override SwordGlow[] Glows => new SwordGlow[]
         {

          new SwordGlow(new SwordGlowColor(
                new List<Color>{
                    new Color(219, 112, 147,  currentCharge / 100),
                    new Color(219, 112, 147, currentCharge / 100),
                    new Color(219, 112, 147, currentCharge / 100)

                }, new List<int>
                {
                    0,
                    1,
                    1,

                }),

            1f, false, "Divergency/Content/Items/Weapons/Melee/SacrestiGlowmask")


           };

        public bool dashed { get; private set; }
        public int timer { get; private set; }
        public bool defensive { get; private set; }
    }
}
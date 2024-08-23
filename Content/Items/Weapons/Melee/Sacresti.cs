using Divergency.Common.Helpers;
using Divergency.Common.Helpers.SwordAnimator;
using Divergency.Common.Players;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Weapons.LivingCore;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles;
using Divergency.Content.Projectiles.Melee;
using Microsoft.CodeAnalysis;
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
using static Humanizer.In;

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
        public override bool AltFunctionUse(Player player)
        {
            return true;
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
            Item.autoReuse = false;
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
            if (Main.mouseLeft)
            SwordAnimator.Swing<SacrestiSwingEnflamed>(player, damage, knockback);
            if (Main.mouseRight)
            SwordAnimator.Swing<SacrestiSwingEnflameAnim>(player, damage, knockback);



            return false;
        }
    }

    public class SacrestiSwingEnflameAnim : SwordSwing
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

        private int freezeFrames = -1;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 3;

            

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 5f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 5f), 0, Color.DarkRed, 1f);
                ParticleManager.NewParticle<Spark>(target.Center, target.DirectionTo(player.Center) * -Main.rand.NextFloat(2f, 9f), new Color(255, 0, 0, 0), 1f, 1);

            }

            if (freezeFrames == -1)
                freezeFrames = 120;
        }


        public float currentCharge;
        private void Update(Projectile projectile)
        {
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

            if (swing.charge > 60 && swing.charge < 90)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Glow>(), projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(10f, swing.charge / 5), projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(10f, swing.charge / 4), 0, Color.DarkRed, 0.5f);

            }
            if (swing.charge >= 90 && freezeFrames <= 0)
            {
                Dust.NewDustPerfect(projectile.position+Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(40f, 90f),DustID.GemRuby,new Vector2(projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(4f, 8) * 0.8f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(0.8f - 0.2f) * MathHelper.Pi) * 0.5f * 0.5f, projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(4, 7)), 0, default, 2f).noGravity = true;

                Dust.NewDust(projectile.position + new Vector2(Main.rand.NextFloat(10)), projectile.width, projectile.height, ModContent.DustType<Glow>(), projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(1f, 2) * 0.8f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(0.8f - 0.2f) * MathHelper.Pi) * 0.5f * 0.5f, projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(2, 2), 0, Color.DarkRed, 1f);

            }




            currentCharge = swing.charge;


        }

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
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/FireHit") with { Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, player.Center);
        }


        private static TimedFunction[] timedFunctions = new TimedFunction[] { new TimedFunction(PlaySound, 0.5f) };

        public override Keyframes SwordFrames => new Keyframes(new SwordAnimation[]
        {
           new SwordAnimation(0,0),
           new SwordAnimation(2f+MathF.PI/2, 15, FrameFunctions: timedFunctions, ChargeAutoRelease: true, MaxCharge: 90f, RotationIn: RotationEase, ScaleMul: ScaleEase),




        });
        public override float Width => MathF.Sqrt(MathF.Pow(10, 2) * 2) + 1f; // 12 is vertical width of blade

        public override SwordTrail SwordTrail => new SwordTrail("Divergency/Assets/Textures/NormalTrail", 147, TrailType.Sqrt);

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

            1f, false, "Divergency/Content/Items/Weapons/Melee/SacrestiGlowmask"),



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

            1f, true, "Divergency/Content/Items/Weapons/Melee/SacrestiProjEnflamed"),
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

            1f, true, "Divergency/Content/Items/Weapons/Melee/SacrestiProjEnflamed"),
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

            1f, true, "Divergency/Content/Items/Weapons/Melee/SacrestiProjEnflamed"),


           };

        public bool dashed { get; private set; }
        public int timer { get; private set; }
    }


    //ENFLAMED __________________________
        //                               |
          //                             v


    public class SacrestiSwingEnflamed : SwordSwing
    {
        public bool Dash2;

        public bool CooldownTimerActivate { get; private set; }

        float ScaleEase(float cur, float max)
        {
            float x = cur / max;
            return 0.7f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(1 - x) * MathHelper.Pi) * 0.4f * 0.4f;
        }

        float RotationEase(float cur, float max)
        {
            float x = cur / max;
            return EaseFunction.EaseCircularInOut.Ease(x);
        }

        private int freezeFrames = -1;
       
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 4;
            player.SetImmuneTimeForAllTypes(60);
  
            bool knockbacked = false;
            if (currentCharge >= 60 && !knockbacked)
            {

                knockbacked = true;
                player.velocity.X = 0;
                target.velocity.Y -= 12;
                
                if (!ChargeReset)
                { 
                    ChargeReset = true;
                    dashed = false;
                    Dash2 = true;
                    CooldownTimerActivate = true;
                }
            }
            if (Dash2 && CooldownTimer >= 60)
            {
                player.velocity = new Vector2(0,0);
                target.velocity = new Vector2(0, 0);

                target.velocity.Y -= 7;
                target.velocity.X = 0;
                player.velocity.Y -= 5;

            }
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/FireHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);


            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 20f), 0, Color.DarkRed, 1f);
                Dust.NewDust(target.position, target.width, target.height, DustID.GemRuby, target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 20f), 0, default, 1f);

            }

            if (freezeFrames == -1 && !Dash2)
            {
                freezeFrames = 75;
            
            }
            else
            {
                freezeFrames = 0;

            }
        }


        public float currentCharge;
        private void Update(Projectile projectile)
        {
            if (CooldownTimerActivate)
            {
                CooldownTimer++;
            }
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


            if (ChargeReset)
            {
                ChargeReset = false;
                swing.charge = 0;
            }
            if (swing.charge > 30 && swing.charge < 60)
            {
                ParticleManager.NewParticle<Spark>(projectile.Center, (projectile.DirectionTo(player.Center) * -Main.rand.NextFloat(2f, swing.charge / 4) * Main.rand.NextVector2Circular(2, 2)), new Color(255, 0, 0, 0), 0.4f, 1);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Glow>(), projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(10f, swing.charge / 3), projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(10f, swing.charge / 4), 0, Color.DarkRed, 0.4f);

            }
            if (swing.charge >= 30)
            {
                if (Main.rand.NextBool(15))
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<SmokeIncendiary>(), projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(5f, 10) * 0.8f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(0.8f - 0.2f) * MathHelper.Pi) * 0.5f * 0.5f, projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(5, 10), 0, Color.DarkRed, 1f);

            }

            if (swing.charge == 60 && !dashed)
            {
                player.velocity += player.DirectionTo(Main.MouseWorld) * 25;
                player.SetImmuneTimeForAllTypes(90);
                dashed = true;
            }

            if (freezeFrames <= 0 && Main.rand.NextBool(10))
            {
                //Dust.NewDustPerfect(projectile.position + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(40f, 90f), DustID.GemRuby, new Vector2(projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(4f, 8) * 0.8f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(0.8f - 0.2f) * MathHelper.Pi) * 0.5f * 0.5f, projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(4, 7)), 0, default, 2f).noGravity = true;

                Dust.NewDust(projectile.position + new Vector2(Main.rand.NextFloat(10)), projectile.width, projectile.height, ModContent.DustType<Glow>(), projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(1f, 2) * 0.8f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(0.8f - 0.2f) * MathHelper.Pi) * 0.5f * 0.5f, projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(2, 2), 0, Color.DarkRed, 1f);
                swing.SpawnDust(new Vector2(0, 0.30f + Main.rand.NextFloat() * 0.5f), DustID.GemRuby, new Vector2(0f, 2f).RotatedBy(Projectile.rotation) * -player.direction, 255, default, 2);

            }



            currentCharge = swing.charge;


        }

        public int attackDirection = 1;
        public int AttackCounter = 1;
        public override int Updates => 10;
        public override string SwordTexture => "Divergency/Content/Items/Weapons/Melee/SacrestiEnflamed";
        public override Vector2 Pivot => new Vector2(0, 60);
        public override float BuildInRotation => MathF.PI / 8;
       
        public override TimedFunction[] SwingFunctions => new TimedFunction[]
        {
            new TimedFunction(Update, 0, RunEveryFrame: true),
        };
        static void PlaySound(Projectile proj)
        {
            Player player = Main.player[proj.owner];
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingStyleSaraishi") with { Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, player.Center);
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
           new SwordAnimation(2f+MathF.PI/2, 15, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(-2f+MathF.PI/2, 15, 4, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase, Flipped: false),
           new SwordAnimation(MathF.PI* 2f, 30, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(-MathF.PI* 2f, 30, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(5+MathF.PI* 2f, 30, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase, Flipped: false),
           new SwordAnimation(-2+MathF.PI/4, 20, FrameFunctions: timedFunctions,  ChargeAutoRelease: true, MaxCharge: 60f, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(2+MathF.PI/4, 20, FrameFunctions: timedFunctions,  ChargeAutoRelease: true, MaxCharge: 60f, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(-2+MathF.PI/4, 20, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(2+MathF.PI/4, 10, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(-2+MathF.PI/4, 10, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),








        });
        public override float Width => MathF.Sqrt(MathF.Pow(10, 2) * 2) + 1f; // 12 is vertical width of blade

        public override SwordTrail SwordTrail => new SwordTrail("Divergency/Assets/Textures/NormalTrail", 147, TrailType.Sqrt);
        
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

            1f, false, "Divergency/Content/Items/Weapons/Melee/SacrestiGlowmask"),

          new SwordGlow(new SwordGlowColor(
                new List<Color>{
                    new Color(219, 112, 147,  255),
                    new Color(219, 112, 147, 255),
                    new Color(219, 112, 147, 255)

                }, new List<int>
                {
                    0,
                    1,
                    1,

                }),

            1f, true, "Divergency/Content/Items/Weapons/Melee/SacrestiEnflamed")


           };

        public bool dashed { get; private set; }
        public int timer { get; private set; }
        public bool ChargeReset { get; private set; }
        public int CooldownTimer { get; private set; }
    }



    // NOOOOOOOOOORMAL 



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

        private int freezeFrames = -1;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 1;
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, new Vector2(7, 10).RotateRandom(3), ModContent.ProjectileType<SacrifeBallThing>(), 0, 0, Projectile.owner); //spawn gauge energy

            }
            bool knockbacked = false;
            if (currentCharge >= 60 && !knockbacked)
            {
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, new Vector2(7, 10).RotateRandom(3), ModContent.ProjectileType<SacrifeBallThing>(), 0, 0, Projectile.owner); //spawn gauge energy

                }
                player.velocity.X *= -0.5f;
                player.velocity.Y *= -0.5f; knockbacked = true;
                Projectile.damage = 0;
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

            if (swing.charge > 60 && swing.charge < 90)
            {
                ParticleManager.NewParticle<Spark>(projectile.Center, (projectile.DirectionTo(player.Center) * -Main.rand.NextFloat(2f, swing.charge / 4) * Main.rand.NextVector2Circular(2, 2)), new Color(255, 0, 0, 0), 0.4f, 1);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Glow>(), projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(10f, swing.charge / 3), projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(10f, swing.charge / 4), 0, Color.DarkRed, 0.4f);

            }
            if (swing.charge >= 90)
            {
                ParticleManager.NewParticle<Spark>(projectile.Center, projectile.DirectionTo(player.Center) * -Main.rand.NextFloat(2, 2), new Color(255, 0, 0, 0), 1f, 1);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Glow>(), projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(1f, 2) * 0.8f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(0.8f - 0.2f) * MathHelper.Pi) * 0.5f * 0.5f, projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(2, 2), 0, Color.DarkRed, 1f);

            }
            if (swing.charge == 90 && !dashed)
            {
                player.velocity += player.DirectionTo(Main.MouseWorld) * 15;
                player.SetImmuneTimeForAllTypes(75);

                dashed = true;
            }



            currentCharge = swing.charge;


        }

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

        public override SwordTrail SwordTrail => new SwordTrail("Divergency/Assets/Textures/NormalTrail", 147, TrailType.Sqrt);

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
    }
}
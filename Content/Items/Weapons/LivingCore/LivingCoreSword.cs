using Divergency.Common.Helpers;
using Divergency.Common.Helpers.SwordAnimator;
using Divergency.Common.Players;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.LivingCore
{
    public class LivingCoreSword : ModItem
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
            Item.damage = 35;
            Item.knockBack = 5f;

            Item.shoot = ModContent.ProjectileType<LivingCoreSwordSwing>();
            Item.shootSpeed = 1f;

            Item.width = Item.height = 90;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 15   ;
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
            if (Main.mouseLeft)
                SwordAnimator.Swing<LivingCoreSwordSwing>(player, damage, knockback);
            if (Main.mouseRight)
                SwordAnimator.Swing<LivingCoreSwordSwing2>(player, damage, knockback);

            return false;
        }
    }

    public class LivingCoreSwordSwing : SwordSwing
    {
        public override float BuildInRotation => 0;

        float ScaleEase(float cur, float max)
        {
            float x = cur / max;
            return 1f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(1 - x) * MathHelper.Pi) * 0.5f * 0.5f;
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

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 2;



            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, Color.LimeGreen, 1f);
                Dust.NewDust(target.position, target.width, target.height, DustID.GemEmerald, target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, default, 1f);

            }

            if (freezeFrames == -1)
                freezeFrames = 40;
        }

       

        private void Update(Projectile projectile)
        {
            if (freezeFrames > -1)
            {
                freezeFrames--;

                if (freezeFrames > 0)
                {
                    SwordSwing proj = (projectile.ModProjectile as SwordSwing);
                    proj.framesPassed -= 1f / proj.Updates;
                }
            }
        }

        public int attackDirection = 1;
        public int AttackCounter = 1;

        public override int Updates => 10;
        public override string SwordTexture => "Divergency/Content/Items/Weapons/LivingCore/LivingCoreSword";
        public override Vector2 Pivot => new Vector2(0, 55);

        public override TimedFunction[] SwingFunctions => new TimedFunction[]
        {
            new TimedFunction(Update, 0, RunEveryFrame: true),
        };

        static void PlaySound(Projectile proj)
        {
            Player player = Main.player[proj.owner];
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingHeavy") with { Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, player.Center);
        }

        private static TimedFunction[] timedFunctions = new TimedFunction[] { new TimedFunction(PlaySound, 0.5f) };

        public override Keyframes SwordFrames => new Keyframes(new SwordAnimation[]
        {
           new SwordAnimation(0,0),
           new SwordAnimation(2f+MathF.PI/2, 30, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase, HoldToContinue: true),
           new SwordAnimation(-2f+MathF.PI/2, 30, 4, FrameFunctions: timedFunctions, Flipped: true, RotationIn: RotationEase, ScaleMul: ScaleEase, HoldToContinue: true),
           new SwordAnimation(MathF.PI* 3f, 55, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase,HoldToContinue: true),
                    



        });
        public override float Width => MathF.Sqrt(MathF.Pow(12, 2) * 2) + 1f; // 12 is vertical width of blade

        public override SwordTrail SwordTrail => new SwordTrail("Divergency/Assets/Textures/TestTrail", 115, TrailType.Raw);
    }

    public class LivingCoreSwordSwing2 : SwordSwing
    {
        public override float BuildInRotation => 0;
        public int initialDamage;
        float ScaleEase(float cur, float max)
        {
            float x = cur / max;
            return 1f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(1 - x) * MathHelper.Pi) * 0.5f * 0.5f;
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

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 2;



            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, Color.LimeGreen, 1f);
                Dust.NewDust(target.position, target.width, target.height, DustID.GemEmerald, target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, default, 1f);

            }

            if (freezeFrames == -1)
                freezeFrames = 40;
        }



        private void Update(Projectile projectile)
        {
            Main.NewText(initialDamage);
            SwordSwing swing = (projectile.ModProjectile as SwordSwing);
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
            if (freezeFrames <= 0 && Main.rand.NextBool(10))
            {
                //Dust.NewDustPerfect(projectile.position + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(40f, 90f), DustID.GemRuby, new Vector2(projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(4f, 8) * 0.8f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(0.8f - 0.2f) * MathHelper.Pi) * 0.5f * 0.5f, projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(4, 7)), 0, default, 2f).noGravity = true;

                Dust.NewDust(projectile.position + new Vector2(Main.rand.NextFloat(10)), projectile.width, projectile.height, ModContent.DustType<GlowLine>(), projectile.DirectionTo(player.Center).X * -Main.rand.NextFloat(1f, 2) * 1.1f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(0.8f - 0.2f) * MathHelper.Pi) * 0.8f * 0.8f, projectile.DirectionTo(player.Center).Y * -Main.rand.NextFloat(2, 2), 0, Color.LimeGreen, 0.2f);

            }
            if (swing.charge == 60 && !dashed)
            {
                player.velocity += new Vector2(player.direction,0) * 7;
                player.SetImmuneTimeForAllTypes(15);
                dashed = true;
            }
            if (swing.charge >= 60)
            {
                stun = true;
                projectile.damage = initialDamage;
            }
             if (swing.charge >= 3 && swing.charge < 60)
            {
                projectile.damage = 0;
            }
            if (swing.charge == 2)
            { 
                initialDamage = player.HeldItem.damage; 
            }
        }

        public int attackDirection = 1;
        public int AttackCounter = 1;

        public override int Updates => 10;
        public override string SwordTexture => "Divergency/Content/Items/Weapons/LivingCore/LivingCoreSword";
        public override Vector2 Pivot => new Vector2(0, 56);

        public override TimedFunction[] SwingFunctions => new TimedFunction[]
        {
            new TimedFunction(Update, 0, RunEveryFrame: true),
        };

        static void PlaySound(Projectile proj)
        {
            Player player = Main.player[proj.owner];
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingHeavy") with { Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, player.Center);
        }

        private static TimedFunction[] timedFunctions = new TimedFunction[] { new TimedFunction(PlaySound, 0.5f) };
        private bool dashed;
        private bool stun;

        public override Keyframes SwordFrames => new Keyframes(new SwordAnimation[]
        {
            new SwordAnimation(MathF.PI/2, 0, 4, LocalOffset: new Vector2(0, -20), HoldToContinue: true),
            new SwordAnimation((MathF.PI/2), 10, LocalOffset: new Vector2(0, 20),RotationIn: RotationEase, ScaleMul: ScaleEase, ChargeAutoRelease: true, MaxCharge: 60f),
                        





        });
        public override float Width => MathF.Sqrt(MathF.Pow(12, 2) * 2) + 1f; // 12 is vertical width of blade

        public override SwordTrail SwordTrail => new SwordTrail("Divergency/Assets/Textures/TestTrail", 115, TrailType.Raw);
    }
}
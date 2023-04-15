using Divergency.Common.Helpers;
using Divergency.Common.Helpers.SwordAnimator;
using Divergency.Common.Players;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
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
    public class LivingCoreSword : ModItem, ISwordSwing
    {
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
        void NPCHit(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];

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

        bool OnHitTile2(Projectile projectile, Vector2 oldVelocity)
        {
            Player player = Main.player[projectile.owner];

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 2;



            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

            

            if (freezeFrames == -1)
                freezeFrames = 40;
            return false;
        }

        private void Update(Projectile projectile)
        {
            if (freezeFrames > -1)
            {
                freezeFrames--;

                if (freezeFrames > 0)
                {
                    SwordProjectile proj = (projectile.ModProjectile as SwordProjectile);
                    ISwordSwing SwingInfo = ModContent.GetModItem(proj.baseItem) as ISwordSwing;
                    proj.FramesPassed-=1f / SwingInfo.Updates;
                }
            }
        }

        public int attackDirection = 1;
        public int AttackCounter = 1;

        public int Updates => 10;
        public Action<Projectile, NPC, int, float, bool> OnHitNPC => NPCHit;
        public Func<Projectile, Vector2, bool> OnHitTile { get { return OnHitTile2; } }
        public string SwordTexture => "Divergency/Content/Items/Weapons/LivingCore/LivingCoreSword";
        public Vector2 Pivot => new Vector2(0, 55);

        public TimedFunction[] SwingFunctions => new TimedFunction[]
        {
            new TimedFunction(Update, 0, RunEveryFrame: true),
        };
        public SwordTrail[] SwordTrails => new SwordTrail[]
        {
            new SwordTrail("Divergency/Content/Items/Weapons/LivingCore/LivingCoreSword", 1),
        };
        static void PlaySound(Projectile proj)
        {
            Player player = Main.player[proj.owner];
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingHeavy") with { Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, player.Center);
        }

        private static TimedFunction[] timedFunctions = new TimedFunction[] { new TimedFunction(PlaySound, 0.5f) };

        public Keyframes SwordFrames => new Keyframes(new SwordAnimation[]
        {
           new SwordAnimation(0,0),
           new SwordAnimation(2f+MathF.PI/2, 30, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(-2f+MathF.PI/2, 30, 4, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(MathF.PI* 3f, 75, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
           new SwordAnimation(-MathF.PI,0),
           new SwordAnimation(2f+MathF.PI/2, 120, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),


        });
        public float Width => MathF.Sqrt(MathF.Pow(12, 2)*2)+1f; // 12 is vertical width of blade


        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Commandant's Blade");
            Tooltip.SetDefault($"Inflicts Flesh Wound [i:{ModContent.ItemType<FleshWoundIcon>()}]");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 35;
            Item.knockBack = 5f;

            Item.shootSpeed = 1f;

            Item.shoot = ModContent.ProjectileType<SwordProjectile>();
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
            attackDirection = -attackDirection;

            //Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attackDirection, 0f);

            SwordAnimator.Swing(player, Item.type, damage, knockback);

            return false;
        }
    }

    
}
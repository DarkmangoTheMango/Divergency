using Divergency.Content.Particles;
using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Common.Helpers.SwordAnimator;
using Divergency.Content.Dusts;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class DecodeDestruction : ModItem
    {
        public int attackDirection = 1;
        public int AttackCounter = 1;
        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
      
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }



        public override void SetStaticDefaults()
        {
            //.setdefault("Life Enforcer");
            ////.setdefault("Direct hits deploy orbs, right click in order to call them back to the player" +
                //"Orb daamage scales with your current health stat (not maximum)");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 50;
            Item.knockBack = 4f;

            Item.shoot = ModContent.ProjectileType<SwordProjectile>(); // this dosent actually have to be there at all...
            Item.shootSpeed = 1f;

            Item.width = Item.height = 96;
            Item.scale = 1.5f;

            Item.useTime = Item.useAnimation = 30;
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
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ProjectileID.None, damage, knockback, player.whoAmI, attackDirection, 0f);
                for (int k = 0; k < 30 ; k++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(player.Center + (vel * 100f), DustID.GemEmerald, vel * -5f, 0, default, Main.rand.NextFloat(0.5f, 1f));
                    dust.noGravity = true;
                }
            }
            else
            {
                attackDirection = -attackDirection;
                SwordAnimator.Swing<EnforcerSwing>(player, damage, knockback);

            }



            return false;
        }
    }
    public class DecodeSwing : SwordSwing
    {
        float ScaleEase(float cur, float max)
        {
            float x = cur / max;
            return 1.15f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(1.3f - x) * MathHelper.Pi) * 0.5f * 0.5f;
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

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 1;


            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectileDirect(projectile.GetSource_FromAI(), target.Center, new Vector2(7,10).RotateRandom(3), ModContent.ProjectileType<EnforcerOrb>(), 20, 0, projectile.owner);

            }
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, Color.MediumPurple, 0.3f);
                Dust.NewDust(target.position, target.width, target.height, DustID.GemAmethyst, target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, default, 0.3f);
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 4f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, Color.MediumPurple, 0.8f);

            }


        }


    

        private void Update(Projectile projectile)
        {
            if (freezeFrames > -1)
            {
                freezeFrames--;

                if (freezeFrames > 0)
                {
                    SwordProjectile proj = (projectile.ModProjectile as SwordProjectile);
                    proj.FramesPassed -= 1f / proj.SwingInfo.Updates;
                }
            }
        }

        public int attackDirection = 1;
        public int AttackCounter = 1;

        public override int Updates => 10;
        public override Action<Projectile, NPC, int, float, bool> OnHitNPC => NPCHit;

        public override string SwordTexture => "Divergency/Content/Items/Weapons/Melee/DecodeDestruction";
        public override Vector2 Pivot => new Vector2(0, 50);

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
            new SwordAnimation(-2f+MathF.PI/2, 0), // TimedFunction should be in here, not down below...
            new SwordAnimation(2f+MathF.PI/2, 45, FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
            new SwordAnimation(-2f+MathF.PI/2, 45, 4, FrameFunctions: timedFunctions,Flipped: true, HoldToContinue: true, RotationIn: RotationEase, ScaleMul: ScaleEase),


        });
        public override float Width => MathF.Sqrt(MathF.Pow(12, 2) * 2) + 1f; // 12 is vertical width of blade

        public override SwordTrail SwordTrail => new SwordTrail("Divergency/Assets/Textures/TestTrail2", 90, TrailType.Raw, 100f);
        public override SwordGlow[] Glows => new SwordGlow[] { new SwordGlow(new Color(0, 188, 0), 0.95f, false) };
    }
   
   
}
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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class Enforcer : ModItem
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
            Item.damage = 32;
            Item.knockBack = 4f;

            Item.shoot = ModContent.ProjectileType<EnforcerSwing>();
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
    public class EnforcerSwing : SwordSwing
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 1;


            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, new Vector2(7,10).RotateRandom(3), ModContent.ProjectileType<EnforcerOrb>(), 20, 0, Projectile.owner);

            }
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, Color.LimeGreen, 0.3f);
                Dust.NewDust(target.position, target.width, target.height, DustID.GemEmerald, target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, default, 0.3f);
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 4f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, Color.LimeGreen, 0.8f);

            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];
           
            return false;
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
        public override string SwordTexture => "Divergency/Content/Items/Weapons/Melee/Enforcer";
        public override Vector2 Pivot => new Vector2(0, 50);
        public override float BuildInRotation => 0;

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
        public override SwordGlow[] Glows => new SwordGlow[] { new SwordGlow(
            new SwordGlowColor(
                new List<Color>{
                    new Color(0, 0, 0, 255),
                    new Color(0, 255, 0),
                    new Color(0, 255, 0)
                }, new List<int>
                {
                    0,
                    0,
                    1,
                }), 
            1f, false) };
    }
   
    public class EnforcerOrb : ModProjectile
    {
        public bool initialzed;
        public override void SetStaticDefaults()
        {
            //.setdefault("Life Orb");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public Trail trail3;

        public Trail trail4;
        float radius = 0;

        float timer = 0;

        float width = 10;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(14);
            Projectile.scale = 1f;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 2000;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 4;



        }

        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            if (!initialzed)
            {
                radius += (10 - radius) / 5f;

               

                Projectile.damage = 0;
            }

            Player player = Main.player[Projectile.owner];

            if (Main.mouseRight && Main.mouseRightRelease && player.HeldItem.type == ModContent.ItemType<Enforcer>())
            {
                initialzed = true;
                Projectile.velocity += Projectile.Center.DirectionTo(player.Center) * 2;


            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (initialzed)
            {
                // Projectile.Move(player.Center, 50);
                Projectile.damage = player.statLife / 8;


            }
            if (Projectile.active && Projectile.Hitbox.Intersects(player.Hitbox) && !player.dead)
            {

                player.Heal(1);
                Projectile.Kill();
                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { Volume = 0.8f, MaxInstances = 3 });

                for (int i = 0; i < 8; i++)
                {
                    Vector2 dir = Main.rand.NextVector2Unit() * 0.1f;

                    ParticleManager.NewParticle(Projectile.Center, dir * Main.rand.NextFloat(10, 25), ParticleManager.NewInstance<StarParticle>(), new Color(0.50f, 2f, 0.5f, 0), 0.3f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);

                }

            }
            if (!Particlespawned)
            {
                for (int i = 0; i < 2; i++)
                {
                    ParticleManager.NewParticle(Projectile.Center, new Vector2(0, 0), ParticleManager.NewInstance<BloomParticleProjectile>(), new Color(0.50f, 2f, 0.5f, 0), 0.03f, Projectile.whoAmI, Layer: Particle.Layer.BeforeNPCs);

                }
                Particlespawned = true;
            }   

        }

        public Trail trail;
        public Trail trail2;
        private int initialDamage;
        float timer2;

        public bool Particlespawned { get; private set; }

        public override bool PreDraw(ref Color lightColor)
        {



            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Shadow").Value;

            for (int k = 0; k < 3; k++)
            {
                if (trail == null)
                {
                    trail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(30, 220, 30, 100)) * (float)Math.Pow(1f - p, 2f));
                    trail.drawOffset = Projectile.Size / 2f;
                }

                trail.Draw(Projectile.oldPos, timer);
                timer2 -= 0.01f;
            }
            if (trail2 == null)
            {
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            trail2.Draw(Projectile.oldPos);

            if (trail3 == null)
            {
                trail3 = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(width), (p) => Projectile.GetAlpha(new Color(0, 255, 0, 0)));
                trail3.drawOffset = Projectile.Size / 2f;

                trail4 = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(width / 1.9f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail4.drawOffset = Projectile.Size / 2f;
            }

            int parts = 8;
            Vector2[] tests = new Vector2[parts];
            float[] rotations = new float[parts];

            for (int point = 0; point < parts; point++)
            {
                float rad = ((float)point / (parts - 1)) * MathHelper.TwoPi;

                tests[point] = Projectile.position + new Vector2(MathF.Cos(rad) * radius, MathF.Sin(rad) * radius);
                rotations[point] = rad;
            }

            timer -= 0.01f;

            trail3.Draw(tests, rotations, timer);
            trail4.Draw(tests, rotations, timer);

            return false;
        }
    }
}
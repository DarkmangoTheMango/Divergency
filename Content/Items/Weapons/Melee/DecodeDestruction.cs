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
using Microsoft.CodeAnalysis;
using System.Threading;
using Mono.Cecil;
using Terraria.GameContent;

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
                SwordAnimator.Swing<DecodeSwing>(player, damage, knockback);

            }



            return false;
        }
    }
    public class DecodeSwing : SwordSwing
    {
        float ScaleEase(float cur, float max)
        {
            float x = cur / max;
            return 0.5f + MathF.Sin(EaseFunction.EaseCircularInOut.Ease(1.3f - x) * MathHelper.Pi) * 0.5f * 0.5f;
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



            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);            

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, Color.MediumPurple, 0.3f);
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Glow>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 4f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, Color.MediumPurple, 0.8f);
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<CodeDust>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 5f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, Color.MediumPurple, 0.4f);
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<CodeDust1>(), target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 5f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, Color.MediumPurple, 0.4f);



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

        public override Vector2 Pivot => new Vector2(0, 140);


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

            new SwordAnimation(-2f+MathF.PI/2, 0, Scale: new Vector2(1f, 1f)), // TimedFunction should be in here, not down below...
            new SwordAnimation(2f+MathF.PI/2, 35, Scale: new Vector2(1f, 1f), FrameFunctions: timedFunctions, RotationIn: RotationEase, ScaleMul: ScaleEase),
            new SwordAnimation(-2f+MathF.PI/2, 35, 4, Scale: new Vector2(1f, 1f), FrameFunctions: timedFunctions,Flipped: true, HoldToContinue: true, RotationIn: RotationEase, ScaleMul: ScaleEase),


        });
        public override float Width => MathF.Sqrt(MathF.Pow(12, 2) * 2) + 1f; // 12 is vertical width of blade

        public override SwordTrail SwordTrail => new SwordTrail("Divergency/Assets/Textures/DecodeTrail", 280, TrailType.Raw, 100, 0.70f);
        //  public override Keyframes SwordFrames => new Keyframes(new SwordAnimation[]
        public override SwordGlow[] Glows => new SwordGlow[]
        {
         new SwordGlow(new SwordGlowColor(
                new List<Color>{
                    new Color(255, 182, 193, 100),
                    new Color(255, 182, 193, 100),
                    new Color(240, 182, 230, 100)

                }, new List<int>
                {
                    0,
                    40,
                    40,
                }),

            1f, false),
          new SwordGlow(new SwordGlowColor(
                new List<Color>{
                    new Color(255, 255, 255, 75),
                    new Color(255, 255, 255, 75),
                    new Color(255, 255, 255, 75)

                }, new List<int>
                {
                    0,
                    1,
                    1,
                    
                }),

            1f, false, "Divergency/Content/Items/Weapons/Melee/DecodeDestructionGlow2"),

          };





        public override bool SlantingSword => false;

        //public override SwordTrail SwordTrail => new SwordTrail("Divergency/Assets/Textures/TestTrail2", 90, TrailType.Raw, 100f);
        //public override SwordGlow[] Glows => new SwordGlow[] { new SwordGlow(new Color(0, 188, 0), 0.95f, false) };
    }
    public class LinkHandler : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";
        public bool ArrowsActive;
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(50);
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 999;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
        }
        public override void AI()
        {

            Projectile.timeLeft = 666;
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            if (player.HeldItem.type != ModContent.ItemType<DecodeDestruction>())
            {
                Projectile.Kill();
                player.GetModPlayer<LinkPlayer>().spawned = false;
            }
            else if (!ArrowsActive && player.HeldItem.type == ModContent.ItemType<DecodeDestruction>())
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center + new Vector2(100,100), new Vector2(0), ModContent.ProjectileType<LinkArrowActive>(), 0, 0, ai0: 0);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center + new Vector2(-100, 100), new Vector2(0), ModContent.ProjectileType<LinkArrowActive>(), 0, 0, ai0: 1);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center + new Vector2(-100, 100), new Vector2(0), ModContent.ProjectileType<LinkArrowActive>(), 0, 0, ai0: 2);

                ArrowsActive = true;
            }
        }
    }
    public class LinkArrowActive : ModProjectile
    {
        public int PointCounter;
        public Vector2 SpawnPosition;
        public int timer;

        public EntitySource_Misc source { get; private set; }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(30);
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 999;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            Vector3 RGB = new Vector3(2.55f, 2.55f, 2.55f);
            float multiplier = 0.3f;
            float max = 0.3f;
            float min = 0.3f;
            RGB *= multiplier;
            if (RGB.X > max)
            {
                multiplier = 0.4f;
            }
            if (RGB.X < min)
            {
                multiplier = 0.4f;
            }
             Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            Player player = Main.player[Projectile.owner];
           
            
                //if (source is EntitySource_Misc entitySource_Misc && entitySource_Misc.Context == "Right Arrow")
                if (Projectile.ai[0] == 0f)
                {
                    Projectile.Center = player.Center + new Vector2(100);
                    Projectile.rotation = 3 * MathHelper.Pi / 4;

                }
                else if (Projectile.ai[0] == 1f)
                {
                    Projectile.Center = player.Center + new Vector2(-100, 100);
                    Projectile.rotation = 5 * MathHelper.Pi/4;
                }
                else
                {
                    Projectile.Center = player.Center + new Vector2(0, -140);

                }
                Projectile.timeLeft = 666;
                if (player.HeldItem.type != ModContent.ItemType<DecodeDestruction>())
                {
                    Projectile.Kill();
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (Projectile.active && Projectile.Hitbox.Intersects(npc.Hitbox) && npc.active)
                {
                    PointCounter--;

                    for (int guh = 0; guh < 1; guh++)
                        {
                            if (Main.rand.NextBool(15))
                            {
                                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CodeDust>(), 0f, -Main.rand.NextFloat(4, 5), 0, Color.MediumPurple, 0.35f);
                                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CodeDust1>(), 0f, -Main.rand.NextFloat(4, 5), 0, Color.MediumPurple, 0.35f);
                            }
                                

                        }
                        

                        //SoundEngine.PlaySound(SoundID.Item2 with { Volume = 0.8f, MaxInstances = 3 });
                    }
                    
                }




        }
           
            
        
        
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];


            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D texture0 = ModContent.Request<Texture2D>("Divergency/Content/Items/Weapons/Melee/LinkArrow").Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
  
            
            Vector2 drawPos = Projectile.Center - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                
            //Main.EntitySpriteDraw(texture0, drawPos, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            //Main.EntitySpriteDraw(texture, drawPos, null, new Color(255, 255, 255, player.GetModPlayer<LinkPlayer>().PointCounter), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return true;
        }
    }

    public class LinkPlayer : ModPlayer
    {
        public bool spawned;
        public int PointCounter = 255;
        public override void PreUpdate()
        {
            Main.NewText(PointCounter);

            if (!Player.dead && Player.HeldItem.type == ModContent.ItemType<DecodeDestruction>() && !spawned)
            {
                Projectile.NewProjectileDirect(null, Player.Center, new Vector2 (0), ModContent.ProjectileType<LinkHandler>(), 0, 0);
                spawned = true;
            }
          
            
        }
    }
   
   
}
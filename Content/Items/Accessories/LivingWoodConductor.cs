
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using Divergency.Content.Items.Weapons.Magic;

namespace Divergency.Content.Items.Accessories
{
    public class LivingWoodConductor : ModItem
    {
        public override void SetStaticDefaults()
        {
            ////.setdefault("Magic attacks summon healing Acorns upon striking enemies");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;


        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AcornDrop>().Acorns = true;

        }




    }

    public class AcornDrop : ModPlayer
    {
        public bool Acorns;
        public int cooldown;
        public override void ResetEffects()
        {
            Acorns = false;
        }

       

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {


            if (proj.DamageType == DamageClass.Magic && Acorns && proj.type != ModContent.ProjectileType<AcornProj>())
            {

                for (int i = 0; i < Main.rand.Next(0, 2); i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                    Projectile.NewProjectile(Player.GetSource_FromAI(), target.Center, speed * 13, ModContent.ProjectileType<AcornProj>(), 0, 0f, Player.whoAmI, 0f, 0);
                }

            }
        
    }
        public override void PreUpdate()
        {
            if (Acorns && cooldown < 180)
            {
                cooldown++;
            }
        }
    }
    public class AcornProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Acorn;


        public override void SetStaticDefaults()
        {
            //.setdefault("Acorn");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 15;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(10);
            Projectile.scale = 0.9f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.velocity.X *= 0.99f;
            for (int i = 0; i < Main.maxNetPlayers; i++)
            {
                Player player = Main.player[i];
                if (Projectile.active && Projectile.Hitbox.Intersects(player.Hitbox) && !player.dead && Projectile.Distance(Projectile.Center) < 15)
                {
                    player.Heal(1);
                    player.AddBuff(BuffID.WellFed2,360);
                    SoundEngine.PlaySound(SoundID.Item2 with { Volume = 0.8f, MaxInstances = 3 });
                    Projectile.Kill();
                }
         
            }
            Projectile.rotation += Projectile.velocity.Length() * (Projectile.direction * 0.04f);

        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) >= float.Epsilon) { Projectile.velocity.X = -oldVelocity.X * 1f; }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) >= float.Epsilon) { Projectile.velocity.Y = -oldVelocity.Y * 0.5f; }
            return false;
        }
    }
}
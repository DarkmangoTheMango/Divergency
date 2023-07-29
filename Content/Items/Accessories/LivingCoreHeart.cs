
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using Divergency.Content.Projectiles;

namespace Divergency.Content.Items.Accessories
{
    public class LivingCoreHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            ////.setdefault("Magic attacks summon Life Orbs upon striking enemies");

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
            player.GetModPlayer<HeartDrop>().Orbs = true;

        }




    }

    public class HeartDrop : ModPlayer
    {
        public bool Orbs;
        public override void ResetEffects()
        {
            Orbs = false;
        }




        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {

        

            
            if (proj.DamageType == DamageClass.Magic && Orbs && proj.type != ModContent.ProjectileType<AcornProj>())
            {

                for (int i = 0; i < Main.rand.Next(-6, 2); i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                    Projectile.NewProjectile(Player.GetSource_FromAI(), target.Center, Player.velocity, ModContent.ProjectileType<LifeOrb>(), 0, 0f, Player.whoAmI, 0f, 0);
                }

            }
        }
    }
   
}
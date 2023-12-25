using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using ParticleLibrary;
using ParticleLibrary.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ParticleLibrary.Examples;
using ParticleLibrary.Utilities;
using Divergency.Content.Particles.ParticleSystems;

namespace Divergency.Content.Items.Test
{
    public class ParticleTester : ModItem
    {
        public override void SetStaticDefaults()
        {
            // //.setdefault("Doorlauncher"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ////.setdefault("'Physics is for wimps'"
            //  + "\nUses Wooden Bullets as ammo!'");
        }

        public override void SetDefaults()
        {
            Item.damage = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = 10000;
            Item.noMelee = true;
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item167;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.VilePowder;
            Item.shootSpeed = 19f;
            Item.scale = 0.1f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 10f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            const int NumProjectiles = 100; // The humber of projectiles that this gun will shoot.

            for (int i = 0; i < NumProjectiles; i++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(360));

                // Decrease velocity randomly for nicer visuals.
                newVelocity *= 1f - Main.rand.NextFloat(0.7f);

                ParticleSystemManager.ExampleQuadSystem.NewParticle(position + (newVelocity * Main.rand.NextFloat(500)), velocity * 1.5f, ParticleSystemManager.CoreGroveVisualForeground);
               // ExampleParticleSystemManager.ExamplePointSystem.NewParticle(position + (velocity * Main.rand.NextFloat(500)), Main.rand.NextVector2Unit() * Main.rand.NextFloat(-8f, 8f + float.Epsilon), ExampleParticleSystemManager.ExamplePointParticle);

                //NewParticleManager.NewParticle(position, newVelocity * 5, new ExampleParticle(), new Color(0.50f, 2f, 0.5f, 0), 0.2f);
                //EmitterSystem.NewEmitter<EmitterExample>(new EmitterSettings(), new EmitterParticleSettings(), new EmitterColorSettings());

            }
            return false;

        }

    }
}

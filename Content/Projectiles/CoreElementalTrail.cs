using Divergency.Content.NPCs.LivingGrove;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Divergency.Common.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Divergency.Content.Projectiles
{
    public class CoreElementalTrail : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public Projectile cachedProjectile { get; private set; }
        public bool spawned { get; private set; }
        public NPC cachedNPC { get; private set; }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.width = Projectile.height = 0;
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            if (!spawned)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is CoreElemental)
                    {
                        cachedNPC = taggedNPC;
                    }
                }

                spawned = true;
            }

            if (!cachedNPC.active)
            {
                Projectile.active = false;
            }
            if (spawned)
            {
                Projectile.Center = cachedNPC.Center;
            }
        }
        public Trail trail;
        public Trail trail2;

        public bool Particlespawned { get; private set; }

        public override bool PreDraw(ref Color lightColor)
        {



            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(30, 220, 30, 100)));
                trail.drawOffset = Projectile.Size / 2f;
            }
            if (trail2 == null)
            {
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(4f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            trail2.Draw(Projectile.oldPos);

            return false;
        }

    }
}

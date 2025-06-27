using Divergency.Common.Helpers;
using Divergency.Content.Projectiles.Magic;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Steamworks;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class CoreHook : ModItem
    { 
        public override Vector2? HoldoutOffset() => Vector2.Zero;

        public override void SetStaticDefaults()
        {
            ////.setdefault("AW FUCK");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 1;
            Item.knockBack = 3f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<CoreHookProj>();
            Item.shootSpeed = 10f;
            Item.channel = true;

            Item.Size = new Vector2(30, 34);
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
        public override bool CanUseItem(Player player)
        {
         
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
}

    public class CoreHookProj : ModProjectile
    {
        float timer;
        private bool goBack;
        private bool Hooked;
        private NPC HookedEnemy;
        private bool pressedLeft;

        public override void Kill(int timeLeft) => SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(15);
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.extraUpdates = 2;
            Projectile.aiStyle = -1;
        }
        private const string ChainTexturePath = "Divergency/Content/Items/Weapons/Melee/CoreHookChain"; 
        public override void AI()
        {
            //Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Projectile.velocity.RotatedByRandom(0.1f), 0, new Color(109, 223, 94), 1.2f).noGravity = true;
            Player player = Main.player[Projectile.owner];
            if (!Hooked)
            {
                timer++;
            }
            if (timer == 60)
            {
                goBack = true;
            }
            if (goBack)
            {
                Projectile.Move(player.Center, 22);
                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    Projectile.Kill();
                }
             
            }
            if (Hooked)
            {
                Projectile.damage = 0;
                Projectile.Center = HookedEnemy.Center;
                if (!HookedEnemy.active)
                {
                    goBack = true;
                }
            }

            if (Hooked)
            {
                
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    HookedEnemy.velocity += HookedEnemy.DirectionTo(player.Center) * 7;
                    HookedEnemy.GetGlobalNPC<CoreHookNPC>().hooked = true;
                    HookedEnemy.GetGlobalNPC<CoreHookNPC>().timer = 0;

                    Hooked = false;
                    goBack = true;
                }
            }
           
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.knockBackResist != 0)
            {
                Hooked = true;
                HookedEnemy = target;
            }
           
        }
        public Trail trail;
        public Trail trail2;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(25f), (p) => Projectile.GetAlpha(new Color(30, 220, 30, 100)));
                trail.drawOffset = Projectile.Size / 2f;
            }
            if (trail2 == null)
            {
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            trail2.Draw(Projectile.oldPos);
            Player player = Main.player[Projectile.owner];
            Asset<Texture2D> chainTex = ModContent.Request<Texture2D>(ChainTexturePath);
            Vector2 Origin = player.Center;

            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Size() / 2f : chainTex.Size() / 2f;
            Vector2 chainDrawPosition = Projectile.Center; /// top bottom etc ------------------------------------------------------------------ TO DOOOOOOOOOOOOOOOOOOOOOOOOOOOOO __________________________________________
            Vector2 vectorFromProjectileToPlayerArms = Origin.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTex.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 32; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
            }
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (chainLengthRemainingToDraw > 0f)
            {
                // This code gets the lighting at the current tile coordinates
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values




                // Here, we draw the chain texture at the coordinates
                Main.spriteBatch.Draw(chainTex.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;

            }


            return true;
        }

        public class CoreHookNPC : GlobalNPC
        {
            public bool hooked;
            public int timer;
            public int timer2;
            public bool marked;
            public bool extender;
            public override bool InstancePerEntity => true;

            public override bool PreAI(NPC npc)
            {
                if (timer == 20)
                {
                    hooked = false;
                    npc.rotation = 0;
                }
                if (hooked)
                {
                    timer++;

                    npc.rotation += npc.velocity.Length() * 0.05f * npc.direction;
                    return false;
                }
                else
                {
                    return base.PreAI(npc);
                }

              


            }
            public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
            {
                if (marked)
                {
                    hit.Damage *= 2;
                    marked = false;
                }
            }
        }

        public class ComboNPC : GlobalNPC
        {

        }
    }
}
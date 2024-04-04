using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Divergency.Common.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Divergency.Content.Dusts;
using Divergency.Common.Players;
using Terraria.Audio;
using System.Drawing.Imaging;
using Divergency.Content.NPCs.LivingGrove;

namespace Divergency.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]

    public class CoreShield : ModItem
	{
		public override void SetStaticDefaults()
		{
			//.setdefault("Rapidity Glove");
			////.setdefault("Doubles your ranged weapons fire rate and enables Auto-Shoot, however ranged damage is decreased by 50% \n'Fire quicker than your own shadow!'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
      

        

        public bool dashActive { get; private set; }
        public int dashSpawnLeft { get; private set; }
        public int dashCooldownLeft { get; private set; }
        public int dashResetLeft { get; private set; }
        public int dashResetRight { get; private set; }
        public int dashSpawnRight { get; private set; }
        public int dashCooldownRight { get; private set; }

        public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(10);	
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
			Item.defense = 2;


		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{  
		
            for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
            {
                Item item = player.armor[i];

                //Set the flag for the ExampleDashAccessory being equipped if we have it equipped OR immediately return if any of the accessories are
                // one of the higher-priority ones
             
                    
                    
                        if (item.type == ItemID.EoCShield || item.type == ItemID.MasterNinjaGear || item.type == ItemID.Tabi || player.setSolar || player.mount.Active)
                        {
                            dashActive = false;
                        }
                        else
                        {
                            dashActive = true;
                        }
                    

             

                 
            }
         
            if (dashCooldownLeft == 1 || dashCooldownRight == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/RechargeDash"), player.Center);
                float radius = 2;
                int numberOfDusts = 20;

                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustPerfect(player.Center, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 10, 0, default, 1f);
                    Dust.NewDustPerfect(player.Center, ModContent.DustType<Glow>(), Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfDusts * i)) * radius, 0, new Color(109, 223, 94), 0.8f);

                    dust.noGravity = true;
                }
                dashCooldownRight = 0;
                dashCooldownLeft = 0;
            }
            //left dash
            if (dashResetLeft == 0)
            {
                dashResetLeft = 15;
                dashSpawnLeft = 0;
            }
            if (dashResetLeft > 0)
            {
                dashResetLeft--;
            }
            if (dashCooldownLeft > 0)
            {
                dashCooldownLeft--;
            }
            if (dashActive && player.controlLeft && player.releaseLeft && dashCooldownLeft == 0)
            {
                dashSpawnLeft++;
                dashResetLeft = 15;
            }
            if (dashSpawnLeft == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/Dash") with { Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, player.Center);

                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.position, new Vector2(0), ModContent.ProjectileType<CoreDashLeftProjectile>(), 0, 0);
                dashSpawnLeft = 0;
                dashCooldownLeft = 420;

            }
            //right dash
            if (dashResetRight == 0)
            {
                dashResetRight = 15;
                dashSpawnRight = 0;
            }
            if (dashResetRight > 0)
            {
                dashResetRight--;
            }
            if (dashCooldownRight > 0)
            {
                dashCooldownRight--;
            }
            if (dashActive && player.controlRight && player.releaseRight && dashCooldownRight == 0)
            {
                dashSpawnRight++;
                dashResetRight = 15;
            }
            if (dashSpawnRight == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/Dash") with { Pitch = Main.rand.NextFloat(-1f, 1f) }, player.Center);

                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.position, new Vector2(0), ModContent.ProjectileType<CoreDashRightProjectile>(), 0, 0);
                dashSpawnRight = 0;
                dashCooldownRight = 420;
            }
        }

		


	}
	
	public class CoreDashLeftProjectile : ModProjectile
	{
        public override string Texture => "Divergency/Assets/Textures/Empty";
        public override void SetStaticDefaults()
        {
            //.setdefault("Shadowflame Effigy");

            Main.projFrames[Projectile.type] = 4;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = new Vector2(32);

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;

            Projectile.timeLeft = 15;//also the duration of the dash
        }
        public override void AI()
        {
            if (!initialize)
            {
                initialize = true;
            }
            {
                Player player = Main.player[Projectile.owner];
                if (player.HeldItem.DamageType == DamageClass.Melee)
                {
                    player.SetImmuneTimeForAllTypes(5);
                }


                if (Projectile.timeLeft >= 10)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 10, 0, default, 1f);
                        // Dust.NewDustPerfect(Projectile.Center + new Vector2(0, Main.rand.NextFloat(-15, 30)), ModContent.DustType<GlowLine>(), Projectile.velocity * 5, 0, new Color(109, 223, 94), 0.65f);

                        dust.noGravity = false;
                    }
                    player.velocity.X -= 20;
                }
                Projectile.Center = player.Center;
                if (Projectile.timeLeft == 10)
                {
                    float radius = 2;
                    int numberOfDusts = 20;

                    for (int i = 0; i < 20; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 10, 0, default, 1f);
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfDusts * i)) * radius, 0, new Color(109, 223, 94), 1f);

                        dust.noGravity = true;
                    }
                    DivergencyDraw.SpawnRing(Projectile.Center, new Color(109, 223, 94));

                    player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 4;
                    player.velocity.X /= 15;
                }
           
            }
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.velocity.X = 0;

        }




        public Trail trail;

        float timer;
        private bool initialize;

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1) { spriteEffects = SpriteEffects.FlipHorizontally; }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Color color = Projectile.GetAlpha(lightColor);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            float offsetX = 30f;
            drawOrigin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Light").Value;

            for (int k = 0; k < 3; k++)
            {
                if (trail == null)
                {
                    trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(130f), (p) => Projectile.GetAlpha(new Color(109, 223, 94, 144)) * (float)Math.Pow(1f - p, 2f));
                    trail.drawOffset = Projectile.Size / 2f;
                }

                trail.Draw(Projectile.oldPos, timer);

                timer -= 0.01f;
            }

            return false;
        }
    }

    public class CoreDashRightProjectile : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";
        public override void SetStaticDefaults()
        {
            //.setdefault("Shadowflame Effigy");

            Main.projFrames[Projectile.type] = 4;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = new Vector2(32);

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;

            Projectile.timeLeft = 15;//also the duration of the dash
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            


            if (Projectile.timeLeft >= 10)
            {
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
                if (player.HeldItem.DamageType == DamageClass.Melee)
                {
                    player.SetImmuneTimeForAllTypes(5);
                }
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 10, 0, default, 1f);
                  //  Dust.NewDustPerfect(Projectile.Center + new Vector2(0, Main.rand.NextFloat(-30, 30)), ModContent.DustType<GlowLine>(), Projectile.velocity * 2, 0, new Color(109, 223, 94), 1f);

                    dust.noGravity = false;
                }
                player.velocity.X += 20;
            }
            Projectile.Center = player.Center;
            if (Projectile.timeLeft == 10)
            {
                float radius = 2;
                int numberOfDusts = 20;

                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), Main.rand.NextVector2Circular(1f, 1f) * 10, 0, default, 1f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfDusts * i)) * radius, 0, new Color(109, 223, 94), 1f);

                    dust.noGravity = true;
                }
                DivergencyDraw.SpawnRing(Projectile.Center, new Color(109, 223, 94));

                player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 4;
                player.velocity.X /= 15;
            }
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.velocity.X = 0;
        }




        public Trail trail;

        float timer;

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1) { spriteEffects = SpriteEffects.FlipHorizontally; }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Color color = Projectile.GetAlpha(lightColor);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            float offsetX = 30f;
            drawOrigin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Light").Value;

            for (int k = 0; k < 3; k++)
            {
                if (trail == null)
                {
                    trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(130f), (p) => Projectile.GetAlpha(new Color(109, 223, 94, 144)) * (float)Math.Pow(1f - p, 2f));
                    trail.drawOffset = Projectile.Size / 2f;
                }

                trail.Draw(Projectile.oldPos, timer);

                timer -= 0.01f;
            }

            return false;
        }
    }
}


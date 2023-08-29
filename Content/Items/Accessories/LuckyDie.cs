using Divergency.Content.Buffs;
using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Accessories
{
    public class LuckyDie : ModItem
    {

        public bool DiceSpawn = true;
        public int DiceCooldown = 100;
        public override void SetStaticDefaults()
        {

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
            if (!hideVisual)
            {
                DiceCooldown--;
            }
            if (DiceCooldown == 0)
            {
                DiceSpawn = true;
            }
            if (DiceSpawn)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity, ModContent.ProjectileType<DiceProj>(), 0, 1f, player.whoAmI);
                DiceSpawn = false;
                DiceCooldown = 1000;
            }

            //player.GetDamage(DamageClass.Magic) *= 0.9f; // Increase ALL player damage by 100%

        }




    }
    public class DiceProj : ModProjectile
    {
        public int Timer;
        public int Timer2;
        public int Timer3;
        public float offsetx = 50;
        public bool offsetxRaise;
        public bool offsetxLow = true;
        public bool RollStop;
        public float offsety = -100;
        public bool offsetyRaise = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()  //currently a damaging projectile
        {
            Projectile.width = 100;
            Projectile.height = 100;

            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ownerHitCheck = false;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 380;
            Projectile.scale = 0.6f;
        }
        public override void AI()
        {
            Timer++;
            Timer2++;
            for (int i = 0; i < 10; i++)
            {
                if (!RollStop)
                {
                    if (offsetxLow)
                    {
                        offsetx--;
                    }
                    if (offsetxRaise)
                    {
                        offsetx++;
                    }

                }
                else
                {
                    offsetx = 45;
                }
            }


            if (Timer2 >= 120)
            {


            }
            for (int i = 0; i < 3; i++)
            {
                if (offsetx == 0)
                {


                    offsetxRaise = true;
                    offsetxLow = false;
                }
                if (offsetx == 100)
                {
                    offsetxRaise = false;
                    offsetxLow = true;
                }




            }


            Player player = Main.player[Projectile.owner];
            if (Timer2 == 200 && Projectile.frame == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                    //ParticleManager.NewParticle(player.Center, speed * 4, ParticleManager.NewInstance<Dice1Particle>(), Color.Purple, Main.rand.NextFloat(0.2f, 0.8f));
                }
            }
            else if (Timer2 == 120)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                    //ParticleManager.NewParticle(player.Center, speed * 4, ParticleManager.NewInstance<StarParticle>(), Color.Purple, Main.rand.NextFloat(0.2f, 0.8f));
                }
            }
            if (Timer == 10 && Timer2 < 200)
            {
                if (Main.rand.NextBool(1000))
                {
                    Projectile.frame = 6;
                }
                else
                {
                    Projectile.frame = Main.rand.Next(6); // 0-5
                }
                SoundEngine.PlaySound(SoundID.MenuTick);

                Timer = 0;

            }
            if (Timer2 == 200)
            {
                RollStop = true;
                SoundEngine.PlaySound(new SoundStyle($"{nameof(Divergency)}/Assets/Sounds/Items/DiceSound")
                {

                    Volume = 0.9f,
                    MaxInstances = 5,
                });


                if (Projectile.frame == 0)
                {
                    player.AddBuff(ModContent.BuffType<Dice1>(), 600);

                }
                if (Projectile.frame == 1)
                {
                    player.AddBuff(ModContent.BuffType<Dice2>(), 600);

                }
                if (Projectile.frame == 2)
                {
                    player.AddBuff(ModContent.BuffType<Dice3>(), 600);

                }
                if (Projectile.frame == 3)
                {
                    player.AddBuff(ModContent.BuffType<Dice4>(), 600);

                }
                if (Projectile.frame == 4)
                {
                    player.AddBuff(ModContent.BuffType<Dice5>(), 600);

                }
                if (Projectile.frame == 5)
                {
                    player.AddBuff(ModContent.BuffType<Dice6>(), 600);

                }
                if (Projectile.frame == 6)
                {
                    player.dead = true;
                    CombatText.NewText(player.getRect(), Color.White, "get fucked pleb lmao", true, false);
                }


            }
            if (Projectile.timeLeft < 10)
            {
                Projectile.alpha += 30;
            }

            Projectile.Center = player.Center;



        }
        public override void Kill(int timeLeft)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            float offsetX = offsetx;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            // If sprite is vertical
            float offsetY = offsety;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);

            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(Color.White);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }





    public class DicePlayer : ModPlayer
    {
        public bool Dice1 = false;
        public bool Dice6 = false;
        public bool DiceSpeed = false;
        public bool Text = false;
        public int TextCooldown = 0;
        public byte TextCounter = 0;
        public int Timer;


        public override void ResetEffects()
        {
            Timer++;

            Dice1 = false;
            Dice6 = false;
            DiceSpeed = false;
            Text = false;

        }
        public override void PreUpdate()
        {

            if (Timer >= 3000)
            {
                TextCounter = 0;
                Timer = 0;

            }
            if (Text && TextCooldown <= 0)
            {
                TextCounter++;
                if (TextCounter == 1)
                {
                    CombatText.NewText(Player.getRect(), Color.Red, "BETTER LUCK NEXT TIME!", true, false);

                }
                if (TextCounter == 2)
                {
                    CombatText.NewText(Player.getRect(), Color.Red, "Not your day, huh?", true, false);


                }
                if (TextCounter == 3)
                {
                    CombatText.NewText(Player.getRect(), Color.White,
"\n⣀ ⣠ ⣤ ⣤ ⣤ ⣤ ⢤ ⣤ ⣄ ⣀ ⣀ ⣀ ⣀ ⡀⡀⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄" +
"\n⠄⠉⠹⣾ ⣿ ⣛ ⣿ ⣿ ⣞ ⣿ ⣛ ⣺ ⣻ ⢾ ⣾ ⣿ ⣿ ⣿ ⣶ ⣶ ⣶ ⣄ ⡀ ⠄  ⠄" +
"\n⠄⠄⠠⣿ ⣷⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣯ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣆ ⠄⠄" +
"\n⠄ ⠄ ⠘ ⠛ ⠛ ⠛ ⠛ ⠋ ⠿ ⣷ ⣿ ⣿ ⡿ ⣿ ⢿ ⠟ ⠟ ⠟ ⠻ ⠻ ⣿ ⣿ ⣿ ⣿⡀⠄" +
"\n⠄ ⢀ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⢛ ⣿ ⣁⠄ ⠒ ⠂ ⠄ ⠄ ⣀ ⣰ ⣿ ⣿ ⣿ ⣿ ⡀" +
"\n⠄⠉ ⠛ ⠺ ⢶ ⣷ ⡶ ⠃ ⠄ ⠄ ⠨ ⣿ ⣿ ⡇ ⠄ ⡺ ⣾ ⣾ ⣾ ⣿ ⣿ ⣿ ⣿ ⣽ ⣿ ⣿" +
"\n⠄ ⠄ ⠄ ⠄ ⠄ ⠛ ⠁ ⠄ ⠄ ⠄ ⢀ ⣿ ⣿ ⣧ ⡀ ⠄ ⠹ ⣿ ⣿ ⣿ ⣿ ⣿ ⡿ ⣿ ⣻ ⣿" +
"\n⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠉ ⠛ ⠟ ⠇ ⢀ ⢰ ⣿ ⣿ ⣿ ⣏ ⠉ ⢿ ⣽ ⢿ ⡏" +
"\n⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠠ ⠤ ⣤ ⣴ ⣾ ⣿ ⣿ ⣾ ⣿ ⣿ ⣦ ⠄ ⢹ ⡿ ⠄" +
"\n⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠒ ⣳ ⣶ ⣤ ⣤ ⣄ ⣀ ⣀ ⡈ ⣀ ⢁  ⢁ ⢁ ⣈ ⣄ ⢐ ⠃ ⠄" +
"\n⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⣰ ⣿ ⣛ ⣻ ⡿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⡯ ⠄ ⠄" +
"\n ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⣬ ⣽ ⣿ ⣻ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⠁ ⠄ ⠄" +
"\n⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⢘ ⣿ ⣿ ⣻ ⣛ ⣿ ⡿ ⣟ ⣻ ⣿ ⣿ ⣿ ⣿ ⡟ ⠄ ⠄ ⠄" +
"\n⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠛ ⢛ ⢿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣿ ⣷ ⡿ ⠁ ⠄ ⠄ ⠄" +
"\n⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄ ⠉ ⠉ ⠉ ⠉ ⠈ ⠄ ⠄ ⠄ ⠄ ⠄ ⠄" +
"\n" +
"\n" +
"\n" +
"\n" +
"\n" +
"\n" +
"\n" +
"\n" +
"\n" +
"\n" +
"\n"
, true, false);
                    TextCounter = 0;
                }


                TextCooldown = 600;


            }
            TextCooldown--;
        }
        public override void PostUpdateRunSpeeds()
        {
            if (DiceSpeed)
            {
                Player.runAcceleration *= 2;
                Player.maxRunSpeed *= 2;
            }
            else
            {
                Player.runAcceleration *= 1;
                Player.maxRunSpeed *= 1;
            }
        }
        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (item.DamageType == DamageClass.Ranged && Dice1)
            {
                velocity *= 0.5f;
            }

        }




    }
    public class Dice1 : ModBuff
    {
        public bool Text = false;

        public override void SetStaticDefaults()
        {
    

            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Ranged) *= 0.5f; // Increase ALL player damage by 100%
            player.AddBuff(BuffID.Slow, 1, true, true);
            player.GetAttackSpeed(DamageClass.Ranged) *= 0.5f;
            player.GetCritChance(DamageClass.Ranged) *= 0;
            player.GetModPlayer<DicePlayer>().Dice1 = true;
            player.GetModPlayer<DicePlayer>().Text = true;

        }

    }
    public class Dice2 : ModBuff
    {
        public override void SetStaticDefaults()
        {
          

            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Ranged) *= 1.3f; // Increase ALL player damage by 100%

        }
    }
    public class Dice3 : ModBuff
    {
        public override void SetStaticDefaults()
        {
        

            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Ranged) *= 1.8f;

        }
    }
    public class Dice4 : ModBuff
    {
        public override void SetStaticDefaults()
        {
           

            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetCritChance(DamageClass.Ranged) += 40;

        }
    }
    public class Dice5 : ModBuff
    {
        public override void SetStaticDefaults()
        {
       

            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<DicePlayer>().DiceSpeed = true;

        }
    }
    public class Dice6 : ModBuff
    {
        public override void SetStaticDefaults()
        {
     

            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
        }
        public override void Update(Player player, ref int buffIndex)
        {

            player.GetModPlayer<DicePlayer>().Dice6 = true;

        }
    }

    public class GlobalDiceProj : GlobalProjectile
    {

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse)
            {
                if (itemUse.Entity is Player player && player.GetModPlayer<DicePlayer>().Dice6 && itemUse.Item.DamageType == DamageClass.Ranged)
                {
                    projectile.velocity *= 1.5f;
                    // Alt: projectile.extraUpdates = (projectile.extraUpdates + 1) * 3 - 1;
                }
            }
        }

    }
}
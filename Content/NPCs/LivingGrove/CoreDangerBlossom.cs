using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading;
using System.Timers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Divergency.Content.Projectiles.Magic.CorescillationProj;

namespace Divergency.Content.NPCs.LivingGrove
{

    public class CoreDangerBlossom : ModNPC
    {
        public bool initialize { get; private set; }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Scale = 0.2f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            Main.npcFrameCount[NPC.type] = 6;
            //.setdefault("Core Sage");


        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.damage = 16;
            NPC.defense = 8;
            NPC.knockBackResist = 0f;

            NPC.noTileCollide = false;

            NPC.scale = 0.2f;
            NPC.Size = new Vector2(84, 70);

            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.sellPrice(0, 0, 0, 0);
           //NPC.hide = true;
            NPC.aiStyle = -1;
            NPC.noGravity = false;

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("A powerful warlock that can focus the energy radiating from living crystals in powerful magic.")
            });
        }
   


       
        public override void AI()
        {

            if (!initialize)
            {

                initialize = true;

            }
            else
            {
                NPC.ai[0]++;
                Player player = Main.player[NPC.target];

                NPC.velocity.Y += 3;

               
                if (NPC.ai[0] == 60)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                        Dust dust = Dust.NewDustPerfect(NPC.Center + (speed * 100f), DustID.TerraBlade, speed * 5f, 0, default, 1f);
                        NPC.ai[0] = 0;
                    }
                    Vector2 speed2 = Main.rand.NextVector2Circular(1f, 1f);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (speed2 * 200f), (NPC.DirectionTo(player.Center) * 10f).RotatedByRandom(0.9f), ModContent.ProjectileType<BlossomButterfly>(), NPC.damage, 0f, 0);
                    Dust dust2 = Dust.NewDustPerfect(NPC.Center + (speed2 * 200f), DustID.TerraBlade, speed2 * 2f, 0, default, 1f);

                }

            }

            
        }


        public override void HitEffect(NPC.HitInfo hit)
        {

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }


            if (NPC.life <= 0)
            {
                //NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<SageDeath>());
            }
        }

     



        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
                           //base textures

                Texture2D a = TextureAssets.Npc[Type].Value; 
        
                Vector2 drawOrigin = new(a.Width / 2, a.Height / 2);

                Main.EntitySpriteDraw(a, NPC.Center - Main.screenPosition + new Vector2(0, 0), null, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

            
            return false;
        }
    }
    public class BlossomButterfly : ModProjectile
    {
        public bool spawned { get; private set; }
        public NPC cachedNPC { get; private set; }

        public override void SetStaticDefaults()
        {
            //.setdefault("Shadowflame Effigy");

            Main.projFrames[Projectile.type] = 3;

      
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.Size = new Vector2(6);
            Projectile.scale = 1f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 3000;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            Player player = Main.LocalPlayer;

            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X >= 0f) ? 1 : -1;
            if(Projectile.direction == -1)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }


            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3) { Projectile.frame = 0; }
            }
            if (player.HasBuff<CoreInfection>())
            {
                Projectile.Move(player.Center, 1.2f, Main.rand.NextFloat(20, 50));
            }
            else if (player.HasBuff<CoreInfectionII>())
            {
                Projectile.Move(player.Center, 1.5f, Main.rand.NextFloat(20, 50));
            }
            else
            {
                Projectile.Move(player.Center, 0.5f, Main.rand.NextFloat(20, 50));

            }

            if (!spawned)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is CoreDangerBlossom&& Projectile.Distance(taggedNPC.Center) < 500)
                    {
                        cachedNPC = taggedNPC;
                    }
                }

                spawned = true;
            }

            if (!cachedNPC.active)
            {
                Projectile.Kill();
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, speed * 3f, 0, default, 1f);
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!target.HasBuff(ModContent.BuffType<CoreInfection>()))
            {
                target.AddBuff(ModContent.BuffType<CoreInfection>(), 180);
            }
            else
            {
                target.AddBuff(ModContent.BuffType<CoreInfectionII>(), 180);
            }
        
        
        }
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

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);


         
            return false;
        }
    }
}

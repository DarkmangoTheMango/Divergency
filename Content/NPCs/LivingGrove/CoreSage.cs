using Divergency.Assets.Particles;
using Divergency.Common.Helpers;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{

    public class CoreSage : ModNPC
    {


        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Scale = 0.5f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            Main.npcFrameCount[NPC.type] = 8;
            DisplayName.SetDefault("Core Sage");


        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.defense = 8;
            NPC.knockBackResist = 0.2f;

            NPC.noTileCollide = true;

            NPC.scale = 1f;
            NPC.Size = new Vector2(70, 130);

            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("A powerful warlock that can focus the energy radiating from living crystals in powerful magic.")
            });
        }
        private enum ActionState
        {
            Teleport,
            TeleportBack,
            FloatingBalls,
            DirectionalAttack,
            SageBeam
            

          
        }
        private int teleport;

        public bool TpBack { get; private set; }

        public float State = 0;

        public float Phase;
        private bool initialize;

        public override void AI()
        {
            if (!initialize)
            {

                initialize = true;  

            }
            else
            {
                Player player = Main.player[NPC.target];

                if (NPC.HasValidTarget && !player.dead)
                {

                    NPC.TargetClosest();
                }
         
                NPC.velocity *= 0.85f;

                switch (Phase)
                {
                    case 0:
                        Teleport();

                        break;

                    case 1:
                        FloatingBalls();

                        break;


                    case 2:

                        DirectionalAttack();

                        break;

                    case 3:
                        SageBeam();
                       
                        break;
                }

                NPC.TargetClosest(true);
            }

        }
        private void Teleport()
        {
            Player player = Main.player[NPC.target];

            teleport++;

            if (teleport >= 260 && Main.netMode != NetmodeID.MultiplayerClient)
            {

                vel = DivUtils.FromAToB(NPC.Center, player.Center);

                if (!TpBack)
                {
                    Vector2 targetPointDiff = player.Center - NPC.Center;
                    NPC.velocity += targetPointDiff * 5f;
                    NPC.velocity.Normalize();
                    NPC.velocity *= 70;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<CoreBeam2>(), 15, 0);


                }
                else
                {
                    Vector2 targetPointDiff = player.Center - NPC.Center;
                    NPC.velocity += targetPointDiff * 5f;
                    NPC.velocity.Normalize();
                    NPC.velocity *= -70;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<CoreBeam2>(), 15, 0);



                }

                Phase = Main.rand.Next(4);
                NPC.netUpdate = true;
            }
        }
        private void DirectionalAttack()
        {
            Player player = Main.player[NPC.target];
            if (teleport < -100 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 90; i++)
                {
                    //var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 87, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, DustID.TerraBlade, default, 2f);
                    //dust.noGravity = true;
                    //dust.velocity /= 1f;
                }

                int dir1 = Main.rand.Next(4);
                int dir2 = Main.rand.Next(4);
                int dir3 = Main.rand.Next(4);
                int dir4 = Main.rand.Next(4);

                while (dir1 == dir2)
                    dir2 = Main.rand.Next(4);

                while (dir1 == dir3 || dir2 == dir3)
                    dir3 = Main.rand.Next(4);

                while (dir1 == dir4 || dir2 == dir4 || dir3 == dir4)
                    dir4 = Main.rand.Next(4);


                //int id1 = Projectile.NewProjectile(NPC.GetBossSpawnSource(NPC.target), player.Center, new Vector2(0, 0), ModContent.ProjectileType<DirectionalAttack>(), 80, 10f, NPC.whoAmI);
                //int id2 = Projectile.NewProjectile(NPC.GetBossSpawnSource(NPC.target), player.Center, new Vector2(0, 0), ModContent.ProjectileType<DirectionalAttack>(), 80, 10f, NPC.whoAmI);
                //int id3 =  Projectile.NewProjectile(NPC.GetBossSpawnSource(NPC.target), player.Center, new Vector2(0, 0), ModContent.ProjectileType<DirectionalAttack2>(), 80, 10f, NPC.whoAmI);
                //int id4 = Projectile.NewProjectile(NPC.GetBossSpawnSource(NPC.target), player.Center, new Vector2(0, 0), ModContent.ProjectileType<DirectionalAttack2>(), 80, 10f, NPC.whoAmI);

                //Main.projectile[id1].ai[0] = NPC.target;
                //Main.projectile[id1].ai[1] = dir1;

                //Main.projectile[id2].ai[0] = NPC.target;
                //Main.projectile[id2].ai[1] = dir2;

                //Main.projectile[id3].ai[0] = NPC.target;
                //Main.projectile[id3].ai[1] = dir3;

                //Main.projectile[id4].ai[0] = NPC.target;
                //Main.projectile[id4].ai[1] = dir4;

                var proj = Projectile.NewProjectileDirect(NPC.GetBossSpawnSource(NPC.target), 
                    player.Center,
                    new Vector2(0, 0), 
                    ModContent.ProjectileType<DirectionalAttack>(), 80, 10f, 0  , NPC.target, dir1);
                var proj1 = Projectile.NewProjectileDirect(NPC.GetBossSpawnSource(NPC.target), player.Center, new Vector2(0, 0), ModContent.ProjectileType<DirectionalAttack>(), 80, 10, Main.myPlayer, 0,1);
                var proj2 = Projectile.NewProjectileDirect(NPC.GetBossSpawnSource(NPC.target), player.Center, new Vector2(0, 0), ModContent.ProjectileType<DirectionalAttack2>(), 80, 10f, Main.myPlayer,0, 1);
                var proj3 = Projectile.NewProjectileDirect(NPC.GetBossSpawnSource(NPC.target), player.Center, new Vector2(0, 0), ModContent.ProjectileType<DirectionalAttack2>(), 80, 10f, Main.myPlayer,0, 1);

                proj.ai[0] = NPC.target;
                proj.ai[1] = dir1;
                proj.netUpdate = true;

                proj1.ai[0] = NPC.target;
                proj1.ai[1] = dir2;
                proj1.netUpdate = true;

                proj2.ai[0] = NPC.target;       
                proj2.ai[1] = dir3;
                proj2.netUpdate = true;

                proj3.ai[0] = NPC.target;
                proj3.ai[1] = dir4;
                proj3.netUpdate = true;

                Phase = 0;
                NPC.alpha = 0;
                NPC.dontTakeDamage = false;
            }
            else
            {
                NPC.alpha = 0;
                teleport -= 10; 
                NPC.dontTakeDamage = true;
            }
            NPC.netUpdate = true;


        }
        private void FloatingBalls()
        {
            Player player = Main.player[NPC.target];

            if (teleport < -100 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 90; i++)
                {
                    //var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 87, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, DustID.TerraBlade, default, 2f);
                    //dust.noGravity = true;
                    //dust.velocity /= 1f;
                }

                for (int i = 0; i < 8; i++)
                {
                    int id = Projectile.NewProjectile(NPC.GetBossSpawnSource(NPC.target), NPC.Center, new Vector2(0, 0), ModContent.ProjectileType<FloatingBalls>(), 40, 10);
                    Main.projectile[id].ai[3] = NPC.target;
                    Main.projectile[id].ai[4] = NPC.whoAmI;
                }

                Phase = 0;
                NPC.alpha = 0;
                NPC.dontTakeDamage = false;
            }
            else
            {
                teleport -= 10;
                NPC.dontTakeDamage = true;
            }
            NPC.netUpdate = true;
        }
        Vector2 vel;

        private void SageBeam()
        {
            Player player = Main.player[NPC.target];

            if (teleport < -100)
            {
                vel = DivUtils.FromAToB(NPC.Center, player.Center);

                for (int i = 0; i < 90; i++)
                {
                    //var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 87, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, DustID.TerraBlade, default, 2f);
                    //dust.noGravity = true;
                    //dust.velocity /= 1f;
                }

                for (int i = 0; i < 5; i++)
                {
                    int id = Projectile.NewProjectile(NPC.GetBossSpawnSource(NPC.target), NPC.Top, NPC.DirectionTo(player.Center).RotateRandom(1) * 5, ModContent.ProjectileType<SageBeam>(), 30, 10);
                    Main.projectile[id].ai[3] = NPC.target;
                    Main.projectile[id].ai[4] = NPC.whoAmI;
                }

                Phase = 0;
                NPC.alpha = 0;
                NPC.dontTakeDamage = false;
            }
            else
            {
                teleport -= 10;
                NPC.dontTakeDamage = true;
            }

            NPC.netUpdate = true;

        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }


            if (NPC.life <= 0)
            {
                NPC.NewNPC(NPC.GetSource_Death(), (int) NPC.Center.X, (int) NPC.Bottom.Y, ModContent.NPCType<SageDeath>());
            }
        }


        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * 8)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.hide)
            {
                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Texture2D tex = Terraria.GameContent.TextureAssets.Npc[Type].Value;
                var fadeMult = 1f / NPCID.Sets.TrailCacheLength[Type];
                for (int i = 0; i < NPC.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(tex, NPC.oldPos[i] - Main.screenPosition + NPC.Size / 2 + new Vector2(-75, 0), NPC.frame, Color.DarkGreen * (1f - fadeMult * i), NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0f);
                }
                Texture2D a = TextureAssets.Npc[Type].Value;
                Main.EntitySpriteDraw(a, NPC.Center - screenPos + new Vector2(-75, 0), NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, 1f, effects, 0);

            }
            return false;
        }
    }

   
}
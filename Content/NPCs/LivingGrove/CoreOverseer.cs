using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Net.Security;
using System.Threading;
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

    public class Overseer : ModNPC
    {


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
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.defense = 15 ;
            NPC.knockBackResist = 0f;

            NPC.noTileCollide = false;

            NPC.scale = 1f;
            NPC.Size = new Vector2(84, 160);

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
        bool secondPhase = false;
        
        private int teleport;

        public bool TpBack { get; private set; }


        public float Phase;
        private bool initialize;
        private Vector2 LeftPoint;
        private Vector2 MiddlePoint;
        private Vector2 RightPoint;
        public override void AI()
        {
           
            if (!initialize)
            {
                LeftPoint = NPC.TopLeft + new Vector2(11, 20);
                RightPoint = NPC.TopLeft + new Vector2(43, 8);
                MiddlePoint = NPC.TopLeft + new Vector2(71, 20);
                initialize = true;

            }
            else
            {
                Player player = Main.player[NPC.target];
                float addY = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) * 10;

                if (NPC.HasValidTarget && !player.dead)
                {

                    NPC.TargetClosest();
                }

                NPC.velocity.Y = 0f;



                NPC.TargetClosest(true);

                if (!Collision.SolidCollision(player.Center, player.width + 10, player.height + 10))
                {
                    NPC.ai[0]++;

                    if (NPC.ai[0] == 120)
                    {

                        player.KillMe(PlayerDeathReason.LegacyDefault(), 999, 0, false);
                        player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 35;

                        DivergencyDraw.SpawnExplosion(player.Center, Color.LimeGreen, DustID.PortalBoltTrail, 0);
                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                            Dust dust = Dust.NewDustPerfect(player.Center + (velocity * 80f), ModContent.DustType<Glow>(), velocity * 6f, 0, Color.LimeGreen, 0.6f);
                        }


                        Projectile.NewProjectile(player.GetSource_FromThis(), player.position, Vector2.Zero, ModContent.ProjectileType<LivingExplosion>(), 200,
                            5, Main.myPlayer);

                        int numberDust = 5;

                        for (int i = 0; i < numberDust; i++)
                        {
                            Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                            Dust dust = Dust.NewDustPerfect(player.Center + (velocity * 80f), ModContent.DustType<Glow>(), velocity * 6f, 0, Color.LimeGreen, 0.6f);
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDustPerfect(player.Center, ModContent.DustType<Smoke>(), Main.rand.NextVector2CircularEdge(1f, 1f) * 5, 0, Color.LimeGreen, 3f);
                            Dust.NewDustPerfect(player.Center, ModContent.DustType<Smoke>(), player.velocity.SafeNormalize(Vector2.One) * Main.rand.NextFloat(-1f, -4f), 0, Color.LimeGreen, 2f);

                        }
                    }
                }
                else
                {
                    if (NPC.ai[0] > 0)
                    {
                        NPC.ai[0] -= 5;
                    }
                }

                if (NPC.ai[0] > 30)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(LeftPoint + new Vector2 (0,addY) , ModContent.DustType<Glow>(), velocity * -5f, 0, Color.LimeGreen, 0.2f);
                }
                if (NPC.ai[0] > 60)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(RightPoint + new Vector2(0, addY), ModContent.DustType<Glow>(), velocity * -5f, 0, Color.LimeGreen, 0.2f);
                }
                if (NPC.ai[0] > 90)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(MiddlePoint + new Vector2(0, addY), ModContent.DustType<Glow>(), velocity * -5f, 0, Color.LimeGreen, 0.2f);
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
            if (!NPC.hide)
            {
                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Texture2D glow = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/OverseerGlow").Value;
                float addY = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) * 10;


                Texture2D a = TextureAssets.Npc[Type].Value;
                Vector2 drawOrigin = new(a.Width / 2, a.Height / 2);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Main.EntitySpriteDraw(a, NPC.Center - Main.screenPosition + new Vector2(0,addY), null, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(glow, NPC.Center - Main.screenPosition + new Vector2(0, addY), null, Color.White, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            }
            return false;
        }
    }
}

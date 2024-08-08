using Divergency.Content.Dusts;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Net.Security;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{

    public class BigGuy : ModNPC
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
            Console.WriteLine("a");
            NPC.lifeMax = 1000;
            NPC.damage = 10;
            NPC.defense = 8;
            NPC.knockBackResist = 0f;

            NPC.noTileCollide = false;

            NPC.scale = 1f;
            NPC.Size = new Vector2(150, 150);

            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;

            Console.WriteLine("b");
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
        enum ActionState
        {
            Idling,
            Lazer,
            Thorns1,
            Thorns2,
            Thorns3,
            Roar




        }
        private int teleport;

        public bool TpBack { get; private set; }

         ActionState state = ActionState.Idling;

        public float Phase;
        private bool initialize;
        public float addDistance = 90;
        private bool spawned = false;
        public override void AI()
        {
            Console.WriteLine("ai");


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

                NPC.velocity.Y = 0f;

              

                NPC.TargetClosest(true); // dosent turn quite right...


                if (state == ActionState.Idling)
                {
                    NPC.ai[0]++;

                    if (NPC.ai[0] == 120)
                    {
                        state = ActionState.Thorns1;
                        NPC.ai[0] = 0;

                    }
                }

                //////THORNS AFTER ANOTHER :)
                if (state == ActionState.Thorns1)
                {
                    NPC.ai[0]++;
                    if (NPC.ai[0] == 25)
                    {

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2 (addDistance,0), Vector2.Zero, ModContent.ProjectileType<Projectiles.Hostile.BigGuy.BigGuySpike>(), NPC.damage, 0, ai1: 1);
                        addDistance += 90;
                        NPC.ai[0] = 0;
                    }
                    
                    if (addDistance == 1440)
                    {
                        state = ActionState.Idling;
                    }
                }
            }

        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            npcHitbox.X = (int)(NPC.Size.X / 2 - 150 / 2);
            npcHitbox.Y = (int)(NPC.Size.Y / 2 - 150 / 2);
            return false;
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


        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 9)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * 6)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.hide)
            {
                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Texture2D glow = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/BigGuyGlow").Value;


                Texture2D a = TextureAssets.Npc[Type].Value;
                Main.EntitySpriteDraw(a, NPC.VisualPosition - screenPos + new Vector2(-30, 34), NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, 1f, effects, 0);
                Main.EntitySpriteDraw(glow, NPC.VisualPosition - screenPos + new Vector2(-30, 34), NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, 1f, effects, 0);

            }
            return false;
        }
    }

}
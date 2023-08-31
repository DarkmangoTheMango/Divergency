using Divergency.Content.Dusts;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{

    public class Corennector : ModNPC
    {
        int startingFrame;

        int endingFrame;

        int framerate;

        int combatFrame = 6;

        float maxSpeed = 2f;

        float attackTimer;

        float attackCooldown = 30f;

        bool attacking;

        enum State
        {
          Connect,
          Idle,
          Death

        }

        State state = State.Connect;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.damage = 30;
            NPC.defense = 5;
            NPC.knockBackResist = 0.2f;

            NPC.noTileCollide = true;
            
            NPC.scale = 0.5f;
            NPC.Size = new Vector2(44f, 54f);

            NPC.HitSound = SoundID.DD2_WitherBeastHurt;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("The most annoying creature in the groves :kojima:")
            });
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];

            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;

            Lighting.AddLight(NPC.Center, new Color(79, 214, 126).ToVector3());

            if (state == State.Connect)
            {
                //spawn 3 projectiles to the nearest enemies from 3 different points (up
            }
        }

            
        public override void OnKill()
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LivingShard>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CradleWoodFurniture>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
            }

           // if (Main.netMode != NetmodeID.Server) { Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, -3f)), Mod.Find<ModGore>("GuardianCorpse").Type, 1f); }
        }

       

           
        

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/Coreling").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

           // spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return true;
        }
    }
   
}
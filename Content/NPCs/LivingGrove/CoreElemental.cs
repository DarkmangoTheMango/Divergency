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
using Divergency.Common;

namespace Divergency.Content.NPCs.LivingGrove
{
    public class CoreElemental : ModNPC
    {
        enum State
        {
            spawn,
            moving, 
            dash,
            death
        }
        State state = State.moving;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 150;
            NPC.damage = 50;
            NPC.defense = 15;
            NPC.knockBackResist = 0.6f;

            NPC.noTileCollide = true;

            NPC.scale = 1f;
            NPC.Size = new Vector2(52f, 32f);

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
                new FlavorTextBestiaryInfoElement("He just took 3 gas station dick pills")
            });
        }

        public override void AI()
        {
            if (state == State.spawn)
            {
                //spawm limbs
                state = State.moving;

            }
            if (state == State.moving)
            {
                Player target = Main.player[NPC.target];

                NPC.TargetClosest(true);
                NPC.Move(target.Center, 10f, 100);
                NPC.rotation = NPC.velocity.X * 0.1f;

            }

        }



        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/CoreElemental").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return true;
        }
    }
}
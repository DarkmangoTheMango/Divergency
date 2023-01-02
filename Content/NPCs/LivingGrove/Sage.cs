using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{
    [AutoloadBossHead]

    public class Sage : ModNPC
    {

        float maxSpeed = 2f;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Scale = 0.5f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1000;
            NPC.damage = 30;
            NPC.defense = 20;
            NPC.knockBackResist = 0.2f;

            NPC.noTileCollide = true;

            NPC.scale = 1f;
            NPC.Size = new Vector2(168, 140);

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

        public override void AI()
        {
            Player target = Main.player[NPC.target];

            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.05f;

            Lighting.AddLight(NPC.Center, new Color(79, 214, 126).ToVector3());

            if (NPC.Distance(target.Center) <= 200f) { NPC.velocity -= NPC.DirectionTo(target.Center) * 0.05f; }
            else { NPC.velocity *= 0.98f; }

            if (NPC.Center.Y >= target.Center.Y) { NPC.velocity.Y -= 0.05f; }

            if (NPC.velocity.X >= maxSpeed) { NPC.velocity.X = maxSpeed; }
            if (NPC.velocity.X <= -maxSpeed) { NPC.velocity.X = -maxSpeed; }
            if (NPC.velocity.Y >= maxSpeed) { NPC.velocity.Y = maxSpeed; }
            if (NPC.velocity.Y <= -maxSpeed) { NPC.velocity.Y = -maxSpeed; }

            NPC.ai[0]++;
        }

        public override void OnKill()
        {
            NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<SageDeath>());
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/Sage_Glow").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 4f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, position, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
        }
    }

    [AutoloadBossHead]

    public class SageDeath : ModNPC
    {
        public override string HeadTexture => "Divergency/Content/NPCs/LivingGrove/Sage_Head_Boss";
        public override string Texture => "Divergency/Content/NPCs/LivingGrove/Sage";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sage");
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
            NPC.knockBackResist = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;

            NPC.noTileCollide = true;

            NPC.scale = 1f;
            NPC.Size = new Vector2(168, 140);

            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.05f;

            NPC.ai[0]++;
            NPC.ai[1]++;

            if (NPC.ai[0] == 120f)
            {
                NPC.velocity.Y -= 3f;
                NPC.velocity.X += NPC.direction;
            }
            
            if (NPC.ai[0] >= 120f)
            {
                NPC.velocity.Y += 0.1f;
            }
            else
            {
                if (NPC.ai[1] >= 5f)
                {
                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (Main.rand.NextVector2Circular(1f, 1f) * 100f), Vector2.Zero, ModContent.ProjectileType<SageDeathBomb>(), 0, 0f, 0, Main.rand.NextFloat(0f, 360f), 0f);

                    NPC.ai[1] = 0f;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/Sage_Glow").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 4f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, position, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
        }
    }

    public class SageDeathBomb : ModProjectile
    {
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0];
            Projectile.scale += 0.01f;

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;

                if (++Projectile.frame >= 5)
                {
                    Projectile.frame = 0;
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
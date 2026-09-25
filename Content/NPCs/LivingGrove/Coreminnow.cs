using Divergency.Content.Biomes;
using Divergency.Content.Dusts;
using Divergency.Content.Items.Consumable;
using System;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace Divergency.Content.NPCs.LivingGrove;

public class Coreminnow : ModNPC
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 6;
        Main.npcCatchable[Type] = true;
        NPCID.Sets.CountsAsCritter[Type] = true;
        NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
        NPCID.Sets.TownCritter[Type] = true;

        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new()
        {
            IsWet = true
        });
    }

    public override void SetDefaults()
    {
        NPC.Size = new(24);
        NPC.lifeMax = 5;
        NPC.catchItem = ModContent.ItemType<CorebettaItem>();
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.noGravity = true;
        NPC.aiStyle = -1;

        SpawnModBiomes = [ModContent.GetInstance<LivingCoreBiome>().Type];
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(
        [
            new FlavorTextBestiaryInfoElement("fish!"),
            new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<LivingCoreBiome>().ModBiomeBestiaryInfoElement)
        ]);
    }

    public override void AI()
    {
        if (NPC.direction == 0)
            NPC.TargetClosest();

        if (NPC.wet)
        {
            if (NPC.collideX)
            {
                NPC.velocity.X *= -1f;
                NPC.direction *= -1;
                NPC.netUpdate = true;
            }

            var tilePos = NPC.Bottom.ToTileCoordinates();

            if (Main.tile[tilePos.X, tilePos.Y].TopSlope)
                if (Main.tile[tilePos.X, tilePos.Y].RightSlope)
                    NPC.direction = 1;
                else
                    NPC.direction = -1; 
            
            if (Main.tile[tilePos.X, tilePos.Y - 1].LiquidAmount > 128)
                if (Main.tile[tilePos.X, tilePos.Y + 1].HasTile)
                    NPC.ai[0] = -Main.rand.NextFloat(2, 10);
                else if (Main.tile[tilePos.X, tilePos.Y + 2].HasTile)
                    NPC.ai[0] = -Main.rand.NextFloat(2, 10);

            NPC.ai[0] *= 0.98f;

            NPC.velocity.X = NPC.direction * 2f;
            NPC.velocity.Y = (MathF.Sin(Main.GameUpdateCount * 0.02f + NPC.whoAmI) * 0.4f) + NPC.ai[0];
            NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction < 0 ? MathHelper.Pi : 0);
            NPC.direction = Math.Sign(NPC.velocity.X);
        }
        else
        {
            NPC.ai[0] = 0;

            if (NPC.collideY)
            {
                NPC.velocity.X = Main.rand.NextFloat(-2, 2);
                NPC.velocity.Y = Main.rand.NextFloat(-5, -2);
            }

            NPC.velocity.Y = Math.Clamp(NPC.velocity.Y + 0.3f, -3, 10);
            NPC.rotation = MathHelper.Clamp(NPC.velocity.Y * 0.1f, -0.2f, 0.2f);
            NPC.direction = Math.Sign(NPC.velocity.X);
        }

        NPC.spriteDirection = NPC.direction;
    }

    public override void OnKill()
    {
        for (int i = 0; i < 5; i++)
        {
            Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LivingShard>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
            Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CradleWoodFurniture>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
        }
    }

    public override void FindFrame(int frameHeight)
    {
        var startingFrame = 3;
        var endingFrame = 5;
        var framerate = 9;

        if (NPC.wet)
        {
            startingFrame = 0;
            endingFrame = 2;
        }

        if (NPC.frameCounter++ >= framerate)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y += frameHeight;

            if (NPC.frame.Y > endingFrame * frameHeight)
                NPC.frame.Y = startingFrame * frameHeight;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;

        Vector2 position = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY);

        SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        Rectangle frame = new(0, NPC.frame.Y, NPC.frame.Width / 2, NPC.frame.Height);
        Rectangle glowFrame = frame with { X = NPC.frame.Width / 2 };

        Main.EntitySpriteDraw(texture, position, frame, NPC.GetAlpha(drawColor), NPC.rotation, frame.Size() / 2, NPC.scale, spriteEffects, 1f);
        Main.EntitySpriteDraw(texture, position, glowFrame, NPC.GetAlpha(Color.White), NPC.rotation, glowFrame.Size() / 2, NPC.scale, spriteEffects, 1f);

        return false;
    }
}
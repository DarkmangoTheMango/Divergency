using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class SavageTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Critical strikes inflict Shred \n7% increased critical strike chance");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.Size = new Vector2(50, 52);
            Item.scale = 1f;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        float timer;

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Items/Accessories/SavageTalismanGlow").Value;
            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null) { frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]); }
            else { frame = texture.Frame(); }

            Vector2 origin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - origin.X, Item.height - frame.Height);
            Vector2 position = Item.position - Main.screenPosition + origin + offset;

            if (timer >= MathHelper.TwoPi) { timer = 0f; }
            timer += 0.02f;

            for (int i = 0; i < 3; i++) { spriteBatch.Draw(texture, position + Vector2.One.RotatedBy(timer + ((2 * i))) * 2f, frame, new Color(222, 0, 13, 100), rotation, origin, scale, SpriteEffects.None, 0f); }

            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Items/Accessories/SavageTalismanGlow").Value;

            if (timer >= MathHelper.TwoPi) { timer = 0f; }
            timer += 0.01f;

            for (int i = 0; i < 3; i++) { spriteBatch.Draw(texture, position + Vector2.One.RotatedBy(timer + ((2 * i))), frame, new Color(222, 0, 13, 100), 0f, origin, scale, SpriteEffects.None, 0f); }

            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += 7;
            player.GetModPlayer<SavageTalismanPlayer>().savageTalismanActive = true;
        }
    }

    public class SavageTalismanPlayer : ModPlayer
    {
        public bool savageTalismanActive;

        public override void ResetEffects() => savageTalismanActive = false;

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (crit && savageTalismanActive)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Impacts/Fleshy"), target.Center);
                target.AddBuff(ModContent.BuffType<Shred>(), 180);

                for (int i = 0; i < 10; i++) { Dust.NewDustPerfect(target.Center, ModContent.DustType<ShredBlood>(), Main.rand.NextVector2Circular(1f, 1f) * 5f, 0, default, 2f).noGravity = true; }
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (crit && savageTalismanActive)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Impacts/Fleshy"), target.Center);
                target.AddBuff(ModContent.BuffType<Shred>(), 180);

                for (int i = 0; i < 10; i++) { Dust.NewDustPerfect(target.Center, ModContent.DustType<ShredBlood>(), Main.rand.NextVector2Circular(1f, 1f) * 5f, 0, default, 2f).noGravity = true; }
            }
        }
    }
}
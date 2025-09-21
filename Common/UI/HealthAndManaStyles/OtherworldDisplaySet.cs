using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace Divergency.Common.UI.HealthAndManaStyles
{
    public class OtherworldDisplaySet : ModResourceDisplaySet
    {
        private Asset<Texture2D> _lifeBar;
        private Asset<Texture2D> _lifeFill;
        private Asset<Texture2D> _lifeFillB; // "bonus" / fruit variant
        private Asset<Texture2D> _manaBar;
        private Asset<Texture2D> _manaFill;

        private PlayerStatsSnapshot preparedSnapshot;

        private bool _hpHovered;
        private bool _mpHovered;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            string path = "Divergency/Common/UI/HealthAndManaStyles/";

            _lifeBar = ModContent.Request<Texture2D>(path + "Life");
            _lifeFill = ModContent.Request<Texture2D>(path + "Life_Fill");
            _lifeFillB = ModContent.Request<Texture2D>(path + "Life_Fill_B");

            _manaBar = ModContent.Request<Texture2D>(path + "Mana");
            _manaFill = ModContent.Request<Texture2D>(path + "Mana_Fill");
        }

        public override void PreDrawResources(PlayerStatsSnapshot snapshot)
        {
            preparedSnapshot = snapshot;
        }

        public override void DrawLife(SpriteBatch spriteBatch)
        {
            int margin = 20;
            int life = preparedSnapshot.Life;
            int lifeMax = preparedSnapshot.LifeMax;
            string text = $"Life: {life}/{lifeMax}";

            // measure text width
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text);

            int hearts = preparedSnapshot.AmountOfLifeHearts;
            int lifePerHeart = 20;
            int fullHearts = life / lifePerHeart;
            int partialLife = life % lifePerHeart;

            // total width = text + hearts
            float totalWidth = textSize.X + (hearts * _lifeBar.Width());

            // anchor to right
            Vector2 basePos = new Vector2(Main.screenWidth - margin - totalWidth, margin);

            // draw text
            Utils.DrawBorderString(spriteBatch, text, basePos, Color.White);

            // draw hearts immediately after text
            Vector2 pos = basePos + new Vector2(textSize.X, 0);

            for (int i = 0; i < hearts; i++)
            {
                Vector2 drawPos = pos + new Vector2(i * _lifeBar.Width(), 0);

                // background
                spriteBatch.Draw(_lifeBar.Value, drawPos, Color.White);

                // full heart
                if (i < fullHearts)
                {
                    spriteBatch.Draw(_lifeFill.Value, drawPos, Color.White);
                }
                // partially filled heart
                else if (i == fullHearts && partialLife > 0)
                {
                    int fillWidth = (int)(_lifeFill.Width() * (partialLife / (float)lifePerHeart));
                    Rectangle sourceRect = new Rectangle(0, 0, fillWidth, _lifeFill.Height());
                    spriteBatch.Draw(_lifeFill.Value, drawPos, sourceRect, Color.White);
                }

                // bonus hearts (fruit upgrade)
                if (i < preparedSnapshot.LifeFruitCount)
                {
                    spriteBatch.Draw(_lifeFillB.Value, drawPos, Color.White);
                }
            }

            _hpHovered = false;
        }

        public override void DrawMana(SpriteBatch spriteBatch)
        {
            int margin = 20;
            int mana = preparedSnapshot.Mana;
            int manaMax = preparedSnapshot.ManaMax;
            string text = $"Mana: {mana}/{manaMax}";

            // measure text width
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text);

            int stars = preparedSnapshot.AmountOfManaStars;
            int manaPerStar = 20;
            int fullStars = mana / manaPerStar;
            int partialMana = mana % manaPerStar;

            // total width = text + stars
            float totalWidth = textSize.X + (stars * _manaBar.Width());

            // anchor to right, below life row
            Vector2 basePos = new Vector2(Main.screenWidth - margin - totalWidth, margin + _lifeBar.Height() + 10);

            // draw text
            Utils.DrawBorderString(spriteBatch, text, basePos, Color.White);

            // draw stars immediately after text
            Vector2 pos = basePos + new Vector2(textSize.X, 0);

            for (int i = 0; i < stars; i++)
            {
                Vector2 drawPos = pos + new Vector2(i * _manaBar.Width(), 0);

                // background
                spriteBatch.Draw(_manaBar.Value, drawPos, Color.White);

                // full star
                if (i < fullStars)
                {
                    spriteBatch.Draw(_manaFill.Value, drawPos, Color.White);
                }
                // partially filled star
                else if (i == fullStars && partialMana > 0)
                {
                    int fillWidth = (int)(_manaFill.Width() * (partialMana / (float)manaPerStar));
                    Rectangle sourceRect = new Rectangle(0, 0, fillWidth, _manaFill.Height());
                    spriteBatch.Draw(_manaFill.Value, drawPos, sourceRect, Color.White);
                }
            }

            _mpHovered = false;
        }

        public override bool PreHover(out bool hoveringLife)
        {
            hoveringLife = _hpHovered;
            return _hpHovered || _mpHovered;
        }
    }
}

using Terraria.ModLoader;

namespace Divergency.Common.Players
{
    public class ComboSystem : ModPlayer
    {
        public int itemCombo;
        public int itemComboReset;
        public int lastSelectedItem;

        public override void ResetEffects()
        {
            if (itemComboReset <= 0)
            {
                itemCombo = 0;
                itemComboReset = 0;
            }
            else { itemComboReset--; }
        }

        public override bool PreItemCheck()
        {
            if (Player.selectedItem != lastSelectedItem)
            {
                itemComboReset = 0;
                itemCombo = 0;
                lastSelectedItem = Player.selectedItem;
            }

            if (itemComboReset > 0)
            {
                itemComboReset--;

                if (itemComboReset == 0) { itemCombo = 0; }
            }

            return true;
        }
    }
}
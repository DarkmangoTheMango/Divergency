using Divergency.Assets.Backgrounds;
using ReLogic.Content;

namespace Divergency;

public class DivergencyMenu : ModMenu
{
    public override Asset<Texture2D> Logo => Mod.Assets.Request<Texture2D>("Assets/Textures/Title");

    public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<LivingCoreBiomeSurfaceStyle>();

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/MainMenu");

    public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
    {
        drawColor = Color.White;

        return base.PreDrawLogo(spriteBatch, ref logoDrawCenter, ref logoRotation, ref logoScale, ref drawColor);
    }
}

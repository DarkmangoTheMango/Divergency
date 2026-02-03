using System;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;

namespace Divergency;

public class ScreenShakeModifier : ICameraModifier
{
    public string UniqueIdentity
    {
        get;
        private set;
    }

    public bool Finished
    {
        get;
        private set;
    }

    private float Intensity;
    private readonly float Fade;
    private readonly Vector2? Source;
    private readonly float MaxDistance;

    public ScreenShakeModifier(float intensity, float fade = 0.9f, Vector2? source = null, float maxDistance = 1000f, string uniqueIdentity = "DivergencyScreenShake")
    {
        Intensity = intensity;
        Fade = fade;
        Source = source;
        MaxDistance = maxDistance;
        UniqueIdentity = uniqueIdentity;
    }

    public void Update(ref CameraInfo cameraInfo)
    {
        if (Main.gamePaused || Main.gameInactive)
            return;

        float effectiveIntensity = Intensity;

        if (Source.HasValue && Main.LocalPlayer?.active == true)
        {
            float dist = Vector2.Distance(Main.LocalPlayer.Center, Source.Value);
            float falloff = 1f - MathHelper.Clamp(dist / MaxDistance, 0f, 1f);
            effectiveIntensity *= falloff;
        }

        if (effectiveIntensity > 0f)
        {
            cameraInfo.CameraPosition += Main.rand.NextVector2Circular(effectiveIntensity, effectiveIntensity);
        }

        Intensity *= Fade;

        if (Intensity < 0.05f)
            Finished = true;
    }
}

public class CameraSystem : ModSystem
{
    public static void ScreenShake(float intensity = 8f, float fade = 0.9f, Vector2? source = null, float maxDistance = 1000f)
    {
        if (Main.dedServ)
            return;

        Main.instance.CameraModifiers.Add(new ScreenShakeModifier(intensity, fade, source, maxDistance));
    }

    public override void ModifyScreenPosition()
    {
        Player player = Main.LocalPlayer;
        if (!isChangingCameraPos)
        {
            zoomBefore = Main.GameZoomTarget;
        }
        if (isChangingCameraPos)
        {
            if (CameraChangeLength > 0)
            {
                if (zoomAmount != 1 && zoomAmount > zoomBefore)
                {
                    Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget + 0.05f, 1f, zoomAmount);
                }
                if (CameraChangeTransition <= 1f)
                {
                    Main.screenPosition = Vector2.SmoothStep(cameraChangeStartPoint, CameraChangePos, CameraChangeTransition += 0.025f);
                }
                else
                {
                    Main.screenPosition = CameraChangePos;
                }
                CameraChangeLength--;
            }
            else if (CameraChangeTransition >= 0)
            {
                if (Main.GameZoomTarget != zoomBefore)
                {
                    Main.GameZoomTarget -= 0.05f;
                }
                Main.screenPosition = Vector2.SmoothStep(player.Center - new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f), CameraChangePos, CameraChangeTransition -= 0.05f);
            }
            else
            {
                isChangingCameraPos = false;
            }
        }
    }

    float zoomBefore;
    public static float zoomAmount;
    public static Vector2 cameraChangeStartPoint;
    public static Vector2 CameraChangePos;
    public static float CameraChangeTransition;
    public static int CameraChangeLength;
    public static bool isChangingCameraPos;
    public static void Zoom(Vector2 pos, int length, float zoom = 1.65f)
    {
        cameraChangeStartPoint = Main.screenPosition;
        CameraChangeLength = length;
        CameraChangePos = pos - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
        isChangingCameraPos = true;
        CameraChangeTransition = 0;
        if (Main.GameZoomTarget < zoom)
            zoomAmount = zoom;
    }

    public static float FlashIntensity = 0f;
    public static float FlashFade = 0f;

    public override void PostDrawInterface(SpriteBatch spriteBatch)
    {
        if (FlashIntensity > 0f)
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * FlashIntensity);
    }

    public override void PostUpdateEverything()
    {
        if (FlashIntensity > 0f)
        {
            FlashIntensity -= FlashFade;
            if (FlashIntensity < 0f)
                FlashIntensity = 0f;
        }
    }

    public static void ScreenFlash(float intensity = 1f, float fade = 0.01f)
    {
        FlashIntensity = Utils.Clamp(intensity, 0f, 1f);
        FlashFade = fade;
    }
}
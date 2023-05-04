static const float PI = 3.14159265f;

sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2;
sampler uImage3;
float3 uColor;
float uOpacity;
float3 uSecondaryColor;
float uTime;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uImageOffset;
float uIntensity;
float uProgress;
float2 uDirection;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;

float3 Pos   : Pos;
float2 Scale : Scale;

float fov: FOV = 60;

float4 PixelShaderFunction(float2 TexCoord : TEXCOORD) : COLOR
{

    float4 curWorldSpacePoint = {
        TexCoord.x * Scale.x + Pos.x,
        TexCoord.y * Scale.y + Pos.y,
        Pos.z,
        1
    };

    float FRI = 1.0 / tan(fov * 0.5 / 180 * PI);

    float4x4 ProjMat = {
        FRI, 0, 0,      0,
        0, FRI, 0,      0,
        0, 0,  -0.002,  0.81,
        0, 0,   1, 0
    };

    /* float4x4 ModelMat = { */
    /*     Scale.x, 0, 0, Pos.x, */
    /*     0, Scale.y, 0, Pos.y, */
    /*     0, 0, Scale.z, Pos.z, */
    /*     0, 0, 0, 1 */
    /* }; */

    float4 ProjectedPosTmp = mul(ProjMat, curWorldSpacePoint);
    float3 ProjectedPos = ProjectedPosTmp.xyz / ProjectedPosTmp.w;

    /* ProjectedPos.x = (ProjectedPos.x) / 4; */
    /* ProjectedPos.y = (ProjectedPos.y) / 4; */

    /* TexCoord = (TexCoord - (Pos - (Scale/2)))/Scale; */
    /* TexCoord = (TexCoord - (ProjectedPos - (Scale/2)))/Scale; */
    TexCoord = TexCoord-ProjectedPos;

    if(TexCoord.x < 0 || TexCoord.x > 1 || TexCoord.y < 0 || TexCoord.y > 1) {
        return float4(1, 0, 1, 1);
        /* discard; */
    }

    return tex2D(uImage0, TexCoord);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

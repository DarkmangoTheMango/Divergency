float uTime;
float2 uSourceRect;

float alpha;

texture2D tex;

sampler2D uImage0 = sampler_state
{
    Texture = <tex>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D uImage1 = sampler_state
{
    Texture = <tex>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

float4 Main(float2 coords : TEXCOORD0) : COLOR0
{
    coords.y = 1.0 - coords.y;
    
    float2 centeredUV = coords * 2.0 - 1.0;
    float r2 = dot(centeredUV, centeredUV);
    
    centeredUV *= 1.0 - 0.2 * r2;
    centeredUV = centeredUV * 0.5 + 0.5;
    
    float2 warp = tex2D(uImage1, coords + float2(uTime, 0)).xy * 0.1;
    float2 timeOffset = float2(uTime, 0);
    
    float4 color = tex2D(uImage1, float2(coords.x, coords.x + timeOffset.x)) + tex2D(uImage0, centeredUV + warp + timeOffset) + tex2D(uImage0, centeredUV * 0.5 + timeOffset);
    
    float vignette = smoothstep(0.6, 2, r2);
    color *= (1.0 - vignette) * pow(coords.y, 4);
    
    float4 tint = float4(0.3, 0.5, 0.8, 1.0);
    color *= tint;
    color.a = alpha;

    return color * alpha;
}

Technique technique1
{
    pass Storm
    {
        PixelShader = compile ps_3_0 Main();
    }
}
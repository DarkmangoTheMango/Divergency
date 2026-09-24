float uTime;

float4 uSourceRect;

texture2D tex;

sampler2D uImage0 = sampler_state
{
    Texture = <tex>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

float4 Main(float2 coords : TEXCOORD0) : COLOR0
{
    float2 centeredUV = coords * 2.0 - 1.0;
    float radius = length(centeredUV);
    
    float2 polarUV = float2(atan2(centeredUV.y, centeredUV.x) / (2.0 * 3.141), pow(radius, 0.2) - uTime);
    
    float4 color = float4(0, 0.1, 0.1, 1) + tex2D(uImage0, polarUV) * (float4(2, 4, 1, 1) * smoothstep(0.2, 1, radius));
    color *= smoothstep(0.85, 0.75, radius);
    
    return color;
}

Technique technique1
{
    pass WraithOrb
    {
        PixelShader = compile ps_2_0 Main();
    }
}
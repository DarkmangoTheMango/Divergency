// -*- mode: hlsl; hlsl-entry: main; hlsl-target: ps_6_0; hlsl-args: ; -*-
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexCoords : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
    float2 TexCoords : TEXCOORD0;
};

VertexShaderOutput PolygonBatchEffectVS(in VertexShaderInput input) 
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Color = input.Color;
    output.Position = mul(mul(mul(input.Position, World), View), Projection);
    output.TexCoords = input.TexCoords;
    return output;
    /*float4 colour = tex2D(uImage0, coords);
    return colour;*/
}

float4 PolygonBatchEffectPS(in VertexShaderOutput input) : COLOR0
{
    return input.Color;
    /*float4 colour = tex2D(uImage0, coords);
    return colour;*/
}

technique Technique1
{
    pass PolygonBatchEffectPS
    {
        PixelShader = compile ps_2_0 PolygonBatchEffectPS();
        VertexShader = compile vs_2_0 PolygonBatchEffectVS();
    }
}

float4x4 WorldMatrix;
float4x4 ViewMatrix;
float4x4 ProjectionMatrix;

float4 AmbienceColor = float4(0.5f, 0.5f, 0.5f, 1.0f);

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, WorldMatrix);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.Position = mul(viewPosition, ProjectionMatrix);

    return input;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return AmbienceColor;
}

technique Testing
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
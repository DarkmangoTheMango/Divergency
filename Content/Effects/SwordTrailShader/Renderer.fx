sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;

float rotation : rot;
float length : len;
int above : dir;

// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    uv -= 0.5;
    uv *= 2;

	float4 color = tex2D(uImage0, uv);

    float len = sqrt(uv.x*uv.x + uv.y*uv.y);
    float r = atan2(uv.y, uv.x) - rotation;
    
    if (len > 1 || r < 0 || r > 0.6){
        discard;
    }

	return float4(1, 1, 1, len);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
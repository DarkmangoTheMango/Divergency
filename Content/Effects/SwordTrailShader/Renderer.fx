#define PI 3.14159265

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
float maxlength : mlen;
int direction : dir;

int type : type;

// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    uv -= 0.5;
    uv *= 2;

    float len = sqrt(uv.x*uv.x + uv.y*uv.y);
    float r = fmod(atan2(uv.y, uv.x) - rotation, PI*2.0); // - rotation;

    if (r < 0)
        r += PI*2.0;

    if (direction > 0)
        r = abs(PI*2.0 - r);

    if (len > 1 || r > length)
        discard;
        
	float4 color = tex2D(uImage0, float2(0, 1-len));

    if (type == 1)
        color.w = sqrt(max(color.w - r/maxlength, 0));
    else if (type == 2)
        color.w = max(color.w - r/maxlength, 0);
    

	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
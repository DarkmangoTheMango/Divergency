// taken from another mod i made lol! - darkmango

float uTime;
float intensity;
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

static const int NUM_STOPS = 4;

static const float stops[NUM_STOPS] =
{
    0.0,
    0.25,
    0.5,
    0.75
};

static const float3 colors[NUM_STOPS] =
{
    float3(0.0, 0.1, 0.1),
    float3(0.0, 0.4, 0.1),
    float3(0.6, 1.0, 0.0),
    float3(1.0, 1.0, 1.0)
};

float3 GradientMap(float t)
{
    t = saturate(t);

    if (t <= stops[0])
        return colors[0];
    if (t >= stops[NUM_STOPS - 1])
        return colors[NUM_STOPS - 1];

    for (int i = 0; i < NUM_STOPS - 1; i++)
    {
        if (t >= stops[i] && t < stops[i + 1])
        {
            float f = (t - stops[i]) / (stops[i + 1] - stops[i]);
            return lerp(colors[i], colors[i + 1], f);
        }
    }

    return colors[NUM_STOPS - 1];
}

float4 Main(float2 coords : TEXCOORD0) : COLOR0
{
    float2 centeredUV = coords * 2.0 - 1.0;
    
    float radius = length(centeredUV);
    float angle = atan2(centeredUV.y, centeredUV.x) / (2.0 * 3.141);
    angle = frac(angle + 1.0);
    
    float scrollSpeed = 0.4;
    float scroll = frac(radius - uTime * scrollSpeed);

    float2 polarUV = float2(angle, scroll);
    
    float2 warpSampleUV = polarUV + uTime * 0.1;
    float2 warp = tex2D(uImage0, warpSampleUV).rg - 0.5;
    warp *= 0.15;

    float2 finalUV = polarUV + warp;
    float4 texColor = tex2D(uImage0, finalUV);
    
    float luminance = texColor.r;
    float3 gradientColor = GradientMap(luminance);
    
    float edging = smoothstep(1.0, 0.95, radius);
    
    float ringGlow1 = smoothstep(0.7, 1.0, radius);
    float ringGlow2 = smoothstep(0.9, 1.0, radius);

    gradientColor += colors[1] * ringGlow1;
    gradientColor += colors[2] * ringGlow2;
    
    float maskRadius = lerp(0.0, 1.0, intensity);
    float visibility = step(maskRadius, luminance * radius);
    
    float3 finalColor = gradientColor * edging * visibility;
    float alpha = texColor.a * edging * visibility;

    return float4(finalColor, alpha);
}

Technique technique1
{
    pass Supernova
    {
        PixelShader = compile ps_3_0 Main();
    }
}
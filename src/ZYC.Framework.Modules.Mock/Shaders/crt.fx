//!WARNING Design by chatgpt 5.4

sampler2D inputSampler : register(s0);

float Time               : register(c0);
float Distortion         : register(c1);
float ScanlineIntensity  : register(c2);
float NoiseAmount        : register(c3);
float Width              : register(c4);
float Height             : register(c5);

float rand(float2 p)
{
    return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
}

float2 Barrel(float2 uv)
{
    float2 p = uv * 2.0 - 1.0;
    float r2 = dot(p, p);

    p *= 1.0 + Distortion * r2;

    return p * 0.5 + 0.5;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    uv.x += sin(uv.y * 80.0 + Time * 7.0) * 0.0008;

    float2 duv = Barrel(uv);

    if (duv.x < 0.0 || duv.x > 1.0 || duv.y < 0.0 || duv.y > 1.0)
    {
        return float4(0, 0, 0, 1);
    }

    float4 color = tex2D(inputSampler, duv);

    float scan = 1.0 - ScanlineIntensity * (0.5 + 0.5 * sin(duv.y * Height * 1.15));

    float n = rand(float2(duv.x + Time * 0.37, duv.y - Time * 0.21));
    float noise = lerp(1.0, n, NoiseAmount);

    float2 v = duv * 2.0 - 1.0;
    float vignette = saturate(1.12 - dot(v, v) * 0.42);

    color.rgb *= scan;
    color.rgb *= noise;
    color.rgb *= vignette;

    return color;
}
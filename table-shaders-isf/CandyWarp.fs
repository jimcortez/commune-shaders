/*{
    "CATEGORIES": [],
    "CREDIT": "Jim Cortez - Commune Project (Original: mojovideotech)",
    "DESCRIPTION": "Generates vibrant candy-colored warping spiral patterns with dynamic distortion effects. Creates mesmerizing concentric rings that warp and morph over time, featuring customizable color palettes, spiral density, and distortion parameters that produce psychedelic, organic visual patterns.",
    "INPUTS": [
        {
            "DEFAULT": 84.0,
            "MAX": 100.0,
            "MIN": 10.0,
            "NAME": "scale",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.4,
            "MAX": 0.99,
            "MIN": 0.01,
            "NAME": "cycle",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.1,
            "MAX": 1.0,
            "MIN": -0.5,
            "NAME": "thickness",
            "TYPE": "float"
        },
        {
            "DEFAULT": 61.0,
            "MAX": 100.0,
            "MIN": 10.0,
            "NAME": "loops",
            "TYPE": "float"
        },
        {
            "DEFAULT": 2.5,
            "MAX": 5.0,
            "MIN": -5.0,
            "NAME": "warp",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.33,
            "MAX": 0.5,
            "MIN": -0.5,
            "NAME": "hue",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.1,
            "MAX": 0.5,
            "MIN": -0.5,
            "NAME": "tint",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1.25,
            "MAX": 3.0,
            "MIN": -3.0,
            "NAME": "rate",
            "TYPE": "float"
        },
        {
            "DEFAULT": false,
            "NAME": "invert",
            "TYPE": "bool"
        }
    ],
    "ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: mojovideotech
- Original Shader: glslsandbox.com/e#38710.0
- Original Posted by: Trisomie21
- Modified by: @hintz
- Source: Originally sourced from editor.isf.video - CandyWarp by mojovideotech
- Description: Candy-colored warping spiral patterns with customizable parameters
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0
- Features: Scale, cycle, thickness, loops, warp, hue, tint, rate, and invert controls
*/

void main(void)
{
    float s = RENDERSIZE.y / scale;
    float radius = RENDERSIZE.x / cycle;
    float gap = s * (1.0 - thickness);
    vec2 pos = gl_FragCoord.xy - RENDERSIZE.xy * 0.5;
    float d = length(pos);
    float T = TIME * rate;
    d += warp * (sin(pos.y * 0.25 / s + T) * sin(pos.x * 0.25 / s + T * 0.5)) * s * 5.0;
    float v = mod(d + radius / (loops * 2.0), radius / loops);
    v = abs(v - radius / (loops * 2.0));
    v = clamp(v - gap, 0.0, 1.0);
    d /= radius - T;
    vec3 m = fract((d - 1.0) * vec3(loops * hue, -loops, loops * tint) * 0.5);
    if (invert)     gl_FragColor = vec4(m / v, 1.0);
    else gl_FragColor = vec4(m * v, 1.0);
} 
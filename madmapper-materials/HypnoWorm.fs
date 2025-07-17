/*{
    "CREDIT": "Adapted by Shader Shaper; Original: mojovideotech; Commune Project (Jim Cortez); see table-shaders-isf/Hypno Worm.fs",
    "DESCRIPTION": "Hypno Worm shader adapted for MadMapper Materials with all original ISF parameters, generator-driven animation, and improved compatibility.",
    "CATEGORIES": ["generator", "spiral", "opart"],
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_center",      "TYPE": "point2D", "DEFAULT": [1, 1], "MAX": [2.5, 2.5], "MIN": [-0.5, -0.5] },
        { "NAME": "mat_size",        "TYPE": "float",   "DEFAULT": 8.95,   "MIN": 1.0,   "MAX": 10.0 },
        { "NAME": "mat_color1",      "TYPE": "color",   "DEFAULT": [0.7, 0.1, 1.0, 1] },
        { "NAME": "mat_color2",      "TYPE": "color",   "DEFAULT": [0.2, 0.8, 0.7, 1] },
        { "NAME": "mat_edge",        "TYPE": "float",   "DEFAULT": 23.0,   "MIN": 3.0,   "MAX": 30.0 },
        { "NAME": "mat_gamma",       "TYPE": "float",   "DEFAULT": 0.9,    "MIN": 0.5,   "MAX": 3.0 },
        { "NAME": "mat_amp1",        "TYPE": "float",   "DEFAULT": 0.1,    "MIN": 0.01,  "MAX": 0.25 },
        { "NAME": "mat_amp2",        "TYPE": "float",   "DEFAULT": 0.05,   "MIN": 0.04,  "MAX": 0.25 },
        { "NAME": "mat_freq1",       "TYPE": "float",   "DEFAULT": -0.5,   "MIN": -10.0, "MAX": 10.0 },
        { "NAME": "mat_freq2",       "TYPE": "float",   "DEFAULT": 1.0,    "MIN": -10.0, "MAX": 10.0 },
        { "NAME": "mat_phase1",      "TYPE": "float",   "DEFAULT": 0.15,   "MIN": -2.0,  "MAX": 2.0 },
        { "NAME": "mat_phase2",      "TYPE": "float",   "DEFAULT": -1.05,  "MIN": -2.0,  "MAX": 2.0 },
        { "NAME": "mat_rate",        "TYPE": "float",   "DEFAULT": 1.5,    "MIN": -5.0,  "MAX": 5.0 }
    ],
    "GENERATORS": [
        { "NAME": "mat_genTime", "TYPE": "time_base", "PARAMS": { "speed": "mat_rate" } }
    ]
}*/
/*
ORIGINAL SHADER INFORMATION:
- Original Author: mojovideotech
- Original Shader: Hypno Worm
- Adapted by: Jim Cortez - Commune Project
- Source: Originally sourced from editor.isf.video - Hypno Worm by mojovideotech
- Description: Animated spiral OpArt worm
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0
- Features: Spiral animation, OpArt, color and shape controls
*/

#define MAX_ITER 50

vec4 materialColorForPixel(vec2 texCoord) {
    // ISF-style coordinate normalization and centering
    vec2 uv = (texCoord * RENDERSIZE.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
    uv -= mat_center;
    uv *= 11.0 - mat_size;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    vec2 pos = mat_center / RENDERSIZE.xy;
    float T = mat_genTime;
    vec3 fc = vec3(mat_color1.r, mat_color1.b, mat_color1.g); // match original ISF's color1.rbg
    for (float i = float(MAX_ITER); i > 0.0; --i) {
        pos += vec2(
            sin(T + (mat_freq1 - i) * mat_phase2) * mat_amp1,
            cos(T + (mat_freq2 - i) * mat_phase1) * mat_amp2
        );
        float dist = length(pos - uv) - 0.1 * i;
        float o = 1.0 - smoothstep(-0.01, mat_edge / RENDERSIZE.x, dist);
        fc = mix(fc, vec3(1.0) * (1.0 - mod(i, 2.0)), o);
    }
    vec3 cc = vec3(mat_color1.g, mat_color1.b, mat_color1.r) * mat_color2.rgb;
    vec4 col = vec4(pow(mat_color2.rgb, fc.rgb) - cc, 1.0);
    col = pow(col, vec4(1.0 / mat_gamma));
    return clamp(col, 0.0, 1.0);
} 
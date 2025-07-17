/*{
    "CREDIT": "Adapted by Shader Shaper; Original: mojovideotech; Commune Project (Jim Cortez); see table-shaders-isf/CandyWarp.fs",
    "DESCRIPTION": "CandyWarp shader adapted for MadMapper Materials with all original ISF parameters, generator-driven animation, and improved compatibility.",
    "CATEGORIES": ["generator", "warp"],
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_scale",     "TYPE": "float", "DEFAULT": 84.0, "MIN": 10.0, "MAX": 100.0 },
        { "NAME": "mat_cycle",     "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.01, "MAX": 0.99 },
        { "NAME": "mat_thickness", "TYPE": "float", "DEFAULT": 0.1,  "MIN": -0.5, "MAX": 1.0 },
        { "NAME": "mat_loops",     "TYPE": "float", "DEFAULT": 61.0, "MIN": 10.0, "MAX": 100.0 },
        { "NAME": "mat_warp",      "TYPE": "float", "DEFAULT": 2.5,  "MIN": -5.0, "MAX": 5.0 },
        { "NAME": "mat_hue",       "TYPE": "float", "DEFAULT": 0.33, "MIN": -0.5, "MAX": 0.5 },
        { "NAME": "mat_tint",      "TYPE": "float", "DEFAULT": 0.1,  "MIN": -0.5, "MAX": 0.5 },
        { "NAME": "mat_rate",      "TYPE": "float", "DEFAULT": 1.25, "MIN": -3.0, "MAX": 3.0 },
        { "NAME": "mat_invert",    "TYPE": "bool",  "DEFAULT": false }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_genTime",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_rate" }
        }
    ]
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

#define PI 3.1415927

vec4 materialColorForPixel(vec2 texCoord) {
    // Compute fragment coordinate from normalized texCoord.
    vec2 fragCoord = texCoord * RENDERSIZE;
    
    // Compute scale and cycle-based values.
    float s = RENDERSIZE.y / mat_scale;
    float radius = RENDERSIZE.x / mat_cycle;
    float gap = s * (1.0 - mat_thickness);
    
    // Center the position.
    vec2 pos = fragCoord - RENDERSIZE * 0.5;
    float d = length(pos);
    
    // Animation time variable using generator-driven time.
    float T = mat_genTime;
    
    // Warping function.
    d += mat_warp * (sin(pos.y * 0.25 / s + T) * sin(pos.x * 0.25 / s + T * 0.5)) * s * 5.0;
    
    // Stripe modulation.
    float v = mod(d + radius / (mat_loops * 2.0), radius / mat_loops);
    v = abs(v - radius / (mat_loops * 2.0));
    v = clamp(v - gap, 0.0, 1.0);
    
    // Normalize distance.
    d /= radius - T;
    
    // Generate color modulation.
    vec3 m = fract((d - 1.0) * vec3(mat_loops * mat_hue, -mat_loops, mat_loops * mat_tint) * 0.5);
    
    // Inversion logic restored from original ISF.
    vec3 color = mat_invert ? m / max(v, 1e-6) : m * v;
    color = clamp(color, 0.0, 1.0);
    return vec4(color, 1.0);
}

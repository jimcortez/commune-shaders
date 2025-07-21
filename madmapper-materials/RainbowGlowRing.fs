/*{
    "CREDIT": "Adapted by Shader Shaper; Original: mojovideotech; Commune Project (Jim Cortez); see table-shaders-isf/RainbowGlowRing.fs",
    "DESCRIPTION": "Rainbow glow ring effect with animated noise and color blending, adapted for MadMapper Materials with interactive controls and generator-driven animation.",
    "CATEGORIES": ["generator", "rainbow", "glow"],
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_center",     "TYPE": "point2D", "DEFAULT": [0.5, 0.5], "MAX": [1.0, 1.0], "MIN": [0.0, 0.0] },
        { "NAME": "mat_noise",      "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "mat_rate",       "TYPE": "float", "DEFAULT": 1.5, "MIN": -5.0, "MAX": 5.0 },
        { "NAME": "mat_radius1",    "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.00999, "MAX": 0.4999 },
        { "NAME": "mat_radius2",    "TYPE": "float", "DEFAULT": 0.333, "MIN": 0.01, "MAX": 0.5 },
        { "NAME": "mat_amp",        "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.005, "MAX": 0.5 },
        { "NAME": "mat_mult",       "TYPE": "float", "DEFAULT": 1.0, "MIN": 1.0, "MAX": 9.0 },
        { "NAME": "mat_warp",       "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.0, "MAX": 16.0 },
        { "NAME": "mat_glow",       "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 6.0 }
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
- Based on: shadertoy.com/view/ltBXRc by phil
- Source: Originally sourced from editor.isf.video - RainbowGlowRing by mojovideotech
- Description: Rainbow glow ring with animated noise and color blending
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0
- Features: Rainbow glow ring, animated noise, color blending
- Adapted for MadMapper Materials with enhanced parameterization and generator-driven animation
*/

// 2D rotation matrix function
mat2 r2d(float a) { 
    float c = cos(a), s = sin(a); 
    return mat2(c, -s, s, c); 
}

// Hash function for noise generation
float hash(float n) { 
    return fract((sin(n * n) * (mod(cos(n), 0.001))) * 10101.001); 
}

// Wave function for ring distortion
float wave(vec2 v1, vec2 v2, float time) { 
    return sin(dot(normalize(v1), normalize(v2) * mat_mult) * mat_warp + time) * mat_amp; 
}

// Circle function that creates the ring effect
vec3 circle(vec2 p, float rad, float wid, float time, float slowTime) {
    vec2 diff = mat_center - p; 
    float len = length(diff);
    len += wave(diff, vec2(1.0, 0.01 + hash(sin(mat_noise))) * r2d(time + slowTime), time) * cos(hash(slowTime) * mat_noise);
    len -= wave(diff, vec2(0.0, 1.1 - hash(cos(mat_noise))) * r2d(time - slowTime), time) * hash(sin(time * mat_noise));
    return vec3(smoothstep(rad - wid, rad, len) - smoothstep(rad, rad + wid, len));
}

vec4 materialColorForPixel(vec2 texCoord) {
    // Convert normalized texture coordinate to ISF-style coordinate system
    vec2 uv = (texCoord * 2.0 - 1.0);
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    // Animation time variables using generator-driven time
    float T = mat_genTime;
    float sT = T * 0.1;
    
    // Calculate ring parameters
    float width = abs(mat_radius2 - mat_radius1);
    float radius = (mat_radius2 + mat_radius1) * 0.5;
    
    // Generate the main ring effect
    vec3 col = circle(uv, radius, width * mat_glow, T, sT);
    
    // Apply color rotation and blending
    vec2 v = r2d(T) * uv;
    col *= vec3(v.x, v.y, 0.7 - v.y * v.x);
    
    // Add inner ring detail
    col += circle(uv, radius, width * 0.05 + 0.01, T, sT);
    
    // Ensure color is clamped and alpha is 1.0
    col = clamp(col, 0.0, 1.0);
    return vec4(col, 1.0);
} 
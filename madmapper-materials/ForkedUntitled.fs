/*{
    "CREDIT": "Adapted by Shader Shaper",
    "DESCRIPTION": "Animated circular pattern with adjustable colors and speed, converted to a MadMapper Material.",
    "CATEGORIES": ["pattern", "abstract"],
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_speed",           "TYPE": "float", "DEFAULT": 0.025,  "MIN": 0.01, "MAX": 0.025 },
        { "NAME": "mat_radiusScale",     "TYPE": "float", "DEFAULT": 0.175,  "MIN": 0.15, "MAX": 0.2 },
        { "NAME": "mat_spacing",         "TYPE": "float", "DEFAULT": 0.04,  "MIN": 0.01, "MAX": 0.1 },
        { "NAME": "mat_colorVariation",  "TYPE": "float", "DEFAULT": 0.6,   "MIN": 0.2,  "MAX": 1.0 },
        { "NAME": "mat_shapeWarp",       "TYPE": "float", "DEFAULT": 0.0,   "MIN": -1.0, "MAX": 1.0 },
        { "NAME": "mat_circleSharpness", "TYPE": "float", "DEFAULT": 0.01,  "MIN": 0.001,"MAX": 0.05 },
        { "NAME": "mat_glowStrength",    "TYPE": "float", "DEFAULT": 1.0,   "MIN": 0.1,  "MAX": 5.0 },
        { "NAME": "mat_paletteShift",    "TYPE": "float", "DEFAULT": 0.0,   "MIN": 0.0,  "MAX": 10.0 }
    ],
    "GENERATORS": [
        { "NAME": "mat_animation_time_speed", "TYPE": "time_base", "PARAMS": { "speed": "mat_speed" } }
    ]
}*/
/*
ORIGINAL SHADER INFORMATION:
- Original Author: Unknown (palette function by IQ)
- Source: Originally sourced from editor.isf.video - ForkedUntitled by Unknown
- Description: Animated circles with palette coloring
- License: Unknown
- Features: Animated circle positions, IQ palette coloring
*/

#define MAX_ITER 60  // Fixed iteration count for WebGL 1.0 compatibility

// Signed distance function for a warped circle
float sdWarpedCircle(vec2 p, float r, float warp, float i) {
    // Warp the radius with a sinusoidal function for visible shape variation
    float angle = atan(p.y, p.x);
    float warpedR = r + warp * sin(angle * (3.0 + 5.0 * warp) + i * 0.2);
    return length(p) - warpedR;
}

// Color palette function
vec3 palette(float t) {
    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 0.6);
    vec3 d = vec3(0.80, 0.90, 0.30);
    return a + b * cos(6.28318 * (c * t + d));
}

vec4 materialColorForPixel(vec2 texCoord) {
    // Convert normalized texture coordinate to a coordinate space matching ISF's gl_FragCoord logic.
    vec2 p = (texCoord * RENDERSIZE.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
    // Initialize color to black (no background fade)
    vec3 col = vec3(0.0);
    vec2 offset;
    float d;
    // Use the generator-driven animation time for animation
    float iTime = mat_animation_time_speed;
    for (int i = 0; i < MAX_ITER; i++) {
        offset = vec2(cos(float(i) * iTime) * (float(i) * mat_spacing),
                      sin(float(i) * iTime) * (float(i) * mat_spacing));
        d = sdWarpedCircle(p + offset, float(i) * mat_radiusScale, mat_shapeWarp, float(i));
        // Update color based on the signed distance, with palette shift
        col = (d > 0.0) ? palette(mat_colorVariation * mod(float(i), 10.0) + mat_paletteShift) : col;
        // Scale the blend by mat_glowStrength for visible glow/softness control
        col = mix(col, vec3(0.0), smoothstep(mat_circleSharpness * mat_glowStrength, 0.0, abs(d)));
    }
    col = clamp(col, 0.0, 1.0);
    return vec4(col, 1.0);
}

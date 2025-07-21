/*{
    "CREDIT": "Adapted by Shader Shaper; Original: Jim Cortez - Commune Project; see table-shaders-isf/Bows.fs",
    "DESCRIPTION": "Creates animated colorful bow patterns with flowing, ribbon-like strands that dance across the screen. Features multiple colored strands with different animation speeds, curves, and glowing effects that create a mesmerizing, organic movement pattern reminiscent of flowing ribbons or streamers.",
    "CATEGORIES": ["generator", "abstract", "bows", "strands"],
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_time_scale",     "TYPE": "float",   "DEFAULT": 2.0,          "MIN": 0.1,           "MAX": 10.0 },
        { "NAME": "mat_horizontal_scale", "TYPE": "float", "DEFAULT": 1.5,          "MIN": 0.5,           "MAX": 3.0 },
        { "NAME": "mat_vertical_scale",   "TYPE": "float", "DEFAULT": 0.2,          "MIN": 0.05,          "MAX": 0.5 },
        { "NAME": "mat_motion_offset",    "TYPE": "float", "DEFAULT": 30.0,         "MIN": 0.0,           "MAX": 100.0 },
        { "NAME": "mat_glow_intensity",   "TYPE": "float", "DEFAULT": 0.002,        "MIN": 0.001,         "MAX": 0.01 },
        { "NAME": "mat_line_count",       "TYPE": "float", "DEFAULT": 6.0,          "MIN": 1.0,           "MAX": 12.0 },
        { "NAME": "mat_line_thickness",   "TYPE": "float", "DEFAULT": 1.0,          "MIN": 0.5,           "MAX": 3.0 },
        { "NAME": "mat_color_variation",  "TYPE": "float", "DEFAULT": 1.0,          "MIN": 0.5,           "MAX": 2.0 },
        { "NAME": "mat_warp_strength",    "TYPE": "float", "DEFAULT": 0.5,          "MIN": 0.0,           "MAX": 2.0 },
        { "NAME": "mat_palette_shift",    "TYPE": "float", "DEFAULT": 0.0,          "MIN": 0.0,           "MAX": 6.28 }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_genTime",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_time_scale" }
        }
    ]
}*/
/*
ORIGINAL SHADER INFORMATION:
- Original Author: Unknown (no original credit provided)
- Source: Originally sourced from editor.isf.video - Bows by Unknown
- Description: Animated colorful bow patterns with flowing strands
- License: Unknown
- Features: Multiple colored strands with different animation speeds and curves
*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718
#define MAX_LINES 12  // Fixed iteration count for WebGL 1.0 compatibility

// Function to generate strand patterns with improved coordinate handling
vec3 Strand(vec2 texCoord, vec3 color, float hoffset, float hscale, float vscale, float timescale) {
    // Convert normalized coordinates to fragment coordinates
    vec2 fragCoord = texCoord * RENDERSIZE;
    
    float glow = mat_glow_intensity * RENDERSIZE.y;
    
    // Apply warp distortion for more organic movement
    float warp = sin(texCoord.x * 3.0 + mat_genTime * 0.5) * mat_warp_strength * 0.1;
    texCoord.x += warp;
    
    float curve = 1.0 - abs(fragCoord.y - (sin(mod(fragCoord.x * hscale / 100.0 / RENDERSIZE.x * 1000.0 
                 + mat_genTime * timescale + hoffset, TWO_PI)) * RENDERSIZE.y * 0.25 * vscale + RENDERSIZE.y / 2.0));
    
    float intensity = clamp(curve * mat_line_thickness, 0.0, 1.0);
    intensity += clamp((glow + curve) / glow, 0.0, 1.0) * 0.4;
    
    return intensity * color;
}

vec4 materialColorForPixel(vec2 texCoord) {
    vec3 colorOutput = vec3(0.0);
    float animatedOffset = 1.0 + sin(mat_genTime) * mat_motion_offset;
    
    // Convert line count to integer for loop control
    int lineCount = int(mat_line_count);
    
    // Fixed loop with early exit based on lineCount
    for (int i = 0; i < MAX_LINES; i++) {
        if (i >= lineCount) break;  // Stop drawing extra lines
        
        float offsetFactor = (float(i) / float(lineCount)) * mat_color_variation;
        
        // Enhanced color generation with palette shift
        float hueShift = mat_palette_shift + float(i) * 0.5;
        vec3 strandColor = vec3(
            sin(float(i) * 0.5 + hueShift) * 0.5 + 0.5, 
            cos(float(i) * 0.7 + hueShift * 0.7) * 0.5 + 0.5, 
            sin(float(i) * 0.3 + hueShift * 0.3) * 0.5 + 0.5
        );

        colorOutput += Strand(texCoord, strandColor, animatedOffset + float(i) * 0.1, 
                              mat_horizontal_scale * (1.0 + offsetFactor), 
                              mat_vertical_scale * (1.0 - offsetFactor), 
                              (8.0 + float(i)) * mat_time_scale);
    }

    // Clamp color to valid range
    colorOutput = clamp(colorOutput, 0.0, 1.0);
    
    return vec4(colorOutput, 1.0);
}

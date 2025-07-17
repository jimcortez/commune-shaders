/*{
    "CATEGORIES": ["Audio Reactive"],
    "CREDIT": "Jim Cortez - Commune Project (Original: https://www.shadertoy.com/user/yozic) | Adapted by Shader Shaper from Shadertoy: yozic",
    "DESCRIPTION": "Creates a mesmerizing acid jelly effect with multiple colorful orbs that flow and morph in a psychedelic pattern. The shader generates organic, fluid-like movements with orb formations that appear to float and interact in a dreamlike space. Converted to a MadMapper Material.",
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_zoom",          "TYPE": "float", "DEFAULT": 0.07,  "MIN": 0.01, "MAX": 1.0 },
        { "NAME": "mat_contrast",      "TYPE": "float", "DEFAULT": 0.13,  "MIN": 0.01, "MAX": 1.0 },
        { "NAME": "mat_radius",        "TYPE": "float", "DEFAULT": 11.0,  "MIN": 1.0,  "MAX": 100.0 },
        { "NAME": "mat_colorShift",    "TYPE": "float", "DEFAULT": 10.32, "MIN": 1.0,  "MAX": 100.0 },
        { "NAME": "mat_rotation",      "TYPE": "float", "DEFAULT": 1.0,   "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_yMul",          "TYPE": "float", "DEFAULT": 0.0,   "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_xMul",          "TYPE": "float", "DEFAULT": 0.28,  "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_xSpeed",        "TYPE": "float", "DEFAULT": 0.0,   "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_ySpeed",        "TYPE": "float", "DEFAULT": 0.0,   "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_yDivide",       "TYPE": "float", "DEFAULT": 4.99,  "MIN": 1.0,  "MAX": 20.0 },
        { "NAME": "mat_xDivide",       "TYPE": "float", "DEFAULT": 6.27,  "MIN": 1.0,  "MAX": 20.0 }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_time",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_rotation" }
        }
    ]
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: yozic (https://www.shadertoy.com/user/yozic)
- Original Shader: https://www.shadertoy.com/view/WtGfRw
- Source: Originally sourced from editor.isf.video - Acid Jelly by yozic
- Description: Audio reactive jelly-like effect with orb formations
- License: Shadertoy license (Creative Commons Attribution-NonCommercial-ShareAlike 3.0)
- Features: Multiple variants with different parameter configurations for various visual effects
*/

#define PI 3.141592
#define ORBS 20.0
#define ORB_SIZE 6.46

// Orb rendering function
vec4 orb(vec2 uv, float s, vec2 p, vec3 color, float c) {
    return pow(vec4(s / length(uv + p) * color, 1.0), vec4(c));
}

// 2D rotation function
mat2 rotate(float a) {
    return mat2(cos(a), -sin(a), sin(a), cos(a));
}

vec4 materialColorForPixel(vec2 texCoord) {
    // Match ISF coordinate normalization
    vec2 uv = (2.0 * texCoord * RENDERSIZE.xy - RENDERSIZE.xy) / RENDERSIZE.y;
    uv *= mat_zoom;
    uv /= dot(uv, uv);
    uv *= rotate(mat_rotation * TIME / 10.0);
    float sinMul = 0.0;
    float cosMul = 2.38;
    vec4 fragColor = vec4(0.0);
    for (float i = 0.0; i < ORBS; i++) {
        uv.x += sinMul * sin(uv.y * mat_yMul + TIME * mat_xSpeed) + cos(uv.y / mat_yDivide - TIME);
        uv.y += cosMul * cos(uv.x * mat_xMul - TIME * mat_ySpeed) - sin(uv.x / mat_xDivide - TIME);
        float t = i * PI / ORBS * 2.0;
        float x = mat_radius * tan(t);
        float y = mat_radius * cos(t + TIME / 10.0);
        vec2 position = vec2(x, y);
        vec3 color = cos(0.02 * uv.x + 0.02 * uv.y * vec3(-2.0, 0.0, -1.0) * PI * 2.0 / 3.0 + PI * (float(i) / mat_colorShift)) * 0.5 + 0.5;
        fragColor += 0.65 - orb(uv, ORB_SIZE, position, 1.0 - color, mat_contrast);
    }
    fragColor = clamp(fragColor, 0.0, 1.0);
    fragColor.a = 1.0;
    return fragColor;
}

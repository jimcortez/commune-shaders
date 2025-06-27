/*{
    "CATEGORIES": ["Audio Reactive"],
    "CREDIT": "Adapted by Shader Shaper from Shadertoy: yozic",
    "DESCRIPTION": "Audio-reactive orbiting shapes with customizable motion and colors, converted to a MadMapper Material.",
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_zoom",          "TYPE": "float", "DEFAULT": 0.07,  "MIN": 0.01, "MAX": 1.0 },
        { "NAME": "mat_contrast",      "TYPE": "float", "DEFAULT": 0.13,  "MIN": 0.01, "MAX": 1.0 },
        { "NAME": "mat_orbSize",       "TYPE": "float", "DEFAULT": 6.46,  "MIN": 1.0,  "MAX": 20.0 },
        { "NAME": "mat_radius",        "TYPE": "float", "DEFAULT": 11.0,  "MIN": 1.0,  "MAX": 100.0 },
        { "NAME": "mat_colorShift",    "TYPE": "float", "DEFAULT": 10.32, "MIN": 1.0,  "MAX": 100.0 },
        { "NAME": "mat_rotationSpeed", "TYPE": "float", "DEFAULT": 1.0,   "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_sinMultiplier", "TYPE": "float", "DEFAULT": 0.0,   "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_cosMultiplier", "TYPE": "float", "DEFAULT": 2.38,  "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_yMultiplier",   "TYPE": "float", "DEFAULT": 0.0,   "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_xMultiplier",   "TYPE": "float", "DEFAULT": 0.28,  "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_xSpeed",        "TYPE": "float", "DEFAULT": 0.0,   "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_ySpeed",        "TYPE": "float", "DEFAULT": 0.0,   "MIN": -10.0,"MAX": 10.0 },
        { "NAME": "mat_gloop",         "TYPE": "float", "DEFAULT": 0.003, "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "mat_yDivide",       "TYPE": "float", "DEFAULT": 4.99,  "MIN": 1.0,  "MAX": 20.0 },
        { "NAME": "mat_xDivide",       "TYPE": "float", "DEFAULT": 6.27,  "MIN": 1.0,  "MAX": 20.0 }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_time",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_rotationSpeed" }
        }
    ]
}*/

#define PI 3.141592
#define MAX_ORBS 20.0  // Fixed loop iteration for WebGL 1.0 compatibility

// Orb rendering function
vec4 orb(vec2 uv, float s, vec2 p, vec3 color, float c) {
    return pow(vec4(s / length(uv + p) * color, 1.0), vec4(c));
}

// 2D rotation function
mat2 rotate2D(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat2(c, -s, s, c);
}

vec4 materialColorForPixel(vec2 texCoord) {
    // Convert normalized texCoord to centered coordinate.
    vec2 uv = (2.0 * texCoord - 1.0) * (RENDERSIZE.y / RENDERSIZE.y);
    
    // Apply zoom.
    uv *= mat_zoom;
    
    // Invert distance to keep the orbits in view.
    uv /= dot(uv, uv);
    
    // Apply continuous rotation using generator-controlled time.
    uv *= rotate2D(mat_time / 10.0);

    vec4 fragColor = vec4(0.0);

    // Iterate over orbs.
    for (float i = 0.0; i < MAX_ORBS; i++) {
        // Modify uv with audio-reactive motion.
        uv.x += mat_sinMultiplier * sin(uv.y * mat_yMultiplier + TIME * mat_xSpeed) + cos(uv.y / mat_yDivide - TIME);
        uv.y += mat_cosMultiplier * cos(uv.x * mat_xMultiplier - TIME * mat_ySpeed) - sin(uv.x / mat_xDivide - TIME);

        float t = i * PI / MAX_ORBS * 2.0;
        float x = mat_radius * tan(t);
        float y = mat_radius * cos(t + TIME / 10.0);
        vec2 position = vec2(x, y);

        vec3 color = cos(0.02 * uv.x + 0.02 * uv.y * vec3(-2.0, 0.0, -1.0) * PI * 2.0 / 3.0 + PI * (i / mat_colorShift)) * 0.5 + 0.5;
        fragColor += 0.65 - orb(uv, mat_orbSize, position, 1.0 - color, mat_contrast);
    }

    fragColor.a = 1.0;
    return fragColor;
}

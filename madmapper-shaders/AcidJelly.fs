/*{
    "CATEGORIES": ["Audio Reactive"],
    "CREDIT": "Adapted by Shader Shaper from Shadertoy: yozic",
    "DESCRIPTION": "Audio-reactive orbiting shapes with customizable motion and colors.",
    "ISFVSN": "2",
    "INPUTS": [
        {
            "NAME": "zoom",
            "TYPE": "float",
            "DEFAULT": 0.07,
            "MIN": 0.01,
            "MAX": 1.0
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "DEFAULT": 0.13,
            "MIN": 0.01,
            "MAX": 1.0
        },
        {
            "NAME": "orbSize",
            "TYPE": "float",
            "DEFAULT": 6.46,
            "MIN": 1.0,
            "MAX": 20.0
        },
        {
            "NAME": "radius",
            "TYPE": "float",
            "DEFAULT": 11.0,
            "MIN": 1.0,
            "MAX": 100.0
        },
        {
            "NAME": "colorShift",
            "TYPE": "float",
            "DEFAULT": 10.32,
            "MIN": 1.0,
            "MAX": 100.0
        },
        {
            "NAME": "rotationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": -10.0,
            "MAX": 10.0
        },
        {
            "NAME": "sinMultiplier",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0
        },
        {
            "NAME": "cosMultiplier",
            "TYPE": "float",
            "DEFAULT": 2.38,
            "MIN": -10.0,
            "MAX": 10.0
        },
        {
            "NAME": "yMultiplier",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0
        },
        {
            "NAME": "xMultiplier",
            "TYPE": "float",
            "DEFAULT": 0.28,
            "MIN": -10.0,
            "MAX": 10.0
        },
        {
            "NAME": "xSpeed",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0
        },
        {
            "NAME": "ySpeed",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0
        },
        {
            "NAME": "gloop",
            "TYPE": "float",
            "DEFAULT": 0.003,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "yDivide",
            "TYPE": "float",
            "DEFAULT": 4.99,
            "MIN": 1.0,
            "MAX": 20.0
        },
        {
            "NAME": "xDivide",
            "TYPE": "float",
            "DEFAULT": 6.27,
            "MIN": 1.0,
            "MAX": 20.0
        }
    ]
}*/

#define PI 3.141592
#define MAX_ORBS 20.0  // Fixed loop iteration for WebGL 1.0 compatibility

// Orb rendering function
vec4 orb(vec2 uv, float s, vec2 p, vec3 color, float c) {
    return pow(vec4(s / length(uv + p) * color, 1.), vec4(c));
}

// 2D rotation function
mat2 rotate2D(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat2(c, -s, s, c);
}

void mainImage(out vec4 fragColor, in vec2 fragCoord) {
    vec2 uv = (2.0 * fragCoord - RENDERSIZE.xy) / RENDERSIZE.y;
    
    // Apply zoom and rotation, keeping continuous rotation accumulation
    uv *= zoom;
    uv /= dot(uv, uv);
    uv *= rotate2D(rotationSpeed * TIME / 10.0);

    fragColor = vec4(0.0);

    // Iterate over orbs (fixed count for WebGL 1.0 compatibility)
    for (float i = 0.0; i < MAX_ORBS; i++) {
        uv.x += sinMultiplier * sin(uv.y * yMultiplier + TIME * xSpeed) + cos(uv.y / yDivide - TIME);
        uv.y += cosMultiplier * cos(uv.x * xMultiplier - TIME * ySpeed) - sin(uv.x / xDivide - TIME);

        float t = i * PI / MAX_ORBS * 2.0;
        float x = radius * tan(t);
        float y = radius * cos(t + TIME / 10.0);
        vec2 position = vec2(x, y);

        vec3 color = cos(0.02 * uv.x + 0.02 * uv.y * vec3(-2, 0, -1) * PI * 2.0 / 3.0 + PI * (i / colorShift)) * 0.5 + 0.5;
        fragColor += 0.65 - orb(uv, orbSize, position, 1.0 - color, contrast);
    }

    fragColor.a = 1.0;
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}

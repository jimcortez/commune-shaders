/*{
    "CREDIT": "Adapted by Shader Shaper from codevinsky",
    "DESCRIPTION": "Fire effect with adjustable speed, size, brightness, and turbulence, converted to a MadMapper Material with generator-controlled fireSpeed.",
    "CATEGORIES": ["fire", "abstract"],
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_fireSpeed",      "TYPE": "float", "DEFAULT": 1.0, "MIN": -5.0, "MAX": 5.0 },
        { "NAME": "mat_fireSize",       "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0,   "MAX": 2.5 },
        { "NAME": "mat_fireBrightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0,  "MAX": 5.0 },
        { "NAME": "mat_turbulence",     "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0,   "MAX": 1.0 },
        { "NAME": "mat_colorIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5,   "MAX": 1.0 },
        { "NAME": "mat_fireSpread",     "TYPE": "float", "DEFAULT": 8.0, "MIN": 4.0,   "MAX": 16.0 },
        { "NAME": "mat_flameDetail",    "TYPE": "float", "DEFAULT": 1.0, "MIN": 1.0,   "MAX": 2.0 },
        { "NAME": "mat_flameBaseColor", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0,   "MAX": 1.0 }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_genFireSpeed",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_fireSpeed" }
        }
    ]
}*/

#define MAX_ITER 8  // Fixed iteration count for WebGL 1.0 compatibility

// Random noise function
float rand(vec2 n) {
    return fract(cos(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

// Smooth noise function
float noise(vec2 n) {
    const vec2 d = vec2(0.0, 1.0);
    vec2 b = floor(n), f = smoothstep(vec2(0.0), vec2(1.0), fract(n));
    return mix(mix(rand(b), rand(b + d.yx), f.x),
               mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
}

// Fractal Brownian Motion (FBM) function
float fbm(vec2 n) {
    float total = 0.0;
    float amplitude = 1.0;
    for (int i = 0; i < MAX_ITER; i++) {
        if (float(i) >= mat_flameDetail) break;
        total += noise(n) * amplitude;
        n += n * mat_turbulence;
        amplitude *= 0.5 * mat_fireSize;
    }
    return total;
}

vec4 materialColorForPixel(vec2 texCoord) {
    // Convert normalized texCoord to pixel coordinates and scale by fireSpread.
    vec2 p = (texCoord * RENDERSIZE.xy) * (mat_fireSpread / RENDERSIZE.x);
    
    // Fire color gradient constants.
    const vec3 c1 = vec3(0.5, 0.0, 0.1);
    const vec3 c2 = vec3(0.9, 0.0, 0.0);
    const vec3 c3 = vec3(0.2, 0.0, 0.0);
    const vec3 c4 = vec3(1.0, 0.9, 0.0);
    const vec3 c5 = vec3(0.1);
    const vec3 c6 = vec3(0.9);
    
    // Use generator-controlled fireSpeed instead of raw TIME.
    vec2 speed = vec2(0.7, 0.4);
    
    // Flame movement calculations.
    float q = fbm(p - mat_genFireSpeed * 0.1);
    vec2 r = vec2(
        fbm(p + q + mat_genFireSpeed * speed.x - p.x - p.y),
        fbm(p + q - mat_genFireSpeed * speed.y) * mat_fireBrightness
    );
    
    // Color blending.
    vec3 col = mix(c1, c2, fbm(p + r))
             + mix(c3, c4, r.x)
             - mix(c5, c6, r.y);
    
    // Apply vertical flame shift and color intensity.
    // Replace gl_FragCoord.y/RENDERSIZE.y with texCoord.y.
    col *= cos(texCoord.y) * mat_colorIntensity;
    
    return vec4(col, 1.0);
}

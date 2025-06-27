/*{
    "CREDIT": "Adapted by Shader Shaper",
    "DESCRIPTION": "Animated circular pattern with adjustable colors and speed, converted to a MadMapper Material.",
    "CATEGORIES": ["pattern", "abstract"],
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_speed",           "TYPE": "float", "DEFAULT": 0.05,  "MIN": 0.01, "MAX": 0.2 },
        { "NAME": "mat_radiusScale",     "TYPE": "float", "DEFAULT": 0.07,  "MIN": 0.05, "MAX": 0.2 },
        { "NAME": "mat_spacing",         "TYPE": "float", "DEFAULT": 0.04,  "MIN": 0.01, "MAX": 0.05 },
        { "NAME": "mat_colorVariation",  "TYPE": "float", "DEFAULT": 0.6,   "MIN": 0.2,  "MAX": 1.0 },
        { "NAME": "mat_shapeCount",      "TYPE": "float", "DEFAULT": 10.0,  "MIN": 10.0, "MAX": 50.0 },
        { "NAME": "mat_circleSharpness", "TYPE": "float", "DEFAULT": 0.01,  "MIN": 0.001,"MAX": 0.05 },
        { "NAME": "mat_backgroundFade",  "TYPE": "float", "DEFAULT": 0.0,   "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "mat_depth",           "TYPE": "float", "DEFAULT": 0.0,   "MIN": -0.25, "MAX": 0.0 }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_time",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_speed" }
        }
	]
}*/

#define MAX_ITER 60  // Fixed iteration count for WebGL 1.0 compatibility

// Signed distance function for a circle
float sdCircle(vec2 p, float r) {
    return length(p) - r;
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
    // Convert normalized texture coordinate to a coordinate space similar to gl_FragCoord mapping.
    // Compute p as in the original shader:
    vec2 p = (texCoord * 2.0 - 1.0);
    p = p * (RENDERSIZE.xy / vec2(min(RENDERSIZE.x, RENDERSIZE.y)));
    
    // Initialize color with the background fade value.
    vec3 col = vec3(mat_backgroundFade);
    vec2 offset;
    float d;
    
    float iTime = mat_time;
    
    for (int i = 0; i < MAX_ITER; i++) {
        if (float(i) >= mat_shapeCount) break;
        
        offset = vec2(cos(float(i) * iTime * mat_speed) * (float(i) * mat_spacing),
                      sin(float(i) * iTime * mat_speed) * (float(i) * mat_spacing));
                      
        d = sdCircle(p + offset, float(i) * mat_radiusScale);
        
        // Update color based on the signed distance.
        col = (d > 0.0) ? palette(mat_colorVariation * mod(float(i), 10.0)) : col;
        col = mix(col, vec3(0.0), smoothstep(mat_circleSharpness, mat_depth, abs(d)));
    }
    
    return vec4(col, 1.0);
}

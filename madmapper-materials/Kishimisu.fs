/*{
    "DESCRIPTION": "Animated hexagram pattern with swirling effects and color controls",
    "CREDIT": "Original by Unknown, Optimized version",
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "CATEGORIES": [
        "Pattern",
        "Geometric"
    ],
    "INPUTS": [
        {
            "NAME": "mat_hue",
            "TYPE": "float",
            "DEFAULT": 0.67,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Base Hue"
        },
        {
            "NAME": "mat_saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Color Saturation"
        },
        {
            "NAME": "mat_luminance",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Brightness"
        },
        {
            "NAME": "mat_speed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "mat_twist",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Twist Amount"
        },
        {
            "NAME": "mat_bloom",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MIN": 0.25,
            "MAX": 2.0,
            "LABEL": "Glow Intensity"
        },
        {
            "NAME": "mat_pointInputX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Center X Offset"
        },
        {
            "NAME": "mat_pointInputY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Center Y Offset"
        }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_time",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_speed" }
        }
    ]
}*/

// Constants
const float PI = 3.14159265359;
const float TWO_PI = 6.28318530718;
const vec4 HEXAGRAM_K = vec4(-0.5, 0.8660254038, 0.5773502692, 1.7320508076);
const float ITERATION_SCALE = 1.5;
const float PATTERN_FREQUENCY = 8.0;
const float GLOW_BASE = 0.005;

// Color palette generation with customizable base color
vec3 generatePalette(float t, vec3 baseColor) {
    // Pre-calculated constants for artistic color variation
    const vec3 a = vec3(0.5);
    const vec3 b = vec3(0.5);
    const vec3 c = vec3(1.0, 1.0, 0.6);
    const vec3 d = vec3(0.80, 0.90, 0.30);
    
    vec3 cyclicColor = a + b * cos(TWO_PI * (c * t + d));
    return mix(cyclicColor, baseColor, 0.5);
}

// Signed distance field for hexagram shape
float sdHexagram(vec2 p, float r) {
    p = abs(p);
    p -= 2.0 * min(dot(HEXAGRAM_K.xy, p), 0.0) * HEXAGRAM_K.xy;
    p -= 2.0 * min(dot(HEXAGRAM_K.yx, p), 0.0) * HEXAGRAM_K.yx;
    p -= vec2(clamp(p.x, r * HEXAGRAM_K.z, r * HEXAGRAM_K.w), r);
    return length(p) * sign(p.y);
}

// Convert HSL to RGB color space
vec3 hsl2rgb(vec3 hsl) {
    float c = (1.0 - abs(2.0 * hsl.z - 1.0)) * hsl.y;
    float h = hsl.x * 6.0;
    float x = c * (1.0 - abs(mod(h, 2.0) - 1.0));
    float m = hsl.z - 0.5 * c;
    
    vec3 rgb;
    if (h < 1.0) rgb = vec3(c, x, 0.0);
    else if (h < 2.0) rgb = vec3(x, c, 0.0);
    else if (h < 3.0) rgb = vec3(0.0, c, x);
    else if (h < 4.0) rgb = vec3(0.0, x, c);
    else if (h < 5.0) rgb = vec3(x, 0.0, c);
    else rgb = vec3(c, 0.0, x);
    
    return rgb + vec3(m);
}

vec4 materialColorForPixel(vec2 texCoord) {
    // Transform coordinate space
    vec2 uv = (texCoord * 2.0 - 1.0) * 0.5;
    uv += vec2(mat_pointInputX, mat_pointInputY);

    // Calculate polar coordinates for rotation
    float rho = length(uv);
    float theta = atan(uv.y, uv.x);
    
    // Apply time-based rotation and twist
    float angle = theta + mat_time - (mat_twist * rho * 2.0);
    vec2 baseUV = rho * vec2(cos(angle), sin(angle));
    
    // Store original UV for distance calculations
    vec2 uv0 = baseUV;
    
    // Convert input parameters to base color
    vec3 baseColor = hsl2rgb(vec3(mat_hue, mat_saturation, mat_luminance));
    
    // Accumulate final color
    vec3 finalColor = vec3(0.0);
    float fadeout = exp(-length(uv0));
    
    for (float i = 0.0; i < 3.0; i++) {
        // Calculate fractional UV coordinates
        vec2 iterationUV = fract(baseUV * pow(ITERATION_SCALE, i + 1.0)) - 0.5;
        
        // Calculate distance field
        float dist = sdHexagram(iterationUV, length(uv0)) * fadeout;
        
        // Generate color for this iteration
        float t = length(uv0) + i * 0.4 + mat_time * 0.4;
        vec3 iterationColor = generatePalette(t, baseColor);
        
        // Apply glow effect
        float glow = abs(sin(dist * PATTERN_FREQUENCY + mat_time)) / PATTERN_FREQUENCY;
        float intensity = pow(GLOW_BASE / abs(glow), mat_bloom);
        
        finalColor += iterationColor * intensity;
    }
    
    return vec4(finalColor, 1.0);
}

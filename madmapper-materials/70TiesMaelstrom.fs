/*{
    "CREDIT": "by mojovideotech, converted to MadMapper Material by ChatGPT",
    "CATEGORIES": ["noise", "Automatically Converted"],
    "DESCRIPTION": "Modified version with customizable parameters, generator-controlled rotation, and zoom control.",
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_speed",         "TYPE": "float", "MIN": -0.5,  "MAX": 0.5,  "DEFAULT": 0.5 },
        { "NAME": "mat_amplitude",     "TYPE": "float", "MIN": 0.1,  "MAX": 2.0,  "DEFAULT": 1.0 },
        { "NAME": "mat_colorShift",    "TYPE": "float", "MIN": 0.0,  "MAX": 6.28, "DEFAULT": 1.0 },
        { "NAME": "mat_rotationSpeed", "TYPE": "float", "MIN": -0.1,  "MAX": 0.1,  "DEFAULT": 0.1 },
        { "NAME": "mat_noiseScale",    "TYPE": "float", "MIN": 0.5,  "MAX": 3.0,  "DEFAULT": 1.2 },
        { "NAME": "mat_layerDensity",  "TYPE": "float", "MIN": 3.0,  "MAX": 10.0, "DEFAULT": 7.0 },
        { "NAME": "mat_detailLevel",   "TYPE": "float", "MIN": 0.1,  "MAX": 1.0,  "DEFAULT": 1.0 },
        { "NAME": "mat_zoom",          "TYPE": "float", "MIN": 0.5,  "MAX": 2.0,  "DEFAULT": 1.0 }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_time",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_speed" }
        },
        {
            "NAME": "mat_rotTime",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_rotationSpeed" }
        }
    ]
}*/

#define PI 3.1415927
#define MAX_ITER 10  // Fixed max iterations for WebGL compatibility

const mat3 m = mat3( 0.00,  0.80,  0.60,
                     -0.80,  0.36, -0.48,
                     -0.60, -0.48,  0.64 );

float hash( float n ) {
    return fract(sin(n) * 43758.5453);
}

float noise( in vec3 x ) {
    vec3 p = floor(x);
    vec3 f = fract(x);
    f = f * f * (3.0 - 2.0 * f);
    float n = p.x + p.y * 57.0 + 113.0 * p.z;
    float res = mix(mix(mix( hash(n +   0.0), hash(n +   1.0), f.x),
                        mix( hash(n +  57.0), hash(n +  58.0), f.x), f.y),
                    mix(mix( hash(n + 113.0), hash(n + 114.0), f.x),
                        mix( hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
    return res;
}

float fbm( vec3 p ) {
    float f;
    f  = 0.5000 * noise(p); p = m * p * 2.02;
    f += 0.2500 * noise(p); p = m * p * 2.03;
    f += 0.1250 * noise(p); p = m * p * 2.01;
    f += 0.0625 * noise(p);
    return f;
}

vec2 sfbm2( vec3 p ) {
    return 2.0 * vec2(fbm(p), fbm(p - 327.67)) - 1.0;
}

vec4 materialColorForPixel(vec2 texCoord) {
    float t = mat_time;
    // Convert normalized texCoord to a centered coordinate and apply zoom control.
    vec2 uv = 2.0 * (gl_FragCoord.xy / RENDERSIZE.y - vec2(0.9, 0.5)) * (1.0 / mat_zoom);
    
    // Use generator-controlled rotation time for smooth rotation.
    float a = mat_rotTime;
    float c = cos(a), s = sin(a);
    mat2 rotMat = mat2(c, -s, s, c);

    vec4 col = vec4(0.0);
    vec3 paint = vec3(0.3, 0.9, 0.7);
    
    for (int i = 0; i < MAX_ITER; i++) {
        float z = float(i) / mat_layerDensity;
        if (z >= 1.0) break;
        
        paint = 0.5 + 0.5 * cos(mat_colorShift + 4.0 * 2.0 * PI * z +
                vec3(0.0, 2.0 * PI / 3.0, -2.0 * PI / 3.0));
        uv *= rotMat;
        vec2 duv = vec2(mat_amplitude, 0.5) * sfbm2(vec3(mat_noiseScale * uv, 3.0 * z + t))
                   - 3.0 * z * vec2(0.5, 0.5);
        float d = length(uv + duv) - 1.2 * (1.0 - z);
        float aVal = smoothstep(0.1, 0.09 * mat_detailLevel, abs(d));
        d = 0.5 * aVal + 0.5 * smoothstep(0.09, 0.08 * mat_detailLevel, abs(d));
        col += (1.0 - col.a) * vec4(d * paint, aVal);
        
        if (col.a >= 0.9) break;
    }
    return col;
}

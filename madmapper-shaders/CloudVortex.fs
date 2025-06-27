/*{
    "CREDIT": "Adapted by Shader Shaper from Shadertoy: mojovideotech",
    "CATEGORIES": ["generator", "clouds", "vortex"],
    "DESCRIPTION": "Cloud Vortex with customizable turbulence, rotation, and color blending.",
    "ISFVSN": "2",
    "INPUTS": [
        {
            "NAME": "rate",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MIN": -5.0,
            "MAX": 5.0
        },
        {
            "NAME": "loops",
            "TYPE": "float",
            "DEFAULT": 90.0,
            "MIN": 50.0,
            "MAX": 200.0
        },
        {
            "NAME": "radius",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": -1.0,
            "MAX": 3.0
        },
        {
            "NAME": "rotationOffset",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": -1.0,
            "MAX": 1.0
        },
        {
            "NAME": "turbulenceAmount",
            "TYPE": "float",
            "DEFAULT": 2.0,
            "MIN": 1.0,
            "MAX": 3.0
        },
        {
            "NAME": "colorMode",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 2.0
        }
    ]
}*/

#define PI 3.14159265359
#define MAX_ITER 200  // Fixed max iteration for WebGL 1.0 compatibility

const mat3 rotationMatrix = mat3(0.33338, 0.56034, -0.71817,
                                 -0.87887, 0.32651, -0.15323,
                                  0.15162, 0.69596, 0.61339) * 1.93;

float timeFactor = TIME * rate;
float animationParam = 0.0;

// 2D Rotation matrix function
mat2 rotate2D(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat2(c, s, -s, c);
}

// Magnitude squared
float magnitudeSquared(vec2 p) { return dot(p, p); }

// Linear step
float linearStep(float min, float max, float x) {
    return clamp((x - min) / (max - min), 0.0, 1.0);
}

// Displacement function
vec2 displacement(float t) {
    return vec2(sin(t * 0.22), cos(t * 0.175)) * 2.0;
}

// 3D mapping function
vec2 map(vec3 p) {
    vec3 p2 = p;
    p2.xy -= displacement(p.z).xy;
    p.xy *= rotate2D(sin(p.z + TIME) * (0.1 + animationParam * 0.05) + TIME * 0.09);

    float cloudLayer = magnitudeSquared(p2.xy);
    float displacementAmount = 0.1 + animationParam * 0.2;
    float density = 0.0, z = 1.0, turbulence = turbulenceAmount;
    
    p *= 0.61;
    for (int i = 0; i < 5; i++) {
        p += sin(p.zxy * 0.75 * turbulence + TIME * turbulence * 0.8) * displacementAmount;
        density -= abs(dot(cos(p), sin(p.yzx)) * z);
        z *= 0.57;
        turbulence *= 1.4;
        p = p * rotationMatrix;
    }

    density = abs(density + animationParam * 3.0) + animationParam * 0.3 - 2.5 + (1.0 - radius);
    return vec2(density + cloudLayer * 0.2 + 0.25, cloudLayer);
}

// Rendering function
vec4 render(vec3 ro, vec3 rd, float timeOffset) {
    vec4 colorResult = vec4(0.0);
    const float lightDist = 8.0;
    vec3 lightPos = vec3(displacement(timeOffset + lightDist) * 0.5, timeOffset + lightDist);
    
    float t = 1.5, fogFactor = 0.0;
    float maxLoops = floor(loops);

    for (int i = 0; i < MAX_ITER; i++) {
        if (colorResult.a > 0.99 || float(i) > maxLoops) { break; }

        vec4 col = vec4(0.0);
        vec3 pos = ro + t * rd;
        vec2 mapped = map(pos);

        float density = clamp(mapped.x - 0.3, 0.0, 1.0) * 1.12;
        float dn = clamp(mapped.x + 2.0, 0.0, 3.0);

        if (mapped.x > 0.6) {
            col = vec4(sin(vec3(5.0, 0.4, 0.2) + mapped.y * 0.1 + sin(pos.z * 0.4) * 0.5 + 1.8) * 0.5 + 0.5, 0.08);
            col *= density * density * density;
            col.rgb *= linearStep(4.0, -2.5, mapped.x) * 2.3;

            float diff = clamp((density - map(pos + 0.8).x) * 0.11111, 0.001, 1.0);
            diff += clamp((density - map(pos + 0.35).x) * 0.4, 0.001, 1.0);
            col.xyz *= density * (vec3(0.005, 0.045, 0.075) + 1.5 * vec3(0.033, 0.07, 0.03) * diff);
        }

        float fogDensity = exp(t * 0.2 - 2.2);
        col.rgba += vec4(0.06, 0.11, 0.11, 0.1) * clamp(fogDensity - fogFactor, 0.0, 1.0);
        fogFactor = fogDensity;
        colorResult = colorResult + col * (1.0 - colorResult.a);

        t += clamp(0.5 - dn * dn * 0.05, 0.09, 0.3);
    }

    return clamp(colorResult, 0.0, 1.0);
}

// Color interpolation function
vec3 interpolateColor(vec3 a, vec3 b, float factor) {
    return mix(a, b, factor);
}

void main() {
    vec2 normalizedCoords = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 position = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    vec3 cameraOrigin = vec3(0.0, 0.0, timeFactor);
    cameraOrigin += vec3(sin(TIME) * 0.5, 0.0, 0.0);

    float displacementStrength = 0.85, targetDist = 3.5;
    cameraOrigin.xy += displacement(cameraOrigin.z) * displacementStrength;
    vec3 target = normalize(cameraOrigin - vec3(displacement(timeFactor + targetDist) * displacementStrength, timeFactor + targetDist));
    
    cameraOrigin.x -= rotationOffset * 2.0;
    vec3 right = normalize(cross(target, vec3(0.0, 1.0, 0.0)));
    vec3 up = normalize(cross(right, target));
    vec3 rayDirection = normalize((position.x * right + position.y * up) - target);

    rayDirection.xy *= rotate2D(-displacement(timeFactor + 3.5).x * 0.2 + rotationOffset);

    animationParam = smoothstep(-0.4, 0.4, sin(TIME * 0.3));
    vec4 scene = render(cameraOrigin, rayDirection, timeFactor);
    vec3 finalColor = scene.rgb;

    //if (colorMode >= 0.5) {
        finalColor = interpolateColor(finalColor.rgb, finalColor.rgb * vec3(colorMode), clamp(1.0 - animationParam, 0.05, 0.9));
    //}

    gl_FragColor = vec4(finalColor, 1.0);
}

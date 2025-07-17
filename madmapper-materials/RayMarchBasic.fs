/*{
    "DESCRIPTION": "Raymarch Basic converted to a MadMapper Material with descriptive variable names, HSL color controls, and adjustable raymarch loop count.",
    "CREDIT": "gyabo, adapted by Shader Shaper",
    "ISFVSN": "2",
    "CATEGORIES": ["XXX"],
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_speed",         "TYPE": "float", "DEFAULT": 0.1,  "MIN": -1.0,  "MAX": 1.0 },
        { "NAME": "mat_hue",           "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,   "MAX": 1.0 },
        { "NAME": "mat_saturation",    "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,   "MAX": 1.0 },
        { "NAME": "mat_luminance",     "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.5,   "MAX": 1.5 },
        { "NAME": "mat_loopCount",     "TYPE": "float", "DEFAULT": 100.0,"MIN": 10.0,  "MAX": 100.0 },
        { "NAME": "mat_rayStepScale",  "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.1,   "MAX": 1.0 },
        { "NAME": "mat_colorIntensity","TYPE": "float", "DEFAULT": 1.0,  "MIN": 1.0,   "MAX": 2.0 },
        { "NAME": "mat_exposure",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 1.0,   "MAX": 2.0 }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_genSpeed",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_speed" }
        }
    ]
}*/

/*
   This shader implements a basic raymarching scene.
   The final color is influenced by a distance-based computation,
   then modulated by HSL color controls and various scaling parameters.
*/

// Helper: HSL to RGB conversion.
vec3 hsl2rgb(vec3 hsl) {
    float c = (1.0 - abs(2.0 * hsl.z - 1.0)) * hsl.y;
    float h = hsl.x * 6.0;
    float x = c * (1.0 - abs(mod(h, 2.0) - 1.0));
    vec3 rgb;
    if (h < 1.0)
        rgb = vec3(c, x, 0.0);
    else if (h < 2.0)
        rgb = vec3(x, c, 0.0);
    else if (h < 3.0)
        rgb = vec3(0.0, c, x);
    else if (h < 4.0)
        rgb = vec3(0.0, x, c);
    else if (h < 5.0)
        rgb = vec3(x, 0.0, c);
    else
        rgb = vec3(c, 0.0, x);
    float m = hsl.z - 0.5 * c;
    return rgb + vec3(m);
}

// Scene distance function (SDF) for raymarching.
float map(vec3 p) {
    vec3 upVector = vec3(0.0, 1.0, 0.0);
    float frequencyFactor = 1.9;
    float sinModulationXZ = (sin(p.x * frequencyFactor) + sin(p.z * frequencyFactor)) * 0.8;
    float sinModulationYZ = (sin(p.y * frequencyFactor) + sin(p.z * frequencyFactor)) * 0.8;
    float distanceWall1 = 4.0 - dot(abs(p), normalize(upVector)) + sinModulationXZ;
    float distanceWall2 = 4.0 - dot(abs(p), normalize(upVector.yzx)) + sinModulationYZ;
    float distanceShape1 = length(mod(p.xy + vec2(sin((p.z + p.x) * 2.0) * 0.3,
                                                  cos((p.z + p.x) * 2.0) * 0.5), 2.0) - 1.0) - 0.2;
    float distanceShape2 = length(mod(p.yz + vec2(sin((p.z + p.x) * 2.0) * 0.3,
                                                  cos((p.z + p.x) * 2.0) * 0.5), 2.0) - 1.0) - 0.2;
    return min(distanceWall1, min(distanceWall2, min(distanceShape1, distanceShape2)));
}

// 2D rotation helper.
vec2 rotate2D(vec2 point, float angle) {
    return vec2(
        point.x * cos(angle) - point.y * sin(angle),
        point.x * sin(angle) + point.y * cos(angle)
    );
}

vec4 materialColorForPixel(vec2 texCoord) {
    // Use generator-controlled time for animation.
    float animationTime = mat_genSpeed;
    
    // Convert normalized texCoord to pixel coordinates, then to NDC space.
    vec2 pixelCoords = texCoord * RENDERSIZE.xy;
    vec2 screenUV = (2.0 * pixelCoords - RENDERSIZE.xy) / RENDERSIZE.y;
    screenUV *= 0.4;
    
    // Compute the ray direction from the screen coordinates.
    vec3 rayDirection = normalize(vec3(screenUV, 1.0));
    rayDirection.xz = rotate2D(rayDirection.xz, animationTime * 0.23);
    rayDirection = rayDirection.yzx;
    rayDirection.xz = rotate2D(rayDirection.xz, animationTime * 0.2);
    rayDirection = rayDirection.yzx;
    
    // Set the camera position along the z-axis based on animation time.
    vec3 cameraPosition = vec3(0.0, 0.0, animationTime);
    
    // Initialize raymarching parameters.
    float totalDistance = 0.0;
    float stepDistance = 0.0;
    
    // Raymarching loop: use mat_loopCount to control the number of iterations.
    for (int i = 0; i < 100; i++) {
        if (float(i) >= mat_loopCount) break;
        stepDistance = map(cameraPosition + rayDirection * totalDistance);
        if (stepDistance < 0.001) break;
        totalDistance += stepDistance * mat_rayStepScale;
    }
    
    // Compute the intersection point.
    vec3 intersectionPoint = cameraPosition + rayDirection * totalDistance;
    
    // Compute an initial color based on the traveled distance.
    vec3 baseColor = vec3(totalDistance * 0.1);
    baseColor = sqrt(baseColor);
    
    // Combine contributions: distance, ray direction, and SDF difference.
    vec3 finalColor = 0.05 * totalDistance + abs(rayDirection) * baseColor +
                      max(0.0, map(intersectionPoint - 0.1) - stepDistance);
    
    // Apply HSL color control: compute a color multiplier from HSL inputs.
    vec3 hslColor = hsl2rgb(vec3(mat_hue, mat_saturation, mat_luminance));
    finalColor *= hslColor;
    
    // Apply additional color intensity scaling.
    finalColor *= mat_colorIntensity;
    
    // Apply exposure adjustment via gamma correction.
    finalColor = pow(finalColor, vec3(1.0 / mat_exposure));
    
    return vec4(finalColor, 1.0);
}

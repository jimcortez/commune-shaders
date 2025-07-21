/*{
    "DESCRIPTION": "Projects an input image onto a sphere, then unwraps the sphere into horizontal rings. Each ring stretches across the output width, sampling the sphere at a different latitude. Designed for remapping to a physical sphere.",
    "CATEGORIES": ["Generator", "Image", "3D", "Sphere", "RingProjection"],
    "INPUTS": [
        { "NAME": "inputImage", "TYPE": "image" },
        { "NAME": "rotationSpeed", "TYPE": "float", "DEFAULT": 0.2, "MIN": -2.0, "MAX": 2.0 },
        { "NAME": "rotationY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
        { "NAME": "rotationX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 }
    ],
    "ISFVSN": "2"
}*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718

mat3 rotationYMatrix(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat3(
        c, 0.0, s,
        0.0, 1.0, 0.0,
        -s, 0.0, c
    );
}
mat3 rotationXMatrix(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat3(
        1.0, 0.0, 0.0,
        0.0, c, -s,
        0.0, s, c
    );
}

void main() {
    // Normalized coordinates (0-1)
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    // For each output row (y), treat as a latitude band on the sphere
    float v = uv.y; // latitude from 0 (south pole) to 1 (north pole)
    float latitude = (v - 0.5) * PI; // -PI/2 to PI/2
    // For each output column (x), treat as longitude
    float u = uv.x;
    float longitude = (u - 0.5) * TWO_PI; // -PI to PI
    // Convert spherical to cartesian (on unit sphere)
    float x = cos(latitude) * cos(longitude);
    float y = cos(latitude) * sin(longitude);
    float z = sin(latitude);
    vec3 spherePos = vec3(x, y, z);
    // Apply Y and X axis rotations
    float yAngle = TIME * rotationY;
    float xAngle = TIME * rotationX;
    spherePos = rotationYMatrix(yAngle) * rotationXMatrix(xAngle) * spherePos;
    // Convert back to spherical coordinates
    float newLongitude = atan(spherePos.y, spherePos.x);
    float newLatitude = asin(clamp(spherePos.z, -1.0, 1.0));
    newLongitude += TIME * rotationSpeed;
    // Map longitude/latitude to [0,1] for equirectangular image
    float imgU = mod((newLongitude / TWO_PI) + 0.5, 1.0);
    float imgV = clamp((newLatitude / PI) + 0.5, 0.0, 1.0);
    vec2 imgUV = vec2(imgU, imgV);
    // Sample the image
    vec4 color = IMG_NORM_PIXEL(inputImage, imgUV);
    // Clamp output color to [0,1]
    gl_FragColor = clamp(color, 0.0, 1.0);
} 
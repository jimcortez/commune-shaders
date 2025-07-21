/*{
    "DESCRIPTION": "Maps an input image onto a rotating sphere with equirectangular projection. Allows 3D rotation controls. Image wraps seamlessly.",
    "CATEGORIES": ["Generator", "Image", "3D", "Sphere"],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "rotationSpeed",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": -2.0,
            "MAX": 2.0
        },
        {
            "NAME": "rotationY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -2.0,
            "MAX": 2.0
        },
        {
            "NAME": "rotationX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -2.0,
            "MAX": 2.0
        }
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
    // Center and scale to fit sphere in output
    vec2 centered = uv - 0.5;
    centered.x *= aspect;
    float r = length(centered);
    float sphereRadius = 0.5;
    if (r > sphereRadius) {
        // Outside sphere: background (black)
        gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0);
        return;
    }
    // Project to sphere surface (z >= 0 hemisphere)
    float z = sqrt(sphereRadius * sphereRadius - r * r);
    vec3 spherePos = vec3(centered, z) / sphereRadius; // normalized sphere
    // Apply Y and X axis rotations (vertical and horizontal axes)
    float yAngle = TIME * rotationY;
    float xAngle = TIME * rotationX;
    spherePos = rotationYMatrix(yAngle) * rotationXMatrix(xAngle) * spherePos;
    // Convert to spherical coordinates
    float longitude = atan(spherePos.y, spherePos.x); // -PI to PI
    float latitude = asin(clamp(spherePos.z, -1.0, 1.0)); // -PI/2 to PI/2
    longitude += TIME * rotationSpeed;
    // Map longitude/latitude to [0,1] for equirectangular image
    float u = mod((longitude / TWO_PI) + 0.5, 1.0);
    float v = clamp((latitude / PI) + 0.5, 0.0, 1.0);
    vec2 imgUV = vec2(u, v);
    // Sample the image
    vec4 color = IMG_NORM_PIXEL(inputImage, imgUV);
    // Clamp output color to [0,1]
    gl_FragColor = clamp(color, 0.0, 1.0);
} 
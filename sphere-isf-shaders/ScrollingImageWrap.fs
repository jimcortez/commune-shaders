/*{
    "DESCRIPTION": "Image fills output, scrolls horizontally and vertically with independent speed inputs, wraps seamlessly.",
    "CATEGORIES": ["Generator", "Image"],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "scrollSpeedX",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": -2.0,
            "MAX": 2.0
        },
        {
            "NAME": "scrollSpeedY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -2.0,
            "MAX": 2.0
        }
    ],
    "ISFVSN": "2"
}*/

void main() {
    // Normalized coordinates (0-1)
    vec2 uv = isf_FragNormCoord.xy;
    
    // Get input image size and output render size
    vec2 imgSize = IMG_SIZE(inputImage);
    vec2 outSize = RENDERSIZE;
    float imgAspect = imgSize.x / imgSize.y;
    float outAspect = outSize.x / outSize.y;

    // Scale and center image to fill output while maintaining aspect ratio
    vec2 scale = vec2(1.0);
    vec2 offset = vec2(0.0);
    if (imgAspect > outAspect) {
        // Image is wider than output: fit height, crop width
        scale.x = outAspect / imgAspect;
        offset.x = (1.0 - scale.x) * 0.5;
    } else {
        // Image is taller than output: fit width, crop height
        scale.y = imgAspect / outAspect;
        offset.y = (1.0 - scale.y) * 0.5;
    }
    // Map output uv to image uv
    vec2 imgUV = (uv - offset) / scale;
    // Wrap coordinates for seamless scrolling
    imgUV = fract(imgUV);

    // Scroll horizontally and vertically over time, wrapping
    float scrollX = TIME * scrollSpeedX;
    float scrollY = TIME * scrollSpeedY;
    imgUV.x = fract(imgUV.x + scrollX);
    imgUV.y = fract(imgUV.y + scrollY);

    // Sample the image
    vec4 color = IMG_NORM_PIXEL(inputImage, imgUV);
    // Clamp output color to [0,1]
    gl_FragColor = clamp(color, 0.0, 1.0);
} 
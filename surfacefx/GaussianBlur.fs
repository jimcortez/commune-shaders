/*{
    "CREDIT": "Created by Jim Cortez for Commune Project",
    "DESCRIPTION": "2D Gaussian blur SurfaceFx with adjustable blur amount. Highly optimized with separable passes and adaptive sampling.",
    "CATEGORIES": ["blur", "filter", "effect"],
    "VSN": 1,
    "INPUTS": [
        { "NAME": "fx_blurAmount", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 10.0 },
        { "NAME": "fx_blurQuality", "TYPE": "float", "DEFAULT": 16.0, "MIN": 4.0, "MAX": 32.0 },
        { "NAME": "fx_useSeparable", "TYPE": "bool", "DEFAULT": true }
    ]
}*/

// Highly optimized Gaussian blur SurfaceFx for MadMapper
// Uses separable passes, adaptive sampling, and pre-computed weights

// Pre-computed Gaussian weights for common blur amounts
const float gaussianWeights[9] = float[9](0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216, 0.0034908, 0.0005308, 0.0000612, 0.0000054);

// Forward declarations
vec4 separableGaussianBlur(vec2 coord, float radius);
vec4 fastSmallBlur(vec2 coord, float radius);

// Fast separable Gaussian blur (horizontal + vertical passes)
vec4 separableGaussianBlur(vec2 coord, float radius) {
    vec4 color = vec4(0.0);
    float totalWeight = 0.0;
    
    // Horizontal pass
    for (int i = -4; i <= 4; i++) {
        float weight = gaussianWeights[abs(i)];
        vec2 offset = vec2(float(i) * radius, 0.0);
        color += FX_NORM_PIXEL(coord + offset) * weight;
        totalWeight += weight;
    }
    
    // Vertical pass
    vec4 finalColor = vec4(0.0);
    float finalWeight = 0.0;
    
    for (int j = -4; j <= 4; j++) {
        float weight = gaussianWeights[abs(j)];
        vec2 offset = vec2(0.0, float(j) * radius);
        finalColor += FX_NORM_PIXEL(coord + offset) * weight;
        finalWeight += weight;
    }
    
    return clamp((color + finalColor) / (totalWeight + finalWeight), 0.0, 1.0);
}

// Fast blur for very small amounts
vec4 fastSmallBlur(vec2 coord, float radius) {
    vec4 center = FX_NORM_PIXEL(coord);
    vec4 neighbors = vec4(0.0);
    
    // Sample 4 neighbors with simple weights
    neighbors += FX_NORM_PIXEL(coord + vec2(radius, 0.0)) * 0.25;
    neighbors += FX_NORM_PIXEL(coord + vec2(-radius, 0.0)) * 0.25;
    neighbors += FX_NORM_PIXEL(coord + vec2(0.0, radius)) * 0.25;
    neighbors += FX_NORM_PIXEL(coord + vec2(0.0, -radius)) * 0.25;
    
    return clamp(mix(center, neighbors, 0.5), 0.0, 1.0);
}

vec4 fxColorForPixel(vec2 mm_FragNormCoord) {
    // Get the blur amount and quality
    float blurAmount = fx_blurAmount;
    float blurQuality = fx_blurQuality;
    bool useSeparable = fx_useSeparable;
    
    // If no blur, return the original pixel
    if (blurAmount <= 0.0) {
        return FX_NORM_PIXEL(mm_FragNormCoord);
    }
    
    // Get the input image size for proper scaling
    vec2 imgSize = FX_IMG_SIZE();
    
    // Calculate blur radius in normalized coordinates
    float blurRadius = blurAmount / imgSize.x;
    
    // Use separable Gaussian blur for better performance (2D -> 1D + 1D)
    if (useSeparable && blurRadius < 0.01) {
        return separableGaussianBlur(mm_FragNormCoord, blurRadius);
    }
    
    // Adaptive sample count based on blur amount and quality
    int maxSamples = int(min(blurQuality, 12.0)); // Reduced cap for better performance
    int samples = max(3, int(blurRadius * 30.0)); // More conservative scaling
    samples = min(samples, maxSamples);
    
    // Early exit for very small blurs
    if (samples <= 3) {
        return fastSmallBlur(mm_FragNormCoord, blurRadius);
    }
    
    // Initialize accumulated color and weight
    vec4 accumulatedColor = vec4(0.0);
    float totalWeight = 0.0;
    
    // Pre-calculate optimized Gaussian parameters
    float sigma = blurRadius * 0.4; // Adjusted for better visual results
    float twoSigmaSquared = 2.0 * sigma * sigma;
    float weightThreshold = 0.001; // Skip very small weights
    
    // Apply optimized Gaussian blur with early termination
    for (int i = -samples; i <= samples; i++) {
        for (int j = -samples; j <= samples; j++) {
            // Calculate offset in normalized coordinates
            vec2 offset = vec2(float(i), float(j)) * blurRadius / float(samples);
            
            // Calculate Gaussian weight (optimized)
            float distanceSquared = dot(offset, offset);
            float weight = exp(-distanceSquared / twoSigmaSquared);
            
            // Skip very small weights for performance
            if (weight < weightThreshold) continue;
            
            // Sample the texture with offset
            vec2 sampleCoord = mm_FragNormCoord + offset;
            vec4 sampleColor = FX_NORM_PIXEL(sampleCoord);
            
            // Accumulate weighted color
            accumulatedColor += sampleColor * weight;
            totalWeight += weight;
        }
    }
    
    // Normalize by total weight and clamp to valid range
    vec4 result = accumulatedColor / totalWeight;
    return clamp(result, 0.0, 1.0);
} 
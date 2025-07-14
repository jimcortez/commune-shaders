/*{
	"CATEGORIES": ["Space", "Planets", "3D", "Ring-Based"],
	"CREDIT": "Jim Cortez - Commune Project (Ring-Based Planet Visualization)",
	"DESCRIPTION": "Creates a simple rotating planet using ring-based spherical coordinate system. Visualizes a planet with ring-based mapping that aligns with LED distribution patterns.",
	"INPUTS": [
		{
			"NAME": "rotationSpeed",
			"TYPE": "float",
			"LABEL": "Rotation Speed",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "planetRadius",
			"TYPE": "float",
			"LABEL": "Planet Radius",
			"DEFAULT": 0.8,
			"MIN": 0.1,
			"MAX": 1.5
		},
		{
			"NAME": "numRings",
			"TYPE": "float",
			"LABEL": "Number of Rings",
			"DEFAULT": 49.0,
			"MIN": 10.0,
			"MAX": 100.0
		},
		{
			"NAME": "ringVisibility",
			"TYPE": "float",
			"LABEL": "Ring Visibility",
			"DEFAULT": 0.3,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "surfaceDetail",
			"TYPE": "float",
			"LABEL": "Surface Detail",
			"DEFAULT": 8.0,
			"MIN": 1.0,
			"MAX": 20.0
		},
		{
			"NAME": "atmosphereGlow",
			"TYPE": "float",
			"LABEL": "Atmosphere Glow",
			"DEFAULT": 0.2,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "baseColor",
			"TYPE": "color",
			"LABEL": "Base Color",
			"DEFAULT": [0.2, 0.4, 0.8, 1.0]
		},
		{
			"NAME": "highlightColor",
			"TYPE": "color",
			"LABEL": "Highlight Color",
			"DEFAULT": [0.8, 0.9, 1.0, 1.0]
		}
	],
	"ISFVSN": "2"
}*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718

// Ring-based spherical coordinate system
// Based on the methodology: ring-centric distribution with variable LED density

// Convert world position to ring coordinates
float getRingFromPosition(vec3 worldPos) {
    float latitude = asin(clamp(worldPos.z / planetRadius, -1.0, 1.0));
    return (latitude / PI + 0.5) * (numRings - 1.0) + 1.0;
}

// Get ring-based UV coordinates
vec2 getRingUV(float ringNum, float positionInRing, float ledsInRing) {
    float u = positionInRing / ledsInRing;  // Longitude (0 to 1)
    float v = (ringNum - 1.0) / (numRings - 1.0);  // Latitude (0 to 1)
    return vec2(u, v);
}

// Simulate LED density variation by latitude (rings near equator have more LEDs)
float getLEDDensity(float ringNum) {
    float latitude = (ringNum - 1.0) / (numRings - 1.0) - 0.5; // -0.5 to 0.5
    float equatorFactor = 1.0 - abs(latitude) * 0.7; // More LEDs near equator
    return max(4.0, 20.0 * equatorFactor); // Minimum 4 LEDs, max around 20
}

// Convert spherical coordinates to Cartesian
vec3 sphericalToCartesian(float radius, float latitude, float longitude) {
    return vec3(
        radius * cos(latitude) * cos(longitude),
        radius * cos(latitude) * sin(longitude),
        radius * sin(latitude)
    );
}

// Convert Cartesian to spherical coordinates
vec3 cartesianToSpherical(vec3 pos) {
    float radius = length(pos);
    float latitude = asin(clamp(pos.z / radius, -1.0, 1.0));
    float longitude = atan(pos.y, pos.x);
    return vec3(radius, latitude, longitude);
}

// Simple noise function for surface detail
float noise(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

// Fractal Brownian Motion for surface texture
float fbm(vec2 p) {
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;
    
    for(int i = 0; i < 4; i++) {
        value += amplitude * noise(p * frequency);
        amplitude *= 0.5;
        frequency *= 2.0;
    }
    return value;
}

// Generate ring-aligned pattern
vec4 generateRingPattern(vec2 uv, float ringNum) {
    float positionInRing = uv.x * getLEDDensity(ringNum);
    
    // Create pattern based on ring structure
    float pattern = sin(positionInRing * TWO_PI) * 0.5 + 0.5;
    
    // Add some variation based on ring number
    float ringVariation = sin(ringNum * 0.3) * 0.3 + 0.7;
    
    return vec4(pattern * ringVariation);
}

// Visualize individual rings
float visualizeRing(vec3 worldPos, float targetRing) {
    float currentRing = getRingFromPosition(worldPos);
    return smoothstep(0.1, 0.0, abs(currentRing - targetRing));
}

// Main shader function
void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 centeredUV = (uv - 0.5) * 2.0;
    
    // Apply rotation based on time
    float rotationAngle = TIME * rotationSpeed;
    mat2 rotationMatrix = mat2(
        cos(rotationAngle), -sin(rotationAngle),
        sin(rotationAngle), cos(rotationAngle)
    );
    centeredUV = rotationMatrix * centeredUV;
    
    // Calculate distance from center
    float dist = length(centeredUV);
    
    // Create planet sphere
    if (dist <= planetRadius) {
        // Convert 2D position to 3D sphere surface
        float x = centeredUV.x / planetRadius;
        float y = centeredUV.y / planetRadius;
        float z = sqrt(1.0 - x*x - y*y);
        
        vec3 worldPos = vec3(x, y, z) * planetRadius;
        
        // Get ring information
        float ringNum = getRingFromPosition(worldPos);
        vec3 spherical = cartesianToSpherical(worldPos);
        
        // Generate ring-based UV coordinates
        float ledsInRing = getLEDDensity(ringNum);
        float positionInRing = (spherical.z / TWO_PI + 0.5) * ledsInRing;
        vec2 ringUV = getRingUV(ringNum, positionInRing, ledsInRing);
        
        // Generate surface texture
        float surfaceNoise = fbm(ringUV * surfaceDetail);
        
        // Create ring visualization
        float ringPattern = 0.0;
        for(int i = 1; i <= 10; i += 2) {
            float ringNum = float(i);
            if (ringNum <= numRings) {
                ringPattern += visualizeRing(worldPos, ringNum) * ringVisibility;
            }
        }
        
        // Generate ring-aligned pattern
        vec4 ringAlignedPattern = generateRingPattern(ringUV, ringNum);
        
        // Combine surface and ring patterns
        vec4 surfaceColor = mix(baseColor, highlightColor, surfaceNoise);
        vec4 finalColor = mix(surfaceColor, ringAlignedPattern, ringPattern);
        
        // Add atmosphere glow at edges
        float edgeGlow = smoothstep(planetRadius, planetRadius * 0.8, dist);
        finalColor = mix(finalColor, vec4(highlightColor.rgb, 1.0), edgeGlow * atmosphereGlow);
        
        gl_FragColor = finalColor;
    } else {
        // Space background
        float spaceNoise = noise(uv * 50.0 + TIME * 0.1);
        vec4 spaceColor = vec4(0.02, 0.05, 0.1, 1.0) + vec4(spaceNoise * 0.1);
        gl_FragColor = spaceColor;
    }
} 
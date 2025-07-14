/*{
	"CATEGORIES": ["Space", "Planets"],
	"CREDIT": "Jim Cortez - Commune Project (Enhanced with Ring-Based Spherical Distribution)",
	"DESCRIPTION": "Creates a procedural alien planet using ring-based spherical coordinate system optimized for LED sphere arrays. Features realistic landmasses, alien oceans, atmospheric effects, and otherworldly features. Uses ring-centric mapping that matches physical LED distribution patterns.",
	"INPUTS": [
		{
			"NAME": "rotationSpeed",
			"TYPE": "float",
			"LABEL": "Rotation Speed",
			"DEFAULT": 0.15,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "tiltAngle",
			"TYPE": "float",
			"LABEL": "Tilt Angle (Degrees)",
			"DEFAULT": 15.0,
			"MIN": 0.0,
			"MAX": 90.0
		},
		{
			"NAME": "zRotation",
			"TYPE": "float",
			"LABEL": "Z Rotation (Degrees)",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 360.0
		},
		{
			"NAME": "landmassScale",
			"TYPE": "float",
			"LABEL": "Landmass Scale",
			"DEFAULT": 12.0,
			"MIN": 1.0,
			"MAX": 30.0
		},
		{
			"NAME": "oceanDepth",
			"TYPE": "float",
			"LABEL": "Ocean Depth",
			"DEFAULT": 0.7,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "continentIntensity",
			"TYPE": "float",
			"LABEL": "Continent Intensity",
			"DEFAULT": 0.8,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "atmosphereIntensity",
			"TYPE": "float",
			"LABEL": "Atmosphere Intensity",
			"DEFAULT": 0.9,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "cloudCoverage",
			"TYPE": "float",
			"LABEL": "Cloud Coverage",
			"DEFAULT": 0.7,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "iceCapSize",
			"TYPE": "float",
			"LABEL": "Ice Cap Size",
			"DEFAULT": 0.2,
			"MIN": 0.0,
			"MAX": 0.5
		},
		{
			"NAME": "volcanicIntensity",
			"TYPE": "float",
			"LABEL": "Volcanic Intensity",
			"DEFAULT": 0.6,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "crystalFormations",
			"TYPE": "float",
			"LABEL": "Crystal Formations",
			"DEFAULT": 0.4,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "alienVegetation",
			"TYPE": "float",
			"LABEL": "Alien Vegetation",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "oceanColorHue",
			"TYPE": "float",
			"LABEL": "Ocean Color Hue",
			"DEFAULT": 0.6,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "landColorHue",
			"TYPE": "float",
			"LABEL": "Land Color Hue",
			"DEFAULT": 0.3,
			"MIN": 0.0,
			"MAX": 1.0
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
			"NAME": "ringDensityCompensation",
			"TYPE": "bool",
			"LABEL": "Ring Density Compensation",
			"DEFAULT": true
		},
		{
			"NAME": "visualizeRings",
			"TYPE": "bool",
			"LABEL": "Visualize Ring Structure",
			"DEFAULT": false
		},
		{
			"NAME": "debugRingMapping",
			"TYPE": "bool",
			"LABEL": "Debug Ring Mapping",
			"DEFAULT": false
		}
	],
	"ISFVSN": "2"
}*/

/*
RING-BASED ALIEN PLANET SHADER INFORMATION:
- Author: Jim Cortez - Commune Project
- Based on: Ring-based spherical coordinate system for LED arrays
- Description: Alien planet using ring-centric mapping optimized for spherical LED array sampling
- Features: Ring-based projection, domain-warped FBM, alien terrain, volcanic features, crystal formations
- Coordinate System: Ring-based spherical distribution matching physical LED layout
- Camera: Positioned to view planet from the side (poles at top/bottom)
- Projection: Ring-based - each horizontal ring represents a latitude circle with variable LED density
- License: Custom/ISF/Book of Shaders
*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718

// Advanced noise functions from Book of Shaders
float hash(float n) {
    return fract(sin(n) * 43758.5453);
}

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f); // Smoothstep for smoother interpolation
    
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
    
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// Fractal Brownian Motion (FBM) for terrain generation
float fbm(vec2 p) {
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;
    
    for(int i = 0; i < 6; i++) {
        value += amplitude * noise(p * frequency);
        amplitude *= 0.5;
        frequency *= 2.0;
    }
    return value;
}

// Domain warping for more realistic terrain
float domainWarpedFbm(vec2 p) {
    vec2 q = vec2(fbm(p + vec2(0.0, 0.0)),
                  fbm(p + vec2(5.2, 1.3)));
    
    vec2 r = vec2(fbm(p + 4.0 * q + vec2(1.7, 9.2)),
                  fbm(p + 4.0 * q + vec2(8.3, 2.8)));
    
    return fbm(p + 4.0 * r);
}

// HSB to RGB conversion
vec3 hsb2rgb(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

// Smooth step function
float smoothstep2(float edge0, float edge1, float x) {
    float t = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
    return t * t * (3.0 - 2.0 * t);
}

// Ring-based coordinate system functions
// These implement the ring-centric spherical distribution methodology

// Convert ring-based UV to spherical coordinates
vec3 ringUVToSphere(vec2 uv) {
    // uv.x: position within ring (0 to 1, longitude)
    // uv.y: ring number (0 to 1, latitude)
    
    // Convert ring number to latitude
    float latitude = (uv.y - 0.5) * PI; // -PI/2 to +PI/2
    
    // Convert position in ring to longitude
    float longitude = uv.x * TWO_PI; // 0 to 2π
    
    // Convert spherical to Cartesian coordinates
    float x = cos(latitude) * cos(longitude);
    float y = cos(latitude) * sin(longitude);
    float z = sin(latitude);
    
    return vec3(x, y, z);
}

// Convert spherical coordinates to ring-based UV
vec2 sphereToRingUV(vec3 sphere) {
    // Convert Cartesian coordinates to spherical coordinates
    float longitude = atan(sphere.y, sphere.x);
    float latitude = asin(clamp(sphere.z, -1.0, 1.0));
    
    // Convert to ring-based UV coordinates
    float u = (longitude / TWO_PI);
    if (u < 0.0) u += 1.0; // Handle negative longitude
    float v = (latitude / PI) + 0.5; // Ring number (0 to 1)
    
    return vec2(u, v);
}

// Get ring number from world position
float getRingFromPosition(vec3 worldPos) {
    float latitude = asin(clamp(worldPos.z, -1.0, 1.0));
    return (latitude / PI + 0.5) * (numRings - 1.0) + 1.0;
}

// Get ring-based UV coordinates
vec2 getRingUV(float ringNum, float positionInRing, float ledsInRing) {
    float u = positionInRing / ledsInRing;  // Longitude (0 to 1)
    float v = (ringNum - 1.0) / (numRings - 1.0);  // Latitude (0 to 1)
    return vec2(u, v);
}

// Simulate LED density variation across rings
// This approximates the physical LED distribution pattern
float getLEDDensity(float ringNum) {
    // Approximate LED density based on latitude
    // Rings near equator have more LEDs than poles
    float normalizedRing = (ringNum - 1.0) / (numRings - 1.0); // 0 to 1
    float latitude = (normalizedRing - 0.5) * PI; // -PI/2 to +PI/2
    
    // Cosine-based density distribution (more LEDs at equator)
    float density = cos(latitude) * cos(latitude);
    
    // Ensure minimum density at poles
    density = mix(0.1, 1.0, density);
    
    return density;
}

// 3D rotation matrices
mat3 rotateX(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat3(
        1.0, 0.0, 0.0,
        0.0, c, -s,
        0.0, s, c
    );
}

mat3 rotateY(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat3(
        c, 0.0, s,
        0.0, 1.0, 0.0,
        -s, 0.0, c
    );
}

mat3 rotateZ(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat3(
        c, -s, 0.0,
        s, c, 0.0,
        0.0, 0.0, 1.0
    );
}

// Generate alien terrain height map with ring-based considerations
float generateAlienTerrain(vec2 uv) {
    // Apply ring density compensation if enabled
    vec2 samplingUV = uv;
    if (ringDensityCompensation) {
        float ringNum = uv.y * (numRings - 1.0) + 1.0;
        float density = getLEDDensity(ringNum);
        float maxDensity = 1.0; // Maximum density at equator
        samplingUV.x = uv.x * (density / maxDensity);
    }
    
    // Base continent shape using domain warped FBM
    float continents = domainWarpedFbm(samplingUV * landmassScale);
    
    // Add smaller landmasses with alien characteristics
    float islands = fbm(samplingUV * landmassScale * 2.0 + vec2(100.0, 200.0)) * 0.5;
    
    // Add volcanic features
    float volcanic = fbm(samplingUV * landmassScale * 4.0 + vec2(300.0, 400.0));
    volcanic = smoothstep2(0.7, 0.9, volcanic);
    
    // Combine and shape
    float terrain = continents + islands * 0.3 + volcanic * volcanicIntensity * 0.2;
    terrain = smoothstep2(0.3, 0.7, terrain);
    
    return terrain;
}

// Generate alien cloud patterns with ring-based sampling
float generateAlienClouds(vec2 uv) {
    vec2 samplingUV = uv;
    if (ringDensityCompensation) {
        float ringNum = uv.y * (numRings - 1.0) + 1.0;
        float density = getLEDDensity(ringNum);
        samplingUV.x = uv.x * density;
    }
    
    float clouds = fbm(samplingUV * 4.0 + TIME * 0.1);
    clouds += fbm(samplingUV * 8.0 + TIME * 0.05) * 0.5;
    clouds += fbm(samplingUV * 16.0 + TIME * 0.02) * 0.25;
    clouds = smoothstep2(0.4, 0.6, clouds);
    return clouds * cloudCoverage;
}

// Generate ice caps based on ring number (latitude)
float generateIceCaps(vec2 uv) {
    float ringNum = uv.y * (numRings - 1.0) + 1.0;
    float normalizedRing = (ringNum - 1.0) / (numRings - 1.0); // 0 to 1
    float latitude = abs(normalizedRing - 0.5) * 2.0; // 0 at equator, 1 at poles
    float ice = smoothstep2(1.0 - iceCapSize, 1.0, latitude);
    return ice;
}

// Generate volcanic regions with ring-based sampling
float generateVolcanicRegions(vec2 uv) {
    vec2 samplingUV = uv;
    if (ringDensityCompensation) {
        float ringNum = uv.y * (numRings - 1.0) + 1.0;
        float density = getLEDDensity(ringNum);
        samplingUV.x = uv.x * density;
    }
    
    float volcanic = fbm(samplingUV * 6.0 + vec2(100.0, 200.0));
    volcanic *= fbm(samplingUV * 12.0 + vec2(300.0, 400.0));
    volcanic = smoothstep2(0.7, 0.9, volcanic);
    return volcanic * volcanicIntensity;
}

// Generate crystal formations with ring-based sampling
float generateCrystalFormations(vec2 uv) {
    vec2 samplingUV = uv;
    if (ringDensityCompensation) {
        float ringNum = uv.y * (numRings - 1.0) + 1.0;
        float density = getLEDDensity(ringNum);
        samplingUV.x = uv.x * density;
    }
    
    float crystals = fbm(samplingUV * 10.0 + vec2(500.0, 600.0));
    crystals *= fbm(samplingUV * 20.0 + vec2(700.0, 800.0));
    crystals = smoothstep2(0.6, 0.8, crystals);
    return crystals * crystalFormations;
}

// Generate alien vegetation with ring-based considerations
float generateAlienVegetation(vec2 uv) {
    float ringNum = uv.y * (numRings - 1.0) + 1.0;
    float normalizedRing = (ringNum - 1.0) / (numRings - 1.0); // 0 to 1
    float latitude = abs(normalizedRing - 0.5) * 2.0; // 0 at equator, 1 at poles
    
    vec2 samplingUV = uv;
    if (ringDensityCompensation) {
        float density = getLEDDensity(ringNum);
        samplingUV.x = uv.x * density;
    }
    
    float vegetation = fbm(samplingUV * 8.0 + vec2(50.0, 150.0));
    vegetation *= smoothstep2(0.1, 0.4, latitude); // Wider temperate zones
    vegetation = smoothstep2(0.5, 0.7, vegetation);
    return vegetation * alienVegetation;
}

// Visualize ring structure for debugging
float visualizeRingStructure(vec2 uv) {
    float ringNum = uv.y * (numRings - 1.0) + 1.0;
    float ringProgress = fract(ringNum);
    
    // Show ring boundaries
    float ringBoundary = smoothstep(0.0, 0.02, ringProgress) * smoothstep(1.0, 0.98, ringProgress);
    
    // Show density variation
    float density = getLEDDensity(ringNum);
    float densityVis = density * 0.5;
    
    return ringBoundary + densityVis;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    
    // Apply rotation and tilt
    float rotation = TIME * rotationSpeed;
    float tilt = tiltAngle * PI / 180.0;
    float zRot = zRotation * PI / 180.0;
    
    // Convert to spherical coordinates using ring-based projection
    vec3 spherePos = ringUVToSphere(uv);
    
    // Apply rotations: Y for day/night, then X for tilt, then Z for alignment
    mat3 rotationMatrix = rotateZ(zRotation * PI / 180.0) * rotateX(tiltAngle * PI / 180.0) * rotateY(TIME * rotationSpeed);
    vec3 rotatedSphere = rotationMatrix * spherePos;
    
    // Convert back to ring-based coordinates
    vec2 worldUV = sphereToRingUV(rotatedSphere);
    
    // Handle wrapping for seamless projection
    worldUV.x = mod(worldUV.x, 1.0);
    if (worldUV.x < 0.0) worldUV.x += 1.0;
    worldUV.y = clamp(worldUV.y, 0.0, 1.0);
    
    // Generate terrain and features using ring-based sampling
    float terrain = generateAlienTerrain(worldUV);
    float clouds = generateAlienClouds(worldUV);
    float iceCaps = generateIceCaps(worldUV);
    float volcanic = generateVolcanicRegions(worldUV);
    float crystals = generateCrystalFormations(worldUV);
    float vegetation = generateAlienVegetation(worldUV);
    
    // Determine land vs ocean
    float isLand = smoothstep2(0.4, 0.6, terrain);
    float isOcean = 1.0 - isLand;
    
    // Alien ocean colors (using HSB for more control)
    vec3 deepOcean = hsb2rgb(vec3(oceanColorHue, 0.8, 0.3));
    vec3 shallowOcean = hsb2rgb(vec3(oceanColorHue, 0.6, 0.6));
    vec3 oceanColor = mix(deepOcean, shallowOcean, oceanDepth);
    
    // Alien land colors
    vec3 grassColor = hsb2rgb(vec3(landColorHue, 0.7, 0.5));
    vec3 volcanicColor = vec3(0.8, 0.3, 0.1);
    vec3 crystalColor = vec3(0.2, 0.8, 0.9);
    vec3 iceColor = vec3(0.9, 0.95, 1.0);
    vec3 mountainColor = vec3(0.4, 0.4, 0.4);
    
    // Mix land colors based on features
    vec3 landColor = grassColor;
    landColor = mix(landColor, volcanicColor, volcanic);
    landColor = mix(landColor, crystalColor, crystals);
    landColor = mix(landColor, iceColor, iceCaps);
    landColor = mix(landColor, mountainColor, terrain * 0.3);
    landColor = mix(landColor, grassColor, vegetation);
    
    // Final color composition
    vec3 baseColor = mix(oceanColor, landColor, isLand);
    
    // Add alien clouds with subtle color tint
    vec3 cloudColor = vec3(1.0, 0.95, 0.9); // Slightly warm tint
    baseColor = mix(baseColor, cloudColor, clouds * 0.7);
    
    // Add atmospheric glow at edges (simulating alien atmosphere)
    float edgeDistance = length(uv - vec2(0.5, 0.5)) * 2.0;
    float atmosphere = smoothstep2(0.8, 1.0, edgeDistance);
    vec3 atmosphereColor = hsb2rgb(vec3(0.6, 0.8, 0.8)); // Alien atmosphere color
    baseColor = mix(baseColor, atmosphereColor, atmosphere * atmosphereIntensity);
    
    // Add subtle vignette
    float vignette = smoothstep2(0.0, 0.8, edgeDistance);
    baseColor *= 1.0 - vignette * 0.3;
    
    // Add subtle color grading for more alien appearance
    baseColor = pow(baseColor, vec3(0.85)); // Slight contrast adjustment
    
    // Debug visualizations
    if (debugRingMapping) {
        // Show ring structure overlay
        float ringVis = visualizeRingStructure(worldUV);
        baseColor = mix(baseColor, vec3(1.0, 0.0, 0.0), ringVis * 0.3);
        
        // Show ring numbers
        float ringNum = worldUV.y * (numRings - 1.0) + 1.0;
        float ringHighlight = smoothstep(0.0, 0.02, fract(ringNum)) * smoothstep(1.0, 0.98, fract(ringNum));
        baseColor = mix(baseColor, vec3(0.0, 1.0, 0.0), ringHighlight * 0.5);
    }
    
    if (visualizeRings) {
        // Replace planet with ring visualization
        float ringVis = visualizeRingStructure(worldUV);
        baseColor = vec3(ringVis);
        
        // Add ring numbers as color
        float ringNum = worldUV.y * (numRings - 1.0) + 1.0;
        baseColor.r = fract(ringNum * 0.1);
        baseColor.g = fract(ringNum * 0.05);
        baseColor.b = fract(ringNum * 0.15);
    }
    
    gl_FragColor = vec4(baseColor, 1.0);
} 
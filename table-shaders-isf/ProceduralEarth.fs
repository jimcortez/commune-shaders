/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Enhanced with Book of Shaders techniques)",
	"DESCRIPTION": "Creates a procedural Earth planet with rectangular projection featuring realistic landmasses, oceans, and atmospheric effects. Features rotation speed, tilt angle, landmass scale, ocean depth, continent intensity, atmosphere intensity, cloud coverage, ice cap size, desert intensity, and vegetation intensity controls.",
	"INPUTS": [
		{
			"NAME": "rotationSpeed",
			"TYPE": "float",
			"LABEL": "Rotation Speed",
			"DEFAULT": 0.2,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "tiltAngle",
			"TYPE": "float",
			"LABEL": "Tilt Angle (Degrees)",
			"DEFAULT": 23.5,
			"MIN": 0.0,
			"MAX": 90.0
		},
		{
			"NAME": "landmassScale",
			"TYPE": "float",
			"LABEL": "Landmass Scale",
			"DEFAULT": 8.0,
			"MIN": 1.0,
			"MAX": 20.0
		},
		{
			"NAME": "oceanDepth",
			"TYPE": "float",
			"LABEL": "Ocean Depth",
			"DEFAULT": 0.6,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "continentIntensity",
			"TYPE": "float",
			"LABEL": "Continent Intensity",
			"DEFAULT": 0.7,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "atmosphereIntensity",
			"TYPE": "float",
			"LABEL": "Atmosphere Intensity",
			"DEFAULT": 0.8,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "cloudCoverage",
			"TYPE": "float",
			"LABEL": "Cloud Coverage",
			"DEFAULT": 0.6,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "iceCapSize",
			"TYPE": "float",
			"LABEL": "Ice Cap Size",
			"DEFAULT": 0.15,
			"MIN": 0.0,
			"MAX": 0.5
		},
		{
			"NAME": "desertIntensity",
			"TYPE": "float",
			"LABEL": "Desert Intensity",
			"DEFAULT": 0.4,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "vegetationIntensity",
			"TYPE": "float",
			"LABEL": "Vegetation Intensity",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Jim Cortez (Book of Shaders techniques)
- Based on: Book of Shaders, ISF, and Shadertoy knowledge
- Source: Originally sourced from editor.isf.video - ProceduralEarth by Jim Cortez
- Description: Procedural planet with domain-warped FBM terrain
- License: Custom/ISF/Book of Shaders
- Features: Procedural planet, domain-warped FBM, realistic landmasses, oceans, atmosphere, spinning, tilt, user controls
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

// Convert rectangular coordinates to spherical (equirectangular projection)
vec3 rectToSphere(vec2 uv) {
    // Map UV coordinates to longitude and latitude
    float longitude = (uv.x - 0.5) * TWO_PI;
    float latitude = (uv.y - 0.5) * PI;
    
    // Convert to 3D spherical coordinates
    float x = cos(latitude) * cos(longitude);
    float y = cos(latitude) * sin(longitude);
    float z = sin(latitude);
    
    return vec3(x, y, z);
}

// Convert spherical coordinates to rectangular (equirectangular projection)
vec2 sphereToRect(vec3 sphere) {
    // Convert 3D spherical coordinates to longitude and latitude
    float longitude = atan(sphere.y, sphere.x);
    float latitude = asin(clamp(sphere.z, -1.0, 1.0));
    
    // Map to UV coordinates
    float u = (longitude / TWO_PI) + 0.5;
    float v = (latitude / PI) + 0.5;
    
    return vec2(u, v);
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

// Generate terrain height map
float generateTerrain(vec2 uv) {
    // Base continent shape using domain warped FBM
    float continents = domainWarpedFbm(uv * landmassScale);
    
    // Add smaller landmasses
    float islands = fbm(uv * landmassScale * 2.0) * 0.5;
    
    // Combine and shape
    float terrain = continents + islands * 0.3;
    terrain = smoothstep2(0.3, 0.7, terrain);
    
    return terrain;
}

// Generate cloud patterns
float generateClouds(vec2 uv) {
    float clouds = fbm(uv * 4.0 + TIME * 0.1);
    clouds += fbm(uv * 8.0 + TIME * 0.05) * 0.5;
    clouds = smoothstep2(0.4, 0.6, clouds);
    return clouds * cloudCoverage;
}

// Generate ice caps based on latitude
float generateIceCaps(vec2 uv) {
    float latitude = abs(uv.y - 0.5) * 2.0; // 0 at equator, 1 at poles
    float ice = smoothstep2(1.0 - iceCapSize, 1.0, latitude);
    return ice;
}

// Generate desert regions
float generateDeserts(vec2 uv) {
    float latitude = abs(uv.y - 0.5) * 2.0;
    float desert = fbm(uv * 6.0 + vec2(100.0, 200.0));
    desert *= smoothstep2(0.2, 0.4, latitude); // Between 20-40 degrees
    desert = smoothstep2(0.6, 0.8, desert);
    return desert * desertIntensity;
}

// Generate vegetation
float generateVegetation(vec2 uv) {
    float latitude = abs(uv.y - 0.5) * 2.0;
    float vegetation = fbm(uv * 8.0 + vec2(50.0, 150.0));
    vegetation *= smoothstep2(0.1, 0.3, latitude); // Temperate zones
    vegetation = smoothstep2(0.5, 0.7, vegetation);
    return vegetation * vegetationIntensity;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    
    // Apply rotation and tilt
    float rotation = TIME * rotationSpeed;
    float tilt = tiltAngle * PI / 180.0;
    
    // Convert to spherical coordinates
    vec3 spherePos = rectToSphere(uv);
    
    // Apply rotations
    mat3 rotationMatrix = rotateY(rotation) * rotateX(tilt);
    vec3 rotatedSphere = rotationMatrix * spherePos;
    
    // Convert back to rectangular coordinates
    vec2 worldUV = sphereToRect(rotatedSphere);
    
    // Handle wrapping for seamless projection - ensure proper wrapping
    worldUV.x = mod(worldUV.x, 1.0);
    worldUV.y = clamp(worldUV.y, 0.0, 1.0);
    
    // Generate terrain and features
    float terrain = generateTerrain(worldUV);
    float clouds = generateClouds(worldUV);
    float iceCaps = generateIceCaps(worldUV);
    float deserts = generateDeserts(worldUV);
    float vegetation = generateVegetation(worldUV);
    
    // Determine land vs ocean
    float isLand = smoothstep2(0.4, 0.6, terrain);
    float isOcean = 1.0 - isLand;
    
    // Ocean colors (deep to shallow)
    vec3 deepOcean = vec3(0.0, 0.1, 0.3);
    vec3 shallowOcean = vec3(0.0, 0.4, 0.6);
    vec3 oceanColor = mix(deepOcean, shallowOcean, oceanDepth);
    
    // Land colors
    vec3 grassColor = vec3(0.2, 0.5, 0.2);
    vec3 desertColor = vec3(0.8, 0.7, 0.5);
    vec3 iceColor = vec3(0.9, 0.95, 1.0);
    vec3 mountainColor = vec3(0.4, 0.4, 0.4);
    
    // Mix land colors based on features
    vec3 landColor = grassColor;
    landColor = mix(landColor, desertColor, deserts);
    landColor = mix(landColor, iceColor, iceCaps);
    landColor = mix(landColor, mountainColor, terrain * 0.3);
    landColor = mix(landColor, grassColor, vegetation);
    
    // Final color composition
    vec3 baseColor = mix(oceanColor, landColor, isLand);
    
    // Add clouds
    vec3 cloudColor = vec3(1.0, 1.0, 1.0);
    baseColor = mix(baseColor, cloudColor, clouds * 0.7);
    
    // Add atmospheric glow at edges (simulating atmosphere)
    float edgeDistance = length(uv - vec2(0.5, 0.5)) * 2.0;
    float atmosphere = smoothstep2(0.8, 1.0, edgeDistance);
    vec3 atmosphereColor = vec3(0.5, 0.7, 1.0);
    baseColor = mix(baseColor, atmosphereColor, atmosphere * atmosphereIntensity);
    
    // Add subtle vignette
    float vignette = smoothstep2(0.0, 0.8, edgeDistance);
    baseColor *= 1.0 - vignette * 0.3;
    
    // Add subtle color grading for more realistic appearance
    baseColor = pow(baseColor, vec3(0.9)); // Slight contrast adjustment
    
    gl_FragColor = vec4(baseColor, 1.0);
} 
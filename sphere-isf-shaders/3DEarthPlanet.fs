/*{
	"CATEGORIES": ["Space", "Planets", "3D"],
	"CREDIT": "Jim Cortez - Commune Project (Enhanced with Book of Shaders techniques)",
	"DESCRIPTION": "Creates a 3D-looking Earth-like planet floating in space with realistic terrain, oceans, atmosphere, and zoom control. Features ray marching for true 3D perspective, atmospheric scattering, and realistic planet features.",
	"INPUTS": [
		{
			"NAME": "zoom",
			"TYPE": "float",
			"LABEL": "Camera Zoom",
			"DEFAULT": 2.0,
			"MIN": 0.5,
			"MAX": 10.0
		},
		{
			"NAME": "rotationSpeed",
			"TYPE": "float",
			"LABEL": "Rotation Speed",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
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
		},
		{
			"NAME": "cameraDistance",
			"TYPE": "float",
			"LABEL": "Camera Distance",
			"DEFAULT": 3.0,
			"MIN": 1.5,
			"MAX": 8.0
		}
	],
	"ISFVSN": "2"
}*/

/*
3D EARTH PLANET SHADER INFORMATION:
- Author: Jim Cortez - Commune Project
- Based on: Book of Shaders ray marching techniques, ISF, and 3D sphere rendering
- Description: 3D Earth-like planet with ray marching for true perspective and zoom control
- Features: Ray marching, atmospheric scattering, realistic terrain, 3D perspective, zoom control
- License: Custom/ISF/Book of Shaders
*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718
#define MAX_STEPS 100
#define MAX_DIST 10.0
#define SURF_DIST 0.001

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

// Convert 3D position to spherical coordinates (UV mapping)
vec2 sphereToUV(vec3 p) {
    float longitude = atan(p.z, p.x);
    float latitude = asin(clamp(p.y, -1.0, 1.0));
    
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

// Distance function for the planet sphere
float planetSDF(vec3 p) {
    // Perfect sphere - no terrain height variation
    float radius = 1.0;
    
    return length(p) - radius;
}

// Ray marching function
float rayMarch(vec3 ro, vec3 rd) {
    float d = 0.0;
    
    for(int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd * d;
        float dist = planetSDF(p);
        
        if(dist < SURF_DIST || d > MAX_DIST) break;
        d += dist * 0.5; // Conservative step size
    }
    
    return d;
}

// Calculate surface normal using gradient
vec3 calcNormal(vec3 p) {
    float d = planetSDF(p);
    vec2 e = vec2(0.001, 0);
    
    vec3 n = d - vec3(
        planetSDF(p - e.xyy),
        planetSDF(p - e.yxy),
        planetSDF(p - e.yyx)
    );
    
    return normalize(n);
}

// Atmospheric scattering
vec3 atmosphere(vec3 ro, vec3 rd, float dist) {
    float atmosphereRadius = 1.2;
    float atmosphereThickness = 0.2;
    
    // Calculate atmosphere intersection
    float b = dot(ro, rd);
    float c = dot(ro, ro) - atmosphereRadius * atmosphereRadius;
    float h = b * b - c;
    
    if(h < 0.0) return vec3(0.0);
    
    h = sqrt(h);
    float t1 = -b - h;
    float t2 = -b + h;
    
    if(t2 < 0.0) return vec3(0.0);
    
    t1 = max(t1, 0.0);
    t2 = min(t2, dist);
    
    float atmosphereDist = t2 - t1;
    if(atmosphereDist <= 0.0) return vec3(0.0);
    
    // Atmospheric color (blue sky)
    vec3 atmosphereColor = vec3(0.3, 0.6, 1.0);
    float intensity = atmosphereDist * atmosphereIntensity;
    
    return atmosphereColor * intensity;
}

// Get planet surface color
vec3 getPlanetColor(vec3 p) {
    // Apply rotation and tilt for texture mapping
    float rotation = TIME * rotationSpeed;
    float tilt = tiltAngle * PI / 180.0;
    
    mat3 rotationMatrix = rotateY(rotation) * rotateX(tilt);
    vec3 rotatedP = rotationMatrix * p;
    
    // Get UV coordinates
    vec2 uv = sphereToUV(rotatedP);
    uv.x = mod(uv.x, 1.0);
    uv.y = clamp(uv.y, 0.0, 1.0);
    
    // Generate features
    float terrain = generateTerrain(uv);
    float clouds = generateClouds(uv);
    float iceCaps = generateIceCaps(uv);
    float deserts = generateDeserts(uv);
    float vegetation = generateVegetation(uv);
    
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
    
    return baseColor;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    
    // Camera setup with zoom
    vec3 ro = vec3(0.0, 0.0, cameraDistance * zoom); // Camera position
    vec3 rd = normalize(vec3(uv * 2.0 - 1.0, -1.0)); // Ray direction
    
    // Adjust ray direction for zoom
    rd.z *= zoom;
    rd = normalize(rd);
    
    // Ray marching
    float dist = rayMarch(ro, rd);
    
    vec3 color = vec3(0.0);
    
    if(dist < MAX_DIST) {
        // Hit the planet
        vec3 hitPoint = ro + rd * dist;
        vec3 normal = calcNormal(hitPoint);
        
        // Get surface color
        color = getPlanetColor(hitPoint);
        
        // Add atmospheric scattering
        vec3 atmosphereColor = atmosphere(ro, rd, dist);
        color += atmosphereColor;
        
        // Add lighting (simple directional light)
        vec3 lightDir = normalize(vec3(1.0, 0.5, 1.0));
        float diffuse = max(0.0, dot(normal, lightDir));
        color *= 0.3 + 0.7 * diffuse;
        
        // Add specular highlight
        vec3 reflectDir = reflect(-lightDir, normal);
        float specular = pow(max(0.0, dot(rd, reflectDir)), 32.0);
        color += specular * 0.3;
        
    } else {
        // Space background
        color = vec3(0.01, 0.02, 0.05); // Dark space
        
        // Add atmospheric glow at edges
        vec3 atmosphereColor = atmosphere(ro, rd, MAX_DIST);
        color += atmosphereColor;
    }
    
    // Add subtle vignette
    float vignette = 1.0 - length(uv - 0.5) * 0.3;
    color *= vignette;
    
    // Color grading
    color = pow(color, vec3(0.9)); // Slight contrast adjustment
    
    gl_FragColor = vec4(color, 1.0);
} 
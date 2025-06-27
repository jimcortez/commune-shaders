/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Cosmic Jellyfish)",
	"DESCRIPTION": "Creates a mesmerizing cosmic jellyfish with animated tentacles and bioluminescent effects. Features a playful, organic creature floating through space with dynamic tentacle movements, particle effects, and customizable bioluminescence that creates a dreamlike underwater-in-space aesthetic.",
	"INPUTS": [
		{
			"DEFAULT": 8.0,
			"LABEL": "Tentacle Count",
			"MAX": 16.0,
			"MIN": 4.0,
			"NAME": "tentacleCount",
			"TYPE": "float"
		},
		{
			"DEFAULT": 0.8,
			"LABEL": "Tentacle Length",
			"MAX": 1.5,
			"MIN": 0.3,
			"NAME": "tentacleLength",
			"TYPE": "float"
		},
		{
			"DEFAULT": 2.0,
			"LABEL": "Tentacle Wave Speed",
			"MAX": 5.0,
			"MIN": 0.5,
			"NAME": "tentacleSpeed",
			"TYPE": "float"
		},
		{
			"DEFAULT": 0.8,
			"LABEL": "Jellyfish Hue",
			"MAX": 1.0,
			"MIN": 0.0,
			"NAME": "jellyfishHue",
			"TYPE": "float"
		},
		{
			"DEFAULT": 0.8,
			"LABEL": "Bioluminescence Intensity",
			"MAX": 1.5,
			"MIN": 0.0,
			"NAME": "bioluminescence",
			"TYPE": "float"
		},
		{
			"DEFAULT": 50.0,
			"LABEL": "Particle Count",
			"MAX": 200.0,
			"MIN": 10.0,
			"NAME": "particleCount",
			"TYPE": "float"
		},
		{
			"DEFAULT": 0.5,
			"LABEL": "Particle Speed",
			"MAX": 2.0,
			"MIN": 0.1,
			"NAME": "particleSpeed",
			"TYPE": "float"
		},
		{
			"DEFAULT": 0.3,
			"LABEL": "Swimming Motion",
			"MAX": 1.0,
			"MIN": 0.0,
			"NAME": "swimmingMotion",
			"TYPE": "float"
		},
		{
			"DEFAULT": 1.5,
			"LABEL": "Pulse Rate",
			"MAX": 3.0,
			"MIN": 0.5,
			"NAME": "pulseRate",
			"TYPE": "float"
		},
		{
			"DEFAULT": 0.02,
			"LABEL": "Tentacle Thickness",
			"MAX": 0.05,
			"MIN": 0.005,
			"NAME": "tentacleThickness",
			"TYPE": "float"
		}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Jim Cortez - Commune Project
- Source: Originally sourced from editor.isf.video - CosmicJellyfish by Jim Cortez
- Description: Cosmic jellyfish with animated tentacles and bioluminescent effects
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0
- Features: Tentacle animation, bioluminescence, particle effects, swimming motion
*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718

// Advanced noise functions
float hash(float n) {
    return fract(sin(n) * 43758.5453);
}

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
    
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// Fractal Brownian Motion
float fbm(vec2 p) {
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;
    
    for(int i = 0; i < 5; i++) {
        value += amplitude * noise(p * frequency);
        amplitude *= 0.5;
        frequency *= 2.0;
    }
    return value;
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

// Convert 2D UV to 3D sphere coordinates
vec3 uvToSphere(vec2 uv) {
    float longitude = (uv.x - 0.5) * TWO_PI;
    float latitude = (uv.y - 0.5) * PI;
    
    float x = cos(latitude) * cos(longitude);
    float y = cos(latitude) * sin(longitude);
    float z = sin(latitude);
    
    return vec3(x, y, z);
}

// Convert 3D sphere coordinates back to 2D UV
vec2 sphereToUV(vec3 sphere) {
    float longitude = atan(sphere.y, sphere.x);
    float latitude = asin(clamp(sphere.z, -1.0, 1.0));
    
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

// Generate tentacle path
vec2 tentaclePath(float t, float tentacleIndex, float time) {
    float angle = (tentacleIndex / tentacleCount) * TWO_PI;
    float wave = sin(t * 10.0 + time * tentacleSpeed + tentacleIndex) * 0.1;
    wave += sin(t * 5.0 + time * tentacleSpeed * 0.5 + tentacleIndex * 2.0) * 0.05;
    
    float x = cos(angle) * (0.1 + t * 0.3) + wave;
    float y = -t * tentacleLength + sin(t * 3.0 + time * tentacleSpeed * 0.3) * 0.05;
    
    return vec2(x, y);
}

// Calculate distance to tentacle
float tentacleDistance(vec2 uv, float tentacleIndex, float time) {
    float minDist = 1000.0;
    
    for(float t = 0.0; t <= 1.0; t += 0.02) {
        vec2 tentaclePos = tentaclePath(t, tentacleIndex, time);
        float dist = length(uv - tentaclePos);
        minDist = min(minDist, dist);
    }
    
    return minDist;
}

// Generate cosmic particles
float particleField(vec2 uv, float time) {
    float particles = 0.0;
    
    for(float i = 0.0; i < 200.0; i++) {
        if(i >= particleCount) break;
        
        float seed = i * 123.456;
        vec2 particlePos = vec2(
            hash(seed) + time * particleSpeed * (hash(seed + 1.0) - 0.5),
            hash(seed + 2.0) + time * particleSpeed * (hash(seed + 3.0) - 0.5)
        );
        
        float particleSize = hash(seed + 4.0) * 0.02 + 0.005;
        float dist = length(uv - particlePos);
        float particle = smoothstep2(particleSize, 0.0, dist);
        
        // Make particles twinkle
        float twinkle = sin(time * 3.0 + seed * 10.0) * 0.5 + 0.5;
        particles += particle * twinkle;
    }
    
    return particles;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    
    // Convert to 3D sphere coordinates
    vec3 spherePos = uvToSphere(uv);
    
    // Add swimming motion to the sphere
    float swimX = sin(TIME * swimmingMotion) * 0.1;
    float swimY = cos(TIME * swimmingMotion * 0.7) * 0.05;
    mat3 swimMatrix = rotateY(swimX) * rotateX(swimY);
    vec3 rotatedSphere = swimMatrix * spherePos;
    
    // Convert back to 2D UV
    vec2 worldUV = sphereToUV(rotatedSphere);
    worldUV = fract(worldUV);
    
    // Calculate distance from center
    vec2 center = vec2(0.5, 0.5);
    float dist = length(worldUV - center);
    
    // Create the cosmic jellyfish
    vec3 color = vec3(0.0);
    
    // Jellyfish bell (head)
    float bellSize = 0.25 + sin(TIME * pulseRate) * 0.02;
    float bellMask = smoothstep2(bellSize, bellSize - 0.05, dist);
    
    // Bell color with bioluminescence
    vec3 bellColor = hsb2rgb(vec3(jellyfishHue, 0.7, 0.8));
    float glow = sin(TIME * pulseRate * 2.0) * 0.3 + 0.7;
    bellColor *= glow * bioluminescence;
    
    color = mix(color, bellColor, bellMask);
    
    // Add bell texture
    float bellTexture = fbm(worldUV * 8.0 + TIME * 0.1);
    bellTexture *= smoothstep2(0.0, bellSize - 0.1, dist);
    color += bellTexture * bellColor * 0.3;
    
    // Generate tentacles
    float tentacleGlow = 0.0;
    for(float i = 0.0; i < 16.0; i++) {
        if(i >= tentacleCount) break;
        
        float tentacleDist = tentacleDistance(worldUV, i, TIME);
        float tentacleMask = smoothstep2(tentacleThickness, 0.0, tentacleDist);
        
        // Tentacle color with variation
        float tentacleHue = jellyfishHue + sin(i * 0.5) * 0.1;
        vec3 tentacleColor = hsb2rgb(vec3(tentacleHue, 0.8, 0.9));
        tentacleColor *= glow * bioluminescence;
        
        color = mix(color, tentacleColor, tentacleMask * 0.8);
        tentacleGlow += tentacleMask;
    }
    
    // Add tentacle glow effect
    color += tentacleGlow * hsb2rgb(vec3(jellyfishHue, 0.6, 0.3)) * bioluminescence * 0.5;
    
    // Add cosmic particles
    float particles = particleField(worldUV, TIME);
    vec3 particleColor = hsb2rgb(vec3(0.6, 0.8, 1.0));
    color += particles * particleColor * 0.3;
    
    // Add depth-based ambient occlusion
    float ao = smoothstep2(0.0, 0.3, dist) * 0.4;
    color *= 1.0 - ao;
    
    // Add subtle vignette
    float vignette = smoothstep2(0.0, 0.8, dist);
    color *= 1.0 - vignette * 0.2;
    
    // Add subtle chromatic aberration for cosmic effect
    float chromaOffset = 0.003;
    vec2 chromaUV = worldUV + vec2(chromaOffset, 0.0);
    float chromaDist = length(chromaUV - center);
    float chromaMask = smoothstep2(bellSize, bellSize - 0.05, chromaDist);
    color.r += chromaMask * 0.2;
    
    // Add subtle breathing effect to the entire jellyfish
    float breathing = sin(TIME * pulseRate * 0.5) * 0.1 + 0.9;
    color *= breathing;
    
    // Ensure we don't go over 1.0
    color = clamp(color, 0.0, 1.0);
    
    gl_FragColor = vec4(color, 1.0);
} 
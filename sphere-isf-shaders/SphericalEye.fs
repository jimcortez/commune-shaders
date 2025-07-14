/*{
	"CATEGORIES": [
		"Generator",
		"Organic",
		"Eye",
		"Spherical",
		"Realistic"
	],
	"CREDIT": "Jim Cortez - Commune Project (Enhanced with Book of Shaders techniques)",
	"DESCRIPTION": "Creates an advanced spherical human eyeball with realistic eye anatomy and dynamic visual effects. Features eye movement speed and range, iris color controls, pupil and iris sizing, vein intensity, reflection intensity, texture detail controls, and LFO parameters for realistic eye animations.",
	"INPUTS": [
		{
			"NAME": "eyeMovementSpeed",
			"TYPE": "float",
			"LABEL": "Eye Movement Speed",
			"DEFAULT": 0.8,
			"MIN": 0.0,
			"MAX": 3.0
		},
		{
			"NAME": "eyeMovementRange",
			"TYPE": "float",
			"LABEL": "Eye Movement Range (Degrees)",
			"DEFAULT": 12.0,
			"MIN": 0.0,
			"MAX": 30.0
		},
		{
			"NAME": "irisHue",
			"TYPE": "float",
			"LABEL": "Iris Hue",
			"DEFAULT": 0.6,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "irisSaturation",
			"TYPE": "float",
			"LABEL": "Iris Saturation",
			"DEFAULT": 0.8,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "irisBrightness",
			"TYPE": "float",
			"LABEL": "Iris Brightness",
			"DEFAULT": 0.7,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "scleraSize",
			"TYPE": "float",
			"LABEL": "Sclera Size",
			"DEFAULT": 0.8,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "scleraBrightness",
			"TYPE": "float",
			"LABEL": "Sclera Brightness",
			"DEFAULT": 0.95,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "pupilSize",
			"TYPE": "float",
			"LABEL": "Pupil Size",
			"DEFAULT": 0.12,
			"MIN": 0.05,
			"MAX": 0.3
		},
		{
			"NAME": "irisSize",
			"TYPE": "float",
			"LABEL": "Iris Size",
			"DEFAULT": 0.35,
			"MIN": 0.2,
			"MAX": 0.5
		},
		{
			"NAME": "veinIntensity",
			"TYPE": "float",
			"LABEL": "Vein Intensity",
			"DEFAULT": 0.4,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "reflectionIntensity",
			"TYPE": "float",
			"LABEL": "Reflection Intensity",
			"DEFAULT": 0.9,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "textureDetail",
			"TYPE": "float",
			"LABEL": "Texture Detail",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "textureDetail2",
			"TYPE": "float",
			"LABEL": "Texture Detail 2",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 5.0
		},
		{
			"NAME": "textureDetail3",
			"TYPE": "float",
			"LABEL": "Texture Detail 3",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 75.0
		},
		{
			"NAME": "lfoRate",
			"TYPE": "float",
			"LABEL": "LFO Rate",
			"DEFAULT": 4.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "lfoRateAmp",
			"TYPE": "float",
			"LABEL": "LFO Rate Amplitude",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "enableChromaticAberration",
			"TYPE": "bool",
			"LABEL": "Enable Chromatic Aberration",
			"DEFAULT": true
		},
		{
			"NAME": "enableVeins",
			"TYPE": "bool",
			"LABEL": "Enable Veins",
			"DEFAULT": true
		},
		{
			"NAME": "enableReflections",
			"TYPE": "bool",
			"LABEL": "Enable Reflections",
			"DEFAULT": true
		},
		{
			"NAME": "zoom",
			"TYPE": "float",
			"LABEL": "Zoom",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 3.0
		}
	],
	"ISFVSN": "2",
	"VSN": "2.0"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Jim Cortez (Book of Shaders techniques)
- Based on: Book of Shaders, ISF, and Shadertoy knowledge
- Source: Originally sourced from editor.isf.video - SphericalEye by Jim Cortez
- Description: Advanced spherical human eyeball with realistic anatomy
- License: Custom/ISF/Book of Shaders
- Features: Spherical human eyeball, advanced anatomy, dynamic effects, user controls
- Updated: Following ISF 2.0 best practices and documentation guidelines
*/

// ISF Standard Constants
#define PI 3.14159265359
#define TWO_PI 6.28318530718
#define HALF_PI 1.57079632679

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

// Enhanced FBM with multiple detail levels
float fbm(vec2 p) {
    float f = 0.0;
    
    f += 0.50000 * textureDetail * noise(p); p = p * 2.02;
    f += 0.25000 * textureDetail2 * noise(p); p = p * 2.03;
    f += 0.12500 * noise(p); p = p * 2.01;
    f += 0.06250 * noise(p); p = p * 2.04;
    f += 0.03125 * noise(p);
    
    return f / 0.984375;
}

// Length function for better shape control
float length2(vec2 p) {
    vec2 q = p * p * p * p;
    return pow(q.x + q.y, 1.0 / 4.0);
}

// HSB to RGB conversion (from Book of Shaders)
vec3 hsb2rgb(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    rgb = rgb * rgb * (3.0 - 2.0 * rgb); // Smoothstep for better color transitions
    return c.z * mix(vec3(1.0), rgb, c.y);
}

// Smooth step function for better transitions
float smoothstep2(float edge0, float edge1, float x) {
    float t = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
    return t * t * (3.0 - 2.0 * t);
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

// Realistic eye movement patterns with smooth saccades and natural sweeps
vec2 getEyeMovement(float time, float speed, float range) {
    // Slower, more natural base movement patterns
    float baseX = sin(time * speed * 0.3) * 0.6;
    float baseY = cos(time * speed * 0.4) * 0.5;
    
    // Smooth horizontal sweeps (left to right)
    float sweepX = sin(time * speed * 0.15) * 0.8;
    
    // Vertical gaze changes (up and down)
    float gazeY = sin(time * speed * 0.25) * 0.4;
    
    // Subtle micro-movements (natural tremor)
    float tremorX = sin(time * speed * 12.0) * 0.05;
    float tremorY = cos(time * speed * 10.0) * 0.05;
    
    // Occasional larger movements (saccades)
    float saccadeX = sin(time * speed * 0.08) * smoothstep(0.0, 0.5, sin(time * speed * 0.02)) * 0.3;
    float saccadeY = cos(time * speed * 0.12) * smoothstep(0.0, 0.5, cos(time * speed * 0.03)) * 0.2;
    
    // Combine movements with weighted blending
    vec2 movement = vec2(
        baseX * 0.3 + sweepX * 0.4 + saccadeX * 0.2 + tremorX * 0.1,
        baseY * 0.3 + gazeY * 0.4 + saccadeY * 0.2 + tremorY * 0.1
    );
    
    // Apply smoothstep for more natural motion curves
    movement = smoothstep(-1.0, 1.0, movement * 0.5 + 0.5) * 2.0 - 1.0;
    
    // Return movement as UV offset (not radians)
    return movement * range * 0.1; // Scale down for UV space
}

void main() {
    // Get the base UV coordinates using ISF standard
    vec2 uv = isf_FragNormCoord;
    
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 centered = uv - 0.5;
    if (aspect > 1.0) {
        centered.x *= aspect;
    }
    centered /= zoom;
    centered += getEyeMovement(TIME, eyeMovementSpeed, eyeMovementRange);
    float dist = length(centered);
    float eyeMask = step(dist, 0.5);
    // Set background color
    vec3 bgColor = vec3(0.92, 0.93, 0.95); // light gray
    vec3 col = bgColor;
    // Debug: add a red outline at the mask edge
    float outline = smoothstep(0.495, 0.5, dist) - smoothstep(0.5, 0.505, dist);
    if (eyeMask > 0.0) {
        // Eye color logic (iris, pupil, sclera, etc.)
        vec2 p = centered; // for mapping
        float r = length(p);
        float a = atan(p.y, p.x);
        // Dynamic distortion based on LFO
        float dd = 0.2 * sin(lfoRate * TIME) * lfoRateAmp;
        float ss = 1.0 + clamp(1.0 - r, 0.0, 1.0) * dd;
        r *= ss;
        // Create the eye structure with enhanced realism
        col = vec3(0.0, 0.3, 0.4); // Base sclera color
        // Enhanced sclera texture
        float f = fbm(5.0 * p);
        col = mix(col, vec3(0.2, 0.5, 0.4), f);
        col = mix(col, vec3(0.9, 0.6, 0.2), 1.0 - smoothstep(0.2, 0.6, r));
        // Add iris texture and detail
        a += textureDetail3 * fbm(20.0 * p);
        f = smoothstep(0.3, 1.0, fbm(vec2(20.0 * a, 6.0 * r)));
        col = mix(col, vec3(1.0, 1.0, 1.0), f);
        // Add iris color using HSB
        vec3 irisColor = hsb2rgb(vec3(irisHue, irisSaturation, irisBrightness));
        float irisMask = smoothstep2(irisSize, irisSize - 0.05, r);
        col = mix(col, irisColor, irisMask * 0.8);
        // Enhanced iris detail
        f = smoothstep(0.4, 0.9, fbm(vec2(15.0 * a, 10.0 * r)));
        col *= 1.0 - 0.5 * f;
        col *= 1.0 - 0.25 * smoothstep(0.6, 0.8, r);
        // Pupil with realistic depth
        float pupilMask = smoothstep2(pupilSize, pupilSize - 0.02, r);
        vec3 pupilColor = vec3(0.0, 0.0, 0.0);
        float pupilDepth = smoothstep2(0.0, pupilSize * 0.5, r);
        pupilColor += pupilDepth * 0.1;
        col = mix(col, pupilColor, pupilMask);
        // Enhanced reflections that move with the eye (conditional)
        if (enableReflections) {
            f = 1.0 - smoothstep(0.0, 0.6, length2(mat2(0.6, 0.8, -0.8, 0.6) * (p - vec2(0.3, 0.5)) * vec2(1.0, 2.0)));
            col += vec3(1.0, 0.9, 0.9) * f * 0.985 * reflectionIntensity;
        }
        // Add color variation based on position
        col *= vec3(0.8 + 0.2 * cos(r * a));
        // Eye outline
        f = 1.0 - smoothstep(0.2, 0.25, r);
        col = mix(col, vec3(0.0), f);
        f = smoothstep(scleraSize - 0.03, scleraSize, r);
        col = mix(col, vec3(1.0), f);
        // Add realistic veins to sclera (conditional)
        if (enableVeins) {
            float veins = fbm(uv * 8.0 + TIME * 0.1) * veinIntensity;
            veins *= smoothstep2(0.0, scleraSize * 0.5, r) * smoothstep2(scleraSize * 0.6, scleraSize * 0.5, r);
            veins *= smoothstep2(0.0, 0.1, veins);
            col += veins * vec3(0.8, 0.2, 0.2) * 0.15;
        }
        // Add subtle chromatic aberration for realism (conditional)
        if (enableChromaticAberration) {
            float chromaOffset = 0.002;
            vec2 chromaUV = p + vec2(chromaOffset, 0.0);
            float chromaDist = length(chromaUV);
            float chromaMask = smoothstep2(irisSize, irisSize - 0.05, chromaDist);
            col.r += chromaMask * 0.1;
        }
        // Add subtle ambient occlusion around the eye
        float ao = smoothstep2(0.0, 0.2, r) * 0.4;
        col *= 1.0 - ao;
        // Final color adjustment for sclera brightness
        float scleraMask = smoothstep2(scleraSize * 0.6, scleraSize * 0.65, r);
        vec3 scleraColor = vec3(scleraBrightness) + vec3(0.02, 0.01, 0.01);
        col = mix(col, scleraColor, scleraMask * 0.3);
    }
    // Add debug outline
    col = mix(col, vec3(1.0, 0.0, 0.0), outline);
    // Ensure proper alpha channel handling as per ISF documentation
    gl_FragColor = vec4(col, 1.0);
} 

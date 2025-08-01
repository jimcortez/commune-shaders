/*{
	"CREDIT": "Adapted by Shader Shaper from mojovideotech",
	"DESCRIPTION": "Fractal noise-based tiling effect with modulation and texture control.",
	"CATEGORIES": ["fractal", "abstract"],
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "tileScale",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "textureDetail1",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "textureDetail2",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 5.0
		},
		{
			"NAME": "textureWarp",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 75.0
		},
		{
			"NAME": "modulationAmount",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "modulationRate",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "colorShift",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 3.0
		},
		{
			"NAME": "contrast",
			"TYPE": "float",
			"DEFAULT": 0.7,
			"MIN": 0.1,
			"MAX": 2.0
		}
	]
}*/

#define PI 3.14159265359

const mat2 m = mat2(0.80, 0.60, -0.60, 0.80);

// Hash function for pseudo-random noise
float hash(float n) {
    return fract(sin(n) * 43758.5453);
}

// Noise function
float noise(vec2 x) {
    vec2 p = floor(x);
    vec2 f = fract(x);
    f = f * f * (3.0 - 2.0 * f);
    float n = (p.x + p.y * 57.0) * tileScale;
    return mix(mix(hash(n + 0.0), hash(n + 1.0), f.x),
               mix(hash(n + 57.0), hash(n + 58.0), f.x), f.y);
}

// Fractal Brownian Motion (FBM) function
float fbm(vec2 p) {
    float f = 0.0;
    f += (0.50000 * textureDetail1) * noise(p);
    p = m * p * 2.02;
    f += (0.25000 * textureDetail2) * noise(p);
    p = m * p * 2.03;
    f += 0.12500 * noise(p);
    p = m * p * 2.01;
    f += 0.06250 * noise(p);
    p = m * p * 2.04;
    f += 0.03125 * noise(p);
    return f / 0.984375;
}

// Length function for distortion
float length2(vec2 p) {
    vec2 q = p * p * p * p;
    return pow(q.x + q.y, 1.0 / 4.0);
}

void main() {
    vec2 q = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = -1.0 + 2.0 * q;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    float r = length(p);
    float a = atan(p.y, p.x);
    
    float modulation = 0.2 * sin(modulationRate * TIME) * modulationAmount;
    float scalingFactor = 1.0 + clamp(1.0 - r, 0.0, 1.0) * modulation;
    r *= scalingFactor;

    vec3 col = vec3(0.0, 0.3, 0.4);
    float f = fbm(5.0 * p);
    col = mix(col, vec3(0.2, 0.5, 0.4), f);
    col = mix(col, vec3(0.9, 0.6, 0.2), 1.0 - smoothstep(0.2, 0.6, r));

    a += (0.05 * textureWarp) * fbm(20.0 * p);
    f = smoothstep(0.3, 1.0, fbm(vec2(20.0 * a, 6.0 * r)));
    col = mix(col, vec3(1.0, 1.0, 1.0), f);
    
    f = smoothstep(0.4, 0.9, fbm(vec2(15.0 * a, 10.0 * r)));
    col *= 1.0 - 0.5 * f;
    col *= 1.0 - 0.25 * smoothstep(0.6, 0.8, r);

    f = 1.0 - smoothstep(0.0, 0.6, length2(mat2(0.6, 0.8, -0.8, 0.6) * (p - vec2(0.3, 0.5)) * vec2(1.0, 2.0)));
    col += vec3(1.0, 0.9, 0.9) * f * 0.985;
    col *= vec3(0.8 + 0.2 * cos(r * a * colorShift));
    
    f = 1.0 - smoothstep(0.2, 0.25, r);
    col = mix(col, vec3(0.0), f);
    f = smoothstep(0.79, 0.82, r);
    col = mix(col, vec3(1.0), f);

    col *= contrast * (0.5 + 0.5 * pow(16.0 * q.x * q.y * (1.0 - q.x) * (1.0 - q.y), 0.1));
    
	gl_FragColor = vec4(col, 1.0);
}

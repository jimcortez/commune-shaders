/*{
	"CREDIT": "Adapted by Shader Shaper from mojovideotech",
	"DESCRIPTION": "Cosmic flare effect with customizable color, brightness, and warp.",
	"CATEGORIES": ["fluid", "light"],
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.0,
			"MAX": 5.0
		},
		{
			"NAME": "rayBrightness",
			"TYPE": "float",
			"DEFAULT": 6.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "gamma",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": -15.0,
			"MAX": 15.0
		},
		{
			"NAME": "spotBrightness",
			"TYPE": "float",
			"DEFAULT": 5.0,
			"MIN": -15.0,
			"MAX": 15.0
		},
		{
			"NAME": "rayDensity",
			"TYPE": "float",
			"DEFAULT": 10.0,
			"MIN": 0.0,
			"MAX": 100.0
		},
		{
			"NAME": "curvature",
			"TYPE": "float",
			"DEFAULT": 300.0,
			"MIN": 1.0,
			"MAX": 1080.0
		},
		{
			"NAME": "frequency",
			"TYPE": "float",
			"DEFAULT": 8.0,
			"MIN": 1.0,
			"MAX": 60.0
		},
		{
			"NAME": "warpAmount",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

#define PI 3.14159265359
#define MAX_ITER 5  // Fixed iteration count for WebGL 1.0 compatibility

// Hash function for noise generation
float hash(float n) {
    return fract(sin(n) * 43758.5453);
}

// Noise function for generating texture
float noise(vec2 x) {
    x *= 1.75;
    vec2 p = floor(x);
    vec2 f = fract(x);
    f = f * f * (3.0 - 2.0 * f);
    float n = p.x + p.y * 57.0;
    return mix(mix(hash(n + 0.0), hash(n + 1.0), f.x),
               mix(hash(n + 57.0), hash(n + 58.0), f.x), f.y);
}

// Fractal Brownian Motion (FBM) for cloud-like texture
mat2 rotationMatrix = mat2(0.80, 0.60, -0.60, 0.80);
float fbm(vec2 p) {	
	float z = 2.0;
	float rz = 0.0;
	p *= 0.25;
	for (int i = 0; i < MAX_ITER; i++) {
		rz += (sin(noise(p) * frequency) * 0.5 + 0.5) / z;
		z *= 2.0;
		p = p * 2.0 * rotationMatrix;
	}
	return rz;
}

void main() {
    float t = -TIME * 0.03;
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy - 0.5;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv *= curvature * 0.05 + 0.0001;

    float r = sqrt(dot(uv, uv));
    float x = dot(normalize(uv), vec2(0.5, 0.0)) + t;	
    float y = dot(normalize(uv), vec2(0.0, 0.5)) + t;

    // Apply warp effect based on `warpAmount`
    x = mix(x, fbm(vec2(y * rayDensity * 0.5, r + x * rayDensity * 0.2)), warpAmount);
    y = mix(y, fbm(vec2(r + y * rayDensity * 0.1, x * rayDensity * 0.5)), warpAmount);

    // Compute light intensity
    float val = fbm(vec2(r + y * rayDensity, r + x * rayDensity - y));
    val = smoothstep(gamma * 0.02 - 0.1, rayBrightness + (gamma * 0.02 - 0.1) + 0.001, val);
    val = sqrt(val);

    // Apply color scaling
    vec3 col = val / vec3(7.0, 2.0, 1.0);
    col = clamp(1.0 - col, 0.0, 1.0);

    // Adjust brightness and blending
    col = mix(col, vec3(1.0), spotBrightness - r / (0.1 * curvature) * 200.0 / brightness);
	
    gl_FragColor = vec4(col, 1.0);
}

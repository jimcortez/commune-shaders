/*{
	"CREDIT": "Adapted by Shader Shaper from mojovideotech",
	"DESCRIPTION": "Fractal cosmic field with animated stars and warp controls.",
	"CATEGORIES": ["fractal", "space"],
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "fractalStrength",
			"TYPE": "float",
			"DEFAULT": 7.0,
			"MIN": 2.0,
			"MAX": 15.0
		},
		{
			"NAME": "fractalWarp",
			"TYPE": "float",
			"DEFAULT": 0.03,
			"MIN": 0.0,
			"MAX": 0.1
		},
		{
			"NAME": "motionSpeed",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.1,
			"MAX": 5.0
		},
		{
			"NAME": "starDensity",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 5.0
		},
		{
			"NAME": "colorR",
			"TYPE": "float",
			"DEFAULT": 1.8,
			"MIN": 0.5,
			"MAX": 3.0
		},
		{
			"NAME": "colorG",
			"TYPE": "float",
			"DEFAULT": 1.4,
			"MIN": 0.5,
			"MAX": 3.0
		},
		{
			"NAME": "colorB",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 3.0
		},
		{
			"NAME": "glowIntensity",
			"TYPE": "float",
			"DEFAULT": 0.7,
			"MIN": 0.1,
			"MAX": 2.0
		}
	]
}*/

#define PI 3.14159265359
#define MAX_ITER 32  // Fixed iteration count for WebGL 1.0 compatibility

// Fractal field function
float field(vec3 p) {
	float strength = fractalStrength + fractalWarp * log(1.e-6 + fract(sin(TIME * motionSpeed) * 4373.11));
	float accum = 0.0;
	float prev = 0.0;
	float totalWeight = 0.0;

	for (int i = 0; i < MAX_ITER; i++) {
		float mag = dot(p, p);
		p = abs(p) / mag + vec3(-0.51, -0.4, -1.3);
		float weight = exp(-float(i) / 7.0);
		accum += weight * exp(-strength * pow(abs(mag - prev), 2.3));
		totalWeight += weight;
		prev = mag;
	}

	return max(0.0, 5.0 * accum / totalWeight - 0.7);
}

// Star generation function
vec3 nrand3(vec2 co) {
	vec3 a = fract(cos(co.x * 8.3e-3 + co.y) * vec3(1.3e5, 4.7e5, 2.9e5));
	vec3 b = fract(sin(co.x * 0.3e-3 + co.y) * vec3(8.1e5, 1.0e5, 0.1e5));
	return mix(a, b, 0.5);
}

void main() {
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy - 1.0;
	vec2 uvs = uv * RENDERSIZE.xy / max(RENDERSIZE.x, RENDERSIZE.y);
	
	// Fractal point calculations
	vec3 p = vec3(uvs / 4.0, 0) + vec3(2.0, -1.3, -1.0);
	p += 0.15 * vec3(sin(TIME / 16.0 * motionSpeed), sin(TIME / 12.0 * motionSpeed), sin(TIME / 128.0 * motionSpeed));

	vec3 p2 = vec3(uvs / (4.0 + sin(TIME * 0.11) * 0.2 + 0.2 + sin(TIME * 0.15) * 0.3 + 0.4), 1.5) + vec3(2.0, -1.3, -1.0);
	p2 += 0.15 * vec3(sin(TIME / 16.0 * motionSpeed), sin(TIME / 12.0 * motionSpeed), sin(TIME / 128.0 * motionSpeed));

	vec3 p3 = vec3(uvs / (4.0 + sin(TIME * 0.14) * 0.23 + 0.23 + sin(TIME * 0.19) * 0.31 + 0.31), 0.5) + vec3(2.0, -1.3, -1.0);
	p3 += 0.15 * vec3(sin(TIME / 16.0 * motionSpeed), sin(TIME / 12.0 * motionSpeed), sin(TIME / 128.0 * motionSpeed));

	// Compute field values
	float t = field(p);
	float t2 = field(p2);
	float t3 = field(p3);

	// Star effect
	float v = (1.0 - exp((abs(uv.x) - 1.0) * 6.0)) * (1.0 - exp((abs(uv.y) - 1.0) * 6.0));
	
	// Compute color variations
	vec4 c1 = mix(0.4, 1.0, v) * vec4(colorR * t * t * t, colorG * t * t, colorB * t, 1.0);
	vec4 c2 = mix(0.4, 1.0, v) * vec4(colorG * t2 * t2 * t2, colorR * t2 * t2, colorB * t2, 1.0);
	vec4 c3 = mix(0.4, 1.0, v) * vec4(colorG * t3 * t3 * t3, colorR * t3 * t3, colorB * t3, 1.0);

	// Star flickering effect based on screen position
	c1.b *= mod(gl_FragCoord.y + 1.0, 2.0) * starDensity * 1.4;
	c2.r *= mod(gl_FragCoord.y, 2.0) * starDensity * 3.4;
	c3.g *= mod(gl_FragCoord.y, 2.0) * starDensity * 2.4;

	// Final color output with glow effect
	gl_FragColor = (c1 * 0.7 + c2 * 0.5 + c3 * 0.3) * glowIntensity;
}

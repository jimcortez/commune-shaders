/*{
	"CREDIT": "Adapted by Shader Shaper from codevinsky",
	"DESCRIPTION": "Fire effect with adjustable speed, size, brightness, and turbulence.",
	"CATEGORIES": ["fire", "abstract"],
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "fireSpeed",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": -10.0,
			"MAX": 10.0
		},
		{
			"NAME": "fireSize",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 2.5
		},
		{
			"NAME": "fireBrightness",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": -5.0,
			"MAX": 5.0
		},
		{
			"NAME": "turbulence",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "colorIntensity",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 3.0
		},
		{
			"NAME": "fireSpread",
			"TYPE": "float",
			"DEFAULT": 8.0,
			"MIN": 4.0,
			"MAX": 16.0
		},
		{
			"NAME": "flameDetail",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 2.0,
			"MAX": 8.0
		},
		{
			"NAME": "flameShift",
			"TYPE": "float",
			"DEFAULT": 1.6,
			"MIN": 0.5,
			"MAX": 3.0
		}
	]
}*/

#define MAX_ITER 8  // Fixed iteration count for WebGL 1.0 compatibility

// Random noise function
float rand(vec2 n) {
    return fract(cos(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

// Smooth noise function
float noise(vec2 n) {
    const vec2 d = vec2(0.0, 1.0);
    vec2 b = floor(n), f = smoothstep(vec2(0.0), vec2(1.0), fract(n));
    return mix(mix(rand(b), rand(b + d.yx), f.x), mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
}

// Fractal Brownian Motion (FBM) function
float fbm(vec2 n) {
    float total = 0.0, amplitude = 1.0;
    
    for (int i = 0; i < MAX_ITER; i++) {
        if (float(i) >= flameDetail) break;  // Stop when reaching dynamic limit

        total += noise(n) * amplitude;
        n += n * turbulence;
        amplitude *= 0.5 * fireSize;
    }
    return total;
}

void main() {
    // Fire color gradient
    const vec3 c1 = vec3(0.5, 0.0, 0.1);
    const vec3 c2 = vec3(0.9, 0.0, 0.0);
    const vec3 c3 = vec3(0.2, 0.0, 0.0);
    const vec3 c4 = vec3(1.0, 0.9, 0.0);
    const vec3 c5 = vec3(0.1);
    const vec3 c6 = vec3(0.9);
	
    vec2 speed = vec2(0.7, 0.4) * fireSpeed;
    vec2 p = gl_FragCoord.xy * fireSpread / RENDERSIZE.xx;
    
    // Flame movement
    float q = fbm(p - TIME * 0.1);
    vec2 r = vec2(fbm(p + q + TIME * speed.x - p.x - p.y), fbm(p + q - TIME * speed.y) * fireBrightness);
    
    // Color blending
    vec3 c = mix(c1, c2, fbm(p + r)) + mix(c3, c4, r.x) - mix(c5, c6, r.y);
    
    // Apply shift and intensity
    c *= cos(flameShift * gl_FragCoord.y / RENDERSIZE.y) * colorIntensity;
    
    gl_FragColor = vec4(c, 1.0);
}

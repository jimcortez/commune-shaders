/*{
	"CREDIT": "Adapted by Shader Shaper",
	"DESCRIPTION": "Animated circular pattern with adjustable colors and speed.",
	"CATEGORIES": ["pattern", "abstract"],
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.05,
			"MIN": 0.01,
			"MAX": 0.2
		},
		{
			"NAME": "radiusScale",
			"TYPE": "float",
			"DEFAULT": 0.07,
			"MIN": 0.01,
			"MAX": 0.2
		},
		{
			"NAME": "spacing",
			"TYPE": "float",
			"DEFAULT": 0.04,
			"MIN": 0.01,
			"MAX": 0.1
		},
		{
			"NAME": "colorVariation",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0.2,
			"MAX": 1.0
		},
		{
			"NAME": "shapeCount",
			"TYPE": "float",
			"DEFAULT": 60.0,
			"MIN": 10.0,
			"MAX": 100.0
		},
		{
			"NAME": "circleSharpness",
			"TYPE": "float",
			"DEFAULT": 0.01,
			"MIN": 0.001,
			"MAX": 0.05
		},
		{
			"NAME": "backgroundFade",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "animationSpeed",
			"TYPE": "float",
			"DEFAULT": 20.0,
			"MIN": 5.0,
			"MAX": 50.0
		}
	]
}*/

#define MAX_ITER 60  // Fixed iteration count for WebGL 1.0 compatibility

// Signed distance function for a circle
float sdCircle(vec2 p, float r) {
    return length(p) - r;
}

// Color palette function
vec3 palette(float t) {
    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 0.6);
    vec3 d = vec3(0.80, 0.90, 0.30);
    return a + b * cos(6.28318 * (c * t + d));
}

void main() {
    vec2 p = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
    vec3 col = vec3(backgroundFade);
    vec2 vec;
    float d;

    float iTime = TIME / animationSpeed;

    for (int i = 0; i < MAX_ITER; i++) {
        if (float(i) >= shapeCount) break;  // Stop when reaching dynamic limit

        vec = vec2(cos(float(i) * iTime * speed) * (float(i) * spacing), 
                   sin(float(i) * iTime * speed) * (float(i) * spacing));

        d = sdCircle(p + vec, float(i) * radiusScale);

        col = (d > 0.0) ? palette(colorVariation * mod(float(i), 10.0)) : col;
        col = mix(col, vec3(0.0), smoothstep(circleSharpness, 0.0, abs(d)));
    }

    gl_FragColor = vec4(col, 1.0);
}

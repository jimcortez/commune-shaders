/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: codevinsky)",
	"DESCRIPTION": "Creates a realistic fire effect with fractal noise and dynamic color mixing. Features customizable speed, size, and brightness controls that generate animated flames with authentic fire colors and movement patterns.",
	"INPUTS": [
		{
			"DEFAULT": 1,
			"LABEL": "speed",
			"MAX": 10,
			"MIN": -10,
			"NAME": "speedN",
			"TYPE": "float"
		},
		{
			"DEFAULT": 1,
			"LABEL": "size",
			"MAX": 2.5,
			"MIN": 0,
			"NAME": "size",
			"TYPE": "float"
		},
		{
			"DEFAULT": 1,
			"LABEL": "brightness",
			"MAX": 5,
			"MIN": -5,
			"NAME": "brightness",
			"TYPE": "float"
		}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: codevinsky (https://www.shadertoy.com/view/XsXXRN)
- Source: Originally sourced from editor.isf.video - Fire Shader by codevinsky
- Description: Fire effect with fractal noise and color mixing
- License: Unknown (Shadertoy license)
- Features: Speed, size, and brightness controls for fire animation
*/

float rand(vec2 n) {
    return fract(cos(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

float noise(vec2 n) {
    const vec2 d = vec2(0.0, 1.0);
    vec2 b = floor(n), f = smoothstep(vec2(0.0), vec2(1.0), fract(n));
    return mix(mix(rand(b), rand(b + d.yx), f.x), mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
}

float fbm(vec2 n) {
    float total = 0.0, amplitude = 1.0;
    for (int i = 0; i < 4; i++) {
        total += noise(n) * amplitude;
        n += n;
        amplitude *= (0.5 * size);
    }
    return total;
}

void main() {
    const vec3 c1 = vec3(0.5, 0.0, 0.1);
    const vec3 c2 = vec3(0.9, 0.0, 0.0);
    const vec3 c3 = vec3(0.2, 0.0, 0.0);
    const vec3 c4 = vec3(1.0, 0.9, 0.0);
    const vec3 c5 = vec3(0.1);
    const vec3 c6 = vec3(0.9);
    
    vec2 speed = vec2(0.7, 0.4) * speedN;
    float shift = 1.6;
    float alpha = 1.0;
    vec2 p = gl_FragCoord.xy * 8.0 / RENDERSIZE.xx;
    float q = fbm(p - TIME * 0.1);
    vec2 r = vec2(fbm(p + q + TIME * speed.x - p.x - p.y), fbm(p + q - TIME * speed.y) * brightness);
    vec3 c = mix(c1, c2, fbm(p + r)) + mix(c3, c4, r.x) - mix(c5, c6, r.y);
    gl_FragColor = vec4(c * cos(shift * gl_FragCoord.y / RENDERSIZE.y), alpha);
}

/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: Unknown, GLSL Sandbox)",
	"DESCRIPTION": "Creates an animated sketch effect with parameterized controls. Features multiple value parameters and function selection for customizable animated patterns.",
	"INPUTS": [
		{"NAME": "v1", "LABEL": "v1", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0.5},
		{"NAME": "v2", "LABEL": "v2", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0.5},
		{"NAME": "v3", "LABEL": "v3", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0.5},
		{"NAME": "v4", "LABEL": "v4", "TYPE": "float", "MIN": 0, "MAX": 0.628318, "DEFAULT": 0.628318},
		{"NAME": "v5", "LABEL": "v5", "TYPE": "float", "MIN": 0.3, "MAX": 0.9, "DEFAULT": 0.6},
		{"NAME": "v6", "LABEL": "v6", "TYPE": "float", "MIN": 0.005, "MAX": 0.05, "DEFAULT": 0.005},
		{"NAME": "v7", "TYPE": "long", "VALUES": [2, 1, 3], "LABELS": ["cos", "sin", "abs"], "DEFAULT": 1}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Unknown
- Source: Originally sourced from editor.isf.video - Untitled Sketch by Unknown
- Original Source: http://glslsandbox.com/e#36696.0
- Description: Animated sketch with parameterized controls
- License: Unknown (GLSL Sandbox)
- Features: Animated sketch, parameterized controls
*/

void main(void) {
	vec2 p = (gl_FragCoord.xy * 2.0 - RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);
	float ratio = (RENDERSIZE.x / 2.) / (RENDERSIZE.y);
	vec3 destColor = vec3(v5);
	float f = 0.1;
	
	for (float i = 0.0; i < 10.0; i++) {
		float s = sin(i * v4) * v2 * sin(TIME);
		float c = cos(i * v4) * v3 * sin(TIME);
		f += v6 / abs(length(p + vec2(c, s)) - v1);
	}
	
	gl_FragColor = vec4(destColor * f, 1.0);
}
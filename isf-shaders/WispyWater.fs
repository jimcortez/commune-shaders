/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: David Hoskins, ISF import by Old Salt)",
	"DESCRIPTION": "Creates a wispy background effect with tiled patterns. Features color, rotation, continuous rotation, density, and intensity controls for dynamic wispy water-like animations.",
	"INPUTS": [
		{"NAME": "uC1", "TYPE": "color", "DEFAULT": [0.0,1.0,0.0,1.0]},
		{"LABEL": "Rotation(or R Speed):", "NAME": "uRotate", "TYPE": "float", "MAX": 180.0, "MIN": -180.0, "DEFAULT": 0.0},
		{"LABEL": "Continuous Rotation? ", "NAME": "uContRot", "TYPE": "bool", "DEFAULT": 1},
		{"LABEL": "Density: ", "NAME": "uDensity", "TYPE": "float", "MAX": 20.0, "MIN": 1, "DEFAULT": 4.0},
		{"LABEL": "Intensity: ", "NAME": "uIntensity", "TYPE": "float", "MAX": 4.0, "MIN": 0, "DEFAULT": 1.0}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: David Hoskins
- ISF Import by: Old Salt
- Source: Originally sourced from editor.isf.video - WispyWater by David Hoskins
- Original Source: http://glslsandbox.com/e#73288.0
- Description: Wispy background with tiled patterns
- License: Unknown (GLSL Sandbox)
- Features: Wispy background, tiled, color and density controls
*/

#define PI 3.141592653589
#define TAU 6.28318530718
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

void main()
	{
  vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;

	uv += 0.5;
	uv = mod(uv*TAU, TAU)-250.0;
	float t = TIME * .5+23.0;
	vec2 i = uv;
	float c = 1.0;
	float inten = .005;
	for (int n = 1; n < 21; n++) 
		{
		float t = t * (1.0 - (3.5 / float(n+1)));
		i = uv + vec2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
		c += 1.0/length(vec2(uv.x / (sin(i.x+t)/inten),uv.y / (cos(i.y+t)/inten)));
		if (float(n) >= uDensity) break;
		}
	c /= uDensity;
	c = 1.17-pow(c, 1.4);
	vec3 colour = vec3(pow(abs(c), 8.0));
	colour = clamp(colour + vec3(0.0, 0.35, 0.5), 0.0, 1.0);

	vec3 cOut = colour;
	cOut = cOut * uIntensity;
	gl_FragColor = vec4(cOut, 1.);
}

/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: mojovideotech)",
	"DESCRIPTION": "Creates an animated spiral OpArt worm effect. Features customizable center, size, colors, edge sharpness, gamma, amplitude, frequency, phase, and rate controls for dynamic spiral animations.",
	"INPUTS": [
		{
			"NAME": "center",
			"TYPE": "point2D",
			"DEFAULT": [1, 1],
			"MAX": [2.5, 2.5],
			"MIN": [-0.5, -0.5]
		},
		{
			"NAME": "size",
			"TYPE": "float",
			"DEFAULT": 8.95,
			"MIN": 1.0,
			"MAX": 10.0
		},
		{
			"NAME": "color1",
			"TYPE": "color",
			"DEFAULT": [0.7, 0.1, 1.0, 1]
		},
		{
			"NAME": "color2",
			"TYPE": "color",
			"DEFAULT": [0.2, 0.8, 0.7, 1]
		},
		{
			"NAME": "edge",
			"TYPE": "float",
			"DEFAULT": 23,
			"MIN": 3,
			"MAX": 30
		},
		{
			"NAME": "gamma",
			"TYPE": "float",
			"DEFAULT": 0.9,
			"MIN": 0.5,
			"MAX": 3.0
		},
		{
			"NAME": "amp1",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.01,
			"MAX": 0.25
		},
		{
			"NAME": "amp2",
			"TYPE": "float",
			"DEFAULT": 0.05,
			"MIN": 0.04,
			"MAX": 0.25
		},
		{
			"NAME": "freq1",
			"TYPE": "float",
			"DEFAULT": -0.5,
			"MIN": -10,
			"MAX": 10
		},
		{
			"NAME": "freq2",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": -10,
			"MAX": 10
		},
		{
			"NAME": "phase1",
			"TYPE": "float",
			"DEFAULT": 0.15,
			"MIN": -2.0,
			"MAX": 2.0
		},
		{
			"NAME": "phase2",
			"TYPE": "float",
			"DEFAULT": -1.05,
			"MIN": -2.0,
			"MAX": 2.0
		},
		{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": -5.0,
			"MAX": 5.0
		}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: mojovideotech
- Source: Originally sourced from editor.isf.video - Hypno Worm by mojovideotech
- Description: Animated spiral OpArt worm
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0
- Features: Spiral animation, OpArt, color and shape controls
*/


void main() {
    vec3 fc = vec3(color1.rbg);
	vec2 uv = 2.0*(gl_FragCoord.xy / RENDERSIZE.xy) - center.xy;
    uv *= 11.0-size;
    uv.x *= RENDERSIZE.x/ RENDERSIZE.y;
    vec2 pos = center.xy/ RENDERSIZE.xy;
    float T = TIME * rate;
    for(float i = 50.0; i > 0.; --i) {
        pos += vec2(sin(T+(freq1 - i)*phase2)*amp1, cos(T+(freq2 - i)*phase1)*amp2);
    	float dist = length( pos - uv) - 0.1 * i;
    	float o = 1. - smoothstep(-0.01,edge/RENDERSIZE.x, dist);
        fc = mix(fc, vec3(1.)* (1. -mod(i,2.)),o);
    }
    vec3 cc = vec3(color1.gbr*color2.rgb);
	vec4 col = vec4(pow(color2.rgb,fc.rgb)-cc,1.0);
	col = pow(col, vec4(1.0/gamma));
	gl_FragColor = clamp(col,0.0,1.2);
	
}

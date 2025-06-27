/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: mojovideotech, based on Twisting Bars by @hintz)",
	"DESCRIPTION": "Creates twisting colored bars with phase and loop controls. Features scale, rate, loops, phase, rotation toggle, and sparse mode for dynamic bar animations.",
	"INPUTS": [
		{"NAME": "scale", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.25, "MAX": 5.0},
		{"NAME": "rate", "TYPE": "float", "DEFAULT": 0.5, "MIN": -2.0, "MAX": 2.0},
		{"NAME": "loops", "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 16.0},
		{"NAME": "phase", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.001, "MAX": 0.999},
		{"NAME": "rot", "TYPE": "bool", "DEFAULT": false},
		{"NAME": "sparse", "TYPE": "bool", "DEFAULT": true}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: mojovideotech
- Based on: Twisting Bars by @hintz (glslsandbox.com/e#42684.4)
- Source: Originally sourced from editor.isf.video - Twisty Coloured Bars by mojovideotech
- Description: Twisting colored bars with phase and loop controls
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0
- Features: Twisting colored bars, phase and loop controls
*/


void main()
{
	vec2 o = ((gl_FragCoord.xy - RENDERSIZE.xy/2.0)*scale)/RENDERSIZE.y;
	if (rot) o.xy = o.yx; 
	float T = TIME * rate, p = 0.5+floor(5.0*o.x), q;
	if (sparse) q = 0.4; 
		else q = 0.2; 
	o.x = mod(o.x, q) - 0.1;
	o.y+=p;
	vec4 s = 0.1*cos(1.6*vec4(0,1,2,3)+p*phase*T+sin(o.y*loops+p*loops+cos(T))),
	e = s.yzwx, 
	f = min(o.x-s,e-o.x);
	gl_FragColor = dot(clamp(-1.0+f*RENDERSIZE.y,0.0,1.0),28.0*(s-e))*(s-0.22)+f*0.5;
}
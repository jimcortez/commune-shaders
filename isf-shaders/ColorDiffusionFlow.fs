/*{
    "CATEGORIES": [],
    "CREDIT": "Jim Cortez - Commune Project (Original: mojovideotech)",
    "DESCRIPTION": "Creates fluid-like color diffusion patterns with flowing, organic movements. Features dual-rate animation systems that generate complex, layered color flows with customizable depth controls, producing mesmerizing liquid-like visual effects that evolve and morph over time.",
    "INPUTS": [
        {
            "DEFAULT": 1.9,
            "MAX": 3.0,
            "MIN": -3.0,
            "NAME": "rate1",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.6,
            "MAX": 3.0,
            "MIN": -3.0,
            "NAME": "rate2",
            "TYPE": "float"
        },
        {
            "DEFAULT": 85.0,
            "MAX": 100.0,
            "MIN": 20.0,
            "NAME": "loopcycle",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.45,
            "MAX": 2.5,
            "MIN": -2.5,
            "NAME": "color1",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1.0,
            "MAX": 1.125,
            "MIN": -1.25,
            "NAME": "color2",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1.33,
            "MAX": 3.1459,
            "MIN": 0.01,
            "NAME": "cycle1",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.22,
            "MAX": 0.497,
            "MIN": -0.497,
            "NAME": "cycle2",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.095,
            "MAX": 0.01,
            "MIN": 0.001,
            "NAME": "nudge",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.85,
            "MAX": 0.9,
            "MIN": 0.001,
            "NAME": "depthX",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.25,
            "MAX": 0.9,
            "MIN": 0.001,
            "NAME": "depthY",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: mojovideotech
- Original Shader: glslsandbox.com/e#35553.0
- Source: Originally sourced from editor.isf.video - ColorDiffusionFlow by mojovideotech
- Description: Fluid-like color diffusion with flowing patterns and depth controls
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0
- Features: Dual rate controls, loop cycles, color modulation, and depth parameters
*/

#ifdef GL_ES
precision mediump float;
#endif

#define pi 3.141592653589793 // pi

void main() {
    float T = TIME * rate1;
    float TT = TIME * rate2;
    vec2 p=(2.*isf_FragNormCoord);
    for(int i=1;i<11;i++) {
        vec2 newp=p;
        float ii = float(i);  
        newp.x+=depthX/ii*sin(ii*pi*p.y+T*nudge+cos((TT/(5.0*ii))*ii));
        newp.y+=depthY/ii*cos(ii*pi*p.x+TT+nudge+sin((T/(5.0*ii))*ii));
        p=newp+log(DATE.w)/loopcycle;
    }
    vec3 col=vec3(cos(p.x+p.y+3.0*color1)*0.5+0.5,sin(p.x+p.y+6.0*cycle1)*0.5+0.5,(sin(p.x+p.y+9.0*color2)+cos(p.x+p.y+12.0*cycle2))*0.25+.5);
    gl_FragColor=vec4(col*col, 1.0);
} 
/*{
    "CATEGORIES": [],
    "CREDIT": "Jim Cortez - Commune Project (Original: Joseph Fiola)",
    "DESCRIPTION": "Creates an animated particle system with dynamic rotation and position controls. Features customizable dot patterns that move in sin/cos wave patterns, with independent canvas and particle rotation systems that create mesmerizing geometric animations.",
    "INPUTS": [
        {
            "DEFAULT": 0.01,
            "MAX": 0.1,
            "MIN": 0,
            "NAME": "dotSize",
            "TYPE": "float"
        },
        {
            "DEFAULT": 100,
            "MAX": 100,
            "MIN": 0,
            "NAME": "iteration",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.3,
            "MAX": 1,
            "MIN": -1,
            "NAME": "xAmp",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.1,
            "MAX": 1,
            "MIN": -1,
            "NAME": "yAmp",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.2,
            "MAX": 10,
            "MIN": 0,
            "NAME": "xFactor",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.2,
            "MAX": 10,
            "MIN": 0,
            "NAME": "yFactor",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.05,
            "MAX": 0.1,
            "MIN": 0,
            "NAME": "speed",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0,
            "MAX": 3.141592653589793,
            "MIN": -3.141592653589793,
            "NAME": "rotateCanvas",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "MAX": 1.5707963267948966,
            "MIN": -1.5707963267948966,
            "NAME": "rotateParticles",
            "TYPE": "float"
        },
        {
            "DEFAULT": 10,
            "MAX": 10,
            "MIN": 0.01,
            "NAME": "rotateMultiplier",
            "TYPE": "float"
        },
        {
            "DEFAULT": [0.5, 0.5],
            "MAX": [1, 1],
            "MIN": [0, 0],
            "NAME": "pos",
            "TYPE": "point2D"
        }
    ],
    "ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Joseph Fiola (http://www.joefiola.com)
- Original Shader: SaturdayShader Week 21: Cosplay
- Date: 2016-01-09
- Source: Originally sourced from editor.isf.video - Cosplay by Joseph Fiola
- Description: Animated particle system with rotation and position controls
- License: Unknown (SaturdayShader license)
- Features: Dot size, iteration count, amplitude controls, rotation, and position parameters
*/

// rotation
vec2 rot(vec2 uv, float a) {
    return vec2(
        uv.x * cos(a) - uv.y * sin(a),
        uv.y * cos(a) + uv.x * sin(a)
    );
}

float circle(vec2 uv, float size){
    return  length(uv) > size?0.0:1.0;
}

void main(){
    vec2 uv = gl_FragCoord.xy/RENDERSIZE;
    uv -= vec2(pos);                        
    uv.x *= RENDERSIZE.x/RENDERSIZE.y;
    
    vec3 color = vec3(0);
    
    //rotate canvas
    uv=rot(uv,rotateCanvas);
    
    float i = 0.0;    
    
    for (int _i = 0; _i<100; _i++) { // for loop fix on Intel - and possible others -  by Imimot @imimothq
        
        i = float(_i);
        
        // set max number of iterations
        if (iteration < i) break;
        
        // sin() cos() animation
        vec2 st = uv - vec2(cos(i * xFactor * (TIME*speed)) * xAmp, sin(i * yFactor * (TIME*speed)) * yAmp);
        
        // set dotSize
        float dots = circle((st), dotSize * (i  * 0.01));
        
        //rotate particles
        uv=rot(uv,rotateParticles*rotateMultiplier);

        color += dots;
    }

    gl_FragColor = vec4(vec3(color),1.0);
} 
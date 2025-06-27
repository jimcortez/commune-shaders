/*{
    "CATEGORIES": [],
    "CREDIT": "Jim Cortez - Commune Project (Original: ISF Import by Old Salt)",
    "DESCRIPTION": "Creates a mesmerizing aurora borealis effect with flowing, ethereal light patterns that dance across the screen. Features multiple layers of sinuous light bands that move and morph organically, simulating the natural phenomenon of the northern lights with customizable colors and movement controls.",
    "INPUTS": [
        {
            "DEFAULT": [0.0, 1.0, 0.0, 1.0],
            "NAME": "uC1",
            "TYPE": "color"
        },
        {
            "DEFAULT": [0.0, 0.0, 1.0, 1.0],
            "NAME": "uC2",
            "TYPE": "color"
        },
        {
            "DEFAULT": [1.0, 0.0, 0.0, 1.0],
            "NAME": "uC3",
            "TYPE": "color"
        },
        {
            "DEFAULT": [0.0, 0.0],
            "LABEL": "Offset: ",
            "MAX": [1.0, 1.0],
            "MIN": [-1.0, -1.0],
            "NAME": "uOffset",
            "TYPE": "point2D"
        },
        {
            "DEFAULT": 1.0,
            "LABEL": "Zoom: ",
            "MAX": 10.0,
            "MIN": 1.0,
            "NAME": "uZoom",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.0,
            "LABEL": "Rotation(or R Speed):",
            "MAX": 180.0,
            "MIN": -180.0,
            "NAME": "uRotate",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "LABEL": "Continuous Rotation? ",
            "NAME": "uContRot",
            "TYPE": "bool"
        },
        {
            "DEFAULT": 0,
            "LABEL": "Color Mode: ",
            "LABELS": [
                "Shader Defaults ",
                "Alternate Color Palette (3 used) "
            ],
            "NAME": "uColMode",
            "TYPE": "long",
            "VALUES": [0, 1]
        },
        {
            "DEFAULT": 1.0,
            "LABEL": "Intensity: ",
            "MAX": 4.0,
            "MIN": 0,
            "NAME": "uIntensity",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: ISF Import by Old Salt
- Original Shader: http://www.glslsandbox.com/e#58544.0
- Source: Originally sourced from editor.isf.video - Aurora by ISF Import by Old Salt
- Description: Aurora borealis effect with flowing, ethereal light patterns
- License: GLSL Sandbox license
- Features: Color customization, rotation controls, zoom and offset functions
*/

#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

void main()
{
    vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize coordinates
    uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
    uv = (uv-uOffset) * 3.0/uZoom;              // offset and zoom functions
    uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0); // rotation

    vec2 p = uv;
    float d = 2.*length(p);
    vec3 col = vec3(0); 
    for (int i = 0; i < 18; i++)
    {
        float dist = abs(p.y + sin(float(i)+TIME*0.3+3.0*p.x)) - 0.2;
        if (dist < 1.0) { col += (1.0-pow(abs(dist), 0.28))*vec3(0.8+0.2*sin(TIME),0.9+0.1*sin(TIME*1.1),1.2); }
        p *= 0.99/d; 
        p *= rotate2D(PI/60.0);  // was: p = rotz(p, 30.0) 
    }
    col *= 0.49 ; 

    vec4 cShad = vec4( col-d-0.4, 1.0);  
    vec3 cOut = cShad.rgb;
    if (uColMode == 1)
    {
        cOut = uC1.rgb * cShad.r;
        cOut += uC2.rgb * cShad.g;
        cOut += uC3.rgb * cShad.b;
    }
    cOut = cOut * uIntensity;
    cOut = clamp(cOut, vec3(0.0), vec3(1.0));
    gl_FragColor = vec4(cOut.rgb,cShad.a);
}

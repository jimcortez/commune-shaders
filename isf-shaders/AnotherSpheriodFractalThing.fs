/*{
    "CATEGORIES": ["Fractal", "3D", "Generative"],
    "CREDIT": "Jim Cortez - Commune Project (Original: mojovideotech)",
    "DESCRIPTION": "Generates complex spherical fractal patterns with intricate mathematical transformations. Creates mesmerizing 3D fractal structures that morph and evolve over time, featuring multiple rotation axes and iterative mathematical operations that produce organic, flowing geometric forms.",
    "INPUTS": [
        {
            "DEFAULT": 0.23,
            "MAX": 1.0,
            "MIN": 0.05,
            "NAME": "range",
            "TYPE": "float"
        },
        {
            "DEFAULT": -1.07,
            "MAX": 3.0,
            "MIN": -3.0,
            "NAME": "cycle",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1.86,
            "MAX": 5.0,
            "MIN": -5.0,
            "NAME": "rate",
            "TYPE": "float"
        },
        {
            "DEFAULT": -0.6,
            "MAX": 1.0,
            "MIN": -1.0,
            "NAME": "CX",
            "TYPE": "float"
        },
        {
            "DEFAULT": -0.8,
            "MAX": 1.0,
            "MIN": -1.0,
            "NAME": "CY",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.1,
            "MAX": 1.0,
            "MIN": -1.0,
            "NAME": "CZ",
            "TYPE": "float"
        },
        {
            "DEFAULT": -3.14159,
            "LABEL": "X rotate",
            "MAX": 6.2831853,
            "MIN": -6.2831853,
            "NAME": "RX",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1.5707963,
            "LABEL": "Y rotate",
            "MAX": 6.2831853,
            "MIN": -6.2831853,
            "NAME": "RY",
            "TYPE": "float"
        },
        {
            "DEFAULT": -1.5707963,
            "LABEL": "Z rotate",
            "MAX": 6.2831853,
            "MIN": -6.2831853,
            "NAME": "RZ",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.76,
            "MAX": 1.0,
            "MIN": 0.0,
            "NAME": "morph",
            "TYPE": "float"
        },
        {
            "DEFAULT": 20.0,
            "MAX": 100.0,
            "MIN": 1.0,
            "NAME": "iterations",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: mojovideotech
- Original Shader: glslsandbox.com/e#41853.0
- Source: Originally sourced from editor.isf.video - AnotherSpheriodFractalThing by mojovideotech
- Description: Spherical fractal generator with complex mathematical transformations
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0
- Features: 3D rotation controls, morphing parameters, and fractal iteration system
*/

#define pi 3.141592653589793   // pi
#define twpi 6.283185307179586 // two pi, 2*pi

// ISF uniforms
// (Removed explicit uniform declarations for ISF inputs)

float t = TIME * rate;

vec2 B(vec2 a) {
    return vec2(log(length(a)), atan(a.y, a.x) - twpi);
}

vec3 rot(vec3 vec, vec3 axis, float ang) {
    vec3 N = vec3(dot(vec2(cos(ang), dot(axis, vec)), vec2(sin(ang), dot(axis, vec))));
    vec3 M = cross(axis, vec) * vec3(sin(ang));
    return vec * mix(N, M, morph);
}

vec3 spin(vec3 v) {
    for (int i = 1; i < 5; i++) {
        vec3 q = (vec3(sin(v.x), sin(v.y + CY / pi), sin(v.z + CZ / pi)) * 0.5 + 0.5).zxy;
        v = (3.145 * rot((v), q, float(i * i)) * float(i) + q);
        v = (vec3(sin(v.x + CX / pi), sin(v.y + CY / pi), sin(v.z)) * 0.5 + 0.5).yzx;
    }
    return (v.xyz);
}

float safe_log2(float x) {
    return log2(max(x, 1e-6));
}

vec3 F(vec2 E, float G, float iterationsF) {
    int iterations = int(iterationsF);
    vec2 e_ = E;
    float c = 0.;
    for (int i = 0; i < 100; i++) {
        if (i >= iterations) break;
        e_ = B(vec2(e_.x, abs(e_.y))) + vec2(.1 * sin(t / 3.) - .1, 5. + .1 * -cos(t / 5.));
        c += length(e_);
    }
    float d = safe_log2(safe_log2(c * .25)) * 9.;
    vec3 color = clamp(vec3(.5 + .75 * cos(d), .3 + .67 * cos(d - TIME * cycle), .1 + .95 * cos(G)), 0.0, 1.0);
    return color;
}

void main(void) {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float th = uv.t * pi, ph = uv.s * twpi;
    vec3 p = vec3(sin(th) * cos(ph), sin(th) * sin(ph), cos(th));
    
    float sax = sin(RX);
    float cax = cos(RX);
    float say = sin(RY);
    float cay = cos(RY);
    float saz = sin(RZ);
    float caz = cos(RZ);
    
    vec3 H = vec3(
        (cay * p.x + say * p.z) + (caz * p.x - saz * p.y),
        (cax * p.y - sax * p.z) + (saz * p.x + caz * p.y),
        (sax * p.y + cax * p.z) + (-say * p.x + cay * p.z)
    );
    
    gl_FragColor = vec4(spin(range * F(H.zx, H.y, iterations)), 1.);
} 
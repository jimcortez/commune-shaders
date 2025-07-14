/*{
    "CATEGORIES": [],
    "CREDIT": "Jim Cortez - Commune Project (Original: mojovideotech)",
    "DESCRIPTION": "Creates an immersive cosmic journey through fractal field patterns with dynamic star effects. Features multiple layered fractal fields that evolve and morph over time, creating a mesmerizing space travel experience with organic color transitions and atmospheric depth effects.",
    "INPUTS": [],
    "ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: mojovideotech
- Original Shader: https://www.shadertoy.com/view/lslGWr
- Stars addition: Thanks to http://glsl.heroku.com/e#6904.0
- Source: Originally sourced from editor.isf.video - CosmicJourney by mojovideotech
- Description: Cosmic journey with fractal field patterns and star effects
- License: Unknown (based on Shadertoy license)
- Features: Fractal field generation, multiple color layers, star effects
*/

#ifdef GL_ES
precision mediump float;
#endif

// http://www.fractalforums.com/new-theories-and-research/very-simple-formula-for-fractal-patterns/
float field(in vec3 p) {
    float strength = 7. + .03 * log(1.e-6 + fract(sin(TIME) * 4373.11));
    float accum = 0.;
    float prev = 0.;
    float tw = 0.;
    for (int i = 0; i < 32; ++i) {
        float mag = dot(p, p);
        p = abs(p) / mag + vec3(-.51, -.4, -1.3);
        float w = exp(-float(i) / 7.);
        accum += w * exp(-strength * pow(abs(mag - prev), 2.3));
        tw += w;
        prev = mag;
    }
    return max(0., 5. * accum / tw - .7);
}

vec3 nrand3( vec2 co )
{
    vec3 a = fract( cos( co.x*8.3e-3 + co.y )*vec3(1.3e5, 4.7e5, 2.9e5) );
    vec3 b = fract( sin( co.x*0.3e-3 + co.y )*vec3(8.1e5, 1.0e5, 0.1e5) );
    vec3 c = mix(a, b, 0.5);
    return c;
}

void main() {
    vec2 uv = 1.0 * gl_FragCoord.xy / RENDERSIZE.xy - 1.0;
    vec2 uvs = uv * RENDERSIZE.xy / max(RENDERSIZE.x, RENDERSIZE.y);
    
    vec3 p = vec3(uvs / 4., 0) + vec3(2., -1.3, -1.);
    p += 0.15 * vec3(sin(TIME / 16.), sin(TIME / 12.),  sin(TIME / 128.));
    
    vec3 p2 = vec3(uvs / (4.+sin(TIME*0.11)*0.2+0.2+sin(TIME*0.15)*0.3+0.4), 1.5) + vec3(2., -1.3, -1.);
    p2 += 0.15 * vec3(sin(TIME / 16.), sin(TIME / 12.),  sin(TIME / 128.));

    vec3 p3 = vec3(uvs / (4.+sin(TIME*0.14)*0.23+0.23+sin(TIME*0.19)*0.31+0.31), 0.5) + vec3(2., -1.3, -1.);
    p3 += 0.15 * vec3(sin(TIME / 16.), sin(TIME / 12.),  sin(TIME / 128.));
    
    float t = field(p);
    float t2 = field(p2);
    float t3 = field(p3);

    float v = (1. - exp((abs(uv.x) - 1.) * 6.)) * (1. - exp((abs(uv.y) - 1.) * 6.));
    
    vec4 c1 = mix(.4, 1., v) * vec4(1.8 * t * t * t, 1.4 * t * t, t, 1.0);
    vec4 c2 = mix(.4, 1., v) * vec4(1.4 * t2 * t2 * t2, 1.8 * t2 * t2, t2, 1.0);
    vec4 c3 = mix(.4, 1., v) * vec4(1.4 * t3 * t3 * t3, 1.8 * t3 * t3, t3, 1.0);
    c1.b *= mod(gl_FragCoord.y+1.0, 2.0)*1.4;
    c2.r *= mod(gl_FragCoord.y, 2.0)*3.4;
    c3.g *= mod(gl_FragCoord.y, 2.0)*2.4;
    gl_FragColor = c1*0.7 + c2*0.5 + c3*0.3;
} 
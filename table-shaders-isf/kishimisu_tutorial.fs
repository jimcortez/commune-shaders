/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: Unknown, inspired by IQ palettes)",
	"DESCRIPTION": "Creates a Kishimisu hexagram tutorial effect with palette coloring. Features hexagram SDF, IQ palette system, speed, twist, bloom, and point controls for educational spiral animations.",
	"INPUTS": [
		{"NAME": "speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": -1.0, "MAX": 1.0},
		{"NAME": "twist", "TYPE": "float", "DEFAULT": 0.1, "MIN": -1.0, "MAX": 1.0},
		{"NAME": "bloom", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.25, "MAX": 2.0},
		{"NAME": "pointInput", "TYPE": "point2D", "DEFAULT": [0, 0]}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Unknown (inspired by IQ palettes)
- Source: Originally sourced from editor.isf.video - kishimisu_tutorial by Unknown
- Features: Hexagram SDF, palette coloring, tutorial structure
- License: Unknown
*/

//https://iquilezles.org/articles/palettes/
vec3 palette( float t ) {
//    vec3 a = vec3(0.5, 0.5, 0.5);
//    vec3 b = vec3(0.5, 0.5, 0.5);
//    vec3 c = vec3(1.0, 1.0, 1.0);
//    vec3 d = vec3(0.263,0.416,0.557);

    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 0.6);
    vec3 d = vec3(0.80, 0.90, 0.30);

    //vec3 d = vec3(0.3, 0.2, 0.2);
    //vec3 d = vec3(0.3, 0.2, 0.2);

    //vec3 a = vec3(0.668, 0.488, 0.670);
    //vec3 b = vec3(0.198, 0.248, 0.248);
    //vec3 c = vec3(2.018, 1.000, 1.000);
    //vec3 d = vec3(0.018, 0.333, 0.667);

    return a + b*cos( 6.28318*(c*t+d) );
}

float sdHexagram( in vec2 p, in float r )
{
    const vec4 k = vec4(-0.5,0.8660254038,0.5773502692,1.7320508076);
    p = abs(p);
    p -= 2.0*min(dot(k.xy,p),0.0)*k.xy;
    p -= 2.0*min(dot(k.yx,p),0.0)*k.yx;
    p -= vec2(clamp(p.x,r*k.z,r*k.w),r);
    return length(p)*sign(p.y);
}

// Translated from https://www.shadertoy.com/view/mtyGWy
// added swirling rotation
void main( ) {
//    vec2 uv = (gl_FragCoord * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
//    vec2 uv = (isf_FragNormCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
//    vec2 uv = isf_FragNormCoord.xy - vec2(0.5,0.5);
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    uv *= 0.5;
    uv += pointInput;

    // make it spiral, from https://www.shadertoy.com/view/Xsl3zX
    float rho = length(uv);
	float ang = atan(uv.y,uv.x) + TIME*speed;
	ang -= twist * (rho * 2.);
    uv = rho*vec2(cos(ang),sin(ang));

    vec2 uv0 = uv;
    vec3 finalColor = vec3(0.0);
    
    for (float i = 0.0; i < 3.0; i++) {
        uv = fract(uv * 1.5) - 0.5;

        //float d = length(uv) * exp(-length(uv0));
        float d = sdHexagram(uv,length(uv0)) * exp(-length(uv0));

        vec3 col = palette(length(uv0) + i*.4 + TIME*.4);

        d = sin(d*8. + TIME)/8.;
        d = abs(d);

        d = pow(0.005 / d, bloom);

        finalColor += col * d;
    }
        
    gl_FragColor = vec4(finalColor, 1.0);
}
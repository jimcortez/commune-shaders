/*{
    "CREDIT": "Adapted by Shader Shaper; Original: inigo quilez - iq, Shadertoy; Commune Project (Jim Cortez); see table-shaders-isf/OctoRays.fs",
    "DESCRIPTION": "OctoRays shader adapted for MadMapper Materials with all original ISF parameters, generator-driven animation, and improved compatibility.",
    "CATEGORIES": ["generator", "raymarch", "prism", "opart"],
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_speed",    "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "mat_rotation", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 360.0, "LABEL": "Octagon Rotation (degrees)" }
    ],
    "GENERATORS": [
        { "NAME": "mat_genTime", "TYPE": "time_base", "PARAMS": { "speed": "mat_speed" } }
    ]
}*/
/*
ORIGINAL SHADER INFORMATION:
- Original Author: inigo quilez - iq (https://www.shadertoy.com/view/ltjXWd)
- Tweaked copy of: https://www.shadertoy.com/view/Xds3zN
- Adapted by: Jim Cortez - Commune Project
- Source: Originally sourced from editor.isf.video - HEX_Rays by inigo quilez
- Description: Octagonal prism raymarching with hue shift
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License
- Features: Speed control, raymarching, hue shifting
*/

vec3 hue(vec3 color, float shift) {
    const vec3  kRGBToYPrime = vec3 (0.299, 0.587, 0.114);
    const vec3  kRGBToI     = vec3 (0.596, -0.275, -0.321);
    const vec3  kRGBToQ     = vec3 (0.212, -0.523, 0.311);
    const vec3  kYIQToR   = vec3 (1.0, 0.956, 0.621);
    const vec3  kYIQToG   = vec3 (1.0, -0.272, -0.647);
    const vec3  kYIQToB   = vec3 (1.0, -1.107, 1.704);
    float   YPrime  = dot (color, kRGBToYPrime);
    float   I      = dot (color, kRGBToI);
    float   Q      = dot (color, kRGBToQ);
    float   hue     = atan (Q, I);
    float   chroma  = sqrt (I * I + Q * Q);
    hue += shift;
    Q = chroma * sin (hue);
    I = chroma * cos (hue);
    vec3    yIQ   = vec3 (YPrime, I, Q);
    color.r = dot (yIQ, kYIQToR);
    color.g = dot (yIQ, kYIQToG);
    color.b = dot (yIQ, kYIQToB);
    return color;
}

float sdNgon(vec2 p, float r, int n) {
    float an = 3.1415926 / float(n);
    float bn = mod(atan(p.y, p.x) + 3.1415926, 2.0 * an) - an;
    return length(p) * cos(bn) - r;
}

float sdOctPrism(vec3 p, vec2 h) {
    float d = sdNgon(p.xy, h.x, 8);
    float dz = abs(p.z) - h.y;
    return max(d, dz);
}

float opS(float d1, float d2) {
    return max(-d1, d2);
}

vec2 opU(vec2 d1, vec2 d2) {
    return (d1.x < d2.x) ? d1 : d2;
}

vec2 rotate2D(vec2 p, float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return vec2(c * p.x - s * p.y, s * p.x + c * p.y);
}

vec2 map(in vec3 pos, float iTime, float rotation) {
    float height = .42;
    float depth = .75;
    float t = 0.02 + sin(iTime) * 0.01;
    pos.z = mod(pos.z, depth * 2.0) - 0.5 * depth * 2.0;
    float rotRad = radians(rotation);
    vec3 posRot = pos;
    posRot.xy = rotate2D(pos.xy, rotRad);
    float cyl = sdOctPrism(posRot, vec2(height - t, depth + t));
    float scyl = sdOctPrism(posRot, vec2(height - t * 2.0, depth + t + 0.001));
    vec2 res = vec2(opS(scyl, cyl), 1.5);
    vec2 final = res;
    for (int i = 1; i < 3; i++) {
        height -= 0.1;
        depth -= 0.19;
        cyl = sdOctPrism(posRot, vec2(height - t, depth + t));
        scyl = sdOctPrism(posRot, vec2(height - t * 2.0, depth + t + 0.001));
        final = opU(final, vec2(opS(scyl, cyl), 2.5));
    }
    return final;
}

vec2 castRay(in vec3 ro, in vec3 rd, float iTime, float rotation) {
    float tmin = 0.0;
    float tmax = 100.0;
    float t = tmin;
    float m = -1.0;
    for (int i = 0; i < 100; i++) {
        vec2 res = map(ro + rd * t, iTime, rotation);
        if (t > tmax) break;
        t += res.x;
        m = res.y;
    }
    if (t > tmax) m = -1.0;
    return vec2(t, m);
}

vec3 calcNormal(in vec3 pos, float iTime, float rotation) {
    vec3 eps = vec3(0.01, 0.0, 0.0);
    vec3 nor = vec3(
        map(pos + eps.xyy, iTime, rotation).x - map(pos - eps.xyy, iTime, rotation).x,
        map(pos + eps.yxy, iTime, rotation).x - map(pos - eps.yxy, iTime, rotation).x,
        map(pos + eps.yyx, iTime, rotation).x - map(pos - eps.yyx, iTime, rotation).x);
    return normalize(nor);
}

float calcAO(in vec3 pos, in vec3 nor, float iTime, float rotation) {
    float occ = 0.0;
    float sca = 1.0;
    for (int i = 0; i < 5; i++) {
        float hr = 0.01 + 0.12 * float(i) / 4.0;
        vec3 aopos = nor * hr + pos;
        float dd = map(aopos, iTime, rotation).x;
        occ += -(dd - hr) * sca;
        sca *= .95;
    }
    return clamp(1.0 - 3.0 * occ, 0.0, 1.0);
}

vec3 render(in vec3 ro, in vec3 rd, float iTime, float rotation) {
    vec3 col = vec3(1.0);
    vec2 res = castRay(ro, rd, iTime, rotation);
    float t = res.x;
    float m = res.y;
    if (m > -0.5) {
        vec3 pos = ro + t * rd;
        vec3 nor = calcNormal(pos, iTime, rotation);
        float occ = calcAO(pos, nor, iTime, rotation);
        col = 1.0 - hue(vec3(0.0, 1.0, 1.0), iTime * 0.02 + pos.z) * occ;
    }
    return clamp(col, 0.0, 1.0);
}

mat3 setCamera(in vec3 ro, in vec3 ta, float cr) {
    vec3 cw = normalize(ta - ro);
    vec3 cp = vec3(sin(cr), cos(cr), 0.0);
    vec3 cu = normalize(cross(cw, cp));
    vec3 cv = normalize(cross(cu, cw));
    return mat3(cu, cv, cw);
}

vec4 materialColorForPixel(vec2 texCoord) {
    // ISF-style coordinate normalization
    vec2 q = texCoord;
    vec2 p = (q * RENDERSIZE.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
    float iTime = mat_genTime;
    float rotation = mat_rotation;
    vec3 ro = vec3(0., 0., iTime);
    vec3 ta = ro + vec3(0., 0., 1.);
    mat3 ca = setCamera(ro, ta, 3.14159 / 2.0);
    vec3 rd = ca * normalize(vec3(p.xy, 4.5));
    vec3 col = render(ro, rd, iTime, rotation);
    return vec4(col, 1.0);
} 
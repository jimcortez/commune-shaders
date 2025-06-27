/*{
	"CREDIT": "Adapted by Shader Shaper",
	"DESCRIPTION": "Hexagonal 3D tunnel with customizable speed, size, and color.",
	"CATEGORIES": ["generator", "abstract"],
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "hueShift",
			"TYPE": "float",
			"DEFAULT": 0.02,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "hexSize",
			"TYPE": "float",
			"DEFAULT": 0.42,
			"MIN": 0.1,
			"MAX": 1.0
		},
		{
			"NAME": "depthFactor",
			"TYPE": "float",
			"DEFAULT": 0.75,
			"MIN": 0.1,
			"MAX": 2.0
		},
		{
			"NAME": "maxRaySteps",
			"TYPE": "float",
			"DEFAULT": 100.0,
			"MIN": 10.0,
			"MAX": 100.0
		}
	]
}*/

#define MAX_ITER 100  // Fixed iteration count for WebGL 1.0 compatibility

float iTime = TIME * speed;

// Hue shift function
vec3 hue(vec3 color, float shift) {
    const vec3  kRGBToYPrime = vec3(0.299, 0.587, 0.114);
    const vec3  kRGBToI      = vec3(0.596, -0.275, -0.321);
    const vec3  kRGBToQ      = vec3(0.212, -0.523, 0.311);

    const vec3  kYIQToR      = vec3(1.0, 0.956, 0.621);
    const vec3  kYIQToG      = vec3(1.0, -0.272, -0.647);
    const vec3  kYIQToB      = vec3(1.0, -1.107, 1.704);

    float YPrime = dot(color, kRGBToYPrime);
    float I = dot(color, kRGBToI);
    float Q = dot(color, kRGBToQ);

    float chroma = sqrt(I * I + Q * Q);
    float newHue = atan(Q, I) + shift;

    Q = chroma * sin(newHue);
    I = chroma * cos(newHue);

    vec3 yIQ = vec3(YPrime, I, Q);
    color.r = dot(yIQ, kYIQToR);
    color.g = dot(yIQ, kYIQToG);
    color.b = dot(yIQ, kYIQToB);

    return color;
}

// Hexagonal prism SDF
float sdHexPrism(vec3 p, vec2 h) {
    vec3 q = abs(p);
    return max(q.z - h.y, max(q.x * 0.866025 + q.y * 0.5, q.y) - h.x);
}

// Smooth boolean subtraction
float opS(float d1, float d2) {
    return max(-d1, d2);
}

// Union operation for SDFs
vec2 opU(vec2 d1, vec2 d2) {
    return (d1.x < d2.x) ? d1 : d2;
}

// Distance field function
vec2 map(vec3 pos) {
    float t = 0.02 + sin(iTime) * 0.01;
    pos.z = mod(pos.z, depthFactor * 2.0) - 0.5 * depthFactor * 2.0;

    float cyl = sdHexPrism(pos, vec2(hexSize - t, depthFactor + t));
    float scyl = sdHexPrism(pos, vec2(hexSize - t * 2.0, depthFactor + t + 0.001));

    vec2 res = vec2(opS(scyl, cyl), 1.5);
    vec2 final = res;
    
    float hexSize2 = hexSize;
    float depthFactor2 = depthFactor;

    for (int i = 1; i < 3; i++) {
        hexSize2 -= 0.1;
        depthFactor2 -= 0.19;
        cyl = sdHexPrism(pos, vec2(hexSize2 - t, depthFactor2 + t));
        scyl = sdHexPrism(pos, vec2(hexSize2 - t * 2.0, depthFactor2 + t + 0.001));

        final = opU(final, vec2(opS(scyl, cyl), 2.5));
    }

    return final;
}

// Ray marching function
vec2 castRay(vec3 ro, vec3 rd) {
    float tmin = 0.0;
    float tmax = 100.0;
    float t = tmin;
    float m = -1.0;

    for (int i = 0; i < MAX_ITER; i++) {
        if (float(i) >= maxRaySteps) break;
        vec2 res = map(ro + rd * t);
        if (t > tmax) break;
        t += res.x;
        m = res.y;
    }

    if (t > tmax) m = -1.0;
    return vec2(t, m);
}

// Normal calculation
vec3 calcNormal(vec3 pos) {
    vec3 eps = vec3(0.01, 0.0, 0.0);
    return normalize(vec3(
        map(pos + eps.xyy).x - map(pos - eps.xyy).x,
        map(pos + eps.yxy).x - map(pos - eps.yxy).x,
        map(pos + eps.yyx).x - map(pos - eps.yyx).x
    ));
}

// Ambient occlusion calculation
float calcAO(vec3 pos, vec3 nor) {
    float occ = 0.0;
    float sca = 1.0;

    for (int i = 0; i < 5; i++) {
        float hr = 0.01 + 0.12 * float(i) / 4.0;
        vec3 aopos = nor * hr + pos;
        float dd = map(aopos).x;
        occ += -(dd - hr) * sca;
        sca *= 0.95;
    }

    return clamp(1.0 - 3.0 * occ, 0.0, 1.0);
}

// Scene rendering function
vec3 render(vec3 ro, vec3 rd) {
    vec3 col = vec3(1.0);
    vec2 res = castRay(ro, rd);
    float t = res.x;
    float m = res.y;

    if (m > -0.5) {
        vec3 pos = ro + t * rd;
        vec3 nor = calcNormal(pos);
        float occ = calcAO(pos, nor);
        col = 1.0 - hue(vec3(0.0, 1.0, 1.0), hueShift + iTime * 0.02 + pos.z) * occ;
    }

    return clamp(col, 0.0, 1.0);
}

// Main function
void main() {
    vec2 q = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = -1.0 + 2.0 * q;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;

    vec3 ro = vec3(0., 0., iTime);
    vec3 ta = ro + vec3(0., 0., 1.);
    vec3 rd = normalize(vec3(p.xy, 4.5));

    vec3 col = render(ro, rd);
    gl_FragColor = vec4(col, 1.0);
}

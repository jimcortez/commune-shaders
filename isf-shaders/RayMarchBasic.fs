/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: gyabo, Shadertoy)",
	"DESCRIPTION": "Creates a basic raymarching effect with animated terrain. Features speed control for dynamic 3D scene exploration with complex geometric patterns.",
	"INPUTS": [
		{"NAME": "speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": -1.0, "MAX": 1.0}
	],
	"ISFVSN": "2",
	"PASSES": [
		{"TARGET": "bufferVariableNameA", "WIDTH": "$WIDTH/16.0", "HEIGHT": "$HEIGHT/16.0"},
		{"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"}
	]
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: gyabo
- Source: Originally sourced from editor.isf.video - RayMarchBasic by gyabo
- Original Source: https://www.shadertoy.com/view/Ml2XRD
- Description: Basic raymarching with animated terrain
- License: Unknown (Shadertoy)
- Features: Raymarching, animated terrain, speed control
*/

float map(vec3 p) {
	vec3 n = vec3(0, 1, 0);
	float k1 = 1.9;
	float k2 = (sin(p.x * k1) + sin(p.z * k1)) * 0.8;
	float k3 = (sin(p.y * k1) + sin(p.z * k1)) * 0.8;
	float w1 = 4.0 - dot(abs(p), normalize(n)) + k2;
	float w2 = 4.0 - dot(abs(p), normalize(n.yzx)) + k3;
	float s1 = length(mod(p.xy + vec2(sin((p.z + p.x) * 2.0) * 0.3, cos((p.z + p.x) * 1.0) * 0.5), 2.0) - 1.0) - 0.2;
	float s2 = length(mod(0.5+p.yz + vec2(sin((p.z + p.x) * 2.0) * 0.3, cos((p.z + p.x) * 1.0) * 0.3), 2.0) - 1.0) - 0.2;
	return min(w1, min(w2, min(s1, s2)));
}

vec2 rot(vec2 p, float a) {
	return vec2(
		p.x * cos(a) - p.y * sin(a),
		p.x * sin(a) + p.y * cos(a));
}

void main() {
    float time = TIME * speed;
	//vec2 uv = ( fragCoord.xy / iResolution.xy ) * 2.0 - 1.0;
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    uv *= 0.4;
	//uv.x *= iResolution.x /  iResolution.y;
	vec3 dir = normalize(vec3(uv, 1.0));
	dir.xz = rot(dir.xz, time * 0.23);dir = dir.yzx;
	dir.xz = rot(dir.xz, time * 0.2);dir = dir.yzx;
	vec3 pos = vec3(0, 0, time);
	vec3 col = vec3(0.0);
	float t = 0.0;
    float tt = 0.0;
	for(int i = 0 ; i < 100; i++) {
		tt = map(pos + dir * t);
		if(tt < 0.001) break;
		t += tt * 0.45;
	}
	vec3 ip = pos + dir * t;
	col = vec3(t * 0.1);
	col = sqrt(col);
	gl_FragColor = vec4(0.05*t+abs(dir) * col + max(0.0, map(ip - 0.1) - tt), 1.0); //Thanks! Shane!
    //gl_FragColor.a = 1.0 / (t * t * t * t);
}


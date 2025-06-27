/*{
    "CATEGORIES": [],
    "CREDIT": "Jim Cortez - Commune Project (Original: Unknown, Heart SDF)",
    "DESCRIPTION": "Creates a signed distance function heart shape with animated color. Features a procedural heart rendered using SDF math and animated with time-based color modulation.",
    "INPUTS": [],
    "ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Unknown (Heart SDF)
- Source: Originally sourced from editor.isf.video - first_shader by Unknown
- Features: Signed distance function heart, animated color
- License: Unknown
*/

float dot2(in vec2 u) {
	return dot(u.xy, u.xy);
}

float sdHeart(in vec2 p) {
	p.x = abs(p.x);

	if (p.y + p.x > 1.0) {
		return sqrt(dot2(p - vec2(0.25, 0.75))) - sqrt(2.0) / 4.0;
	}
	return sqrt(min(dot2(p - vec2(0.00, 1.00)),
					dot2(p - 0.5 * max(p.x + p.y, 0.0)))) * sign(p.x - p.y);
}

void main() {
	// vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	// vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy * 2.0 - 1.0;
	vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
	// vec2 uv = isf_FragNormCoord.xy / RENDERSIZE.xy;
	float d = sdHeart(uv);
	vec3 col = vec3(1.0, 2.0, 3.0);
	
	d = sin(d * 8.0 + TIME) / 8.0;
	d = abs(d);
	// d = smoothstep(0.0,0.1,d);
	
	d = 0.02 / d;
	
	col *= d;
	
	// gl_FragColor = vec4(d,d,d,1.0);
	
	gl_FragColor = vec4(col, 1.0); 
}
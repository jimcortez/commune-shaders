/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project",
	"DESCRIPTION": "Creates animated colorful bow patterns with flowing, ribbon-like strands that dance across the screen. Features multiple colored strands with different animation speeds, curves, and glowing effects that create a mesmerizing, organic movement pattern reminiscent of flowing ribbons or streamers.",
	"INPUTS": [],
	"ISFVSN": "2",
	"PASSES": [
		{
			"TARGET": "bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Unknown (no original credit provided)
- Source: Originally sourced from editor.isf.video - Bows by Unknown
- Description: Animated colorful bow patterns with flowing strands
- License: Unknown
- Features: Multiple colored strands with different animation speeds and curves
*/

#define iTime (TIME / 8.0)
#define iResolution RENDERSIZE

vec3 Strand(in vec2 fragCoord, in vec3 color, in float hoffset, in float hscale, in float vscale, in float timescale)
{
    float glow = 0.002 * iResolution.y;
    float twopi = 6.28318530718;
    float curve = 1.0 - abs(fragCoord.y - (sin(mod(fragCoord.x * hscale / 100.0 / iResolution.x * 1000.0 + iTime * timescale + hoffset, twopi)) * iResolution.y * 0.25 * vscale + iResolution.y / 2.0));
    float i = clamp(curve, 0.0, 1.0);
    i += clamp((glow + curve) / glow, 0.0, 1.0) * 0.4;
    return i * color;
}

void main()
{
    float timescale = 2.0;
	vec3 c = vec3(0, 0, 0);
    c += Strand(gl_FragCoord.xy, vec3(1.0, 0, 0), 0.7934 + 1.0 + sin(iTime) * 30.0, 1.0, 0.16, 10.0 * timescale);
    c += Strand(gl_FragCoord.xy, vec3(0.0, 1.0, 0.0), 0.645 + 1.0 + sin(iTime) * 30.0, 1.5, 0.2, 10.3 * timescale);
    c += Strand(gl_FragCoord.xy, vec3(0.0, 0.0, 1.0), 0.735 + 1.0 + sin(iTime) * 30.0, 1.3, 0.19, 8.0 * timescale);
    c += Strand(gl_FragCoord.xy, vec3(1.0, 1.0, 0.0), 0.9245 + 1.0 + sin(iTime) * 30.0, 1.6, 0.14, 12.0 * timescale);
    c += Strand(gl_FragCoord.xy, vec3(0.0, 1.0, 1.0), 0.7234 + 1.0 + sin(iTime) * 30.0, 1.9, 0.23, 14.0 * timescale);
    c += Strand(gl_FragCoord.xy, vec3(1.0, 0.0, 1.0), 0.84525 + 1.0 + sin(iTime) * 30.0, 1.2, 0.18, 9.0 * timescale);
	gl_FragColor = vec4(c.r, c.g, c.b, 1.0);
}


/*{
	"DESCRIPTION": "Animated strands with customizable speed, scale, color, line count, and thickness.",
	"CREDIT": "Adapted by Shader Shaper",
	"ISFVSN": "2",
	"CATEGORIES": ["generator", "abstract"],
	"INPUTS": [
		{
			"NAME": "timeScale",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.1,
			"MAX": 10.0
		},
		{
			"NAME": "horizontalScale",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": 0.5,
			"MAX": 3.0
		},
		{
			"NAME": "verticalScale",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": 0.05,
			"MAX": 0.5
		},
		{
			"NAME": "motionOffset",
			"TYPE": "float",
			"DEFAULT": 30.0,
			"MIN": 0.0,
			"MAX": 100.0
		},
		{
			"NAME": "glowIntensity",
			"TYPE": "float",
			"DEFAULT": 0.002,
			"MIN": 0.001,
			"MAX": 0.01
		},
		{
			"NAME": "lineCount",
			"TYPE": "float",
			"DEFAULT": 6.0,
			"MIN": 1.0,
			"MAX": 12.0
		},
		{
			"NAME": "lineThickness",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 3.0
		},
		{
			"NAME": "colorVariation",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 2.0
		}
	]
}*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718
#define MAX_LINES 12  // Fixed iteration count for WebGL 1.0 compatibility
#define iTime (TIME / 8.0)

// Function to generate strand patterns
vec3 Strand(vec2 fragCoord, vec3 color, float hoffset, float hscale, float vscale, float timescale) {
    float glow = glowIntensity * RENDERSIZE.y;
    
    float curve = 1.0 - abs(fragCoord.y - (sin(mod(fragCoord.x * hscale / 100.0 / RENDERSIZE.x * 1000.0 
                 + iTime * timescale + hoffset, TWO_PI)) * RENDERSIZE.y * 0.25 * vscale + RENDERSIZE.y / 2.0));
    
    float intensity = clamp(curve * lineThickness, 0.0, 1.0);
    intensity += clamp((glow + curve) / glow, 0.0, 1.0) * 0.4;
    
    return intensity * color;
}

void main() {
    vec3 colorOutput = vec3(0.0);
    float animatedOffset = 1.0 + sin(iTime) * motionOffset;

    // Fixed loop with early exit based on lineCount
    for (int i = 0; i < MAX_LINES; i++) {
        if (float(i) >= lineCount) break;  // Stop drawing extra lines
        
        float offsetFactor = (float(i) / lineCount) * colorVariation;
        vec3 strandColor = vec3(sin(float(i) * 0.5) * 0.5 + 0.5, 
                                cos(float(i) * 0.7) * 0.5 + 0.5, 
                                sin(float(i) * 0.3) * 0.5 + 0.5);

        colorOutput += Strand(gl_FragCoord.xy, strandColor, animatedOffset + float(i) * 0.1, 
                              horizontalScale * (1.0 + offsetFactor), 
                              verticalScale * (1.0 - offsetFactor), 
                              (8.0 + float(i)) * timeScale);
    }

    gl_FragColor = vec4(colorOutput, 1.0);
}

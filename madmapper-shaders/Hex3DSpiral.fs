/*{
	"CREDIT": "Adapted by Shader Shaper",
	"DESCRIPTION": "Hex 3D Spiral with adjustable zoom, rotation, twist, and color blending.",
	"CATEGORIES": ["generator", "abstract"],
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "offsetX",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "offsetY",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.1,
			"MAX": 10.0
		},
		{
			"NAME": "rotationSpeed",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -180.0,
			"MAX": 180.0
		},
		{
			"NAME": "twistAmount",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "outlineThickness",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "maxIterations",
			"TYPE": "float",
			"DEFAULT": 128.0,
			"MIN": 32.0,
			"MAX": 128.0
		},
		{
			"NAME": "intensity",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 4.0
		}
	]
}*/

#define PI 3.141592653589
#define MAX_ITER 128  // Fixed iteration count for WebGL 1.0 compatibility

// 2D rotation function
#define rotate2D(a) mat2(cos(a), -sin(a), sin(a), cos(a))

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE - 0.5;  // Normalize coordinates
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;           // Maintain aspect ratio
    uv -= vec2(offsetX, offsetY);                 // Apply offset
    uv *= 1.0 / zoom;                             // Apply zoom
    uv *= rotate2D(TIME * rotationSpeed / 36.0);  // Apply rotation

/**** Start of Core Imported Shader Code *****/
    vec2 p;
    float c = 0.0;
    float twist = twistAmount * 4.0;
    float outthick = 1.0 - (0.78 * outlineThickness + 0.1);
    
    float i, g, d = 1.0;
    for (int j = 0; j < MAX_ITER; j++) {
        if (float(j) >= maxIterations) break;  // Stop when reaching dynamic limit
        
        i++;
        if (d <= 0.001) break;
        
        p = uv * g + vec2(0.3) * rotate2D(g * twist);
        g += d = -(length(p) - 2.0 + g / 9.0) / 2.0;
    }

    p = vec2(atan(p.x, p.y), g) * 8.28 + TIME * 2.0;
    p = abs(fract(p + vec2(0.0, 0.5 * ceil(p.x))) - 0.5);
    
    c += 30.0 / i - 0.5 / smoothstep(outthick, 0.9, 1.0 - abs(max(p.x * 1.5 + p.y, p.y * 2.0) - 1.0));
/****  End of Core Imported Shader Code  *****/

    vec3 cOut = vec3(c / 4.0);
    cOut = clamp(cOut * 4.0 * intensity, 0.0, 1.0);
    
    gl_FragColor = vec4(cOut, 1.0);
}

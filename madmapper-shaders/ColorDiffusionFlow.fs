/*{
  "CREDIT": "Adapted by Shader Shaper from mojovideotech",
  "DESCRIPTION": "Fluid diffusion with color control and smooth animation.",
  "CATEGORIES": ["fluid", "liquid"],
  "ISFVSN": "2",
  "INPUTS": [
  	{
		"NAME": "flowSpeed1",
		"TYPE": "float",
		"DEFAULT": 1.9,
		"MIN": -3.0,
		"MAX": 3.0
	},
	{
		"NAME": "flowSpeed2",
		"TYPE": "float",
		"DEFAULT": 0.6,
		"MIN": -3.0,
		"MAX": 3.0
	},
	{
		"NAME": "loopStrength",
		"TYPE": "float",
		"DEFAULT": 85.0,
		"MIN": 20.0,
		"MAX": 100.0
	},
	{
		"NAME": "primaryColorFactor",
		"TYPE": "float",
		"DEFAULT": 0.45,
		"MIN": -2.5,
		"MAX": 2.5
	},
	{
		"NAME": "secondaryColorFactor",
		"TYPE": "float",
		"DEFAULT": 1.0,
		"MIN": -1.25,
		"MAX": 1.125
	},
	{
		"NAME": "waveCycle1",
		"TYPE": "float",
		"DEFAULT": 1.33,
		"MIN": 0.01,
		"MAX": 3.1459
	},
	{
		"NAME": "waveCycle2",
		"TYPE": "float",
		"DEFAULT": 0.22,
		"MIN": -0.497,
		"MAX": 0.497
	},
	{
		"NAME": "flowIntensity",
		"TYPE": "float",
		"DEFAULT": 0.095,
		"MIN": 0.001,
		"MAX": 0.1
	}
  ]
}*/

#define PI 3.141592653589793
#define MAX_ITER 10  // Fixed iteration count for WebGL 1.0 compatibility

void main() {
    float time1 = TIME * flowSpeed1;
    float time2 = TIME * flowSpeed2;
    vec2 uv = 2.0 * isf_FragNormCoord;
    
    for (int i = 1; i <= MAX_ITER; i++) {
        if (float(i) > loopStrength) break;  // Dynamic limit based on user input

        vec2 newUV = uv;
        float fi = float(i);
        
        newUV.x += (0.85 / fi) * sin(fi * PI * uv.y + time1 * flowIntensity + cos((time2 / (5.0 * fi)) * fi));
        newUV.y += (0.25 / fi) * cos(fi * PI * uv.x + time2 + flowIntensity + sin((time1 / (5.0 * fi)) * fi));
        
        uv = newUV + TIME / loopStrength;
    }

    vec3 colorOutput = vec3(
        cos(uv.x + uv.y + 3.0 * primaryColorFactor) * 0.5 + 0.5,
        sin(uv.x + uv.y + 6.0 * waveCycle1) * 0.5 + 0.5,
        (sin(uv.x + uv.y + 9.0 * secondaryColorFactor) + cos(uv.x + uv.y + 12.0 * waveCycle2)) * 0.25 + 0.5
    );

    gl_FragColor = vec4(colorOutput * colorOutput, 1.0);
}

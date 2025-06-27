/*{
	"DESCRIPTION": "Aurora shader adapted for ISF with fully float-based inputs and continuous animation control.",
	"CATEGORIES": ["generator"],
	"ISFVSN": "2",
	"CREDIT": "Adapted by Shader Shaper from GLSL Sandbox",
	"VSN": "1.2",
	"INPUTS": [
		{
			"NAME": "color1",
			"TYPE": "color",
			"DEFAULT": [0.0, 1.0, 0.0, 1.0]
		},
		{
			"NAME": "color2",
			"TYPE": "color",
			"DEFAULT": [0.0, 0.0, 1.0, 1.0]
		},
		{
			"NAME": "color3",
			"TYPE": "color",
			"DEFAULT": [1.0, 0.0, 0.0, 1.0]
		},
		{
			"NAME": "offsetX",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "offsetY",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "zoom",
			"TYPE": "float",
			"MIN": 1.0,
			"MAX": 10.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "rotationSpeed",
			"TYPE": "float",
			"MIN": -180.0,
			"MAX": 180.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "rotationOffset",
			"TYPE": "float",
			"MIN": -180.0,
			"MAX": 180.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "colorMode",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 4.0,
			"DEFAULT": 1.0
		}
	]
}*/

#define PI 3.141592653589
#define MAX_ITER 18  // Fixed iteration count for WebGL 1.0 compatibility

// 2D rotation function
mat2 rotate2D(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat2(c, -s, s, c);
}

void main()
{
    // Normalize coordinates
    vec2 uv = gl_FragCoord.xy / RENDERSIZE - 0.5;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;  // Aspect ratio correction

    // Apply offset, zoom, and rotation without resetting animation state
    uv -= vec2(offsetX, offsetY);
    uv *= 3.0 / zoom;

    // Continuous rotation accumulation
    float timeRotation = rotationSpeed * TIME / 36.0; 
    uv *= rotate2D(timeRotation + rotationOffset * PI / 180.0);

    /**** Start of Core Imported Shader Code *****/
    vec2 p = uv;
    float d = 2.0 * length(p);
    vec3 col = vec3(0.0);

    for (int i = 0; i < MAX_ITER; i++) {
        float dist = abs(p.y + sin(float(i) + TIME * 0.3 + 3.0 * p.x)) - 0.2;
        if (dist < 1.0) {
            col += (1.0 - pow(abs(dist), 0.28)) * vec3(0.8 + 0.2 * sin(TIME),
                                                       0.9 + 0.1 * sin(TIME * 1.1),
                                                       1.2);
        }
        p *= 0.99 / d;
        p *= rotate2D(PI / 60.0); // Apply fixed rotation step
    }

    col *= 0.49;
    /**** End of Core Imported Shader Code *****/

    // Apply color modifications based on user input
    vec4 cShad = vec4(col - d - 0.4, 1.0);
    vec3 cOut = cShad.rgb;

    if (colorMode >= 0.5) {
        cOut = color1.rgb * cShad.r;
        cOut += color2.rgb * cShad.g;
        cOut += color3.rgb * cShad.b;
    }

    // Apply brightness and clamp values
    cOut *= brightness;
    cOut = clamp(cOut, vec3(0.0), vec3(1.0));

    gl_FragColor = vec4(cOut.rgb, cShad.a);
}

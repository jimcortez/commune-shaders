/*{
    "CREDIT": "Adapted by Shader Shaper; Original: mojovideotech; Commune Project (Jim Cortez); see table-shaders-isf/PrimeWaves.fs",
    "DESCRIPTION": "PrimeWaves shader adapted for MadMapper Materials with all original ISF parameters, generator-driven animation, and improved compatibility. Creates prime waves effect using prime number-based noise patterns.",
    "CATEGORIES": ["generator", "noise", "waves"],
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_center_x",   "TYPE": "float",   "DEFAULT": -2.0,         "MIN": -10.0,         "MAX": 10.0 },
        { "NAME": "mat_center_y",   "TYPE": "float",   "DEFAULT": -1.0,         "MIN": -10.0,         "MAX": 10.0 },
        { "NAME": "mat_rate",       "TYPE": "float",   "DEFAULT": -1.0,         "MIN": -3.0,          "MAX": 3.0 },
        { "NAME": "mat_zoom",       "TYPE": "float",   "DEFAULT": 5.0,          "MIN": -10.0,         "MAX": 10.0 },
        { "NAME": "mat_depth",      "TYPE": "float",   "DEFAULT": 0.6,          "MIN": 0.0,           "MAX": 1.0 },
        { "NAME": "mat_prime_x",    "TYPE": "float",   "DEFAULT": 11.0,         "MIN": 1.0,           "MAX": 17.0 },
        { "NAME": "mat_prime_z",    "TYPE": "float",   "DEFAULT": 13.0,         "MIN": 1.0,           "MAX": 17.0 },
        { "NAME": "mat_warp_strength", "TYPE": "float", "DEFAULT": 0.5,         "MIN": 0.0,           "MAX": 2.0 },
        { "NAME": "mat_color_shift", "TYPE": "float",  "DEFAULT": 0.0,          "MIN": 0.0,           "MAX": 6.28 }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_genTime",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_rate" }
        }
    ]
}*/
/*
ORIGINAL SHADER INFORMATION:
- Original Author: mojovideotech
- Based on: glslsandbox.com/e#21344.0
- Source: Originally sourced from editor.isf.video - PrimeWaves by mojovideotech
- Description: Prime waves using prime number-based noise
- License: Unknown (GLSL Sandbox license)
- Features: Prime number noise, wave patterns, mathematical distortion
*/

vec2 distort(vec2 p, float depth)
{
    float theta  = atan(p.y, p.x);
    float radius = length(p);
    radius = pow(radius, 1.0 + depth);
    p.x = radius * cos(theta);
    p.y = radius * sin(theta);
    return 0.5 * (p + 1.0);
}
	
vec4 pattern(vec2 p)
{
	vec2 m = mod(p.xy + p.x + p.y, 2.0) - 1.0;
	return vec4(length(m + p * 0.1));
}

float hash(const float n)
{
	return fract(sin(n) * 29712.15073);
}

float noise(const vec3 x, float y, float z)
{
	vec3 p = floor(x); 
	vec3 f = fract(x);
	f = f * f * (3.0 - 2.0 * f);
	float n = p.x + p.y * y + p.z * z;
	float r1 = mix(mix(hash(n + 0.0), hash(n + 1.0), f.x), mix(hash(n + y), hash(n + y + 1.0), f.x), f.y);
    float r2 = mix(mix(hash(n + z), hash(n + z + 1.0), f.x), mix(hash(n + y + z), hash(n + y + z + 1.0), f.x), f.y);
	return mix(r1, r2, f.z);
}

float getPrimeNumber(float index)
{
	if (index <= 1.0)			return 11.0;
	else if (index <= 2.0)		return 13.0;
	else if (index <= 3.0)		return 17.0;
	else if (index <= 4.0)		return 19.0;
	else if (index <= 5.0)		return 23.0;
	else if (index <= 6.0)		return 29.0;
	else if (index <= 8.0)		return 31.0;
	else if (index <= 9.0)		return 37.0;
	else if (index <= 10.0)		return 41.0;
	else if (index <= 11.0)		return 43.0;
	else if (index <= 12.0)		return 47.0;
	else if (index <= 13.0)		return 53.0;
	else if (index <= 14.0)		return 59.0;
	else if (index <= 15.0)		return 61.0;
	else if (index <= 16.0)		return 67.0;
	else						return 71.0;
}

vec4 materialColorForPixel(vec2 texCoord) {
	// Convert normalized texture coordinate to fragment coordinate space
	vec2 fragCoord = texCoord * RENDERSIZE;
	
	// Get prime numbers for noise function
	float RY = getPrimeNumber(mat_prime_x);
	float RZ = getPrimeNumber(mat_prime_z);
	
	// Calculate position with zoom and center offset
	vec2 pos = (fragCoord / RENDERSIZE * mat_zoom) + vec2(mat_center_x, mat_center_y);
	
	// Use generator-driven time for animation
	float col = noise(pos.xyx + (mat_genTime * mat_rate), RY, RZ);
	
	// Apply distortion with warp strength
	vec2 distortedPos = pos + col * mat_warp_strength;
	vec4 c = pattern(distort(distortedPos, mat_depth));
	c.xy = distort(c.xy, mat_depth);
	
	// Generate color with color shift
	vec3 color = vec3(
		c.x - col + sin(mat_color_shift) * 0.1,
		sin(c.y) - col + cos(mat_color_shift) * 0.1,
		cos(c.z) + sin(mat_color_shift * 0.5) * 0.1
	);
	
	// Clamp color to valid range
	color = clamp(color, 0.0, 1.0);
	
	return vec4(color, 1.0);
} 
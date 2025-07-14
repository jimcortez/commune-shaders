/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: Unknown, inspired by Shadertoy)",
	"DESCRIPTION": "Creates an animated snow effect with realistic falling snowflakes. Features speed and density controls for customizable snowfall patterns with gradient sky background.",
	"INPUTS": [
		{"NAME": "speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0},
		{"NAME": "density", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 12.0}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Unknown (inspired by Shadertoy)
- Source: Originally sourced from editor.isf.video - Snow by Unknown
- Original Source: https://www.shadertoy.com/view/Mdt3Df
- Description: Animated snow effect with realistic falling snowflakes
- License: Unknown (Shadertoy)
- Features: Animated snow, density and speed controls
*/

// Simple noise function
float noise(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

// Snowflake function
float snowflake(vec2 uv, float time, float seed) {
    // Create hexagonal grid
    vec2 hex = vec2(uv.x + uv.y * 0.5, uv.y * 0.866025);
    vec2 id = floor(hex);
    vec2 gv = fract(hex) - 0.5;
    
    // Add randomness based on seed
    float n = noise(id + seed);
    if (n < 0.3) return 0.0; // Only some cells have snowflakes
    
    // Create snowflake shape
    float d = length(gv);
    float angle = atan(gv.y, gv.x);
    float arms = 6.0;
    float arm = sin(angle * arms + time * 0.5) * 0.1;
    
    return smoothstep(0.3 + arm, 0.1 + arm, d);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = (uv - 0.5) * 2.0;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    float time = TIME * speed;
    
    // Create gradient sky background (always visible)
    vec3 sky = mix(vec3(0.4, 0.7, 1.0), vec3(0.1, 0.2, 0.4), uv.y);
    
    // Generate multiple layers of snowflakes
    float snow = 0.0;
    float snowDensity = max(density, 1.0);
    
    for (int i = 0; i < 8; i++) {
        float layer = float(i);
        vec2 offset = vec2(
            sin(time * 0.3 + layer * 1.5) * 0.2,
            fract(time * 0.2 + layer * 0.7) * 2.0 - 1.0
        );
        float flake = snowflake(p + offset, time + layer * 10.0, layer * 100.0);
        snow += flake * (1.0 - layer * 0.1) / snowDensity;
    }
    
    // Ensure snow is visible
    snow = clamp(snow * 2.0, 0.0, 1.0);
    
    // Mix snow (white) with sky background
    vec3 color = mix(sky, vec3(1.0), snow);
    
    // Ensure output is always visible and properly clamped
    gl_FragColor = vec4(clamp(color, 0.0, 1.0), 1.0);
}

/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: Panteleymonov Aleksandr Konstantinovich)",
	"DESCRIPTION": "Creates miracle snowflakes with iterative 3D snowflake generation. Features speed control for animated snowflake patterns with complex noise-based geometry and lighting effects.",
	"INPUTS": [
		{"NAME": "speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": -1.0, "MAX": 1.0}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Panteleymonov Aleksandr Konstantinovich (2015)
- Source: Originally sourced from editor.isf.video - Snowflakes by Panteleymonov Aleksandr Konstantinovich
- Original Source: https://www.shadertoy.com/view/Xsd3zf
- Description: Miracle snowflakes with iterative 3D generation
- License: Unknown (Shadertoy)
- Features: Iterative snowflake generation, noise, and lighting
*/

// Simplified noise function
float noise(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

float noise3D(vec3 p) {
    return fract(sin(dot(p, vec3(12.9898, 78.233, 45.164))) * 43758.5453);
}

// Simplified snowflake function
float snowflake(vec2 uv, float time) {
    float scale = 8.0;
    vec2 p = uv * scale;
    
    // Create hexagonal pattern
    vec2 hex = vec2(p.x + p.y * 0.5, p.y * 0.866025);
    vec2 id = floor(hex);
    vec2 gv = fract(hex) - 0.5;
    
    // Add some randomness
    float n = noise(id + time * 0.1);
    if (n < 0.4) return 0.0;
    
    // Create snowflake shape
    float d = length(gv);
    float angle = atan(gv.y, gv.x);
    float arms = 6.0;
    float arm = sin(angle * arms + time * 0.3) * 0.15;
    
    return smoothstep(0.25 + arm, 0.05 + arm, d);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = (uv - 0.5) * 2.0;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    float time = TIME * speed;
    
    // Create multiple layers of snowflakes
    float snow = 0.0;
    for (int i = 0; i < 6; i++) {
        float layer = float(i);
        vec2 offset = vec2(
            sin(time * 0.4 + layer * 1.2) * 0.15,
            fract(time * 0.25 + layer * 0.6) * 2.0 - 1.0
        );
        snow += snowflake(p + offset, time + layer * 8.0) * (1.0 - layer * 0.12);
    }
    
    // Add some background color
    vec3 bg = mix(vec3(0.05, 0.1, 0.3), vec3(0.7, 0.8, 0.9), uv.y);
    
    // Combine snow with background
    vec3 color = mix(bg, vec3(1.0), snow);
    
    gl_FragColor = vec4(clamp(color, 0.0, 1.0), 1.0);
}


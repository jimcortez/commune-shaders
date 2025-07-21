/*{
    "DESCRIPTION": "Spinning arrow lands in one of 8 slices (Wheel of Fortune)",
    "CATEGORIES": ["animation", "game", "demo"],
    "INPUTS": [
        {
            "NAME": "Seed",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        }
    ]
}*/
#ifdef GL_ES
precision mediump float;
#endif

// Helper: random based on seed
float hash(float n) {
    return fract(sin(n) * 43758.5453123);
}

// Helper: rotate a 2D vector
vec2 rotate(vec2 v, float a) {
    float s = sin(a);
    float c = cos(a);
    return vec2(c * v.x - s * v.y, s * v.x + c * v.y);
}

// Helper: get a seed based on the current minute of the day
float getDaySeed() {
    // DATE.w is seconds since midnight
    return floor(DATE.w / 60.0); // Seed changes every minute
}

void main() {
    // Normalized coordinates, centered
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Animation timing
    float t = clamp(TIME, 0.0, 10.0);
    float spinDuration = 10.0;
    float spins = 3.0;
    float slices = 8.0;

    // Use DATE-based seed for randomness
    float rand = hash(Seed * 123.456 + getDaySeed());
    float finalSlice = floor(rand * slices);
    float finalAngle = finalSlice * (6.2831853 / slices); // 2*PI / slices

    // Ease out timing (cubic ease out)
    float progress = t / spinDuration;
    float ease = 1.0 - pow(1.0 - progress, 3.0);

    // Arrow angle: spins, then slows to final
    float angle = (spins * 6.2831853) * (1.0 - ease) + finalAngle * ease;

    // Draw arrow shape (triangle)
    float arrowLength = 0.7;
    float arrowWidth = 0.18;
    float shaftWidth = 0.06;
    float shaftLength = 0.45;

    // Rotate coordinates by -angle
    vec2 p = rotate(uv, -angle);

    // Arrow head (triangle)
    float head = step(0.0, p.y) * step(p.y, arrowLength) * step(abs(p.x), mix(arrowWidth * (1.0 - p.y / arrowLength), 0.01, p.y / arrowLength));
    // Arrow shaft (rectangle)
    float shaft = step(-shaftWidth, p.x) * step(p.x, shaftWidth) * step(-shaftLength, p.y) * step(p.y, 0.0);

    float arrow = max(head, shaft);

    // Draw 8 slice lines for reference
    float sliceLine = 0.0;
    for (int i = 0; i < 8; i++) {
        float a = float(i) * (6.2831853 / 8.0);
        vec2 dir = vec2(sin(a), cos(a));
        float d = abs(dot(normalize(uv), dir));
        sliceLine += smoothstep(0.995, 1.0, d) * step(0.2, length(uv));
    }

    // Color
    vec3 bg = vec3(0.12, 0.13, 0.16);
    vec3 arrowColor = vec3(1.0, 0.2, 0.1);
    vec3 shaftColor = vec3(1.0, 0.8, 0.2);
    vec3 sliceColor = vec3(0.2, 0.5, 1.0);

    vec3 color = bg;
    color = mix(color, sliceColor, sliceLine * 0.5);
    color = mix(color, shaftColor, shaft * 0.9);
    color = mix(color, arrowColor, head * 0.95);

    // Clamp output
    color = clamp(color, 0.0, 1.0);
    gl_FragColor = vec4(color, 1.0);
} 
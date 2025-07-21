/*{
    "CREDIT": "Adapted by Shader Shaper; Original: mojovideotech; Commune Project (Jim Cortez); see table-shaders-isf/PsychedeliaGenerica.fs",
    "DESCRIPTION": "Psychedelic raymarching effect with hue-based coloring, adapted for MadMapper Materials with interactive camera control and dynamic color transformations.",
    "CATEGORIES": ["generator", "raymarching", "psychedelic"],
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "INPUTS": [
        { "NAME": "mat_camera_x",     "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "mat_camera_y",     "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "mat_camera_sensitivity", "TYPE": "float", "DEFAULT": 3.0,  "MIN": 0.1, "MAX": 10.0 },
        { "NAME": "mat_ground_height", "TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0, "MAX": 10.0 },
        { "NAME": "mat_ground_warp_x", "TYPE": "float", "DEFAULT": 0.2,  "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "mat_ground_warp_z", "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "mat_grid_size",    "TYPE": "float", "DEFAULT": 20.0, "MIN": 5.0, "MAX": 50.0 },
        { "NAME": "mat_grid_thickness", "TYPE": "float", "DEFAULT": 1.4, "MIN": 0.1, "MAX": 5.0 },
        { "NAME": "mat_ray_steps",    "TYPE": "float", "DEFAULT": 20.0, "MIN": 10.0, "MAX": 50.0 },
        { "NAME": "mat_ray_threshold", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.001, "MAX": 0.1 },
        { "NAME": "mat_hue_shift",    "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "mat_animation_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_genTime",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_animation_speed" }
        }
    ]
}*/
/*
ORIGINAL SHADER INFORMATION:
- Original Author: mojovideotech
- Based on: http://glslsandbox.com/e#25956.0
- Source: Originally sourced from editor.isf.video - PsychedeliaGenerica by mojovideotech
- Description: Psychedelic raymarching with hue-based coloring
- License: Unknown (GLSL Sandbox)
- Features: Psychedelic raymarching, hue-based coloring, mouse input
- Adapted for MadMapper Materials with enhanced parameterization and generator-driven animation
*/

// Convert hue to RGB color
vec3 hueToRGB(float hue) {
    return clamp( 
        abs(mod(hue * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 
        0.0, 1.0);
}

// Distance field function for raymarching
float map(vec3 p) {
    float t = 0.0;
    // Ground plane with warping
    t = max(-t, mat_ground_height - dot(p, vec3(0, 1, 0)) + (sin(p.x * mat_ground_warp_x) * sin(p.z * mat_ground_warp_z)));
    // Grid pattern
    t = min(t, length(mod(p.xz, mat_grid_size) - mat_grid_size * 0.5) - mat_grid_thickness);
    return t;
}

// 2D rotation function
vec2 rot(vec2 p, float t) {
    return vec2(
        cos(t) * p.x - sin(t) * p.y,
        sin(t) * p.x + cos(t) * p.y);
}

vec4 materialColorForPixel(vec2 texCoord) {
    // Convert normalized texture coordinate to ISF-style coordinate system
    vec2 uv = (texCoord * 2.0 - 1.0);
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv.y = -uv.y;
    
    // Create ray direction
    vec3 dir = normalize(vec3(uv, 1.0));
    
    // Apply camera rotation based on input parameters
    dir.yz = rot(dir.yz, (mat_camera_y - 0.5) * mat_camera_sensitivity);
    dir.xz = rot(dir.xz, (mat_camera_x - 0.5) * -mat_camera_sensitivity);
    
    // Camera position with generator-driven time
    vec3 pos = vec3(0.0, 0.0, mat_genTime);
    
    // Raymarching loop
    float t = 0.0;
    int maxSteps = int(mat_ray_steps);
    for(int i = 0; i < 50; i++) {
        if (i >= maxSteps) break;
        float temp = map(pos + dir * t);
        if(temp < mat_ray_threshold) break;
        t += temp;
    }
    
    // Calculate intersection point and apply hue-based coloring
    vec3 ip = pos + dir * t;
    vec3 color = hueToRGB(map(ip) + mat_hue_shift);
    
    // Ensure color is clamped and alpha is 1.0
    color = clamp(color, 0.0, 1.0);
    return vec4(color, 1.0);
} 
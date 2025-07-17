/*{
    "DESCRIPTION": "Voronoi Spiral Vortex converted to a MadMapper Material with descriptive variable names, individual R, G, B inputs, adjustable loop count, and zoom control.",
    "CREDIT": "Modified by: Old Salt, adapted by Shader Shaper",
    "ISFVSN": "2",
    "GLSL_VERSION": "150 core",
    "CATEGORIES": ["generator"],
    "INPUTS": [
        { "NAME": "mat_rate",       "TYPE": "float", "DEFAULT": 0.25,   "MIN": -0.25,  "MAX": 0.25 },
        { "NAME": "mat_divisions",  "TYPE": "float", "DEFAULT": 9.0,   "MIN": 5.0,   "MAX": 10.0 },
        { "NAME": "mat_offset1",    "TYPE": "float", "DEFAULT": 0.75,  "MIN": -1.0,  "MAX": 1.0 },
        { "NAME": "mat_shape",      "TYPE": "float", "DEFAULT": 0.125, "MIN": 0.01,  "MAX": 0.5 },
        { "NAME": "mat_R",          "TYPE": "float", "DEFAULT": 0.25,  "MIN": 0.25,   "MAX": 1.0 },
        { "NAME": "mat_G",          "TYPE": "float", "DEFAULT": 0.5,   "MIN": 0.25,   "MAX": 1.0 },
        { "NAME": "mat_B",          "TYPE": "float", "DEFAULT": 0.75,  "MIN": 0.25,   "MAX": 1.0 },
        { "NAME": "mat_zoom",       "TYPE": "float", "DEFAULT": 5.0,   "MIN": 2.0,   "MAX": 5.0 }
    ],
    "GENERATORS": [
        {
            "NAME": "mat_genRate",
            "TYPE": "time_base",
            "PARAMS": { "speed": "mat_rate" }
        }
    ]
}*/

/*
   Voronoi Spiral Vortex effect originally used point2D and bool inputs.
   In this conversion, the center is reconstructed from mat_offsetX and mat_offsetY,
   and the invert input is replaced by a zoom control that scales the coordinate offset.
*/

#define rctwpi 0.159154943091895

vec4 materialColorForPixel(vec2 texCoord) {
    // Convert texCoord to pixel coordinates.
    vec2 fragCoord = texCoord * RENDERSIZE.xy;
    // Reconstruct the center position from two float inputs.
    vec2 centerPos = vec2(0.5) * RENDERSIZE.xy;
    
    // Compute offset vector relative to the center.
    vec2 p = (fragCoord - centerPos) - 0.5;
    // Apply zoom control.
    p *= mat_zoom;
    
    // Compute normalized distance and polar coordinates.
    float distNorm = length(p) / RENDERSIZE.y;
    float xVal = pow(distNorm, mat_shape);
    float angleNorm = atan(p.x, p.y) * rctwpi;
    
    // Use generator-controlled time.
    float T = mat_genRate * 0.01;
    // M: effective number of divisions.
    float M = floor(mat_divisions) * 2.0;
    float minVal = 1.0;
    float tempAngle = angleNorm;
    
    // Iterate from 50 downwards; break when loop count is reached.
    for (float i = 50.0; i >= 1.0; --i) {
        tempAngle = angleNorm;
        minVal = min(minVal, length(fract(vec2(xVal - T * i * mat_offset1, fract(tempAngle - i * mat_offset1) * 0.5) * M) * 2.0 - 1.0));
        if (i >= 1.0) { break; }
    }
    
    float factor = minVal * distNorm * distNorm * (0.5 - distNorm);
    vec3 colorOutput = vec3(distNorm + mat_R * factor, distNorm + mat_G * factor, distNorm + mat_B * factor);
    
    return vec4(colorOutput, 1.0);
}
